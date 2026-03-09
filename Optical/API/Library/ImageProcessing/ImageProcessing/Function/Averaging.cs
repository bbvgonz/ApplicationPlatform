using OpenCvSharp;
using Optical.Platform.Types;
using System;
using System.Collections.Generic;

namespace Optical.API.Library
{
    /// <summary>
    /// 画像処理ライブラリクラス
    /// </summary>
    public partial class ImageProcessing
    {
        /// <summary>
        /// 画像の移動平均化クラス
        /// </summary>
        public class MovingAverage : IDisposable
        {
            #region Fields
            private List<Mat> averagingBuffers = [];

            /// <summary>
            /// 移動平均用画像キュー
            /// </summary>
            private Queue<Mat> averagingQueue = [];

            /// <summary>
            /// 画像変換用ビット深度
            /// </summary>
            private int convertBitDepth;

            /// <summary>
            /// 移動平均用積算画像
            /// </summary>
            private Mat integratedImage = new();

            private int latestBufferIndex;
            #endregion // Fields

            #region Constructors
            /// <summary>
            /// <see cref="MovingAverage"/>クラスの新しいインスタンスを生成します。
            /// </summary>
            public MovingAverage()
            {
                initializeParameter();
            }

            /// <summary>
            /// <see cref="MovingAverage"/>クラスの新しいインスタンスを生成します。
            /// </summary>
            /// <param name="averagingCount">平均化回数</param>
            public MovingAverage(int averagingCount)
            {
                initializeParameter();

                if (averagingCount > 0)
                {
                    adjustBufferSize(averagingCount + 1);
                }
            }
            #endregion // Constructors  

            #region Properties
            /// <summary>
            /// 平均化回数
            /// </summary>
            public int AveragingCount
            {
                get => averagingBuffers.Count - 1;
                set
                {
                    if ((value < 1) || (value == (averagingBuffers.Count - 1)))
                    {
                        return;
                    }

                    adjustBufferSize(value + 1);
                    clearBuffer();
                }
            }

            /// <summary>
            /// 積算画像枚数
            /// </summary>
            public int BufferCount => averagingQueue.Count;
            #endregion // Properties

            #region Methods
            private void adjustBufferSize(int bufferSize)
            {
                if (bufferSize == averagingBuffers.Count)
                {
                    return;
                }

                averagingQueue.Clear();

                if (bufferSize > averagingBuffers.Count)
                {
                    for (int index = averagingBuffers.Count; index < bufferSize; index++)
                    {
                        averagingBuffers.Add(new Mat(integratedImage.Height, integratedImage.Width, integratedImage.Type()));
                    }
                }
                else
                {
                    averagingBuffers.RemoveRange(bufferSize, averagingBuffers.Count - bufferSize);
                }

                if (averagingBuffers.Count > 1)
                {
                    for (int index = 0; index < averagingBuffers.Count - 1; index++)
                    {
                        averagingQueue.Enqueue(averagingBuffers[index]);
                    }

                    latestBufferIndex = averagingBuffers.Count - 2;
                }
            }

            private void adjustImageSize(int width, int height, int bitDepth)
            {
                if ((width == integratedImage.Width) &&
                    (height == integratedImage.Height) &&
                    (convertBitDepth == bitDepth))
                {
                    return;
                }

                averagingQueue.Clear();
                for (int index = 0; index < averagingBuffers.Count; index++)
                {
                    averagingBuffers[index].Dispose();
                    averagingBuffers[index] = new Mat(height, width, MatType.CV_32SC1);
                }

                for (int index = 0; index < averagingBuffers.Count - 1; index++)
                {
                    averagingQueue.Enqueue(averagingBuffers[index]);
                }

                convertBitDepth = bitDepth;

                integratedImage.Dispose();
                integratedImage = new Mat(height, width, MatType.CV_32SC1);
            }

            private void clearBuffer()
            {
                int height = integratedImage.Height;
                int width = integratedImage.Width;

                for (int index = 0; index < averagingBuffers.Count; index++)
                {
                    averagingBuffers[index].Dispose();
                    averagingBuffers[index] = new Mat(height, width, MatType.CV_32SC1);
                }

                convertBitDepth = 0;

                integratedImage.Dispose();
                integratedImage = new Mat(height, width, MatType.CV_32SC1);
            }

            private void enqueue(ImageComponent image)
            {
                adjustImageSize(image.Width, image.Height, image.BitDepth);

                int nextBufferIndex = (latestBufferIndex + 1) % averagingBuffers.Count;
                latestBufferIndex = nextBufferIndex;

                createImage(image).ConvertTo(averagingBuffers[nextBufferIndex], integratedImage.Type());
                averagingQueue.Enqueue(averagingBuffers[nextBufferIndex]);
                Mat dequeue = averagingQueue.Dequeue();
                integratedImage -= dequeue;
                integratedImage += averagingBuffers[nextBufferIndex];
            }

            private ImageComponent imageAveraging()
            {
                if (averagingQueue.Count < 1)
                {
                    throw new InvalidOperationException("No images have been queued.");
                }

                MatType imageType;
                if (convertBitDepth <= 8)
                {
                    imageType = MatType.CV_8UC1;
                }
                else if (convertBitDepth <= 16)
                {
                    imageType = MatType.CV_16UC1;
                }
                else
                {
                    throw new PlatformNotSupportedException("Larger than 16bit is not supported.");
                }

                var convertImage = new Mat();
                ((Mat)(integratedImage / AveragingCount)).ConvertTo(convertImage, imageType);

                var averagedImage = new ImageComponent(integratedImage.Height, integratedImage.Width, convertBitDepth);
                System.Runtime.InteropServices.Marshal.Copy(convertImage.Data, averagedImage.Pixels, 0, averagedImage.Pixels.Length);

                return averagedImage;
            }

            private void initializeParameter()
            {
                averagingBuffers = [];
                averagingQueue = new Queue<Mat>();
                integratedImage = new Mat(0, 0, MatType.CV_32SC1);
            }

            /// <summary>
            /// 移動平均用バッファーを初期化します。
            /// </summary>
            public void Clear()
            {
                clearBuffer();
            }

            /// <summary>
            /// 指定された画像を移動平均用バッファーに追加します。
            /// </summary>
            /// <param name="image">画像データ</param>
            /// <remarks>指定された画像とバッファリングされている画像の形式が異なる場合、バッファー初期化後指定画像が追加される。</remarks>
            public void Enqueue(ImageComponent image)
            {
                enqueue(image);
            }

            /// <summary>
            /// 現在バッファリングされている画像の平均化結果を出力する。
            /// </summary>
            /// <returns>移動平均化画像</returns>
            public ImageComponent ImageAveraging()
            {
                return imageAveraging();
            }

            /// <summary>
            /// 指定された画像に基づいた移動平均画像を出力する。
            /// </summary>
            /// <param name="image">画像データ</param>
            /// <returns>移動平均化画像</returns>
            public ImageComponent ImageAveraging(ImageComponent image)
            {
                enqueue(image);
                return imageAveraging();
            }

            /// <summary>
            /// 指定された画像情報を元にインスタンスを初期化します。
            /// </summary>
            /// <param name="width">画像幅</param>
            /// <param name="height">画像高さ</param>
            /// <param name="bitDepth">ビット深度</param>
            /// <param name="averagingCount">平均化回数</param>
            public void Initialize(int width, int height, int bitDepth, int averagingCount)
            {
                adjustImageSize(width, height, bitDepth);
                adjustBufferSize(averagingCount + 1);

                clearBuffer();
            }
            #endregion // Methods

            #region IDisposable Support
            private bool disposedValue = false; // 重複する呼び出しを検出するには

            /// <summary>
            /// <see cref="MovingAverage"/> によって使用されているすべてのリソースを解放します。
            /// </summary>
            /// <param name="disposing">メソッドの呼び出し元が <see cref="IDisposable.Dispose"/> かどうかを示します。</param>
            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        // TODO: マネージド状態を破棄します (マネージド オブジェクト)。
                        for (int index = 0; index < averagingBuffers.Count; index++)
                        {
                            averagingBuffers[index].Dispose();
                        }

                        averagingBuffers.Clear();
                        averagingQueue.Clear();
                        integratedImage.Dispose();
                    }

                    // TODO: アンマネージド リソース (アンマネージド オブジェクト) を解放し、下のファイナライザーをオーバーライドします。
                    // TODO: 大きなフィールドを null に設定します。

                    disposedValue = true;
                }
            }

            // TODO: 上の Dispose(bool disposing) にアンマネージド リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします。
            // ~MovingAverage() {
            //   // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
            //   Dispose(false);
            // }

            // このコードは、破棄可能なパターンを正しく実装できるように追加されました。
            /// <summary>
            /// <see cref="MovingAverage"/> によって使用されているすべてのリソースを解放します。
            /// </summary>
            public void Dispose()
            {
                // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
                Dispose(true);
                // TODO: 上のファイナライザーがオーバーライドされる場合は、次の行のコメントを解除してください。
                // GC.SuppressFinalize(this);
            }
            #endregion
        }
    }
}
