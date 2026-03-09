using Optical.API.Library.Optics;
using Optical.Platform.Types;

namespace Optical.API.Autocollimator
{
    /// <summary>
    /// ビームスポットの各種情報を格納します。
    /// </summary>
    public class TiltSpotContainer : SpotContainer
    {
        #region Constructors
        /// <summary>
        /// <see cref="TiltSpotContainer"/>クラスの新しいインスタンスを生成します。
        /// </summary>
        internal TiltSpotContainer()
        {
            Tilt = new AngleD();
        }
        #endregion // Constructors

        #region Peoperties
        /// <summary>
        /// 角度測定結果 [deg]
        /// </summary>
        public AngleD Tilt { get; internal set; }
        #endregion // Peoperties
    }
}
