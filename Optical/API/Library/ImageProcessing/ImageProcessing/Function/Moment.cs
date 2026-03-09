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
        #region Methods
        /// <summary>
        /// 画像の各モーメントを出力します。
        /// </summary>
        /// <param name="image">2次元画像データ</param>
        /// <returns>画像のモーメント</returns>
        private static ImageMoments moment(ImageComponent image)
        {
            using (Mat cvImage = createImage(image.Pixels, image.Height, image.Width, image.BitDepth))
            {
                Moments moment = Cv2.Moments(cvImage);
                var imageMoment = new ImageMoments
                {
                    Raw00 = moment.M00,
                    Raw01 = moment.M01,
                    Raw02 = moment.M02,
                    Raw10 = moment.M10,
                    Raw11 = moment.M11,
                    Raw20 = moment.M20,
                    Centroid02 = moment.Mu02,
                    Centroid11 = moment.Mu11,
                    Centroid20 = moment.Mu20
                };

                return imageMoment;
            }
        }

        /// <summary>
        /// 画像の輝度値を減算した後の各モーメントを出力します。
        /// </summary>
        /// <param name="image">2次元画像データ</param>
        /// <param name="baseline">輝度基準値</param>
        /// <returns>画像のモーメント</returns>
        private static ImageMoments momentOffset(ImageComponent image, double baseline)
        {
            using (Mat cvImage = createImage(image.Pixels, image.Height, image.Width, image.BitDepth))
            using (var result = new Mat())
            {
                cvImage.ConvertTo(cvImage, MatType.CV_64FC1);

                Cv2.Subtract(cvImage, baseline, result);

                Moments moment = Cv2.Moments(result);
                var imageMoment = new ImageMoments
                {
                    Raw00 = moment.M00,
                    Raw01 = moment.M01,
                    Raw02 = moment.M02,
                    Raw10 = moment.M10,
                    Raw11 = moment.M11,
                    Raw20 = moment.M20,
                    Centroid02 = moment.Mu02,
                    Centroid11 = moment.Mu11,
                    Centroid20 = moment.Mu20
                };

                return imageMoment;
            }
        }

        /// <summary>
        /// 画像の各モーメントを出力します。
        /// </summary>
        /// <param name="image">2次元画像データ</param>
        /// <returns>画像のモーメント</returns>
        private static ImageMoments moment(ImageComponent<double> image)
        {
            using (var cvImage = Mat.FromPixelData(image.Height, image.Width, MatType.CV_64FC1, image.Pixels))
            {
                Moments moment = Cv2.Moments(cvImage);
                var imageMoment = new ImageMoments
                {
                    Raw00 = moment.M00,
                    Raw01 = moment.M01,
                    Raw02 = moment.M02,
                    Raw10 = moment.M10,
                    Raw11 = moment.M11,
                    Raw20 = moment.M20,
                    Centroid02 = moment.Mu02,
                    Centroid11 = moment.Mu11,
                    Centroid20 = moment.Mu20
                };

                return imageMoment;
            }
        }

        /// <summary>
        /// 画像の輝度値を減算した後の各モーメントを出力します。
        /// </summary>
        /// <param name="image">2次元画像データ</param>
        /// <param name="baseline">輝度基準値</param>
        /// <returns>画像のモーメント</returns>
        private static ImageMoments momentOffset(ImageComponent<double> image, double baseline)
        {
            using (var cvImage = Mat.FromPixelData(image.Height, image.Width, MatType.CV_64FC1, image.Pixels))
            using (var result = new Mat())
            {
                Cv2.Subtract(cvImage, baseline, result);

                Moments moment = Cv2.Moments(result);
                var imageMoment = new ImageMoments
                {
                    Raw00 = moment.M00,
                    Raw01 = moment.M01,
                    Raw02 = moment.M02,
                    Raw10 = moment.M10,
                    Raw11 = moment.M11,
                    Raw20 = moment.M20,
                    Centroid02 = moment.Mu02,
                    Centroid11 = moment.Mu11,
                    Centroid20 = moment.Mu20
                };

                return imageMoment;
            }
        }

        /// <summary>
        /// 画像の対象領域内の輝度重心を出力します。
        /// </summary>
        /// <param name="image">2次元画像データ</param>
        /// <param name="roi">対象領域</param>
        /// <param name="noiseLevel">ノイズレベル</param>
        /// <param name="peak">最大輝度値</param>
        /// <returns>輝度重心座標[pixel]</returns>
        /// <remarks>ノイズレベル未満のピクセルデータは、輝度重心計算の対象外になります。</remarks>
        public static PointD LuminanceCentroid(ImageComponent image, int noiseLevel, Rectangle roi, out int peak)
        {
            // Momentに統合する。

            // luminunce(Max, Total, FirstMomentX, FirstMomentY)
            (int Max, ulong Total, ulong FirstMomentX, ulong FirstMomentY) luminunce = (Max: 0, Total: 0ul, FirstMomentX: 0ul, FirstMomentY: 0ul);
            object lockLuminunce = new object();
            Func<byte[], int, uint> bitConverter = Bit.SelectConverter(image.BitDepth);
            OrderablePartitioner<Tuple<int, int>> rangePartitioner = Partitioner.Create(0, roi.Height * roi.Width);
            Parallel.ForEach(rangePartitioner,
                             () => (Max: 0, Total: 0ul, FirstMomentX: 0ul, FirstMomentY: 0ul),
                             (range, loopState, subLuminunce) =>
                             {
                                 for (int index = range.Item1; index < range.Item2; index++)
                                 {
                                     int offsetIndex = roi.Top * image.Width + roi.Left;
                                     int targetIndex = offsetIndex + ((index / roi.Width) * image.Width) + (index % roi.Width);

                                     int pixelValue = (int)bitConverter(image.Pixels, targetIndex);
                                     if (pixelValue < noiseLevel)
                                     {
                                         continue;
                                     }

                                     // max
                                     subLuminunce.Max = (pixelValue > subLuminunce.Max) ? pixelValue : subLuminunce.Max;
                                     // total
                                     subLuminunce.Total += (ulong)pixelValue;
                                     // firstMomentX
                                     subLuminunce.FirstMomentX += (ulong)pixelValue * (ulong)(targetIndex % image.Width);
                                     // firstMomentY
                                     subLuminunce.FirstMomentY += (ulong)pixelValue * (ulong)(targetIndex / image.Width);
                                 }

                                 return subLuminunce;
                             },
                             (result) =>
                             {
                                 lock (lockLuminunce)
                                 {
                                     luminunce.Max = (luminunce.Max > result.Max) ? luminunce.Max : result.Max;
                                     luminunce.Total += result.Total;
                                     luminunce.FirstMomentX += result.FirstMomentX;
                                     luminunce.FirstMomentY += result.FirstMomentY;
                                 }
                             });

            peak = luminunce.Max;

            return new PointD((double)luminunce.FirstMomentX / luminunce.Total, (double)luminunce.FirstMomentY / luminunce.Total);
        }

        /// <summary>
        /// 画像の各モーメントを出力します。
        /// </summary>
        /// <param name="image">2次元画像データ</param>
        /// <param name="baseline">輝度基準値</param>
        /// <returns></returns>
        public static ImageMoments Moment(ImageComponent image, double baseline = 0)
        {
            if (baseline > 0)
            {
                return momentOffset(image, baseline);
            }
            else
            {
                return moment(image);
            }
        }

        /// <summary>
        /// 画像の各モーメントを出力します。
        /// </summary>
        /// <param name="image">2次元画像データ</param>
        /// <param name="baseline">輝度基準値（> 0）</param>
        /// <returns></returns>
        public static ImageMoments Moment(ImageComponent<double> image, double baseline = 0)
        {
            if (baseline > 0)
            {
                return momentOffset(image, baseline);
            }
            else
            {
                return moment(image);
            }
        }

        /// <summary>
        /// 行列の各モーメントを出力します。
        /// </summary>
        /// <param name="matrix">行列データ</param>
        /// <returns></returns>
        public static ImageMoments Moment(float[,] matrix)
        {
            Moments moment = Cv2.Moments(matrix);
            var imageMoment = new ImageMoments
            {
                Raw00 = moment.M00,
                Raw01 = moment.M01,
                Raw02 = moment.M02,
                Raw10 = moment.M10,
                Raw11 = moment.M11,
                Raw20 = moment.M20,
                Centroid02 = moment.Mu02,
                Centroid11 = moment.Mu11,
                Centroid20 = moment.Mu20
            };

            return imageMoment;
        }
        #endregion // Methods

        #region Classes
        /// <summary>
        /// 画像のモーメント情報格納クラス
        /// </summary>
        public class ImageMoments
        {
            /// <summary>
            /// 0次モーメント
            /// </summary>
            public double Raw00 { get; set; }

            /// <summary>
            /// 1次モーメント(X軸)
            /// </summary>
            public double Raw10 { get; set; }

            /// <summary>
            /// 1次モーメント(Y軸)
            /// </summary>
            public double Raw01 { get; set; }

            /// <summary>
            /// 2次モーメント(X軸)
            /// </summary>
            public double Raw20 { get; set; }

            /// <summary>
            /// 慣性モーメント
            /// </summary>
            public double Raw11 { get; set; }

            /// <summary>
            /// 2次モーメント(Y軸)
            /// </summary>
            public double Raw02 { get; set; }

            /// <summary>
            /// 中心2次モーメント(X軸)
            /// </summary>
            public double Centroid20 { get; set; }

            /// <summary>
            /// 中心慣性モーメント
            /// </summary>
            public double Centroid11 { get; set; }

            /// <summary>
            /// 中心2次モーメント(Y軸)
            /// </summary>
            public double Centroid02 { get; set; }
        }
        #endregion // Classes
    }
}
