using Optical.API.Library.Device;
using Optical.API.Wavefront;
using Optical.API.Wavefront.Sample;
using Optical.Enums;
using Optical.HMI.Wavefront;
using Optical.Platform.Backup;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WavefrontTester.Views;

namespace WavefrontTester
{
    public partial class MainForm
    {
        #region Fields
        private static readonly string configKey = "WavefrontTester.wavefrontConfig";
        private static readonly string configDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\WavefrontTester";
        private static readonly string configFileName = "WavefrontConfig.xml";

        private IReadOnlyList<SensorComponents> cameraDevices;

        private bool enableDrawing;

        private ConfigFile configFile;
        private WavefrontStatus status;
        private WavefrontConfig wavefrontConfig;

        private BindingList<LegendreComponents> legendreCoefficients;
        private BindingList<ZernikeComponents> zernikeCoefficients;

        private List<Stopwatch> stopwatches;

        private CalibrationComponents userCalibration;
        #endregion // Fields

        #region Methods
        private void applyCalibration(CalibrationComponents calibration)
        {
            scube?.Calibration(calibration);
            spotMapView.ViewCalibration = calibration;

            if (InvokeRequired)
            {
                Invoke((MethodInvoker)(() => status.Notify()));
            }
            else
            {
                status.Notify();
            }
        }

        private void adjustPolynomialView()
        {
            int legendreDegree = wavefrontConfig.Equation.LegendreDegree + 1;
            if (legendreCoefficients.Count != (legendreDegree * legendreDegree))
            {
                initializeLegendreView(wavefrontConfig.Equation.LegendreDegree);
            }

            if (zernikeCoefficients.Count != (wavefrontConfig.Equation.ZernikeIndex + 1))
            {
                for (int index = 0; index < zernikeCoefficients.Count; index++)
                {
                    zernikeCoefficients[index].Value = 0;
                }
            }
        }

        private void close()
        {
            wavefrontCalibration?.Dispose();
            wavefrontConfig?.Detach();
            intensityMapView?.Detach();

            statusGrid.SelectedObject = null;

            status.Detach();

            scube?.Close();
            scube = null;
        }

        private void deviceExtraction()
        {
            cameraDevices = WavefrontSensor.EnumerateDevice();
            if (cameraDevices.Count > 0)
            {
                deviceListView.DataSource = cameraDevices;
            }
            else
            {
                deviceListView.DataSource = new List<SensorComponents>() { new SensorComponents() };
            }
        }

        private void drawAberrationMap(WavefrontContainer frame)
        {
            var zernikeTask = Task.Run(() =>
            {
                if (!scube.EnableZernike)
                {
                    return;
                }

                var zernikes = new (double Coefficient, bool Enabled)[zernikeCoefficients.Count];
                for (int index = 0; index < frame.Zernike.Length; index++)
                {
                    zernikes[index].Coefficient = frame.Zernike[index];
                    zernikes[index].Enabled = zernikeCoefficients[index].Enabled;
                }

                wavefrontMapView.DrawZernike(zernikes);
            });

            var legendreTask = Task.Run(() =>
            {
                if (!scube.EnableLegendre)
                {
                    return;
                }

                var legendres = new (double Coefficient, bool Enabled)[legendreCoefficients.Count];
                for (int legendreIndex = 0; legendreIndex < frame.Legendre.Length; legendreIndex++)
                {
                    legendres[legendreIndex].Coefficient = frame.Legendre[legendreIndex];
                    legendres[legendreIndex].Enabled = legendreCoefficients[legendreIndex].Enabled;
                }

                wavefrontMapView.DrawLegendre(legendres);
            });

            Task.WaitAll(zernikeTask, legendreTask);
        }

        private void initializeConfigFile()
        {
            wavefrontConfig = new WavefrontConfig();
            configFile = new ConfigFile();
            configFile.Register(configKey, wavefrontConfig);

            loadWavefrontConfig();

            propertyGrid.SelectedObject = wavefrontConfig;
            propertyGrid.ExpandAllGridItems();
        }

        private void initializeLegendreView(int degree)
        {
            var list = new List<LegendreComponents>((degree + 1) * (degree + 1));
            for (int n = 0; n <= degree; n++)
            {
                for (int m = 0; m <= degree; m++)
                {
                    list.Add(new LegendreComponents(m, n, true));
                }
            }

            legendreCoefficients = new BindingList<LegendreComponents>(list);
            legendreView.DataSource = legendreCoefficients;
        }

        private void initializeLog()
        {
            logDescription.Rows.Add("Product", "");
            logDescription.Rows.Add("Serial", "");
            logDescription.Rows.Add("Note", "");
        }

        private void initializePageResults()
        {
            initializeZernikeView();
            initializeLegendreView(wavefrontConfig?.Equation.LegendreDegree ?? 0);
        }

        private void initializePageViews()
        {
            spotMapView = new SpotMapView();
            tabPageSpotView.Controls.Add(spotMapView);

            intensityMapView = new IntensityMapView();
            tabPageIntensityView.Controls.Add(intensityMapView);

            wavefrontMapView = new WavefrontMapView();
            tabPageWavefrontView.Controls.Add(wavefrontMapView);
        }

        private void initializeParameter()
        {
            cameraDevices = WavefrontSensor.EnumerateDevice();
            if (cameraDevices.Count > 0)
            {
                deviceListView.DataSource = cameraDevices;
            }
            else
            {
                deviceListView.DataSource = new List<SensorComponents>() { new SensorComponents() };
            }

            status = new WavefrontStatus();
            checkCalibration.DataBindings.Add(nameof(checkCalibration.Checked), status, nameof(status.IsCalibrated), false, DataSourceUpdateMode.OnPropertyChanged);
            checkMeasure.DataBindings.Add(nameof(checkMeasure.Checked), status, nameof(status.IsMeasuring), false, DataSourceUpdateMode.OnPropertyChanged);
            checkOpen.DataBindings.Add(nameof(checkOpen.Checked), status, nameof(status.IsOpened), false, DataSourceUpdateMode.OnPropertyChanged);

            enableDrawing = true;
            stopwatches = new List<Stopwatch>();
        }

        private void initializeZernikeView()
        {
            zernikeCoefficients = new BindingList<ZernikeComponents>()
            {
                new ZernikeComponents("Z0 (no use)", false),
                new ZernikeComponents("Z1 (Piston)", false),
                new ZernikeComponents("Z2 (Tilt X)", true),
                new ZernikeComponents("Z3 (Tilt Y)", true),
                new ZernikeComponents("Z4 (Defocus)", true),
                new ZernikeComponents("Z5 (AS 0/90)", true),
                new ZernikeComponents("Z6 (AS +/-45)", true),
                new ZernikeComponents("Z7 (Coma X)", true),
                new ZernikeComponents("Z8 (Coma Y)", true),
                new ZernikeComponents("Z9 (3rd SA)", true),
                new ZernikeComponents("Z10 (5th Trefoil X)", true),
                new ZernikeComponents("Z11 (5th Trefoil Y)", true),
                new ZernikeComponents("Z12 (5th AS X)", true),
                new ZernikeComponents("Z13 (5th AS Y)", true),
                new ZernikeComponents("Z14 (5th Coma X)", true),
                new ZernikeComponents("Z15 (5th Coma Y)", true),
                new ZernikeComponents("Z16 (5th SA)", true),
                new ZernikeComponents("Z17 (7th Tetrafoil X)", true),
                new ZernikeComponents("Z18 (7th Tetrafoil Y)", true),
                new ZernikeComponents("Z19 (7th Trefoil X)", true),
                new ZernikeComponents("Z20 (7th Trefoil Y)", true),
                new ZernikeComponents("Z21 (7th AS X)", true),
                new ZernikeComponents("Z22 (7th AS Y)", true),
                new ZernikeComponents("Z23 (7th Coma X)", true),
                new ZernikeComponents("Z24 (7th Coma Y)", true),
                new ZernikeComponents("Z25 (7th SA)", true),
                new ZernikeComponents("Z26 (9th Pentafoil X)", true),
                new ZernikeComponents("Z27 (9th Pentafoil Y)", true),
                new ZernikeComponents("Z28 (9th Tetrafoil X)", true),
                new ZernikeComponents("Z29 (9th Tetrafoil Y)", true),
                new ZernikeComponents("Z30 (9th Trefoil X)", true),
                new ZernikeComponents("Z31 (9th Trefoil Y)", true),
                new ZernikeComponents("Z32 (9th AS X)", true),
                new ZernikeComponents("Z33 (9th AS Y)", true),
                new ZernikeComponents("Z34 (9th Coma X)", true),
                new ZernikeComponents("Z35 (9th Coma Y)", true),
                new ZernikeComponents("Z36 (9th SA)", true)
            };

            zernikeView.DataSource = zernikeCoefficients;
        }

        private void legendreEnabledCheckAll(bool enabled)
        {
            for (int index = 0; index < legendreCoefficients.Count; index++)
            {
                legendreCoefficients[index].Enabled = enabled;
            }

            legendreView.EndEdit();
        }

        private void loadWavefrontConfig()
        {
            if (!File.Exists($@"{configDirectory}\{configFileName}"))
            {
                return;
            }

            configFile?.Load($@"{configDirectory}\{configFileName}");
            wavefrontConfig.Apply(configFile?.Refer<WavefrontConfig>(configKey));
        }

        private void open()
        {
            if (scube?.IsOpened ?? false)
            {
                return;
            }

            if (!(cameraDevices?.Count > 0))
            {
                return;
            }

            SensorComponents camera = cameraDevices[deviceListView.SelectedRows[0].Index];
            scube = new WavefrontSensor(camera);

            scube.NewFrame += Scube_NewFrame;
            scube.NewResult += Scube_NewResult;

            wavefrontCalibration = new WavefrontCalibration(scube);
            wavefrontConfig.Attach(scube);
            intensityMapView.Attach(scube);

            statusGrid.SelectedObject = scube;

            status.Attach(scube);
            wavefrontConfig.PropertyChanged += WavefrontConfig_PropertyChanged;

            spotMapView.Wavefront = scube;
        }

        private void saveLog()
        {
            if (!File.Exists(filePath.Text))
            {
                resultLog.CreateResultFile(filePath.Text, wavefrontConfig.Equation.ZernikeIndex, wavefrontConfig.Equation.LegendreDegree);
            }

            var logCaption = new Logs.Log.LogComponents()
            {
                Product = logDescription.Rows[0].Cells["Contents"].Value.ToString(),
                Serial = logDescription.Rows[1].Cells["Contents"].Value.ToString(),
                Note = logDescription.Rows[2].Cells["Contents"].Value.ToString()
            };

            if (saveOnce.Checked)
            {
                int targetIndex = scube.BufferIndex;
                if (scube[targetIndex].WaitForCompletion(3000))
                {
                    resultLog.Write(filePath.Text, scube[targetIndex], logCaption);
                }
                else
                {
                    MessageBox.Show("波面の計算結果がタイムアウト（3000ms）。", "計算エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                bool triggerMode = scube.TriggerMode;
                TriggerInput triggerType = scube.TriggerType;
                bool isMeasuring = scube.IsMeasuring;

                scube.Stop();
                scube.TriggerMode = true;
                scube.TriggerType = TriggerInput.Software;
                scube.Start();

                var counter = new ConcurrentQueue<int>(Enumerable.Range(0, (int)loggingCount.Value));

                void scubeNewResult(object sender, int e)
                {
                    if (counter.TryDequeue(out int result))
                    {
                        resultLog.Write(filePath.Text, scube[e], logCaption);
                    }
                }

                scube.NewResult += scubeNewResult;
                logPanel.Enabled = false;
                Task.Run(() =>
                {
                    try
                    {
                        while (counter.Count > 0)
                        {
                            Thread.Sleep((int)loggingInterval.Value);
                            if (counter.Count < 1)
                            {
                                break;
                            }

                            scube.TakeSnapshot();
                        }
                    }
                    finally
                    {
                        scube.NewResult -= scubeNewResult;
                        Invoke((MethodInvoker)(() => logPanel.Enabled = true));
                        MessageBox.Show("ログ取得が完了しました。", "Log", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                });
            }
        }

        private void saveWavefrontConfig()
        {
            if (!Directory.Exists(configDirectory))
            {
                Directory.CreateDirectory(configDirectory);
            }

            configFile.Save($@"{configDirectory}\{configFileName}");
        }

        private void updateResultView(WavefrontContainer frame)
        {
            adjustPolynomialView();

            switch (resultView.SelectedIndex)
            {
                case 0:
                    // Zernike Tab
                    for (int index = 0; index < frame.Zernike.Length; index++)
                    {
                        zernikeCoefficients[index].Value = frame.Zernike[index];
                    }

                    zernikeCoefficients[0].NotifyPropertyChanged();
                    break;
                case 1:
                    // Legendre Tab
                    for (int index = 0; index < frame.Legendre.Length; index++)
                    {
                        legendreCoefficients[index].Value = frame.Legendre[index];
                    }

                    legendreCoefficients[0].NotifyPropertyChanged();
                    break;
                case 2:
                    // Seidel Tab
                    {
                        tiltMagnitudeValue.Text = frame.Seidel.Tilt.Rho.ToString("f6");
                        tiltAngleValue.Text = frame.Seidel.Tilt.Theta.ToString("f6");

                        defocusValue.Text = frame.Seidel.Defocus.ToString("f6");

                        sphericalAberrationValue.Text = frame.Seidel.SphericalAberration.ToString("f6");

                        comaMagnitudeValue.Text = frame.Seidel.Coma.Rho.ToString("f6");
                        comaAngleValue.Text = frame.Seidel.Coma.Theta.ToString("f6");

                        astigmatismMagnitudeValue.Text = frame.Seidel.Astigmatism.Rho.ToString("f6");
                        astigmatismAngleValue.Text = frame.Seidel.Astigmatism.Theta.ToString("f6");
                    }
                    break;
                case 3:
                    // Total Tab
                    {
                        incidentAngleXValue.Text = frame.Total.IncidentAngle.X.ToString("f6");
                        incidentAngleYValue.Text = frame.Total.IncidentAngle.Y.ToString("f6");
                        incidentAngleSyntheticValue.Text = frame.Total.IncidentAngle.Synthetic.ToString("f6");
                        powerValue.Text = frame.Total.Power.ToString("f6");
                        rocXValue.Text = frame.Total.RoCX.ToString("f6");
                        rocYValue.Text = frame.Total.RoCY.ToString("f6");
                        rhoXValue.Text = frame.Total.RhoX.ToString("f6");
                        rhoYValue.Text = frame.Total.RhoY.ToString("f6");
                        pvValue.Text = frame.Total.PV.ToString("f6");
                        rmsValue.Text = frame.Total.RMS.ToString("f6");
                        strehlRatioValue.Text = frame.Total.StrehlRatio.ToString("f6");
                    }
                    break;
                default:
                    break;
            }

            pairCountLabel.Text = $"Pairs: {frame.SpotPairs.Count}";
        }

        private void calibrate(int averagingCount)
        {
            if (scube == null)
            {
                return;
            }

            bool enableCalculation = scube.EnableCalculation;
            double frameRate = scube.FrameRate;
            bool isMeasuring = scube.IsMeasuring;
            TriggerInput triggerType = scube.TriggerType;
            bool triggerMode = scube.TriggerMode;
            int bufferSize = scube.BufferSize;
            int average = scube.AveragingCount;

            scube.Stop();

            scube.EnableCalculation = false;
            scube.FrameRate = 20;
            scube.TriggerMode = true;
            scube.TriggerType = TriggerInput.Software;
            scube.BufferSize = averagingCount;
            scube.AveragingCount = averagingCount;
            scube.BufferIndex = averagingCount - 1;

            scube.Start();

            for (int index = 0; index < averagingCount; index++)
            {
                scube.TakeSnapshot();
                if (!scube[index].WaitForCompletion(3000))
                {
                    throw new TimeoutException($"{nameof(scube.TakeSnapshot)} is timeout.(3000[ms])\nThe camera does not respond.");
                }
            }

            userCalibration = wavefrontCalibration.Generate(scube[averagingCount - 1].Image.RawData, wavefrontConfig.Image.NoiseLevel, scube.Device.DeviceID);
            applyCalibration(userCalibration);

            scube.TriggerType = triggerType;
            scube.TriggerMode = triggerMode;
            scube.AveragingCount = average;
            scube.BufferSize = bufferSize;
            scube.FrameRate = frameRate;
            scube.EnableCalculation = enableCalculation;
            if (isMeasuring)
            {
                scube.Start();
            }
        }

        private void WavefrontConfig_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            status.Notify();
        }

        private void zernikeEnabledCheckAll(bool enabled)
        {
            for (int index = 0; index < zernikeCoefficients.Count; index++)
            {
                zernikeCoefficients[index].Enabled = enabled;
            }

            zernikeView.EndEdit();
        }

        protected void OnNewFrameEvent(WavefrontContainer frame)
        {
            if (!enableDrawing)
            {
                return;
            }

            int viewIndex = 0;
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)(() =>
                {
                    viewIndex = tabControlViews.SelectedIndex;
                    framesLabel.Text = $"Frames: {frame.Image.FrameNumber}";
                }));
            }
            else
            {
                viewIndex = tabControlViews.SelectedIndex;
                framesLabel.Text = $"Frames: {frame.Image.FrameNumber}";
            }

            switch (viewIndex)
            {
                case 0:
                    spotMapView.ApertureCenter = wavefrontConfig.Image.ApertureCenter;
                    spotMapView.ApertureCenterTracking = wavefrontConfig.Image.ApertureCenterTracking;
                    spotMapView.ApertureSize = wavefrontConfig.Image.ApertureSize;
                    spotMapView.ApertureType = wavefrontConfig.Image.ApertureType;
                    spotMapView.NoiseLevel = wavefrontConfig.Image.NoiseLevel;
                    spotMapView.Draw(frame);
                    break;
                case 1:
                    intensityMapView.Draw(frame.Image.RawData);
                    break;
                case 2:
                default:
                    break;
            }
        }

        protected void OnNewResultEvent(WavefrontContainer frame)
        {
            if (!enableDrawing)
            {
                return;
            }

            int viewIndex = 0;
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)(() =>
                {
                    viewIndex = tabControlViews.SelectedIndex;
                    updateResultView(frame);
                    framesLabel.Text = $"Frames: {frame.Image.FrameNumber}";
                }));
            }
            else
            {
                viewIndex = tabControlViews.SelectedIndex;
                updateResultView(frame);
                framesLabel.Text = $"Frames: {frame.Image.FrameNumber}";
            }

            switch (viewIndex)
            {
                case 0:
                    spotMapView.ApertureCenter = wavefrontConfig.Image.ApertureCenter;
                    spotMapView.ApertureCenterTracking = wavefrontConfig.Image.ApertureCenterTracking;
                    spotMapView.ApertureSize = wavefrontConfig.Image.ApertureSize;
                    spotMapView.ApertureType = wavefrontConfig.Image.ApertureType;
                    spotMapView.NoiseLevel = wavefrontConfig.Image.NoiseLevel;
                    spotMapView.Draw(frame);
                    break;
                case 1:
                    intensityMapView.Draw(frame.Image.RawData);
                    break;
                case 2:
                    drawAberrationMap(frame);
                    break;
                default:
                    break;
            }
        }
        #endregion // Methods

        #region Classes
        private class WavefrontStatus : INotifyPropertyChanged
        {
            private IWavefront wavefront;

            public event PropertyChangedEventHandler PropertyChanged;

            private bool isAttached => wavefront != null;

            public bool IsCalibrated => wavefront?.IsCalibrated ?? false;

            public bool IsMeasuring => wavefront?.IsMeasuring ?? false;

            public bool IsOpened => wavefront?.IsOpened ?? false;

            public void OnPropertyChanged(string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            public void Attach(IWavefront wavefront)
            {
                this.wavefront = wavefront;
                OnPropertyChanged();
            }

            public void Detach()
            {
                wavefront = null;
                OnPropertyChanged();
            }

            public void Notify()
            {
                OnPropertyChanged();
            }
        }

        public class LegendreComponents : INotifyPropertyChanged
        {
            private bool enabled;
            private double value;

            public LegendreComponents(int m, int n, bool enabled)
            {
                Caption = $"L({m},{n})";
                this.enabled = enabled;
            }

            public event PropertyChangedEventHandler PropertyChanged;

            public string Caption { get; set; }

            public bool Enabled
            {
                get => enabled;
                set
                {
                    if (value == enabled)
                    {
                        return;
                    }

                    enabled = value;
                    NotifyPropertyChanged();
                }
            }

            public double Value
            {
                get => value;
                set
                {
                    if (value == this.value)
                    {
                        return;
                    }

                    this.value = value;
                    //NotifyPropertyChanged(nameof(Value));
                }
            }

            public void NotifyPropertyChanged(string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public class ZernikeComponents : INotifyPropertyChanged
        {
            private bool enabled;
            private double value;

            public ZernikeComponents(string caption, bool enabled)
            {
                Caption = caption;
                this.enabled = enabled;
            }

            public event PropertyChangedEventHandler PropertyChanged;

            public string Caption { get; set; }

            public bool Enabled
            {
                get => enabled;
                set
                {
                    if (value == enabled)
                    {
                        return;
                    }

                    enabled = value;
                    NotifyPropertyChanged();
                }
            }

            public double Value
            {
                get => value;
                set
                {
                    if (value == this.value)
                    {
                        return;
                    }

                    this.value = value;
                    //NotifyPropertyChanged(nameof(Value));
                }
            }

            public void NotifyPropertyChanged(string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion // Classes
    }
}
