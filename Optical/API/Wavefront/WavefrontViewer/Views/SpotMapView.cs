using Optical.API.Library;
using Optical.API.Library.Optics;
using Optical.API.Wavefront;
using Optical.Platform.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace WavefrontTester.Views
{
    public partial class SpotMapView : UserControl
    {
        #region Fields
        private Bitmap[] drawBuffers;

        private int bufferCounter;

        private SemaphoreSlim drawEvent;
        private Size<double> apertureSize;
        private PointD apertureCenter;
        private ApertureShape apertureType;
        #endregion

        #region Constructors
        public SpotMapView()
        {
            InitializeComponent();
            initializeParameter();
        }
        #endregion

        #region Properties
        public PointD ApertureCenter
        {
            get => apertureCenter;
            set
            {
                if (value == null)
                {
                    return;
                }

                apertureCenter.XY(value.X, value.Y);
            }
        }

        public bool ApertureCenterTracking { get; set; }

        public Size<double> ApertureSize
        {
            get => apertureSize;
            set
            {
                if (value == null)
                {
                    return;
                }

                apertureSize.Height = value.Height;
                apertureSize.Width = value.Width;
            }
        }

        public ApertureShape ApertureType
        {
            get => apertureType;
            set
            {
                if (value == apertureType)
                {
                    return;
                }

                apertureType = value;
            }
        }

        public bool DrawSpotPair { get; set; }

        public bool DrawPairingArea { get; set; }

        public bool DrawProfile { get; set; }

        public int NoiseLevel { get; set; }

        public IWavefront Wavefront { get; set; }

        public CalibrationComponents ViewCalibration { get; set; }
        #endregion // Properties

        #region Methods
        private void changeDrawImageSize(int width, int height)
        {
            drawBuffers = new Bitmap[]
            {
                new Bitmap(width, height),
                new Bitmap(width, height)
            };
        }

        private void drawSpotMap(Bitmap bitmap)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)(() => drawSpotMap(bitmap)));
            }
            else
            {
                Image oldImage = spotMapPicture.Image;

                spotMapPicture.Image = bitmap;
                spotMapPicture.Invalidate();

                oldImage?.Dispose();
            }
        }

        private void drawSpotPairs(Graphics graphics, List<CorrectionPair<PointD>> spotPairs, int spotSize)
        {
            using (var blueBrush = new SolidBrush(Color.Blue))
            using (var redBrush = new SolidBrush(Color.Red))
            {
                foreach (CorrectionPair<PointD> spotPair in spotPairs)
                {
                    float spotRadius = spotSize / 2;
                    var referencePoint = new Rectangle((int)(spotPair.Reference.X - spotRadius), (int)(spotPair.Reference.Y - spotRadius), spotSize, spotSize);
                    var targetPoint = new Rectangle((int)(spotPair.Target.X - spotRadius), (int)(spotPair.Target.Y - spotRadius), spotSize, spotSize);

                    graphics.FillEllipse(blueBrush, referencePoint);
                    graphics.FillEllipse(redBrush, targetPoint);
                }
            }
        }

        private PointF[] generateHorizontalProfile(ImageComponent image)
        {
            var imageCenter = new PointD(image.Width / 2, image.Height / 2);
            var profile = new PointF[image.Width];
            double graphRange = image.Height / 5.0;
            int peak = (1 << image.BitDepth) - 1;

            Func<byte[], int, uint> bitConverter = Bit.SelectConverter(image.BitDepth);
            for (int index = 0; index < profile.Length; index++)
            {
                int pixelValue = (int)bitConverter(image.Pixels, (image.Width * (int)imageCenter.Y) + index);
                double yCoordinate = (pixelValue * (graphRange / peak));
                profile[index] = new PointF(index, (float)(image.Height - yCoordinate));
            }

            return profile;
        }

        private PointF[] generateVerticalProfile(ImageComponent image)
        {
            var imageCenter = new PointD(image.Width / 2, image.Height / 2);
            var profile = new PointF[image.Height];
            double graphRange = image.Width / 5.0;
            int peak = (1 << image.BitDepth) - 1;

            Func<byte[], int, uint> bitConverter = Bit.SelectConverter(image.BitDepth);
            for (int index = 0; index < profile.Length; index++)
            {
                int pixelValue = (int)bitConverter(image.Pixels, (image.Width * index) + (int)imageCenter.X);
                double xCoordinate = pixelValue * (graphRange / peak);
                profile[index] = new PointF((float)xCoordinate, index);
            }

            return profile;
        }

        private void initializeParameter()
        {
            apertureCenter = new PointD();
            apertureSize = new Size<double>();
            apertureType = ApertureShape.Circle;

            changeDrawImageSize(spotMapPicture.Width, spotMapPicture.Height);

            drawEvent = new SemaphoreSlim(0, 1);
            drawEvent.Release();
        }

        public void Draw(WavefrontContainer frame)
        {
            if (!drawEvent.Wait(0))
            {
                return;
            }

            if ((drawBuffers[0].Height != frame.Image.Height) ||
                (drawBuffers[0].Width != frame.Image.Width))
            {
                changeDrawImageSize(frame.Image.Width, frame.Image.Height);
            }

            int targetIndex = (bufferCounter++ % drawBuffers.Length);
            ImageProcessing.RawToBitmap(frame.Image.RawData, ref drawBuffers[targetIndex]);

            using (var graphics = Graphics.FromImage(drawBuffers[targetIndex]))
            using (var centerLinePen = new Pen(Color.Cyan, 2))
            using (var aperturePen = new Pen(Color.DodgerBlue, 2))
            {
                // センターライン表示
                graphics.DrawLine(centerLinePen, 0, (float)(frame.Image.RawData.Height / 2.0), frame.Image.RawData.Width, (float)(frame.Image.RawData.Height / 2.0));
                graphics.DrawLine(centerLinePen, (float)(frame.Image.RawData.Width / 2.0), 0, (float)(frame.Image.RawData.Width / 2.0), frame.Image.RawData.Height);

                // アパーチャー表示
                if (apertureType == ApertureShape.Circle)
                {
                    graphics.DrawEllipse(aperturePen, new Rectangle((int)(apertureCenter.X - (apertureSize.Width / 2)),
                                                                    (int)(apertureCenter.Y - (apertureSize.Height / 2)),
                                                                    (int)apertureSize.Width,
                                                                    (int)apertureSize.Height));
                }
                else
                {
                    graphics.DrawRectangle(aperturePen, new Rectangle((int)(apertureCenter.X - (apertureSize.Width / 2)),
                                                                      (int)(apertureCenter.Y - (apertureSize.Height / 2)),
                                                                      (int)apertureSize.Width,
                                                                      (int)apertureSize.Height));
                }

                if (ApertureCenterTracking)
                {
                    using (var trackingPen = new Pen(Color.Gray, 2))
                    {
                        trackingPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                        graphics.DrawLine(trackingPen, 0, (float)(apertureCenter.Y), frame.Image.RawData.Width, (float)(apertureCenter.Y));
                        graphics.DrawLine(trackingPen, (float)(apertureCenter.X), 0, (float)(apertureCenter.X), frame.Image.RawData.Height);
                    }
                }

                // ペアリングエリア表示
                if (DrawPairingArea)
                {
                    using (var areaPen = new Pen(Color.LimeGreen, 2))
                    {
                        int areaBaseSize = (int)(Wavefront.MicroLensArrayPitch / (Math.Sqrt(2) * Wavefront.PixelPitch * Wavefront.Binning));
                        var imageSize = new Size<int>(frame.Image.Width, frame.Image.Height);
                        double imageDiagonal = imageSize.Diagonal / 2;
                        foreach (CorrectionPair<PointD> spotPair in frame.SpotPairs)
                        {
                            double distance = apertureCenter.DistanceTo(spotPair.Reference.X, spotPair.Reference.Y);
                            float areaSize = areaBaseSize + (float)((Wavefront.PairingAreaRate - 1) * areaBaseSize * distance / imageDiagonal);
                            var area = new Rectangle((int)(spotPair.Reference.X - areaSize / 2),
                                                     (int)(spotPair.Reference.Y - areaSize / 2),
                                                     (int)areaSize,
                                                     (int)areaSize);
                            graphics.DrawRectangle(areaPen, area);
                        }
                    }
                }

                // ペアリングスポット表示
                if (DrawSpotPair)
                {
                    using (var referenceBrush = new SolidBrush(Color.Blue))
                    using (var targetBrush = new SolidBrush(Color.Red))
                    {
                        foreach (CorrectionPair<PointD> spotPair in frame.SpotPairs)
                        {
                            int spotSize = 10;
                            float spotRadius = spotSize / 2;
                            var referencePoint = new Rectangle((int)(spotPair.Reference.X - spotRadius), (int)(spotPair.Reference.Y - spotRadius), spotSize, spotSize);
                            graphics.FillEllipse(referenceBrush, referencePoint);

                            spotSize = 8;
                            spotRadius = spotSize / 2;
                            var targetPoint = new Rectangle((int)(spotPair.Target.X - spotRadius), (int)(spotPair.Target.Y - spotRadius), spotSize, spotSize);
                            graphics.FillEllipse(targetBrush, targetPoint);
                        }
                    }
                }

                // プロファイルの表示
                if (DrawProfile)
                {
                    using (var profilePen = new Pen(Color.Yellow, 2))
                    {
                        graphics.DrawLines(profilePen, generateHorizontalProfile(frame.Image.RawData));
                        graphics.DrawLines(profilePen, generateVerticalProfile(frame.Image.RawData));
                    }

                    using (var noisePen = new Pen(Color.HotPink, 2))
                    {
                        int y = ((frame.Image.RawData.Height / 5) * NoiseLevel / ((1 << frame.Image.BitDepth) - 1));
                        graphics.DrawLine(noisePen, 0, frame.Image.RawData.Height - y, frame.Image.RawData.Width, frame.Image.RawData.Height - y);

                        int x = ((frame.Image.RawData.Width / 5) * NoiseLevel / ((1 << frame.Image.BitDepth) - 1));
                        graphics.DrawLine(noisePen, x, 0, x, frame.Image.RawData.Height);
                    }
                }
            };

            //drawBuffers[targetIndex].RotateFlip(RotateFlipType.RotateNoneFlipY);

            drawSpotMap(drawBuffers[targetIndex].Clone() as Bitmap);

            drawEvent.Release();
        }
        #endregion // Methods
    }
}
