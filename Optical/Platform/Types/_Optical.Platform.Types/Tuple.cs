using System;

namespace Optical.Platform.Types
{
    /// <summary>
    /// 基準座標と測定値の関係を表すクラス。
    /// </summary>
    /// <typeparam name="T"><see langword="new"/>が可能な型</typeparam>
    public class CorrectionPair<T> where T : new()
    {
        #region Constructors
        /// <summary>
        /// <see cref="CorrectionPair{T}"/>クラスの新しいインスタンスを生成します。
        /// </summary>
        public CorrectionPair()
        {
            Reference = new PointD();
            Target = new T();
        }

        /// <summary>
        /// 指定された基準座標と測定値で、<see cref="CorrectionPair{T}"/>クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="reference">基準座標</param>
        /// <param name="target">対象値</param>
        public CorrectionPair(PointD reference, T target)
        {
            if ((reference == null) || (target == null))
            {
                throw new ArgumentNullException();
            }

            Reference = reference;
            Target = target;
        }
        #endregion // Constructors

        #region Properties
        /// <summary>
        /// 基準座標[pixel] (※左上原点)
        /// </summary>
        public PointD Reference { get; set; }

        /// <summary>
        /// 比較対象の測定値。
        /// </summary>
        public T Target { get; set; }
        #endregion // Properties
    }
}
