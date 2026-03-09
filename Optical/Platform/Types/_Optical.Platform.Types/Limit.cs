using System.ComponentModel;

namespace Optical.Platform.Types
{
    /// <summary>
    /// 境界値（最大値と最小値）のペアを格納します。
    /// </summary>
    /// <typeparam name="TNumeric">数値型</typeparam>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class Limit<TNumeric> where TNumeric : struct
    {
        #region Fields
        private static Limit<TNumeric> empty = new Limit<TNumeric>();
        #endregion // Fields

        #region Constructors
        /// <summary>
        /// <see cref="Limit{TNumeric}"/>クラスの新しいインスタンスを生成します。
        /// </summary>
        public Limit() { }

        /// <summary>
        /// 指定された境界値で、<see cref="Limit{TNumeric}"/>クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="maximum">最小値</param>
        /// <param name="minimum">最大値</param>
        public Limit(TNumeric maximum, TNumeric minimum)
        {
            Maximum = maximum;
            Minimum = minimum;
        }
        #endregion // Constructors

        #region Properties
        /// <summary>
        /// プロパティが初期化されていない状態の<see cref="Limit{TNumeric}"/>クラスを表します。
        /// </summary>
        public static Limit<TNumeric> Empty => empty;

        /// <summary>
        /// この<see cref="Limit{TNumeric}"/>が空かどうかを示す値を取得します。
        /// </summary>
        /// <remarks>この<see cref="Limit{TNumeric}"/>が<see cref="Empty"/>と等しい場合は<see langword="true"/>。それ以外の場合は<see langword="false"/>。</remarks>
        public bool IsEmpty => this == empty;

        /// <summary>
        /// 最大値
        /// </summary>
        public TNumeric Maximum { get; set; }

        /// <summary>
        /// 最小値
        /// </summary>
        public TNumeric Minimum { get; set; }

        /// <summary>
        /// 境界値幅
        /// </summary>
        public TNumeric Range => (dynamic)Maximum - Minimum;
        #endregion // Properties
    }
}
