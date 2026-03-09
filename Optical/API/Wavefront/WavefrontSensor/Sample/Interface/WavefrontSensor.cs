using Optical.API.Library.Device;
using Optical.API.Library.Optics;
using Optical.Enums;
using Optical.Platform.Types;
using System;
using System.Collections.Generic;

namespace Optical.API.Wavefront.Sample
{
    /// <summary>
    /// 波面センサー機能を提供します。
    /// </summary>
    public partial class WavefrontSensor : IWavefront
    {
        #region Fields
        private WavefrontLogic logic;
        #endregion // Fields

        #region Constructors
        /// <summary>
        /// センサーの識別情報に従い、<see cref="WavefrontSensor"/>の新しいインスタンスを生成します。
        /// </summary>
        /// <param name="sensor">センサー識別情報</param>
        public WavefrontSensor(SensorComponents sensor)
        {
            logic = new WavefrontLogic(sensor);
            logic.NewFrame += Logic_NewFrame;
            logic.NewResult += Logic_NewResult;
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
        #endregion // Events

        #region Properties
        /// <summary>
        /// 測定結果バッファの参照先インデックスを取得・設定します。
        /// </summary>
        /// <seealso cref="CurrentBuffer"/>
        /// <seealso cref="this[int]"/>
        public int BufferIndex
        {
            get => logic.BufferIndex;
            set => logic.BufferIndex = value;
        }

        /// <summary>
        /// 測定結果バッファのサイズを取得・設定します。
        /// </summary>
        /// <remarks>初期サイズ：1
        /// <para>測定停止中のみ変更可能です。</para></remarks>
        public int BufferSize
        {
            get => logic.BufferSize;
            set => logic.BufferSize = value;
        }

        /// <summary>
        /// デバイス情報を取得します。
        /// </summary>
        public SensorComponents Device => logic.Device;

        /// <summary>
        /// <see cref="BufferIndex"/>で指定された測定結果バッファの参照を取得します。
        /// </summary>
        /// <remarks><see cref="BufferIndex"/>で指定されたBufferList(<see cref="this[int]"/>)内の要素を参照します。</remarks>
        /// <seealso cref="BufferIndex"/>
        /// <seealso cref="BufferSize"/>
        public WavefrontContainer CurrentBuffer => logic.CurrentBuffer;

        /// <summary>
        /// 校正情報が適用されているかどうかを確認します。
        /// </summary>
        public bool IsCalibrated => logic.IsCalibrated;

        /// <summary>
        /// デバイスが測定中かどうかを確認します。
        /// </summary>
        public bool IsMeasuring => logic.IsMeasuring;

        /// <summary>
        /// デバイスが使用可能な状態かどうかを確認します。
        /// </summary>
        public bool IsOpened => logic.IsOpened;

        #region Calibration
        /// <summary>
        /// 光源伝播距離[mm]
        /// </summary>
        /// <remarks>伝播距離が0以下の場合、平面波で校正されたとみなします。</remarks>
        public double PropagationDistance
        {
            get => logic.PropagationDistance;
            set => logic.PropagationDistance = value;
        }

        /// <summary>
        /// 基準点座標リスト
        /// </summary>
        public List<PointD> ReferencePoints => logic.ReferencePoints;
        #endregion // Calibration

        #region Camera
        /// <summary>
        /// 自動露光時間調整の動作可否を取得・設定します。
        /// </summary>
        public bool AutoExposure
        {
            get => logic.AutoExposure;
            set => logic.AutoExposure = value;
        }

        /// <summary>
        /// 自動ゲイン調整調整の動作可否を取得・設定します。
        /// </summary>
        public bool AutoGain
        {
            get => logic.AutoGain;
            set => logic.AutoGain = value;
        }

        /// <summary>
        /// ビニングする一辺のピクセル数を取得・設定します。
        /// </summary>
        public int Binning
        {
            get => logic.Binning;
            set => logic.Binning = value;
        }

        /// <summary>
        /// ビニング時のピクセル値算出方法を取得・設定します。
        /// </summary>
        public BinningPixelFormat BinningMode
        {
            get => logic.BinningMode;
            set => logic.BinningMode = value;
        }

        /// <summary>
        /// ビット深度[bit]
        /// </summary>
        public int BitDepth
        {
            get => logic.BitDepth;
            set => logic.BitDepth = value;
        }

        /// <summary>
        /// イメージセンサーの黒レベルを取得・設定します。
        /// </summary>
        public double BlackLevel
        {
            get => logic.BlackLevel;
            set => logic.BlackLevel = value;
        }

        /// <summary>
        /// イメージセンサーの露光時間[ms]を取得・設定します。
        /// </summary>
        public double ExposureTime
        {
            get => logic.ExposureTime;
            set => logic.ExposureTime = value;
        }

        /// <summary>
        /// 露光時間の設定範囲[ms]
        /// </summary>
        public Limit<double> ExposureTimeRange => logic.ExposureTimeRange;

        /// <summary>
        /// 画像の水平方向のミラーリングを有効にします。
        /// </summary>
        /// <remarks>キャプチャした画像の各ラインのピクセル値は、ラインの中心を軸に端と端が入れ替わります。</remarks>
        public bool FlipHorizontal
        {
            get => logic.FlipHorizontal;
            set => logic.FlipHorizontal = value;
        }

        /// <summary>
        /// 画像の垂直方向のミラーリングを有効にします。
        /// </summary>
        /// <remarks>撮影した画像の各行のピクセル値が、その行の中心を軸に端から端まで入れ替わります。</remarks>
        public bool FlipVertical
        {
            get => logic.FlipVertical;
            set => logic.FlipVertical = value;
        }

        /// <summary>
        /// 画像データの更新周期[Hz]を設定します。
        /// </summary>
        public double FrameRate
        {
            get => logic.FrameRate;
            set => logic.FrameRate = value;
        }

        /// <summary>
        /// 画像データの更新周期の設定範囲[Hz]
        /// </summary>
        public Limit<double> FrameRateRange => logic.FrameRateRange;

        /// <summary>
        /// イメージセンサーのゲイン[dB]を取得・設定します。
        /// </summary>
        public double Gain
        {
            get => logic.Gain;
            set => logic.Gain = value;
        }

        /// <summary>
        /// ゲインの設定範囲[dB]
        /// </summary>
        public Limit<double> GainRange => logic.GainRange;

        /// <summary>
        /// マイクロレンズアレイの焦点距離[um]を取得・設定します。
        /// </summary>
        public double MicroLensArrayFocalLength
        {
            get => logic.MicroLensArrayFocalLength;
            set => logic.MicroLensArrayFocalLength = value;
        }

        /// <summary>
        /// マイクロレンズアレイのピッチ[um]を取得・設定します。
        /// </summary>
        public double MicroLensArrayPitch
        {
            get => logic.MicroLensArrayPitch;
            set => logic.MicroLensArrayPitch = value;
        }

        /// <summary>
        /// 光学倍率を取得します。
        /// </summary>
        public double OpticalMagnification
        {
            get => logic.OpticalMagnification;
            set => logic.OpticalMagnification = value;
        }

        /// <summary>
        /// イメージセンサーのピクセルピッチ[um]を取得します。
        /// </summary>
        public double PixelPitch
        {
            get => logic.PixelPitch;
            set => logic.PixelPitch = value;
        }

        /// <summary>
        /// イメージセンサーの画面解像度[pixel]を取得します。
        /// </summary>
        public Size<int> SensorSize => logic.SensorSize;

        /// <summary>
        /// トリガー受信後、トリガーがアクティブになるまでの時間を取得・設定します。[ms]
        /// </summary>
        public double TriggerDelay
        {
            get => logic.TriggerDelay;
            set => logic.TriggerDelay = value;
        }

        /// <summary>
        /// 外部トリガーで動作するどうかを示す値を取得・設定します。
        /// </summary>
        /// <remarks>初期値:<see langword="false"/></remarks>
        /// <seealso cref="TriggerType"/>
        public bool TriggerMode
        {
            get => logic.TriggerMode;
            set => logic.TriggerMode = value;
        }

        /// <summary>
        /// トリガーモード動作時の入力タイプを取得・設定します。
        /// </summary>
        /// <remarks>初期値:<see cref="TriggerInput.Hardware"/></remarks>
        /// <seealso cref="TriggerMode"/>
        public TriggerInput TriggerType
        {
            get => logic.TriggerType;
            set => logic.TriggerType = value;
        }
        #endregion // Camera

        #region Equation
        /// <summary>
        /// 波面収差を自動的に計算するかどうかを取得・設定します。
        /// </summary>
        public bool EnableCalculation
        {
            get => logic.EnableCalculation;
            set => logic.EnableCalculation = value;
        }

        /// <summary>
        /// Legendre多項式係数を計算するかどうかを取得・設定します。
        /// </summary>
        public bool EnableLegendre
        {
            get => logic.EnableLegendre;
            set => logic.EnableLegendre = value;
        }

        /// <summary>
        /// Zernike多項式係数を計算するかどうかを取得・設定します。
        /// </summary>
        public bool EnableZernike
        {
            get => logic.EnableZernike;
            set => logic.EnableZernike = value;
        }

        /// <summary>
        /// Legendre多項式次数（>= 0）
        /// </summary>
        public int LegendreDegree
        {
            get => logic.LegendreDegree;
            set => logic.LegendreDegree = value;
        }

        /// <summary>
        /// Frienge・Zernike指数（1～36）
        /// </summary>
        public int ZernikeIndex
        {
            get => logic.ZernikeIndex;
            set => logic.ZernikeIndex = value;
        }
        #endregion // Equation

        #region Image
        /// <summary>
        /// アパーチャー中心座標[pixel]を取得・設定します。
        /// </summary>
        public PointD ApertureCenter => logic.ApertureCenter;

        /// <summary>
        /// アパーチャー中心が入力画像のスポットを追跡するかどうかを設定します。
        /// </summary>
        public bool ApertureCenterTracking
        {
            get => logic.ApertureCenterTracking;
            set => logic.ApertureCenterTracking = value;
        }

        /// <summary>
        /// 波面計算時にApertureを固定するかどうかを取得・設定します。
        /// </summary>
        /// <remarks>Fitting関数の情報が都度更新されなくなります。</remarks>
        public bool ApertureFixed
        {
            get => logic.ApertureFixed;
            set => logic.ApertureFixed = value;
        }

        /// <summary>
        /// アパーチャーのサイズ[pixel]を取得・設定します。
        /// </summary>
        public Size<double> ApertureSize => logic.ApertureSize;

        /// <summary>
        /// アパーチャー形状を取得・設定します。
        /// </summary>
        public ApertureShape ApertureType
        {
            get => logic.ApertureType;
            set => logic.ApertureType = value;
        }

        /// <summary>
        /// アパーチャサイズの自動調整をするかどうかを示す値を取得・設定します。
        /// </summary>
        public bool AutoApertureSize
        {
            get => logic.AutoApertureSize;
            set => logic.AutoApertureSize = value;
        }

        /// <summary>
        /// アパーチャの縦横比を(1:1)になるように調整します。
        /// </summary>
        public bool AutoApertureAspectFit
        {
            get => logic.AutoApertureAspectFit;
            set => logic.AutoApertureAspectFit = value;
        }

        /// <summary>
        /// 画像の平均化回数を取得・設定します。（移動平均）
        /// </summary>
        public int AveragingCount
        {
            get => logic.AveragingCount;
            set => logic.AveragingCount = value;
        }

        /// <summary>
        /// イメージセンサーのノイズレベルを取得・設定します。
        /// </summary>
        /// <remarks>
        /// <para>輝度値がノイズレベル以下のピクセルは計算対象外になります。</para>
        /// <para>設定可能な値の範囲は、取得画像のビット深度に依存します。</para></remarks>
        /// <seealso cref="ICameraConfig.BitDepth"/>
        public int NoiseLevel
        {
            get => logic.NoiseLevel;
            set => logic.NoiseLevel = value;
        }

        /// <summary>
        /// ペアリング対象領域の拡大率
        /// </summary>
        public double PairingAreaRate
        {
            get => logic.PairingAreaRate;
            set => logic.PairingAreaRate = value;
        }

        /// <summary>
        /// スポットの重心算出方法
        /// </summary>
        public CentroidMethod SpotCentroidMode
        {
            get => logic.SpotCentroidMode;
            set => logic.SpotCentroidMode = value;
        }

        /// <summary>
        /// スポットとみなす光点の面積範囲を取得・設定します。
        /// </summary>
        public Limit<int> SpotSizeRange => logic.SpotSizeRange;
        #endregion // Image

        #region Light Source
        /// <summary>
        /// 波長[nm]
        /// </summary>
        public double Wavelength
        {
            get => logic.Wavelength;
            set => logic.Wavelength = value;
        }

        /// <summary>
        /// 光路が複光路方式かどうかを取得・設定する。
        /// </summary>
        public bool DoublePass
        {
            get => logic.DoublePass;
            set => logic.DoublePass = value;
        }
        #endregion // Light

        #region PSF
        /// <summary>
        /// パワースペクトルを正規化する。
        /// </summary>
        public bool Normalization
        {
            get => logic.Normalization;
            set => logic.Normalization = value;
        }

        /// <summary>
        /// サンプリンググリッド分割数。
        /// </summary>
        public int SamplingGridDivision
        {
            get => logic.SamplingGridDivision;
            set => logic.SamplingGridDivision = value;
        }

        /// <summary>
        /// 空間分解能[um]
        /// </summary>
        public int SpatialResolution => logic.SpatialResolution;
        #endregion // PSF
        #endregion // Properties

        #region Indexers
        /// <summary>
        /// 測定結果バッファ参照用インデクサー
        /// </summary>
        /// <param name="index">バッファインデックス</param>
        /// <returns>指定された測定結果のバッファデータ</returns>
        public WavefrontContainer this[int index] => logic.FrameBuffers[index % logic.BufferSize];
        #endregion // Indexers

        #region Methods
        // Interfaceがstaticに対応していないため、エラー回避用。
        // C#8.0以降、Interfaceがstaticに対応したら削除。
        IReadOnlyList<SensorComponents> IWavefront.EnumerateDevice()
        {
            throw new NotImplementedException();
        }

        private void Logic_NewFrame(object sender, int e)
        {
            NewFrame?.Invoke(this, e);
        }

        private void Logic_NewResult(object sender, int e)
        {
            NewResult?.Invoke(this, e);
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
            return WavefrontLogic.AberrationMapZernike(zernikes, mapSize);
        }

        /// <summary>
        /// ルジャンドル多項式による2次元収差マップを出力します。
        /// </summary>
        /// <param name="legendres">ルジャンドル多項式係数</param>
        /// <param name="mapSize">収差マップ一辺の長さ[pixels]</param>
        /// <returns>2次元収差マップ</returns>
        public static ImageComponent<double> AberrationMapLegendre((double coefficient, bool Enabled)[] legendres, int mapSize)
        {
            return WavefrontLogic.AberrationMapLegendre(legendres, mapSize);
        }

        /// <summary>
        /// センサーデバイス一覧を列挙します。
        /// </summary>
        /// <returns>センサーデバイス一覧</returns>
        public static IReadOnlyList<SensorComponents> EnumerateDevice()
        {
            return WavefrontLogic.EnumerateDevice();
        }

        /// <summary>
        /// 波面収差を計算するための校正情報を適用します。
        /// </summary>
        /// <param name="calibration">校正情報</param>
        public void Calibration(CalibrationComponents calibration)
        {
            logic.Calibration(calibration);
        }

        /// <summary>
        /// 指定された測定結果バッファの波面収差を計算します。
        /// </summary>
        /// <param name="bufferIndex">測定結果バッファインデックス</param>
        /// <remarks>FreeRun動作中は計算不可。</remarks>
        public void Calculate(int bufferIndex)
        {
            if (!logic.TriggerMode && logic.IsMeasuring)
            {
                return;
            }

            logic.Calculate(bufferIndex);
        }

        /// <summary>
        /// 校正情報を初期化します。
        /// </summary>
        public void ClearCalibration()
        {
            logic.ClearCalibration();
        }

        /// <summary>
        /// デバイスの使用を停止します。
        /// </summary>
        public void Close()
        {
            logic.Close();
        }

        /// <summary>
        /// スポット画像の2次元強度マップを出力します。
        /// </summary>
        /// <param name="spotImage">スポット画像</param>
        /// <param name="mapSize">出力画像サイズ</param>
        /// <returns>2次元強度マップ</returns>
        public ImageComponent IntensityMap(ImageComponent spotImage, Size<int> mapSize)
        {
            return logic.IntensityMap(spotImage, mapSize);
        }

        /// <summary>
        /// デバイスの使用を開始します。
        /// </summary>
        public void Open()
        {
            logic.Open();
        }

        /// <summary>
        /// デバイスを測定開始状態にします。
        /// </summary>
        public void Start()
        {
            logic.Start();
        }

        /// <summary>
        /// デバイスの測定を停止します。
        /// </summary>
        public void Stop()
        {
            logic.Stop();
        }

        /// <summary>
        /// ソフトウェアトリガーによるキャプチャー処理を行います。
        /// </summary>
        /// <remarks><see cref="TriggerMode"/>が<see langword="true"/>の場合のみ動作可能です。</remarks>
        public void TakeSnapshot()
        {
            logic.TakeSnapshot();
        }
        #endregion // Methods
    }
}
