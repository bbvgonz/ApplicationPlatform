using OpenCvSharp;
using Optical.API.Library.Optics;
using Optical.Platform.Types;
using System.Drawing;

namespace Optical.API.Library
{
    /// <summary>
    /// 画像処理ライブラリクラス
    /// </summary>
    public partial class ImageProcessing
    {
        private static ImageComponent<double> trimEllipse(ImageComponent<double> image, Rectangle roi, double angle)
        {
            // マスク画像を生成する。
            Mat mask = Mat.Zeros(roi.Height, roi.Width, MatType.CV_64FC1);
            var rotatedRect = new RotatedRect()
            {
                Angle = (float)angle,
                Center = new Point2f((roi.Width / 2), (roi.Height / 2)),
                Size = new Size2f(roi.Width, roi.Height)
            };
            Cv2.Ellipse(mask, rotatedRect, Scalar.All(1), -1);

            // マスク画像の範囲外調整。
            var roiArea = new Rect(0, 0, roi.Width, roi.Height);
            var imageArea = new Rect(-roi.X, -roi.Y, image.Width, image.Height);
            Rect trim = roiArea.Intersect(imageArea);
            if ((trim.Width <= 0) || (trim.Height <= 0))
            {
                mask.Dispose();
                return new ImageComponent<double>();
            }
            Mat trimMask = mask.Clone(trim);

            // オリジナル画像にマスク画像を適用する。
            trim.X = (roi.X < 0) ? 0 : roi.X;
            trim.Y = (roi.Y < 0) ? 0 : roi.Y;
            Mat original = Mat.FromPixelData(image.Height, image.Width, MatType.CV_64FC1, image.Pixels).SubMat(trim);
            var result = new Mat();
            Cv2.Multiply(original, trimMask, result);

            mask.Dispose();
            original.Dispose();

            var output = new ImageComponent<double>()
            {
                BitDepth = image.BitDepth,
                Height = result.Height,
                Pixels = new double[result.Total()],
                Width = result.Width
            };
            System.Runtime.InteropServices.Marshal.Copy(result.Data, output.Pixels, 0, output.Pixels.Length);

            result.Dispose();

            return output;
        }

        private static ImageComponent<double> trimRectangle(ImageComponent<double> image, Rectangle roi, double angle)
        {
            // 画像切り抜きの範囲外調整。
            var roiArea = new Rect(0, 0, roi.Width, roi.Height);
            var imageArea = new Rect(-roi.X, -roi.Y, image.Width, image.Height);
            Rect trim = roiArea.Intersect(imageArea);

            // オリジナル画像にマスク画像を適用する。
            using (Mat original = createImage(image.Pixels, image.Height, image.Width, image.BitDepth).SubMat(trim))
            {
                original.ConvertTo(original, MatType.CV_64FC1);

                var output = new ImageComponent<double>()
                {
                    BitDepth = image.BitDepth,
                    Height = original.Height,
                    Pixels = new double[original.Total()],
                    Width = original.Width
                };
                System.Runtime.InteropServices.Marshal.Copy(original.Data, output.Pixels, 0, output.Pixels.Length);

                return output;
            }
        }

        /// <summary>
        /// 指定された範囲を切り抜いた画像を出力します。
        /// </summary>
        /// <param name="image">2次元画像データ</param>
        /// <param name="roi">切り抜く範囲</param>
        /// <param name="shape">切り抜く範囲の形</param>
        /// <param name="view">Trim後の画像表示</param>
        /// <returns></returns>
        public static ImageComponent Trim(ImageComponent image, Rectangle roi, ApertureShape shape, bool view = false)
        {
            using (Mat cvImage = createImage(image.Pixels, image.Height, image.Width, image.BitDepth))
            {
                var rect = new Rect(roi.X, roi.Y, roi.Width, roi.Height);
                Mat trimImage = cvImage.Clone(rect);

                if (view)
                {
                    Cv2.ImShow("Trim", trimImage);
                }

                var result = new ImageComponent()
                {
                    BitDepth = image.BitDepth,
                    Height = roi.Height,
                    Pixels = new byte[roi.Height * roi.Width * Bit.ToByte(image.BitDepth)],
                    Width = roi.Width
                };
                System.Runtime.InteropServices.Marshal.Copy(trimImage.Data, result.Pixels, 0, result.Pixels.Length);

                return result;
            }
        }

        /// <summary>
        /// 指定された範囲を切り抜いた画像を出力します。
        /// </summary>
        /// <param name="image">2次元画像データ</param>
        /// <param name="roi">切り抜く範囲</param>
        /// <param name="shape">切り抜く範囲の形</param>
        /// <returns></returns>
        public static ImageComponent<double> Trim(ImageComponent<double> image, Rectangle roi, ApertureShape shape)
        {
            if (shape == ApertureShape.Circle)
            {
                return trimEllipse(image, roi, 0);
            }
            else
            {
                return trimRectangle(image, roi, 0);
            }
        }
    }
}
