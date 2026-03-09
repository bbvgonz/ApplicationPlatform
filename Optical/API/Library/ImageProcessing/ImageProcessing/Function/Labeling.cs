using OpenCvSharp;
using Optical.Platform.Types;
using System.Drawing;
using System.Threading.Tasks;

namespace Optical.API.Library
{
    /// <summary>
    /// 画像処理ライブラリクラス
    /// </summary>
    public partial class ImageProcessing
    {
        #region Methods
        private static LabelingContainer labelingStats(Mat binaryImage)
        {
            using (var labels = new Mat())
            using (var stats = new Mat<int>())
            using (var centroids = new Mat<double>())
            {
                // 統計情報付きラベリング
                int labelsCount = Cv2.ConnectedComponentsWithStats(binaryImage, labels, stats, centroids, PixelConnectivity.Connectivity8);

                var container = new LabelingContainer(labelsCount)
                {
                    Labels = new int[binaryImage.Total()]
                };
                System.Runtime.InteropServices.Marshal.Copy(labels.Data, container.Labels, 0, container.Labels.Length);

                Mat.Indexer<int> statsIndexer = stats.GetGenericIndexer<int>();
                Mat.Indexer<double> centroidsIndexer = centroids.GetGenericIndexer<double>();

                Parallel.For(0, container.Components.Length, labelIndex =>
                {
                    container[labelIndex].Centroid.XY(centroidsIndexer[labelIndex, 0], centroidsIndexer[labelIndex, 1]);

                    container[labelIndex].Circumscribed = new Rectangle(statsIndexer[labelIndex, (int)ConnectedComponentsTypes.Left],
                                                                        statsIndexer[labelIndex, (int)ConnectedComponentsTypes.Top],
                                                                        statsIndexer[labelIndex, (int)ConnectedComponentsTypes.Width],
                                                                        statsIndexer[labelIndex, (int)ConnectedComponentsTypes.Height]);

                    container[labelIndex].Area = statsIndexer[labelIndex, (int)ConnectedComponentsTypes.Area];
                });

                return container;
            }
        }

        /// <summary>
        /// 2値化された2次元画像データに対してラベリングを行います。(8近傍)
        /// </summary>
        /// <param name="binaryImage">2値化画像（ビット深度:8）</param>
        /// <param name="height">画像高さ[pixel]</param>
        /// <param name="width">画像幅[pixel]</param>
        /// <returns>ラベリング結果</returns>
        public static int[] Labeling(byte[] binaryImage, int height, int width)
        {
            using (Mat matrix = createImage(binaryImage, height, width))
            using (var label = new Mat<int>())
            {
                int labelCount = Cv2.ConnectedComponents(matrix, label);
                return label.ToArray();
            }
        }

        /// <summary>
        /// 2次元画像データに対してラベリングを行い、各ラベルに対する統計データを出力します。
        /// </summary>
        /// <param name="image">2次元画像データ</param>
        /// <param name="threshold">輝度閾値[count]</param>
        /// <param name="cutImageEnabled">閾値以下切り捨て画像出力（<paramref name="threshold"/> > 0 の場合のみ有効）</param>
        /// <returns>ラベリング統計データ</returns>
        public static LabelingContainer LabelingStats(ImageComponent image, int threshold = 0, bool cutImageEnabled = false)
        {
            using (Mat matrix = createImage(image.Pixels, image.Height, image.Width, image.BitDepth))
            {
                if (threshold > 0)
                {
                    using (Mat binaryImage = matrix.Threshold(threshold, (1 << image.BitDepth) - 1, ThresholdTypes.Tozero))
                    {
                        ImageComponent sourceImage = image;
                        if (cutImageEnabled)
                        {
                            sourceImage = new ImageComponent()
                            {
                                BitDepth = image.BitDepth,
                                Height = image.Height,
                                Pixels = new byte[image.Pixels.Length],
                                Width = image.Width
                            };
                            System.Runtime.InteropServices.Marshal.Copy(binaryImage.Data, sourceImage.Pixels, 0, sourceImage.Pixels.Length);
                        }

                        LabelingContainer stats;
                        if (binaryImage.Type() == MatType.CV_8UC1)
                        {
                            stats = labelingStats(binaryImage);
                        }
                        else
                        {
                            using (Mat convertImage = new Mat())
                            {
                                binaryImage.ConvertTo(convertImage, MatType.CV_8UC1);
                                stats = labelingStats(convertImage);
                            }
                        }

                        stats.SourceImage = sourceImage;

                        return stats;
                    }
                }
                else
                {
                    LabelingContainer stats;
                    if (matrix.Type() == MatType.CV_8UC1)
                    {
                        stats = labelingStats(matrix);
                    }
                    else
                    {
                        using (Mat convertImage = new Mat())
                        {
                            matrix.ConvertTo(convertImage, MatType.CV_8UC1);
                            stats = labelingStats(convertImage);
                        }
                    }

                    stats.SourceImage = image;

                    return stats;
                }
            }
        }
        #endregion // Methods

        #region Classes
        /// <summary>
        /// ラベリング結果格納用クラス
        /// </summary>
        public class LabelingContainer
        {
            /// <summary>
            /// 指定されたラベル数から新しいインスタンスを生成します。 
            /// </summary>
            /// <param name="labelCount">ラベル数</param>
            public LabelingContainer(int labelCount)
            {
                Components = new Stats[labelCount];
                for (int index = 0; index < Components.Length; index++)
                {
                    Components[index] = new Stats();
                }
            }

            /// <summary>
            /// 各ラベルの統計情報
            /// </summary>
            public Stats[] Components { get; internal set; }

            /// <summary>
            /// ラベリングされた行列データ
            /// </summary>
            public int[] Labels { get; internal set; } = [];

            /// <summary>
            /// ラベリングの元画像（閾値以下が0）
            /// </summary>
            public ImageComponent SourceImage { get; internal set; } = new();

            /// <summary>
            /// 各統計情報へのインデクサー
            /// </summary>
            /// <param name="index"></param>
            /// <returns></returns>
            public Stats this[int index] => Components[index];

            /// <summary>
            /// ラベルの統計情報
            /// </summary>
            public class Stats
            {
                /// <summary>
                /// <see cref="Stats"/>クラスの新しいインスタンスを生成します。
                /// </summary>
                public Stats()
                {
                    Centroid = new PointD();
                }

                /// <summary>
                /// 外接矩形
                /// </summary>
                public Rectangle Circumscribed { get; internal set; }

                /// <summary>
                /// 面積[pixel]
                /// </summary>
                public int Area { get; internal set; }

                /// <summary>
                /// 面積重心座標[pixel]
                /// </summary>
                public PointD Centroid { get; internal set; }
            }
        }
        #endregion // Classes
    }
}
