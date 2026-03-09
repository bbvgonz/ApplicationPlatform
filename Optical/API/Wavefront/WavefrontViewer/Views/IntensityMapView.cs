using Optical.API.Wavefront;
using Optical.Platform.Types;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WavefrontTester.Views
{
    public partial class IntensityMapView : UserControl
    {
        #region Fields
        private int bitDepth;
        private Bitmap colorBarImage;
        private Color[] colorTable;

        private SemaphoreSlim drawEvent;

        private IWavefront wavefront;
        #endregion // Fields

        #region Constructors
        public IntensityMapView()
        {
            InitializeComponent();
            initializeControls();
        }
        #endregion // Constructors

        #region Properties
        protected int BitDepth
        {
            get => bitDepth;
            set
            {
                if (value == bitDepth)
                {
                    return;
                }

                bitDepth = value;
                bitDepthChanged();
            }
        }
        #endregion // Properties

        #region Methods
        private void bitDepthChanged()
        {
            colorTable = ViewsCommon.GetColorTable(colorBarImage, (1 << bitDepth) - 1, 0);
        }

        private void initializeControls()
        {
            colorBarImage = ViewsCommon.GenerateColorBarImage(pictureBoxColorBar.Size);
            pictureBoxColorBar.Image = colorBarImage;
            pictureBoxColorBar.Invalidate();

            bitDepth = 8;
            bitDepthChanged();

            drawEvent = new SemaphoreSlim(0, 1);
            drawEvent.Release();
        }

        private void drawIntensityMap(Bitmap bitmap)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)(() => drawIntensityMap(bitmap)));
            }
            else
            {
                Image oldImage = intensityView.Image;

                intensityView.Image = bitmap;
                intensityView.Invalidate();

                oldImage?.Dispose();
            }
        }

        public void Attach(IWavefront wavefront)
        {
            this.wavefront = wavefront;
        }

        public void Detach()
        {
            wavefront = null;
        }

        public void Draw(ImageComponent image)
        {
            if (!drawEvent.Wait(0))
            {
                return;
            }

            if (image == null)
            {
                drawEvent.Release();
                return;
            }

            BitDepth = image.BitDepth;

            var mapSize = new Size<int>(intensityView.Width, intensityView.Height);
            ImageComponent intensityMap = wavefront?.IntensityMap(image, mapSize);
            if (intensityMap == null)
            {
                drawEvent.Release();
                return;
            }

            Func<byte[], int, uint> bitConverter = Bit.SelectConverter(image.BitDepth);
            var drawImage = new Bitmap(intensityMap.Width, intensityMap.Height);
            BitmapData bitmapData = drawImage.LockBits(new Rectangle(0, 0, drawImage.Width, drawImage.Height), ImageLockMode.ReadWrite, drawImage.PixelFormat);
            Parallel.For(0, intensityMap.Height, y =>
            {
                for (int x = 0; x < intensityMap.Width; x++)
                {
                    int index = (y * intensityMap.Width) + x;
                    Color color = colorTable[bitConverter(intensityMap.Pixels, index)];
                    Marshal.WriteInt32(bitmapData.Scan0, index * sizeof(int), color.ToArgb());
                }
            });

            drawImage.UnlockBits(bitmapData);

            using (var graphics = Graphics.FromImage(drawImage))
            using (var limeGreenPen = new Pen(Color.LimeGreen, 2))
            {
                // センターライン表示
                graphics.DrawLine(limeGreenPen, 0, (float)(intensityMap.Height / 2.0), intensityMap.Width, (float)(intensityMap.Height / 2.0));
                graphics.DrawLine(limeGreenPen, (float)(intensityMap.Width / 2.0), 0, (float)(intensityMap.Width / 2.0), intensityMap.Height);
            };

            //drawImage.RotateFlip(RotateFlipType.RotateNoneFlipY);
            drawIntensityMap(drawImage);

            drawEvent.Release();
        }
        #endregion // Methods
    }
}
