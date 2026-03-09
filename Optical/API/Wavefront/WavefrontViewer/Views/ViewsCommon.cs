using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace WavefrontTester.Views
{
    public static class ViewsCommon
    {
        #region Public Methods
        public static Bitmap GenerateColorBarImage(Size imageSize)
        {
            var colorBarBitmap = new Bitmap(imageSize.Width, imageSize.Height);

            using (var colorBarGraphics = Graphics.FromImage(colorBarBitmap))
            using (var linearGradientBrush = new LinearGradientBrush(colorBarGraphics.VisibleClipBounds, Color.Red, Color.Blue, LinearGradientMode.Vertical))
            {
                var colorBlend = new ColorBlend()
                {
                    Colors = new Color[] { Color.Red, Color.Yellow, Color.Lime, Color.Cyan, Color.Blue },
                    Positions = new float[] { 0.0f, 0.25f, 0.5f, 0.75f, 1.0f },
                };
                linearGradientBrush.InterpolationColors = colorBlend;
                linearGradientBrush.GammaCorrection = true;

                colorBarGraphics.FillRectangle(linearGradientBrush, colorBarGraphics.VisibleClipBounds);
            }

            return colorBarBitmap;
        }

        public static Color[] GetColorTable(Bitmap colorBarImage, int max, int min)
        {
            if (colorBarImage == null)
            {
                throw new ArgumentNullException();
            }

            int colorTableSize = (max - min) + 1;
            var colorTable = new Color[colorTableSize];

            double colorConversionFactor = (colorBarImage.Height - 1) / (double)(colorTableSize - 1);

            BitmapData bitmapData = colorBarImage.LockBits(new Rectangle(0, 0, colorBarImage.Width, colorBarImage.Height), ImageLockMode.ReadOnly, colorBarImage.PixelFormat);

            for (int tableIndex = 0; tableIndex < colorTableSize; tableIndex++)
            {
                int bitmapDataIndex = (int)(tableIndex * colorConversionFactor) * colorBarImage.Width;

                int pixelData = Marshal.ReadInt32(bitmapData.Scan0, bitmapDataIndex * sizeof(int));
                int red = pixelData & 0xFF;
                int green = (pixelData >> 8) & 0xFF;
                int blue = (pixelData >> 16) & 0xFF;

                colorTable[tableIndex] = Color.FromArgb(0xFF, red, green, blue);
            }

            colorBarImage.UnlockBits(bitmapData);

            return colorTable;
        }
        #endregion
    }
}
