using OpenCvSharp;
using Optical.Platform.Types;

namespace Optical.API.Library
{
    public partial class ImageProcessing
    {
        /// <summary>
        /// 画像の各Pixel値を指定された値分オフセットした画像データを出力する。
        /// </summary>
        /// <param name="image">元画像</param>
        /// <param name="offset">オフセット値</param>
        /// <returns>オフセット後画像</returns>
        public static ImageComponent<double> ImageOffset(ImageComponent image, double offset)
        {
            Mat original = createImage(image.Pixels, image.Height, image.Width, image.BitDepth);
            original.ConvertTo(original, MatType.CV_64FC1);

            Mat originalOffset;
            if (offset == 0)
            {
                originalOffset = original;
            }
            else
            {
                originalOffset = new Mat();
                Cv2.Add(original, offset, originalOffset);
            }

            var output = new ImageComponent<double>()
            {
                BitDepth = image.BitDepth,
                Height = originalOffset.Height,
                Pixels = new double[originalOffset.Total()],
                Width = originalOffset.Width
            };
            System.Runtime.InteropServices.Marshal.Copy(originalOffset.Data, output.Pixels, 0, output.Pixels.Length);

            original.Dispose();
            if (!originalOffset.IsDisposed)
            {
                originalOffset.Dispose();
            }

            return output;
        }
    }
}
