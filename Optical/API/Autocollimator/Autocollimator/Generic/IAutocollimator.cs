using Optical.API.Library.Device;
using Optical.API.Library.Optics;
using Optical.Platform.Types;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Optical.API.Autocollimator
{
    /// <summary>
    /// オートコリメーターを制御するためのインターフェースを提供します。
    /// </summary>
    public interface IAutocollimator : ICameraConfig
    {
        #region Events
        /// <summary>
        /// 角度の測定結果が更新された場合に発生します。
        /// </summary>
        event EventHandler<int> NewResult;
        #endregion // Events

        #region Properties
        /// <summary>
        /// 移動平均の平均化回数を取得・設定します。
        /// </summary>
        int AveragingTimes { get; set; }
        /// <summary>
        /// 重心計算方法を取得・設定します。
        /// </summary>
        CentroidMethod CentroidType { get; set; }
        /// <summary>
        /// 内部光源を使用するかどうかを示す値を取得・設定します。
        /// </summary>
        /// <remarks>true:内部光源ON、false:内部光源OFF</remarks>
        bool InternalLightSource { get; set; }
        /// <summary>
        /// 最新の測定結果が格納されたバッファの参照を取得します。
        /// </summary>
        TiltContainer LatestBuffer { get; }
        /// <summary>
        /// 複数の光点を個別に測定するかどうかを示す値を取得・設定します。
        /// </summary>
        bool MultiMode { get; set; }
        /// <summary>
        /// 対象領域の範囲[pixel]を取得・設定します。
        /// </summary>
        /// <remarks>各対象領域に対し、<see cref="MultiMode"/>は適用されません。</remarks>
        List<Rectangle> Roi { get; set; }
        /// <summary>
        /// 最新の測定角度[deg]を取得します。
        /// </summary>
        /// <remarks>
        /// <para>測定された角度は、原点からの距離が近い順に格納されます。</para>
        /// <para>ただし、<see cref="Roi"/>が設定されている場合は、各<see cref="Roi"/>の測定結果が同じ順で格納されます。</para>
        /// </remarks>
        List<AngleD> Tilt { get; }
        #endregion // Properties

        #region Methods
        /// <summary>
        /// デバイスの使用を停止します。
        /// </summary>
        void Close();
        /// <summary>
        /// センサーデバイス一覧を列挙します。
        /// </summary>
        /// <returns>センサーデバイス一覧</returns>
        IReadOnlyList<SensorComponents> EnumerateDevice();
        /// <summary>
        /// デバイスの使用を開始します。
        /// </summary>
        void Open();
        /// <summary>
        /// デバイスを測定開始状態にします。
        /// </summary>
        void Start();
        /// <summary>
        /// デバイスの測定を停止します。
        /// </summary>
        void Stop();
        /// <summary>
        /// 校正ファイルを読み込みます。
        /// </summary>
        /// <param name="filePath">校正ファイルパス</param>
        void ReadCalibrationFile(string filePath);
        #endregion // Methods
    }
}