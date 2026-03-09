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
        /// ナイフエッジ法により指定された割合に対する２点間の距離を出力する。
        /// </summary>
        /// <param name="image">2次元画像データ</param>
        /// <param name="lowRate">輝度の総量に対する下限の割合（≦ 1.0）</param>
        /// <param name="upperRate">輝度の総量に対する上限の割合（≦ 1.0）</param>
        /// <param name="multiplier">乗数（デフォルト値：1.561）</param>
        /// <returns>ナイフエッジ幅[pixels]</returns>
        /// <remarks>乗数：（参考文献）IEEE Journal of Quantum Electronics, Vol.27, No.4, April 1991 Choice of Clip Levels for Beam-Width Measurements Using Knife-Edge Techniques by Siegman, Sansnett and Johnston.</remarks>
        public static Size<int> KnifeEdge(ImageComponent<double> image, double lowRate, double upperRate, double multiplier = 1.561)
        {
            if ((lowRate > 1.0) || (upperRate > 1.0))
            {
                return new Size<int>();
            }

            if (lowRate > upperRate)
            {
                return new Size<int>();
            }

            int borderDetection(Mat luminunceProfile, double low, double upper)
            {
                Scalar total = luminunceProfile.Sum();
                double lowThreshold = total.Val0 * low;
                double upperThreshold = total.Val0 * upper;

                double count = 0;
                int lowIndex = 0;
                int upperIndex = (int)luminunceProfile.Total() - 1;
                for (int index = 0; index < luminunceProfile.Total(); index++)
                {
                    count += luminunceProfile.At<double>(0, index);
                    if (count >= lowThreshold)
                    {
                        lowIndex = index;
                        break;
                    }
                }

                for (int index = lowIndex + 1; index < luminunceProfile.Total(); index++)
                {
                    count += luminunceProfile.At<double>(0, index);
                    if (count >= upperThreshold)
                    {
                        upperIndex = index;
                        break;
                    }
                }

                return (int)((upperIndex - lowIndex) * multiplier);
            }

            using (var matrix = Mat.FromPixelData(image.Height, image.Width, MatType.CV_64FC1, image.Pixels))
            {
                var output = new Size<int>();

                using (Mat horizontalProfile = matrix.Reduce(ReduceDimension.Row, ReduceTypes.Sum, -1))
                {
                    output.Width = borderDetection(horizontalProfile, lowRate, upperRate);
                }

                using (Mat verticalProfile = matrix.Reduce(ReduceDimension.Column, ReduceTypes.Sum, -1))
                {
                    output.Height = borderDetection(verticalProfile, lowRate, upperRate);
                }

                return output;
            }
        }
    }
}
