using Optical.Platform.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Optical.Platform.Mathematics
{
    /// <summary>
    /// 近似式クラス
    /// </summary>
    public static class Approximation
    {
        /// <summary>
        /// ガウスの消去法
        /// </summary>
        /// <param name="augmentedMatrix">拡大係数行列</param>
        /// <returns>近似多項式係数列([n] = n次の項)</returns>
        private static double[] GaussianElimination(double[,] augmentedMatrix)
        {
            // 枢軸選択
            for (int i = 0; i < augmentedMatrix.GetLength(0); i++)
            {
                double m = 0;
                int pivot = i;

                for (int l = i; l < augmentedMatrix.GetLength(0); l++)
                {
                    if (Math.Abs(augmentedMatrix[l, i]) > m)
                    {
                        m = Math.Abs(augmentedMatrix[l, i]);
                        pivot = l;
                    }
                }

                if (pivot != i)
                {
                    for (int j = 0; j < augmentedMatrix.GetLength(1); j++)
                    {
                        double temp = augmentedMatrix[i, j];
                        augmentedMatrix[i, j] = augmentedMatrix[pivot, j];
                        augmentedMatrix[pivot, j] = temp;
                    }
                }
            }

            // 前進消去
            for (int k = 0; k < augmentedMatrix.GetLength(0); k++)
            {
                double p = augmentedMatrix[k, k];
                augmentedMatrix[k, k] = 1;

                for (int j = k + 1; j < augmentedMatrix.GetLength(1); j++)
                {
                    augmentedMatrix[k, j] /= p;
                }

                for (int i = k + 1; i < augmentedMatrix.GetLength(0); i++)
                {
                    double q = augmentedMatrix[i, k];

                    for (int j = k + 1; j < augmentedMatrix.GetLength(1); j++)
                    {
                        augmentedMatrix[i, j] -= q * augmentedMatrix[k, j];
                    }

                    augmentedMatrix[i, k] = 0;
                }
            }

            // 後退代入
            double[] x = new double[augmentedMatrix.GetLength(0)];
            for (int i = (augmentedMatrix.GetLength(0) - 1); i >= 0; i--)
            {
                x[i] = augmentedMatrix[i, augmentedMatrix.GetLength(0)];
                for (int j = (augmentedMatrix.GetLength(0) - 1); j > i; j--)
                {
                    x[i] -= augmentedMatrix[i, j] * x[j];
                }
            }

            // 係数解
            double[] coefficients = new double[augmentedMatrix.GetLength(0)];
            for (int degree = 0; degree < coefficients.Length; degree++)
            {
                coefficients[degree] = double.IsNaN(x[degree]) ? 0 : x[degree];
            }

            return coefficients;
        }

        /// <summary>
        /// 近似多項式
        /// </summary>
        /// <param name="points">近似対象座標列</param>
        /// <param name="degree">近似多項式次数</param>
        /// <returns>近似多項式係数列([n] = n次の項)</returns>
        /// <remarks>
        /// X軸方向の値は、平均値からの誤差を入力値とすること（※スケーリング：演算精度対策）
        /// </remarks>
        public static double[] Polynomial(List<PointD> points, uint degree)
        {
            uint unknownNumber = degree + 1;
            double[,] augmentedMatrix = new double[unknownNumber, unknownNumber + 1];

            for (int row = 0; row < augmentedMatrix.GetLength(0); row++)
            {
                for (int column = 0; column < (augmentedMatrix.GetLength(1) - 1); column++)
                {
                    for (int index = 0; index < points.Count; index++)
                    {
                        augmentedMatrix[row, column] += Math.Pow(points[index].X, (row + column));
                    }
                }
            }

            for (int row = 0; row < augmentedMatrix.GetLength(0); row++)
            {
                for (int index = 0; index < points.Count; index++)
                {
                    augmentedMatrix[row, augmentedMatrix.GetLength(0)] += Math.Pow(points[index].X, row) * points[index].Y;
                }
            }

            return GaussianElimination(augmentedMatrix);
        }

        /// <summary>
        /// 回帰直線（1次近似）
        /// </summary>
        /// <param name="points">近似対称座標列</param>
        /// <returns>回帰直線係数</returns>
        public static LinearFunction<double> RegressionLine(PointD[] points)
        {
            double xAverage = points.Average(point => point.X);
            double yAverage = points.Average(point => point.Y);

            // 分散
            double variance = points.Average(point => (point.X - xAverage) * (point.X - xAverage));

            // 共分散
            double covariance = points.Average(point => (point.X - xAverage) * (point.Y - yAverage));

            var linear = new LinearFunction<double>();
            linear.Slope = double.IsNaN(covariance / variance) ? 0 : (covariance / variance);
            linear.Intercept = (-1) * (linear.Slope * xAverage) + yAverage;

            return linear;
        }
    }
}
