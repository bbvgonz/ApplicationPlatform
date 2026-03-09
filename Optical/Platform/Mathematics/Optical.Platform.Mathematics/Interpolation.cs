using Optical.Platform.Types;
using System;
using System.Collections.Generic;

namespace Optical.Platform.Mathematics
{
    /// <summary>
    /// 補間クラス
    /// </summary>
    public static class Interpolation
    {
        /// <summary>
        /// 3次スプライン補間係数を計算する。
        /// </summary>
        /// <param name="profile">2次元プロファイル。</param>
        /// <returns>3次スプライン補間係数</returns>
        public static List<SplineCoefficient> CubicSplineCurve(double[] profile)
        {
            // 3次多項式の定数項(d)を設定
            var coefficients = new List<SplineCoefficient>(profile.Length);
            for (int index = 0; index < profile.Length; index++)
            {
                coefficients.Add(new SplineCoefficient(profile[index]));
            }

            // 3次多項式の2次係数(b)を計算
            // 連立方程式を解く。
            // 但し、一般解法でなくスプライン計算に最適化した方法
            int curveCount = profile.Length - 1;
            coefficients[0].b = coefficients[curveCount].b = 0.0;
            for (int index = 1; index < curveCount; index++)
            {
                coefficients[index].b = 3.0 * (coefficients[index - 1].d - 2.0 * coefficients[index].d + coefficients[index + 1].d);
            }

            // 行列式の左下を消す
            // 回帰計算用係数
            double[] w = new double[profile.Length + 1];
            for (int index = 1; index < curveCount; index++)
            {
                double tmp = 4.0 - w[index - 1];
                if (tmp != 0)
                {
                    coefficients[index].b = (coefficients[index].b - coefficients[index - 1].b) / tmp;
                    w[index] = 1.0 / tmp;
                }
                else
                {
                    return [];
                }
            }

            // 行列式の右上を消す
            for (int index = curveCount - 1; index > 0; index--)
            {
                coefficients[index].b = coefficients[index].b - coefficients[index + 1].b * w[index];
            }

            // 3次多項式の1次係数(a)と3次係数(c)を計算
            for (int index = 0; index < curveCount; index++)
            {
                coefficients[index].a = (coefficients[index + 1].b - coefficients[index].b) / 3.0;
                coefficients[index].c = coefficients[index + 1].d - coefficients[index].d - coefficients[index].b - coefficients[index].a;
            }

            return coefficients;
        }

        /// <summary>
        /// 3次スプライン曲線係数。
        /// </summary>
        public class SplineCoefficient
        {
            // 数式の係数表現に合わせて、アルファベット小文字で定義する。

            /// <summary>
            /// 
            /// </summary>
            /// <param name="constant">定数項</param>
            public SplineCoefficient(double constant)
            {
                d = constant;
            }

            /// <summary>
            /// 3次係数
            /// </summary>
            public double a { get; set; }
            /// <summary>
            /// 2次係数
            /// </summary>
            public double b { get; set; }
            /// <summary>
            /// 1次係数
            /// </summary>
            public double c { get; set; }
            /// <summary>
            /// 定数項
            /// </summary>
            public double d { get; set; }
        };

        /// <summary>
        /// 指定された補正用の座標を使って、補正対象の座標をバイリニア法で補間します。
        /// </summary>
        /// <param name="correctionPairs">補正用の座標と角度のペア(4点必要)</param>
        /// <param name="targetPoint">補正対象の座標</param>
        /// <returns>補正後の角度</returns>
        public static AngleD Bilinear(List<CorrectionPair<AngleD>> correctionPairs, PointD targetPoint)
        {
            #region 引数の異常チェック
            ArgumentNullException.ThrowIfNull(correctionPairs);

            ArgumentNullException.ThrowIfNull(targetPoint);

            if (correctionPairs.Count < 4)
            {
                throw new ArgumentOutOfRangeException(nameof(correctionPairs), correctionPairs.Count, "要素数が不足しています。");
            }
            #endregion // 引数の異常チェック

            #region X方向計算
            double topWidth = correctionPairs[1].Reference.X - correctionPairs[0].Reference.X;
            double bottomWidth = correctionPairs[3].Reference.X - correctionPairs[2].Reference.X;

            double topAngle = correctionPairs[0].Target.X * (correctionPairs[1].Reference.X - targetPoint.X) / topWidth
                            + correctionPairs[1].Target.X * (targetPoint.X - correctionPairs[0].Reference.X) / topWidth;
            double bottomAngle = correctionPairs[2].Target.X * (correctionPairs[3].Reference.X - targetPoint.X) / bottomWidth
                               + correctionPairs[3].Target.X * (targetPoint.X - correctionPairs[2].Reference.X) / bottomWidth;

            // 上辺の傾きと切片から上辺とtargetPointの交点のY座標を算出
            double topSlope = (correctionPairs[1].Reference.Y - correctionPairs[0].Reference.Y) / (topWidth);
            double topIntercept = correctionPairs[0].Reference.Y - (topSlope * correctionPairs[0].Reference.X);
            double topIntersectionY = (topSlope * targetPoint.X) + topIntercept;

            // 下辺の傾きと切片から下辺とtargetPointの交点のY座標を算出
            double bottomSlope = (correctionPairs[3].Reference.Y - correctionPairs[2].Reference.Y) / bottomWidth;
            double bottomIntercept = correctionPairs[2].Reference.Y - (bottomSlope * correctionPairs[2].Reference.X);
            double bottomIntersectionY = (bottomSlope * targetPoint.X) + bottomIntercept;

            // tragetPointから上辺・下辺までの比率より角度Xを算出
            double ratioTargetToTop = (topIntersectionY - targetPoint.Y) / (topIntersectionY - bottomIntersectionY);
            double ratioTargetToBottom = (targetPoint.Y - bottomIntersectionY) / (topIntersectionY - bottomIntersectionY);
            double angleX = (topAngle * (1 - ratioTargetToBottom)) + (bottomAngle * (1 - ratioTargetToTop));
            #endregion // X方向計算

            #region Y方向計算
            double leftHeight = correctionPairs[0].Reference.Y - correctionPairs[2].Reference.Y;
            double leftAngle = correctionPairs[2].Target.Y * (correctionPairs[0].Reference.Y - targetPoint.Y) / leftHeight
                             + correctionPairs[0].Target.Y * (targetPoint.Y - correctionPairs[2].Reference.Y) / leftHeight;

            double rightHeight = correctionPairs[1].Reference.Y - correctionPairs[3].Reference.Y;
            double rightAngle = correctionPairs[3].Target.Y * (correctionPairs[1].Reference.Y - targetPoint.Y) / rightHeight
                              + correctionPairs[1].Target.Y * (targetPoint.Y - correctionPairs[3].Reference.Y) / rightHeight;

            double leftIntersectionX;
            if ((correctionPairs[0].Reference.X - correctionPairs[2].Reference.X) == 0)
            {
                // 左辺の傾きが0の場合、correctionPairs[0]と等しくなる (分母が0になるため別計算)
                leftIntersectionX = correctionPairs[0].Reference.X;
            }
            else
            {
                // 左辺の傾きと切片から左辺とtargetPointの交点のX座標を算出
                double leftSlope = leftHeight / (correctionPairs[0].Reference.X - correctionPairs[2].Reference.X);
                double leftIntercept = correctionPairs[0].Reference.Y - (leftSlope * correctionPairs[0].Reference.X);
                leftIntersectionX = (leftIntercept - targetPoint.Y) / leftSlope;
            }

            // 右辺交点のX座標を算出
            double rightIntersectionX;
            if ((correctionPairs[1].Reference.X - correctionPairs[3].Reference.X) == 0)
            {
                // 右辺の傾きが0の場合、correctionPairs[1]と等しくなる (分母が0になるため別計算)
                rightIntersectionX = correctionPairs[1].Reference.X;
            }
            else
            {
                // 右辺の傾きと切片から右辺とtargetPointの交点のX座標を算出
                double rightSlope = rightHeight / (correctionPairs[1].Reference.X - correctionPairs[3].Reference.X);
                double rightIntercept = correctionPairs[1].Reference.Y - (rightSlope * correctionPairs[1].Reference.X);
                rightIntersectionX = (rightIntercept - targetPoint.Y) / rightSlope;
            }

            // targetPointから左辺交点・右辺交点までの距離の比率を算出
            double raitoTargetToLeft = (targetPoint.Y - leftIntersectionX) / (rightIntersectionX - leftIntersectionX);
            double raitoTargetToRight = (rightIntersectionX - targetPoint.Y) / (rightIntersectionX - leftIntersectionX);

            // 校正後の角度Xを算出
            double angleY = (leftAngle * (1 - raitoTargetToRight)) + (rightAngle * (1 - raitoTargetToLeft));
            #endregion // Y方向計算

            return new AngleD(angleX, angleY);
        }
    }
}
