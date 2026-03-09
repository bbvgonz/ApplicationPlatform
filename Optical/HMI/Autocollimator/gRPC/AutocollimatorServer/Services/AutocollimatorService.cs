using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Optical.HMI.Autocollimator.Grpc;
using Optical.API.Autocollimator;
using Optical.API.Autocollimator.Sample;
using Optical.API.Library.Optics;
using System.Drawing;
using Optical.Platform.Types;
using Optical.API.Library.Device;
using Google.Protobuf;
using Optical.Enums;

namespace AutocollimatorServer.Services
{
    public class AutocollimatorService : Autocollimatorh.AutocollimatorhBase
    {
        #region Fields
        private readonly ILogger<AutocollimatorService> _logger;
        private IAutocollimator autocollimator;

        private ManualResetEventSlim newFrameResetEvent;
        #endregion // Fields

        #region Constructors
        public AutocollimatorService(ILogger<AutocollimatorService> logger)
        {
            _logger = logger;
            
            newFrameResetEvent = new ManualResetEventSlim(false);

        }
        #endregion // Constructors

        #region Properties

        #endregion // Properties

        #region Methods
        public override Task<NotifyReply> GenerateInstanceNotify(SensorDevice request, ServerCallContext context)
        {
            var sensor = new SensorComponents()
            {
                ConnectionType = request.ConnectionType,
                DeviceID = request.DeviceId,
                Key = request.Key,
                ModelName = request.ModelName,
                SerialNumber = request.SerialNumber,
                VendorName = request.VendorName
            };
            autocollimator = new LaserAutocollimator(sensor);
            return Task.FromResult(new NotifyReply { Result = true });
        }


        public override async Task NewFrameEvent(Empty request, IServerStreamWriter<BufferUpdated> responseStream, ServerCallContext context)
        {
            int index = 0;
            void Autocollimator_NewFrame(object? sender, int e)
            {
                if (newFrameResetEvent.IsSet)
                {
                    return;
                }

                index = e;
                newFrameResetEvent.Set();
            }

            autocollimator.NewFrame += Autocollimator_NewFrame;

            try
            {
                while (!context.CancellationToken.IsCancellationRequested)
                {
                    newFrameResetEvent.Wait(context.CancellationToken);
                    if (!newFrameResetEvent.IsSet)
                    {
                        break;
                    }

                    await responseStream.WriteAsync(new BufferUpdated { TargetIndex = index });
                    newFrameResetEvent.Reset();
                }
            }
            catch (OperationCanceledException)
            {
                autocollimator.NewFrame -= Autocollimator_NewFrame;
            }
        }

        public override Task NewResultEvent(Empty request, IServerStreamWriter<BufferUpdated> responseStream, ServerCallContext context)
        {
            return base.NewResultEvent(request, responseStream, context);
        }


        public override Task<NotifyReply> AutoExposureNotify(AutoExposure request, ServerCallContext context)
        {
            autocollimator.AutoExposure = request.Enabled;
            return Task.FromResult(new NotifyReply { Result = true });
        }

        public override Task<AutoExposure> AutoExposureRequest(Empty request, ServerCallContext context)
        {
            return Task.FromResult(new AutoExposure
            {
                Enabled = autocollimator.AutoExposure
            });
        }

        public override Task<NotifyReply> AutoGainNotify(AutoGain request, ServerCallContext context)
        {
            autocollimator.AutoGain = request.Enabled;
            return Task.FromResult(new NotifyReply { Result = true });
        }

        public override Task<AutoGain> AutoGainRequest(Empty request, ServerCallContext context)
        {
            return Task.FromResult(new AutoGain
            {
                Enabled = autocollimator.AutoGain
            });
        }

        public override Task<NotifyReply> AveragingTimesNotify(Averaging request, ServerCallContext context)
        {
            autocollimator.AveragingTimes = request.Times;
            return Task.FromResult(new NotifyReply { Result = true });
        }

        public override Task<Averaging> AveragingTimesRequest(Empty empty, ServerCallContext context)
        {
            return Task.FromResult(new Averaging
            {
                Times = autocollimator.AveragingTimes
            });
        }

        public override Task<NotifyReply> BinningModeNotify(BinningMode request, ServerCallContext context)
        {
            autocollimator.BinningMode = request.Format switch
            {
                PixelFormat.Average => BinningPixelFormat.Average,
                PixelFormat.Sum => BinningPixelFormat.Sum,
                _ => throw new ArgumentOutOfRangeException(nameof(request.Format))
            };
            return Task.FromResult(new NotifyReply { Result = true });
        }

        public override Task<BinningMode> BinningModeRequest(Empty request, ServerCallContext context)
        {
            PixelFormat format = autocollimator.BinningMode switch
            {
                BinningPixelFormat.Average => PixelFormat.Average,
                BinningPixelFormat.Sum => PixelFormat.Sum,
                _ => throw new IndexOutOfRangeException(nameof(autocollimator.BinningMode))
            };
            return Task.FromResult(new BinningMode { Format = format });
        }

        public override Task<NotifyReply> BinningNotify(Binning request, ServerCallContext context)
        {
            autocollimator.Binning = request.PixelCount;
            return Task.FromResult(new NotifyReply { Result = true });
        }

        public override Task<Binning> BinningRequest(Empty request, ServerCallContext context)
        {
            return Task.FromResult(new Binning { PixelCount = autocollimator.Binning });
        }

        public override Task<NotifyReply> BitDepthNotify(BitDepth request, ServerCallContext context)
        {
            autocollimator.BitDepth = request.Value;
            return Task.FromResult(new NotifyReply { Result = true });
        }

        public override Task<BitDepth> BitDepthRequest(Empty request, ServerCallContext context)
        {
            return Task.FromResult(new BitDepth { Value = autocollimator.BitDepth });
        }

        public override Task<NotifyReply> BlackLevelNotify(BlackLevel request, ServerCallContext context)
        {
            autocollimator.BlackLevel = request.Value;
            return Task.FromResult(new NotifyReply { Result = true });
        }

        public override Task<BlackLevel> BlackLevelRequest(Empty request, ServerCallContext context)
        {
            return Task.FromResult(new BlackLevel { Value = autocollimator.BlackLevel });
        }

        public override Task<NotifyReply> CentroidTypeNotify(CentroidType request, ServerCallContext context)
        {
            autocollimator.CentroidType = request.Centroid switch
            {
                CentroidAnalysis.Area => CentroidMethod.Area,
                CentroidAnalysis.Luminance => CentroidMethod.Luminance,
                _ => throw new ArgumentOutOfRangeException(nameof(request.Centroid))
            };
            return Task.FromResult(new NotifyReply { Result = true });
        }

        public override Task<CentroidType> CentroidTypeRequest(Empty request, ServerCallContext context)
        {
            CentroidAnalysis centroid = autocollimator.CentroidType switch
            {
                CentroidMethod.Area => CentroidAnalysis.Area,
                CentroidMethod.Luminance => CentroidAnalysis.Luminance,
                _ => throw new IndexOutOfRangeException(nameof(autocollimator.CentroidType))
            };
            return Task.FromResult(new CentroidType { Centroid = centroid });
        }

        public override Task<NotifyReply> CloseNotify(Empty request, ServerCallContext context)
        {
            autocollimator.Close();
            return Task.FromResult(new NotifyReply { Result = true });
        }

        public override Task<SensorComponentsReply> DeviceRequest(Empty request, ServerCallContext context)
        {
            var sensor = new SensorComponentsReply()
            {
                ConnectionType = autocollimator.Device.ConnectionType,
                DeviceId = autocollimator.Device.DeviceID,
                Key = autocollimator.Device.Key,
                ModelName = autocollimator.Device.ModelName,
                SerialNumber = autocollimator.Device.SerialNumber,
                VendorName = autocollimator.Device.VendorName
            };
            return Task.FromResult(sensor);
        }

        public override Task<NotifyReply> ExposureTimeNotify(ExposureTime request, ServerCallContext context)
        {
            autocollimator.ExposureTime = request.Value;
            return Task.FromResult(new NotifyReply { Result = true });
        }

        public override Task<ExposureTimeRangeReply> ExposureTimeRangeRequest(Empty request, ServerCallContext context)
        {
            var range = new ExposureTimeRangeReply
            {
                Maximum = autocollimator.ExposureTimeRange.Maximum,
                Minimum = autocollimator.ExposureTimeRange.Minimum
            };
            return Task.FromResult(range);
        }

        public override Task<ExposureTime> ExposureTimeRequest(Empty request, ServerCallContext context)
        {
            return Task.FromResult(new ExposureTime { Value = autocollimator.ExposureTime });
        }

        public override Task<NotifyReply> FlipHorizontalNotify(FlipHorizontal request, ServerCallContext context)
        {
            autocollimator.FlipHorizontal = request.Enabled;
            return Task.FromResult(new NotifyReply { Result = true });
        }

        public override Task<FlipHorizontal> FlipHorizontalRequest(Empty request, ServerCallContext context)
        {
            return Task.FromResult(new FlipHorizontal { Enabled = autocollimator.FlipHorizontal });
        }

        public override Task<NotifyReply> FlipVerticalNotify(FlipVertical request, ServerCallContext context)
        {
            autocollimator.FlipVertical = request.Enabled;
            return Task.FromResult(new NotifyReply { Result = true });
        }

        public override Task<FlipVertical> FlipVerticalRequest(Empty request, ServerCallContext context)
        {
            return Task.FromResult(new FlipVertical { Enabled = autocollimator.FlipVertical });
        }

        public override Task<NotifyReply> FrameRateNotify(FrameRate request, ServerCallContext context)
        {
            autocollimator.FrameRate = request.Value;
            return Task.FromResult(new NotifyReply { Result = true });
        }

        public override Task<FrameRate> FrameRateRequest(Empty request, ServerCallContext context)
        {
            return Task.FromResult(new FrameRate { Value = autocollimator.FrameRate });
        }

        public override Task<FrameRateRangeReply> FrameRateRangeRequest(Empty request, ServerCallContext context)
        {
            var range = new FrameRateRangeReply
            {
                Maximum = autocollimator.FrameRateRange.Maximum,
                Minimum = autocollimator.FrameRateRange.Minimum
            };
            return Task.FromResult(range);
        }

        public override Task<NotifyReply> GainNotify(Gain request, ServerCallContext context)
        {
            autocollimator.Gain = request.Value;
            return Task.FromResult(new NotifyReply { Result = true });
        }

        public override Task<GainRangeReply> GainRangeRequest(Empty request, ServerCallContext context)
        {
            var range = new GainRangeReply
            {
                Maximum = autocollimator.GainRange.Maximum,
                Minimum = autocollimator.GainRange.Minimum
            };
            return Task.FromResult(range);
        }

        public override Task<Gain> GainRequest(Empty request, ServerCallContext context)
        {
            return Task.FromResult(new Gain { Value = autocollimator.Gain });
        }

        public override Task<NotifyReply> InternalLightSourceNotify(LightSource request, ServerCallContext context)
        {
            autocollimator.InternalLightSource = request.EnableInternal;
            return Task.FromResult(new NotifyReply { Result = true });
        }

        public override Task<LightSource> InternalLightSourceRequest(Empty request, ServerCallContext context)
        {
            return Task.FromResult(new LightSource { EnableInternal = autocollimator.InternalLightSource });
        }

        public override Task<BufferReply> LatestBufferRequest(Empty request, ServerCallContext context)
        {
            var latestBuffer = autocollimator.LatestBuffer;
            var buffer = new BufferReply
            {
                Image = new Optical.HMI.Autocollimator.Grpc.ImageContainer
                {
                    BitDepth = latestBuffer.Image.BitDepth,
                    FrameNumber = latestBuffer.Image.FrameNumber,
                    Height = latestBuffer.Image.Height,
                    Id = latestBuffer.Image.Id,
                    RawData = new Optical.HMI.Autocollimator.Grpc.ImageContainer.Types.ImageComponent()
                    {
                        BitDepth = latestBuffer.Image.BitDepth,
                        Height = latestBuffer.Image.Height,
                        Pixels = ByteString.CopyFrom(latestBuffer.Image.RawData.Pixels),
                        Width = latestBuffer.Image.Width
                    },
                    Timestamp = Duration.FromTimeSpan(latestBuffer.Image.Timestamp),
                    Width = latestBuffer.Image.Width
                },
                IsCompleted = latestBuffer.IsCompleted,
            };
            foreach (var item in latestBuffer.Tilts)
            {
                var angle = new Angle()
                {
                    X = item.Tilt.X,
                    Y = item.Tilt.Y
                };
                buffer.Tilts.Add(angle);
            }

            return Task.FromResult(buffer);
        }

        public override Task<NotifyReply> MultiModeNotify(MeasureMode request, ServerCallContext context)
        {
            autocollimator.MultiMode = request.MultiMode;
            return Task.FromResult(new NotifyReply { Result = true });
        }

        public override Task<MeasureMode> MultiModeRequest(Empty request, ServerCallContext context)
        {
            return Task.FromResult(new MeasureMode { MultiMode = autocollimator.MultiMode });
        }

        public override Task<NotifyReply> OpenNotify(Empty request, ServerCallContext context)
        {
            autocollimator.Open();
            return Task.FromResult(new NotifyReply { Result = true });
        }

        public override Task<NotifyReply> RoiNotify(TargetArea request, ServerCallContext context)
        {
            autocollimator.Roi.Clear();
            foreach (var item in request.Roi)
            {
                var roi = new Rectangle(item.Left, item.Top, item.Width, item.Height);
                autocollimator.Roi.Add(roi);
            }

            return Task.FromResult(new NotifyReply { Result = true });
        }

        public override Task<TargetArea> RoiRequest(Empty request, ServerCallContext context)
        {
            var areas = new TargetArea();
            foreach (var roi in autocollimator.Roi)
            {
                var area = new TargetArea.Types.Rectangle
                {
                    Left = roi.Left,
                    Top = roi.Top,
                    Height = roi.Height,
                    Width = roi.Width
                };
                areas.Roi.Add(area);
            }

            return base.RoiRequest(request, context);
        }

        public override Task<SensorSizeReply> SensorSizeRequest(Empty request, ServerCallContext context)
        {
            return Task.FromResult(new SensorSizeReply
            {
                Height = autocollimator.SensorSize.Height,
                Width = autocollimator.SensorSize.Width
            });
        }

        public override Task<NotifyReply> StartNotify(Empty request, ServerCallContext context)
        {
            autocollimator.Start();
            return Task.FromResult(new NotifyReply { Result = true });
        }

        public override Task<NotifyReply> StopNotify(Empty request, ServerCallContext context)
        {
            autocollimator.Stop();
            return Task.FromResult(new NotifyReply { Result = false });
        }

        public override Task<TiltReply> TiltRequest(Empty request, ServerCallContext context)
        {
            var reply = new TiltReply();
            var latestBuffer = autocollimator.LatestBuffer;
            foreach (var item in latestBuffer.Tilts)
            {
                var angle = new Angle()
                {
                    X = item.Tilt.X,
                    Y = item.Tilt.Y
                };
                reply.Tilt.Add(angle);
            }

            return Task.FromResult(reply);
        }

        public override Task<NotifyReply> TriggerDelayNotify(TriggerDelay request, ServerCallContext context)
        {
            autocollimator.TriggerDelay = request.DelayTime;
            return Task.FromResult(new NotifyReply { Result = true });
        }

        public override Task<TriggerDelay> TriggerDelayRequest(Empty request, ServerCallContext context)
        {
            return Task.FromResult(new TriggerDelay { DelayTime = autocollimator.TriggerDelay });
        }

        public override Task<NotifyReply> TriggerModeNotify(TriggerMode request, ServerCallContext context)
        {
            autocollimator.TriggerMode = request.Enabled;
            return Task.FromResult(new NotifyReply { Result = true });
        }

        public override Task<TriggerMode> TriggerModeRequest(Empty request, ServerCallContext context)
        {
            return Task.FromResult(new TriggerMode { Enabled = autocollimator.TriggerMode });
        }

        public override Task<NotifyReply> TriggerTypeNotify(TriggerType request, ServerCallContext context)
        {
            autocollimator.TriggerType = request.Type switch
            {
                TriggerOperation.Hardware => TriggerInput.Hardware,
                TriggerOperation.Software => TriggerInput.Software,
                _ => throw new ArgumentOutOfRangeException(nameof(request.Type))
            };
            return Task.FromResult(new NotifyReply { Result = true });
        }

        public override Task<TriggerType> TriggerTypeRequest(Empty request, ServerCallContext context)
        {
            var trigger = new TriggerType();
            trigger.Type = autocollimator.TriggerType switch
            {
                TriggerInput.Hardware => TriggerOperation.Hardware,
                TriggerInput.Software => TriggerOperation.Software,
                _ => throw new IndexOutOfRangeException(nameof(autocollimator.TriggerType))
            };
            return Task.FromResult(trigger);
        }
        #endregion // Methods
    }
}