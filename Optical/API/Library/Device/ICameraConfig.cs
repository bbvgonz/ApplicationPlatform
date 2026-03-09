using Optical.Platform.Types;
using Optical.Enums;
using System;

namespace Optical.API.Library.Device
{
    /// <summary>
    /// カメラの設定を取得・更新するためのインターフェースを提供します。
    /// </summary>
    public interface ICameraConfig
    {
        #region Events
        /// <summary>
        /// 画像データが更新された場合にイベントが発生します。
        /// </summary>
        event EventHandler<int> NewFrame;
        #endregion // Events

        #region Properties
        /// <summary>
        /// 自動露光時間調整の動作可否を取得・設定します。
        /// </summary>
        bool AutoExposure { get; set; }
        /// <summary>
        /// 自動ゲイン調整調整の動作可否を取得・設定します。
        /// </summary>
        bool AutoGain { get; set; }
        /// <summary>
        /// ビニングする一辺のピクセル数を取得・設定します。
        /// </summary>
        int Binning { get; set; }
        /// <summary>
        /// ビニング時のピクセル値算出方法を取得・設定します。
        /// </summary>
        BinningPixelFormat BinningMode { get; set; }
        /// <summary>
        /// 画像データのビット深度を取得・設定します。
        /// </summary>
        int BitDepth { get; set; }
        /// <summary>
        /// イメージセンサーの黒レベルを取得・設定します。
        /// </summary>
        double BlackLevel { get; set; }
        /// <summary>
        /// デバイス情報を取得します。
        /// </summary>
        SensorComponents Device { get; }
        /// <summary>
        /// イメージセンサーの露光時間[ms]を取得・設定します。
        /// </summary>
        double ExposureTime { get; set; }
        /// <summary>
        /// 露光時間の設定範囲[ms]
        /// </summary>
        Limit<double> ExposureTimeRange { get; }
        /// <summary>
        /// 画像の水平方向のミラーリングを有効にします。
        /// </summary>
        /// <remarks>キャプチャした画像の各ラインのピクセル値は、ラインの中心を軸に端と端が入れ替わります。</remarks>
        bool FlipHorizontal { get; set; }
        /// <summary>
        /// 画像の垂直方向のミラーリングを有効にします。
        /// </summary>
        /// <remarks>撮影した画像の各行のピクセル値が、その行の中心を軸に端から端まで入れ替わります。</remarks>
        bool FlipVertical { get; set; }
        /// <summary>
        /// 画像データの更新周期[Hz]を設定します。
        /// </summary>
        double FrameRate { get; set; }
        /// <summary>
        /// 画像データの更新周期の設定範囲[Hz]
        /// </summary>
        Limit<double> FrameRateRange { get; }
        /// <summary>
        /// イメージセンサーのゲイン[dB]を取得・設定します。
        /// </summary>
        double Gain { get; set; }
        /// <summary>
        /// ゲインの設定範囲[dB]
        /// </summary>
        Limit<double> GainRange { get; }
        /// <summary>
        /// イメージセンサーの画面解像度[pixel]を取得します。
        /// </summary>
        Size<int> SensorSize { get; }
        /// <summary>
        /// トリガー受信後、トリガーがアクティブになるまでの時間を取得・設定します。[ms]
        /// </summary>
        double TriggerDelay { get; set; }
        /// <summary>
        /// トリガーモードで動作するどうかを示す値を取得・設定します。
        /// </summary>
        /// <remarks><see langword="true"/>:トリガーモード、<see langword="false"/>:フリーランモード
        /// 初期値:<see langword="false"/></remarks>
        /// <seealso cref="TriggerType"/>
        bool TriggerMode { get; set; }
        /// <summary>
        /// トリガーモード動作時の入力タイプを取得・設定します。
        /// </summary>
        /// <remarks>初期値:<see cref="TriggerInput.Hardware"/></remarks>
        /// <seealso cref="TriggerMode"/> 
        TriggerInput TriggerType { get; set; }
        #endregion // Properties
    }
}
