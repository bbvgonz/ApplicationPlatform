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
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Optical.API.Wavefront
{
    internal class WavefrontLogic
    {
        #region Fields
        private bool apertureCenterTracking;
        private bool autoApertureSize;
        private int bufferIndex;
        private List<ManualResetEventSlim> bufferLockEvent;

        /// <summary>
        /// 校正点マップ（原点からの距離で昇順）
        /// </summary>
        /// <remarks>内容をコピーして使用する。</remarks>
        private List<FittingComponents> calibrationMap;

        /// <summary>
        /// 2次元強度マップ用校正点最大数
        /// </summary>
        private int calibrationPointMaxCount;

        /// <summary>
        /// 接続されているカメラのインスタンス。
        /// </summary>
        private ISensorCamera camera;

        /// <summary>
        /// 計算短縮用開口中心
        /// </summary>
        /// <remarks>Aperture固定時に使用。</remarks>
        private PointD fixedApertureCenter;

        /// <summary>
        /// 計算短縮用開口サイズ
        /// </summary>
        /// <remarks>Aperture固定時に使用。</remarks>
        private Size<double> fixedApertureSize;

        /// <summary>
        /// 計算短縮用校正点マップ（原点からの距離で昇順）
        /// </summary>
        /// <remarks>Aperture固定時に使用。</remarks>
        private HashSet<FittingComponents> fixedCalibrationMap;

        /// <summary>
        /// 画像の移動平均制御
        /// </summary>
        private ImageProcessing.MovingAverage movingAverage;

        /// <summary>
        /// フィッティング用行列制御
        /// </summary>
        private List<FittingMatrix> equationBuffers;

        private int legendreDegree;

        private int zernikeIndex;
        #endregion // Fields

        #region Constructors
        public WavefrontLogic(SensorComponents sensor)
        {
            // 現在接続されているセンサーに対象のセンサーがあるかどうか確認する。
            IReadOnlyList<CameraComponents> cameraList = CameraSearch.EnumerateDevice();
            var sensorTable = new Dictionary<string, CameraComponents>();
            foreach (CameraComponents camera in cameraList)
            {
                sensorTable[camera.GenerateKey()] = camera;
            }

            if (!sensorTable.ContainsKey(sensor.Key))
            {
                throw new ArgumentOutOfRangeException(nameof(sensor), sensor.Key, "存在しないデバイスです。");
            }

            initializeParameter();

            Device = sensor;

            // 対象センサーのインスタンス生成。
            var classType = Type.GetType((sensorTable[sensor.Key]).Identifier);
            camera = (ISensorCamera)Activator.CreateInstance(classType, sensorTable[sensor.Key]);
            openCamera();

            movingAverage.Initialize(camera.Width, camera.Height, camera.BitDepth, movingAverage.AveragingCount);

            equationBuffers = new List<FittingMatrix>
            {
                new FittingMatrix()
            };
        }
        #endregion // Constructors

        #region Events
        public event EventHandler<int> NewFrame;
        public event EventHandler<int> NewResult;
        #endregion // Events

        #region Properties
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
        /// アパーチャー中心座標[pixel]を取得・設定します。
        /// </summary>
        public PointD ApertureCenter { get; private set; }

        /// <summary>
        /// 波面計算時にApertureを固定するかどうかを取得・設定します。
        /// </summary>
        public bool ApertureFixed
        {
            get => fixedCalibrationMap != null;
            set
            {
                if (value)
                {
                    ApertureCenterTracking = false;
                    AutoApertureSize = false;

                    fixCalibrationMap();
                }
                else
                {
                    fixedApertureCenter.Clear();
                    fixedApertureSize.Clear();
                    fixedCalibrationMap = null;
                }
            }
        }

        /// <summary>
        /// アパーチャーのサイズ[pixel]を取得・設定します。
        /// </summary>
        public Size<double> ApertureSize { get; private set; }

        /// <summary>
        /// アパーチャー中心が入力画像のスポットを追跡するかどうかを設定します。
        /// </summary>
        public bool ApertureCenterTracking
        {
            get => apertureCenterTracking;
            set
            {
                apertureCenterTracking = value;

                if (value)
                {
                    fixedCalibrationMap = null;
                }
            }
        }

        /// <summary>
        /// アパーチャー形状を取得・設定します。
        /// </summary>
        public ApertureShape ApertureType { get; set; }

        /// <summary>
        /// アパーチャサイズの自動調整をするかどうかを示す値を取得・設定します。
        /// </summary>
        public bool AutoApertureSize
        {
            get => autoApertureSize;
            set
            {
                autoApertureSize = value;

                if (value)
                {
                    fixedCalibrationMap = null;
                }
            }
        }

        /// <summary>
        /// アパーチャの縦横比が(1:1)になるように調整します。
        /// </summary>
        public bool AutoApertureAspectFit { get; set; }

        /// <summary>
        /// 画像の平均化回数を取得・設定します。（移動平均）
        /// </summary>
        public int AveragingCount
        {
            get => movingAverage?.AveragingCount ?? 0;
            set
            {
                if (movingAverage == null)
                {
                    return;
                }

                movingAverage.AveragingCount = value;
            }
        }

        /// <summary>
        /// ビニングする一辺のピクセル数を取得・設定します。
        /// </summary>
        public int Binning
        {
            get => camera.Binning;
            set
            {
                if (camera.Binning == value)
                {
                    return;
                }

                camera.Binning = value;
            }
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
        /// ビット深度[bit]
        /// </summary>
        public int BitDepth
        {
            get => camera.BitDepth;
            set => camera.BitDepth = value;
        }

        /// <summary>
        /// イメージセンサーの黒レベルを取得・設定します。
        /// </summary>
        public double BlackLevel
        {
            get => camera.BlackLevel;
            set => camera.BlackLevel = value;
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
                if (value == FrameBuffers.Count)
                {
                    return;
                }

                if (value < 1)
                {
                    return;
                }

                OnChangeBufferSize(value);
            }
        }

        public WavefrontContainer CurrentBuffer => FrameBuffers[bufferIndex];

        /// <summary>
        /// デバイス情報を取得します。
        /// </summary>
        public SensorComponents Device { get; private set; }

        /// <summary>
        /// 光路が複光路方式かどうかを取得・設定する。
        /// </summary>
        public bool DoublePass { get; set; }

        /// <summary>
        /// 波面収差を自動的に計算するかどうかを取得・設定します。
        /// </summary>
        public bool EnableCalculation { get; set; }

        /// <summary>
        /// Legendre多項式係数を計算するかどうかを取得・設定します。
        /// </summary>
        public bool EnableLegendre { get; set; }

        /// <summary>
        /// Zernike多項式係数を計算するかどうかを取得・設定します。
        /// </summary>
        public bool EnableZernike { get; set; }

        /// <summary>
        /// イメージセンサーの露光時間[ms]を取得・設定します。
        /// </summary>
        public double ExposureTime
        {
            get => camera.ExposureTime;
            set => camera.ExposureTime = value;
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
        /// イメージセンサーのゲイン[dB]を取得・設定します。
        /// </summary>
        public double Gain
        {
            get => camera.Gain;
            set => camera.Gain = value;
        }

        /// <summary>
        /// ゲインの設定範囲[dB]
        /// </summary>
        public Limit<double> GainRange => camera.GainRange;

        /// <summary>
        /// 測定結果バッファを参照します。
        /// </summary>
        public List<WavefrontContainer> FrameBuffers { get; private set; }

        /// <summary>
        /// 校正情報が適用されているかどうかを確認します。
        /// </summary>
        public bool IsCalibrated => (calibrationMap?.Count > 0);

        public bool IsMeasuring => camera?.IsGrabbing ?? false;

        public bool IsOpened => camera?.IsOpened ?? false;

        /// <summary>
        /// Legendre多項式次数
        /// </summary>
        public int LegendreDegree
        {
            get => legendreDegree;
            set
            {
                if (value == legendreDegree)
                {
                    return;
                }

                if (value < 0)
                {
                    return;
                }

                legendreDegree = value;
                OnChangeLegendreDegree(value);
            }
        }

        /// <summary>
        /// マイクロレンズアレイの焦点距離[um]を取得・設定します。
        /// </summary>
        public double MicroLensArrayFocalLength { get; set; }

        /// <summary>
        /// マイクロレンズアレイのレンズ間距離[um]
        /// </summary>
        public double MicroLensArrayPitch { get; set; }

        /// <summary>
        /// イメージセンサーのノイズレベルを取得・設定します。
        /// </summary>
        public int NoiseLevel { get; set; }

        /// <summary>
        /// パワースペクトルを正規化する。
        /// </summary>
        public bool Normalization { get; set; }

        /// <summary>
        /// 光学倍率
        /// </summary>
        public double OpticalMagnification { get; set; }

        /// <summary>
        /// ペアリング対象領域の拡大率
        /// </summary>
        public double PairingAreaRate { get; set; }

        /// <summary>
        /// イメージセンサーのピクセルピッチ[um]を取得します。
        /// </summary>
        public double PixelPitch { get; set; }

        /// <summary>
        /// 光源伝播距離[um]
        /// </summary>
        /// <remarks>伝播距離が0以下の場合、平面波で校正されたとみなします。</remarks>
        public double PropagationDistance { get; set; }

        /// <summary>
        /// 基準点座標リスト
        /// </summary>
        public List<PointD> ReferencePoints { get; set; }

        /// <summary>
        /// サンプリンググリッド分割数。
        /// </summary>
        public int SamplingGridDivision { get; set; }

        /// <summary>
        /// イメージセンサーの画面解像度[pixel]を取得します。
        /// </summary>
        public Size<int> SensorSize => new Size<int>(camera.Width, camera.Height);

        /// <summary>
        /// 空間分解能[um]
        /// </summary>
        public int SpatialResolution { get; }

        /// <summary>
        /// スポットの重心算出方法
        /// </summary>
        public CentroidMethod SpotCentroidMode { get; set; }

        /// <summary>
        /// スポットとみなす光点の面積範囲を取得・設定します。
        /// </summary>
        public Limit<int> SpotSizeRange { get; private set; }

        /// <summary>
        /// トリガー受信後、トリガーがアクティブになるまでの時間を取得・設定します。[ms]
        /// </summary>
        public double TriggerDelay
        {
            get => camera.TriggerDelay;
            set => camera.TriggerDelay = value;
        }

        /// <summary>
        /// 外部トリガーで動作するどうかを示す値を取得・設定します。
        /// </summary>
        /// <remarks>初期値:<see langword="false"/></remarks>
        /// <seealso cref="TriggerType"/>
        public bool TriggerMode
        {
            get => camera.TriggerMode;
            set => camera.TriggerMode = value;
        }

        /// <summary>
        /// トリガーモード動作時の入力タイプを取得・設定します。
        /// </summary>
        /// <remarks>初期値:<see cref="Library.Device.TriggerInput.Hardware"/></remarks>
        /// <seealso cref="TriggerMode"/>
        public TriggerInput TriggerType
        {
            get => camera.TriggerType;
            set => camera.TriggerType = value;
        }

        /// <summary>
        /// 光源波長[um]
        /// </summary>
        public double Wavelength { get; set; }

        /// <summary>
        /// Frienge・Zernike指数
        /// </summary>
        public int ZernikeIndex
        {
            get => zernikeIndex;
            set
            {
                if (value == zernikeIndex)
                {
                    return;
                }

                if ((value < 1) || (value > 36))
                {
                    return;
                }

                zernikeIndex = value;
                OnChangeZernikeIndex(value);
            }
        }
        #endregion // Properties

        #region Methods
        private void fixCalibrationMap()
        {
            fixedApertureCenter.XY(ApertureCenter);
            fixedApertureSize.Height = ApertureSize.Height;
            fixedApertureSize.Width = ApertureSize.Width;

            // フィッティング情報更新
            var fittings = new HashSet<FittingComponents>(generateFittingComponents(calibrationMap, fixedApertureCenter, fixedApertureSize));
            fittings.RemoveWhere(fitting => !fitting.Enabled);
            fixedCalibrationMap = fittings;
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
            if (movingAverage.AveragingCount < 2)
            {
                FrameBuffers[latestIndex].Image.Update(e, true);
            }
            else
            {
                FrameBuffers[latestIndex].Image.Update(imageAveraging(e), true);
            }

            bufferIndex = latestIndex;

            if (IsCalibrated && EnableCalculation)
            {
                // パラメーター保持
                FrameBuffers[latestIndex].Parameter.ApertureType = ApertureType;
                FrameBuffers[latestIndex].Parameter.Wavelength = Wavelength;

                // 波面計算開始
                Task wavefrontTask(int index)
                {
                    bufferLockEvent[latestIndex].Set();
                    return new Task(() =>
                    {
                        calculateWavefront(FrameBuffers[index], equationBuffers[index]);
                        OnNewResult(index);

                        bufferLockEvent[index].Reset();
                    });
                }

                wavefrontTask(latestIndex).Start();
            }
            else
            {
                FrameBuffers[latestIndex].Complete();
            }

            Task.Run(() => OnNewFrame(latestIndex));
        }

        private int calculateMaxCalibrationPoint()
        {
            int width = camera.Width * camera.Binning;
            int height = camera.Height * camera.Binning;
            int horizontalSpotCount = (int)((width * PixelPitch) / (MicroLensArrayPitch * Math.Sqrt(2)));
            int verticalSpotCount = (int)((height * PixelPitch) / (MicroLensArrayPitch * Math.Sqrt(2)));
            return (horizontalSpotCount * verticalSpotCount) + ((horizontalSpotCount + 1) * (verticalSpotCount + 1));
        }

        private void calculateWavefront(WavefrontContainer wavefront, FittingMatrix equation)
        {
            // ラベリング
            ImageProcessing.LabelingContainer labelingStats = ImageProcessing.LabelingStats(wavefront.Image.RawData, NoiseLevel, true);

            // 有効スポットを抽出する
            List<PointD> spotCentroids = extractSpots(labelingStats);
            if (spotCentroids.Count < 1)
            {
                wavefront.Complete();
                return;
            }

            HashSet<FittingComponents> fittings;
            if (fixedCalibrationMap == null)
            {
                // Aperture調整
                (PointD Center, Size<double> Size) aperture = fitAperture(spotCentroids);
                wavefront.Parameter.ApertureCenter.XY(aperture.Center);
                wavefront.Parameter.ApertureSize.Height = ApertureSize.Height;
                wavefront.Parameter.ApertureSize.Width = ApertureSize.Width;

                // フィッティング情報更新
                fittings = new HashSet<FittingComponents>(generateFittingComponents(calibrationMap, aperture.Center, aperture.Size));
                fittings.RemoveWhere(fitting => !fitting.Enabled);
            }
            else
            {
                wavefront.Parameter.ApertureCenter.XY(fixedApertureCenter);
                wavefront.Parameter.ApertureSize.Height = fixedApertureSize.Height;
                wavefront.Parameter.ApertureSize.Width = fixedApertureSize.Width;

                fittings = new HashSet<FittingComponents>(fixedCalibrationMap);
            }

            // ペアリングされたスポットから目的変数・説明変数を生成
            wavefront.SpotPairs.Capacity = spotCentroids.Count;
            equation.Clear();

            double unitConversion = PixelPitch * PixelPitch / MicroLensArrayFocalLength;
            IOrderedEnumerable<PointD> sortedCentroids = spotCentroids.OrderBy(spot => spot.DistanceTo(wavefront.Parameter.ApertureCenter));
            foreach (PointD spotCentroid in sortedCentroids)
            {
                float centroidX = (float)spotCentroid.X;
                float centroidY = (float)spotCentroid.Y;

                // スポットがどの校正点の対象領域内にあるか探索する。
                foreach (FittingComponents fitting in fittings)
                {
                    if (!fitting.PairingArea.Contains(centroidX, centroidY))
                    {
                        continue;
                    }

                    wavefront.SpotPairs.Add(new CorrectionPair<PointD>(new PointD(fitting.Position), spotCentroid));

                    // (mm -> radian) * 変化量補正
                    equation.AddResponse((spotCentroid.X - fitting.Position.X) * unitConversion * (fitting.ApertureArea.Width / 2),
                                         (fitting.Position.Y - spotCentroid.Y) * unitConversion * (fitting.ApertureArea.Height / 2));

                    equation.AddZernike(fitting.ZernikeDerivativeX, fitting.ZernikeDerivativeY);

                    equation.AddLegendre(fitting.LegendreDerivativeX, fitting.LegendreDerivativeY);

                    fittings.Remove(fitting);
                    break;
                }
            }

            // ペア数が[Zernike係数 * 2]未満の場合、計算不可。
            if (wavefront.SpotPairs.Count < (zernikeIndex * 2))
            {
                wavefront.Complete();
                return;
            }

            // 多項式Fitting
            var tasks = new List<Task>();
            if (EnableZernike)
            {
                tasks.Add(Task.Run(() => wavefront.Zernike = equation.SolveZernike()));
            }

            if (EnableLegendre)
            {
                tasks.Add(Task.Run(() => wavefront.Legendre = equation.SolveLegendre()));
            }

            if (tasks.Count > 0)
            {
                Task.WaitAll(tasks.ToArray());
            }

            wavefront.Complete();
        }

        private List<PointD> extractSpots(ImageProcessing.LabelingContainer labelingStats)
        {
            var spotCentroids = new List<PointD>(labelingStats.Components.Length);
            if (SpotCentroidMode == CentroidMethod.Area)
            {
                // 面積重心
                for (int index = 0; index < labelingStats.Components.Length; index++)
                {
                    if ((labelingStats[index].Area > SpotSizeRange.Maximum) ||
                        (labelingStats[index].Area < SpotSizeRange.Minimum))
                    {
                        continue;
                    }

                    spotCentroids.Add(labelingStats[index].Centroid);
                }
            }
            else
            {
                // 輝度重心
                for (int index = 0; index < labelingStats.Components.Length; index++)
                {
                    if ((labelingStats[index].Area > SpotSizeRange.Maximum) ||
                        (labelingStats[index].Area < SpotSizeRange.Minimum))
                    {
                        continue;
                    }

                    ImageComponent trimImage = ImageProcessing.Trim(labelingStats.SourceImage, labelingStats[index].Circumscribed, ApertureShape.Rectangle);
                    ImageProcessing.ImageMoments moments = ImageProcessing.Moment(trimImage);
                    var centroid = new PointD((moments.Raw10 / moments.Raw00) + labelingStats[index].Circumscribed.X,
                                              (moments.Raw01 / moments.Raw00) + labelingStats[index].Circumscribed.Y);

                    spotCentroids.Add(centroid);
                }
            }

            return spotCentroids;
        }

        private (PointD, Size<double>) fitAperture(List<PointD> spotCentroids)
        {
            // Aperture追跡
            var apertureCenter = new PointD();
            if (ApertureCenterTracking && (spotCentroids != null))
            {
                apertureCenter.XY(spotCentroids.Average(centroid => centroid.X), spotCentroids.Average(centroid => centroid.Y));
                ApertureCenter.XY(apertureCenter.X, apertureCenter.Y);
            }
            else
            {
                apertureCenter.XY(ApertureCenter.X, ApertureCenter.Y);
            }

            // Apertureサイズ自動調整
            var apertureSize = new Size<double>();
            if (AutoApertureSize && (spotCentroids != null))
            {
                PointD[] spots = spotCentroids.ToArray();
                Rectangle roi;
                if (ApertureType == ApertureShape.Circle)
                {
                    roi = ImageProcessing.CircumscribedCircle(spots);
                }
                else
                {
                    roi = ImageProcessing.CircumscribedRectangle(spots);
                }

                if (AutoApertureAspectFit && (roi.Width != roi.Height))
                {
                    if (roi.Width > roi.Height)
                    {
                        roi.Inflate(0, roi.Width - roi.Height);
                    }
                    else
                    {
                        roi.Inflate(roi.Height - roi.Width, 0);
                    }
                }

                apertureSize.Height = roi.Height;
                apertureSize.Width = roi.Width;

                ApertureSize.Height = roi.Height;
                ApertureSize.Width = roi.Width;
            }
            else
            {
                apertureSize.Height = ApertureSize.Height;
                apertureSize.Width = ApertureSize.Width;
            }

            return (apertureCenter, apertureSize);
        }

        private List<FittingComponents> generateFittingComponents(List<FittingComponents> source, PointD apertureCenter, Size<double> apertureSize)
        {
            var fitting = new List<FittingComponents>(source.Count);
            var apertureArea = new RectangleF((float)(apertureCenter.X - (apertureSize.Width / 2)),
                                              (float)(apertureCenter.Y - (apertureSize.Height / 2)),
                                              (float)apertureSize.Width,
                                              (float)apertureSize.Height);
            var sortedMap = source.OrderBy(component => component.Position.DistanceTo(apertureCenter.X, apertureCenter.Y)).ToList();
            for (int index = 0; index < sortedMap.Count; index++)
            {
                fitting.Add(new FittingComponents(sortedMap[index]));
                fitting[index].ApertureArea = apertureArea;
                fitting[index].ApertureShape = ApertureType;
            }

            if (fitting.Count < 1)
            {
                return fitting;
            }

            var centerPoint = new PointD(fitting[0].ApertureArea.X + (fitting[0].ApertureArea.Width / 2),
                                         fitting[0].ApertureArea.Y + (fitting[0].ApertureArea.Height / 2));

            int areaBaseSize = (int)(MicroLensArrayPitch / (Math.Sqrt(2) * PixelPitch * Binning));
            double imageDiagonal = SensorSize.Diagonal / 2;
            void derivative(int index)
            {
                if (!fitting[index].Enabled)
                {
                    return;
                }

                // 測定中心からの座標
                var point = new PointD(fitting[index].Position.X - centerPoint.X,
                                       centerPoint.Y - fitting[index].Position.Y);

                // ペアリング対象領域
                float areaSize = areaBaseSize + (float)((PairingAreaRate - 1) * areaBaseSize * point.Distance / imageDiagonal);
                fitting[index].PairingArea = new RectangleF((float)(fitting[index].Position.X - (areaSize / 2)),
                                                            (float)(fitting[index].Position.Y - (areaSize / 2)),
                                                            areaSize,
                                                            areaSize);

                // 正規化
                point.X /= ((fitting[index].ApertureArea.Width == 0) ? camera.Width : fitting[index].ApertureArea.Width) / 2;
                point.Y /= ((fitting[index].ApertureArea.Height == 0) ? camera.Height : fitting[index].ApertureArea.Height) / 2;

                // Zernike
                fitting[index].ZernikeDerivative(Equation.ZernikeDerivativeX(point, fitting[index].ZernikeIndex),
                                                 Equation.ZernikeDerivativeY(point, fitting[index].ZernikeIndex));

                // Legendre
                (double[] DerivativesX, double[] PolynomialsX) = Equation.LegendreDerivatives(point.X, fitting[index].LegendreDegree);
                (double[] DerivativesY, double[] PolynomialsY) = Equation.LegendreDerivatives(point.Y, fitting[index].LegendreDegree);
                fitting[index].LegendreDerivative(DerivativesX, PolynomialsX, DerivativesY, PolynomialsY);
            }

            Parallel.For(0, fitting.Count, index => derivative(index));

            return fitting;
        }

        /// <summary>
        /// 画像の平均化。
        /// </summary>
        /// <param name="image">画像情報</param>
        /// <returns>移動平均化後画像情報</returns>
        /// <remarks><see cref="ImageProcessing.MovingAverage.AveragingCount"/>が2未満の場合、入力された<paramref name="image"/>をそのまま返します。</remarks>
        private ImageContainer imageAveraging(ImageContainer image)
        {
            if (movingAverage.AveragingCount < 2)
            {
                return image;
            }

            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            ImageComponent averagedImage = movingAverage.ImageAveraging(image.RawData);
            ImageContainer result = image.Clone();
            result.RawData = averagedImage;

            return result;
        }

        private void initializeParameter()
        {
            bufferLockEvent = new List<ManualResetEventSlim> { new ManualResetEventSlim(false) };

            calibrationMap = new List<FittingComponents>();

            fixedApertureCenter = new PointD();

            fixedApertureSize = new Size<double>();

            FrameBuffers = new List<WavefrontContainer> { new WavefrontContainer() };

            movingAverage = new ImageProcessing.MovingAverage();

            ApertureCenter = new PointD();

            ApertureSize = new Size<double>();

            EnableCalculation = true;

            EnableLegendre = true;

            EnableZernike = true;

            PairingAreaRate = 1;

            SpotSizeRange = new Limit<int>();

            SpotCentroidMode = CentroidMethod.Luminance;

            SpotSizeRange = new Limit<int>();
        }

        private void openCamera()
        {
            if (camera.IsOpened)
            {
                return;
            }

            camera.Open();
            if (Device.VendorName == "Basler")
            {
                camera.FlipVertical = true;
            }
            else
            {
                camera.FlipVertical = false;
            }

            camera.FrameRateEnabled = true;
            camera.NewFrame += Camera_NewFrame;
        }

        protected void OnChangeBufferSize(int value)
        {
            if (FrameBuffers.Count > value)
            {
                int delta = FrameBuffers.Count - value;
                FrameBuffers.RemoveRange(value, delta);
                FrameBuffers.TrimExcess();

                equationBuffers.RemoveRange(value, equationBuffers.Count - value);
                equationBuffers.TrimExcess();

                bufferLockEvent.RemoveRange(value, delta);
                bufferLockEvent.TrimExcess();
                bufferLockEvent.ForEach(lockEvent => lockEvent.Reset());
            }
            else
            {
                for (int index = FrameBuffers.Count; index < value; index++)
                {
                    bufferLockEvent.Add(new ManualResetEventSlim(false));
                    FrameBuffers.Add(new WavefrontContainer());
                    equationBuffers.Add(new FittingMatrix());
                }
            }
        }

        protected void OnChangeLegendreDegree(int degree)
        {
            for (int index = 0; index < calibrationMap.Count; index++)
            {
                calibrationMap[index].LegendreDegree = degree;
            }

            if (fixedCalibrationMap != null)
            {
                fixCalibrationMap();
            }
        }

        protected void OnChangeZernikeIndex(int zernikeIndex)
        {
            for (int index = 0; index < calibrationMap.Count; index++)
            {
                calibrationMap[index].ZernikeIndex = zernikeIndex;
            }

            if (fixedCalibrationMap != null)
            {
                fixCalibrationMap();
            }
        }

        protected void OnNewFrame(int bufferIndex)
        {
            NewFrame?.Invoke(this, bufferIndex);
        }

        protected void OnNewResult(int bufferIndex)
        {
            NewResult?.Invoke(this, bufferIndex);
        }

        /// <summary>
        /// ゼルニケ多項式による2次元収差マップを出力します。
        /// </summary>
        /// <param name="zernikes">ゼルニケ多項式係数 ([1]～[n], [0]:未使用)</param>
        /// <param name="mapSize">収差マップ一辺の長さ[pixels]</param>
        /// <returns>2次元収差マップ</returns>
        /// <remarks>ゼルニケ多項式範囲外のピクセルは<see cref="double.NaN"/>。</remarks>
        public static ImageComponent<double> AberrationMapZernike((double coefficient, bool Enabled)[] zernikes, int mapSize)
        {
            if (!(zernikes?.Length > 1))
            {
                throw new ArgumentOutOfRangeException(nameof(zernikes));
            }

            if (mapSize < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(mapSize), mapSize, "Map size invalid.");
            }

            var aberrationMap = new ImageComponent<double>()
            {
                Height = mapSize,
                Pixels = new double[mapSize * mapSize],
                Width = mapSize
            };
            int mapCenter = mapSize / 2;
            double pitch = 2 / (double)mapSize;
            Parallel.For(0, mapSize, y =>
            {
                var point = new PointD();
                for (int x = 0; x < mapSize; x++)
                {
                    point.XY((x - mapCenter) * pitch, (y - mapCenter) * pitch);
                    if (point.Distance > 1)
                    {
                        aberrationMap.Pixels[(y * mapSize) + x] = double.NaN;
                        continue;
                    }

                    double[] aberration = Equation.ZernikePolynomials(point, zernikes.Length - 1);
                    for (int index = 0; index < zernikes.Length; index++)
                    {
                        if (!zernikes[index].Enabled)
                        {
                            continue;
                        }

                        aberrationMap.Pixels[(y * mapSize) + x] += zernikes[index].coefficient * aberration[index];
                    }
                }
            });

            return aberrationMap;
        }

        /// <summary>
        /// ルジャンドル多項式による2次元収差マップを出力します。
        /// </summary>
        /// <param name="legendres">ルジャンドル多項式係数</param>
        /// <param name="mapSize">収差マップ一辺の長さ[pixels]</param>
        /// <returns>2次元収差マップ</returns>
        public static ImageComponent<double> AberrationMapLegendre((double coefficient, bool Enabled)[] legendres, int mapSize)
        {
            if (!(legendres?.Length > 1))
            {
                throw new ArgumentOutOfRangeException(nameof(legendres));
            }

            if (mapSize < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(mapSize), mapSize, "Map size invalid.");
            }

            var aberrationMap = new ImageComponent<double>()
            {
                Height = mapSize,
                Pixels = new double[mapSize * mapSize],
                Width = mapSize
            };
            int mapCenter = mapSize / 2;
            double pitch = (double)2 / mapSize;
            int degree = (int)Math.Sqrt(legendres.Length) - 1;
            Parallel.For(0, mapSize, y =>
            {
                var point = new PointD();
                for (int x = 0; x < mapSize; x++)
                {
                    point.XY((x - mapCenter) * pitch, (y - mapCenter) * pitch);
                    double[] aberrationX = Equation.LegendrePolynomials(point.X, degree);
                    double[] aberrationY = Equation.LegendrePolynomials(point.Y, degree);
                    for (int n = 0; n < aberrationY.Length; n++)
                    {
                        for (int m = 0; m < aberrationX.Length; m++)
                        {
                            int legendreIndex = (n * aberrationX.Length) + m;
                            if (!legendres[legendreIndex].Enabled)
                            {
                                continue;
                            }

                            aberrationMap.Pixels[(y * mapSize) + x] += legendres[legendreIndex].coefficient * aberrationX[m] * aberrationY[n];
                        }
                    }
                }
            });

            return aberrationMap;
        }

        /// <summary>
        /// センサーデバイス一覧を列挙します。
        /// </summary>
        /// <returns>センサーデバイス一覧</returns>
        public static IReadOnlyList<SensorComponents> EnumerateDevice()
        {
            IReadOnlyList<CameraComponents> cameraList = CameraSearch.EnumerateDevice();

            var sensorList = new List<SensorComponents>();
            foreach (CameraComponents camera in cameraList)
            {
                var sensor = new SensorComponents
                {
                    Key = camera.GenerateKey(),
                    VendorName = camera.VendorName,
                    ModelName = camera.ModelName,
                    DeviceID = camera.DeviceId,
                    ConnectionType = camera.ConnectionType.ToString(),
                    SerialNumber = camera.SerialNumber
                };
                sensorList.Add(sensor);
            }

            return sensorList.AsReadOnly();
        }

        /// <summary>
        /// 指定された測定結果バッファの波面収差を計算します。
        /// </summary>
        /// <param name="bufferIndex">測定結果バッファインデックス</param>
        public void Calculate(int bufferIndex)
        {
            if (BufferSize == 0)
            {
                return;
            }

            int targetIndex = bufferIndex % BufferSize;
            if ((FrameBuffers[targetIndex].Image.RawData.Pixels == null) ||
                (FrameBuffers[targetIndex].Image.RawData.Pixels.Length == 0))
            {
                return;
            }

            calculateWavefront(FrameBuffers[bufferIndex], equationBuffers[bufferIndex]);
            OnNewResult(bufferIndex);
        }

        /// <summary>
        /// 波面収差を計算するための校正情報を適用します。
        /// </summary>
        /// <param name="calibration">校正情報</param>
        public void Calibration(CalibrationComponents calibration)
        {
            // デバイス情報が一致しない
            if (Device.DeviceID != calibration.DeviceId)
            {
                throw new ArgumentException("Device ID do not match.", nameof(calibration.DeviceId));
            }

            // 校正点データなし
            if (calibration.ReferencePoints.Count == 0)
            {
                throw new ArgumentException("Reference points do not exist.", nameof(calibration.ReferencePoints));
            }

            // 測定停止
            camera.Stop();

            // 校正データ削除
            calibrationMap.Clear();

            // 校正値適用
            MicroLensArrayFocalLength = calibration.MicroLensArrayFocalLength;
            MicroLensArrayPitch = calibration.MicroLensArrayPitch;
            OpticalMagnification = calibration.OpticalMagnification;
            PixelPitch = calibration.PixelPitch;
            PropagationDistance = calibration.PropagationDistance;
            ReferencePoints = new List<PointD>(calibration.ReferencePoints.Count);
            ReferencePoints.AddRange(calibration.ReferencePoints.Select(point => new PointD(point)));
            Wavelength = calibration.Wavelength;

            // 校正点座標から多項式テーブルを生成
            PointD[] sortPoints = calibration.ReferencePoints.Select(point => new PointD(point)).OrderBy(point => point.DistanceTo(ApertureCenter.X, ApertureCenter.Y)).ToArray();
            if (calibration.PropagationDistance > 0)
            {
                // 球面波補正
                int centerX = calibration.SensorSize.Width / 2;
                int centerY = calibration.SensorSize.Height / 2;
                double correctionRate = calibration.PropagationDistance / (calibration.PropagationDistance + MicroLensArrayFocalLength);
                for (int index = 0; index < sortPoints.Length; index++)
                {
                    double x = ((sortPoints[index].X - centerX) * correctionRate) + centerX;
                    double y = ((sortPoints[index].Y - centerY) * correctionRate) + centerY;
                    sortPoints[index].XY(x, y);
                }
            }

            calibrationMap.Capacity = sortPoints.Length;
            for (int index = 0; index < sortPoints.Length; index++)
            {
                calibrationMap.Add(new FittingComponents(sortPoints[index]));
                calibrationMap[index].LegendreDegree = legendreDegree;
                calibrationMap[index].ZernikeIndex = zernikeIndex;
            }

            // 2次元強度マップ用校正点最大数算出
            calibrationPointMaxCount = calculateMaxCalibrationPoint();
        }

        public void ClearCalibration()
        {
            calibrationMap?.Clear();
            calibrationMap = null;
        }

        public void Close()
        {
            if (!camera.IsOpened)
            {
                return;
            }

            camera.NewFrame -= Camera_NewFrame;

            if (camera.IsGrabbing)
            {
                camera.Stop();
            }

            camera.Close();
        }

        /// <summary>
        /// スポット画像の2次元強度マップを出力します。
        /// </summary>
        /// <param name="spotImage">スポット画像</param>
        /// <param name="mapSize">出力画像サイズ</param>
        /// <returns>2次元強度マップ</returns>
        public ImageComponent IntensityMap(ImageComponent spotImage, Size<int> mapSize)
        {
            if (!IsCalibrated)
            {
                return null;
            }

            if (calibrationPointMaxCount <= 0)
            {
                throw new InvalidOperationException("Calibration data is not found.");
            }

            ImageComponent image = ImageProcessing.Resize(spotImage, new Size<int>(mapSize.Width, mapSize.Height), ImageProcessing.InterpolationMethod.Area);

            // モフォロジー変換（Closing）
            int kernelSize = ((int)(Math.Sqrt(mapSize.Width * mapSize.Height / calibrationPointMaxCount) * Math.Sqrt(2)) / 2 * 2) + 1;
            int kernelCenter = kernelSize / 2;
            ImageComponent kernel = ImageProcessing.MorphologyKernel(ImageProcessing.MorphologyShape.Ellipse, new Size<int>(kernelSize, kernelSize), new Point<int>(kernelCenter, kernelCenter));
            ImageComponent close = ImageProcessing.MorphologyTransform(image, ImageProcessing.MorphologyOperation.Close, kernel);

            // ぼかし
            ImageComponent blur = ImageProcessing.GaussianBlur(close, new Size(kernelSize, kernelSize), kernelCenter, kernelCenter);

            return blur;
        }

        public void Open()
        {
            openCamera();
        }

        public void Start()
        {
            camera.Start();
        }

        public void Stop()
        {
            camera.Stop();
        }

        /// <summary>
        /// ソフトウェアトリガーによるキャプチャー処理を行います。
        /// </summary>
        public void TakeSnapshot()
        {
            int nextIndex = (bufferIndex + 1) % FrameBuffers.Count;
            FrameBuffers[nextIndex].Clear();

            camera.TakeSnapshot();
        }
        #endregion // Methods

        #region Classes
        private class FittingMatrix
        {
            #region Fields
            private List<double> responseX;
            private List<double> responseY;

            private List<double[]> zernikeExplanatoryX;
            private List<double[]> zernikeExplanatoryY;

            private List<double[]> legendreExplanetoryX;
            private List<double[]> legendreExplanetoryY;
            #endregion // Fields

            #region Constructors
            public FittingMatrix()
            {
                responseX = new List<double>();
                responseY = new List<double>();

                legendreExplanetoryX = new List<double[]>();
                legendreExplanetoryY = new List<double[]>();

                zernikeExplanatoryX = new List<double[]>();
                zernikeExplanatoryY = new List<double[]>();
            }
            #endregion // Constructors

            #region Methods
            public void Clear()
            {
                responseX.Clear();
                responseY.Clear();

                zernikeExplanatoryX.Clear();
                zernikeExplanatoryY.Clear();

                legendreExplanetoryX.Clear();
                legendreExplanetoryY.Clear();
            }

            public void AddResponse(double x, double y)
            {
                responseX.Add(x);
                responseY.Add(y);
            }

            public void AddLegendre(double[] x, double[] y)
            {
                legendreExplanetoryX.Add(x);
                legendreExplanetoryY.Add(y);
            }

            public void AddZernike(double[] x, double[] y)
            {
                zernikeExplanatoryX.Add(x);
                zernikeExplanatoryY.Add(y);
            }

            public double[] SolveLegendre()
            {
                double[] response = responseX.Concat(responseY).ToArray();
                var expolanatory = legendreExplanetoryX.Concat(legendreExplanetoryY).ToList();
                double[] result = Equation.SolveMatrix(response, expolanatory);

                return result;
            }

            public double[] SolveZernike()
            {
                double[] response = responseX.Concat(responseY).ToArray();
                var expolanatory = zernikeExplanatoryX.Concat(zernikeExplanatoryY).ToList();

                double[] result = Equation.SolveMatrix(response, expolanatory);

                double[] zernikes = new double[result.Length + 1];
                Array.Copy(result, 0, zernikes, 1, result.Length);

                return zernikes;
            }
            #endregion // Methods
        }

        /// <summary>
        /// 多項式フィッティングクラス
        /// </summary>
        private class FittingComponents
        {
            #region Fields
            private Dictionary<ApertureShape, Func<bool>> areaContains;

            private int legendreDegree;

            private int zernikeIndex;

            /// <summary>
            /// スポットペアリング対象領域
            /// </summary>
            public RectangleF PairingArea;

            /// <summary>
            /// 開口領域
            /// </summary>
            public RectangleF ApertureArea;
            #endregion // Fields

            #region Constructors
            public FittingComponents(PointD position)
            {
                initializeParameter();

                Position = new PointD(position);
            }

            /// <summary>
            /// コピーコンストラクタ
            /// </summary>
            /// <param name="fitting">コピー元の <see cref="FittingComponents"/> クラスインスタンス。</param>
            public FittingComponents(FittingComponents fitting)
            {
                initializeParameter();

                changeLegendreDegree(fitting.LegendreDegree);
                changeZernikeIndex(fitting.ZernikeIndex);

                PairingArea = fitting.PairingArea;
                ApertureArea = fitting.ApertureArea;

                ApertureShape = fitting.ApertureShape;

                fitting.LegendreDerivativeX.CopyTo(LegendreDerivativeX, 0);
                fitting.LegendreDerivativeY.CopyTo(LegendreDerivativeY, 0);

                Position = new PointD(fitting.Position);

                fitting.ZernikeDerivativeX.CopyTo(ZernikeDerivativeX, 0);
                fitting.LegendreDerivativeY.CopyTo(LegendreDerivativeY, 0);
            }
            #endregion // Constructors

            #region Properties
            public ApertureShape ApertureShape { get; set; }

            public bool Enabled => areaContains[ApertureShape]();

            /// <summary>
            /// Legendre多項式次数
            /// </summary>
            public int LegendreDegree
            {
                get => legendreDegree;
                set => changeLegendreDegree(value);
            }

            /// <summary>
            /// Legendre X偏導関数
            /// </summary>
            public double[] LegendreDerivativeX { get; set; }

            /// <summary>
            /// Legendre Y偏導関数
            /// </summary>
            public double[] LegendreDerivativeY { get; set; }

            /// <summary>
            /// 校正点座標
            /// </summary>
            /// <remarks>入力画像の左上を原点とした座標系。</remarks>
            public PointD Position { get; }

            /// <summary>
            /// Zernike X偏導関数
            /// </summary>
            public double[] ZernikeDerivativeX { get; set; }

            /// <summary>
            /// Zernike Y偏導関数
            /// </summary>
            public double[] ZernikeDerivativeY { get; set; }

            /// <summary>
            /// Frienge・Zernike指数
            /// </summary>
            public int ZernikeIndex
            {
                get => zernikeIndex;
                set => changeZernikeIndex(value);
            }
            #endregion // Properties

            #region Methods
            private void changeLegendreDegree(int degree)
            {
                if (degree == legendreDegree)
                {
                    return;
                }

                legendreDegree = degree;
                OnChangeLegendreDegree();
            }

            private void changeZernikeIndex(int index)
            {
                if (index == zernikeIndex)
                {
                    return;
                }

                zernikeIndex = index;
                OnChangeZernikeIndex();
            }

            private bool ellipseContains()
            {
                double x = Position.X - (ApertureArea.X + (ApertureArea.Width / 2));
                double y = (ApertureArea.Y + (ApertureArea.Height / 2)) - Position.Y;

                double x2 = x * x;
                double y2 = y * y;

                double a2 = ApertureArea.Width * ApertureArea.Width / 4;
                double b2 = ApertureArea.Height * ApertureArea.Height / 4;

                return ((x2 / a2) + (y2 / b2)) <= 1;
            }

            private void initializeParameter()
            {
                LegendreDerivativeX = Array.Empty<double>();
                LegendreDerivativeY = Array.Empty<double>();

                ZernikeDerivativeX = Array.Empty<double>();
                ZernikeDerivativeY = Array.Empty<double>();

                areaContains = new Dictionary<ApertureShape, Func<bool>>
                {
                    [ApertureShape.Circle] = () => ellipseContains(),
                    [ApertureShape.Rectangle] = () => rectangleContains()
                };
            }

            private bool rectangleContains()
            {
                return ApertureArea.Contains((float)Position.X, (float)Position.Y);
            }

            protected void OnChangeLegendreDegree()
            {
                int legendreLength = (legendreDegree + 1) * (legendreDegree + 1);
                LegendreDerivativeX = new double[legendreLength];
                LegendreDerivativeY = new double[legendreLength];
            }

            protected void OnChangeZernikeIndex()
            {
                ZernikeDerivativeX = new double[zernikeIndex];
                ZernikeDerivativeY = new double[zernikeIndex];
            }

            public void LegendreDerivative(double[] derivativeX, double[] polynomialX, double[] derivativeY, double[] polynomialY)
            {
                if (derivativeX.Length != (legendreDegree + 1))
                {
                    throw new ArgumentException($"配列長異常。\n{nameof(derivativeX)}:{derivativeX.Length}, degree:{legendreDegree}\n", nameof(derivativeX));
                }

                if (polynomialX.Length != (legendreDegree + 1))
                {
                    throw new ArgumentException($"配列長異常。\n{nameof(polynomialX)}:{polynomialX.Length}, degree:{legendreDegree}\n", nameof(polynomialX));
                }

                if (derivativeY.Length != (legendreDegree + 1))
                {
                    throw new ArgumentException($"配列長異常。\n{nameof(derivativeY)}:{derivativeY.Length}, degree:{legendreDegree}\n", nameof(derivativeY));
                }

                if (polynomialY.Length != (legendreDegree + 1))
                {
                    throw new ArgumentException($"配列長異常。\n{nameof(polynomialY)}:{polynomialY.Length}, degree:{legendreDegree}\n", nameof(polynomialY));
                }

                for (int n = 0; n < derivativeY.Length; n++)
                {
                    for (int m = 0; m < derivativeX.Length; m++)
                    {
                        int targetIndex = ((legendreDegree + 1) * n) + m;
                        LegendreDerivativeX[targetIndex] = derivativeX[m] * polynomialY[n];
                        LegendreDerivativeY[targetIndex] = polynomialX[m] * derivativeY[n];
                    }
                }
            }

            public void ZernikeDerivative(double[] x, double[] y)
            {
                if (x.Length != zernikeIndex)
                {
                    throw new ArgumentException($"X配列長異常。\n{nameof(x)}:{x.Length}, index:{zernikeIndex}\n", nameof(x));
                }

                if (y.Length != zernikeIndex)
                {
                    throw new ArgumentException($"Y配列長異常。\n{nameof(y)}:{y.Length}, index:{zernikeIndex}\n", nameof(y));
                }

                x.CopyTo(ZernikeDerivativeX, 0);
                y.CopyTo(ZernikeDerivativeY, 0);
            }
            #endregion // Methods
        }
        #endregion // Classes
    }
}
