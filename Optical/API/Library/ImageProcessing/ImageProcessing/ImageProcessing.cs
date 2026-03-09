using OpenCvSharp;
using Optical.API.Library.Optics;
using Optical.Platform.Types;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Optical.API.Library
{
    /// <summary>
    /// 画像処理ライブラリクラス
    /// </summary>
    public partial class ImageProcessing
    {
        #region Enums
        /// <summary>
        /// 補間方法
        /// </summary>
        [Flags]
        public enum InterpolationMethod
        {
            /// <summary>
            /// 最近傍補間
            /// </summary>
            Nearest = InterpolationFlags.Nearest,

            /// <summary>
            /// Bilinear補間
            /// </summary>
            Bilinear = InterpolationFlags.Linear,

            /// <summary>
            /// Bicubic補間
            /// </summary>
            Bicubic = InterpolationFlags.Cubic,

            /// <summary>
            /// 面積補間
            /// </summary>
            Area = InterpolationFlags.Area,

            /// <summary>
            /// Lanczos補間
            /// </summary>
            Lanczos = InterpolationFlags.Lanczos4,

            /// <summary>
            /// mask for interpolation codes
            /// </summary>
            Max = InterpolationFlags.Max,

            /// <summary>
            /// Fill all the destination image pixels. If some of them correspond to outliers in the source image, they are set to fillval.
            /// </summary>
            WarpFillOutliers = InterpolationFlags.WarpFillOutliers,

            /// <summary>
            /// Indicates that matrix is inverse transform from destination image to source and, thus, can be used directly for pixel interpolation.
            /// Otherwise, the function finds the inverse transform from map_matrix.
            /// </summary>
            WarpInverseMap = InterpolationFlags.WarpInverseMap
        }
        #endregion // Enums

        #region Methods
        private static Mat createImage(Array rawImage, int rows, int columns, int bitDepth = 8)
        {
            if (rawImage == null)
            {
                throw new ArgumentNullException(nameof(rawImage), "画像データがNULLです。");
            }

            MatType imageType;
            if (bitDepth < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(bitDepth), bitDepth, "ビット深度が範囲外です。（1～16）");
            }
            else if (bitDepth <= 8)
            {
                imageType = MatType.CV_8UC1;
            }
            else if (bitDepth <= 16)
            {
                imageType = MatType.CV_16UC1;
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(bitDepth), bitDepth, "ビット深度が範囲外です。（1～16）");
            }

            return Mat.FromPixelData(rows, columns, imageType, rawImage);
        }

        private static Mat createImage(ImageComponent rawImage)
        {
            if (rawImage.Pixels == null)
            {
                throw new ArgumentNullException(nameof(rawImage.Pixels), "画像データがNULLです。");
            }

            MatType imageType;
            if (rawImage.BitDepth < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(rawImage.BitDepth), rawImage.BitDepth, "ビット深度が範囲外です。（1～16）");
            }
            else if (rawImage.BitDepth <= 8)
            {
                imageType = MatType.CV_8UC1;
            }
            else if (rawImage.BitDepth <= 16)
            {
                imageType = MatType.CV_16UC1;
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(rawImage.BitDepth), rawImage.BitDepth, "ビット深度が範囲外です。（1～16）");
            }

            return Mat.FromPixelData(rawImage.Height, rawImage.Width, imageType, rawImage.Pixels);
        }
        #endregion // Methods
    }
}
