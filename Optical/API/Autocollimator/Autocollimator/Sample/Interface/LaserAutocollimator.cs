using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Optical.API.Library.Device;
using Optical.API.Library.Optics;
using Optical.Enums;
using Optical.Platform.Types;

namespace Optical.API.Autocollimator.Sample
{
    /// <summary>
    /// LaserAutocollimator機能を提供します。
    /// </summary>
    public class LaserAutocollimator : IAutocollimator
    {
        #region Fields
        private static readonly LaserAutocollimator empty;
        private readonly LaserAutocollimatorLogic logic;
        #endregion // Fields

        #region Constructors
        static LaserAutocollimator()
        {
            empty = new LaserAutocollimator();
        }

        private LaserAutocollimator() { }

        /// <summary>
        /// <see cref="LaserAutocollimator"/>クラスの新しいインスタンスを生成します。
        /// </summary>
        /// <param name="component">センサー識別情報</param>
        public LaserAutocollimator(SensorComponents component)
        {
            logic = new LaserAutocollimatorLogic(component);
        }
        #endregion // Constructors

        #region Events
        /// <summary>
        /// 画像データが更新された場合にイベントが発生します。
        /// </summary>
        public event EventHandler<int> NewFrame
        {
            add { logic.NewFrame += value; }
            remove { logic.NewFrame -= value; }
        }

        /// <summary>
        /// 計算結果が更新された場合にイベントが発生します。
        /// </summary>
        public event EventHandler<int> NewResult
        {
            add { logic.NewResult += value; }
            remove { logic.NewResult -= value; }
        }
        #endregion // Events

        #region Properties
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
        /// 移動平均の平均化回数を取得・設定します。
        /// </summary>
        public int AveragingTimes
        {
            get => logic.AveragingTimes;
            set => logic.AveragingTimes = value;
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
        /// 画像データのビット深度を取得・設定します。
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
        /// 重心計算方法を取得・設定します。
        /// </summary>
        public CentroidMethod CentroidType
        {
            get => logic.CentroidType;
            set => logic.CentroidType = value;
        }

        /// <summary>
        /// <see cref="BufferIndex"/>で指定された測定結果バッファの参照を取得します。
        /// </summary>
        /// <remarks><see cref="BufferIndex"/>で指定されたBufferList(<see cref="this[int]"/>)内の要素を参照します。</remarks>
        /// <seealso cref="BufferIndex"/>
        /// <seealso cref="BufferSize"/>
        public TiltContainer CurrentBuffer => logic.FrameBuffer;

        /// <summary>
        /// 初期校正値の分割点数
        /// </summary>
        public int DivisionPoints
        {
            get => logic.DivisionPoints;
            set => logic.DivisionPoints = value;
        }

        /// <summary>
        /// ビーム検出閾値を取得・設定します。
        /// </summary>
        public int DetectionThreshold
        {
            get => logic.DetectionThreshold;
            set => logic.DetectionThreshold = value;
        }

        /// <summary>
        /// デバイス情報を取得します。
        /// </summary>
        public SensorComponents Device { get; } = new();

        /// <summary>
        /// デバイスIDを取得します。
        /// </summary>
        public string DeviceId => logic.DeviceId;

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
        /// 画像データの更新周期[Hz]を取得・設定します。
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
        /// 内部光源を使用するかどうかを示す値を取得・設定します。
        /// </summary>
        /// <remarks>true:内部光源ON、false:内部光源OFF</remarks>
        public bool InternalLightSource
        {
            get => logic.InternalLightSource;
            set => logic.InternalLightSource = value;
        }

        /// <summary>
        /// デバイスが測定中かどうかを確認します。
        /// </summary>
        public bool IsMeasuring => logic.IsMeasuring;

        /// <summary>
        /// デバイスが使用可能な状態かどうかを確認します。
        /// </summary>
        public bool IsOpened => logic.IsOpened;

        /// <summary>
        /// 最新の測定結果が格納されたバッファの参照を取得します。
        /// </summary>
        public TiltContainer LatestBuffer { get; } = new();

        /// <summary>
        /// 複数の光点を個別に測定するかどうかを示す値を取得・設定します。
        /// </summary>
        public bool MultiMode { get; set; }

        /// <summary>
        /// イメージセンサーのノイズレベルを取得・設定します。
        /// </summary>
        /// <remarks>設定可能な値の範囲は、取得画像のビット深度、及び<see cref="Binning"/>に依存します。</remarks>
        /// <seealso cref="Binning"/>
        /// <seealso cref="BitDepth"/>
        public int NoiseLevel
        {
            get => logic.NoiseLevel;
            set => logic.NoiseLevel = value;
        }

        /// <summary>
        /// 対象領域の範囲[pixel]を取得・設定します。
        /// </summary>
        /// <remarks>各対象領域に対し、<see cref="MultiMode"/>は適用されません。</remarks>
        public List<Rectangle> Roi { get; set; } = [];

        /// <summary>
        /// イメージセンサーの画面解像度[pixel]を取得します。
        /// </summary>
        public Size<int> SensorSize => logic.SensorSize;

        /// <summary>
        /// 最新の測定角度[deg]を取得します。
        /// </summary>
        /// <remarks>
        /// <para>測定された角度は、原点からの距離が近い順に格納されます。</para>
        /// <para>ただし、<see cref="Roi"/>が設定されている場合は、各<see cref="Roi"/>の測定結果が同じ順で格納されます。</para>
        /// </remarks>
        public List<AngleD> Tilt { get; } = [];

        /// <summary>
        /// 測定範囲を取得・設定します。±[deg]
        /// </summary>
        public double TiltRange
        {
            get => logic.TiltRange;
            set => logic.TiltRange = value;
        }

        /// <summary>
        /// トリガー受信後、トリガーがアクティブになるまでの時間を取得・設定します。[ms]
        /// </summary>
        public double TriggerDelay { get; set; }

        /// <summary>
        /// トリガーモードで動作するどうかを示す値を取得・設定します。
        /// </summary>
        /// <remarks><see langword="true"/>:トリガーモード、<see langword="false"/>:フリーランモード
        /// 初期値:<see langword="false"/></remarks>
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

        /// <summary>
        /// ワークディスタンスを取得・設定します。[um]
        /// </summary>
        public double WorkDistance
        {
            get => logic.WorkDistance;
            set => logic.WorkDistance = value;
        }
        #endregion // Properties

        #region Indexers
        /// <summary>
        /// 測定結果バッファ参照用インデクサー
        /// </summary>
        /// <param name="index">バッファインデックス</param>
        /// <returns>指定された測定結果のバッファデータ</returns>
        public TiltContainer this[int index] => logic.FrameBuffers[index];
        #endregion

        #region Methods
        // Interfaceがstaticに対応していないため、エラー回避用。
        // C#8.0以降、Interfaceがstaticに対応したら削除。
        IReadOnlyList<SensorComponents> IAutocollimator.EnumerateDevice()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// センサーデバイス一覧を列挙します。
        /// </summary>
        /// <returns>センサーデバイス一覧</returns>
        public static IReadOnlyList<SensorComponents> EnumerateDevice()
        {
            return LaserAutocollimatorLogic.EnumerateDevice();
        }

        /// <summary>
        /// 校正データを削除します。
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
        /// デバイスの使用を開始します。
        /// </summary>
        public void Open()
        {
            logic.Open();
        }

        /// <summary>
        /// 校正ファイルを読み込みます。
        /// </summary>
        /// <param name="filePath">校正ファイルパス</param>
        public void ReadCalibrationFile(string filePath)
        {
            logic.ReadCalibrationFile(filePath);
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
