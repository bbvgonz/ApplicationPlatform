using Optical.API.Library.Optics;
using Optical.Platform.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Optical.API.Wavefront
{
    /// <summary>
    /// 波面の各種測定結果を格納します。
    /// </summary>
    public class WavefrontContainer : IDisposable
    {
        #region Fields
        private ManualResetEventSlim completedEvent;
        private double[] zernike;
        #endregion // Fields

        #region Constructors
        /// <summary>
        /// 波面の測定結果を格納するための新しいインスタンスを生成します。
        /// </summary>
        public WavefrontContainer()
        {
            completedEvent = new ManualResetEventSlim(false);
            Image = new ImageContainer();
            Legendre = Array.Empty<double>();
            Parameter = new WavefrontParameter();
            Seidel = new SeidelAberration();
            SpotPairs = new List<CorrectionPair<PointD>>();
            Total = new TotalAberration();
            zernike = Array.Empty<double>();

            Image.Updated += Image_Updated;
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="wavefront">コピー元の <see cref="WavefrontContainer"/> クラスインスタンス</param>
        public WavefrontContainer(WavefrontContainer wavefront)
        {
            completedEvent = new ManualResetEventSlim(wavefront.IsCompleted);

            Image = new ImageContainer(wavefront.Image);

            Legendre = new double[wavefront.Legendre.Length];
            wavefront.Legendre.CopyTo(Legendre, 0);

            SpotPairs = new List<CorrectionPair<PointD>>(wavefront.SpotPairs.Count);
            SpotPairs.AddRange(wavefront.SpotPairs.Select(spot => new CorrectionPair<PointD>() { Reference = new PointD(spot.Reference), Target = new PointD(spot.Target) }));

            Parameter = new WavefrontParameter();

            zernike = new double[wavefront.Zernike.Length];
            wavefront.Zernike.CopyTo(zernike, 0);

            Seidel = new SeidelAberration(zernike);

            Total = new TotalAberration(zernike, wavefront.Parameter.ApertureSize, wavefront.Parameter.Wavelength);

            Image.Updated += Image_Updated;
        }
        #endregion // Constructors

        #region Properties
        /// <summary>
        /// 波面の計算が完了しているかどうかを示す値を取得します。
        /// </summary>
        /// <remarks>true:完了 / false:未完了</remarks>
        public bool IsCompleted => completedEvent.IsSet;

        /// <summary>
        /// カメラ画像情報。
        /// </summary>
        public ImageContainer Image { get; }

        /// <summary>
        /// ルジャンドル多項式係数[PV]
        /// </summary>
        /// <remarks>Order:[(0, 0)],[(1, 0)],...,[(m, 0)],[(0, 1)],...,[(m, n)]</remarks>
        public double[] Legendre { get; internal set; }

        /// <summary>
        /// 波面収差計算時パラメーター
        /// </summary>
        public WavefrontParameter Parameter { get; internal set; }

        /// <summary>
        /// ザイデル収差
        /// </summary>
        public SeidelAberration Seidel { get; }

        /// <summary>
        /// 校正点と測定点のペアリング結果を取得します。
        /// </summary>
        public List<CorrectionPair<PointD>> SpotPairs { get; internal set; }

        /// <summary>
        /// 総合波面収差
        /// </summary>
        public TotalAberration Total { get; }

        /// <summary>
        /// フリンジ・ゼルニケ多項式係数[PV]
        /// </summary>
        /// <remarks>index:0はデータ無し。</remarks>
        public double[] Zernike
        {
            get => zernike;
            internal set
            {
                zernike = value;
                Seidel.Compute(value);
            }
        }
        #endregion // Properties

        #region Methods
        private void Image_Updated(object sender, string e)
        {
            completedEvent.Reset();
            Legendre = Array.Empty<double>();
            Seidel.Clear();
            SpotPairs.Clear();
            Total.Clear();
            zernike = Array.Empty<double>();
        }

        /// <summary>
        /// 格納されている情報を削除します。
        /// </summary>
        internal void Clear()
        {
            completedEvent.Reset();
            Image.Clear();
            Legendre = Array.Empty<double>();
            Parameter.Clear();
            Seidel.Clear();
            SpotPairs.Clear();
            Total.Clear();
            zernike = Array.Empty<double>();
        }

        /// <summary>
        /// 波面の計算を完了状態にします。
        /// </summary>
        internal void Complete()
        {
            completedEvent.Set();
        }

        /// <summary>
        /// 32ビット符号付き整数を使用して時間間隔を計測し、波面の計算が完了するまで、現在のスレッドをブロックします。
        /// </summary>
        /// <param name="millisecondsTimeout">待機するミリ秒数。無制限に待機する場合は <see cref="Timeout.Infinite"/>。</param>
        /// <returns>スポット情報の計算が完了した場合は <see langword="true"/>。 それ以外の場合は <see langword="false"/>。</returns>
        public bool WaitForCompletion(int millisecondsTimeout)
        {
            return completedEvent.Wait(millisecondsTimeout);
        }

        #region IDisposable Support
        private bool disposedValue = false; // 重複する呼び出しを検出するには

#pragma warning disable CS1591 // 公開されている型またはメンバー 'WavefrontContainer.Dispose(bool)' の XML コメントがありません。
        protected virtual void Dispose(bool disposing)
#pragma warning restore CS1591 // 公開されている型またはメンバー 'WavefrontContainer.Dispose(bool)' の XML コメントがありません。
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: マネージド状態を破棄します (マネージド オブジェクト)。
                    completedEvent.Dispose();
                }

                // TODO: アンマネージド リソース (アンマネージド オブジェクト) を解放し、下のファイナライザーをオーバーライドします。
                // TODO: 大きなフィールドを null に設定します。

                disposedValue = true;
            }
        }

        // TODO: 上の Dispose(bool disposing) にアンマネージド リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします。
        // ~WavefrontContainer() {
        //   // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
        //   Dispose(false);
        // }

        // このコードは、破棄可能なパターンを正しく実装できるように追加されました。
#pragma warning disable CS1591 // 公開されている型またはメンバー 'WavefrontContainer.Dispose()' の XML コメントがありません。
        public void Dispose()
#pragma warning restore CS1591 // 公開されている型またはメンバー 'WavefrontContainer.Dispose()' の XML コメントがありません。
        {
            // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
            Dispose(true);
            // TODO: 上のファイナライザーがオーバーライドされる場合は、次の行のコメントを解除してください。
            // GC.SuppressFinalize(this);
        }
        #endregion
        #endregion // Methods

        #region Classes
        /// <summary>
        /// 波面センサーパラメーター
        /// </summary>
        public class WavefrontParameter
        {
            #region Constructors
            /// <summary>
            /// <see cref="WavefrontParameter"/>クラスの新しいインスタンスを生成します。
            /// </summary>
            public WavefrontParameter()
            {
                ApertureCenter = new PointD();
                ApertureSize = new Size<double>();
            }
            #endregion // Constructors

            #region Properties
            /// <summary>
            /// アパーチャー中心座標[pixel]を取得・設定します。
            /// </summary>
            public PointD ApertureCenter { get; private set; }

            /// <summary>
            /// アパーチャーのサイズ[pixel]を取得・設定します。
            /// </summary>
            public Size<double> ApertureSize { get; private set; }

            /// <summary>
            /// アパーチャー形状を取得・設定します。
            /// </summary>
            public ApertureShape ApertureType { get; set; }

            /// <summary>
            /// 光源波長[um]
            /// </summary>
            public double Wavelength { get; set; }
            #endregion // Properties

            #region Methods
            /// <summary>
            /// すべての要素を初期化します。
            /// </summary>
            public void Clear()
            {
                ApertureCenter.Clear();
                ApertureSize.Clear();
                ApertureType = ApertureShape.Rectangle;
                Wavelength = 0;
            }
            #endregion // Methods
        }
        #endregion // Classes
    }
}
