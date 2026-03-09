using System.ComponentModel;

namespace Optical.Platform.Types
{
    /// <summary>
    /// 円座標
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class Circular
    {
        #region Constructors
        /// <summary>
        /// <see cref="Circular"/>クラスの新しいインスタンスを生成します。
        /// </summary>
        public Circular() { }

        /// <summary>
        /// <see cref="Circular"/>クラスの新しいインスタンスを生成します。
        /// </summary>
        /// <param name="rho">動径</param>
        /// <param name="theta">偏角</param>
        public Circular(double rho, double theta)
        {
            Rho = rho;
            Theta = theta;
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="source">コピー元の<see cref="Circular"/>クラスインスタンス</param>
        public Circular(Circular source)
        {
            Rho = source.Rho;
            Theta = source.Theta;
        }
        #endregion // Constructors

        #region Properties
        /// <summary>
        /// 動径
        /// </summary>
        public double Rho { get; set; }

        /// <summary>
        /// 偏角
        /// </summary>
        public double Theta { get; set; }
        #endregion // Properties

        #region Methods
        /// <summary>
        /// 円座標を初期化します。
        /// </summary>
        public void Clear()
        {
            Rho = 0;
            Theta = 0;
        }
        #endregion // Methods
    }
}
