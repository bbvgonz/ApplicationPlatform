namespace Optical.Core.Camera
{
    #region Enums
    /// <summary>
    /// 画像取得動作形式
    /// </summary>
    internal enum GrabOperation
    {
        /// <summary>
        /// フリーラン
        /// </summary>
        FreeRun,
        /// <summary>
        /// ハードウェアトリガー
        /// </summary>
        HardwareTrigger,
        /// <summary>
        /// ソフトウェアトリガー
        /// </summary>
        SoftwareTrigger,
        /// <summary>
        /// 不明
        /// </summary>
        Unknown
    }

    /// <summary>
    /// カメラ製造元種別
    /// </summary>
    public enum VendorIdentifier
    {
        /// <summary>
        /// Sample
        /// </summary>
        Sample,
    }

    /// <summary>
    /// トリガーパルス検出方法
    /// </summary>
    public enum TriggerEdgeDetection
    {
        /// <summary>
        /// 立ち下がり検出
        /// </summary>
        Falling,
        /// <summary>
        /// 立ち上がり検出
        /// </summary>
        Rising
    }
    #endregion // Enums
}
