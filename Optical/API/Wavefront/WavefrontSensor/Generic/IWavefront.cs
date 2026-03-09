using Optical.API.Library.Device;
using Optical.API.Library.Optics;
using Optical.Platform.Types;
using System;
using System.Collections.Generic;

namespace Optical.API.Wavefront
{
    /// <summary>
    /// 波面収差を測定するためのインターフェースを提供します。
    /// </summary>
    public interface IWavefront : ICameraConfig
    {
        #region Events
        /// <summary>
        /// 計算結果が更新された場合にイベントが発生します。
        /// </summary>
        event EventHandler<int> NewResult;
        #endregion // Events

        #region Properties
        /// <summary>
        /// アパーチャー中心座標[pixel]を取得・設定します。
        /// </summary>
        PointD ApertureCenter { get; }

        /// <summary>
        /// アパーチャー中心が入力画像のスポットを追跡するかどうかを設定します。
        /// </summary>
        bool ApertureCenterTracking { get; set; }

        /// <summary>
        /// 波面計算時にApertureを固定するかどうかを取得・設定します。
        /// </summary>
        /// <remarks>Fitting関数の情報が都度更新されなくなります。</remarks>
        bool ApertureFixed { get; set; }

        /// <summary>
        /// アパーチャーのサイズ[pixel]を取得・設定します。
        /// </summary>
        Size<double> ApertureSize { get; }

        /// <summary>
        /// アパーチャー形状を取得・設定します。
        /// </summary>
        ApertureShape ApertureType { get; set; }

        /// <summary>
        /// アパーチャの縦横比を(1:1)になるように調整します。
        /// </summary>
        bool AutoApertureAspectFit { get; set; }

        /// <summary>
        /// アパーチャサイズの自動調整をするかどうかを示す値を取得・設定します。
        /// </summary>
        bool AutoApertureSize { get; set; }

        /// <summary>
        /// 画像の平均化回数を取得・設定します。（移動平均）
        /// </summary>
        int AveragingCount { get; set; }

        /// <summary>
        /// 計算結果用バッファの参照先インデックスを取得・設定します。
        /// </summary>
        int BufferIndex { get; set; }

        /// <summary>
        /// 計算結果用バッファのサイズを取得・設定します。
        /// </summary>
        /// <remarks>初期サイズ：1
        /// <para>測定停止中のみ変更可能です。</para></remarks>
        int BufferSize { get; set; }

        /// <summary>
        /// 計算結果用バッファの参照を取得します。
        /// </summary>
        WavefrontContainer CurrentBuffer { get; }

        /// <summary>
        /// 光路が複光路方式かどうかを取得・設定する。
        /// </summary>
        bool DoublePass { get; set; }

        /// <summary>
        /// 波面収差を自動的に計算するかどうかを取得・設定します。
        /// </summary>
        /// <remarks><see lang="true"/>の場合、画像取得後に波面収差の計算を行います。<see lang="false"/>の場合、画像取得後に波面収差の計算を行いません。 </remarks>
        bool EnableCalculation { get; set; }

        /// <summary>
        /// Legendre多項式係数を計算するかどうかを取得・設定します。
        /// </summary>
        bool EnableLegendre { get; set; }

        /// <summary>
        /// Zernike多項式係数を計算するかどうかを取得・設定します。
        /// </summary>
        bool EnableZernike { get; set; }

        /// <summary>
        /// 校正情報が適用されているかどうかを確認します。
        /// </summary>
        bool IsCalibrated { get; }

        /// <summary>
        /// デバイスが測定中かどうかを確認します。
        /// </summary>
        /// <remarks>測定中の場合は<see langword="true"/>。それ以外は<see langword="false"/>。</remarks>
        bool IsMeasuring { get; }

        /// <summary>
        /// デバイスが使用可能な状態かどうかを確認します。
        /// </summary>
        /// <remarks>使用可能な場合は<see langword="true"/>。それ以外は<see langword="false"/>。</remarks>
        bool IsOpened { get; }

        /// <summary>
        /// Legendre多項式次数（>= 0）
        /// </summary>
        int LegendreDegree { get; set; }

        /// <summary>
        /// マイクロレンズアレイの焦点距離[mm]
        /// </summary>
        double MicroLensArrayFocalLength { get; set; }

        /// <summary>
        /// マイクロレンズアレイのレンズ間距離[um]
        /// </summary>
        double MicroLensArrayPitch { get; set; }

        /// <summary>
        /// イメージセンサーのノイズレベルを取得・設定します。
        /// </summary>
        /// <remarks>
        /// <para>輝度値がノイズレベル以下のピクセルは計算対象外になります。</para>
        /// <para>設定可能な値の範囲は、取得画像のビット深度に依存します。</para></remarks>
        /// <seealso cref="ICameraConfig.BitDepth"/>
        int NoiseLevel { get; set; }

        /// <summary>
        /// PSFのパワースペクトルを規格化する。
        /// </summary>
        bool Normalization { get; set; }

        /// <summary>
        /// 光学倍率
        /// </summary>
        double OpticalMagnification { get; set; }

        /// <summary>
        /// ペアリング対象領域の拡大率
        /// </summary>
        double PairingAreaRate { get; set; }

        /// <summary>
        /// センサーカメラのピクセルサイズ[um]
        /// </summary>
        double PixelPitch { get; set; }

        /// <summary>
        /// 光源伝播距離[mm]
        /// </summary>
        /// <remarks>伝播距離が0以下の場合、平面波で校正されたとみなします。</remarks>
        double PropagationDistance { get; set; }

        /// <summary>
        /// 基準点座標リスト
        /// </summary>
        List<PointD> ReferencePoints { get; }

        /// <summary>
        /// PSFのサンプリンググリッド分割数。
        /// </summary>
        int SamplingGridDivision { get; set; }

        /// <summary>
        /// PSFの空間分解能[um]
        /// </summary>
        int SpatialResolution { get; }

        /// <summary>
        /// スポットの重心算出方法
        /// </summary>
        CentroidMethod SpotCentroidMode { get; set; }

        /// <summary>
        /// スポットとみなす光点の面積範囲を取得・設定します。
        /// </summary>
        Limit<int> SpotSizeRange { get; }

        /// <summary>
        /// 波長[nm]
        /// </summary>
        double Wavelength { get; set; }

        /// <summary>
        /// Frienge・Zernike指数（0～36）
        /// </summary>
        int ZernikeIndex { get; set; }
        #endregion // Properties

        #region Indexers
        /// <summary>
        /// 測定結果用バッファの参照用インデクサー
        /// </summary>
        /// <param name="index">バッファインデックス</param>
        /// <returns>指定された測定結果用バッファのデータ</returns>
        WavefrontContainer this[int index] { get; }
        #endregion // Indexers

        #region Methods
        /// <summary>
        /// 波面収差を計算するための校正情報を適用します。
        /// </summary>
        /// <param name="calibration">校正情報</param>
        void Calibration(CalibrationComponents calibration);

        /// <summary>
        /// 校正情報を初期化します。
        /// </summary>
        void ClearCalibration();

        /// <summary>
        /// デバイスの使用を停止します。
        /// </summary>
        void Close();

        /// <summary>
        /// センサーデバイス一覧を列挙します。
        /// </summary>
        /// <returns>センサーデバイス一覧</returns>
        IReadOnlyList<SensorComponents> EnumerateDevice();

        /// <summary>
        /// スポット画像の2次元強度マップを出力します。
        /// </summary>
        /// <param name="spotImage">スポット画像</param>
        /// <param name="mapSize">出力画像サイズ</param>
        /// <returns>2次元強度マップ</returns>
        ImageComponent IntensityMap(ImageComponent spotImage, Size<int> mapSize);

        /// <summary>
        /// デバイスの使用を開始します。
        /// </summary>
        void Open();

        /// <summary>
        /// デバイスを測定開始状態にします。
        /// </summary>
        void Start();

        /// <summary>
        /// デバイスの測定を停止します。
        /// </summary>
        void Stop();
        #endregion // Methods
    }
}