using Optical.API.Library.Optics;
using Optical.Platform.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

namespace Optical.API.Beams
{
    /// <summary>
    /// ビームプロファイルの各種情報を格納します。
    /// </summary>
    public class BeamProfileContainer : IDisposable
    {
        #region Fields
        private readonly List<BeamSpotContainer> emptySpotList = new List<BeamSpotContainer>();

        private ManualResetEventSlim completedEvent;
        private List<BeamSpotContainer> spotList;
        #endregion

        #region Constructors
        /// <summary>
        /// ビームプロファイルの測定結果を格納するための新しいインスタンスを生成します。
        /// </summary>
        public BeamProfileContainer()
        {
            completedEvent = new ManualResetEventSlim(false);
            Image = new ImageContainer();
            spotList = new List<BeamSpotContainer>();

            Image.Updated += Image_Updated;
        }
        #endregion

        #region Properties
        private void Image_Updated(object sender, string e)
        {
            completedEvent.Reset();
            spotList.Clear();
        }

        /// <summary>
        /// スポット情報の計算が完了しているかどうかを示す値を取得します。
        /// </summary>
        /// <remarks>true:完了 / false:未完了</remarks>
        public bool IsCompleted => completedEvent.IsSet;

        /// <summary>
        /// ビームプロファイル画像情報。
        /// </summary>
        public ImageContainer Image { get; }

        /// <summary>
        /// スポット情報リスト。
        /// </summary>
        public IReadOnlyList<BeamSpotContainer> Spots => spotList.AsReadOnly();
        #endregion

        #region Methods
        internal void UpdateSpotList(List<BeamSpotContainer> spots)
        {
            spotList = spots;
            completedEvent.Set();
        }

        /// <summary>
        /// 格納されている情報を削除します。
        /// </summary>
        public void Clear()
        {
            completedEvent.Reset();
            Image.Clear();
            spotList = emptySpotList;
        }

        /// <summary>
        /// 32ビット符号付き整数を使用して時間間隔を計測し、スポット情報の計算が完了するまで、現在のスレッドをブロックします。
        /// </summary>
        /// <param name="millisecondsTimeout">待機するミリ秒数。無制限に待機する場合は <see cref="Timeout.Infinite"/>。</param>
        /// <returns>スポット情報の計算が完了した場合は <see lang="true"/>。 それ以外の場合は <see lang="false"/>。</returns>
        public bool WaitForCompletion(int millisecondsTimeout)
        {
            return completedEvent.Wait(millisecondsTimeout);
        }

        // Dispose自動生成パターン
        #region IDisposable Support
        private bool disposedValue = false; // 重複する呼び出しを検出するには

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: マネージ状態を破棄します (マネージ オブジェクト)。
                    completedEvent.Dispose();
                }

                // TODO: アンマネージ リソース (アンマネージ オブジェクト) を解放し、下のファイナライザーをオーバーライドします。
                // TODO: 大きなフィールドを null に設定します。

                disposedValue = true;
            }
        }

        // TODO: 上の Dispose(bool disposing) にアンマネージ リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします。
        // ~BeamProfileContainer() {
        //   // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
        //   Dispose(false);
        // }

        // このコードは、破棄可能なパターンを正しく実装できるように追加されました。
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
            Dispose(true);
            // TODO: 上のファイナライザーがオーバーライドされる場合は、次の行のコメントを解除してください。
            // GC.SuppressFinalize(this);
        }
        #endregion // IDisposable Support
        #endregion // Methods
    }

    /// <summary>
    /// ビームスポットの各種情報を格納します。
    /// </summary>
    public class BeamSpotContainer : SpotContainer
    {
        #region Constructors
        /// <summary>
        /// <see cref="BeamSpotContainer"/>クラスの新しいインスタンスを生成します。
        /// </summary>
        internal BeamSpotContainer()
        {
            D4Sigma = new PointD();
            RoiImage = new ImageComponent<double>();
        }
        #endregion

        #region Peoperties
        /// <summary>
        /// 自動アパーチャサイズ[pixel]
        /// </summary>
        /// <remarks><see cref="IBeamProfiler.AutoApertureRate"/>が適用された値が出力されます。</remarks>
        public Rectangle AutoAperture { get; internal set; }

        /// <summary>
        /// ビーム径（D4σ）[pixel]
        /// </summary>
        public PointD D4Sigma { get; internal set; }

        /// <summary>
        /// ビーム径（Encircled Energy:86.5%）[pixel]
        /// </summary>
        public double D86 { get; internal set; }

        /// <summary>
        /// 楕円ビームの回転角[deg]
        /// </summary>
        public double Orientation { get; internal set; }

        /// <summary>
        /// 対象領域内の画像データ
        /// </summary>
        public new ImageComponent<double> RoiImage { get; set; }
        #endregion // Peoperties
    }
}
