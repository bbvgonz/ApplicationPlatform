using Optical.Platform.Types;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Optical.API.Autocollimator
{
    /// <summary>
    /// 角度測定結果に関する各種情報を格納します。
    /// </summary>
    public class TiltContainer : IDisposable
    {
        #region Fields
        private ManualResetEventSlim completedEvent;
        private List<TiltSpotContainer> tiltList;
        #endregion // Fields

        #region Constructors
        /// <summary>
        /// 角度の測定結果を格納するための新しいインスタンスを生成します。
        /// </summary>
        public TiltContainer()
        {
            completedEvent = new ManualResetEventSlim(false);
            Image = new ImageContainer();
            tiltList = new List<TiltSpotContainer>();

            Image.Updated += Image_Updated;
        }
        #endregion // Constructors

        #region Properties
        /// <summary>
        /// 角度の計算が完了しているかどうかを示す値を取得します。
        /// </summary>
        /// <remarks>true:完了 / false:未完了</remarks>
        public bool IsCompleted => completedEvent.IsSet;

        /// <summary>
        /// オートコリメーター画像情報。
        /// </summary>
        public ImageContainer Image { get; }

        /// <summary>
        /// 角度情報リスト。
        /// </summary>
        public IReadOnlyList<TiltSpotContainer> Tilts => tiltList.AsReadOnly();
        #endregion

        #region Methods
        private void Image_Updated(object? sender, string e)
        {
            completedEvent.Reset();
            tiltList.Clear();
        }

        internal void UpdateTiltList(List<TiltSpotContainer> tilts)
        {
            tiltList = tilts;
            completedEvent.Set();
        }

        /// <summary>
        /// 格納されている情報を削除します。
        /// </summary>
        public void Clear()
        {
            completedEvent.Reset();
            tiltList.Clear();
        }

        /// <summary>
        /// 32ビット符号付き整数を使用して時間間隔を計測し、角度の計算が完了するまで、現在のスレッドをブロックします。
        /// </summary>
        /// <param name="millisecondsTimeout">待機するミリ秒数。無制限に待機する場合は <see cref="Timeout.Infinite"/>。</param>
        /// <returns>角度の計算が完了した場合は <see lang="true"/>。 それ以外の場合は <see lang="false"/>。</returns>
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
}
