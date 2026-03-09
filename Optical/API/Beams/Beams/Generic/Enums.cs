using System;

namespace Optical.API.Beams
{
    /// <summary>
    /// ビーム幅の定義
    /// </summary>
    [Flags]
    public enum BeamwidthType
    {
        /// <summary>
        /// 2次モーメント幅（D4σ）
        /// </summary>
        D4Sigma = 0x0001,
        /// <summary>
        /// トータルビームパワーの86%幅
        /// </summary>
        D86 = 0x0002,
        /// <summary>
        /// 半値全幅（Full width at half maximum）
        /// </summary>
        FWHM = 0x0004,
        /// <summary>
        /// ナイフエッジ幅
        /// </summary>
        KnifeEdge = 0x0008,
        /// <summary>
        /// 1/e^2幅
        /// </summary>
        ReciprocalNapierSquared = 0x0010,
    }
}
