using Optical.Enums;
using Optical.Platform.Types;
using System;
using System.Collections.Generic;

namespace Optical.Core.Camera
{
    /// <summary>
    /// ラッパーの基底クラス。
    /// 一部パラメータは、接続形式毎に実装しなければならない。
    /// </summary>
    internal abstract class SampleCameraWrapper : IDisposable
    {
        #region Fields
        #endregion // Fields

        #region Constructors
        public SampleCameraWrapper() { }

        public SampleCameraWrapper(string serialNumber) { }
        #endregion // Constructors

        #region Events
        public event EventHandler<ImageContainer> NewFrame = delegate { };
        #endregion // Events

        #region Properties
        /// <summary>
        /// カメラ使用可否
        /// </summary>
        public bool IsOpen { get; init; }

        /// <summary>
        /// 画像取得中
        /// </summary>
        public bool IsGrabbing { get; init; }

        /// <summary>
        /// トリガーパルス検出方法
        /// </summary>
        public TriggerEdgeDetection TriggerEdge { get; set; }

        /// <summary>
        /// トリガーモード動作状態
        /// </summary>
        /// <remarks>true:ON, false:OFF</remarks>
        public bool TriggerMode { get; set; }

        /// <summary>
        /// トリガー入力形式
        /// </summary>
        public TriggerInput TriggerType { get; set; }

        /// <summary>
        /// カメラ縦幅[pixels]
        /// </summary>
        public int Height { get; init; }

        /// <summary>
        /// カメラ横幅[pixels]
        /// </summary>
        public int Width { get; init; }

        /// <summary>
        /// 画像の中央揃えを有効にします。
        /// </summary>
        public abstract bool AutoCenter { get; set; }

        /// <summary>
        /// 自動露光時間調整設定
        /// </summary>
        /// <remarks>true:自動調整ON、false:自動調整OFF</remarks>
        public abstract bool AutoExposure { get; set; }

        /// <summary>
        /// 露光時間自動調整時の下限値[ms]
        /// </summary>
        public abstract double AutoExposureLower { get; set; }

        /// <summary>
        /// 露光時間を自動調整する時の下限値の設定範囲[ms]
        /// </summary>
        public abstract Limit<double> AutoExposureLowerRange { get; init; }

        /// <summary>
        /// 露光時間自動調整時の上限値[ms]
        /// </summary>
        public abstract double AutoExposureUpper { get; set; }

        /// <summary>
        /// 露光時間を自動調整する時の上限値の設定範囲[ms]
        /// </summary>
        public abstract Limit<double> AutoExposureUpperRange { get; init; }

        /// <summary>
        /// 自動ゲイン調整設定
        /// </summary>
        /// <remarks>true:自動調整ON、false:自動調整OFF</remarks>
        public abstract bool AutoGain { get; set; }

        /// <summary>
        /// ゲイン自動調整時の下限値[dB]
        /// </summary>
        public abstract double AutoGainLower { get; set; }

        /// <summary>
        /// ゲイン自動調整時の下限値の設定範囲[dB]
        /// </summary>
        public abstract Limit<double> AutoGainLowerRange { get; init; }

        /// <summary>
        /// ゲイン自動調整時の上限値[dB]
        /// </summary>
        public abstract double AutoGainUpper { get; set; }

        /// <summary>
        /// ゲイン自動調整時の上限値の設定範囲[dB]
        /// </summary>
        public abstract Limit<double> AutoGainUpperRange { get; init; }

        /// <summary>
        /// ビニングピクセル数
        /// </summary>
        public abstract int Binning { get; set; }

        /// <summary>
        /// ビニングピクセル値算出方法
        /// </summary>
        public abstract BinningPixelFormat BinningMode { get; set; }

        /// <summary>
        /// ビット深度[bpp]
        /// </summary>
        public abstract int BitDepth { get; set; }

        /// <summary>
        /// 黒レベル
        /// </summary>
        public abstract double BlackLevel { get; set; }

        /// <summary>
        /// 露光時間[ms]
        /// </summary>
        public abstract double ExposureTime { get; set; }

        /// <summary>
        /// 露光時間の設定範囲[ms]
        /// </summary>
        public abstract Limit<double> ExposureTimeRange { get; init; }

        /// <summary>
        /// 画像の水平方向のミラーリングを有効にします。
        /// </summary>
        /// <remarks>キャプチャした画像の各ラインのピクセル値は、ラインの中心を軸に端と端が入れ替わります。</remarks>
        public abstract bool FlipHorizontal { get; set; }

        /// <summary>
        /// 画像の垂直方向のミラーリングを有効にします。
        /// </summary>
        /// <remarks>撮影した画像の各行のピクセル値が、その行の中心を軸に端から端まで入れ替わります。</remarks>
        public abstract bool FlipVertical { get; set; }

        /// <summary>
        /// カメラの画像更新レート[fps]
        /// </summary>
        /// <seealso cref="FrameRateEnabled"/>
        public abstract double FrameRate { get; set; }

        /// <summary>
        /// 指定された画像更新レートの使用可否
        /// </summary>
        /// <remarks><see langword="true"/>が設定された場合、<see cref="FrameRate"/>に設定された値が有効になります。</remarks>
        /// <seealso cref="FrameRate"/>
        public abstract bool FrameRateEnabled { get; set; }

        /// <summary>
        /// 画像更新レートの設定範囲[fps]
        /// </summary>
        public abstract Limit<double> FrameRateRange { get; init; }

        /// <summary>
        /// ゲイン[dB]
        /// </summary>
        public abstract double Gain { get; set; }

        /// <summary>
        /// ゲインの設定範囲[dB]
        /// </summary>
        public abstract Limit<double> GainRange { get; init; }

        /// <summary>
        /// カメラが送信するピクセルデータのフォーマット
        /// </summary>
        public abstract string PixelFormat { get; set; }

        /// <summary>
        /// ピクセルデータの有効なフォーマットリスト
        /// </summary>
        public abstract string[] PixelFormatList { get; init; }

        /// <summary>
        /// トリガー遅延時間[ms]
        /// </summary>
        public abstract double TriggerDelay { get; set; }
        #endregion // Properties

        #region Methods
        /// <summary>
        /// デバイス一覧を出力します。
        /// </summary>
        /// <param name="className">カメラインスタンスを生成するクラスの<see cref="Type.AssemblyQualifiedName"/></param>
        /// <returns></returns>
        public static IReadOnlyList<CameraComponents> EnumerateDevice(string className)
        {
            var devices = new List<CameraComponents>();
            return devices.AsReadOnly();
        }

        /// <summary>
        /// カメラとの接続を閉じます。
        /// </summary>
        public void Close() { }

        /// <summary>
        /// デバイスを初期化します。
        /// </summary>
        public abstract void Initialize();

        /// <summary>
        /// カメラとの接続を確立します。
        /// </summary>
        /// <param name="timeout">タイムアウト時間[ms] (初期値:10秒)</param>
        public void Open(int timeout = 10000) { }

        /// <summary>
        /// 測定を開始します。
        /// </summary>
        public void Start() { }

        /// <summary>
        /// 測定を停止します。
        /// </summary>
        public void Stop() { }

        /// <summary>
        /// スナップショットを取得します。
        /// </summary>
        /// <remarks><see cref="TriggerType"/>が<see cref="TriggerInput.Software"/>の場合のみ動作します。</remarks>
        public void TakeSnapshot() { }

        public void Dispose() { }
        #endregion // Methods
    }
}
