using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Optical.Platform.Types;

namespace Optical.API.Library
{
    public partial class ImageProcessing
    {

        [SupportedOSPlatform("windows")]
        private static void toBitmap(ImageComponent image, ref Bitmap bitmap)
        {
            if (bitmap == null)
            {
                return;
            }

            if (image.BitDepth > 16)
            {
                throw new ArgumentOutOfRangeException(nameof(image.BitDepth), image.BitDepth, "Unsupported bit depth.");
            }

            int width = image.Width;
            int height = image.Height;

            var rectangle = new Rectangle(0, 0, width, height);
            BitmapData bitmapData = bitmap.LockBits(rectangle, ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);
            int[] colorMap = new int[image.Width * image.Height];

            try
            {
                Func<byte[], int, uint> bitConverter = Bit.SelectConverter(image.BitDepth);
                int bitShift = image.BitDepth - 8;
                Parallel.For(0, height, row =>
                {
                    for (int column = 0; column < width; column++)
                    {
                        int pixelIndex = (row * width + column);
                        int gray = (int)(bitConverter(image.Pixels, pixelIndex) >> bitShift);

                        colorMap[pixelIndex] = (int)(0xff000000 + (gray << 16) + (gray << 8) + gray);
                    }
                });

                Marshal.Copy(colorMap, 0, bitmapData.Scan0, colorMap.Length);
            }
            catch
            {
                throw;
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }
        }


        [SupportedOSPlatform("windows")]
        /// <summary>
        /// RAW画像をBitmap形式に変換します。
        /// </summary>
        /// <param name="rawImage">RAW画像</param>
        /// <param name="bitmap">出力先Bitmap画像</param>
        public static void RawToBitmap(ImageComponent rawImage, ref Bitmap bitmap)
        {
            toBitmap(rawImage, ref bitmap);
        }


        [SupportedOSPlatform("windows")]
        /// <summary>
        /// RAW画像をBitmap形式に変換します。
        /// </summary>
        /// <param name="rawImage">RAW画像データ</param>
        /// <returns>Bitmap画像</returns>
        public static Bitmap RawToBitmap(ImageComponent rawImage)
        {
            var bitmap = new Bitmap(rawImage.Width, rawImage.Height);
            toBitmap(rawImage, ref bitmap);
            return bitmap;
        }
    }
}
