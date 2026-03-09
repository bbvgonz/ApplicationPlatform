using OpenCvSharp;
using Optical.Platform.Types;
using System;
using System.Drawing;

namespace Optical.API.Library
{
    /// <summary>
    /// 画像処理ライブラリクラス
    /// </summary>
    public partial class ImageProcessing
    {
        /// <summary>
        /// 画像内のエッジを検出します。(Canny法)
        /// </summary>
        /// <param name="image">画像データ</param>
        /// <param name="roi">
        /// <para>エッジ検出対象領域</para>
        /// <para><see cref="Rectangle.Empty"/>の場合は、画像全体が対象になります。</para> 
        /// </param>
        /// <param name="minThreshold">最小しきい値</param>
        /// <param name="maxThreshold">最大しきい値</param>
        /// <returns>エッジ検出後画像データ</returns>
        public static ImageContainer EdgeDetectionCanny(ImageContainer image, Rectangle roi, int minThreshold, int maxThreshold)
        {
            // 画像の対象領域を決定する。
            var targetImage = new ImageContainer()
            {
                BitDepth = image.BitDepth
            };

            if (roi.IsEmpty)
            {
                targetImage.Height = image.Height;
                targetImage.Width = image.Width;
                targetImage.RawData = image.RawData;
            }
            else
            {
                targetImage.Height = roi.Height;
                targetImage.Width = roi.Width;

                int bytePerPixel = Bit.ToByte(image.BitDepth);
                targetImage.RawData = new ImageComponent()
                {
                    BitDepth = image.BitDepth,
                    Height = roi.Height,
                    Pixels = new byte[roi.Height * roi.Width * bytePerPixel],
                    Width = roi.Width,
                };
                for (int rows = 0; rows < roi.Height; rows++)
                {
                    Buffer.BlockCopy(image.RawData.Pixels,
                                     ((roi.Top + rows) * image.Width + roi.Left) * bytePerPixel,
                                     targetImage.RawData.Pixels,
                                     rows * roi.Width * bytePerPixel,
                                     roi.Width * bytePerPixel);
                }
            }

            // Canny法でエッジ検出を行う。
            using Mat cvImage = createImage(targetImage.RawData.Pixels, targetImage.Height, targetImage.Width, targetImage.BitDepth);
            Mat edgeImage = cvImage.Canny(minThreshold, maxThreshold);

            if (edgeImage.Type() != MatType.CV_8UC1)
            {
                edgeImage.ConvertTo(edgeImage, MatType.CV_8UC1);
            }

            byte[] outputImage = new byte[edgeImage.Total()];
            System.Runtime.InteropServices.Marshal.Copy(edgeImage.Data, outputImage, 0, outputImage.Length);

            // 結果を出力する。
            var result = new ImageContainer()
            {
                Height = targetImage.Height,
                Width = targetImage.Width,
                BitDepth = 8,
                RawData = new ImageComponent()
                {
                    BitDepth = 8,
                    Height = targetImage.Height,
                    Pixels = outputImage,
                    Width = targetImage.Width
                }
            };

            return result;
        }
    }
}
