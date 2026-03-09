namespace Optical.API.Library.Device
{
    /// <summary>
    /// センサーデバイスの識別情報を共有します。
    /// </summary>
    public class SensorComponents
    {
        #region Constructors
        /// <summary>
        /// <see cref="SensorComponents"/>クラスの新しいインスタンスを生成します。
        /// </summary>
        public SensorComponents()
        {
            ConnectionType = string.Empty;
            DeviceID = string.Empty;
            Key = string.Empty;
            ModelName = string.Empty;
            SerialNumber = string.Empty;
            VendorName = string.Empty;
        }
        #endregion // Constructors

        #region Properties
        /// <summary>
        /// 接続形式
        /// </summary>
        public string ConnectionType { get; set; }

        /// <summary>
        /// デバイスの識別子
        /// </summary>
        public string DeviceID { get; set; }

        /// <summary>
        /// センサーデバイス識別用キー
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// モデル型式
        /// </summary>
        public string ModelName { get; set; }

        /// <summary>
        ///  シリアルナンバー
        /// </summary>
        public string SerialNumber { get; set; }

        /// <summary>
        /// ベンダー名称
        /// </summary>
        public string VendorName { get; set; }
        #endregion // Properties
    }
}
