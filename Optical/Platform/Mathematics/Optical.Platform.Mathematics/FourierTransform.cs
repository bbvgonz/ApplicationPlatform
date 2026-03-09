using OpenCvSharp;
using Optical.Platform.Types;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Optical.Platform.Mathematics
{
    /// <summary>
    /// フーリエ変換クラス
    /// </summary>
    public static class FourierTransform
    {
        private static Mat fft2d(ImageComponent image)
        {
            if ((image == null) || (image.Pixels == null))
            {
                throw new ArgumentNullException();
            }

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

            var destination = new Mat();
            using (var rawImage = Mat.FromPixelData(image.Height, image.Width, imageType, image.Pixels))
            {
                // フーリエ変換
                Cv2.Dft(rawImage, destination);
            }

            return destination;
        }

        /// <summary>
        /// 64bit用フーリエ変換クラス
        /// </summary>
        /// <remarks>FFT計算を高速化する場合、「optimalEnable」を有効にします。</remarks>
        private static Mat fft2d(ImageComponent<double> image, bool optimalEnable)
        {
            if ((image == null) || (image.Pixels == null))
            {
                throw new ArgumentNullException();
            }

            using (var rawImage = Mat.FromPixelData(image.Height, image.Width, MatType.CV_64FC1, image.Pixels))
            {
                if (optimalEnable)
                {
                    int rows = Cv2.GetOptimalDFTSize(rawImage.Rows);
                    int columns = Cv2.GetOptimalDFTSize(rawImage.Cols);

                    if (rows != rawImage.Rows || columns != rawImage.Cols)
                    {
                        Cv2.CopyMakeBorder(rawImage, rawImage, 0, rows - rawImage.Rows, 0, columns - rawImage.Cols, BorderTypes.Constant, Scalar.All(0));
                    }
                }

                Mat[] planes = { rawImage, Mat.Zeros(rawImage.Size(), MatType.CV_64FC1) };

                Mat destination = new Mat();
                using (Mat complex = new Mat())
                {
                    Cv2.Merge(planes, complex);
                    // フーリエ変換
                    Cv2.Dft(complex, destination);
                }
                return destination;
            }
        }

        /// <summary>
        /// 離散フーリエ変換（Discrete Fourier Transform）
        /// </summary>
        /// <param name="profile">サンプリングプロファイル（X:距離、Y:量）</param>
        /// <param name="spatialFrequency">空間周波数[cycles/pixel]</param>
        /// <returns>フーリエ変換結果。</returns>
        public static Complex DFT(PointD[] profile, double spatialFrequency)
        {
            // 指数の定数部分
            double fixedIndex = 2 * Math.PI * spatialFrequency;

            double real = 0;
            double imaginary = 0;
            for (int index = 0; index < profile.Length; index++)
            {
                if (profile[index].Y == 0)
                {
                    continue;
                }

                real += profile[index].Y * Math.Cos(fixedIndex * profile[index].X);
                imaginary += profile[index].Y * Math.Sin(fixedIndex * profile[index].X);
            }

            return new Complex(real, imaginary);
        }

        /// <summary>
        /// 2次元高速フーリエ変換（Fast Fourier Transform）
        /// </summary>
        /// <param name="image">変換元画像</param>
        /// <returns>フーリエ変換後複素数画像</returns>
        public static ImageComponent<Complex> FFT(ImageComponent image)
        {
            Mat dftImage = fft2d(image);

            Mat[] dftComplex = Cv2.Split(dftImage);
            var complexs = new List<Complex>(dftImage.Width * dftImage.Height);
            Mat.Indexer<double> realIndexer = dftComplex[0].GetGenericIndexer<double>();
            Mat.Indexer<double> imagineIndexer = dftComplex[1].GetGenericIndexer<double>();
            for (int index = 0; index < complexs.Capacity; index++)
            {
                complexs.Add(new Complex(realIndexer[index], imagineIndexer[index]));
            }

            var complexMap = new ImageComponent<Complex>()
            {
                BitDepth = image.BitDepth,
                Height = dftImage.Height,
                Pixels = complexs.ToArray(),
                Width = dftImage.Width
            };

            return complexMap;
        }

        /// <summary>
        /// 2次元高速フーリエ変換（Fast Fourier Transform）
        /// </summary>
        /// <param name="image">変換元画像</param>
        /// <returns>フーリエ変換後振幅画像</returns>
        public static ImageComponent<double> FFTMagnitude(ImageComponent image)
        {
            Mat dftImage = fft2d(image);
            Mat[] dftComplex = Cv2.Split(dftImage);

            var magnitude = new Mat();
            Cv2.Magnitude(dftComplex[0], dftComplex[1], magnitude);

            var output = new ImageComponent<double>()
            {
                BitDepth = image.BitDepth,
                Height = magnitude.Height,
                Pixels = new double[magnitude.Total()],
                Width = magnitude.Width
            };
            System.Runtime.InteropServices.Marshal.Copy(magnitude.Data, output.Pixels, 0, output.Pixels.Length);

            return output;
        }

        /// <summary>
        /// 2次元高速フーリエ変換（Fast Fourier Transform）
        /// </summary>
        /// <param name="image">変換元画像</param>
        /// <param name="optimalEnable">FFT計算高速化のための画像サイズ最適化</param>
        /// <returns>フーリエ変換後振幅画像</returns>
        public static ImageComponent<double> FFTMagnitude(ImageComponent<double> image, bool optimalEnable)
        {
            Mat dftImage = fft2d(image, optimalEnable);
            Mat[] dftComplex = Cv2.Split(dftImage);

            var magnitude = new Mat();
            Cv2.Magnitude(dftComplex[0], dftComplex[1], magnitude);

            var output = new ImageComponent<double>()
            {
                BitDepth = image.BitDepth,
                Height = magnitude.Height,
                Pixels = new double[magnitude.Total()],
                Width = magnitude.Width
            };
            System.Runtime.InteropServices.Marshal.Copy(magnitude.Data, output.Pixels, 0, output.Pixels.Length);

            return output;
        }
    }
}
