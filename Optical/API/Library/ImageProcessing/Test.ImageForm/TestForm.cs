using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Optical.API.Library;
using Optical.Platform.Types;
using OpenCvSharp;
using Optical.Platform.Mathematics;

namespace Test.API.Library
{
    public partial class TestForm : Form
    {
        public TestForm()
        {
            InitializeComponent();
        }

        #region Enums
        /// <summary>
        /// ビーム幅の定義
        /// </summary>
        public enum BeamwidthType
        {
            /// <summary>
            /// 半値全幅（Full width at half maximum）
            /// </summary>
            FWHM,
            /// <summary>
            /// 1/e^2幅
            /// </summary>
            ReciprocalNapierSquared,
            /// <summary>
            /// 2次モーメント幅（D4σ）
            /// </summary>
            D4Sigma,
            /// <summary>
            /// ナイフエッジ幅
            /// </summary>
            KnifeEdge,
            /// <summary>
            /// トータルビームパワーの86%幅
            /// </summary>
            D86
        }

        #endregion // Enums

        #region Methods
        private void button1_Click(object sender, EventArgs e)
        {
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            for (int i = 0; i < 1; i++)
            {
                testMat();
            }

            stopwatch.Stop();
            Console.WriteLine($"{stopwatch.ElapsedMilliseconds}[ms]\n");
        }

        /// <summary>
        /// デバッグ用メソッド
        /// </summary>
        private void LabelingDebug()
        {
            using (var fs = new FileStream(@"C:\Work\img\20210219_AutoAperture.bin", FileMode.Open, FileAccess.Read))
            //using (var fs = new FileStream(@"C:\Work\img\20210222_8bit.bin", FileMode.Open, FileAccess.Read))
            {
                byte[] raw = new byte[fs.Length];
                fs.Read(raw, 0, raw.Length);

                var image = new ImageComponent()
                {
                    BitDepth = 12,
                    Height = 1536,
                    Pixels = raw,
                    Width = 2048
                };
                ImageComponent cutImage = ImageProcessing.ToZero(image, 128, true);
            }
        }

        private void shadingCorrectionLuminuanceEdge(int imageHeight, int imageWidth, byte[] raw, Mat testImage)
        {
            // シェーディング調査
            // Y軸方向補正関数算出
            byte[] rowMin = new byte[imageHeight];
            byte[] rowMax = new byte[imageHeight];
            byte[] rows = new byte[imageWidth];
            for (int row = 0; row < rows.Length; row++)
            {
                Buffer.BlockCopy(raw, row * imageWidth, rows, 0, imageWidth);
                rowMin[row] = rows.Min();
                rowMax[row] = rows.Max();
            }

            // 輝度カーブのエッジ検出
            int rowAverage = (int)rowMax.Average(row => row);
            byte[] rowEdge = new byte[imageWidth];
            rowEdge[0] = (rowMax[0] < rowMax[1]) ? (byte)0 : rowMax[0];
            for (int column = 1; column < rows.Length; column++)
            {
                if ((rowMax[column] >= rowMax[column - 1]) &&
                    (rowMax[column] >= rowMax[column + 1]))
                {
                    if (rowMax[column] >= rowAverage)
                    {
                        rowEdge[column] = rowMax[column];
                    }
                }
            }

            byte rowPrevious = 0;
            byte rowNext = 0;
            for (int index = 0; index < rowEdge.Length; index++)
            {
                if (rowEdge[index] == 0)
                {
                    continue;
                }

                for (int searchIndex = index + 1; searchIndex < rowEdge.Length; searchIndex++)
                {
                    if (rowEdge[searchIndex] == 0)
                    {
                        continue;
                    }

                    rowNext = rowEdge[searchIndex];
                    break;
                }

                if ((rowEdge[index] < rowPrevious) &&
                    (rowEdge[index] < rowNext))
                {
                    rowEdge[index] = 0;
                }
                else
                {
                    rowPrevious = rowEdge[index];
                }
            }

            var rowEdgeList = rowEdge.Select((edge, index) => new PointD(index, edge)).Where(point => point.Y != 0).ToList();

            double[] rowCoefficient = Approximation.Polynomial(rowEdgeList, 6);

            // X軸方向補正関数算出
            byte[] columnMin = new byte[imageWidth];
            byte[] columnMax = new byte[imageWidth];
            var tempMat = new Mat();
            byte[] columns = new byte[imageWidth];
            for (int column = 0; column < columns.Length; column++)
            {
                testImage.Col(column).CopyTo(tempMat);
                System.Runtime.InteropServices.Marshal.Copy(tempMat.Data, columns, 0, columns.Length);
                columnMin[column] = columns.Min();
                columnMax[column] = columns.Max();
            }

            // 輝度カーブのエッジ検出
            int columnAverage = (int)columnMax.Average(column => column);
            byte[] columnEdge = new byte[imageWidth];
            columnEdge[0] = (columnMax[0] < columnMax[1]) ? (byte)0 : columnMax[0];
            for (int column = 1; column < columns.Length; column++)
            {
                if ((columnMax[column] >= columnMax[column - 1]) &&
                    (columnMax[column] >= columnMax[column + 1]))
                {
                    if (columnMax[column] >= columnAverage)
                    {
                        columnEdge[column] = columnMax[column];
                    }
                }
            }

            byte columnPrevious = 0;
            byte columnNext = 0;
            for (int index = 0; index < columnEdge.Length; index++)
            {
                if (columnEdge[index] == 0)
                {
                    continue;
                }

                for (int searchIndex = index + 1; searchIndex < columnEdge.Length; searchIndex++)
                {
                    if (columnEdge[searchIndex] == 0)
                    {
                        continue;
                    }

                    columnNext = columnEdge[searchIndex];
                    break;
                }

                if ((columnEdge[index] < columnPrevious) &&
                    (columnEdge[index] < columnNext))
                {
                    columnEdge[index] = 0;
                }
                else
                {
                    columnPrevious = columnEdge[index];
                }
            }

            var columnEdgeList = rowEdge.Select((edge, index) => new PointD(index, edge)).Where(point => point.Y != 0).ToList();

            double[] columnCoefficient = Approximation.Polynomial(columnEdgeList, 6);
            int bitDepth = 8;
            int maxPixel = (1 << bitDepth) - 1;
            byte[] shadingImage = new byte[raw.Length];
            for (int indexY = 0; indexY < imageHeight; indexY++)
            {
                for (int indexX = 0; indexX < imageWidth; indexX++)
                {
                    double pixel = raw[indexY * imageWidth + indexX] * correctionRate(indexX, columnCoefficient, indexY, rowCoefficient, bitDepth);
                    shadingImage[indexY * imageWidth + indexX] = (pixel > maxPixel) ? (byte)maxPixel : (byte)pixel;
                }

                continue;
            }

            var resultImage = new Mat(imageHeight, imageWidth, MatType.CV_8UC1);
            resultImage.SetArray(shadingImage);
            resultImage.SaveImage(@"C:\Users\09854\Pictures\Shading.bmp");
        }

        private void shadingCorrectionPeakEdge(ImageComponent image, Mat testImage)
        {
            var time = new System.Diagnostics.Stopwatch[3];
            for (int index = 0; index < time.Length; index++)
            {
                time[index] = new System.Diagnostics.Stopwatch();
            }


            // 2値化
            time[0].Restart();
            ImageComponent test = ImageProcessing.Binarize(image, 20);
            time[0].Stop();

            // ラベリングstats
            time[1].Restart();
            ImageProcessing.LabelingContainer labels = ImageProcessing.LabelingStats(test);
            time[1].Stop();

            time[2].Restart();
            //var labelImage = new Mat();
            //testImage.CopyTo(labelImage);
            foreach (ImageProcessing.LabelingContainer.Stats label in labels.Components)
            //Parallel.ForEach(labels.Components, label =>
            {
                if ((label.Circumscribed.X == 0) ||
                    (label.Circumscribed.Y == 0) ||
                    ((label.Circumscribed.Right) == image.Width) ||
                    ((label.Circumscribed.Bottom) == image.Height))
                {
                    double aspect = ((double)label.Circumscribed.Width / label.Circumscribed.Height);
                    if ((aspect <= 0.9) || (aspect >= 1.1))
                    {
                        continue;
                    }
                }

                if ((label.Area > 120) && (label.Area < 500))
                {
                    testImage[label.Circumscribed.Y,
                              label.Circumscribed.Y + label.Circumscribed.Height,
                              label.Circumscribed.X,
                              label.Circumscribed.X + label.Circumscribed.Width].MinMaxLoc(out double min, out double max);

                    testImage[label.Circumscribed.Y,
                              label.Circumscribed.Y + label.Circumscribed.Height,
                              label.Circumscribed.X,
                              label.Circumscribed.X + label.Circumscribed.Width] = testImage[label.Circumscribed.Y,
                                                                                             label.Circumscribed.Y + label.Circumscribed.Height,
                                                                                             label.Circumscribed.X,
                                                                                             label.Circumscribed.X + label.Circumscribed.Width] * (255 / max);

                    // 指定範囲をネガポジ反転
                    //testImage[label.Circumscribed.Y,
                    //          label.Circumscribed.Y + label.Circumscribed.Height,
                    //          label.Circumscribed.X,
                    //          label.Circumscribed.X + label.Circumscribed.Width] = ~testImage[label.Circumscribed.Y,
                    //                                                                          label.Circumscribed.Y + label.Circumscribed.Height,
                    //                                                                          label.Circumscribed.X,
                    //                                                                          label.Circumscribed.X + label.Circumscribed.Width];
                }
            }
            //});
            time[2].Stop();

            System.Diagnostics.Debug.WriteLine($"{time[0].ElapsedMilliseconds}, {time[1].ElapsedMilliseconds}, {time[2].ElapsedMilliseconds}");

            //using (var writer = new System.IO.StreamWriter(@"C:\Users\09854\Pictures\centroid.csv"))
            //{
            //    foreach (var label in labels.Components)
            //    {
            //        writer.WriteLine($"{label.Centroid.X},{label.Centroid.Y}");
            //    }
            //}

            //testImage.SaveImage(@"C:\Users\09854\Pictures\label.bmp");
#if false
            // Y軸方向補正関数算出
            var rowMin = new byte[imageHeight];
            var rowMax = new byte[imageHeight];
            var rows = new byte[imageHeight];
            for (int row = 0; row < rows.Length; row++)
            {
                Buffer.BlockCopy(raw, row * imageWidth, rows, 0, imageWidth);
                rowMin[row] = rows.Min();
                rowMax[row] = rows.Max();
            }

            // 輝度カーブのエッジ検出
            int rowAverage = (int)rowMax.Average(row => row);
            var rowEdge = new byte[imageWidth];
            rowEdge[0] = (rowMax[0] < rowMax[1]) ? (byte)0 : rowMax[0];
            for (int row = 1; row < rows.Length - 1; row++)
            {
                if ((rowMax[row] >= rowMax[row - 1]) &&
                    (rowMax[row] >= rowMax[row + 1]))
                {
                    if (rowMax[row] >= rowAverage)
                    {
                        rowEdge[row] = rowMax[row];
                    }
                }
            }

            rowEdge[rows.Length - 1] = (rowMax[rows.Length - 1] < rowMax[rows.Length - 1]) ? (byte)0 : rowMax[rows.Length - 1];

            byte rowPrevious = 0;
            byte rowNext = 0;
            for (int index = 0; index < rowEdge.Length; index++)
            {
                if (rowEdge[index] == 0)
                {
                    continue;
                }

                for (int searchIndex = index + 1; searchIndex < rowEdge.Length; searchIndex++)
                {
                    if (rowEdge[searchIndex] == 0)
                    {
                        continue;
                    }

                    rowNext = rowEdge[searchIndex];
                    break;
                }

                if ((rowEdge[index] < rowPrevious) &&
                    (rowEdge[index] < rowNext))
                {
                    rowEdge[index] = 0;
                }
                else
                {
                    rowPrevious = rowEdge[index];
                }
            }

            List<PointD> rowEdgeList = rowEdge.Select((edge, index) => new PointD(index, edge)).Where(point => point.Y != 0).ToList();

            double[] rowCoefficient = Approximation.Polynomial(rowEdgeList, 6);

            // X軸方向補正関数算出
            var columnMin = new byte[imageWidth];
            var columnMax = new byte[imageWidth];
            var tempMat = new Mat();
            var columns = new byte[imageWidth];
            for (int column = 0; column < columns.Length; column++)
            {
                testImage.Col[column].CopyTo(tempMat);
                System.Runtime.InteropServices.Marshal.Copy(tempMat.Data, columns, 0, columns.Length);
                columnMin[column] = columns.Min();
                columnMax[column] = columns.Max();
            }

            // 輝度カーブのエッジ検出
            int columnAverage = (int)columnMax.Average(column => column);
            var columnEdge = new byte[imageWidth];
            columnEdge[0] = (columnMax[0] < columnMax[1]) ? (byte)0 : columnMax[0];
            for (int column = 1; column < columns.Length - 1; column++)
            {
                if ((columnMax[column] >= columnMax[column - 1]) &&
                    (columnMax[column] >= columnMax[column + 1]))
                {
                    if (columnMax[column] >= columnAverage)
                    {
                        columnEdge[column] = columnMax[column];
                    }
                }
            }

            columnEdge[columns.Length - 1] = (columnMax[columns.Length - 1] < columnMax[columns.Length - 1]) ? (byte)0 : columnMax[columns.Length - 1];

            byte columnPrevious = 0;
            byte columnNext = 0;
            for (int index = 0; index < columnEdge.Length; index++)
            {
                if (columnEdge[index] == 0)
                {
                    continue;
                }

                for (int searchIndex = index + 1; searchIndex < columnEdge.Length; searchIndex++)
                {
                    if (columnEdge[searchIndex] == 0)
                    {
                        continue;
                    }

                    columnNext = columnEdge[searchIndex];
                    break;
                }

                if ((columnEdge[index] < columnPrevious) &&
                    (columnEdge[index] < columnNext))
                {
                    columnEdge[index] = 0;
                }
                else
                {
                    columnPrevious = columnEdge[index];
                }
            }

            List<PointD> columnEdgeList = rowEdge.Select((edge, index) => new PointD(index, edge)).Where(point => point.Y != 0).ToList();

            double[] columnCoefficient = Approximation.Polynomial(columnEdgeList, 6);
            int bitDepth = 8;
            int maxPixel = (1 << bitDepth) - 1;
            var shadingImage = new byte[raw.Length];
            for (int indexY = 0; indexY < imageHeight; indexY++)
            {
                for (int indexX = 0; indexX < imageWidth; indexX++)
                {
                    var pixel = raw[indexY * imageWidth + indexX] * correctionRate(indexX, columnCoefficient, indexY, rowCoefficient, bitDepth);
                    shadingImage[indexY * imageWidth + indexX] = (pixel > maxPixel) ? (byte)maxPixel : (byte)pixel;
                }
            }

            var resultImage = new Mat(imageHeight, imageWidth, MatType.CV_8UC1, shadingImage);
            resultImage.SaveImage(@"C:\Users\09854\Pictures\Shading.bmp");
#endif
        }

        private double correctionRate(int indexX, double[] columnCoefficient, int indexY, double[] rowCoefficient, int bitDepth)
        {
            if ((columnCoefficient is null) || (columnCoefficient.Length < 1))
            {
                return 0.0;
            }

            if ((rowCoefficient is null) || (rowCoefficient.Length < 1))
            {
                return 0.0;
            }

            if (bitDepth < 0)
            {
                return 0.0;
            }

            double variableX = indexX;
            double correctionValueX = columnCoefficient[0];
            for (int degree = 1; degree < columnCoefficient.Length; degree++)
            {
                correctionValueX += columnCoefficient[degree] * variableX;
                variableX *= indexX;
            }

            double variableY = indexY;
            double correctionValueY = rowCoefficient[0];
            for (int degree = 1; degree < rowCoefficient.Length; degree++)
            {
                correctionValueY += rowCoefficient[degree] * variableY;
                variableY *= indexY;
            }

            double correctionValue = Math.Sqrt(((correctionValueX * correctionValueX) + (correctionValueY * correctionValueY)) / 2);

            double testResult = (((1 << bitDepth) - 1) / correctionValue) * (((1 << bitDepth) - 1) / correctionValue);

            //return ((1 << bitDepth) - 1) / correctionValue;
            return testResult;
        }

        private void testMat()
        {
            var sub = new Mat(2448, 2048, MatType.CV_32SC1, (Scalar)4);

            var add = new Mat(2448, 2048, MatType.CV_32SC1, (Scalar)2);
            var mat = new Mat(2448, 2048, MatType.CV_16UC1, (Scalar)1);
            var matInt = new Mat();
            matInt += mat;
            var m = new Mat();
            if (!m.Empty())
            {
                matInt += m - sub;
            }
            else
            {
                matInt += add - sub;
            }

            var convertImage = new Mat();
            ((Mat)(matInt / 1)).ConvertTo(convertImage, MatType.CV_16UC1);
            var averagedImage = new ImageComponent(matInt.Height, matInt.Width, 12);
            System.Runtime.InteropServices.Marshal.Copy(convertImage.Data, averagedImage.Pixels, 0, averagedImage.Pixels.Length);

        }
        #endregion // Methods

        #region Classes

        #endregion // Classes

    }
}
