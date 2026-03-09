using Optical.API.Library;
using Optical.API.Library.Device;
using Optical.API.Library.Optics;
using Optical.Core.Camera;
using Optical.Enums;
using Optical.Platform.Mathematics;
using Optical.Platform.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace Optical.API.Beams.Sample
{
    internal class BeamProfilerLogic
    {
        #region Fields
        private static Dictionary<string, CameraComponents> sensorTable;

        private int bitDepth;
        private int bufferIndex;
        private List<ManualResetEventSlim> bufferLockEvent;
        private CpuComponents cpu;
        private ISensorCamera camera;

        private double luminanceBaseline;
        #endregion // Fields

        #region Constructors
        static BeamProfilerLogic()
        {
            sensorTable = new Dictionary<string, CameraComponents>();
        }

        /// <summary>
        /// センサーの識別情報に従い、新しいインスタンスを生成します。
        /// </summary>
        /// <param name="component">センサー識別情報</param>
        public BeamProfilerLogic(SensorComponents component)
        {
            initializeParameter();
            initializeSensor(component);
        }
        #endregion // Constructors

        #region Events
        /// <summary>
        /// 画像データが更新された場合にイベントが発生します。
        /// </summary>
        public event EventHandler<int> NewFrame;

        /// <summary>
        /// 計算結果が更新された場合にイベントが発生します。
        /// </summary>
        public event EventHandler<int> NewResult;
        #endregion

        #region Properties
        /// <summary>
        /// このプロセスでのスレッドの実行をスケジュールできるプロセッサーを取得または設定します
        /// </summary>
        public ProcessorId AffinityMask { get; set; }

        /// <summary>
        /// プロセッサーの関係性（<see cref="AffinityMask"/>）の有効状態を取得・設定します。
        /// </summary>
        public bool AffinityMaskEnabled { get; set; }

        /// <summary>
        /// ビーム重心の算出方法を取得・設定します。
        /// </summary>
        /// <remarks><see cref="AutoApertureEnabled"/>が<see langword="true"/>の場合のみ有効。</remarks>
        public CentroidMethod AutoApertureCalculationType { get; set; }

        /// <summary>
        /// スポット開口の自動計算可否を示す値を取得・設定します。
        /// </summary>
        /// <seealso cref="AutoApertureRate"/>
        public bool AutoApertureEnabled { get; set; }

        /// <summary>
        /// スポット開口の自動計算方式を取得・設定します。
        /// </summary>
        public BeamwidthType AutoApertureMethod { get; set; }

        /// <summary>
        /// スポット開口の自動計算結果に対する倍率を取得・設定します。
        /// </summary>
        /// <seealso cref="AutoApertureEnabled"/>
        public double AutoApertureRate { get; set; }

        /// <summary>
        /// 自動露光時間調整の動作可否を取得・設定します。
        /// </summary>
        public bool AutoExposure
        {
            get => camera.AutoExposure;
            set => camera.AutoExposure = value;
        }

        /// <summary>
        /// 自動ゲイン調整調整の動作可否を取得・設定します。
        /// </summary>
        public bool AutoGain
        {
            get => camera.AutoGain;
            set => camera.AutoGain = value;
        }

        /// <summary>
        /// Centroid の算出方法を取得・設定します。
        /// </summary>
        public CentroidMethod CentroidType { get; set; } = CentroidMethod.Luminance;

        /// <summary>
        /// ビニングする一辺のピクセル数を取得・設定します。
        /// </summary>
        /// <seealso cref="BinningMode"/>
        public int Binning
        {
            get => camera.Binning;
            set => camera.Binning = value;
        }

        /// <summary>
        /// ビニング時のピクセル値算出方法を取得・設定します。
        /// </summary>
        public BinningPixelFormat BinningMode
        {
            get => camera.BinningMode;
            set => camera.BinningMode = value;
        }

        /// <summary>
        /// 画像データのビット深度を取得・設定します。
        /// </summary>
        public int BitDepth
        {
            get
            {
                if (bitDepth == 0)
                {
                    bitDepth = camera.BitDepth;
                }

                return bitDepth;
            }

            set
            {
                camera.BitDepth = value;
                bitDepth = camera.BitDepth;
            }
        }

        /// <summary>
        /// イメージセンサーの黒レベルを取得・設定します。
        /// </summary>
        public double BlackLevel
        {
            get => camera.BlackLevel;
            set
            {
                IsCalibrated = false;
                camera.BlackLevel = value;
            }
        }

        /// <summary>
        /// 測定結果バッファの参照先インデックスを取得・設定します。
        /// </summary>
        public int BufferIndex
        {
            get => bufferIndex;
            set => bufferIndex = value % FrameBuffers.Count;
        }

        /// <summary>
        /// 測定結果バッファのサイズを取得・設定します。
        /// </summary>
        /// <remarks>初期サイズ：1
        /// <para>測定停止中のみ変更可能です。</para></remarks>
        public int BufferSize
        {
            get => FrameBuffers.Count;
            set
            {
                if (FrameBuffers.Count > value)
                {
                    int delta = FrameBuffers.Count - value;
                    FrameBuffers.RemoveRange(value, delta);
                    FrameBuffers.TrimExcess();

                    bufferLockEvent.RemoveRange(value, delta);
                    bufferLockEvent.TrimExcess();
                    bufferLockEvent.ForEach(lockEvent => lockEvent.Reset());
                }
                else
                {
                    for (int index = FrameBuffers.Count; index < value; index++)
                    {
                        bufferLockEvent.Add(new ManualResetEventSlim(false));
                        FrameBuffers.Add(new BeamProfileContainer());
                    }
                }
            }
        }

        /// <summary>
        /// デバイス情報を取得します。
        /// </summary>
        public SensorComponents Device { get; internal set; }

        /// <summary>
        /// デバイスIDを取得します。
        /// </summary>
        public string DeviceId { get; private set; }

        /// <summary>
        /// スポット径の自動計算を設定します。
        /// </summary>
        /// <remarks><see lang="true"/>のときは画像取得後にスポット計算を行い、<see lang="false"/>のときには画像取得後にスポット計算を行いません。 </remarks>
        public bool EnableCalculation { get; set; }

        /// <summary>
        /// 計算をするスポット径の種類を取得・設定します。
        /// </summary>
        public BeamwidthType EnableDiameter { get; set; }

        /// <summary>
        /// イメージセンサーの露光時間[ms]を取得・設定します。
        /// </summary>
        public double ExposureTime
        {
            get => camera.ExposureTime;
            set
            {
                IsCalibrated = false;
                camera.ExposureTime = value;
            }
        }

        /// <summary>
        /// 露光時間の設定範囲[ms]
        /// </summary>
        public Limit<double> ExposureTimeRange => camera.ExposureTimeRange;

        /// <summary>
        /// 画像の水平方向のミラーリングを有効にします。
        /// </summary>
        /// <remarks>キャプチャした画像の各ラインのピクセル値は、ラインの中心を軸に端と端が入れ替わります。</remarks>
        public bool FlipHorizontal
        {
            get => camera.FlipHorizontal;
            set => camera.FlipHorizontal = value;
        }

        /// <summary>
        /// 画像の垂直方向のミラーリングを有効にします。
        /// </summary>
        /// <remarks>撮影した画像の各行のピクセル値が、その行の中心を軸に端から端まで入れ替わります。</remarks>
        public bool FlipVertical
        {
            get => camera.FlipVertical;
            set => camera.FlipVertical = value;
        }

        /// <summary>
        /// 画像データの更新周期[Hz]を設定します。
        /// </summary>
        public double FrameRate
        {
            get => camera.FrameRate;
            set => camera.FrameRate = value;
        }

        /// <summary>
        /// 画像データの更新周期の設定範囲[Hz]
        /// </summary>
        public Limit<double> FrameRateRange => camera.FrameRateRange;

        /// <summary>
        /// 測定結果バッファを参照します。
        /// </summary>
        public List<BeamProfileContainer> FrameBuffers { get; private set; }

        /// <summary>
        /// 指定されたインデックスのフレームバッファを参照します。
        /// <seealso cref="BufferIndex"/>
        /// </summary>
        public BeamProfileContainer FrameBuffer => FrameBuffers[bufferIndex];

        /// <summary>
        /// イメージセンサーのゲイン[dB]を取得・設定します。
        /// </summary>
        public double Gain
        {
            get => camera.Gain;
            set
            {
                IsCalibrated = false;
                camera.Gain = value;
            }
        }

        /// <summary>
        /// ゲインの設定範囲[dB]
        /// </summary>
        public Limit<double> GainRange => camera.GainRange;

        /// <summary>
        /// デバイスが校正済みかどうかを確認します。
        /// </summary>
        public bool IsCalibrated { get; internal set; }

        /// <summary>
        /// デバイスが使用可能な状態かどうかを確認します。
        /// </summary>
        public bool IsOpened => camera.IsOpened;

        /// <summary>
        /// デバイスが測定中かどうかを確認します。
        /// </summary>
        public bool IsMeasuring => camera.IsGrabbing;

        /// <summary>
        /// 楕円ビームの向きを考慮するかどうかを示す値を取得・設定します。
        /// </summary>
        public bool Orientation { get; internal set; }

        /// <summary>
        /// 対象領域の範囲[pixel]を取得・設定します。
        /// </summary>
        public List<Rectangle> RoiList { get; set; }

        /// <summary>
        /// 対象領域の開口形状を取得・設定します。
        /// </summary>
        public ApertureShape RoiShape { get; set; }

        /// <summary>
        /// イメージセンサーの画面解像度[pixel]を取得します。
        /// </summary>
        public Size<int> SensorSize { get; private set; }

        /// <summary>
        /// トリガーモード動作時の入力タイプを取得・設定します。
        /// </summary>
        public TriggerInput TriggerType
        {
            get => camera.TriggerType;
            set => camera.TriggerType = value;
        }

        /// <summary>
        /// トリガー受信後、トリガーがアクティブになるまでの時間[ms]を取得・設定します。
        /// </summary>
        public double TriggerDelay
        {
            get => camera.TriggerDelay;
            set => camera.TriggerDelay = value;
        }

        /// <summary>
        /// 外部トリガーで動作するどうかを示す値を取得・設定します。
        /// </summary>
        public bool TriggerMode
        {
            get => camera.TriggerMode;
            set => camera.TriggerMode = value;
        }
        #endregion // Properties

        #region Methods
        /// <summary>
        /// 自動で取得した画像から輝度のベースラインを算出する。
        /// </summary>
        /// <exception cref="OperationCanceledException">入射光が強すぎます。</exception>
        /// <exception cref="TimeoutException">センサーからの応答がありません。</exception>
        private double autoLuminanceBaseline()
        {
            // 画像取得イベント生成
            var newFrameEvent = new AutoResetEvent(false);
            var frame = new ImageContainer();
            int latestIndex = -1;
            void newFrame(object sender, ImageContainer e)
            {
                frame = e;
                latestIndex++;
                newFrameEvent.Set();
            };

            camera.NewFrame += newFrame;

            // Black Level初期化
            double preBlackLevel = camera.BlackLevel;
            camera.BlackLevel = 0;

            // Black Levelが反映されるまで待機。
            Thread.Sleep(500);

            bool preTriggerMode = camera.TriggerMode;
            TriggerInput preTriggerType = camera.TriggerType;
            bool preGrabbing = camera.IsGrabbing;

            camera.TriggerMode = true;
            camera.TriggerType = TriggerInput.Software;
            camera.Start();

            try
            {
                // 入射光チェック
                camera.TakeSnapshot();
                int cameraWaitTime = 3000;
                if (!newFrameEvent.WaitOne(cameraWaitTime))
                {
                    throw new TimeoutException($"{nameof(camera.TakeSnapshot)} is timeout.({cameraWaitTime}[ms])\nThe camera does not respond.");
                }

                double peak = ImageProcessing.LuminanceRange(frame.RawData).Maximum;
                double threshold = ((1 << frame.RawData.BitDepth) - 1) * 0.1;
                if (peak > threshold)
                {
                    camera.BlackLevel = preBlackLevel;
                    throw new OperationCanceledException("It is necessary to block the light.");
                }

                // BlackLevel調整
                for (; camera.BlackLevel < (1 << frame.RawData.BitDepth); camera.BlackLevel += 10)
                {
                    // Black Levelが反映されるまで待機。
                    Thread.Sleep(500);

                    camera.TakeSnapshot();
                    if (!newFrameEvent.WaitOne(cameraWaitTime))
                    {
                        camera.BlackLevel = preBlackLevel;
                        throw new TimeoutException($"{nameof(camera.TakeSnapshot)} is timeout.({cameraWaitTime}[ms])\nThe camera does not respond.");
                    }

                    Limit<double> luminance = ImageProcessing.LuminanceRange(frame.RawData);
                    if (luminance.Minimum > 0)
                    {
                        break;
                    }
                }

                // Baseline測定(10回平均)
                double[] baselines = new double[10];
                for (int baselineIndex = 0; baselineIndex < baselines.Length; baselineIndex++)
                {
                    int[] noise = new int[1 << frame.RawData.BitDepth];

                    camera.TakeSnapshot();
                    if (!newFrameEvent.WaitOne(cameraWaitTime))
                    {
                        throw new TimeoutException($"{nameof(camera.TakeSnapshot)} is timeout.({cameraWaitTime}[ms])\nThe camera does not respond.");
                    }

                    // 各輝度値のPixel数をカウントする。
                    Func<byte[], int, uint> bitConverter = Bit.SelectConverter(frame.RawData.BitDepth);
                    int imageSize = frame.RawData.Height * frame.RawData.Width;
                    for (int pixelIndex = 0; pixelIndex < imageSize; pixelIndex++)
                    {
                        uint luminance = bitConverter(frame.RawData.Pixels, pixelIndex);
                        noise[luminance]++;
                    }

                    // Pixel数が+/-で同数になるような輝度値を算出する。
                    double pixelCount = 0;
                    double countThreshold = imageSize / 2;
                    for (int noiseIndex = 0; noiseIndex < noise.Length; noiseIndex++)
                    {
                        if ((pixelCount + noise[noiseIndex]) < countThreshold)
                        {
                            pixelCount += noise[noiseIndex];
                            continue;
                        }

                        baselines[baselineIndex] = ((countThreshold - pixelCount) / noise[noiseIndex]) + (noiseIndex - 1);
                        break;
                    }
                }

                return baselines.Average();
            }
            finally
            {
                camera.NewFrame -= newFrame;
                camera.TriggerMode = preTriggerMode;
                camera.TriggerType = preTriggerType;
                if (preGrabbing)
                {
                    camera.Start();
                }
                else
                {
                    camera.Stop();
                }
            }
        }

        private void Camera_NewFrame(object sender, ImageContainer e)
        {
            if (FrameBuffers.Count == 0)
            {
                return;
            }

            int latestIndex = (bufferIndex + 1) % FrameBuffers.Count;
            if (bufferLockEvent[latestIndex].IsSet)
            {
                return;
            }

            FrameBuffers[latestIndex].Clear();
            FrameBuffers[latestIndex].Image.Update(e, true);
            bufferIndex = latestIndex;

            if (EnableCalculation)
            {
                Task createTask(int index)
                {
                    bufferLockEvent[index].Set();
                    return new Task(() =>
                    {
                        if (AffinityMaskEnabled)
                        {
                            cpu.CurrentThreadAffinity(AffinityMask);
                        }

                        calculateSpotDiameter(index);
                        raiseNewResult(index);

                        bufferLockEvent[index].Reset();
                    });
                }

                createTask(latestIndex).Start();
            }

            Task.Run(() =>
            {
                raiseNewFrame(latestIndex);
            });
        }

        /// <summary>
        /// 指定されたバッファのデータからビーム径を算出します。
        /// </summary>
        /// <param name="bufferIndex">Frame Buffer Index</param>
        private void calculateSpotDiameter(int bufferIndex)
        {
            // Manual Aperture指定
            var targetRoi = new List<Rectangle>();
            var rawImageArea = new Rectangle(0, 0, FrameBuffers[bufferIndex].Image.Width, FrameBuffers[bufferIndex].Image.Height);
            if (RoiList.Count > 0)
            {
                targetRoi.AddRange(RoiList);
            }
            else
            {
                targetRoi.Add(rawImageArea);
            }

            var spotList = new List<BeamSpotContainer>(targetRoi.Count);
            for (int index = 0; index < targetRoi.Count; index++)
            {
                spotList.Add(new BeamSpotContainer());
                spotList[index].Roi = targetRoi[index];
                spotList[index].RoiNumber = index + 1;
                spotList[index].RoiShape = RoiShape;
            }

            if (AutoApertureEnabled)
            {
                spotDiameterAuto(bufferIndex, ref spotList);
            }
            else
            {
                spotDiameterManual(bufferIndex, ref spotList);
            }

            FrameBuffers[bufferIndex].UpdateSpotList(spotList);
        }

        /// <summary>
        /// Encircled Energy Profile情報から、指定された割合のビーム径を算出します。
        /// </summary>
        /// <param name="encircledEnergy">Encircled Energy Profile情報</param>
        /// <param name="encircledRate">全ビームエネルギーの積算値に対する割合</param>
        /// <returns></returns>
        private double encircledEnergyDiameter(List<double> encircledEnergy, double encircledRate)
        {
            if (!(encircledEnergy?.Count > 0))
            {
                return 0;
            }

            // プロファイルから、指定された割合を超える位置のインデックスを取得。
            int targetIndex = 0;
            for (int index = 0; index < encircledEnergy.Count; index++)
            {
                if ((targetIndex == 0) && (encircledEnergy[index] > encircledRate))
                {
                    targetIndex = index - 1;
                    break;
                }
            }

            // 指定された割合とプロファイルの交点を算出。
            List<Interpolation.SplineCoefficient> coefficients = Interpolation.CubicSplineCurve(encircledEnergy.ToArray());
            Complex[] solution = Equation.SolveCubic(coefficients[targetIndex].a, coefficients[targetIndex].b, coefficients[targetIndex].c, coefficients[targetIndex].d - encircledRate);

            double diameter = 0;
            for (int index = 0; index < solution.Length; index++)
            {
                // 虚数部は浮動小数点の演算誤差未満を無視する(machine.epsilon:2.22044604925031e-16)
                // 演算誤差はmachine.epsilonの整数倍の値になる場合がある。
                if ((Math.Abs(solution[index].Imaginary) < 1e-14) && (solution[index].Real >= 0) && (solution[index].Real <= 1))
                {
                    diameter = (targetIndex + solution[index].Real) * 2;
                    break;
                }
            }

            return diameter;
        }

        private double generateBeamwidthThreshold(double peak, BeamwidthType autoApertureMethod)
        {
            switch (autoApertureMethod)
            {
                case BeamwidthType.FWHM:
                    return peak / 2;
                case BeamwidthType.ReciprocalNapierSquared:
                    return peak / (Math.E * Math.E);
                default:
                    throw new NotImplementedException($"\"{autoApertureMethod}\" is not supported by Auto Aperture.");
            }
        }

        /// <summary>
        /// パラメーター初期化
        /// </summary>
        private void initializeParameter()
        {
            cpu = new CpuComponents(CpuComponents.ExecutableUnit.Thread);

            foreach (ProcessorId id in Enum.GetValues(typeof(ProcessorId)))
            {
                AffinityMask |= id;
            }

            AutoApertureMethod = BeamwidthType.KnifeEdge;
            EnableCalculation = true;
            foreach (BeamwidthType type in Enum.GetValues(typeof(BeamwidthType)))
            {
                EnableDiameter |= type;
            }

            SensorSize = new Size<int>();

            luminanceBaseline = 0;
        }

        /// <summary>
        /// センサー設定初期化
        /// </summary>
        /// <param name="sensor"></param>
        private void initializeSensor(SensorComponents sensor)
        {
            // 現在接続されているセンサーに対象のセンサーがあるかどうか確認する。
            if (sensorTable.Count == 0)
            {
                IReadOnlyList<CameraComponents> cameraList = CameraSearch.EnumerateDevice();
                foreach (CameraComponents camera in cameraList)
                {
                    sensorTable[camera.GenerateKey()] = camera;
                }
            }

            if (!sensorTable.ContainsKey(sensor.Key))
            {
                throw new ArgumentOutOfRangeException(nameof(sensor), sensor.Key, "存在しないデバイスです。");
            }

            // 対象センサーのインスタンス生成。
            var classType = Type.GetType((sensorTable[sensor.Key]).Identifier);
            camera = (ISensorCamera)Activator.CreateInstance(classType, sensorTable[sensor.Key]);
            camera.Open();
            camera.FrameRateEnabled = true;

            camera.NewFrame += Camera_NewFrame;
            SensorSize.Height = camera.Height;
            SensorSize.Width = camera.Width;
            Device = sensor;

            bufferLockEvent = new List<ManualResetEventSlim>();
            bufferLockEvent.Add(new ManualResetEventSlim(false));
            FrameBuffers = new List<BeamProfileContainer>();
            FrameBuffers.Add(new BeamProfileContainer());

            RoiList = new List<Rectangle>();
        }

        private void raiseNewFrame(int bufferIndex)
        {
            NewFrame?.Invoke(this, bufferIndex);
        }

        private void raiseNewResult(int bufferIndex)
        {
            NewResult?.Invoke(this, bufferIndex);
        }

        /// <summary>
        /// Auto Apertureでビーム径を算出する。
        /// </summary>
        /// <param name="bufferIndex">Frame Buffer Index</param>
        /// <param name="spots">計算対象スポット</param>
        private void spotDiameterAuto(int bufferIndex, ref List<BeamSpotContainer> spots)
        {
            List<BeamSpotContainer> spotList = spots;

            // 各ROI内のビーム系を算出
            Parallel.For(0, spotList.Count, index =>
            {
                if ((spotList[index].Roi.Width <= 0) || (spotList[index].Roi.Height <= 0))
                {
                    return;
                }

                ImageComponent<double> offsetImage = ImageProcessing.ImageOffset(FrameBuffers[bufferIndex].Image.RawData, -luminanceBaseline);

                // ROI範囲外のピクセル値が0になるようにMask処理を行う。
                spotList[index].RoiImage = ImageProcessing.Trim(offsetImage, spotList[index].Roi, RoiShape);

                // スポット重心算出
                Limit<double> manualLuminance = ImageProcessing.LuminanceRange(spotList[index].RoiImage);
                ImageProcessing.ImageMoments manualMoments = ImageProcessing.Moment(spotList[index].RoiImage);
                PointD roiCentroid = calculateCentroid(AutoApertureCalculationType, spotList[index].RoiImage, manualMoments, manualLuminance);
                if (roiCentroid.IsEmpty)
                {
                    return;
                }

                // 自動アパーチャ算出
                Size<int> apertureSize = ImageProcessing.KnifeEdge(spotList[index].RoiImage, 0.1, 0.9);
                var autoAperture = new Rectangle()
                {
                    X = (int)(roiCentroid.X - ((double)apertureSize.Width / 2)),
                    Y = (int)(roiCentroid.Y - ((double)apertureSize.Height / 2)),
                    Width = apertureSize.Width,
                    Height = apertureSize.Height
                };
                int roiOffsetX = (spotList[index].Roi.X > 0) ? spotList[index].Roi.X : 0;
                int roiOffsetY = (spotList[index].Roi.Y > 0) ? spotList[index].Roi.Y : 0;
                autoAperture.X += roiOffsetX;
                autoAperture.Y += roiOffsetY;

                // 自動アパーチャを倍率で拡大
                int inflateWidth = (int)Math.Round(autoAperture.Width * (AutoApertureRate - 1) / 2);
                int inflateHeight = (int)Math.Round(autoAperture.Height * (AutoApertureRate - 1) / 2);
                autoAperture.Inflate(inflateWidth, inflateHeight);
                spotList[index].AutoAperture = autoAperture;
                if ((autoAperture.Width <= 0) || (autoAperture.Height <= 0))
                {
                    spotList[index].Bottom = manualLuminance.Minimum;
                    spotList[index].Centroid.X = roiCentroid.X + roiOffsetX;
                    spotList[index].Centroid.Y = roiCentroid.Y + roiOffsetY;
                    spotList[index].Peak = manualLuminance.Maximum;
                    return;
                }

                ImageComponent<double> autoApertureImage = ImageProcessing.Trim(offsetImage, autoAperture, ApertureShape.Circle);
                if ((autoApertureImage.Width <= 0) || (autoApertureImage.Height <= 0))
                {
                    spotList[index].Bottom = manualLuminance.Minimum;
                    spotList[index].Centroid.X = roiCentroid.X + roiOffsetX;
                    spotList[index].Centroid.Y = roiCentroid.Y + roiOffsetY;
                    spotList[index].Peak = manualLuminance.Maximum;
                    return;
                }

                // モーメント算出。
                ImageProcessing.ImageMoments moments = ImageProcessing.Moment(autoApertureImage);

                // Centroid 算出
                Limit<double> autoApertureLuminance = ImageProcessing.LuminanceRange(autoApertureImage);
                PointD autoCentroid = calculateCentroid(CentroidType, autoApertureImage, moments, autoApertureLuminance);

                int apertureOffsetX = (spotList[index].AutoAperture.X > 0) ? spotList[index].AutoAperture.X : 0;
                int apertureOffsetY = (spotList[index].AutoAperture.Y > 0) ? spotList[index].AutoAperture.Y : 0;
                spotList[index].Centroid.XY(autoCentroid.X + apertureOffsetX, autoCentroid.Y + apertureOffsetY);
                spotList[index].TotalCount = moments.Raw00;

                // D86
                if (EnableDiameter.HasFlag(BeamwidthType.D86))
                {
                    // Encircled Energy Profile生成
                    List<double> encircledEnergy = ImageProcessing.EncircledEnergyProfile(autoApertureImage, autoCentroid);
                    spotList[index].D86 = encircledEnergyDiameter(encircledEnergy, 0.865);
                }
                else
                {
                    spotList[index].D86 = -1;
                }

                // D4Sigma
                if (EnableDiameter.HasFlag(BeamwidthType.D4Sigma))
                {
                    if (Orientation)
                    {
                        // 共分散行列の固有値を算出。
                        double u20 = moments.Centroid20 / moments.Raw00;
                        double u11 = moments.Centroid11 / moments.Raw00;
                        double u02 = moments.Centroid02 / moments.Raw00;
                        double commonTerm = Math.Sqrt(((u20 - u02) * (u20 - u02)) + (4 * u11 * u11));
                        if ((u20 + u02) < commonTerm)
                        {
                            spotList[index].D4Sigma.XY(0, 0);
                        }
                        else
                        {
                            spotList[index].D4Sigma.X = 2 * Math.Sqrt(2 * (u20 + u02 + commonTerm));
                            spotList[index].D4Sigma.Y = 2 * Math.Sqrt(2 * (u20 + u02 - commonTerm));
                        }
                    }
                    else
                    {
                        spotList[index].D4Sigma.X = 4 * Math.Sqrt(moments.Centroid20 / moments.Raw00);
                        spotList[index].D4Sigma.Y = 4 * Math.Sqrt(moments.Centroid02 / moments.Raw00);
                    }
                }
                else
                {
                    spotList[index].D4Sigma.XY(-1, -1);
                }

                // 固有ベクトルの角度を算出。（座標系変換）
                if (Orientation)
                {
                    double u20 = moments.Centroid20 / moments.Raw00;
                    double u11 = moments.Centroid11 / moments.Raw00;
                    double u02 = moments.Centroid02 / moments.Raw00;
                    double orientation = Unit.ToDegree((Math.Atan(2 * u11 / (u20 - u02)) / 2));
                    spotList[index].Orientation = (orientation < 0) ? (90 + orientation) : (orientation - 90);
                }

                // Peak, Total Count
                Limit<double> luminance = ImageProcessing.LuminanceRange(autoApertureImage);
                spotList[index].Bottom = luminance.Minimum - luminanceBaseline;
                spotList[index].Peak = luminance.Maximum - luminanceBaseline;
            });
        }

        /// <summary>
        /// Manual Apertureでビーム径を算出する。
        /// </summary>
        /// <param name="bufferIndex">Frame Buffer Index</param>
        /// <param name="spots">計算対象スポット</param>
        private void spotDiameterManual(int bufferIndex, ref List<BeamSpotContainer> spots)
        {
            List<BeamSpotContainer> spotList = spots;

            // 各ROI内のビーム系を算出
            Parallel.For(0, spotList.Count, index =>
            {
                if ((spotList[index].Roi.Width <= 0) || (spotList[index].Roi.Height <= 0))
                {
                    return;
                }

                spotList[index].AutoAperture = Rectangle.Empty;

                ImageComponent<double> offsetImage = ImageProcessing.ImageOffset(FrameBuffers[bufferIndex].Image.RawData, -luminanceBaseline);

                // 元画像からROIを切り抜く。
                spotList[index].RoiImage = ImageProcessing.Trim(offsetImage, spotList[index].Roi, RoiShape);

                // モーメント算出。
                ImageProcessing.ImageMoments moments = ImageProcessing.Moment(spotList[index].RoiImage);

                // Centroid 算出
                Limit<double> manualLuminance = ImageProcessing.LuminanceRange(spotList[index].RoiImage);
                PointD manualCentroid = calculateCentroid(CentroidType, spotList[index].RoiImage, moments, manualLuminance);
                if (manualCentroid.IsEmpty)
                {
                    return;
                }

                int centroidOffsetX = (spotList[index].Roi.X > 0) ? spotList[index].Roi.X : 0;
                int centroidOffsetY = (spotList[index].Roi.Y > 0) ? spotList[index].Roi.Y : 0;
                spotList[index].Centroid.X = manualCentroid.X + centroidOffsetX;
                spotList[index].Centroid.Y = manualCentroid.Y + centroidOffsetY;
                spotList[index].TotalCount = moments.Raw00;

                // D86
                if (EnableDiameter.HasFlag(BeamwidthType.D86))
                {
                    List<double> encircledEnergy = ImageProcessing.EncircledEnergyProfile(spotList[index].RoiImage, manualCentroid);
                    spotList[index].D86 = encircledEnergyDiameter(encircledEnergy, 0.865);
                }

                // D4Sigma
                if (EnableDiameter.HasFlag(BeamwidthType.D4Sigma))
                {
                    if (Orientation)
                    {
                        // 共分散行列の固有値を算出。
                        double u20 = moments.Centroid20 / moments.Raw00;
                        double u11 = moments.Centroid11 / moments.Raw00;
                        double u02 = moments.Centroid02 / moments.Raw00;
                        double commonTerm = Math.Sqrt(((u20 - u02) * (u20 - u02)) + (4 * u11 * u11));
                        if ((u20 + u02) < commonTerm)
                        {
                            spotList[index].D4Sigma.XY(0, 0);
                        }
                        else
                        {
                            spotList[index].D4Sigma.X = 2 * Math.Sqrt(2 * (u20 + u02 + commonTerm));
                            spotList[index].D4Sigma.Y = 2 * Math.Sqrt(2 * (u20 + u02 - commonTerm));
                        }

                        // 固有ベクトルの角度を算出。（座標系変換）
                        double orientation = Unit.ToDegree((Math.Atan(2 * u11 / (u20 - u02)) / 2));
                        spotList[index].Orientation = (orientation < 0) ? (90 + orientation) : (orientation - 90);
                    }
                    else
                    {
                        spotList[index].D4Sigma.X = 4 * Math.Sqrt(moments.Centroid20 / moments.Raw00);
                        spotList[index].D4Sigma.Y = 4 * Math.Sqrt(moments.Centroid02 / moments.Raw00);
                    }
                }
                else
                {
                    spotList[index].D4Sigma.XY(-1, -1);
                }

                // Peak
                Limit<double> luminance = ImageProcessing.LuminanceRange(spotList[index].RoiImage);
                spotList[index].Bottom = luminance.Minimum - luminanceBaseline;
                spotList[index].Peak = luminance.Maximum - luminanceBaseline;
            });
        }

        /// <summary>
        /// センサーデバイス一覧を列挙します。
        /// </summary>
        /// <returns>センサーデバイス一覧</returns>
        public static IReadOnlyList<SensorComponents> EnumerateDevice()
        {
            IReadOnlyList<CameraComponents> cameraList = CameraSearch.EnumerateDevice();

            sensorTable.Clear();
            var sensorList = new List<SensorComponents>();
            foreach (CameraComponents camera in cameraList)
            {
                string key = camera.GenerateKey();
                sensorTable[key] = camera;

                var sensor = new SensorComponents();
                sensor.Key = key;
                sensor.VendorName = camera.VendorName;
                sensor.ModelName = camera.ModelName;
                sensor.DeviceID = camera.DeviceId;
                sensor.ConnectionType = camera.ConnectionType.ToString();
                sensor.SerialNumber = camera.SerialNumber;
                sensorList.Add(sensor);
            }

            return sensorList.AsReadOnly();
        }

        /// <summary>
        /// 取得した画像から輝度のベースラインを自動で設定する。
        /// </summary>
        /// <exception cref="OperationCanceledException">入射光が強すぎます。</exception>
        /// <exception cref="TimeoutException">センサーからの応答がありません。</exception>
        public void AdaptiveCalibration()
        {
            bool grabbing = camera.IsGrabbing;
            if (!grabbing)
            {
                camera.Start();
            }

            camera.NewFrame -= Camera_NewFrame;

            try
            {
                luminanceBaseline = autoLuminanceBaseline();
            }
            catch (TimeoutException)
            {
                throw;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            finally
            {
                camera.NewFrame += Camera_NewFrame;

                if (!grabbing)
                {
                    camera.Stop();
                }
            }

            IsCalibrated = true;
        }

        /// <summary>
        /// 指定されたFrameBuffer内のビーム画像を現状のパラメーターで再計算する。
        /// </summary>
        /// <param name="startIndex">再計算するFrame Bufferの開始インデックス</param>
        /// <param name="count">再計算するバッファの個数</param>
        public void BufferRecalculation(int startIndex, int count)
        {
            if (BufferSize == 0)
            {
                return;
            }

            Parallel.For(startIndex, startIndex + count, index =>
            {
                int targetIndex = index % BufferSize;
                if ((FrameBuffers[targetIndex].Image.RawData.Pixels == null) ||
                    (FrameBuffers[targetIndex].Image.RawData.Pixels.Length == 0))
                {
                    return;
                }

                calculateSpotDiameter(targetIndex);
                raiseNewResult(bufferIndex);
            });
        }

        /// <summary>
        /// デバイスの使用を停止します。
        /// </summary>
        public void Close()
        {
            if (!camera.IsOpened)
            {
                return;
            }

            if (camera.IsGrabbing)
            {
                camera.Stop();
            }

            camera.Close();
        }

        /// <summary>
        /// バッファに格納されている情報を削除します。
        /// </summary>
        public void ClearBuffer()
        {
            foreach (BeamProfileContainer frame in FrameBuffers)
            {
                frame.Clear();
            }
        }

        /// <summary>
        /// 輝度ベースラインの校正状態を初期化する。
        /// </summary>
        public void ClearCalibration()
        {
            IsCalibrated = false;
            luminanceBaseline = 0;
        }

        /// <summary>
        /// デバイスの使用を開始します。
        /// </summary>
        public void Open()
        {
            if (camera.IsOpened)
            {
                return;
            }

            camera.Open();
        }

        /// <summary>
        /// ソフトウェアトリガーによるキャプチャー処理を行います。
        /// </summary>
        public void TakeSnapshot()
        {
            camera.TakeSnapshot();
        }

        /// <summary>
        /// デバイスを測定開始状態にします。
        /// </summary>
        public void Start()
        {
            camera.Start();
        }

        /// <summary>
        /// デバイスの測定を停止します。
        /// </summary>
        public void Stop() => camera.Stop();

        /// <summary>
        /// Centroid を算出
        /// </summary>
        /// <param name="method"></param>
        /// <param name="image"></param>
        /// <param name="moments"></param>
        /// <param name="luminance"></param>
        /// <returns></returns>
        private PointD calculateCentroid(CentroidMethod method, ImageComponent<double> image, ImageProcessing.ImageMoments moments, Limit<double> luminance)
        {
            if (method == CentroidMethod.Area)
            {
                ImageComponent binaryImage = ImageProcessing.Binarize(image, (int)(luminance.Maximum / (Math.E * Math.E)));
                ImageProcessing.LabelingContainer labelStatus = ImageProcessing.LabelingStats(binaryImage);

                // Components[0] には全体情報が入る為要素数 2 以上でなければならない
                if (labelStatus.Components.Length < 2)
                {
                    return PointD.Empty;
                }

                int maxArea = -1;
                int maxAreaIndex = -1;
                for (int labelIndex = 1; labelIndex < labelStatus.Components.Length; labelIndex++)
                {
                    if (maxArea < labelStatus.Components[labelIndex].Area)
                    {
                        maxArea = labelStatus.Components[labelIndex].Area;
                        maxAreaIndex = labelIndex;
                    }
                }

                return new PointD(labelStatus.Components[maxAreaIndex].Centroid.X, labelStatus.Components[maxAreaIndex].Centroid.Y);
            }
            else
            {
                return new PointD(moments.Raw10 / moments.Raw00, moments.Raw01 / moments.Raw00);
            }
        }

        #endregion // Methods
    }
}
