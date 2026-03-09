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
        #region Enums
        /// <summary>
        /// MorphologyのKernel形状
        /// </summary>
        public enum MorphologyShape
        {
            /// <summary>
            /// 十字
            /// </summary>
            Cross = MorphShapes.Cross,
            /// <summary>
            /// 楕円
            /// </summary>
            Ellipse = MorphShapes.Ellipse,
            /// <summary>
            /// 矩形
            /// </summary>
            Rectangle = MorphShapes.Rect
        }

        /// <summary>
        /// Morphology操作の種類
        /// </summary>
        public enum MorphologyOperation
        {
            /// <summary>
            /// 収縮
            /// </summary>
            Erode = MorphTypes.Erode,
            /// <summary>
            /// 膨張
            /// </summary>
            Dilate = MorphTypes.Dilate,
            /// <summary>
            /// 収縮後膨張
            /// </summary>
            Open = MorphTypes.Open,
            /// <summary>
            /// 膨張後収縮
            /// </summary>
            Close = MorphTypes.Close,
            /// <summary>
            /// 膨張・収縮の差分（境界線）
            /// </summary>
            Gradient = MorphTypes.Gradient,
            /// <summary>
            /// 入力と<see cref="MorphTypes.Open"/>の差分
            /// </summary>
            TopHat = MorphTypes.TopHat,
            /// <summary>
            /// 入力と<see cref="MorphTypes.Close"/>の差分
            /// </summary>
            BlackHat = MorphTypes.BlackHat,
            /// <summary>
            /// Hit-or-Miss変換（Kernelパターン検出）
            /// </summary>
            HitMiss = MorphTypes.HitMiss
        }
        #endregion // Enums

        #region Methods
        /// <summary>
        /// Morphology変換用の構造要素を出力します。
        /// </summary>
        /// <param name="shape">Kernel形状</param>
        /// <param name="size">Kernelサイズ</param>
        /// <param name="anchor">Kernel内のAnchor座標 (左上原点 / (-1, -1):Kernel中心)</param>
        /// <returns>構造要素</returns>
        public static ImageComponent MorphologyKernel(MorphologyShape shape, Size<int> size, Point<int> anchor)
        {
            if (!Enum.IsDefined(typeof(MorphologyShape), shape))
            {
                throw new ArgumentOutOfRangeException(nameof(shape), shape, "The specified shape is not supported.");
            }

            var kernelSize = new OpenCvSharp.Size(size.Width, size.Height);
            var anchorPoint = new OpenCvSharp.Point(anchor.X, anchor.Y);
            Mat kernel = Cv2.GetStructuringElement((MorphShapes)shape, kernelSize, anchorPoint);
            var output = new ImageComponent()
            {
                BitDepth = 8,
                Height = kernel.Height,
                Pixels = new byte[kernel.Total() * kernel.ElemSize()],
                Width = kernel.Width
            };
            System.Runtime.InteropServices.Marshal.Copy(kernel.Data, output.Pixels, 0, output.Pixels.Length);

            kernel.Dispose();

            return output;
        }

        /// <summary>
        /// Morphology変換した画像を出力します。
        /// </summary>
        /// <param name="image">入力画像</param>
        /// <param name="operation">Morphology操作方法</param>
        /// <param name="kernel">構造要素</param>
        /// <param name="iteration">繰り返し回数</param>
        /// <returns>出力画像</returns>
        public static ImageComponent MorphologyTransform(ImageComponent image, MorphologyOperation operation, ImageComponent kernel, int iteration = 1)
        {
            Mat sourceImage = createImage(image);
            var destinationImage = new Mat();
            Mat element = createImage(kernel);
            Cv2.MorphologyEx(sourceImage, destinationImage, (MorphTypes)operation, element, iterations: iteration);

            var output = new ImageComponent()
            {
                BitDepth = image.BitDepth,
                Height = destinationImage.Height,
                Pixels = new byte[destinationImage.Total() * destinationImage.ElemSize()],
                Width = destinationImage.Width
            };
            System.Runtime.InteropServices.Marshal.Copy(destinationImage.Data, output.Pixels, 0, output.Pixels.Length);

            sourceImage.Dispose();
            destinationImage.Dispose();
            element.Dispose();

            return output;
        }
        #endregion // Methods
    }
}
