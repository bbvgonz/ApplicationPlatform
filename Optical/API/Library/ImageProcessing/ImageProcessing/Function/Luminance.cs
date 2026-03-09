using OpenCvSharp;
using Optical.Platform.Types;
using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Threading.Tasks;

namespace Optical.API.Library
{
    /// <summary>
    /// 画像処理ライブラリクラス
    /// </summary>
    public partial class ImageProcessing
    {
        /// <summary>
        /// 画像の最大輝度値を出力します。
        /// </summary>
        /// <param name="image">2次元画像データ</param>
        /// <param name="roi">対象領域</param>
        /// <param name="noiseLevel">ノイズレベル</param>
        /// <returns>輝度重心座標[pixel]</returns>
        /// <remarks>ノイズレベル未満のピクセルデータは、輝度重心計算の対象外になります。</remarks>
        public static int LuminancePeakValue(ImageComponent image, Rectangle roi, int noiseLevel)
        {
            // LuminanceRangeに統合する。

            int peak = new int();
            object lockLuminunce = new object();
            OrderablePartitioner<Tuple<int, int>> rangePartitioner = Partitioner.Create(0, roi.Height * roi.Width);
            int offsetIndex = roi.Top * image.Width + roi.Left;
            Func<byte[], int, uint> bitConverter = Bit.SelectConverter(image.BitDepth);
            Parallel.ForEach(rangePartitioner,
                             () => new int(),
                             (range, loopState, subLuminunce) =>
                             {
                                 for (int index = range.Item1; index < range.Item2; index++)
                                 {
                                     int targetIndex = offsetIndex + ((index / roi.Width) * image.Width) + (index % roi.Width);

                                     int pixelValue = (int)bitConverter(image.Pixels, targetIndex);
                                     if (pixelValue < noiseLevel)
                                     {
                                         continue;
                                     }

                                     subLuminunce = (pixelValue > subLuminunce) ? pixelValue : subLuminunce;
                                 }

                                 return subLuminunce;
                             },
                             (result) =>
                             {
                                 lock (lockLuminunce)
                                 {
                                     peak = (peak > result) ? peak : result;
                                 }
                             });

            return peak;
        }

        /// <summary>
        /// 画像の最大・最小輝度値を出力します。
        /// </summary>
        /// <param name="image">2次元画像データ</param>
        /// <returns>最大輝度値[counts]</returns>
        public static Limit<double> LuminanceRange(ImageComponent image)
        {
            using (Mat cvImage = createImage(image.Pixels, image.Height, image.Width, image.BitDepth))
            {
                cvImage.MinMaxLoc(out double min, out double max);
                return new Limit<double>(max, min);
            }
        }

        /// <summary>
        /// 画像の最大・最小輝度値を出力します。
        /// </summary>
        /// <param name="image">2次元画像データ</param>
        /// <returns>最大輝度値[counts]</returns>
        public static Limit<double> LuminanceRange(ImageComponent<double> image)
        {
            using (var cvImage = Mat.FromPixelData(image.Height, image.Width, MatType.CV_64FC1, image.Pixels))
            {
                cvImage.MinMaxLoc(out double min, out double max);
                return new Limit<double>(max, min);
            }
        }
    }
}
