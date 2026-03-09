using OpenCvSharp;
using Optical.API.Library.Optics;
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
        private static ImageComponent maskImageEllipse(ImageComponent image, Rectangle roi, double angle, MatType imageType, bool view = false)
        {
            // マスク画像を生成する。
            Mat mask = Mat.Zeros(image.Height, image.Width, imageType);
            var rotatedRect = new RotatedRect()
            {
                Angle = (float)angle,
                Center = new Point2f((roi.Width / 2) + roi.X, (roi.Height / 2) + roi.Y),
                Size = new Size2f(roi.Width, roi.Height)
            };
            Cv2.Ellipse(mask, rotatedRect, Scalar.All((1 << image.BitDepth) - 1), -1);

            // オリジナル画像にマスク画像を適用する。
            Mat original = createImage(image.Pixels, image.Height, image.Width, image.BitDepth);
            var result = new Mat();
            Cv2.BitwiseAnd(original, mask, result);

            if (view)
            {
                var display = new Mat();
                result.ConvertTo(display, MatType.CV_8UC1);
                Cv2.ImShow("Mask Image", result);
            }

            mask.Dispose();
            original.Dispose();

            var output = new ImageComponent()
            {
                BitDepth = image.BitDepth,
                Height = result.Height,
                Pixels = new byte[result.Total() * result.ElemSize()],
                Width = result.Width
            };
            System.Runtime.InteropServices.Marshal.Copy(result.Data, output.Pixels, 0, output.Pixels.Length);

            result.Dispose();

            return output;
        }

        private static ImageComponent<double> maskImageEllipse(ImageComponent<double> image, Rectangle roi, double angle)
        {
            // マスク画像を生成する。
            Mat mask = Mat.Zeros(image.Height, image.Width, MatType.CV_64FC1);
            var rotatedRect = new RotatedRect()
            {
                Angle = (float)angle,
                Center = new Point2f((roi.Width / 2) + roi.X, (roi.Height / 2) + roi.Y),
                Size = new Size2f(roi.Width, roi.Height)
            };
            Cv2.Ellipse(mask, rotatedRect, Scalar.All(1), -1);

            // オリジナル画像にマスク画像を適用する。
            var original = Mat.FromPixelData(image.Height, image.Width, MatType.CV_64FC1, image.Pixels);
            var result = new Mat();
            Cv2.Multiply(original, mask, result);

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

        private static ImageComponent maskImageRectangle(ImageComponent image, Rectangle roi, MatType imageType, bool view = false)
        {
            // マスク画像を生成する。
            Mat mask = Mat.Zeros(roi.Height, roi.Width, imageType);
            var rect = new Rect(roi.X, roi.Y, roi.Width, roi.Height);
            Cv2.Rectangle(mask, rect, Scalar.All((1 << image.BitDepth) - 1), -1);

            // オリジナル画像にマスク画像を適用する。
            Mat original = createImage(image.Pixels, image.Height, image.Width, image.BitDepth);
            var result = new Mat();
            Cv2.BitwiseAnd(original, mask, result);

            if (view)
            {
                var display = new Mat();
                result.ConvertTo(display, MatType.CV_8UC1);
                Cv2.ImShow("Mask Image", display);
            }

            mask.Dispose();
            original.Dispose();

            var output = new ImageComponent()
            {
                BitDepth = image.BitDepth,
                Height = result.Height,
                Pixels = new byte[result.Total() * result.ElemSize()],
                Width = result.Width
            };
            System.Runtime.InteropServices.Marshal.Copy(result.Data, output.Pixels, 0, output.Pixels.Length);

            result.Dispose();

            return output;
        }

        private static ImageComponent<double> maskImageRectangle(ImageComponent<double> image, Rectangle roi)
        {
            // マスク画像を生成する。
            Mat mask = Mat.Zeros(image.Height, image.Width, MatType.CV_64FC1);
            var rect = new Rect(roi.X, roi.Y, roi.Width, roi.Height);
            Cv2.Rectangle(mask, rect, Scalar.All(1), -1);

            // オリジナル画像にマスク画像を適用する。
            var original = Mat.FromPixelData(image.Height, image.Width, MatType.CV_64FC1, image.Pixels);
            var result = new Mat();
            Cv2.Multiply(original, mask, result);

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

        /// <summary>
        /// 画像の対象領域外のピクセル値を0にする。
        /// </summary>
        /// <param name="image">Mask対象画像</param>
        /// <param name="roi">Mask対象領域</param>
        /// <param name="shape">Apertureの形状</param>
        /// <param name="angle">ROIの回転角[deg]（極座標系）</param>
        /// <param name="view">Mask後画像の表示</param>
        /// <returns>Mask後画像</returns>
        public static ImageComponent MaskImage(ImageComponent image, Rectangle roi, ApertureShape shape, double angle = 0, bool view = false)
        {
            MatType imageType;
            if (image.BitDepth < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(image.BitDepth), image.BitDepth, "ビット深度が範囲外です。（1～16）");
            }
            else if (image.BitDepth <= 8)
            {
                imageType = MatType.CV_8UC1;
            }
            else if (image.BitDepth <= 16)
            {
                imageType = MatType.CV_16UC1;
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(image.BitDepth), image.BitDepth, "ビット深度が範囲外です。（1～16）");
            }

            if (shape == ApertureShape.Circle)
            {
                return maskImageEllipse(image, roi, angle, imageType, view);
            }
            else if (shape == ApertureShape.Rectangle)
            {
                return maskImageRectangle(image, roi, imageType, view);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(shape), shape, $"{shape} is not supported.");
            }
        }

        /// <summary>
        /// 画像の対象領域外のピクセル値を0にする。
        /// </summary>
        /// <param name="image">Mask対象画像</param>
        /// <param name="roi">Mask対象領域</param>
        /// <param name="shape">Apertureの形状</param>
        /// <param name="angle">ROIの回転角[deg]（極座標系）</param>
        /// <returns>マスク処理後の画像</returns>
        public static ImageComponent<double> MaskImage(ImageComponent<double> image, Rectangle roi, ApertureShape shape, double angle = 0)
        {
            MatType imageType;
            if (image.BitDepth < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(image.BitDepth), image.BitDepth, "ビット深度が範囲外です。（1～16）");
            }
            else if (image.BitDepth <= 8)
            {
                imageType = MatType.CV_8UC1;
            }
            else if (image.BitDepth <= 16)
            {
                imageType = MatType.CV_16UC1;
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(image.BitDepth), image.BitDepth, "ビット深度が範囲外です。（1～16）");
            }

            if (shape == ApertureShape.Circle)
            {
                return maskImageEllipse(image, roi, angle);
            }
            else if (shape == ApertureShape.Rectangle)
            {
                return maskImageRectangle(image, roi);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(shape), shape, $"{shape} is not supported.");
            }
        }
    }
}
