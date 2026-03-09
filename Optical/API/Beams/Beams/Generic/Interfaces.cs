using Optical.API.Library.Device;
using Optical.API.Library.Optics;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Optical.API.Beams
{
    /// <summary>
    /// ビームプロファイルを測定するためのインターフェースを提供します。
    /// </summary>
	public interface IBeamProfiler : ICameraConfig
    {
        #region Events
        /// <summary>
        /// 計算結果が更新された場合にイベントが発生します。
        /// </summary>
        event EventHandler<int> NewResult;
        #endregion // Events

        #region Properties
        /// <summary>
        /// ビーム重心の算出方法を取得・設定します。
        /// </summary>
        /// <remarks><see cref="AutoApertureEnabled"/>が<see langword="true"/>の場合のみ有効。</remarks>
        CentroidMethod AutoApertureCalculationType { get; set; }
        /// <summary>
        /// スポット開口の自動計算可否を示す値を取得・設定します。
        /// </summary>
        bool AutoApertureEnabled { get; set; }
        /// <summary>
        /// スポット開口の自動計算方式を取得・設定します。
        /// </summary>
        BeamwidthType AutoApertureMethod { get; set; }
        /// <summary>
        /// スポット開口の自動計算結果に対する倍率を取得・設定します。
        /// </summary>
        double AutoApertureRate { get; set; }
        /// <summary>
        /// 内部バッファの参照先インデックスを取得・設定します。
        /// </summary>
        int BufferIndex { get; set; }
        /// <summary>
        /// 測定用の内部バッファサイズを取得・設定します。
        /// </summary>
        /// <remarks>初期サイズ：1
        /// <para>測定停止中のみ変更可能です。</para></remarks>
        int BufferSize { get; set; }
        /// <summary>
        /// Centroid の算出方法を取得・設定します。
        /// </summary>
        CentroidMethod CentroidType { get; set; }
        /// <summary>
        /// 測定バッファの参照を取得します。
        /// </summary>
        BeamProfileContainer CurrentBuffer { get; }
        /// <summary>
        /// スポット径を自動計算するかどうかを取得・設定します。
        /// </summary>
        /// <remarks><see lang="true"/>のときは画像取得後にスポット計算を行い、<see lang="false"/>のときには画像取得後にスポット計算を行いません。 </remarks>
        bool EnableCalculation { get; set; }
        /// <summary>
        /// 計算をするスポット径の種類を取得・設定します。
        /// </summary>
        BeamwidthType EnableDiameter { get; set; }
        /// <summary>
        /// デバイスが校正済みかどうかを確認します。
        /// </summary>
        bool IsCalibrated { get; }
        /// <summary>
        /// デバイスが測定中かどうかを確認します。
        /// </summary>
        bool IsMeasuring { get; }
        /// <summary>
        /// デバイスが使用可能な状態かどうかを確認します。
        /// </summary>
        bool IsOpened { get; }
        /// <summary>
        /// 楕円ビームの向きを考慮するかどうかを示す値を取得・設定します。
        /// </summary>
        bool Orientation { get; set; }
        /// <summary>
        /// 対象領域の範囲[pixel]を取得・設定します。
        /// </summary>
        List<Rectangle> RoiList { get; }
        /// <summary>
        /// 対象領域の開口形状を取得・設定します。
        /// </summary>
        ApertureShape RoiShape { get; set; }
        #endregion // Properties

        #region Indexers
        /// <summary>
        /// 測定結果バッファ参照用インデクサー
        /// </summary>
        /// <param name="index">バッファインデックス</param>
        /// <returns>指定された測定結果のバッファデータ</returns>
        BeamProfileContainer this[int index] { get; }
        #endregion // Indexers

        #region Methods
        /// <summary>
        /// センサーデバイス一覧を列挙します。
        /// </summary>
        /// <returns>センサーデバイス一覧</returns>
        IReadOnlyList<SensorComponents> EnumerateDevice();
        /// <summary>
        /// デバイスの使用を停止します。
        /// </summary>
		void Close();
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
        /// ソフトウェアトリガーによる画像キャプチャー処理を行います。
        /// </summary>
        /// <remarks><see cref="ICameraConfig.TriggerMode"/>が<see langword="true"/>の場合のみ動作可能です。</remarks>
        void TakeSnapshot();
        #endregion // Methods
    }
}
