namespace Optical.Platform.Types
{
    /// <summary>
    /// 1次関数係数
    /// </summary>
    /// <typeparam name="TNumeric">数値型</typeparam>
    public class LinearFunction<TNumeric> where TNumeric : struct
    {
        /// <summary>
        /// 傾き
        /// </summary>
        public TNumeric Slope { get; set; }
        /// <summary>
        /// 切片
        /// </summary>
        public TNumeric Intercept { get; set; }
    }
}
