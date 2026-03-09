using Optical.API.Wavefront;
using Optical.API.Wavefront.Sample;
using Optical.Platform.Types;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using WavefrontTester.Logs;
using WavefrontTester.Views;

namespace WavefrontTester
{
    public partial class MainForm : Form
    {
        #region Fields
        private static readonly GridHeaderColumn[] spotPairsHeaderColumns = new GridHeaderColumn[]
        {
            new GridHeaderColumn("Number", "No.", 50, string.Empty, true),
            new GridHeaderColumn("ReferenceX", "Reference X [pix]", 100, "F6", true),
            new GridHeaderColumn("ReferenceY", "Reference Y [pix]", 100, "F6", true),
            new GridHeaderColumn("TargetX", "Target X [pix]", 100, "F6", true),
            new GridHeaderColumn("TargetY", "Target Y [pix]", 100, "F6", true)
        };

        private Log resultLog;
        private Stopwatch frameRateStopwatch;

        private IntensityMapView intensityMapView;
        private SpotMapView spotMapView;
        private WavefrontMapView wavefrontMapView;

        private CalibrationComponents defaultCalibration;
        private WavefrontSensor scube;
        private WavefrontCalibration wavefrontCalibration;
        #endregion

        #region Constructors
        public MainForm()
        {
            InitializeComponent();

            initializeConfigFile();

            initializePageResults();
            initializePageViews();
            initializeParameter();
            initializeLog();

            frameRateStopwatch = new Stopwatch();

            resultLog = new Log();
        }
        #endregion

        #region Methods
        private void closeButton_Click(object sender, EventArgs e)
        {
            close();
        }

        private void deviceButton_Click(object sender, EventArgs e)
        {
            deviceExtraction();
        }

        private void drawPairingArea_Click(object sender, EventArgs e)
        {
            spotMapView.DrawPairingArea = drawPairingArea.Checked;
        }

        private void drawSpotPairMap_Click(object sender, EventArgs e)
        {
            spotMapView.DrawSpotPair = drawSpotPairMap.Checked;
        }

        private void drawSpotProfile_Click(object sender, EventArgs e)
        {
            spotMapView.DrawProfile = drawSpotProfile.Checked;
        }

        private void filePathButton_Click(object sender, EventArgs e)
        {
            var dialog = new SaveFileDialog();
            dialog.AddExtension = true;
            dialog.Filter = "Logファイル(*.log)|*.log";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                filePath.Text = dialog.FileName;
                resultLog.CreateResultFile(dialog.FileName, wavefrontConfig.Equation.ZernikeIndex, wavefrontConfig.Equation.LegendreDegree);
            }
        }

        private void generateDefaultCalibration_Click(object sender, EventArgs e)
        {
            defaultCalibration = wavefrontCalibration.GenerateDefault(scube.SensorSize, scube.Device.DeviceID);
            applyCalibration(defaultCalibration);

            calibrationFileName.Text = "Default calibrated.";
        }

        private async void generateUserCalibration_Click(object sender, EventArgs e)
        {
            await Task.Run(() => calibrate(wavefrontConfig.Image.BufferSize));

            MessageBox.Show("ユーザー校正完了。", "Calibration", MessageBoxButtons.OK, MessageBoxIcon.Information);

            calibrationFileName.Text = "User calibrated.";
        }

        private void legendreCheckAll_Click(object sender, EventArgs e)
        {
            legendreEnabledCheckAll(true);
        }

        private void legendreUncheckAll_Click(object sender, EventArgs e)
        {
            legendreEnabledCheckAll(false);
        }

        private void loadDefaultCalibration_Click(object sender, EventArgs e)
        {
            CalibrationComponents calibration = wavefrontCalibration?.Load(@"./default.clb");
            if (calibration == null)
            {
                MessageBox.Show("キャリブレーションファイルが読み込めません。", "読み込みエラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            applyCalibration(calibration);

            calibrationFileName.Text = @"./default.clb";
        }

        private void loadOptions_Click(object sender, EventArgs e)
        {
            loadWavefrontConfig();
        }

        private void loadUserCalibration_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "校正ファイル(*.clb)|*.clb";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                CalibrationComponents calibration = wavefrontCalibration?.Load(dialog.FileName);
                if (calibration == null)
                {
                    MessageBox.Show("キャリブレーションファイルが読み込めません。", "ファイル読み込みエラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                applyCalibration(calibration);

                calibrationFileName.Text = dialog.FileName;
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (scube == null)
            {
                return;
            }

            scube.NewFrame -= Scube_NewFrame;
            scube.NewResult -= Scube_NewResult;
            scube.Stop();
            scube.Close();
            scube = null;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            AssemblyName assemblyName = Assembly.GetExecutingAssembly().GetName();
            Version assemblyVersion = assemblyName.Version;
            Text += $" Ver.{assemblyVersion.Major}.{assemblyVersion.Minor}.{assemblyVersion.Build}" + (Environment.Is64BitProcess ? " 64bit" : " 32bit");

            resultView.GetType().InvokeMember("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty, null, resultView, new object[] { true });
        }

        private void openButton_Click(object sender, EventArgs e)
        {
            open();
        }

        private void updateToolStripLabels(ulong frames, double frameRate)
        {
        }

        private void saveDefaultCalibration_Click(object sender, EventArgs e)
        {
            if (defaultCalibration == null)
            {
                return;
            }

            wavefrontCalibration.Save(defaultCalibration, @"./default.clb");
        }

        private void saveLogButton_Click(object sender, EventArgs e)
        {
            saveLog();
        }

        private void saveUserCalibration_Click(object sender, EventArgs e)
        {
            if (userCalibration == null)
            {
                MessageBox.Show("校正データがありません。", "ファイル保存エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            var dialog = new SaveFileDialog();
            dialog.AddExtension = true;
            dialog.Filter = "校正ファイル(*.clb)|*.clb";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                wavefrontCalibration.Save(userCalibration, dialog.FileName);
            }
        }

        private void saveOptionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveWavefrontConfig();
        }

        private void Scube_NewFrame(object sender, int e)
        {
            for (int count = stopwatches.Count; count < wavefrontConfig.Image.BufferSize; count++)
            {
                stopwatches.Add(new Stopwatch());
            }

            if (wavefrontConfig.Equation.EnableCalculation)
            {
                stopwatches[e].Restart();
                return;
            }

            var image = new ImageContainer(scube[e].Image);
            var frame = new WavefrontContainer(scube[e]);
            Task.Run(() => OnNewFrameEvent(frame));
        }

        private void Scube_NewResult(object sender, int e)
        {
            if (!wavefrontConfig.Equation.EnableCalculation)
            {
                return;
            }

            stopwatches[e].Stop();
            string interval = stopwatches[e].ElapsedMilliseconds.ToString();

            var image = new ImageContainer(scube[e].Image);
            var frame = new WavefrontContainer(scube[e]);
            Task.Run(() =>
            {
                OnNewResultEvent(frame);
                Invoke((System.Windows.Forms.MethodInvoker)(() => resultIntervalLabel.Text = $"Result Interval: {stopwatches[e].ElapsedMilliseconds}[ms]"));
            });
        }

        private void snapshotButton_Click(object sender, EventArgs e)
        {
            if (!(scube?.IsMeasuring ?? false))
            {
                return;
            }

            scube?.TakeSnapshot();
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            adjustPolynomialView();

            for (int count = stopwatches.Count; count < wavefrontConfig.Image.BufferSize; count++)
            {
                stopwatches.Add(new Stopwatch());
            }

            scube?.Start();

            status.Notify();
        }

        private void stopDrawing_Click(object sender, EventArgs e)
        {
            enableDrawing = !stopDrawing.Checked;
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            scube?.Stop();
            status.Notify();
        }

        private void zernikeCheckAll_Click(object sender, EventArgs e)
        {
            zernikeEnabledCheckAll(true);
        }

        private void zernikeUncheckAll_Click(object sender, EventArgs e)
        {
            zernikeEnabledCheckAll(false);
        }
        #endregion

        #region Classes
        private class GridHeaderColumn
        {
            public GridHeaderColumn(string columnName, string columnText, int minimumWidth, string format, bool readOnly)
            {
                ColumnName = columnName;
                ColumnText = columnText;
                MinimumWidth = minimumWidth;
                Format = format;
                ReadOnly = readOnly;
            }

            public string ColumnName { get; }
            public string ColumnText { get; }
            public int MinimumWidth { get; }
            public string Format { get; }
            public bool ReadOnly { get; }
        }
        #endregion
    }
}
