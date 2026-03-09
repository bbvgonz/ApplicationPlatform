using Optical.Platform.Types;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Optical.API.Wavefront
{
    /// <summary>
    /// 校正情報設定
    /// </summary>
    [DataContract]
    public class CalibrationComponents : IExtensibleDataObject
    {
        #region Constructors
        /// <summary>
        /// <see cref="CalibrationComponents"/>クラスの新しいインスタンスを生成します。
        /// </summary>
        public CalibrationComponents()
        {
            SensorSize = new Size<int>();
            ReferencePoints = new List<PointD>();
        }
        #endregion // Constructors

        #region Properties
        /// <summary>
        /// 新しいメンバーの追加によって拡張された、バージョン付きのデータ コントラクトのデータを格納します。
        /// </summary>
        [Browsable(false)]
        public ExtensionDataObject ExtensionData { get; set; }

        /// <summary>
        /// デバイス識別子
        /// </summary>
        [Category("Calibration")]
        [DataMember]
        public string DeviceId { get; set; }

        /// <summary>
        /// マイクロレンズアレイの焦点距離[um]
        /// </summary>
        [Category("Calibration")]
        [DataMember]
        public double MicroLensArrayFocalLength { get; set; }

        /// <summary>
        /// マイクロレンズアレイのレンズ間距離[um]
        /// </summary>
        [Category("Calibration")]
        [DataMember]
        public double MicroLensArrayPitch { get; set; }

        /// <summary>
        /// 光学倍率
        /// </summary>
        [Category("Calibration")]
        [DataMember]
        public double OpticalMagnification { get; set; }

        /// <summary>
        /// センサーカメラのピクセルサイズ[um]
        /// </summary>
        [Category("Calibration")]
        [DataMember]
        public double PixelPitch { get; set; }

        /// <summary>
        /// 光源伝播距離[um]
        /// </summary>
        /// <remarks>伝播距離が0以下の場合、平面波で校正されたとみなします。</remarks>
        [Category("Calibration")]
        [DataMember]
        public double PropagationDistance { get; set; }

        /// <summary>
        /// 基準点座標リスト
        /// </summary>
        /// <remarks>入力画像の左上を原点とした座標系。</remarks>
        [Category("Calibration")]
        [DataMember]
        public List<PointD> ReferencePoints { get; set; }

        /// <summary>
        /// イメージセンサーの画面解像度[pixel]を取得します。
        /// </summary>
        [Category("Calibration")]
        [DataMember]
        public Size<int> SensorSize { get; set; }

        /// <summary>
        /// システム識別子
        /// </summary>
        [Category("Calibration")]
        [DataMember]
        public string SystemId { get; set; }

        /// <summary>
        /// 光源波長[um]
        /// </summary>
        [Category("Calibration")]
        [DataMember]
        public double Wavelength { get; set; }
        #endregion // Properties
    }
}
