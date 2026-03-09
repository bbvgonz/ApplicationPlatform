using Optical.API.Library.Device;
using Optical.API.Library.Optics;
using Optical.Enums;
using Optical.Platform.Types;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Optical.API.Beams.Sample
{
    /// <summary>
    /// BeamProfiler機能を提供します。
    /// </summary>
    public class BeamProfiler : IBeamProfiler
    {
        #region Fields
        private BeamProfilerLogic logic;
        #endregion // Fields

        #region Constructors
        /// <summary>
        /// センサーの識別情報に従い、新しいインスタンスを生成します。
        /// </summary>
        /// <param name="component">センサー識別情報</param>
        public BeamProfiler(SensorComponents component)
        {
            logic = new BeamProfilerLogic(component);
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
        #endregion

        #region Properties
        /// <summary>
        /// ビーム径計算の実行をスケジュールできるプロセッサを取得または設定します
        /// </summary>
        public ProcessorId AffinityMask
        {
            get => logic.AffinityMask;
            set => logic.AffinityMask = value;
        }

        /// <summary>
        /// プロセッサーの関係性（<see cref="AffinityMask"/>）の有効状態を取得・設定します。
        /// </summary>
        public bool AffinityMaskEnabled
        {
            get => logic.AffinityMaskEnabled;
            set => logic.AffinityMaskEnabled = value;
        }

        /// <summary>
        /// スポット開口の自動計算可否を示す値を取得・設定します。
        /// </summary>
        /// <seealso cref="AutoApertureRate"/>
        public bool AutoApertureEnabled
        {
            get => logic.AutoApertureEnabled;
            set => logic.AutoApertureEnabled = value;
        }

        /// <summary>
        /// スポット開口の自動計算方式を取得・設定します。
        /// </summary>
        /// <remarks>Only <see cref="BeamwidthType.KnifeEdge"/></remarks>
        public BeamwidthType AutoApertureMethod
        {
            get => logic.AutoApertureMethod;
            set
            {
                switch (value)
                {
                    case BeamwidthType.KnifeEdge:
                        logic.AutoApertureMethod = value;
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// スポット開口の自動計算結果に対する倍率を取得・設定します。
        /// </summary>
        /// <seealso cref="AutoApertureEnabled"/>
        public double AutoApertureRate
        {
            get => logic.AutoApertureRate;
            set => logic.AutoApertureRate = value;
        }

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
        /// <seealso cref="BinningMode"/>
        public int Binning
        {
            get => logic.Binning;
            set => logic.Binning = value;
        }

        /// <summary>
        /// ビニング時のピクセル値算出方法を取得・設定します。
        /// </summary>
        /// <seealso cref="Binning"/>
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
        /// オートアパーチャー算出時のビーム重心計算方法を取得・設定します。
        /// </summary>
        /// <remarks><see cref="AutoApertureEnabled"/>が<see langword="true"/>の場合のみ有効。</remarks>
        public CentroidMethod AutoApertureCalculationType
        {
            get => logic.AutoApertureCalculationType;
            set => logic.AutoApertureCalculationType = value;
        }

        /// <summary>
        /// Centroid の計算方法を取得・設定します。
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
        public BeamProfileContainer CurrentBuffer => logic.FrameBuffer;

        /// <summary>
        /// デバイス情報を取得します。
        /// </summary>
        public SensorComponents Device => logic.Device;

        /// <summary>
        /// スポット径を自動計算するかどうかを取得・設定します。
        /// </summary>
        /// <remarks><see lang="true"/>のときは画像取得後にスポット計算を行い、<see lang="false"/>のときには画像取得後にスポット計算を行いません。 </remarks>
        public bool EnableCalculation
        {
            get => logic.EnableCalculation;
            set => logic.EnableCalculation = value;
        }

        /// <summary>
        /// 計算をするスポット径の種類を取得・設定します。
        /// </summary>
        public BeamwidthType EnableDiameter
        {
            get => logic.EnableDiameter;
            set => logic.EnableDiameter = value;
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
        /// デバイスが校正済みかどうかを確認します。
        /// </summary>
        public bool IsCalibrated => logic.IsCalibrated;

        /// <summary>
        /// デバイスが使用可能な状態かどうかを確認します。
        /// </summary>
        public bool IsOpened => logic.IsOpened;

        /// <summary>
        /// デバイスが測定中かどうかを確認します。
        /// </summary>
        public bool IsMeasuring => logic.IsMeasuring;

        /// <summary>
        /// 楕円ビームの向きを考慮するかどうかを示す値を取得・設定します。
        /// </summary>
        public bool Orientation
        {
            get => logic.Orientation;
            set => logic.Orientation = value;
        }

        /// <summary>
        /// 対象領域の範囲[pixel]を取得・設定します。
        /// </summary>
        public List<Rectangle> RoiList => logic.RoiList;

        /// <summary>
        /// 対象領域の開口形状を取得・設定します。
        /// </summary>
        public ApertureShape RoiShape
        {
            get => logic.RoiShape;
            set => logic.RoiShape = value;
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
        #endregion // Properties

        #region Indexers
        /// <summary>
        /// 測定結果バッファ参照用インデクサー
        /// </summary>
        /// <param name="index">バッファインデックス</param>
        /// <returns>指定された測定結果のバッファデータ</returns>
        public BeamProfileContainer this[int index] => logic.FrameBuffers[index];
        #endregion

        #region Methods
        // Interfaceがstaticに対応していないため、エラー回避用。
        // C#8.0以降、Interfaceがstaticに対応したら削除。
        IReadOnlyList<SensorComponents> IBeamProfiler.EnumerateDevice()
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
        /// センサーデバイス一覧を列挙します。
        /// </summary>
        /// <returns>センサーデバイス一覧</returns>
        public static IReadOnlyList<SensorComponents> EnumerateDevice()
        {
            return BeamProfilerLogic.EnumerateDevice();
        }

        /// <summary>
        /// 取得した画像から輝度のベースラインを自動で設定する。
        /// </summary>
        /// <exception cref="OperationCanceledException">入射光が強すぎます。</exception>
        /// <exception cref="TimeoutException">センサーからの応答がありません。</exception>
        public void AdaptiveCalibration()
        {
            try
            {
                logic.AdaptiveCalibration();
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (TimeoutException)
            {
                throw;
            }
        }

        /// <summary>
        /// 指定されたFrameBuffer内のビーム画像を現状のパラメーターで再計算する。
        /// </summary>
        /// <param name="startIndex">再計算するFrame Bufferの開始インデックス</param>
        /// <param name="count">再計算するバッファの個数</param>
        public void BufferRecalculation(int startIndex, int count)
        {
            logic.BufferRecalculation(startIndex, count);
        }

        /// <summary>
        /// 輝度ベースラインの校正状態を初期化する。
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
