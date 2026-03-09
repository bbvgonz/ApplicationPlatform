using Optical.Platform.Types;
using System;
using System.Collections.Generic;

namespace Optical.Platform.Mathematics
{
    /// <summary>
    /// ユークリッド空間クラス
    /// </summary>
    public static class Euclidean
    {
        /// <summary>
        /// 象限インデックスに対する座標の符号を取得する。
        /// </summary>
        /// <seealso cref="QuadrantIndex(PointD, PointD)"/>
        public static Dictionary<int, Point<int>> QuadrantSign => new Dictionary<int, Point<int>>
        {
            // 第2象限
            [0] = new Point<int>(-1, 1),
            // 第1象限
            [1] = new Point<int>(1, 1),
            // 第3象限
            [2] = new Point<int>(-1, -1),
            // 第4象限
            [3] = new Point<int>(1, -1)
        };

        /// <summary>
        /// 指定された中心座標と対象座標から象限を出力する。
        /// </summary>
        /// <param name="center">中心座標</param>
        /// <param name="target">対象座標</param>
        /// <returns>対象象限のインデックス。（0～3）</returns>
        /// <remarks>
        /// <para>0:第2象限, 1:第1象限, 2:第3象限, 3:第4象限</para>
        /// <para>(0, 0)は第一象限扱いとする。</para>
        /// </remarks>
        public static int QuadrantIndex(PointD center, PointD target)
        {
            int signX = Math.Sign(target.X - center.X);
            int signY = Math.Sign(target.Y - center.Y);

            if (signX == 0)
            {
                signX = 1;
            }

            if (signY == 0)
            {
                signY = 1;
            }

            return (3 + signX - (2 * signY)) / 2;
        }
    }
}
