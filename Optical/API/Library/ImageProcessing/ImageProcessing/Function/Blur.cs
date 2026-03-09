using OpenCvSharp;
using Optical.Platform.Types;

namespace Optical.API.Library
{
    /// <summary>
    /// 画像処理ライブラリクラス
    /// </summary>
    public partial class ImageProcessing
    {
        /// <summary>
        /// ガウシアンフィルタを用いて画像の平滑化を行います。
        /// </summary>
        /// <param name="image">入力画像</param>
        /// <param name="kernelSize">ガウシアンカーネルサイズ。0 または 正の奇数。</param>
        /// <param name="sigmaX">ガウシアンカーネルのX方向の標準偏差。0 の場合、<paramref name="sigmaY"/>と同値とみなす。</param>
        /// <param name="sigmaY">ガウシアンカーネルのY方向の標準偏差。0 の場合、<paramref name="sigmaX"/>と同値とみなす。</param>
        /// <returns>出力画像</returns>
        /// <remarks><paramref name="sigmaX"/>,<paramref name="sigmaY"/>両方の値が 0 の場合、それぞれ kernelSize.width と kernelSize.height から求められます。</remarks>
        public static ImageComponent GaussianBlur(ImageComponent image, System.Drawing.Size kernelSize, double sigmaX, double sigmaY = 0)
        {
            Mat source = createImage(image);
            var destination = new Mat();
            Cv2.GaussianBlur(source, destination, new OpenCvSharp.Size(kernelSize.Width, kernelSize.Height), sigmaX, sigmaY);

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
