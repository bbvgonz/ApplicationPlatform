using OpenCvSharp;
using Optical.Platform.Types;
using System;

namespace Optical.API.Library
{
    public partial class ImageProcessing
    {
        /// <summary>
        /// 画像のサイズを変更します。
        /// </summary>
        /// <param name="image">入力画像</param>
        /// <param name="outputSize">出力画像サイズ</param>
        /// <param name="interpolation">補間方法</param>
        /// <returns>出力画像</returns>
        public static ImageComponent Resize(ImageComponent image, Size<int> outputSize, InterpolationMethod interpolation)
        {
            if (!Enum.IsDefined(typeof(InterpolationMethod), interpolation))
            {
                throw new ArgumentOutOfRangeException(nameof(interpolation), interpolation, "The specified interpolation is not supported.");
            }

            Mat source = createImage(image);
            var destination = new Mat();
            Cv2.Resize(source, destination, new OpenCvSharp.Size(outputSize.Width, outputSize.Height), interpolation: (InterpolationFlags)interpolation);

            var output = new ImageComponent()
            {
                BitDepth = image.BitDepth,
                Height = destination.Height,
                Pixels = new byte[destination.Total() * destination.ElemSize()],
                Width = destination.Width
            };
            System.Runtime.InteropServices.Marshal.Copy(destination.Data, output.Pixels, 0, output.Pixels.Length);

            source.Dispose();
            destination.Dispose();

            return output;
        }
    }
}
