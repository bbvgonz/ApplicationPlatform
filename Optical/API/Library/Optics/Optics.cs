using Optical.Platform.Types;
using System.Drawing;

namespace Optical.API.Library.Optics
{
    /// <summary>
    /// 開口形状
    /// </summary>
    public enum ApertureShape
    {
        /// <summary>
        /// 矩形
        /// </summary>
        Rectangle,
        /// <summary>
        /// 円形
        /// </summary>
        Circle
    }

    /// <summary>
    /// 重心計算種別
    /// </summary>
    public enum CentroidMethod
    {
        /// <summary>
        /// 面積重心
        /// </summary>
        Area,
        /// <summary>
        /// 輝度重心
        /// </summary>
        Luminance
    }

    /// <summary>
    /// ビームスポットの各種情報を格納します。
    /// </summary>
    public abstract class SpotContainer
    {
        #region Constructors
        /// <summary>
        /// <see cref="SpotContainer"/>クラスの新しいインスタンスを生成します。
        /// </summary>
        protected SpotContainer()
        {
            Centroid = new PointD();
            PeakPosition = new PointD();
            RoiImage = new ImageComponent();
        }
        #endregion

        #region Peoperties
        /// <summary>
        /// 最小輝度値
        /// </summary>
        public double Bottom { get; set; }

        /// <summary>
        /// スポット重心座標[pixel]
        /// </summary>
        public PointD Centroid { get; set; }

        /// <summary>
        /// 最大輝度値
        /// </summary>
        public double Peak { get; set; }

        /// <summary>
        /// 最大輝度座標[pixel]
        /// </summary>
        public PointD PeakPosition { get; set; }

        /// <summary>
        /// 生画像に対する対象領域範囲[pixel]
        /// </summary>
        public Rectangle Roi { get; set; }

        /// <summary>
        /// 対象領域内の画像データ
        /// </summary>
        public ImageComponent RoiImage { get; set; }

        /// <summary>
        /// 対象領域番号
        /// </summary>
        public int RoiNumber { get; set; }

        /// <summary>
        /// 対象領域形状
        /// </summary>
        public ApertureShape RoiShape { get; set; }

        /// <summary>
        /// 合計輝度値
        /// </summary>
        public double TotalCount { get; set; }
        #endregion // Peoperties
    }
}
