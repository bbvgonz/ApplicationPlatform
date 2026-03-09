using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Numerics;
using OpenCvSharp;
using Optical.Platform.Types;

namespace Optical.Platform.Mathematics
{
    /// <summary>
    /// 方程式クラス
    /// </summary>
    public static class Equation
    {
        #region Fields
        /// <summary>
        /// フリンジ・ゼルニケ指数
        /// </summary>
        private static readonly (int n, int m)[] fringeZernikeIndex =
        [
            (-1, -1),
            (0, 0),
            (1, 1),
            (1, -1),
            (2, 0),
            (2, 2),
            (2, -2),
            (3, 1),
            (3, -1),
            (4, 0),
            (3, 3),
            (3, -3),
            (4, 2),
            (4, -2),
            (5, 1),
            (5, -1),
            (6, 0),
            (4, 4),
            (4, -4),
            (5, 3),
            (5, -3),
            (6, 2),
            (6, -2),
            (7, 1),
            (7, -1),
            (8, 0),
            (5, 5),
            (5, -5),
            (6, 4),
            (6, -4),
            (7, 3),
            (7, -3),
            (8, 2),
            (8, -2),
            (9, 1),
            (9, -1),
            (10, 0)
        ];

        /// <summary>
        /// Zernike多項式 X偏導関数
        /// </summary>
        private static readonly Dictionary<int, Func<double[], double[], double>> zernikeDerivativeXTable = new()
        {
            [1] = (x, y) => 0,
            [2] = (x, y) => 1,
            [3] = (x, y) => 0,
            [4] = (x, y) => 4 * x[1],
            [5] = (x, y) => 2 * x[1],
            [6] = (x, y) => 2 * y[1],
            [7] = (x, y) => (9 * x[2]) + (3 * y[2]) - 2,
            [8] = (x, y) => 6 * x[1] * y[1],
            [9] = (x, y) => (24 * x[3]) + (24 * x[1] * y[2]) - (12 * x[1]),
            [10] = (x, y) => (3 * x[2]) - (3 * y[2]),
            [11] = (x, y) => 6 * x[1] * y[1],
            [12] = (x, y) => (16 * x[3]) - (6 * x[1]),
            [13] = (x, y) => (24 * x[2] * y[1]) + (8 * y[3]) - (6 * y[1]),
            [14] = (x, y) => (50 * x[4]) + (60 * x[2] * y[2]) + (10 * y[4]) - (36 * x[2]) - (12 * y[2]) + 3,
            [15] = (x, y) => (40 * x[3] * y[1]) + (40 * x[1] * y[3]) - (24 * x[1] * y[1]),
            [16] = (x, y) => (120 * x[5]) + (240 * x[3] * y[2]) + (120 * x[1] * y[4]) - (120 * x[3]) - (120 * x[1] * y[2]) + (24 * x[1]),
            [17] = (x, y) => (4 * x[3]) - (12 * x[1] * y[2]),
            [18] = (x, y) => (12 * x[2] * y[1]) - (4 * y[3]),
            [19] = (x, y) => (25 * x[4]) - (30 * x[2] * y[2]) - (15 * y[4]) - (12 * x[2]) + (12 * y[2]),
            [20] = (x, y) => (60 * x[3] * y[1]) + (20 * x[1] * y[3]) - (24 * x[1] * y[1]),
            [21] = (x, y) => (90 * x[5]) + (60 * x[3] * y[2]) - (30 * x[1] * y[4]) - (80 * x[3]) + (12 * x[1]),
            [22] = (x, y) => (150 * x[4] * y[1]) + (180 * x[2] * y[3]) + (30 * y[5]) - (120 * x[2] * y[1]) - (40 * y[3]) + (12 * y[1]),
            [23] = (x, y) => (245 * x[6]) + (525 * x[4] * y[2]) + (315 * x[2] * y[4]) + (35 * y[6]) - (300 * x[4]) - (360 * x[2] * y[2]) - (60 * y[4]) + (90 * x[2]) + (30 * y[2]) - 4,
            [24] = (x, y) => (210 * x[5] * y[1]) + (420 * x[3] * y[3]) + (210 * x[1] * y[5]) - (240 * x[3] * y[1]) - (240 * x[1] * y[3]) + (60 * x[1] * y[1]),
            [25] = (x, y) => (560 * x[7]) + (1680 * x[5] * y[2]) + (1680 * x[3] * y[4]) + (560 * x[1] * y[6]) - (840 * x[5]) - (1680 * x[3] * y[2]) - (840 * x[1] * y[4]) + (360 * x[3]) + (360 * x[1] * y[2]) - (40 * x[1]),
            [26] = (x, y) => (5 * x[4]) - (30 * x[2] * y[2]) + (5 * y[4]),
            [27] = (x, y) => (20 * x[3] * y[1]) - (20 * x[1] * y[3]),
            [28] = (x, y) => (36 * x[5]) - (120 * x[3] * y[2]) - (60 * x[1] * y[4]) - (20 * x[3]) + (60 * x[1] * y[2]),
            [29] = (x, y) => (120 * x[4] * y[1]) - (24 * y[5]) - (60 * x[2] * y[1]) + (20 * y[3]),
            [30] = (x, y) => (147 * x[6]) - (105 * x[4] * y[2]) - (315 * x[2] * y[4]) - (63 * y[6]) - (150 * x[4]) + (180 * x[2] * y[2]) + (90 * y[4]) + (30 * x[2]) - (30 * y[2]),
            [31] = (x, y) => (378 * x[5] * y[1]) + (420 * x[3] * y[3]) + (42 * x[1] * y[5]) - (360 * x[3] * y[1]) - (120 * x[1] * y[3]) + (60 * x[1] * y[1]),
            [32] = (x, y) => (448 * x[7]) + (672 * x[5] * y[2]) - (224 * x[1] * y[6]) - (630 * x[5]) - (420 * x[3] * y[2]) + (210 * x[1] * y[4]) + (240 * x[3]) - (20 * x[1]),
            [33] = (x, y) => (784 * x[6] * y[1]) + (1680 * x[4] * y[3]) + (1008 * x[2] * y[5]) - (1050 * x[4] * y[1]) - (1260 * x[2] * y[3]) + (360 * x[2] * y[1]) + (112 * y[7]) - (210 * y[5]) + (120 * y[3]) - (20 * y[1]),
            [34] = (x, y) => (1134 * x[8]) + (3528 * x[6] * y[2]) + (3780 * x[4] * y[4]) + (1512 * x[2] * y[6]) + (126 * y[8]) - (1960 * x[6]) - (4200 * x[4] * y[2]) - (2520 * x[2] * y[4]) - (280 * y[6]) + (1050 * x[4]) + (1260 * x[2] * y[2]) + (210 * y[4]) - (180 * x[2]) - (60 * y[2]) + 5,
            [35] = (x, y) => (1008 * x[7] * y[1]) + (3024 * x[5] * y[3]) + (3024 * x[3] * y[5]) + (1008 * x[1] * y[7]) - (1680 * x[5] * y[1]) - (3360 * x[3] * y[3]) - (1680 * x[1] * y[5]) + (840 * x[3] * y[1]) + (840 * x[1] * y[3]) - (120 * x[1] * y[1]),
            [36] = (x, y) => (2520 * x[9]) + (10080 * x[7] * y[2]) + (15120 * x[5] * y[4]) + (10080 * x[3] * y[6]) + (2520 * x[1] * y[8]) - (5040 * x[7]) - (15120 * x[5] * y[2]) - (15120 * x[3] * y[4]) - (5040 * x[1] * y[6]) + (3360 * x[5]) + (6720 * x[3] * y[2]) + (3360 * x[1] * y[4]) - (840 * x[3]) - (840 * x[1] * y[2]) + (60 * x[1]),
        };

        /// <summary>
        /// Zernike多項式 Y偏導関数
        /// </summary>
        private static readonly Dictionary<int, Func<double[], double[], double>> zernikeDerivativeYTable = new()
        {
            [1] = (x, y) => 0,
            [2] = (x, y) => 0,
            [3] = (x, y) => 1,
            [4] = (x, y) => 4 * y[1],
            [5] = (x, y) => -2 * y[1],
            [6] = (x, y) => 2 * x[1],
            [7] = (x, y) => 6 * x[1] * y[1],
            [8] = (x, y) => (3 * x[2]) + (9 * y[2]) - 2,
            [9] = (x, y) => (24 * x[2] * y[1]) + (24 * y[3]) - (12 * y[1]),
            [10] = (x, y) => -6 * x[1] * y[1],
            [11] = (x, y) => (3 * x[2]) - (3 * y[2]),
            [12] = (x, y) => (-16 * y[3]) + (6 * y[1]),
            [13] = (x, y) => (8 * x[3]) + (24 * x[1] * y[2]) - (6 * x[1]),
            [14] = (x, y) => (40 * x[3] * y[1]) + (40 * x[1] * y[3]) - (24 * x[1] * y[1]),
            [15] = (x, y) => (10 * x[4]) + (60 * x[2] * y[2]) + (50 * y[4]) - (12 * x[2]) - (36 * y[2]) + 3,
            [16] = (x, y) => (120 * x[4] * y[1]) + (240 * x[2] * y[3]) + (120 * y[5]) - (120 * x[2] * y[1]) - (120 * y[3]) + (24 * y[1]),
            [17] = (x, y) => (-12 * x[2] * y[1]) + (4 * y[3]),
            [18] = (x, y) => (4 * x[3]) - (12 * x[1] * y[2]),
            [19] = (x, y) => (-20 * x[3] * y[1]) - (60 * x[1] * y[3]) + (24 * x[1] * y[1]),
            [20] = (x, y) => (15 * x[4]) + (30 * x[2] * y[2]) - (25 * y[4]) - (12 * x[2]) + (12 * y[2]),
            [21] = (x, y) => (30 * x[4] * y[1]) - (60 * x[2] * y[3]) - (90 * y[5]) + (80 * y[3]) - (12 * y[1]),
            [22] = (x, y) => (30 * x[5]) + (180 * x[3] * y[2]) + (150 * x[1] * y[4]) - (40 * x[3]) - (120 * x[1] * y[2]) + (12 * x[1]),
            [23] = (x, y) => (210 * x[5] * y[1]) + (420 * x[3] * y[3]) + (210 * x[1] * y[5]) - (240 * x[3] * y[1]) - (240 * x[1] * y[3]) + (60 * x[1] * y[1]),
            [24] = (x, y) => (35 * x[6]) + (315 * x[4] * y[2]) + (525 * x[2] * y[4]) + (245 * y[6]) - (60 * x[4]) - (360 * x[2] * y[2]) - (300 * y[4]) + (30 * x[2]) + (90 * y[2]) - 4,
            [25] = (x, y) => (560 * x[6] * y[1]) + (1680 * x[4] * y[3]) + (1680 * x[2] * y[5]) + (560 * y[7]) - (840 * x[4] * y[1]) - (1680 * x[2] * y[3]) - (840 * y[5]) + (360 * x[2] * y[1]) + (360 * y[3]) - (40 * y[1]),
            [26] = (x, y) => (-20 * x[3] * y[1]) + (20 * x[1] * y[3]),
            [27] = (x, y) => (5 * x[4]) - (30 * x[2] * y[2]) + (5 * y[4]),
            [28] = (x, y) => (-60 * x[4] * y[1]) - (120 * x[2] * y[3]) + (36 * y[5]) + (60 * x[2] * y[1]) - (20 * y[3]),
            [29] = (x, y) => (24 * x[5]) - (120 * x[1] * y[4]) - (20 * x[3]) + (60 * x[1] * y[2]),
            [30] = (x, y) => (-42 * x[5] * y[1]) - (420 * x[3] * y[3]) - (378 * x[1] * y[5]) + (120 * x[3] * y[1]) + (360 * x[1] * y[3]) - (60 * x[1] * y[1]),
            [31] = (x, y) => (63 * x[6]) + (315 * x[4] * y[2]) + (105 * x[2] * y[4]) - (90 * x[4]) - (180 * x[2] * y[2]) + (30 * x[2]) - (147 * y[6]) + (150 * y[4]) - (30 * y[2]),
            [32] = (x, y) => (224 * x[6] * y[1]) - (672 * x[2] * y[5]) - (448 * y[7]) - (210 * x[4] * y[1]) + (420 * x[2] * y[3]) + (630 * y[5]) - (240 * y[3]) + (20 * y[1]),
            [33] = (x, y) => (112 * x[7]) + (1008 * x[5] * y[2]) + (1680 * x[3] * y[4]) - (210 * x[5]) - (1260 * x[3] * y[2]) + (120 * x[3]) + (784 * x[1] * y[6]) - (1050 * x[1] * y[4]) + (360 * x[1] * y[2]) - (20 * x[1]),
            [34] = (x, y) => (1008 * x[7] * y[1]) + (3024 * x[5] * y[3]) + (3024 * x[3] * y[5]) + (1008 * x[1] * y[7]) - (1680 * x[5] * y[1]) - (3360 * x[3] * y[3]) - (1680 * x[1] * y[5]) + (840 * x[3] * y[1]) + (840 * x[1] * y[3]) - (120 * x[1] * y[1]),
            [35] = (x, y) => (126 * x[8]) + (1512 * x[6] * y[2]) + (3780 * x[4] * y[4]) + (3528 * x[2] * y[6]) + (1134 * y[8]) - (280 * x[6]) - (2520 * x[4] * y[2]) - (4200 * x[2] * y[4]) - (1960 * y[6]) + (210 * x[4]) + (1260 * x[2] * y[2]) + (1050 * y[4]) - (60 * x[2]) - (180 * y[2]) + 5,
            [36] = (x, y) => (2520 * x[8] * y[1]) + (10080 * x[6] * y[3]) + (15120 * x[4] * y[5]) + (10080 * x[2] * y[7]) + (2520 * y[9]) - (5040 * x[6] * y[1]) - (15120 * x[4] * y[3]) - (15120 * x[2] * y[5]) - (5040 * y[7]) + (3360 * x[4] * y[1]) + (6720 * x[2] * y[3]) + (3360 * y[5]) - (840 * x[2] * y[1]) - (840 * y[3]) + (60 * y[1]),
        };
        #endregion // Fields

        /// <summary>
        /// 複素数の立方根を計算する。
        /// </summary>
        /// <param name="x">複素数値</param>
        /// <returns>立方根</returns>
        private static Complex cubicRoot(Complex x)
        {
            double z = Complex.Abs(x);
            z = Math.Pow(z, 1.0 / 3.0);
            double t = x.Phase / 3.0;
            return new Complex(z * Math.Cos(t), z * Math.Sin(t));
        }

        /// <summary>
        /// 指定された次数までのLegendre多項式計算結果を出力します。
        /// </summary>
        /// <param name="x">説明変数</param>
        /// <param name="degree">次数</param>
        /// <returns>Legendre多項式計算結果。</returns>
        /// <remarks>[Index] = 次数</remarks>
        private static double[] legendrePolynomials(double x, int degree)
        {
            if (degree < 0)
            {
                return [];
            }

            var legendre = new double[degree + 1];
            legendre[0] = 1;
            if (degree == 0)
            {
                return legendre;
            }

            legendre[1] = x;
            if (degree == 1)
            {
                return legendre;
            }

            for (int index = 1; index < degree; index++)
            {
                legendre[index + 1] = ((((2 * index) + 1) * x * legendre[index]) - (index * legendre[index - 1])) / (index + 1);
            }

            return legendre;
        }

        /// <summary>
        /// 複素数の立方根を計算する。
        /// </summary>
        /// <param name="x">複素数値</param>
        /// <returns>立方根</returns>
        public static Complex CubicRoot(Complex x)
        {
            return cubicRoot(x);
        }

        /// <summary>
        /// 指定された次数までのLegendre多項式導関数による計算結果を出力します。
        /// </summary>
        /// <param name="x">説明変数</param>
        /// <param name="degree">次数</param>
        /// <returns>Legendre導関数計算結果。</returns>
        /// <remarks>[Index] = 次数</remarks>
        public static (double[] Derivatives, double[] Polynomials) LegendreDerivatives(double x, int degree)
        {
            if (degree < 0)
            {
                return ([], []);
            }

            var polynomials = legendrePolynomials(x, degree);
            var derivatives = new double[degree + 1];
            derivatives[0] = 0;
            for (int n = 0; n < degree; n++)
            {
                for (int k = 0; k <= (n / 2); k++)
                {
                    derivatives[n + 1] += (2 * (n - (2 * k)) + 1) * polynomials[n - (2 * k)];
                }
            }

            return (derivatives, polynomials);
        }

        /// <summary>
        /// 指定された次数までのLegendre多項式による計算結果を出力します。
        /// </summary>
        /// <param name="x">説明変数</param>
        /// <param name="degree">次数</param>
        /// <returns>Legendre多項式計算結果。</returns>
        /// <remarks>[Index] = 次数</remarks>
        public static double[] LegendrePolynomials(double x, int degree)
        {
            return legendrePolynomials(x, degree);
        }

        /// <summary>
        /// 3次方程式の解を計算する。
        /// </summary>
        /// <param name="a">3次係数</param>
        /// <param name="b">2次係数</param>
        /// <param name="c">1次係数</param>
        /// <param name="d">定数項</param>
        /// <returns>3次方程式の解</returns>
        /// <remarks>虚数部に計算機イプシロン(2.22044e-16)のn倍の演算誤差が発生する場合がある。</remarks>
        public static Complex[] SolveCubic(double a, double b, double c, double d)
        {
            // カルダノの公式を用いて計算する。

            // q/2
            var A = new Complex(((2 * b * b * b) - (9 * a * b * c) + (27.0 * a * a * d)) / (54.0 * a * a * a), 0);

            // p/3
            double b3 = ((3.0 * a * c) - (b * b)) / (9.0 * a * a);

            // p^3/27
            var B3 = new Complex(b3 * b3 * b3, 0);

            // u^3
            var C = Complex.Sqrt(A * A + B3);

            // A/3
            var D = new Complex(b / (3.0 * a), 0);

            // u
            Complex Tplus = -A + C;
            Complex E1;
            if (Tplus.Imaginary == 0)
            {
                var u3 = new Complex(Math.Abs(Tplus.Real), 0);
                E1 = cubicRoot(u3);
                if (Tplus.Real < 0)
                {
                    E1 *= -Complex.One;
                }
            }
            else
            {
                E1 = cubicRoot(Tplus);
            }

            // v
            Complex Tminus = -A - C;
            Complex E2;
            if (Tminus.Imaginary == 0)
            {
                var u3 = new Complex(Math.Abs(Tminus.Real), 0);
                E2 = cubicRoot(u3);
                if (Tminus.Real < 0)
                {
                    E2 *= -Complex.One;
                }
            }
            else
            {
                E2 = cubicRoot(Tminus);
            }

            // 3次式の解生成
            var x = new Complex[3];

            // x = (u + v) - A/3
            x[0] = E1 + E2 - D;

            Complex W1 = (-Complex.One + (Math.Sqrt(3.0) * Complex.One) * Complex.ImaginaryOne) / (2 * Complex.One);
            Complex W2 = (-Complex.One - (Math.Sqrt(3.0) * Complex.One) * Complex.ImaginaryOne) / (2 * Complex.One);

            x[1] = W1 * E1 + W2 * E2 - D;
            x[2] = W2 * E1 + W1 * E2 - D;

            return x;
        }

        /// <summary>
        /// "A=GW"を満たす行列式の回帰係数Wを計算する。
        /// </summary>
        /// <param name="response">目的変数A</param>
        /// <param name="explanatory">説明変数G</param>
        /// <returns>回帰係数W</returns>
        public static double[] SolveMatrix(double[] response, List<double[]> explanatory)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            if (explanatory == null)
            {
                throw new ArgumentNullException(nameof(explanatory));
            }

            // 特異値分解で行列式を解く
            double[,] matrix = new double[explanatory.Count, explanatory[0].Length];
            for (int row = 0; row < explanatory.Count; row++)
            {
                for (int column = 0; column < explanatory[row].Length; column++)
                {
                    matrix[row, column] = explanatory[row][column];
                }
            }

            // 擬似逆行列と目的変数から回帰係数を算出する。
            var matrixG = Mat.FromArray(matrix);
            using (var svd = new SVD(matrixG))
            {
                var vectorA = Mat.FromPixelData(response.Length, 1, MatType.CV_64FC1, response);
                var vectorW = new Mat();
                svd.BackSubst(vectorA, vectorW);
                vectorW.GetArray(out double[] vector);

                return vector;
            }
        }

        /// <summary>
        /// Zernike多項式のX偏微分導関数による計算結果を出力します。
        /// </summary>
        /// <param name="point">スポット誤差値</param>
        /// <param name="zernikeIndex">FringeZernike指数(1～36)</param>
        /// <returns></returns>
        /// <remarks>[Index] = (指数 - 1)</remarks>
        public static double[] ZernikeDerivativeX(PointD point, int zernikeIndex = 36)
        {
            if (zernikeIndex < 1)
            {
                return Array.Empty<double>();
            }

            // 指数計算前処理
            double[] x = new double[11];
            double[] y = new double[11];
            x[0] = 1;
            y[0] = 1;
            for (int index = 1; index < x.Length; index++)
            {
                x[index] = x[index - 1] * point.X;
                y[index] = y[index - 1] * point.Y;
            }

            double[] results = new double[((zernikeIndex > zernikeDerivativeXTable.Count) ? zernikeDerivativeXTable.Count : zernikeIndex)];
            for (int index = 0; index < results.Length; index++)
            {
                results[index] = zernikeDerivativeXTable[(index + 1)](x, y);
            }

            return results;
        }

        /// <summary>
        /// Zernike多項式のY偏微分導関数による計算結果を出力します。
        /// </summary>
        /// <param name="point">評価値</param>
        /// <param name="zernikeIndex">FringeZernike指数(1～36)</param>
        /// <returns></returns>
        /// <remarks>[Index] = (指数 - 1)</remarks>
        public static double[] ZernikeDerivativeY(PointD point, int zernikeIndex = 36)
        {
            if (zernikeIndex < 1)
            {
                return Array.Empty<double>();
            }

            // 指数計算前処理
            double[] x = new double[11];
            double[] y = new double[11];
            x[0] = 1;
            y[0] = 1;
            for (int index = 1; index < x.Length; index++)
            {
                x[index] = x[index - 1] * point.X;
                y[index] = y[index - 1] * point.Y;
            }

            double[] results = new double[((zernikeIndex > zernikeDerivativeYTable.Count) ? zernikeDerivativeYTable.Count : zernikeIndex)];
            for (int index = 0; index < results.Length; index++)
            {
                results[index] = zernikeDerivativeYTable[(index + 1)](x, y);
            }

            return results;
        }

        /// <summary>
        /// 指定されたZernike係数項までのZernike多項式結果を出力します。
        /// </summary>
        /// <param name="point">座標</param>
        /// <param name="zernikeIndex">Zernike係数項（フリンジ指数：1～36）</param>
        /// <returns>Zernike多項式係数。</returns>
        /// <remarks>[0]は常に0を返します。</remarks>
        public static double[] ZernikePolynomials(PointD point, int zernikeIndex)
        {
            // 前計算
            double rho = point.Distance;
            double rho2 = rho * rho;
            double angle = Math.Atan2(point.Y, point.X);

            double[] zernike = new double[zernikeIndex + 1];
            double[,] radialPolynomials = new double[11, 11];
            for (int index = 1; index <= zernikeIndex; index++)
            {
                int mAbs = Math.Abs(fringeZernikeIndex[index].m);
                double R;
                if (radialPolynomials[fringeZernikeIndex[index].n, mAbs] == 0)
                {
                    if (fringeZernikeIndex[index].n == mAbs)
                    {
                        R = 1;
                        for (int count = 0; count < mAbs; count++)
                        {
                            R *= rho;
                        }
                    }
                    else if (fringeZernikeIndex[index].n == (mAbs + 2))
                    {
                        R = ((mAbs + 2) * rho2) - (mAbs + 1);
                        for (int count = 0; count < mAbs; count++)
                        {
                            R *= rho;
                        }
                    }
                    else
                    {
                        int n = fringeZernikeIndex[index].n;
                        int m = mAbs;
                        double R2 = 2 * (n - 1) * ((2 * n * (n - 2) * rho * rho) - (m * m) - (n * (n - 2))) * radialPolynomials[(n - 2), m];
                        double R4 = n * (n + m - 2) * (n - m - 2) * radialPolynomials[(n - 4), m];
                        R = (R2 - R4) / ((n + m) * (n - m) * (n - 2));
                    }

                    radialPolynomials[fringeZernikeIndex[index].n, mAbs] = R;
                }
                else
                {
                    R = radialPolynomials[fringeZernikeIndex[index].n, mAbs];
                }

                double z = R * ((fringeZernikeIndex[index].m >= 0) ? Math.Cos(mAbs * angle) : Math.Sin(mAbs * angle));
                zernike[index] = z;
            }

            return zernike;
        }
    }
}
