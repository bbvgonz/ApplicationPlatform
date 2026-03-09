using Optical.Enums;
using Optical.Platform.Types;
using System;

namespace Optical.Core.Camera
{
    /// <summary>
    /// カメラの制御機能を提供します。
    /// </summary>
    public class SampleCamera : ISensorCamera, IDisposable
    {
        #region Fields
        private SampleCameraWrapper pylon;
        #endregion // Fields

        #region Constructors
        /// <summary>
        /// Basler製カメラを制御するための新しいインスタンスを生成します。
        /// </summary>
        /// <param name="device">カメラデバイス情報</param>
        /// <seealso cref="CameraComponents"/>
        public SampleCamera(CameraComponents device)
        {
            if (device == null)
            {
                throw new ArgumentNullException(nameof(device));
            }

            switch (device.ConnectionType)
            {
                case Connection.Terminal.CameraLink:
                    throw new NotImplementedException("カメラリンク接続には対応していません。");
                case Connection.Terminal.IEEE1394:
                    throw new NotImplementedException("IEEE1394接続には対応していません。");
                case Connection.Terminal.GigE:
                    pylon = new EtherSampleCameraWrapper(device.SerialNumber);
                    break;
                case Connection.Terminal.Usb:
                    pylon = new UsbSampleCameraWrapper(device.SerialNumber);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(device.ConnectionType), "不明な接続形式です。");
            }
        }
        #endregion // Constructors

        #region Events
        /// <summary>
        /// 新しい画像を取得したときに発生します。
        /// </summary>
        public event EventHandler<ImageContainer> NewFrame
        {
            add { pylon.NewFrame += value; }
            remove { pylon.NewFrame -= value; }
        }
        #endregion // Events

        #region Properties
        /// <summary>
        /// 画像の中央揃えを有効にします。
        /// </summary>
        public bool AutoCenter
        {
            get => pylon.AutoCenter;
            set => pylon.AutoCenter = value;
        }

        /// <summary>
        /// 自動露光時間調整
        /// </summary>
        public bool AutoExposure
        {
            get => pylon.AutoExposure;
            set => pylon.AutoExposure = value;
        }

        /// <summary>
        /// 露光時間自動調整時の下限値[ms]
        /// </summary>
        /// <seealso cref="AutoExposure"/>
        /// <seealso cref="AutoExposureLowerRange"/>
        public double AutoExposureLower
        {
            get => pylon.AutoExposureLower;
            set => pylon.AutoExposureLower = value;
        }

        /// <summary>
        /// 露光時間を自動調整する時の下限値の設定範囲[ms]
        /// </summary>
        /// <seealso cref="AutoExposureLower"/>
        public Limit<double> AutoExposureLowerRange => pylon.AutoExposureLowerRange;

        /// <summary>
        /// 露光時間自動調整時の上限値[ms]
        /// </summary>
        /// <seealso cref="AutoExposure"/>
        /// <seealso cref="AutoExposureUpperRange"/>
        public double AutoExposureUpper
        {
            get => pylon.AutoExposureUpper;
            set => pylon.AutoExposureUpper = value;
        }

        /// <summary>
        /// 露光時間を自動調整する時の上限値の設定範囲[ms]
        /// </summary>
        /// <seealso cref="AutoExposureUpper"/>
        public Limit<double> AutoExposureUpperRange => pylon.AutoExposureUpperRange;

        /// <summary>
        /// 自動ゲイン調整
        /// </summary>
        public bool AutoGain
        {
            get => pylon.AutoGain;
            set => pylon.AutoGain = value;
        }

        /// <summary>
        /// ゲイン自動調整時の下限値[dB]
        /// </summary>
        /// <seealso cref="AutoGain"/>
        /// <seealso cref="AutoGainLowerRange"/>
        public double AutoGainLower
        {
            get => pylon.AutoGainLower;
            set => pylon.AutoGainLower = value;
        }

        /// <summary>
        /// ゲイン自動調整時の下限値の設定範囲[dB]
        /// </summary>
        /// <seealso cref="AutoGainLower"/>
        public Limit<double> AutoGainLowerRange => pylon.AutoGainLowerRange;

        /// <summary>
        /// ゲイン自動調整時の上限値[dB]
        /// </summary>
        /// <seealso cref="AutoGain"/>
        /// <seealso cref="AutoGainUpperRange"/>
        public double AutoGainUpper
        {
            get => pylon.AutoGainUpper;
            set => pylon.AutoGainUpper = value;
        }

        /// <summary>
        /// ゲイン自動調整時の上限値の設定範囲[dB]
        /// </summary>
        /// <seealso cref="AutoGainUpper"/>
        public Limit<double> AutoGainUpperRange => pylon.AutoGainUpperRange;

        /// <summary>
        /// ビニングピクセル数
        /// </summary>
        /// <seealso cref="BinningMode"/>
        public int Binning
        {
            get => pylon.Binning;
            set => pylon.Binning = value;
        }

        /// <summary>
        /// ビニングピクセル値算出方法
        /// </summary>
        /// <seealso cref="Binning"/>
        public BinningPixelFormat BinningMode
        {
            get => pylon.BinningMode;
            set => pylon.BinningMode = value;
        }

        /// <summary>
        /// ビット深度[bpp]
        /// </summary>
        public int BitDepth
        {
            get => pylon.BitDepth;
            set => pylon.BitDepth = value;
        }

        /// <summary>
        /// 黒レベル
        /// </summary>
        public double BlackLevel
        {
            get => pylon.BlackLevel;
            set => pylon.BlackLevel = value;
        }

        /// <summary>
        /// 露光時間[ms]
        /// </summary>
        /// <seealso cref="ExposureTimeRange"/>
        public double ExposureTime
        {
            get => pylon.ExposureTime;
            set => pylon.ExposureTime = value;
        }

        /// <summary>
        /// 露光時間の設定範囲[ms]
        /// </summary>
        /// <seealso cref="ExposureTime"/>
        public Limit<double> ExposureTimeRange => pylon.ExposureTimeRange;

        /// <summary>
        /// 画像の水平方向のミラーリングを有効にします。
        /// </summary>
        /// <remarks>キャプチャした画像の各ラインのピクセル値は、ラインの中心を軸に端と端が入れ替わります。</remarks>
        public bool FlipHorizontal
        {
            get => pylon.FlipHorizontal;
            set => pylon.FlipHorizontal = value;
        }

        /// <summary>
        /// 画像の垂直方向のミラーリングを有効にします。
        /// </summary>
        /// <remarks>撮影した画像の各行のピクセル値が、その行の中心を軸に端から端まで入れ替わります。</remarks>
        public bool FlipVertical
        {
            get => pylon.FlipVertical;
            set => pylon.FlipVertical = value;
        }

        /// <summary>
        /// カメラの画像更新レート[fps]
        /// </summary>
        /// <seealso cref="FrameRateEnabled"/>
        public double FrameRate
        {
            get => pylon.FrameRate;
            set => pylon.FrameRate = value;
        }

        /// <summary>
        /// 指定された画像更新レートの使用可否
        /// </summary>
        /// <remarks><see langword="true"/>が設定された場合、<see cref="FrameRate"/>に設定された値が有効になります。</remarks>
        /// <seealso cref="FrameRate"/>
        public bool FrameRateEnabled
        {
            get => pylon.FrameRateEnabled;
            set => pylon.FrameRateEnabled = value;
        }

        /// <summary>
        /// 画像更新レートの設定範囲[fps]
        /// </summary>
        public Limit<double> FrameRateRange => pylon.FrameRateRange;

        /// <summary>
        /// ゲイン[dB]
        /// </summary>
        /// <seealso cref="GainRange"/>
        public double Gain
        {
            get => pylon.Gain;
            set => pylon.Gain = value;
        }

        /// <summary>
        /// ゲインの設定範囲[dB]
        /// </summary>
        /// <seealso cref="Gain"/>
        public Limit<double> GainRange => pylon.GainRange;

        /// <summary>
        /// カメラの縦幅[pixels]
        /// </summary>
        public int Height => pylon.Height;

        /// <summary>
        /// 画像取得中状態を取得します。
        /// </summary>
        public bool IsGrabbing => pylon.IsGrabbing;

        /// <summary>
        /// カメラデバイスが使用可能かどうかを示す値を取得します。
        /// </summary>
        /// <remarks></remarks>
        public bool IsOpened => pylon.IsOpen;

        /// <summary>
        /// カメラが送信するピクセルデータのフォーマット
        /// </summary>
        public string PixelFormat
        {
            get => pylon.PixelFormat;
            set => pylon.PixelFormat = value;
        }

        /// <summary>
        /// ピクセルデータの有効なフォーマットリスト
        /// </summary>
        public string[] PixelFormatList => pylon.PixelFormatList;

        /// <summary>
        /// トリガー受信後、トリガーがアクティブになるまでの時間を取得・設定します。[ms]
        /// </summary>
        public double TriggerDelay
        {
            get => pylon.TriggerDelay;
            set => pylon.TriggerDelay = value;
        }

        /// <summary>
        /// トリガーモードで動作するかどうかを示す値を取得します。
        /// </summary>
        /// <remarks><see langword="true"/>:トリガーモード動作, <see langword="false"/>:フリーランモード動作
        /// (初期値:<see langword="false"/>)</remarks>
        public bool TriggerMode
        {
            get => pylon.TriggerMode;
            set => pylon.TriggerMode = value;
        }

        /// <summary>
        /// トリガーモード動作時の入力タイプを取得・設定します。
        /// </summary>
        /// <remarks>初期値:<see cref="TriggerInput.Hardware"/></remarks>
        public TriggerInput TriggerType
        {
            get => pylon.TriggerType;
            set => pylon.TriggerType = value;
        }

        /// <summary>
        /// カメラの横幅[pixels]
        /// </summary>
        public int Width => pylon.Width;
        #endregion // Properties

        #region Methods
        /// <summary>
        /// デバイスの利用を停止します。
        /// </summary>
        public void Close()
        {
            pylon.Close();
        }

        /// <summary>
        /// デバイスを利用可能な状態にします。
        /// </summary>
        public void Open()
        {
            pylon.Open();
            pylon.Initialize();
        }

        /// <summary>
        /// 画像の取り込みを開始します。
        /// </summary>
        public void Start()
        {
            pylon.Start();
        }

        /// <summary>
        /// 画像の取り込みを停止します。
        /// </summary>
        public void Stop()
        {
            pylon.Stop();
        }

        /// <summary>
        /// ソフトウェアトリガーによるキャプチャー処理を行います。
        /// </summary>
        /// <remarks><see cref="TriggerMode"/>が<see langword="true"/>の場合のみ動作可能です。</remarks>
        public void TakeSnapshot()
        {
            pylon.TakeSnapshot();
        }
        #endregion // Methods

        #region IDisposable Support
        private bool disposedValue = false; // 重複する呼び出しを検出するには

        /// <summary>
        /// リソースを開放します。
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: マネージド状態を破棄します (マネージド オブジェクト)。
                    pylon?.Dispose();
                }

                // TODO: アンマネージド リソース (アンマネージド オブジェクト) を解放し、下のファイナライザーをオーバーライドします。
                // TODO: 大きなフィールドを null に設定します。

                disposedValue = true;
            }
        }

        // TODO: 上の Dispose(bool disposing) にアンマネージド リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします。
        // ~SampleCamera() {
        //   // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
        //   Dispose(false);
        // }

        // このコードは、破棄可能なパターンを正しく実装できるように追加されました。
        /// <summary>
        /// リソースを開放します。
        /// </summary>
        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
            Dispose(true);
            // TODO: 上のファイナライザーがオーバーライドされる場合は、次の行のコメントを解除してください。
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
