using OpenCvSharp;
using Optical.Platform.Types;
using System;

namespace Optical.API.Library
{
    /// <summary>
    /// 画像処理ライブラリクラス
    /// </summary>
    public partial class ImageProcessing
    {
        private static Mat binarize(Array rawImage, int rows, int columns, int bitDepth, int threshold)
        {
            using (Mat image = createImage(rawImage, rows, columns, bitDepth))
            {
                return image.Threshold(threshold, (1 << bitDepth) - 1, ThresholdTypes.Binary);
            }
        }

        private static Mat binarize(ImageComponent rawImage, int threshold)
        {
            using (Mat image = createImage(rawImage))
            {
                return image.Threshold(threshold, (1 << rawImage.BitDepth) - 1, ThresholdTypes.Binary);
            }
        }

        /// <summary>
        /// 2次元画像データを2値化します。
        /// </summary>
        /// <param name="rawImage">2次元画像データ。2値化後の値で上書きされます。</param>
        /// <param name="threshold">閾値</param>
        public static void Binarize(ref ImageComponent rawImage, int threshold)
        {
            using (Mat binary = binarize(rawImage.Pixels, rawImage.Height, rawImage.Width, rawImage.BitDepth, threshold))
            {
                System.Runtime.InteropServices.Marshal.Copy(binary.Data, rawImage.Pixels, 0, rawImage.Pixels.Length);
            }
        }

        /// <summary>
        /// 2値化した2次元画像データを新たに生成します。
        /// </summary>
        /// <param name="rawImage">2次元画像データ</param>
        /// <param name="threshold">閾値</param>
        /// <returns>2値化画像（ビット深度:8）</returns>
        public static ImageComponent Binarize(ImageComponent rawImage, int threshold)
        {
            using (Mat binary = binarize(rawImage.Pixels, rawImage.Height, rawImage.Width, rawImage.BitDepth, threshold))
            {
                if (binary.Type() != MatType.CV_8UC1)
                {
                    binary.ConvertTo(binary, MatType.CV_8UC1);
                }

                var output = new ImageComponent()
                {
                    BitDepth = 8,
                    Height = rawImage.Height,
                    Pixels = new byte[binary.Total()],
                    Width = rawImage.Width
                };
                System.Runtime.InteropServices.Marshal.Copy(binary.Data, output.Pixels, 0, output.Pixels.Length);

                return output;
            }
        }

        /// <summary>
        /// 2次元画像データを2値化します。
        /// </summary>
        /// <param name="rawImage">2次元画像データ。2値化後の値で上書きされます。</param>
        /// <param name="threshold">閾値</param>
        /// <returns>2値化画像（ビット深度:8）</returns>
        public static ImageComponent Binarize(ImageComponent<double> rawImage, int threshold)
        {
            using (var cvImage = Mat.FromPixelData(rawImage.Height, rawImage.Width, MatType.CV_64FC1, rawImage.Pixels))
            using (var binary = new Mat())
            {
                cvImage.Threshold(threshold, (1 << rawImage.BitDepth) - 1, ThresholdTypes.Binary).ConvertTo(binary, MatType.CV_8UC1);

                var output = new ImageComponent()
                {
                    BitDepth = 8,
                    Height = rawImage.Height,
                    Pixels = new byte[binary.Total()],
                    Width = rawImage.Width
                };
                System.Runtime.InteropServices.Marshal.Copy(binary.Data, output.Pixels, 0, output.Pixels.Length);

                return output;
            }
        }

        /// <summary>
        /// 閾値以下の値を0にした画像を出力します。
        /// </summary>
        /// <param name="image">2次元画像データ</param>
        /// <param name="threshold">輝度閾値</param>
        /// <param name="view">2値化後画像表示</param>
        /// <returns>2値化後画像データ</returns>
        public static ImageComponent ToZero(ImageComponent image, double threshold, bool view = false)
        {
            using (Mat cvImage = createImage(image.Pixels, image.Height, image.Width, image.BitDepth))
            using (Mat toZeroImage = cvImage.Threshold(threshold, (1 << image.BitDepth) - 1, ThresholdTypes.Tozero))
            {
                if (view)
                {
                    Cv2.ImShow("To zero image.", toZeroImage);
                }

                var result = new ImageComponent(image)
                {
                    Pixels = new byte[toZeroImage.Total() * toZeroImage.ElemSize()]
                };
                System.Runtime.InteropServices.Marshal.Copy(toZeroImage.Data, result.Pixels, 0, result.Pixels.Length);

                return result;
            }
        }
    }
}
