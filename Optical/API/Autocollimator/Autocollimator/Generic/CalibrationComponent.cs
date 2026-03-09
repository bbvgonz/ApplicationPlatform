using Optical.API.Library.Optics;
using Optical.Platform.Types;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Optical.API.Autocollimator
{
    /// <summary>
    /// 光源位置
    /// </summary>
    public enum LightSourcePosition
    {
        /// <summary>
        /// 内部光源
        /// </summary>
        Internal,
        /// <summary>
        /// 外部光源
        /// </summary>
        External
    }

    /// <summary>
    /// Autocollimatorの校正データクラス。
    /// </summary>
    [DataContract]
    public class CalibrationComponent
    {
        public CalibrationComponent()
        {
            DeviceId = string.Empty;
            PointMap = [];
            SensorSize = new();
        }

        /// <summary>
        /// 重心計算方法
        /// </summary>
        [DataMember]
        public CentroidMethod CentroidType { get; set; }
        /// <summary>
        /// デバイスID
        /// </summary>
        [DataMember]
        public string DeviceId { get; set; }
        /// <summary>
        /// 光源タイプ
        /// </summary>
        [DataMember]
        public LightSourcePosition LightSource { get; set; }
        /// <summary>
        /// 校正点マップ
        /// </summary>
        [DataMember]
        public List<CorrectionPair<AngleD>> PointMap { get; set; }
        /// <summary>
        /// 校正点マップ水方向点数
        /// </summary>
        [DataMember]
        public int PointMapHorizontalCount { get; set; }
        /// <summary>
        /// 校正点マップ垂直方向点数
        /// </summary>
        [DataMember]
        public int PointMapVerticalCount { get; set; }
        /// <summary>
        /// センサーサイズ[pixel]
        /// </summary>
        [DataMember]
        public Size<int> SensorSize { get; set; }
        /// <summary>
        /// 光源波長[um]
        /// </summary>
        [DataMember]
        public double WaveLength { get; set; }
    }
}
