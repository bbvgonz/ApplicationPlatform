using Optical.API.Library.Device;
using Optical.API.Library.Optics;
using Optical.API.Wavefront;
using Optical.Enums;
using Optical.Platform.Types;
using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Optical.HMI.Wavefront
{
    /// <summary>
    /// 波面収差計算用方程式設定
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DataContract]
    internal class EquationConfig : INotifyPropertyChanged, IExtensibleDataObject
    {
        #region Fields
        private IWavefront wavefront;

        private bool enableCalculation;
        private bool enableLegendre;
        private bool enableZernike;
        private int legendreDegree;
        private int zernikeIndex;
        #endregion // Fields

        #region Constructors
        internal EquationConfig()
        {
            initializeParameter();
        }

        /// <summary>
        /// <see cref="EquationConfig"/>クラスの新しいインスタンスを生成します。
        /// </summary>
        /// <param name="wavefront"><see cref="IWavefront"/>を実装したインスタンス</param>
        public EquationConfig(IWavefront wavefront)
        {
            initializeParameter();
            this.wavefront = wavefront;
            referToApi();
        }
        #endregion // Constructors

        #region Events
        /// <summary>
        /// プロパティ値が変更するときに発生します。
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion // Events

        #region Properties
        private bool isAttached => wavefront != null;

        /// <summary>
        /// 新しいメンバーの追加によって拡張された、バージョン付きのデータ コントラクトのデータを格納します。
        /// </summary>
        [Browsable(false)]
        public ExtensionDataObject ExtensionData { get; set; }

        /// <summary>
        /// 波面収差を自動的に計算するかどうかを設定します。
        /// </summary>
        [DataMember]
        public bool EnableCalculation
        {
            get => wavefront?.EnableCalculation ?? enableCalculation;
            set
            {
                if (value == (wavefront?.EnableCalculation ?? enableCalculation))
                {
                    return;
                }

                if (isAttached)
                {
                    wavefront.EnableCalculation = value;
                }
                else
                {
                    enableCalculation = value;
                }

                OnPropertyChanged(nameof(EnableCalculation));
            }
        }

        /// <summary>
        /// Legendre多項式係数を計算するかどうかを取得・設定します。
        /// </summary>
        [DataMember]
        public bool EnableLegendre
        {
            get => wavefront?.EnableLegendre ?? enableLegendre;
            set
            {
                if (value == (wavefront?.EnableLegendre ?? enableLegendre))
                {
                    return;
                }

                if (isAttached)
                {
                    wavefront.EnableLegendre = value;
                }
                else
                {
                    enableLegendre = value;
                }

                OnPropertyChanged(nameof(EnableLegendre));
            }
        }

        /// <summary>
        /// Zernike多項式係数を計算するかどうかを取得・設定します。
        /// </summary>
        [DataMember]
        public bool EnableZernike
        {
            get => wavefront?.EnableZernike ?? enableLegendre;
            set
            {
                if (value == (wavefront?.EnableZernike ?? enableZernike))
                {
                    return;
                }

                if (isAttached)
                {
                    wavefront.EnableZernike = value;
                }
                else
                {
                    enableZernike = value;
                }

                OnPropertyChanged(nameof(EnableZernike));
            }
        }


        /// <summary>
        /// Legendre多項式次数（>= 0）
        /// </summary>
        [Description("Legendre polynomial degree (>= 0)")]
        [DataMember]
        public int LegendreDegree
        {
            get => wavefront?.LegendreDegree ?? legendreDegree;
            set
            {
                if ((wavefront?.LegendreDegree ?? legendreDegree) == value)
                {
                    return;
                }

                if (isAttached)
                {
                    wavefront.LegendreDegree = value;
                }
                else
                {
                    legendreDegree = value;
                }

                OnPropertyChanged(nameof(LegendreDegree));
            }
        }

        /// <summary>
        /// Frienge・Zernike指数（1～36）
        /// </summary>
        [Description("Zernike polynomial index (1 - 36)")]
        [DataMember]
        public int ZernikeIndex
        {
            get => wavefront?.ZernikeIndex ?? zernikeIndex;
            set
            {
                if ((wavefront?.ZernikeIndex ?? zernikeIndex) == value)
                {
                    return;
                }

                if (isAttached)
                {
                    wavefront.ZernikeIndex = value;
                }
                else
                {
                    zernikeIndex = value;
                }

                OnPropertyChanged(nameof(ZernikeIndex));
            }
        }
        #endregion // Properties

        #region Methods
        private void referToApi()
        {
            if (wavefront == null)
            {
                return;
            }

            enableCalculation = wavefront.EnableCalculation;
            legendreDegree = wavefront.LegendreDegree;
            zernikeIndex = wavefront.ZernikeIndex;
        }

        private void initializeParameter()
        {
            enableCalculation = true;
            legendreDegree = 6;
            zernikeIndex = 36;
        }

        private void applyToApi()
        {
            if (wavefront == null)
            {
                return;
            }

            wavefront.EnableCalculation = enableCalculation;
            wavefront.LegendreDegree = legendreDegree;
            wavefront.ZernikeIndex = zernikeIndex;
        }

        protected void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 指定された波面センサーに各プロパティを関連付けます。
        /// </summary>
        /// <param name="wavefront">波面センサーのインスタンス</param>
        /// <remarks>現在設定されているパラメーターの値がアタッチした波面センサーに反映されます。</remarks>
        public void Attach(IWavefront wavefront)
        {
            if (this.wavefront != null)
            {
                referToApi();
            }

            this.wavefront = wavefront;

            applyToApi();
        }

        /// <summary>
        /// 波面センサーとの関連付けを解除します。
        /// </summary>
        public void Detach()
        {
            referToApi();
            wavefront = null;
        }

        /// <summary>
        /// 指定された設定データを自身に反映します。
        /// </summary>
        /// <param name="source">反映元データ</param>
        public void Apply(EquationConfig source)
        {
            if (source == null)
            {
                return;
            }

            enableCalculation = source.EnableCalculation;
            legendreDegree = source.LegendreDegree;
            zernikeIndex = source.ZernikeIndex;

            applyToApi();

            OnPropertyChanged();
        }
        #endregion // Methods
    }

    /// <summary>
    /// 画像処理設定
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DataContract]
    internal class ImageConfig : INotifyPropertyChanged, IExtensibleDataObject
    {
        #region Fields
        private IWavefront wavefront;

        private PointD apertureCenter;
        private bool apertureFixed;
        private bool apertureCenterTracking;
        private Size<double> apertureSize;
        private ApertureShape apertureType;
        private bool autoApertureSize;
        private bool autoApertureAspectFit;
        private int averagingCount;
        private int bufferSize;
        private int noiseLevel;
        private double pairingAreaRate;
        private Limit<int> spotSizeRange;
        #endregion // Fields

        #region Constructors
        internal ImageConfig()
        {
            initializeParameter();
        }

        /// <summary>
        /// <see cref="LightConfig"/>クラスの新しいインスタンスを生成します。
        /// </summary>
        /// <param name="wavefront"><see cref="IWavefront"/>を実装したインスタンス</param>
        public ImageConfig(IWavefront wavefront)
        {
            initializeParameter();
            this.wavefront = wavefront;
            referToApi();
        }
        #endregion // Constructors

        #region Events
        /// <summary>
        /// プロパティ値が変更するときに発生します。
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion // Events

        #region Properties
        private bool isAttached => wavefront != null;

        /// <summary>
        /// 新しいメンバーの追加によって拡張された、バージョン付きのデータ コントラクトのデータを格納します。
        /// </summary>
        [Browsable(false)]
        public ExtensionDataObject ExtensionData { get; set; }

        /// <summary>
        /// アパーチャー中心座標[pixel]を取得・設定します。
        /// </summary>
        [DataMember]
        public PointD ApertureCenter
        {
            get => wavefront?.ApertureCenter ?? apertureCenter;
            set
            {
                if (wavefront == null)
                {
                    if (apertureCenter == null)
                    {
                        apertureCenter = new PointD();
                    }

                    if ((apertureCenter.X == value.X) &&
                        (apertureCenter.Y == value.Y))
                    {
                        return;
                    }

                    apertureCenter.XY(value.X, value.Y);
                    OnPropertyChanged(nameof(ApertureCenter));
                    return;
                }
                else
                {
                    if ((wavefront.ApertureCenter.X == value.X) &&
                        (wavefront.ApertureCenter.Y == value.Y))
                    {
                        return;
                    }

                    wavefront.ApertureCenter.XY(value.X, value.Y);

                    OnPropertyChanged(nameof(ApertureCenter));
                }
            }
        }

        /// <summary>
        /// 波面計算時にApertureを固定するかどうかを取得・設定します。
        /// </summary>
        [DataMember]
        public bool ApertureFixed
        {
            get => wavefront?.ApertureFixed ?? apertureFixed;
            set
            {
                if (wavefront == null)
                {
                    if (apertureFixed == value)
                    {
                        return;
                    }

                    apertureFixed = value;
                }
                else
                {
                    if (wavefront.ApertureFixed == value)
                    {
                        return;
                    }

                    wavefront.ApertureFixed = value;
                }

                OnPropertyChanged(nameof(ApertureFixed));
            }
        }

        /// <summary>
        /// アパーチャー中心が入力画像のスポットを追跡するかどうかを設定します。
        /// </summary>
        [DataMember]
        public bool ApertureCenterTracking
        {
            get => wavefront?.ApertureCenterTracking ?? apertureCenterTracking;
            set
            {
                if (wavefront == null)
                {
                    if (apertureCenterTracking == value)
                    {
                        return;
                    }

                    apertureCenterTracking = value;
                }
                else
                {
                    if (wavefront.ApertureCenterTracking == value)
                    {
                        return;
                    }

                    wavefront.ApertureCenterTracking = value;
                }

                OnPropertyChanged(nameof(ApertureCenterTracking));
            }
        }

        /// <summary>
        /// アパーチャーのサイズ[pixel]を取得・設定します。
        /// </summary>
        [DataMember]
        public Size<double> ApertureSize
        {
            get => wavefront?.ApertureSize ?? apertureSize;
            set
            {
                if (wavefront == null)
                {
                    if (apertureSize == null)
                    {
                        apertureSize = new Size<double>();
                    }

                    if ((apertureSize.Width == value.Height) &&
                        (apertureSize.Height == value.Width))
                    {
                        return;
                    }

                    apertureSize.Width = value.Width;
                    apertureSize.Height = value.Height;

                    OnPropertyChanged(nameof(ApertureSize));
                }
                else
                {
                    if ((wavefront.ApertureSize.Width == value.Height) &&
                        (wavefront.ApertureSize.Height == value.Width))
                    {
                        return;
                    }

                    wavefront.ApertureSize.Width = value.Width;
                    wavefront.ApertureSize.Height = value.Height;

                    OnPropertyChanged(nameof(ApertureSize));
                }
            }
        }

        /// <summary>
        /// アパーチャー形状を取得・設定します。
        /// </summary>
        [DataMember]
        public ApertureShape ApertureType
        {
            get => wavefront?.ApertureType ?? apertureType;
            set
            {
                if (wavefront == null)
                {
                    apertureType = value;
                    OnPropertyChanged(nameof(ApertureType));
                    return;
                }

                if (wavefront.ApertureType == value)
                {
                    return;
                }

                wavefront.ApertureType = value;

                OnPropertyChanged(nameof(ApertureType));
            }
        }

        /// <summary>
        /// アパーチャサイズの自動調整をするかどうかを示す値を取得・設定します。
        /// </summary>
        [DataMember]
        public bool AutoApertureSize
        {
            get => wavefront?.AutoApertureSize ?? autoApertureSize;
            set
            {
                if (wavefront == null)
                {
                    autoApertureSize = value;
                    OnPropertyChanged(nameof(AutoApertureSize));
                    return;
                }

                if (wavefront.AutoApertureSize == value)
                {
                    return;
                }

                wavefront.AutoApertureSize = value;

                OnPropertyChanged(nameof(AutoApertureSize));
            }
        }

        /// <summary>
        /// アパーチャの縦横比を(1:1)になるように調整します。
        /// </summary>
        [DataMember]
        public bool AutoApertureAspectFit
        {
            get => wavefront?.AutoApertureAspectFit ?? autoApertureAspectFit;
            set
            {
                if (wavefront == null)
                {
                    autoApertureAspectFit = value;
                    OnPropertyChanged(nameof(AutoApertureAspectFit));
                    return;
                }

                if (wavefront.AutoApertureAspectFit == value)
                {
                    return;
                }

                wavefront.AutoApertureAspectFit = value;

                OnPropertyChanged(nameof(AutoApertureAspectFit));
            }
        }

        /// <summary>
        /// 画像の平均化回数を取得・設定します。（移動平均）
        /// </summary>
        [DataMember]
        public int AveragingCount
        {
            get => wavefront?.AveragingCount ?? averagingCount;
            set
            {
                if (wavefront == null)
                {
                    averagingCount = value;
                    OnPropertyChanged(nameof(AveragingCount));
                    return;
                }

                if (wavefront.AveragingCount == value)
                {
                    return;
                }

                wavefront.AveragingCount = value;

                OnPropertyChanged(nameof(AveragingCount));
            }
        }

        /// <summary>
        /// 測定結果バッファのサイズを取得・設定します。
        /// </summary>
        /// <remarks>初期サイズ：1
        /// <para>測定停止中のみ変更可能です。</para></remarks>
        [DataMember]
        public int BufferSize
        {
            get => wavefront?.BufferSize ?? bufferSize;
            set
            {
                if (value == (wavefront?.BufferSize ?? bufferSize))
                {
                    return;
                }

                if (value < 1)
                {
                    return;
                }

                if (isAttached)
                {
                    wavefront.BufferSize = value;
                }
                else
                {
                    bufferSize = value;
                }

                OnPropertyChanged(nameof(BufferSize));
            }
        }

        /// <summary>
        /// イメージセンサーのノイズレベルを取得・設定します。
        /// </summary>
        /// <remarks>
        /// <para>輝度値がノイズレベル以下のピクセルは計算対象外になります。</para>
        /// <para>設定可能な値の範囲は、取得画像のビット深度に依存します。</para></remarks>
        /// <seealso cref="SensorConfig.BitDepth"/>
        [DataMember]
        public int NoiseLevel
        {
            get => wavefront?.NoiseLevel ?? noiseLevel;
            set
            {
                if (wavefront == null)
                {
                    noiseLevel = value;
                    OnPropertyChanged(nameof(NoiseLevel));
                    return;
                }

                if (wavefront.NoiseLevel == value)
                {
                    return;
                }

                wavefront.NoiseLevel = value;

                OnPropertyChanged(nameof(NoiseLevel));
            }
        }

        /// <summary>
        /// ペアリング対象領域の拡大率
        /// </summary>
        [DataMember]
        public double PairingAreaRate
        {
            get => wavefront?.PairingAreaRate ?? pairingAreaRate;
            set
            {
                if (value == (wavefront?.PairingAreaRate ?? pairingAreaRate))
                {
                    return;
                }

                if (value < 1)
                {
                    return;
                }

                if (isAttached)
                {
                    wavefront.PairingAreaRate = value;
                }
                else
                {
                    pairingAreaRate = value;
                }

                OnPropertyChanged(nameof(PairingAreaRate));
            }
        }

        /// <summary>
        /// スポットとみなす光点の面積範囲を取得・設定します。
        /// </summary>
        [DataMember]
        public Limit<int> SpotSizeRange
        {
            get => wavefront?.SpotSizeRange ?? spotSizeRange;
            set
            {
                if (wavefront == null)
                {
                    if (spotSizeRange == null)
                    {
                        spotSizeRange = new Limit<int>();
                    }

                    spotSizeRange.Maximum = value.Maximum;
                    spotSizeRange.Minimum = value.Minimum;
                    OnPropertyChanged(nameof(NoiseLevel));
                    return;
                }

                if (wavefront.SpotSizeRange == value)
                {
                    return;
                }

                wavefront.SpotSizeRange.Maximum = value.Maximum;
                wavefront.SpotSizeRange.Minimum = value.Minimum;

                OnPropertyChanged(nameof(SpotSizeRange));
            }
        }
        #endregion // Properties

        #region Methods
        private void referToApi()
        {
            if (wavefront == null)
            {
                return;
            }

            apertureCenter.XY(wavefront.ApertureCenter.X, wavefront.ApertureCenter.Y);
            apertureCenterTracking = wavefront.ApertureCenterTracking;
            apertureFixed = wavefront.ApertureFixed;
            apertureSize.Height = wavefront.ApertureSize.Height;
            apertureSize.Width = wavefront.ApertureSize.Width;
            apertureType = wavefront.ApertureType;
            autoApertureSize = wavefront.AutoApertureSize;
            autoApertureAspectFit = wavefront.AutoApertureAspectFit;
            averagingCount = wavefront.AveragingCount;
            bufferSize = wavefront.BufferSize;
            noiseLevel = wavefront.NoiseLevel;
            pairingAreaRate = wavefront.PairingAreaRate;
            spotSizeRange.Maximum = wavefront.SpotSizeRange.Maximum;
            spotSizeRange.Minimum = wavefront.SpotSizeRange.Minimum;
        }

        private void initializeParameter()
        {
            apertureCenter = new PointD();
            apertureSize = new Size<double>();
            bufferSize = 1;
            pairingAreaRate = 1;
            spotSizeRange = new Limit<int>();
        }

        private void applyToApi()
        {
            if (wavefront == null)
            {
                return;
            }

            wavefront.ApertureCenter.XY(apertureCenter.X, apertureCenter.Y);
            wavefront.ApertureCenterTracking = apertureCenterTracking;
            wavefront.ApertureFixed = apertureFixed;
            wavefront.ApertureSize.Height = apertureSize.Height;
            wavefront.ApertureSize.Width = apertureSize.Width;
            wavefront.ApertureType = apertureType;
            wavefront.AutoApertureSize = autoApertureSize;
            wavefront.AutoApertureAspectFit = autoApertureAspectFit;
            wavefront.AveragingCount = averagingCount;
            wavefront.BufferSize = bufferSize;
            wavefront.NoiseLevel = noiseLevel;
            wavefront.PairingAreaRate = pairingAreaRate;
            wavefront.SpotSizeRange.Maximum = spotSizeRange.Maximum;
            wavefront.SpotSizeRange.Minimum = spotSizeRange.Minimum;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 指定された波面センサーに各プロパティを関連付けます。
        /// </summary>
        /// <param name="wavefront">波面センサーのインスタンス</param>
        /// <remarks>現在設定されているパラメーターの値がアタッチした波面センサーに反映されます。</remarks>
        public void Attach(IWavefront wavefront)
        {
            if (this.wavefront != null)
            {
                referToApi();
            }

            this.wavefront = wavefront;

            applyToApi();
        }

        /// <summary>
        /// 波面センサーとの関連付けを解除します。
        /// </summary>
        public void Detach()
        {
            referToApi();
            wavefront = null;
        }

        /// <summary>
        /// 指定された設定データを自身に反映します。
        /// </summary>
        /// <param name="source">反映元データ</param>
        public void Apply(ImageConfig source)
        {
            if (source == null)
            {
                return;
            }

            apertureCenter.XY(source.ApertureCenter.X, source.ApertureCenter.Y);
            apertureCenterTracking = source.ApertureCenterTracking;
            apertureFixed = source.ApertureFixed;
            apertureSize.Height = source.ApertureSize.Height;
            ApertureSize.Width = source.ApertureSize.Width;
            apertureType = source.ApertureType;
            autoApertureSize = source.AutoApertureSize;
            autoApertureAspectFit = source.AutoApertureAspectFit;
            averagingCount = source.AveragingCount;
            bufferSize = source.BufferSize;
            noiseLevel = source.NoiseLevel;
            pairingAreaRate = source.PairingAreaRate;
            spotSizeRange.Maximum = source.SpotSizeRange.Maximum;
            SpotSizeRange.Minimum = source.SpotSizeRange.Minimum;

            applyToApi();

            OnPropertyChanged(string.Empty);
        }
        #endregion // Methods
    }

    /// <summary>
    /// 光源情報設定
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DataContract]
    internal class LightConfig : INotifyPropertyChanged, IExtensibleDataObject
    {
        #region Fields
        private IWavefront wavefront;

        private double wavelength;
        private bool doublePass;
        #endregion // Fields

        #region Constructors
        internal LightConfig() { }

        /// <summary>
        /// <see cref="LightConfig"/>クラスの新しいインスタンスを生成します。
        /// </summary>
        /// <param name="wavefront"><see cref="IWavefront"/>を実装したインスタンス</param>
        public LightConfig(IWavefront wavefront)
        {
            initializeParameter();
            this.wavefront = wavefront;
            referToApi();
        }
        #endregion // Constructors

        #region Events
        /// <summary>
        /// プロパティ値が変更するときに発生します。
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
        /// 波長[nm]
        /// </summary>
        [DataMember]
        public double Wavelength
        {
            get => wavefront?.Wavelength ?? wavelength;
            set
            {
                if (wavefront == null)
                {
                    wavelength = value;
                    OnPropertyChanged(nameof(Wavelength));
                    return;
                }

                if (wavefront.Wavelength == value)
                {
                    return;
                }

                wavefront.Wavelength = value;

                OnPropertyChanged(nameof(Wavelength));
            }
        }

        /// <summary>
        /// 光路が複光路方式かどうかを取得・設定する。
        /// </summary>
        [DataMember]
        public bool DoublePass
        {
            get => wavefront?.DoublePass ?? doublePass;
            set
            {
                if (wavefront == null)
                {
                    doublePass = value;
                    OnPropertyChanged(nameof(DoublePass));
                    return;
                }

                if (wavefront.DoublePass == value)
                {
                    return;
                }

                wavefront.DoublePass = value;

                OnPropertyChanged(nameof(DoublePass));
            }
        }

        #endregion // Properties

        #region Methods
        private void referToApi()
        {
            if (wavefront == null)
            {
                return;
            }

            doublePass = wavefront.DoublePass;
            wavelength = wavefront.Wavelength;
        }

        private void initializeParameter()
        {
            throw new NotImplementedException();
        }

        private void applyToApi()
        {
            if (wavefront == null)
            {
                return;
            }

            wavefront.DoublePass = doublePass;
            wavefront.Wavelength = wavelength;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 指定された波面センサーに各プロパティを関連付けます。
        /// </summary>
        /// <param name="wavefront">波面センサーのインスタンス</param>
        /// <remarks>現在設定されているパラメーターの値がアタッチした波面センサーに反映されます。</remarks>
        public void Attach(IWavefront wavefront)
        {
            if (this.wavefront != null)
            {
                referToApi();
            }

            this.wavefront = wavefront;

            applyToApi();
        }

        /// <summary>
        /// 波面センサーとの関連付けを解除します。
        /// </summary>
        public void Detach()
        {
            referToApi();
            wavefront = null;
        }

        /// <summary>
        /// 指定された設定データを自身に反映します。
        /// </summary>
        /// <param name="source">反映元データ</param>
        public void Apply(LightConfig source)
        {
            if (source == null)
            {
                return;
            }

            doublePass = source.DoublePass;
            wavelength = source.Wavelength;

            applyToApi();

            OnPropertyChanged(string.Empty);
        }
        #endregion // Methods
    }

    /// <summary>
    /// Point Spread Function演算設定
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DataContract]
    internal class PsfConfig : INotifyPropertyChanged, IExtensibleDataObject
    {
        #region Fields
        private IWavefront wavefront;

        private bool normalization;
        private int samplingGridDivision;
        #endregion // Fields

        #region Constructors
        internal PsfConfig() { }

        /// <summary>
        /// <see cref="PsfConfig"/>クラスの新しいインスタンスを生成します。
        /// </summary>
        /// <param name="wavefront"><see cref="IWavefront"/>を実装したインスタンス</param>
        public PsfConfig(IWavefront wavefront)
        {
            this.wavefront = wavefront;
            referToApi();
        }
        #endregion // Constructors

        #region Events
        /// <summary>
        /// プロパティ値が変更するときに発生します。
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
        /// パワースペクトルを正規化する。
        /// </summary>
        [DataMember]
        public bool Normalization
        {
            get => wavefront?.Normalization ?? normalization;
            set
            {
                if (wavefront == null)
                {
                    normalization = value;
                    OnPropertyChanged(nameof(Normalization));
                    return;
                }

                if (wavefront.Normalization == value)
                {
                    return;
                }

                wavefront.Normalization = value;

                OnPropertyChanged(nameof(Normalization));
            }
        }

        /// <summary>
        /// サンプリンググリッド分割数。
        /// </summary>
        [DataMember]
        public int SamplingGridDivision
        {
            get => wavefront?.SamplingGridDivision ?? samplingGridDivision;
            set
            {
                if (wavefront == null)
                {
                    samplingGridDivision = value;
                    OnPropertyChanged(nameof(SamplingGridDivision));
                    return;
                }

                if (wavefront.SamplingGridDivision == value)
                {
                    return;
                }

                wavefront.SamplingGridDivision = value;

                OnPropertyChanged(nameof(SamplingGridDivision));
            }
        }
        #endregion // Properties

        #region Methods
        private void referToApi()
        {
            if (wavefront == null)
            {
                return;
            }

            normalization = wavefront.Normalization;
            samplingGridDivision = wavefront.SamplingGridDivision;
        }

        private void applyToApi()
        {
            if (wavefront == null)
            {
                return;
            }

            wavefront.Normalization = normalization;
            wavefront.SamplingGridDivision = samplingGridDivision;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 指定された波面センサーに各プロパティを関連付けます。
        /// </summary>
        /// <param name="wavefront">波面センサーのインスタンス</param>
        /// <remarks>現在設定されているパラメーターの値がアタッチした波面センサーに反映されます。</remarks>
        public void Attach(IWavefront wavefront)
        {
            if (this.wavefront != null)
            {
                referToApi();
            }

            this.wavefront = wavefront;

            applyToApi();
        }

        /// <summary>
        /// 波面センサーとの関連付けを解除します。
        /// </summary>
        public void Detach()
        {
            referToApi();
            wavefront = null;
        }

        /// <summary>
        /// 指定された設定データを自身に反映します。
        /// </summary>
        /// <param name="source">反映元データ</param>
        public void Apply(PsfConfig source)
        {
            if (source == null)
            {
                return;
            }

            normalization = source.Normalization;
            samplingGridDivision = source.SamplingGridDivision;

            applyToApi();

            OnPropertyChanged(string.Empty);
        }
        #endregion // Methods
    }

    /// <summary>
    /// センサーカメラ構成情報設定
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DataContract]
    internal class SensorConfig : INotifyPropertyChanged, IExtensibleDataObject
    {
        #region Fields
        private IWavefront wavefront;

        private bool autoExposure;
        private bool autoGain;
        private int binning;
        private BinningPixelFormat binningMode;
        private int bitDepth;
        private double blackLevel;
        private double exposureTime;
        private Limit<double> exposureTimeRange;
        private double frameRate;
        private double gain;
        private Limit<double> gainRange;
        private string serialNumber;
        private double triggerDelay;
        private bool triggerMode;
        private TriggerInput triggerType;
        #endregion // Fields

        #region Constructors
        public SensorConfig()
        {
            initializeParameter();
        }

        /// <summary>
        /// <see cref="SensorConfig"/>クラスの新しいインスタンスを生成します。
        /// </summary>
        /// <param name="wavefront"><see cref="IWavefront"/>を実装したインスタンス</param>
        public SensorConfig(IWavefront wavefront)
        {
            initializeParameter();
            this.wavefront = wavefront;
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
        /// 自動露光時間調整の動作可否を取得・設定します。
        /// </summary>
        [DataMember]
        public bool AutoExposure
        {
            get => wavefront?.AutoExposure ?? autoExposure;
            set
            {
                if (wavefront == null)
                {
                    autoExposure = value;
                    OnPropertyChanged(nameof(AutoExposure));
                    return;
                }

                if (wavefront.AutoExposure == value)
                {
                    return;
                }

                wavefront.AutoExposure = value;

                OnPropertyChanged(nameof(AutoExposure));
            }
        }

        /// <summary>
        /// 自動ゲイン調整調整の動作可否を取得・設定します。
        /// </summary>
        [DataMember]
        public bool AutoGain
        {
            get => wavefront?.AutoGain ?? autoGain;
            set
            {
                if (wavefront == null)
                {
                    autoGain = value;
                    OnPropertyChanged(nameof(AutoGain));
                    return;
                }

                if (wavefront.AutoGain == value)
                {
                    return;
                }

                wavefront.AutoGain = value;

                OnPropertyChanged(nameof(AutoGain));
            }
        }

        /// <summary>
        /// ビニングする一辺のピクセル数を取得・設定します。
        /// </summary>
        [DataMember]
        public int Binning
        {
            get => wavefront?.Binning ?? binning;
            set
            {
                if (wavefront == null)
                {
                    binning = value;
                    OnPropertyChanged(nameof(Binning));
                    return;
                }

                if (wavefront.Binning == value)
                {
                    return;
                }

                wavefront.Binning = value;

                OnPropertyChanged(nameof(Binning));
            }
        }

        /// <summary>
        /// ビニング時のピクセル値算出方法を取得・設定します。
        /// </summary>
        [DataMember]
        public BinningPixelFormat BinningMode
        {
            get => wavefront?.BinningMode ?? binningMode;
            set
            {
                if (wavefront == null)
                {
                    binningMode = value;
                    OnPropertyChanged(nameof(BinningMode));
                    return;
                }

                if (wavefront.BinningMode == value)
                {
                    return;
                }

                wavefront.BinningMode = value;

                OnPropertyChanged(nameof(BinningMode));
            }
        }

        /// <summary>
        /// ビット深度[bit]
        /// </summary>
        [DataMember]
        public int BitDepth
        {
            get => wavefront?.BitDepth ?? bitDepth;
            set
            {
                if (wavefront == null)
                {
                    bitDepth = value;
                    OnPropertyChanged(nameof(BitDepth));
                    return;
                }

                if (wavefront.BitDepth == value)
                {
                    return;
                }

                wavefront.BitDepth = value;

                OnPropertyChanged(nameof(BitDepth));
            }
        }

        /// <summary>
        /// イメージセンサーの黒レベルを取得・設定します。
        /// </summary>
        [DataMember]
        public double BlackLevel
        {
            get => wavefront?.BlackLevel ?? blackLevel;
            set
            {
                if (wavefront == null)
                {
                    blackLevel = value;
                    OnPropertyChanged(nameof(BlackLevel));
                    return;
                }

                if (wavefront.BlackLevel == value)
                {
                    return;
                }

                wavefront.BlackLevel = value;

                OnPropertyChanged(nameof(BlackLevel));
            }
        }

        /// <summary>
        /// デバイスの識別子
        /// </summary>
        [DataMember]
        public string DeviceSerialNumber
        {
            get => wavefront?.Device.SerialNumber ?? serialNumber;
            set
            {
                if (wavefront == null)
                {
                    serialNumber = value;
                    OnPropertyChanged(nameof(DeviceSerialNumber));
                    return;
                }
            }
        }

        /// <summary>
        /// イメージセンサーの露光時間[ms]を取得・設定します。
        /// </summary>
        [DataMember]
        public double ExposureTime
        {
            get => wavefront?.ExposureTime ?? exposureTime;
            set
            {
                if (wavefront == null)
                {
                    exposureTime = value;
                    OnPropertyChanged(nameof(ExposureTime));
                    return;
                }

                if (wavefront.ExposureTime == value)
                {
                    return;
                }

                wavefront.ExposureTime = value;

                OnPropertyChanged(nameof(ExposureTime));
            }
        }

        /// <summary>
        /// 露光時間の設定範囲[ms]
        /// </summary>
        [DataMember]
        public Limit<double> ExposureTimeRange
        {
            get => wavefront?.ExposureTimeRange ?? exposureTimeRange;
            set
            {
                if (wavefront == null)
                {
                    if (exposureTimeRange == null)
                    {
                        exposureTimeRange = new Limit<double>();
                    }

                    exposureTimeRange.Maximum = value.Maximum;
                    exposureTimeRange.Minimum = value.Minimum;

                    OnPropertyChanged(nameof(ExposureTime));
                    return;
                }

                if ((wavefront.ExposureTimeRange.Maximum == value.Maximum) &&
                    (wavefront.ExposureTimeRange.Minimum == value.Minimum))
                {
                    return;
                }

                wavefront.ExposureTimeRange.Maximum = value.Maximum;
                wavefront.ExposureTimeRange.Minimum = value.Minimum;

                OnPropertyChanged(nameof(ExposureTimeRange));
            }
        }

        /// <summary>
        /// 画像データの更新周期[Hz]を設定します。
        /// </summary>
        [DataMember]
        public double FrameRate
        {
            get => wavefront?.FrameRate ?? frameRate;
            set
            {
                if (wavefront == null)
                {
                    frameRate = value;
                    OnPropertyChanged(nameof(FrameRate));
                    return;
                }

                if (wavefront.FrameRate == value)
                {
                    return;
                }

                wavefront.FrameRate = value;

                OnPropertyChanged(nameof(FrameRate));
            }
        }

        /// <summary>
        /// イメージセンサーのゲイン[dB]を取得・設定します。
        /// </summary>
        [DataMember]
        public double Gain
        {
            get => wavefront?.Gain ?? gain;
            set
            {
                if (wavefront == null)
                {
                    gain = value;
                    OnPropertyChanged(nameof(Gain));
                    return;
                }

                if (wavefront.Gain == value)
                {
                    return;
                }

                wavefront.Gain = value;

                OnPropertyChanged(nameof(Gain));
            }
        }

        /// <summary>
        /// ゲインの設定範囲[dB]
        /// </summary>
        [DataMember]
        public Limit<double> GainRange
        {
            get => wavefront?.GainRange ?? gainRange;
            set
            {
                if (wavefront == null)
                {
                    if (gainRange == null)
                    {
                        gainRange = new Limit<double>();
                    }

                    gainRange.Maximum = value.Maximum;
                    gainRange.Minimum = value.Minimum;

                    OnPropertyChanged(nameof(GainRange));
                    return;
                }

                if ((wavefront.GainRange.Maximum == value.Maximum) &&
                    (wavefront.GainRange.Minimum == value.Minimum))
                {
                    return;
                }

                wavefront.GainRange.Maximum = value.Maximum;
                wavefront.GainRange.Minimum = value.Minimum;

                OnPropertyChanged(nameof(GainRange));
            }
        }

        /// <summary>
        /// トリガー受信後、トリガーがアクティブになるまでの時間を取得・設定します。[ms]
        /// </summary>
        [DataMember]
        public double TriggerDelay
        {
            get => wavefront?.TriggerDelay ?? triggerDelay;
            set
            {
                if (wavefront == null)
                {
                    triggerDelay = value;
                    OnPropertyChanged(nameof(GainRange));
                    return;
                }

                if (wavefront.TriggerDelay == value)
                {
                    return;
                }

                wavefront.TriggerDelay = value;

                OnPropertyChanged(nameof(GainRange));
            }
        }

        /// <summary>
        /// 外部トリガーで動作するどうかを示す値を取得・設定します。
        /// </summary>
        /// <remarks>初期値:<see langword="false"/></remarks>
        /// <seealso cref="TriggerType"/>
        [DataMember]
        public bool TriggerMode
        {
            get => wavefront?.TriggerMode ?? triggerMode;
            set
            {
                if (wavefront == null)
                {
                    triggerMode = value;
                    OnPropertyChanged(nameof(TriggerMode));
                    return;
                }

                if (wavefront.TriggerMode == value)
                {
                    return;
                }

                wavefront.TriggerMode = value;

                OnPropertyChanged(nameof(TriggerMode));
            }
        }

        /// <summary>
        /// トリガーモード動作時の入力タイプを取得・設定します。
        /// </summary>
        /// <remarks>初期値:<see cref="TriggerInput.Hardware"/></remarks>
        /// <seealso cref="TriggerMode"/>
        [DataMember]
        public TriggerInput TriggerType
        {
            get => wavefront?.TriggerType ?? triggerType;
            set
            {
                if (wavefront == null)
                {
                    triggerType = value;
                    OnPropertyChanged(nameof(TriggerType));
                    return;
                }

                if (wavefront.TriggerType == value)
                {
                    return;
                }

                wavefront.TriggerType = value;

                OnPropertyChanged(nameof(TriggerType));
            }
        }
        #endregion // Properties

        #region Methods
        private void referToApi()
        {
            if (wavefront == null)
            {
                return;
            }

            autoExposure = wavefront.AutoExposure;
            autoGain = wavefront.AutoGain;
            binning = wavefront.Binning;
            binningMode = wavefront.BinningMode;
            bitDepth = wavefront.BitDepth;
            blackLevel = wavefront.BlackLevel;
            exposureTime = wavefront.ExposureTime;
            exposureTimeRange.Maximum = wavefront.ExposureTimeRange.Maximum;
            exposureTimeRange.Minimum = wavefront.ExposureTimeRange.Minimum;
            frameRate = wavefront.FrameRate;
            gain = wavefront.Gain;
            gainRange.Maximum = wavefront.GainRange.Maximum;
            gainRange.Minimum = wavefront.GainRange.Minimum;
            serialNumber = wavefront.Device.SerialNumber;
            triggerDelay = wavefront.TriggerDelay;
            triggerMode = wavefront.TriggerMode;
            triggerType = wavefront.TriggerType;
        }

        private void initializeParameter()
        {
            exposureTimeRange = new Limit<double>();
            gainRange = new Limit<double>();
        }

        private void applyToApi()
        {
            if (wavefront == null)
            {
                return;
            }

            wavefront.AutoExposure = autoExposure;
            wavefront.AutoGain = autoGain;
            wavefront.Binning = binning;
            wavefront.BinningMode = binningMode;
            wavefront.BitDepth = bitDepth;
            wavefront.BlackLevel = blackLevel;
            wavefront.ExposureTime = exposureTime;
            wavefront.ExposureTimeRange.Maximum = exposureTimeRange.Maximum;
            wavefront.ExposureTimeRange.Minimum = exposureTimeRange.Minimum;
            wavefront.FrameRate = frameRate;
            wavefront.Gain = gain;
            wavefront.GainRange.Maximum = gainRange.Maximum;
            wavefront.GainRange.Minimum = gainRange.Minimum;
            wavefront.TriggerDelay = triggerDelay;
            wavefront.TriggerMode = triggerMode;
            wavefront.TriggerType = triggerType;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 指定された波面センサーに各プロパティを関連付けます。
        /// </summary>
        /// <param name="wavefront">波面センサーのインスタンス</param>
        /// <remarks>現在設定されているパラメーターの値がアタッチした波面センサーに反映されます。</remarks>
        public void Attach(IWavefront wavefront)
        {
            if (this.wavefront != null)
            {
                referToApi();
            }

            this.wavefront = wavefront;

            applyToApi();
        }

        /// <summary>
        /// 波面センサーとの関連付けを解除します。
        /// </summary>
        public void Detach()
        {
            referToApi();
            wavefront = null;
        }

        /// <summary>
        /// 指定された設定データを自身に反映します。
        /// </summary>
        /// <param name="source">反映元データ</param>
        public void Apply(SensorConfig source)
        {
            if (source == null)
            {
                return;
            }

            autoExposure = source.AutoExposure;
            autoGain = source.AutoGain;
            binning = source.Binning;
            binningMode = source.BinningMode;
            bitDepth = source.BitDepth;
            blackLevel = source.BlackLevel;
            exposureTime = source.ExposureTime;
            exposureTimeRange.Maximum = source.ExposureTimeRange.Maximum;
            exposureTimeRange.Minimum = source.ExposureTimeRange.Minimum;
            frameRate = source.FrameRate;
            gain = source.Gain;
            gainRange.Maximum = source.GainRange.Maximum;
            gainRange.Minimum = source.GainRange.Minimum;
            serialNumber = source.DeviceSerialNumber;
            triggerDelay = source.TriggerDelay;
            triggerMode = source.TriggerMode;
            triggerType = source.TriggerType;

            applyToApi();

            OnPropertyChanged(string.Empty);
        }
        #endregion // Methods
    }

    /// <summary>
    /// 波面センサー構成情報設定
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [DataContract]
    internal class WavefrontConfig : INotifyPropertyChanged, IExtensibleDataObject
    {
        #region Constructors
        /// <summary>
        /// <see cref="WavefrontConfig"/>クラスの新しいインスタンスを生成します。
        /// </summary>
        public WavefrontConfig()
        {
            initializeParameter();
        }

        /// <summary>
        /// <see cref="WavefrontConfig"/>クラスの新しいインスタンスを生成します。
        /// </summary>
        /// <param name="wavefront"><see cref="IWavefront"/>を実装したインスタンス</param>
        public WavefrontConfig(IWavefront wavefront)
        {
            initializeParameter(wavefront);
        }
        #endregion // Constructors

        #region Events
        /// <summary>
        /// プロパティ値が変更するときに発生します。
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
        /// 波面収差計算用方程式設定
        /// </summary>
        [DataMember]
        public EquationConfig Equation { get; private set; }

        /// <summary>
        /// 画像処理設定
        /// </summary>
        [DataMember]
        public ImageConfig Image { get; private set; }

        /// <summary>
        /// 光源情報設定
        /// </summary>
        [DataMember]
        public LightConfig Light { get; private set; }

        /// <summary>
        /// Point Spread Function演算設定
        /// </summary>
        [DataMember]
        public PsfConfig Psf { get; private set; }

        /// <summary>
        /// センサーカメラ構成情報設定
        /// </summary>
        [DataMember]
        public SensorConfig Sensor { get; private set; }
        #endregion // Properties

        #region Methods
        private void initializeParameter()
        {
            Equation = new EquationConfig();
            Image = new ImageConfig();
            Light = new LightConfig();
            Psf = new PsfConfig();
            Sensor = new SensorConfig();

            Equation.PropertyChanged += OnPropertyChanged;
            Image.PropertyChanged += OnPropertyChanged;
            Light.PropertyChanged += OnPropertyChanged;
            Psf.PropertyChanged += OnPropertyChanged;
            Sensor.PropertyChanged += OnPropertyChanged;
        }

        private void initializeParameter(IWavefront wavefront)
        {
            Equation = new EquationConfig(wavefront);
            Image = new ImageConfig(wavefront);
            Light = new LightConfig(wavefront);
            Psf = new PsfConfig(wavefront);
            Sensor = new SensorConfig(wavefront);

            Equation.PropertyChanged += OnPropertyChanged;
            Sensor.PropertyChanged += OnPropertyChanged;
            Image.PropertyChanged += OnPropertyChanged;
            Light.PropertyChanged += OnPropertyChanged;
            Psf.PropertyChanged += OnPropertyChanged;
        }

        [OnDeserializing]
        private void OnDeserializingMethod(StreamingContext sc)
        {
            initializeParameter();
        }

        protected void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(sender, e);
        }

        /// <summary>
        /// 指定された波面センサーに各プロパティを関連付けます。
        /// </summary>
        /// <param name="wavefront">波面センサーのインスタンス</param>
        /// <remarks>現在設定されているパラメーターの値がアタッチしたビームプロファイラーに反映されます。</remarks>
        public void Attach(IWavefront wavefront)
        {
            Equation.Attach(wavefront);
            Image.Attach(wavefront);
            Light.Attach(wavefront);
            Psf.Attach(wavefront);
            Sensor.Attach(wavefront);
        }

        /// <summary>
        /// 波面センサーとの関連付けを解除します。
        /// </summary>
        public void Detach()
        {
            Equation.Detach();
            Image.Detach();
            Light.Detach();
            Psf.Detach();
            Sensor.Detach();
        }

        /// <summary>
        /// 指定された設定データを反映します。
        /// </summary>
        /// <param name="source">反映元データ</param>
        public void Apply(WavefrontConfig source)
        {
            if (source == null)
            {
                return;
            }

            Equation.Apply(source.Equation);
            Image.Apply(source.Image);
            Light.Apply(source.Light);
            Psf.Apply(source.Psf);
            Sensor.Apply(source.Sensor);
        }
        #endregion // Methods
    }
}
