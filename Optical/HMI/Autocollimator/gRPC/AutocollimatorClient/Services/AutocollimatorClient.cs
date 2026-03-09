using Grpc.Net.Client;
using Optical.API.Autocollimator;
using Optical.API.Library.Device;
using Optical.API.Library.Optics;
using Optical.Platform.Types;
using System.Drawing;
using Optical.HMI.Autocollimator.Grpc;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Optical.Enums;

namespace AutocollimatorClient.Services
{
    public class AutocollimatorClient : IAutocollimator
    {
        #region Fields
        private const int defaultTimeout = 5;

        private static readonly Camera.CameraClient sensor;

        private readonly Autocollimatorh.AutocollimatorhClient client;
        private readonly SensorComponents device;

        private EventHandler<int>? newFrameEvent;
        private CancellationTokenSource newFrameEventCanceller;

        private EventHandler<int>? newResultEvent;
        private CancellationTokenSource newResultEventCanceller;
        #endregion // Fields

        #region Constructors
        static AutocollimatorClient()
        {
            var channel = GrpcChannel.ForAddress("https://localhost:5002");
            sensor = new Camera.CameraClient(channel);
        }

        public AutocollimatorClient(SensorComponents component)
        {
            device = component;

            newFrameEventCanceller = new CancellationTokenSource();
            newResultEventCanceller = new CancellationTokenSource();

            var channel = GrpcChannel.ForAddress("https://localhost:5001");
            client = new Autocollimatorh.AutocollimatorhClient(channel);

            var sensor = new SensorDevice()
            {
                ConnectionType = component.ConnectionType,
                DeviceId = component.DeviceID,
                Key = component.Key,
                ModelName = component.ModelName,
                SerialNumber = component.SerialNumber,
                VendorName = component.VendorName
            };
            try
            {
                var reply = client.GenerateInstanceNotify(sensor, deadline: DateTime.Now.AddSeconds(defaultTimeout));
                if (!reply.Result)
                {
                    throw new HttpRequestException($"Failed to generate an instance.");
                }
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.DeadlineExceeded)
            {
                throw new HttpRequestException($"Communication timeout.({defaultTimeout}[sec])");
            }
        }
        #endregion // Constructors

        #region Events
        public event EventHandler<int> NewFrame
        {
            add
            {
                int eventCount = newFrameEvent?.GetInvocationList().Length ?? 0;
                if (eventCount == 0)
                {
                    if (!newFrameEventCanceller.TryReset())
                    {
                        newFrameEventCanceller = new CancellationTokenSource();
                    }

                    var response = client.NewFrameEvent(new Empty(), cancellationToken: newFrameEventCanceller.Token);
                    _ = Task.Run(async () =>
                    {
                        await foreach (var update in response.ResponseStream.ReadAllAsync(newFrameEventCanceller.Token))
                        {
                            newFrameEvent?.Invoke(this, update.TargetIndex);
                        }
                    });
                }

                newFrameEvent += value;
            }

            remove
            {
                int preCount = newFrameEvent?.GetInvocationList().Length ?? 0;
                if (preCount > 0)
                {
                    newFrameEvent -= value;
                }

                int postCount = newFrameEvent?.GetInvocationList().Length ?? 0;
                if (postCount == 0)
                {
                    newFrameEventCanceller?.Cancel();
                }
            }
        }

        public event EventHandler<int> NewResult
        {
            add
            {
                int eventCount = newResultEvent?.GetInvocationList().Length ?? 0;
                if (eventCount == 0)
                {
                    if (!newResultEventCanceller.TryReset())
                    {
                        newResultEventCanceller = new CancellationTokenSource();
                    }

                    var response = client.NewResultEvent(new Empty(), cancellationToken: newResultEventCanceller.Token);
                    _ = Task.Run(async () =>
                    {
                        await foreach (var update in response.ResponseStream.ReadAllAsync(newResultEventCanceller.Token))
                        {
                            newResultEvent?.Invoke(this, update.TargetIndex);
                        }
                    });
                }

                newResultEvent += value;
            }

            remove
            {
                int preCount = newResultEvent?.GetInvocationList().Length ?? 0;
                if (preCount > 0)
                {
                    newResultEvent -= value;
                }

                int postCount = newResultEvent?.GetInvocationList().Length ?? 0;
                if (postCount == 0)
                {
                    newResultEventCanceller?.Cancel();
                }
            }
        }
        #endregion // Events

        #region Properties
        public int AveragingTimes
        {
            get => client.AveragingTimesRequest(new Empty()).Times;
            set
            {
                var reply = client.AveragingTimesNotify(new Averaging { Times = value });
                if (!reply.Result)
                {
                    throw new HttpRequestException($"Failed to set the parameter. {value}");
                }
            }
        }

        public CentroidMethod CentroidType
        {
            get
            {
                var type = client.CentroidTypeRequest(new Empty()).Centroid;
                return type switch
                {
                    CentroidAnalysis.Area => CentroidMethod.Area,
                    CentroidAnalysis.Luminance => CentroidMethod.Luminance,
                    _ => throw new IndexOutOfRangeException($"An unknown type was obtained.:{type}"),
                };
            }

            set
            {
                var centroid = value switch
                {
                    CentroidMethod.Area => CentroidAnalysis.Area,
                    CentroidMethod.Luminance => CentroidAnalysis.Luminance,
                    _ => throw new IndexOutOfRangeException($"An unknown type was specified.:{value}"),
                };
                var reply = client.CentroidTypeNotify(new CentroidType { Centroid = centroid });
                if (!reply.Result)
                {
                    throw new HttpRequestException($"Failed to set the parameter. {value}");
                }
            }
        }

        public bool InternalLightSource
        {
            get => client.InternalLightSourceRequest(new Empty()).EnableInternal;
            set
            {
                var reply = client.InternalLightSourceNotify(new LightSource { EnableInternal = value });
                if (!reply.Result)
                {
                    throw new HttpRequestException($"Failed to set the parameter. {value}");
                }
            }
        }

        public TiltContainer LatestBuffer
        {
            get
            {
                var buffer = client.LatestBufferRequest(new Empty());
                var tilts = new TiltContainer();
                tilts.Image.Id = buffer.Image.Id;
                tilts.Image.Timestamp = buffer.Image.Timestamp.ToTimeSpan();
                tilts.Image.FrameNumber = buffer.Image.FrameNumber;
                tilts.Image.Height = buffer.Image.Height;
                tilts.Image.Width = buffer.Image.Width;
                tilts.Image.BitDepth = buffer.Image.BitDepth;
                tilts.Image.RawData = new ImageComponent(buffer.Image.Height, buffer.Image.Width, buffer.Image.BitDepth)
                {
                    Pixels = buffer.Image.RawData.Pixels.ToByteArray()
                };
                return tilts;
            }
        }

        public bool MultiMode
        {
            get => client.MultiModeRequest(new Empty()).MultiMode;
            set
            {
                var reply = client.MultiModeNotify(new MeasureMode { MultiMode = value });
                if (!reply.Result)
                {
                    throw new HttpRequestException($"Failed to set the parameter. {value}");
                }
            }
        }

        public List<Rectangle> Roi
        {
            get
            {
                var rois = client.RoiRequest(new Empty()).Roi;
                var rectangles = new List<Rectangle>(rois.Count);
                for (int index = 0; index < rois.Count; index++)
                {
                    var rectangle = new Rectangle(rois[index].Left, rois[index].Top, rois[index].Width, rois[index].Height);
                    rectangles.Add(rectangle);
                }

                return rectangles;
            }

            set
            {
                var area = new TargetArea();
                for (int index = 0; index < value.Count; index++)
                {
                    var rectangle = new TargetArea.Types.Rectangle()
                    {
                        Height = value[index].Height,
                        Left = value[index].Left,
                        Top = value[index].Top,
                        Width = value[index].Width
                    };
                    area.Roi.Add(rectangle);
                }

                var reply = client.RoiNotify(area);
                if (!reply.Result)
                {
                    throw new HttpRequestException($"Failed to set the parameter. {value}");
                }
            }
        }

        public List<AngleD> Tilt
        {
            get
            {
                var tilt = client.TiltRequest(new Empty()).Tilt;
                var angles = new List<AngleD>(tilt.Count);
                for (var index = 0; index < tilt.Count; index++)
                {
                    var angle = new AngleD(tilt[index].X, tilt[index].Y);
                    angles.Add(angle);
                }

                return angles;
            }
        }

        public bool AutoExposure
        {
            get => client.AutoExposureRequest(new Empty()).Enabled;
            set
            {
                var reply = client.AutoExposureNotify(new AutoExposure { Enabled = value });
                if (!reply.Result)
                {
                    throw new HttpRequestException($"Failed to set the parameter. {value}");
                }
            }
        }

        public bool AutoGain
        {
            get => client.AutoGainRequest(new Empty()).Enabled;
            set
            {
                var reply = client.AutoGainNotify(new AutoGain { Enabled = value });
                if (!reply.Result)
                {
                    throw new HttpRequestException($"Failed to set the parameter. {value}");
                }
            }
        }

        public int Binning
        {
            get => client.BinningRequest(new Empty()).PixelCount;
            set
            {
                var reply = client.BinningNotify(new Binning { PixelCount = value });
                if (!reply.Result)
                {
                    throw new HttpRequestException($"Failed to set the parameter. {value}");
                }
            }
        }

        public BinningPixelFormat BinningMode
        {
            get
            {
                var format = client.BinningModeRequest(new Empty()).Format;
                return format switch
                {
                    PixelFormat.Average => BinningPixelFormat.Average,
                    PixelFormat.Sum => BinningPixelFormat.Sum,
                    _ => throw new IndexOutOfRangeException($"An unknown type was obtained.:{format}"),
                };
            }

            set
            {
                var format = value switch
                {
                    BinningPixelFormat.Average => PixelFormat.Average,
                    BinningPixelFormat.Sum => PixelFormat.Sum,
                    _ => throw new IndexOutOfRangeException($"An unknown type was specified.:{value}"),
                };
                var reply = client.BinningModeNotify(new BinningMode { Format = format });
                if (!reply.Result)
                {
                    throw new HttpRequestException($"Failed to set the parameter. {value}");
                }
            }
        }

        public int BitDepth
        {
            get => client.BitDepthRequest(new Empty()).Value;
            set
            {
                var reply = client.BitDepthNotify(new BitDepth { Value = value });
                if (!reply.Result)
                {
                    throw new HttpRequestException($"Failed to set the parameter. {value}");
                }
            }
        }

        public double BlackLevel
        {
            get => client.BlackLevelRequest(new Empty()).Value;
            set
            {
                var reply = client.BlackLevelNotify(new BlackLevel { Value = value });
                if (!reply.Result)
                {
                    throw new HttpRequestException($"Failed to set the parameter. {value}");
                }
            }
        }

        public SensorComponents Device
        {
            get
            {
                var reply = client.DeviceRequest(new Empty());
                var device = new SensorComponents()
                {
                    ConnectionType = reply.ConnectionType,
                    DeviceID = reply.DeviceId,
                    Key = reply.Key,
                    ModelName = reply.ModelName,
                    SerialNumber = reply.SerialNumber,
                    VendorName = reply.VendorName
                };
                return device;
            }
        }

        public double ExposureTime
        {
            get => client.ExposureTimeRequest(new Empty()).Value;
            set
            {
                var reply = client.ExposureTimeNotify(new ExposureTime { Value = value });
                if (!reply.Result)
                {
                    throw new HttpRequestException($"Failed to set the parameter. {value}");
                }
            }
        }

        public Limit<double> ExposureTimeRange
        {
            get
            {
                var reply = client.ExposureTimeRangeRequest(new Empty());
                var range = new Limit<double>(reply.Maximum, reply.Minimum);
                return range;
            }
        }

        public bool FlipHorizontal
        {
            get => client.FlipHorizontalRequest(new Empty()).Enabled;
            set
            {
                var reply = client.FlipHorizontalNotify(new FlipHorizontal { Enabled = value });
                if (!reply.Result)
                {
                    throw new HttpRequestException($"Failed to set the parameter. {value}");
                }
            }
        }

        public bool FlipVertical
        {
            get => client.FlipVerticalRequest(new Empty()).Enabled;
            set
            {
                var reply = client.FlipVerticalNotify(new FlipVertical { Enabled = value });
                if (!reply.Result)
                {
                    throw new HttpRequestException($"Failed to set the parameter. {value}");
                }
            }
        }

        public double FrameRate
        {
            get => client.FrameRateRequest(new Empty()).Value;
            set
            {
                var reply = client.FrameRateNotify(new FrameRate { Value = value });
                if (!reply.Result)
                {
                    throw new HttpRequestException($"Failed to set the parameter. {value}");
                }
            }
        }

        public Limit<double> FrameRateRange
        {
            get
            {
                var reply = client.FrameRateRangeRequest(new Empty());
                var range = new Limit<double>(reply.Maximum, reply.Minimum);
                return range;
            }
        }


        public double Gain
        {
            get => client.GainRequest(new Empty()).Value;
            set
            {
                var reply = client.GainNotify(new Gain { Value = value });
                if (!reply.Result)
                {
                    throw new HttpRequestException($"Failed to set the parameter. {value}");
                }
            }
        }

        public Limit<double> GainRange
        {
            get
            {
                var reply = client.GainRangeRequest(new Empty());
                var range = new Limit<double>(reply.Maximum, reply.Minimum);
                return range;
            }
        }

        public Size<int> SensorSize
        {
            get
            {
                var reply = client.SensorSizeRequest(new Empty());
                var size = new Size<int>(reply.Width, reply.Height);
                return size;
            }
        }

        public double TriggerDelay
        {
            get => client.TriggerDelayRequest(new Empty()).DelayTime;
            set
            {
                var reply = client.TriggerDelayNotify(new TriggerDelay { DelayTime = value });
                if (!reply.Result)
                {
                    throw new HttpRequestException($"Failed to set the parameter. {value}");
                }
            }
        }

        public bool TriggerMode
        {
            get => client.TriggerModeRequest(new Empty()).Enabled;
            set
            {
                var reply = client.TriggerModeNotify(new TriggerMode { Enabled = value });
                if (!reply.Result)
                {
                    throw new HttpRequestException($"Failed to set the parameter. {value}");
                }
            }
        }

        public TriggerInput TriggerType
        {
            get
            {
                var type = client.TriggerTypeRequest(new Empty()).Type;
                return type switch
                {
                    TriggerOperation.Hardware => TriggerInput.Hardware,
                    TriggerOperation.Software => TriggerInput.Software,
                    _ => throw new IndexOutOfRangeException($"An unknown type was obtained.:{type}"),
                };
            }

            set
            {
                var type = value switch
                {
                    TriggerInput.Hardware => TriggerOperation.Hardware,
                    TriggerInput.Software => TriggerOperation.Software,
                    _ => throw new IndexOutOfRangeException($"An unknown type was specified.:{value}")
                };

                var reply = client.TriggerTypeNotify(new TriggerType { Type = type });
                if (!reply.Result)
                {
                    throw new HttpRequestException($"Failed to set the parameter. {value}");
                }
            }
        }
        #endregion // Properties

        #region Methods
        public void Close()
        {
            client.CloseNotify(new Empty());
        }

        IReadOnlyList<SensorComponents> IAutocollimator.EnumerateDevice()
        {
            throw new NotImplementedException();
        }

        public static IReadOnlyList<SensorComponents> EnumerateDevice()
        {
            var devices = sensor.EnumerateDeviceRequest(new Empty());
            List<SensorComponents> sensors = new List<SensorComponents>();
            foreach (var device in devices.Devices)
            {
                var sensor = new SensorComponents()
                {
                    ConnectionType = device.ConnectionType,
                    DeviceID = device.DeviceId,
                    Key = device.Key,
                    ModelName = device.ModelName,
                    SerialNumber = device.SerialNumber,
                    VendorName = device.VendorName
                };
                sensors.Add(sensor);
            }

            return sensors;
        }

        public void Open()
        {
            client.OpenNotify(new Empty());
        }

        /// <summary>
        /// ※未使用
        /// </summary>
        /// <param name="filePath"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void ReadCalibrationFile(string filePath)
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            client.StartNotify(new Empty());
        }

        public void Stop()
        {
            client.StopNotify(new Empty());
        }
        #endregion // Methods
    }
}
