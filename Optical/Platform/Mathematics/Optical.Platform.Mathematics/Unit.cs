using System;

namespace Optical.Platform.Mathematics
{
    /// <summary>
    /// 単位変換クラス
    /// </summary>
    public static class Unit
    {
        /// <summary>
        /// "%" を "割合" に変換します。
        /// </summary>
        /// <typeparam name="TNumeric">数値型</typeparam>
        /// <param name="percent">パーセンテージ</param>
        /// <returns>割合</returns>
        public static double ToRate<TNumeric>(TNumeric percent) where TNumeric : struct
        {
            return ((dynamic)percent / 100);
        }

        /// <summary>
        /// "割合" を "%" に変換します。
        /// </summary>
        /// <typeparam name="TNumeric">数値型</typeparam>
        /// <param name="rate">割合</param>
        /// <returns>パーセンテージ</returns>
        public static double ToPercent<TNumeric>(TNumeric rate) where TNumeric : struct
        {
            return ((dynamic)rate * 100);
        }

        /// <summary>
        /// "マイクロ" を "ミリ" に変換します。
        /// </summary>
        /// <typeparam name="TNumeric">数値型</typeparam>
        /// <param name="micro">マイクロ</param>
        /// <returns>ミリ</returns>
        public static double MicroToMili<TNumeric>(TNumeric micro) where TNumeric : struct
        {
            return ((dynamic)micro / 1000);
        }

        /// <summary>
        /// "ミリ" を "マイクロ" に変換します。
        /// </summary>
        /// <typeparam name="TNumeric">数値型</typeparam>
        /// <param name="mili">ミリ</param>
        /// <returns>マイクロ</returns>
        public static double MiliToMicro<TNumeric>(TNumeric mili) where TNumeric : struct
        {
            return ((dynamic)mili * 1000);
        }

        /// <summary>
        /// "ミリ" を "標準単位" に変換します。
        /// </summary>
        /// <typeparam name="TNumeric">数値型</typeparam>
        /// <param name="mili">ミリ</param>
        /// <returns>標準単位</returns>
        public static double MiliToStandard<TNumeric>(TNumeric mili) where TNumeric : struct
        {
            return ((dynamic)mili / 1000);
        }

        /// <summary>
        /// "標準単位" を "ミリ" に変換します。
        /// </summary>
        /// <typeparam name="TNumeric">数値型</typeparam>
        /// <param name="Standard">標準単位</param>
        /// <returns>ミリ</returns>
        public static double StandardToMili<TNumeric>(TNumeric Standard) where TNumeric : struct
        {
            return ((dynamic)Standard * 1000);
        }

        /// <summary>
        /// "ラジアン" を "度" に変換します。
        /// </summary>
        /// <typeparam name="TNumeric">数値型</typeparam>
        /// <param name="radian">ラジアン[rad]</param>
        /// <returns>度[°]</returns>
        public static double ToDegree<TNumeric>(TNumeric radian) where TNumeric : struct
        {
            return ((dynamic)radian * 180 / Math.PI);
        }

        /// <summary>
        /// "度" を "ラジアン" に変換します。
        /// </summary>
        /// <typeparam name="TNumeric">数値型</typeparam>
        /// <param name="degree">度[°]</param>
        /// <returns>ラジアン[rad]</returns>
        public static double ToRadian<TNumeric>(TNumeric degree) where TNumeric : struct
        {
            return ((dynamic)degree * Math.PI / 180);
        }
    }
}
