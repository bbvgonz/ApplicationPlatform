using Optical.Enums;
using Optical.Platform.Types;
using System;
using System.Collections.Generic;


namespace Optical.Core.Camera
{
    internal interface ICameraSearch
    {
        /// <summary>
        /// デバイスの一覧を列挙します。
        /// </summary>
        /// <returns>接続デバイス情報一覧</returns>
        IReadOnlyList<CameraComponents> EnumerateDevice();
    }

    /// <summary>
    /// カメラデバイスの制御方法を表します。
    /// </summary>
	public interface ISensorCamera
    {
        #region Events
        /// <summary>
        /// 画像データが更新された場合に発生するイベント。
        /// </summary>
        event EventHandler<ImageContainer> NewFrame;
        #endregion // Events

        #region Properties
        /// <summary>
        /// 画像の中央揃えを有効にします。
        /// </summary>
        bool AutoCenter { get; set; }

        /// <summary>
        /// 自動露光時間調整
        /// </summary>
        bool AutoExposure { get; set; }

        /// <summary>
        /// 露光時間自動調整時の下限値[ms]
        /// </summary>
        double AutoExposureLower { get; set; }

        /// <summary>
        /// 露光時間を自動調整する時の下限値の設定範囲[ms]
        /// </summary>
        Limit<double> AutoExposureLowerRange { get; }

        /// <summary>
        /// 露光時間自動調整時の上限値[ms]
        /// </summary>
        double AutoExposureUpper { get; set; }

        /// <summary>
        /// 露光時間を自動調整する時の上限値の設定範囲[ms]
        /// </summary>
        Limit<double> AutoExposureUpperRange { get; }

        /// <summary>
        /// 自動ゲイン調整
        /// </summary>
        bool AutoGain { get; set; }

        /// <summary>
        /// ゲイン自動調整時の下限値[dB]
        /// </summary>
        double AutoGainLower { get; set; }

        /// <summary>
        /// ゲイン自動調整時の下限値の設定範囲[dB]
        /// </summary>
        Limit<double> AutoGainLowerRange { get; }

        /// <summary>
        /// ゲイン自動調整時の上限値[dB]
        /// </summary>
        double AutoGainUpper { get; set; }

        /// <summary>
        /// ゲイン自動調整時の上限値の設定範囲[dB]
        /// </summary>
        Limit<double> AutoGainUpperRange { get; }

        /// <summary>
        /// ビニングピクセル数
        /// </summary>
        /// <remarks>一辺のピクセル数(N)を取得・設定する。ビニングの範囲は(N*N)。</remarks>
        /// <seealso cref="BinningMode"/>
        int Binning { get; set; }

        /// <summary>
        /// ビニング時のピクセル値算出方法
        /// </summary>
        /// <seealso cref="Binning"/>
        BinningPixelFormat BinningMode { get; set; }

        /// <summary>
        /// 画像の1ピクセル辺りのビット数
        /// </summary>
        int BitDepth { get; set; }

        /// <summary>
        /// カメラの黒レベル
        /// </summary>
        double BlackLevel { get; set; }

        /// <summary>
        /// 露光時間[ms]
        /// </summary>
        /// <seealso cref="ExposureTimeRange"/>
        double ExposureTime { get; set; }

        /// <summary>
        /// 露光時間の設定範囲[ms]
        /// </summary>
        /// <seealso cref="ExposureTime"/>
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
        /// カメラの画像更新レート[fps]
        /// </summary>
        /// <seealso cref="FrameRateEnabled"/>
        double FrameRate { get; set; }

        /// <summary>
        /// 指定された画像更新レートの使用可否
        /// </summary>
        /// <remarks><see langword="true"/>が設定された場合、<see cref="FrameRate"/>に設定された値が有効になります。</remarks>
        /// <seealso cref="FrameRate"/>
        bool FrameRateEnabled { get; set; }

        /// <summary>
        /// 画像更新レートの設定範囲[fps]
        /// </summary>
        Limit<double> FrameRateRange { get; }

        /// <summary>
        /// ゲイン[dB]
        /// </summary>
        /// <seealso cref="GainRange"/>
        double Gain { get; set; }

        /// <summary>
        /// ゲインの設定範囲[dB]
        /// </summary>
        /// <seealso cref="Gain"/>
        Limit<double> GainRange { get; }

        /// <summary>
        /// カメラの縦幅[pixels]
        /// </summary>
        int Height { get; }

        /// <summary>
        /// 画像取得中状態を取得します。
        /// </summary>
        /// <seealso cref="Start"/>
        bool IsGrabbing { get; }

        /// <summary>
        /// カメラデバイスが使用可能かどうかを示す値を取得します。
        /// </summary>
        /// <seealso cref="Open"/>
        bool IsOpened { get; }

        /// <summary>
        /// カメラが送信するピクセルデータのフォーマット
        /// </summary>
        /// <remarks>有効なピクセルデータフォーマットは<see cref="PixelFormatList"/>で取得します。</remarks>
        /// <seealso cref="PixelFormatList"/>
        string PixelFormat { get; set; }

        /// <summary>
        /// ピクセルデータの有効なフォーマットリスト
        /// </summary>
        /// <seealso cref="PixelFormat"/>
        string[] PixelFormatList { get; }

        /// <summary>
        /// トリガー受信後、トリガーがアクティブになるまでの時間を取得・設定します。[ms]
        /// </summary>
        double TriggerDelay { get; set; }

        /// <summary>
        /// トリガーモードで動作するかどうかを示す値を取得します。
        /// </summary>
        /// <remarks>
        /// <para><see langword="true"/>:トリガーモード動作, <see langword="false"/>:フリーランモード動作</para>
        /// <para>(初期値:<see langword="false"/>)</para>
        /// </remarks>
        bool TriggerMode { get; set; }

        /// <summary>
        /// トリガーモード動作時の入力タイプを取得・設定します。
        /// </summary>
        /// <remarks>初期値:<see cref="TriggerInput.Hardware"/></remarks>
        /// <seealso cref="TriggerMode"/>
        TriggerInput TriggerType { get; set; }

        /// <summary>
        /// カメラの横幅[pixels]
        /// </summary>
        int Width { get; }
        #endregion // Properties

        #region Methods
        /// <summary>
        /// デバイスの利用を停止します。
        /// </summary>
        void Close();

        /// <summary>
        /// デバイスを利用可能な状態にします。
        /// </summary>
        void Open();

        /// <summary>
        /// 画像の取り込みを開始します。
        /// </summary>
        void Start();

        /// <summary>
        /// 画像の取り込みを停止します。
        /// </summary>
        void Stop();

        /// <summary>
        /// ソフトウェアトリガーによるキャプチャー処理を行います。
        /// </summary>
        /// <remarks><see cref="TriggerMode"/>が<see langword="true"/>の場合のみ動作可能です。</remarks>
		void TakeSnapshot();
        #endregion // Methods
    }
}
