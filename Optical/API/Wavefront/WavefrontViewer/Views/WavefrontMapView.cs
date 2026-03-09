using Optical.API.Wavefront.Sample;
using Optical.Platform.Types;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WavefrontTester.Views
{
    public partial class WavefrontMapView : UserControl
    {
        #region Fields
        private int legendreBuffreCounter;
        private int zernikeBufferCounter;

        private bool isDrawingLegendre;
        private bool isDrawingZernike;

        private Bitmap[] legendreDrawBuffers;
        private Bitmap[] zernikeDrawBuffers;
        #endregion // Fields

        #region Constructors
        public WavefrontMapView()
        {
            InitializeComponent();

            initializeParameters();
            initializeControls();
        }
        #endregion // Constructors

        #region Methods
        private void initializeControls()
        {
            pictureBoxColorBar.Image = ViewsCommon.GenerateColorBarImage(pictureBoxColorBar.Size);
        }

        private void initializeParameters()
        {
            zernikeDrawBuffers = new Bitmap[2]
            {
                new Bitmap(zernikeView.Width, zernikeView.Height),
                new Bitmap(zernikeView.Width, zernikeView.Height)
            };

            legendreDrawBuffers = new Bitmap[2]
            {
                new Bitmap(legendreView.Width, legendreView.Height),
                new Bitmap(legendreView.Width, legendreView.Height)
            };
        }

        private Bitmap drawAberrationMap(ImageComponent<double> aberrationMap, int resolution, double upper, double lower)
        {
            if (aberrationMap == null)
            {
                return null;
            }

            var drawImage = new Bitmap(resolution, resolution);
            double center = (upper + lower) / 2;
            double upperHalf = (upper + center) / 2;
            double lowerHalf = (lower + center) / 2;
            BitmapData bitmapData = drawImage.LockBits(new Rectangle(0, 0, drawImage.Width, drawImage.Height), ImageLockMode.ReadWrite, drawImage.PixelFormat);
            for (int y = 0; y < aberrationMap.Height; y++)
            {
                for (int x = 0; x < aberrationMap.Width; x++)
                {
                    int index = (y * aberrationMap.Width) + x;
                    double aberration = aberrationMap.Pixels[(y * resolution) + x];
                    Color color;
                    if (double.IsNaN(aberration))
                    {
                        color = Color.Black;
                    }
                    else
                    {
                        double pixel = (aberration - center);

                        int red = (int)(255 * pixel / (upperHalf - center));
                        red = (red > 255) ? 255 :
                              (red < 0) ? 0 :
                              red;

                        int green = (int)(255 * 2 * (1 - Math.Abs(pixel / (upper - center))));
                        green = (green > 255) ? 255 :
                                (green < 0) ? 0 :
                                green;

                        int blue = (int)(255 * pixel / (center - lowerHalf));
                        blue = (blue > 0) ? 0 :
                               (blue < -255) ? 255 :
                               -blue;

                        color = Color.FromArgb(red, green, blue);
                    }

                    Marshal.WriteInt32(bitmapData.Scan0, index * sizeof(int), color.ToArgb());
                }
            };

            drawImage.UnlockBits(bitmapData);

            return drawImage;
        }

        private void drawGraphics(Bitmap canvas, Bitmap aberrationMapImage)
        {
            using (var graphics = Graphics.FromImage(canvas))
            {
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;

                var rect_src = new RectangleF
                {
                    X = 0f,
                    Y = 0f,
                    Width = aberrationMapImage.Width,
                    Height = aberrationMapImage.Height
                };

                var rect_dst = new RectangleF
                {
                    X = 0.0f,
                    Y = 0.0f,
                    Width = canvas.Width,
                    Height = canvas.Height
                };

                graphics.DrawImage(aberrationMapImage, rect_dst, rect_src, GraphicsUnit.Pixel);
                graphics.DrawLine(Pens.CadetBlue, canvas.Width / 2, 0, canvas.Width / 2, canvas.Height);
                graphics.DrawLine(Pens.CadetBlue, 0, canvas.Height / 2, canvas.Width, canvas.Height / 2);
            }
        }

        private void drawLegendreMap(Bitmap bitmap)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)(() => drawLegendreMap(bitmap)));
            }
            else
            {
                legendreView.Image = bitmap;
                legendreView.Invalidate();
            }
        }

        private void drawZernikeMap(Bitmap bitmap)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)(() => drawZernikeMap(bitmap)));
            }
            else
            {
                zernikeView.Image = bitmap;
                zernikeView.Invalidate();
            }
        }

        public void DrawLegendre((double coefficient, bool Enabled)[] legendres)
        {
            if (isDrawingLegendre)
            {
                return;
            }

            isDrawingLegendre = true;

            int targetIndex = (legendreBuffreCounter++ % 2);

            //2次元収差マップ取得
            ImageComponent<double> aberrationMap = WavefrontSensor.AberrationMapLegendre(legendres, legendreView.Width);
            legendreDrawBuffers[targetIndex] = drawAberrationMap(aberrationMap, legendreView.Width, (double)aberrationUpper.Value, (double)aberrationLower.Value);
            legendreDrawBuffers[targetIndex].RotateFlip(RotateFlipType.RotateNoneFlipY);

            // 中心線描画
            using (var graphics = Graphics.FromImage(legendreDrawBuffers[targetIndex]))
            {
                graphics.DrawLine(Pens.CadetBlue, legendreView.Width / 2, 0, legendreView.Width / 2, legendreView.Height);
                graphics.DrawLine(Pens.CadetBlue, 0, legendreView.Height / 2, legendreView.Width, legendreView.Height / 2);
            }

            drawLegendreMap(legendreDrawBuffers[targetIndex]);

            isDrawingLegendre = false;
        }

        public void DrawZernike((double coefficient, bool Enabled)[] zernikes)
        {
            if (isDrawingZernike)
            {
                return;
            }

            isDrawingZernike = true;

            int targetIndex = (zernikeBufferCounter++ % 2);

            //2次元収差マップ取得
            ImageComponent<double> aberrationMap = WavefrontSensor.AberrationMapZernike(zernikes, zernikeView.Width);
            zernikeDrawBuffers[targetIndex] = drawAberrationMap(aberrationMap, zernikeView.Width, (double)aberrationUpper.Value, (double)aberrationLower.Value);
            zernikeDrawBuffers[targetIndex].RotateFlip(RotateFlipType.RotateNoneFlipY);

            // 中心線描画
            using (var graphics = Graphics.FromImage(zernikeDrawBuffers[targetIndex]))
            {
                graphics.DrawLine(Pens.CadetBlue, zernikeView.Width / 2, 0, zernikeView.Width / 2, zernikeView.Height);
                graphics.DrawLine(Pens.CadetBlue, 0, zernikeView.Height / 2, zernikeView.Width, zernikeView.Height / 2);
            }

            drawZernikeMap(zernikeDrawBuffers[targetIndex]);

            isDrawingZernike = false;
        }
        #endregion // Methods
    }
}
