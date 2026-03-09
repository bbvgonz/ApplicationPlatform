using Optical.API.Beams;
using Optical.API.Library.Device;
using Optical.API.Library.Optics;
using Optical.Enums;
using Optical.Platform.Types;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.Serialization;

namespace Optical.HMI.Beams
{
    /// <summary>
    /// Beam Profiler設定保存用クラス
    /// </summary>
    [DataContract]
    internal class BeamProfilerConfig : INotifyPropertyChanged, IExtensibleDataObject
    {
        #region Fields
        private IBeamProfiler beamProfiler;

        private bool autoApertureEnabled;
        private BeamwidthType autoApertureMethod;
        private double autoApertureRate;
        private bool autoExposure;
        private bool autoGain;
        private int binning;
        private BinningPixelFormat binningMode;
        private int bitDepth;
        private double blackLevel;
        private int bufferSize;
        private string serialNumber;
        private bool enableCalculation;
        private double exposureTime;
        private bool flipHorizontal;
        private bool flipVertical;
        private double frameRate;
        private double gain;
        private bool orientation;
        private double pixelPitch;
        private List<Rectangle> roiList;
        private ApertureShape roiShape;
        private double triggerDelay;
        private bool triggerMode;
        private TriggerInput triggerType;
        #endregion // Fields

        #region Constructors
        /// <summary>
        /// <see cref="BeamProfilerConfig"/>クラスの新しいインスタンスを生成します。
        /// </summary>
        public BeamProfilerConfig()
        {
            initialize();
        }

        /// <summary>
        /// <see cref="BeamProfilerConfig"/>クラスの新しいインスタンスを生成します。
        /// </summary>
        /// <param name="beamProfiler">ビームプロファイラーのインスタンス</param>
        public BeamProfilerConfig(IBeamProfiler beamProfiler)
        {
            initialize();
            this.beamProfiler = beamProfiler;
            referToApi();
        }
        #endregion // Constructors

        #region Events
        /// <summary>
        /// プロパティ値が変更されたときに発生します。
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion // Events

        #region Properties
        /// <summary>
        /// 新しいメンバーの追加によって拡張された、バージョン付きのデータ コントラクトのデータを格納します。
        /// </summary>
        [Browsable(false)]
        public ExtensionDataObject ExtensionData { get; set; }

        /// <summary>
        /// スポット開口の自動計算可否を示す値を取得・設定します。
        /// </summary>
        [DataMember]
        public bool AutoApertureEnabled
        {
            get => beamProfiler?.AutoApertureEnabled ?? autoApertureEnabled;
            set
            {
                if (beamProfiler == null)
                {
                    autoApertureEnabled = value;
                    OnPropertyChanged(nameof(AutoApertureEnabled));
                    return;
                }

                if (beamProfiler.AutoApertureEnabled == value)
                {
                    return;
                }

                beamProfiler.AutoApertureEnabled = value;
                OnPropertyChanged(nameof(AutoApertureEnabled));
            }
        }

        /// <summary>
        /// スポット開口の自動計算方式を取得・設定します。
        /// </summary>
        [DataMember]
        public BeamwidthType AutoApertureMethod
        {
            get => beamProfiler?.AutoApertureMethod ?? autoApertureMethod;
            set
            {
                if (beamProfiler == null)
                {
                    autoApertureMethod = value;
                    OnPropertyChanged(nameof(AutoApertureMethod));
                    return;
                }

                if (beamProfiler.AutoApertureMethod == value)
                {
                    return;
                }

                beamProfiler.AutoApertureMethod = value;
                OnPropertyChanged(nameof(AutoApertureMethod));
            }
        }

        /// <summary>
        /// スポット開口の自動計算結果に対する倍率を取得・設定します。
        /// </summary>
        [DataMember]
        public double AutoApertureRate
        {
            get => beamProfiler?.AutoApertureRate ?? autoApertureRate;
            set
            {
                if (beamProfiler == null)
                {
                    autoApertureRate = value;
                    OnPropertyChanged(nameof(AutoApertureRate));
                    return;
                }

                if (beamProfiler.AutoApertureRate == value)
                {
                    return;
                }

                beamProfiler.AutoApertureRate = value;
                OnPropertyChanged(nameof(AutoApertureRate));
            }
        }

        /// <summary>
        /// 自動露光時間調整の動作可否を取得・設定します。
        /// </summary>
        [DataMember]
        public bool AutoExposure
        {
            get => beamProfiler?.AutoExposure ?? autoExposure;
            set
            {
                if (beamProfiler == null)
                {
                    autoExposure = value;
                    OnPropertyChanged(nameof(AutoExposure));
                    return;
                }

                if (beamProfiler.AutoExposure == value)
                {
                    return;
                }

                beamProfiler.AutoExposure = value;
                OnPropertyChanged(nameof(AutoExposure));
            }
        }

        /// <summary>
        /// 自動ゲイン調整調整の動作可否を取得・設定します。
        /// </summary>
        [DataMember]
        public bool AutoGain
        {
            get => beamProfiler?.AutoGain ?? autoGain;
            set
            {
                if (beamProfiler == null)
                {
                    autoGain = value;
                    OnPropertyChanged(nameof(AutoGain));
                    return;
                }

                if (beamProfiler.AutoGain == value)
                {
                    return;
                }

                beamProfiler.AutoGain = value;
                OnPropertyChanged(nameof(AutoGain));
            }
        }

        /// <summary>
        /// ビニングする一辺のピクセル数を取得・設定します。
        /// </summary>
        [DataMember]
        public int Binning
        {
            get => beamProfiler?.Binning ?? binning;
            set
            {
                if (beamProfiler == null)
                {
                    binning = value;
                    OnPropertyChanged(nameof(Binning));
                    return;
                }

                if (beamProfiler.Binning == value)
                {
                    return;
                }

                beamProfiler.Binning = value;
                OnPropertyChanged(nameof(Binning));
            }
        }

        /// <summary>
        /// ビニング時のピクセル値算出方法を取得・設定します。
        /// </summary>
        [DataMember]
        public BinningPixelFormat BinningMode
        {
            get => beamProfiler?.BinningMode ?? binningMode;
            set
            {
                if (beamProfiler == null)
                {
                    binningMode = value;
                    OnPropertyChanged(nameof(BinningMode));
                    return;
                }

                if (beamProfiler.BinningMode == value)
                {
                    return;
                }

                beamProfiler.BinningMode = value;
                OnPropertyChanged(nameof(BinningMode));
            }
        }

        /// <summary>
        /// 画像データのビット深度を取得・設定します。
        /// </summary>
        [DataMember]
        public int BitDepth
        {
            get => beamProfiler?.BitDepth ?? bitDepth;
            set
            {
                if (beamProfiler == null)
                {
                    bitDepth = value;
                    OnPropertyChanged(nameof(BitDepth));
                    return;
                }

                if (beamProfiler.BitDepth == value)
                {
                    return;
                }

                beamProfiler.BitDepth = value;
                OnPropertyChanged(nameof(BitDepth));
            }
        }

        /// <summary>
        /// イメージセンサーの黒レベルを取得・設定します。
        /// </summary>
        [DataMember]
        public double BlackLevel
        {
            get => beamProfiler?.BlackLevel ?? blackLevel;
            set
            {
                if (beamProfiler == null)
                {
                    blackLevel = value;
                    OnPropertyChanged(nameof(BlackLevel));
                    return;
                }

                if (beamProfiler.BlackLevel == value)
                {
                    return;
                }

                beamProfiler.BlackLevel = value;
                OnPropertyChanged(nameof(BlackLevel));
            }
        }

        /// <summary>
        /// 測定用の内部バッファサイズを取得・設定します。
        /// </summary>
        [DataMember]
        public int BufferSize
        {
            get => beamProfiler?.BufferSize ?? bufferSize;
            set
            {
                if (beamProfiler == null)
                {
                    bufferSize = value;
                    OnPropertyChanged(nameof(BufferSize));
                    return;
                }

                if (beamProfiler.BufferSize == value)
                {
                    return;
                }

                beamProfiler.BufferSize = value;
                OnPropertyChanged(nameof(BufferSize));
            }
        }

        /// <summary>
        /// デバイスの識別子
        /// </summary>
        [DataMember]
        public string DeviceSerialNumber
        {
            get => beamProfiler?.Device.SerialNumber ?? serialNumber;
            private set
            {
                if (beamProfiler == null)
                {
                    serialNumber = value;
                    OnPropertyChanged(nameof(DeviceSerialNumber));
                    return;
                }
            }
        }

        /// <summary>
        /// スポット径の計算要否を取得・設定します。
        /// </summary>
        [DataMember]
        public bool EnableCalculation
        {
            get => beamProfiler?.EnableCalculation ?? enableCalculation;
            set
            {
                if (beamProfiler == null)
                {
                    enableCalculation = value;
                    OnPropertyChanged(nameof(EnableCalculation));
                    return;
                }

                if (beamProfiler.EnableCalculation == value)
                {
                    return;
                }

                beamProfiler.EnableCalculation = value;
                OnPropertyChanged(nameof(EnableCalculation));
            }
        }

        /// <summary>
        /// イメージセンサーの露光時間[ms]を取得・設定します。
        /// </summary>
        [DataMember]
        [Description("イメージセンサーの露光時間[ms]を取得・設定します。")]
        public double ExposureTime
        {
            get => beamProfiler?.ExposureTime ?? exposureTime;
            set
            {
                if (beamProfiler == null)
                {
                    exposureTime = value;
                    OnPropertyChanged(nameof(ExposureTime));
                    return;
                }

                if (beamProfiler.ExposureTime == value)
                {
                    return;
                }

                beamProfiler.ExposureTime = value;
                OnPropertyChanged(nameof(ExposureTime));
            }
        }

        [DataMember]
        [Description("画像の水平方向のミラーリングを有効にします。")]
        public bool FlipHorizontal
        {
            get => beamProfiler?.FlipHorizontal ?? flipHorizontal;
            set
            {
                if (beamProfiler == null)
                {
                    flipHorizontal = value;
                    OnPropertyChanged(nameof(FlipHorizontal));
                    return;
                }

                if (beamProfiler.FlipHorizontal == value)
                {
                    return;
                }

                beamProfiler.FlipHorizontal = value;
                OnPropertyChanged(nameof(FlipHorizontal));
            }
        }

        [DataMember]
        [Description("画像の垂直方向のミラーリングを有効にします。")]
        public bool FlipVertical
        {
            get => beamProfiler?.FlipVertical ?? flipVertical;
            set
            {
                if (beamProfiler == null)
                {
                    flipVertical = value;
                    OnPropertyChanged(nameof(FlipVertical));
                    return;
                }

                if (beamProfiler.FlipVertical == value)
                {
                    return;
                }

                beamProfiler.FlipVertical = value;
                OnPropertyChanged(nameof(FlipVertical));
            }
        }

        /// <summary>
        /// 画像データの更新周期[Hz]を設定します。
        /// </summary>
        [DataMember]
        [Description("画像データの更新周期[Hz]を設定します。")]
        public double FrameRate
        {
            get => beamProfiler?.FrameRate ?? frameRate;
            set
            {
                if (beamProfiler == null)
                {
                    frameRate = value;
                    OnPropertyChanged(nameof(FrameRate));
                    return;
                }

                if (beamProfiler.FrameRate == value)
                {
                    return;
                }

                beamProfiler.FrameRate = value;
                OnPropertyChanged(nameof(FrameRate));
            }
        }

        /// <summary>
        /// イメージセンサーのゲイン[dB]を取得・設定します。
        /// </summary>
        [DataMember]
        [Description("イメージセンサーのゲイン[dB]を取得・設定します。")]
        public double Gain
        {
            get => beamProfiler?.Gain ?? gain;
            set
            {
                if (beamProfiler == null)
                {
                    gain = value;
                    OnPropertyChanged(nameof(Gain));
                    return;
                }

                if (beamProfiler.Gain == value)
                {
                    return;
                }

                beamProfiler.Gain = value;
                OnPropertyChanged(nameof(Gain));
            }
        }

        /// <summary>
        /// 楕円ビームの向きを考慮するかどうかを示す値を取得・設定します。
        /// </summary>
        [DataMember]
        public bool Orientation
        {
            get => beamProfiler?.Orientation ?? orientation;
            set
            {
                if (beamProfiler == null)
                {
                    orientation = value;
                    OnPropertyChanged(nameof(Orientation));
                    return;
                }

                if (beamProfiler.Orientation == value)
                {
                    return;
                }

                beamProfiler.Orientation = value;
                OnPropertyChanged(nameof(Orientation));
            }
        }

        /// <summary>
        /// 1ピクセルあたりの大きさ[um]を取得・設定します。
        /// </summary>
        [DataMember]
        [Description("1ピクセルあたりの大きさ[um]を取得・設定します。")]
        public double PixelPitch
        {
            get => pixelPitch;
            set
            {
                pixelPitch = value;
                OnPropertyChanged(nameof(PixelPitch));
            }
        }

        /// <summary>
        /// 対象領域の範囲[pixel]を取得・設定します。
        /// </summary>
        [DataMember]
        [Description("対象領域の範囲[pixel]を取得・設定します。")]
        public List<Rectangle> RoiList
        {
            get => beamProfiler?.RoiList ?? roiList;
            set
            {
                if (beamProfiler == null)
                {
                    roiList = value;
                    OnPropertyChanged(nameof(RoiList));
                    return;
                }

                if (beamProfiler.RoiList == value)
                {
                    return;
                }

                beamProfiler.RoiList.Clear();
                beamProfiler.RoiList.AddRange(value);
                OnPropertyChanged(nameof(RoiList));
            }
        }

        /// <summary>
        /// 対象領域の開口形状を取得・設定します。
        /// </summary>
        [DataMember]
        public ApertureShape RoiShape
        {
            get => beamProfiler?.RoiShape ?? roiShape;
            set
            {
                if (beamProfiler == null)
                {
                    roiShape = value;
                    OnPropertyChanged(nameof(RoiShape));
                    return;
                }

                if (beamProfiler.RoiShape == value)
                {
                    return;
                }

                beamProfiler.RoiShape = value;
                OnPropertyChanged(nameof(RoiShape));
            }
        }

        /// <summary>
        /// トリガー受信後、トリガーがアクティブになるまでの時間[ms]を取得・設定します。
        /// </summary>
        [DataMember]
        [Description("トリガー受信後、トリガーがアクティブになるまでの時間[ms]を取得・設定します。")]
        public double TriggerDelay
        {
            get => beamProfiler?.TriggerDelay ?? triggerDelay;
            set
            {
                if (beamProfiler == null)
                {
                    triggerDelay = value;
                    OnPropertyChanged(nameof(TriggerDelay));
                    return;
                }

                if (beamProfiler.TriggerDelay == value)
                {
                    return;
                }

                beamProfiler.TriggerDelay = value;
                OnPropertyChanged(nameof(TriggerDelay));
            }
        }

        /// <summary>
        /// トリガーモードで動作するどうかを示す値を取得・設定します。
        /// </summary>
        [DataMember]
        public bool TriggerMode
        {
            get => beamProfiler?.TriggerMode ?? triggerMode;
            set
            {
                if (beamProfiler == null)
                {
                    triggerMode = value;
                    OnPropertyChanged(nameof(TriggerMode));
                    return;
                }

                if (beamProfiler.TriggerMode == value)
                {
                    return;
                }

                beamProfiler.TriggerMode = value;
                OnPropertyChanged(nameof(TriggerMode));
            }
        }

        /// <summary>
        /// トリガーモード動作時の入力タイプを取得・設定します。
        /// </summary>
        [DataMember]
        public TriggerInput TriggerType
        {
            get => beamProfiler?.TriggerType ?? triggerType;
            set
            {
                if (beamProfiler == null)
                {
                    triggerType = value;
                    OnPropertyChanged(nameof(TriggerType));
                    return;
                }

                if (beamProfiler.TriggerType == value)
                {
                    return;
                }

                beamProfiler.TriggerType = value;
                OnPropertyChanged(nameof(TriggerType));
            }
        }
        #endregion // Properties

        #region Methods
        private void referToApi()
        {
            if (beamProfiler == null)
            {
                return;
            }

            autoApertureEnabled = beamProfiler.AutoApertureEnabled;
            autoApertureMethod = beamProfiler.AutoApertureMethod;
            autoApertureRate = beamProfiler.AutoApertureRate;
            autoExposure = beamProfiler.AutoExposure;
            autoGain = beamProfiler.AutoGain;
            binning = beamProfiler.Binning;
            binningMode = beamProfiler.BinningMode;
            bitDepth = beamProfiler.BitDepth;
            blackLevel = beamProfiler.BlackLevel;
            bufferSize = beamProfiler.BufferSize;
            enableCalculation = beamProfiler.EnableCalculation;
            exposureTime = beamProfiler.ExposureTime;
            flipHorizontal = beamProfiler.FlipHorizontal;
            flipVertical = beamProfiler.FlipVertical;
            frameRate = beamProfiler.FrameRate;
            gain = beamProfiler.Gain;
            orientation = beamProfiler.Orientation;
            roiList.Clear();
            if (beamProfiler.RoiList.Count > 0)
            {
                roiList.AddRange(beamProfiler.RoiList);
            }

            roiShape = beamProfiler.RoiShape;
            serialNumber = beamProfiler.Device.SerialNumber;
            triggerDelay = beamProfiler.TriggerDelay;
            triggerMode = beamProfiler.TriggerMode;
            triggerType = beamProfiler.TriggerType;
        }

        private void initialize()
        {
            roiList = new List<Rectangle>();
        }

        private void applyToApi()
        {
            if (beamProfiler == null)
            {
                return;
            }

            beamProfiler.AutoApertureEnabled = autoApertureEnabled;
            beamProfiler.AutoApertureMethod = autoApertureMethod;
            beamProfiler.AutoApertureRate = autoApertureRate;
            beamProfiler.AutoExposure = autoExposure;
            beamProfiler.AutoGain = autoGain;
            beamProfiler.Binning = binning;
            beamProfiler.BinningMode = binningMode;
            beamProfiler.BitDepth = bitDepth;
            beamProfiler.BlackLevel = blackLevel;
            beamProfiler.BufferSize = bufferSize;
            beamProfiler.EnableCalculation = enableCalculation;
            beamProfiler.ExposureTime = exposureTime;
            beamProfiler.FlipHorizontal = flipHorizontal;
            beamProfiler.FlipVertical = flipVertical;
            beamProfiler.FrameRate = frameRate;
            beamProfiler.Gain = beamProfiler.GainRange.Minimum > gain ? beamProfiler.GainRange.Minimum : gain;
            beamProfiler.Orientation = orientation;
            beamProfiler.RoiList.Clear();
            if (roiList.Count > 0)
            {
                beamProfiler.RoiList.AddRange(roiList);
            }

            beamProfiler.RoiShape = roiShape;
            beamProfiler.TriggerDelay = triggerDelay;
            beamProfiler.TriggerMode = triggerMode;
            beamProfiler.TriggerType = triggerType;
        }

        [OnDeserializing]
        private void OnDeserializingMethod(StreamingContext sc)
        {
            initialize();
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 指定されたビームプロファイラーに各プロパティを関連付けます。
        /// </summary>
        /// <param name="beamProfiler">ビームプロファイラーのインスタンス</param>
        /// <remarks>現在設定されているパラメーターの値がアタッチしたビームプロファイラーに反映されます。</remarks>
        public void Attach(IBeamProfiler beamProfiler)
        {
            if (this.beamProfiler != null)
            {
                referToApi();
            }

            this.beamProfiler = beamProfiler;
            applyToApi();
        }

        /// <summary>
        /// ビームプロファイラーとの関連付けを解除します。
        /// </summary>
        public void Detach()
        {
            referToApi();
            serialNumber = string.Empty;
            beamProfiler = null;
        }

        /// <summary>
        /// 指定された設定データを反映します。
        /// </summary>
        /// <param name="source">反映元データ</param>
        public void Apply(BeamProfilerConfig source)
        {
            if (source == null)
            {
                return;
            }

            autoApertureEnabled = source.AutoApertureEnabled;
            autoApertureMethod = source.AutoApertureMethod;
            autoApertureRate = source.AutoApertureRate;
            autoExposure = source.AutoExposure;
            autoGain = source.AutoGain;
            binning = source.Binning;
            binningMode = source.BinningMode;
            bitDepth = source.BitDepth;
            blackLevel = source.BlackLevel;
            bufferSize = source.BufferSize;
            enableCalculation = source.EnableCalculation;
            exposureTime = source.ExposureTime;
            flipHorizontal = source.FlipHorizontal;
            flipVertical = source.FlipVertical;
            frameRate = source.FrameRate;
            gain = source.Gain;
            orientation = source.Orientation;
            roiList.Clear();
            if (source.RoiList.Count > 0)
            {
                roiList.AddRange(source.RoiList);
            }

            roiShape = source.RoiShape;
            serialNumber = source.DeviceSerialNumber;
            triggerDelay = source.TriggerDelay;
            triggerMode = source.TriggerMode;
            triggerType = source.TriggerType;

            applyToApi();

            OnPropertyChanged(string.Empty);
        }
        #endregion // Methods
    }
}
