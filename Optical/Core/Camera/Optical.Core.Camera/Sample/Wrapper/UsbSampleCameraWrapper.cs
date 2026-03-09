using Optical.Platform.Types;

namespace Optical.Core.Camera
{
    /// <summary>
    /// USB接続専用ラッパークラス
    /// </summary>
    internal class UsbSampleCameraWrapper : SampleCameraWrapper
    {
        #region Constructors
        public UsbSampleCameraWrapper(string serialNumber) : base(serialNumber) { }
        #endregion // Constructors

        #region Properties
        public override bool AutoCenter { get; set; }

		public override bool AutoExposure { get; set; }

		public override double AutoExposureLower { get; set; }

		public override Limit<double> AutoExposureLowerRange { get; init; } = new Limit<double>();

        public override double AutoExposureUpper { get; set; }

		public override Limit<double> AutoExposureUpperRange { get; init; } = new Limit<double>();

        public override bool AutoGain { get; set; }

		public override double AutoGainLower { get; set; }

		public override Limit<double> AutoGainLowerRange { get; init; } = new Limit<double>();

        public override double AutoGainUpper { get; set; }

		public override Limit<double> AutoGainUpperRange { get; init; } = new Limit<double>();

        public override int Binning { get; set; }

		public override BinningPixelFormat BinningMode { get; set; }

		public override int BitDepth { get; set; }

		public override double BlackLevel { get; set; }

		public override double ExposureTime { get; set; }

		public override Limit<double> ExposureTimeRange { get; init; } = new Limit<double>();

        public override bool FlipHorizontal { get; set; }

		public override bool FlipVertical { get; set; }

		public override double FrameRate { get; set; }

		public override bool FrameRateEnabled { get; set; }

		public override Limit<double> FrameRateRange { get; init; } = new Limit<double>();

        public override double Gain { get; set; }

		public override Limit<double> GainRange { get; init; } = new Limit<double>();

        public override string PixelFormat { get; set; } =  string.Empty;

		public override string[] PixelFormatList { get; init; } = [];

        public override double TriggerDelay { get; set; }
		#endregion // Properties

		#region Methods
		public override void Initialize() { }
        #endregion // Methods
    }
}
