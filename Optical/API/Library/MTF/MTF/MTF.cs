using OpenCvSharp;
using Optical.API.Library.Optics;
using Optical.Platform.Mathematics;
using Optical.Platform.Types;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace Optical.API.Library
{
    /// <summary>
    /// Modulated Transfer Function(MTF)計算クラス
    /// </summary>
    public static class MTF
    {
        #region Methods
        /// <summary>
        /// 左右のコントラスト比から検査画像の状態確認を行います。
        /// </summary>
        /// <returns>画像状態： -1 = "左側：白,右側：黒" / 0 = "判別不可(低コントラスト)"/ 1= 左側：黒、右側：白
        /// </returns>
        private static int contrastCheck(ImageComponent roiImage)
        {
            using (Mat image = Mat.FromPixelData(roiImage.Height, roiImage.Width, MatType.CV_8UC1, roiImage.Pixels))
            {
                // 左5列分、右5列分の輝度値を平均する。
                using (Mat sumColumns = image.Reduce(ReduceDimension.Row, ReduceTypes.Avg, -1))
                {
                    double leftSumPixel = 0;
                    double rightSumPixel = 0;
                    for (int checkIndex = 0; checkIndex < 5; checkIndex++)
                    {
                        leftSumPixel += sumColumns.At<byte>(0, checkIndex);
                        rightSumPixel += sumColumns.At<byte>(0, (sumColumns.Width - checkIndex));
                    }

                    double errorThreshold = 0.2;
                    if (Math.Abs((leftSumPixel - rightSumPixel) / (leftSumPixel + rightSumPixel)) < errorThreshold)
                    {
                        return 0;
                    }

                    return leftSumPixel > rightSumPixel ? -1 : 1;
                }
            }
        }

        /// <summary>
        /// 畳み込み演算を行います。
        /// </summary>
        private static List<double[]> convolution(ImageComponent image, double[] filter)
        {
            int startingRow = 0;
            int startingColumn = 1;
            int rowCount = image.Height;
            int columnsCount = image.Width;

            var convolutionProfiles = new List<double[]>(rowCount);
            Func<byte[], int, uint> bitConverter = Bit.SelectConverter(image.BitDepth);
            for (int rows = startingRow; rows < rowCount; rows++)
            {
                double[] lineProfile = new double[image.Width];
                for (int columns = startingColumn; columns < columnsCount; columns++)
                {
                    int targetIndex = rows * image.Width + columns;
                    double currentPixel = (double)bitConverter(image.Pixels, targetIndex);
                    double previousPixel = (double)bitConverter(image.Pixels, targetIndex - 1);

                    lineProfile[columns - 1] = (currentPixel * filter[0]) + (previousPixel * filter[1]);
                }
                // 両端に隣の値をコピー
                lineProfile[0] = lineProfile[1];
                lineProfile[columnsCount - 1] = lineProfile[columnsCount - 2];

                convolutionProfiles.Add(lineProfile);
            }

            return convolutionProfiles;
        }

        /// <summary>
        /// ハミング窓関数を適用します。
        /// </summary>
        private static double[] correctionHammingWindow(double[] profileData, double[] hammingWindow)
        {
            double[] targetData = new double[profileData.Length];
            for (int targetIndex = 0; targetIndex < profileData.Length; targetIndex++)
            {
                targetData[targetIndex] = profileData[targetIndex] * hammingWindow[targetIndex];
            }
            return targetData;
        }

        /// <summary>
        /// 重心位置の検出を行います。
        /// </summary>
        private static double detectCentroid(double[] targetData)
        {
            double sumTargetData = targetData.Sum();
            double centroid;
            double totalTarget = 0;

            if (sumTargetData < 1e-4)
            {
                centroid = 0;
            }
            else
            {
                int targetPosition = 1;
                for (int targetIndex = 0; targetIndex < targetData.Length; targetIndex++)
                {
                    totalTarget += targetData[targetIndex] * (targetIndex + targetPosition);
                }
                centroid = totalTarget / sumTargetData;
            }
            return centroid;
        }

        /// <summary>
        /// 重心位置の座標を抽出します。
        /// </summary>
        private static PointD[] extractionCentroidCoordinate(double[] centroidProfile)
        {
            PointD[] centroidCoordinate = new PointD[centroidProfile.Length];
            for (int centroidIndex = 0; centroidIndex < centroidProfile.Length; centroidIndex++)
            {
                centroidCoordinate[centroidIndex] = new PointD { X = centroidIndex, Y = centroidProfile[centroidIndex] };
            }
            return centroidCoordinate;
        }

        /// <summary>
        /// FIRフィルタ処理
        /// </summary>
        private static double[] fir2Fix(int nn2, int difference = 3)
        {
            double[] correct = new double[nn2];
            correct = Enumerable.Repeat(1.0, correct.Length).ToArray();
            difference = difference - 1;
            int scale = 1;

            for (int index = 1; index < nn2; index++)
            {
                correct[index] = Math.Abs((Math.PI * (index + 1) * difference / (2 * (nn2 + 1))) / Math.Sin(Math.PI * (index + 1) * difference / (2 * (nn2 + 1))));
                correct[index] = 1 + scale * (correct[index] - 1);
                // 補正を範囲 [1, 10] に制限する(Peter.Burnsコメント)
                if (correct[index] > 10)
                {
                    correct[index] = 10;
                }
            }
            return correct;
        }

        /// <summary>
        /// ESFプロファイル生成
        /// </summary>
        private static double[] generateEsfProfile(ImageComponent roiImage, LinearFunction<double> regressionLine, int binning)
        {
            using (Mat image = Mat.FromPixelData(roiImage.Height, roiImage.Width, MatType.CV_8UC1, roiImage.Pixels))
            {
                int offset = (int)Math.Round(binning * (0 - (image.Height - 1) * regressionLine.Slope));
                int del = Math.Abs(offset);
                if (offset > 0)
                {
                    offset = 0;
                }

                // ESFプロファイルの長さを(offset+100)余分に用意しておく
                double[,] detectPixel = new double[2, (image.Width * binning) + del + 100];
                int countPixels = 0;
                int sumPixels = 1;
                // エッジ方向に沿ってピクセルを抽出する
                for (int columnsIndex = 0; columnsIndex < image.Width; columnsIndex++)
                {
                    for (int rowsIndex = 0; rowsIndex < image.Height; rowsIndex++)
                    {
                        int coordinateX = columnsIndex;
                        int coordinateY = rowsIndex;
                        int profilePosition = (int)Math.Ceiling((coordinateX - coordinateY * regressionLine.Slope) * binning) - offset;

                        detectPixel[countPixels, profilePosition] += 1;
                        detectPixel[sumPixels, profilePosition] += image.At<byte>(coordinateY, coordinateX);
                    }
                }

                // ピクセルが1度も抽出されなかったprofilePositionのピクセル補完
                int checkStartPosition = (int)(Math.Round(0.5 * del));
                int binningImageWidth = image.Width * binning;
                for (int checkIndex = checkStartPosition; checkIndex < checkStartPosition + binningImageWidth; checkIndex++)
                {
                    if (detectPixel[countPixels, checkIndex] == 0)
                    {
                        if (checkIndex == 0)
                        {
                            detectPixel[countPixels, checkIndex] = detectPixel[countPixels, checkIndex + 1];
                            detectPixel[sumPixels, checkIndex] = detectPixel[sumPixels, checkIndex + 1];
                        }
                        else
                        {
                            if (checkIndex == detectPixel.Length - 1)
                            {
                                detectPixel[countPixels, checkIndex] = detectPixel[countPixels, checkIndex - 1];
                                detectPixel[sumPixels, checkIndex] = detectPixel[sumPixels, checkIndex - 1];
                            }
                            detectPixel[countPixels, checkIndex] = (detectPixel[countPixels, checkIndex - 1] + detectPixel[countPixels, checkIndex + 1]) / 2;
                            detectPixel[sumPixels, checkIndex] = (detectPixel[sumPixels, checkIndex - 1] + detectPixel[sumPixels, checkIndex + 1]) / 2;
                        }
                    }
                }

                // 抽出されたピクセルデータを平均化する。
                double[] esfProfile = new double[binningImageWidth];
                for (int index = 0; index < binningImageWidth; index++)
                {
                    esfProfile[index] = detectPixel[sumPixels, index + checkStartPosition] / detectPixel[countPixels, index + checkStartPosition];
                }
                return esfProfile;
            }
        }

        private static double[] generateHammingWindow(int roiWidth, double center)
        {
            double radius = Math.Max(center - 1, roiWidth - center);
            double[] hammingData = new double[roiWidth];
            for (int index = 0; index < roiWidth; index++)
            {
                double arg = (index + 1) - center;
                hammingData[index] = 0.54 + (0.46 * Math.Cos(Math.PI * arg / (radius)));
            }

            return hammingData;
        }

        /// <summary>
        /// LSFプロファイルの生成を行います。
        /// </summary>
        private static double[] generateLsfProfile(double[] esfProfile, double[] filter)
        {
            double[] lsfProfile = new double[esfProfile.Length];
            for (int index = 1; index < (esfProfile.Length - 1); index++)
            {
                lsfProfile[index] = ((esfProfile[index + 1]) * filter[0]) + (esfProfile[index] * filter[1]) + ((esfProfile[index - 1]) * filter[2]);
            }
            // 両端に隣の値をコピー
            lsfProfile[0] = lsfProfile[1];
            lsfProfile[lsfProfile.Length - 1] = lsfProfile[lsfProfile.Length - 2];

            return lsfProfile;
        }

        /// <summary>
        /// 出力用ESFプロファイル作成
        /// </summary>
        private static List<int[]> resultEsf(double[] esfProfile)
        {

            int[] profile = new int[esfProfile.Length];
            for (int index = 0; index < esfProfile.Length; index++)
            {
                profile[index] = (int)Math.Round(esfProfile[index]);
            }

            var result = new List<int[]>(1)
            {
                profile
            };

            return result;
        }

        private static PointD[] resultLsf(double[] lsfHamming, int height, int binning)
        {
            double samplingPitch = (double)lsfHamming.Length / binning / height;

            var result = new PointD[lsfHamming.Length];
            for (int index = 0; index < result.Length; index++)
            {
                result[index] = new PointD((index * samplingPitch), lsfHamming[index]);
            }
            return result;
        }

        /// <summary>
        /// LSFのプロファイル位置を中心にシフトします。。
        /// </summary>
        private static double[] shiftCenterPosition(double[] profile)
        {
            int centroid = (int)Math.Round(detectCentroid(profile));

            int middlePosition = (int)Math.Round((profile.Length + 1.0) / 2, MidpointRounding.AwayFromZero);
            int differencePosition = centroid - middlePosition;
            double[] shiftPosition = new double[profile.Length];
            if (differencePosition > 0)
            {
                for (int index = 0; index < (profile.Length - differencePosition); index++)
                {
                    shiftPosition[index] = profile[index + differencePosition];
                }
            }
            else if (differencePosition < 1)
            {
                for (int index = ((-1) * differencePosition); index < profile.Length; index++)
                {
                    shiftPosition[index] = profile[index + differencePosition];
                }
            }
            else
            {
                return profile;
            }

            return shiftPosition;
        }

        private static ImageComponent slanteEdgeVertical(ImageComponent roiImage)
        {
            using Mat image = Mat.FromPixelData(roiImage.Height, roiImage.Width, MatType.CV_8UC1, roiImage.Pixels);
            // 各行の平均
            using Mat averageRows = image.Reduce(ReduceDimension.Row, ReduceTypes.Avg, -1);
            // 各列の平均
            using Mat averageColumns = image.Reduce(ReduceDimension.Column, ReduceTypes.Avg, -1);
            int line = 3;
            double vertical = Math.Abs(averageRows.At<byte>(0, averageRows.Width - line) - averageRows.At<byte>(0, line));
            double horizontal = Math.Abs(averageColumns.At<byte>(averageColumns.Height - line, 0) - averageColumns.At<byte>(line, 0));

            ImageComponent result = roiImage;
            if (horizontal > vertical)
            {
                Cv2.Rotate(image, image, RotateFlags.Rotate90Clockwise);

                result = new ImageComponent()
                {
                    BitDepth = roiImage.BitDepth,
                    Height = image.Height,
                    Pixels = new byte[image.Height * image.Width * Bit.ToByte(roiImage.BitDepth)],
                    Width = image.Width
                };

                System.Runtime.InteropServices.Marshal.Copy(image.Data, result.Pixels, 0, result.Pixels.Length);
            }

            return result;
        }
        private static PointD[] slantedEdgeExtraction(ImageContainer edgeImage)
        {
            if ((edgeImage == null) || (edgeImage.RawData == null))
            {
                throw new ArgumentNullException(nameof(edgeImage));
            }

            var edge = new ConcurrentBag<PointD>();
            Parallel.For(0, edgeImage.RawData.Height, rows =>
            {
                for (int columns = 0; columns < edgeImage.RawData.Width; columns++)
                {
                    // エッジ検出部分のピクセル座標を記録
                    if (edgeImage.RawData.Pixels[rows * edgeImage.Width + columns] == 0xFF)
                    {
                        edge.Add(new PointD { X = columns, Y = rows });
                    }
                }
            });

            return [.. edge];
        }

        /// <summary>
        /// 指定画像から各行の輝度プロファイルを生成します。
        /// </summary>
        /// <param name="image">画像データ</param>
        /// <param name="Roi">
        /// <para>プロファイル生成対象領域。</para>
        /// <para><see cref="Rectangle.Empty"/>の場合は、画像全体が対象になります。</para> 
        /// </param>
        /// <returns>輝度プロファイル。</returns>
        private static List<int[]> luminanceProfileGeneration(ImageContainer image, Rectangle Roi)
        {
            if ((Roi.Left < 0) || (Roi.Top < 0))
            {
                throw new ArgumentOutOfRangeException(nameof(Roi), "対象領域が画像の範囲外です。");
            }

            if ((Roi.Height > image.Height) || (Roi.Width > image.Width))
            {
                throw new ArgumentOutOfRangeException(nameof(Roi), "対象領域が画像の大きさを超えています。");
            }

            int startingRow = 0;
            int startingColumn = 0;
            int targetWidth = image.Width;
            int rowCount = image.Height;
            if (!Roi.IsEmpty)
            {
                startingRow = Roi.Top;
                startingColumn = Roi.Left;
                targetWidth = Roi.Width;
                rowCount = Roi.Height;
            }

            Func<byte[], int, uint> bitConverter = Bit.SelectConverter(image.BitDepth);
            var luminanceProfiles = new List<int[]>(rowCount);
            for (int rows = startingRow; rows < (startingRow + rowCount); rows++)
            {
                int[] lineProfile = new int[targetWidth];
                for (int columns = startingColumn; columns < (startingColumn + targetWidth); columns++)
                {
                    lineProfile[columns - startingColumn] = (int)bitConverter(image.RawData.Pixels, (rows * image.Width + columns));
                }

                luminanceProfiles.Add(lineProfile);
            }

            return luminanceProfiles;
        }

        /// <summary>
        /// Canny方式のエッジ検出により算出された各空間周波数に対するMTF値を出力します。
        /// </summary>
        /// <param name="parameter">MTF（エッジ法）計算用パラメーター</param>
        /// <returns>MTF計算結果</returns>
        public static MtfEdgeContainer SlantedEdgeCanny(MtfEdgeParameter parameter)
        {
            if (!parameter.IsValid)
            {
                throw new ArgumentException("不正なパラメーターです。", nameof(parameter));
            }

            // エッジ検出画像取得
            ImageContainer edgeImage = ImageProcessing.EdgeDetectionCanny(parameter.Image, parameter.Roi, parameter.MinThreshold, parameter.MaxThreshold);

            // エッジ座標抽出
            PointD[] edgePointSampling = slantedEdgeExtraction(edgeImage);
            if (edgePointSampling.Length == 0)
            {
                throw new ArithmeticException("画像内にエッジが見つかりません。");
            }

            // エッジの回帰直線から角度算出（極座標系）
            LinearFunction<double> regressionLine = Approximation.RegressionLine(edgePointSampling);
            if (regressionLine.Slope == 0)
            {
                throw new ArithmeticException("垂直なエッジでは計算できません。");
            }

            // 必要Line数を算出（1/tanθ）
            int lineCount = (int)Math.Ceiling(Math.Abs(regressionLine.Slope));
            if (lineCount > edgeImage.Height)
            {
                throw new ArithmeticException($"対象画像が小さすぎます。\n(Image Height : {lineCount})");
            }

            if (lineCount < 1)
            {
                throw new ArithmeticException($"エッジ角度が大きすぎます。\n(Edge Angle : {Unit.ToDegree(Math.Atan(1 / regressionLine.Slope))}[deg])");
            }

            // エッジ画像の中間点を基準に各Lineの微分プロファイル作成
            Rectangle differentialRoi = parameter.Roi;
            differentialRoi.Y += (parameter.Roi.Height - lineCount) / 2;
            differentialRoi.Height = lineCount;
            if (differentialRoi.IsEmpty)
            {
                differentialRoi.Width = parameter.Image.Width;
            }

            List<int[]> differentialProfiles = ImageProcessing.DifferentialHorizontalProfiles(parameter.Image, differentialRoi);

            // 微分プロファイルを並び替えて合成LSFを作成。
            int profileLength = differentialProfiles[0].Length;
            var lineSpreadFunction = new PointD[differentialProfiles.Count * profileLength];
            double samplingPitch = (double)1 / differentialProfiles.Count;
            int peak = 0;
            for (int columns = 0; columns < profileLength; columns++)
            {
                for (int rows = 0; rows < differentialProfiles.Count; rows++)
                {
                    int targetIndex = columns * differentialProfiles.Count + rows;
                    lineSpreadFunction[targetIndex] = new PointD((targetIndex * samplingPitch), differentialProfiles[rows][columns]);
                    peak = (Math.Abs(differentialProfiles[rows][columns]) > Math.Abs(peak)) ? differentialProfiles[rows][columns] : peak;
                }
            }

            // 合成LSFを正規化。
            if (peak != 0)
            {
                Parallel.For(0, lineSpreadFunction.Length, index =>
                {
                    lineSpreadFunction[index].Y /= peak;
                });
            }

            // 出力データ生成
            var result = new MtfEdgeContainer
            {
                EdgeImage = edgeImage,
                EsfProfile = luminanceProfileGeneration(parameter.Image, differentialRoi),
                LsfPeak = peak,
                LsfProfile = lineSpreadFunction,
                MtfProfile = new PointD[parameter.SpatialFrequency.Length],
                SlantedEdgeAngle = Unit.ToDegree(Math.Atan(1 / regressionLine.Slope)),
            };

            // 正規化LSFを各空間周波数毎にフーリエ変換。
            Parallel.For(0, parameter.SpatialFrequency.Length, index =>
            {
                double spatialFrequency = parameter.SpatialFrequency[index];
                result.MtfProfile[index] = new PointD(spatialFrequency, FourierTransform.DFT(lineSpreadFunction, spatialFrequency).Magnitude);
            });

            double referenceMtf = FourierTransform.DFT(lineSpreadFunction, 0).Magnitude;
            foreach (PointD mtf in result.MtfProfile)
            {
                mtf.Y /= referenceMtf;
            }

            return result;
        }

        /// <summary>
        /// Peter D. Burns方式のエッジ検出により算出された各空間周波数に対するMTF値を出力します。
        /// </summary>
        /// <param name="parameter">MTF（エッジ法）計算用パラメーター(※RoiとRAW画像データでMTF計算します。)</param>
        /// <returns>MTF計算結果</returns>
        /// <remarks>
        /// 引用：sfrmat3: SFR analysis for digital cameras and scanners
        /// ISO 12233:2000準拠
        /// Matlabコード：sfrmat3　著者: Peter D. Burns
        /// </remarks>
        public static MtfEdgeContainer SlantedEdgePeterBurns(MtfEdgeParameter parameter)
        {
            // ROI画像データの取得
            var region = new Rectangle(parameter.Roi.X, parameter.Roi.Y, parameter.Roi.Width, parameter.Roi.Height);
            ImageComponent roiImage = ImageProcessing.Trim(parameter.Image.RawData, region, ApertureShape.Rectangle);

            // エッジの特徴が水平方向の場合、垂直にする
            roiImage = slanteEdgeVertical(roiImage);

            // コントラストチェック
            int contrastResult = contrastCheck(roiImage);
            if (contrastResult == 0)
            {
                throw new OperationCanceledException("The contrast of the object to be measured is low.\nErrors may occur in the measurement results.");
            }

            double[] roiFilter = [(0.5 * contrastResult), (-0.5 * contrastResult)];
            // 畳み込み演算
            List<double[]> convolutionProfiles = convolution(roiImage, roiFilter);

            // 窓関数を作成(フラットな状態で平滑化)
            double[] hammingWindow = generateHammingWindow(roiImage.Width, (roiImage.Width + 1) / 2);
            double[] centroidProfile = new double[convolutionProfiles.Count];

            // 重心位置検出(-0.5 shift for FIR phase)
            for (int index = 0; index < convolutionProfiles.Count; index++)
            {
                double[] correctionResult = correctionHammingWindow(convolutionProfiles[index], hammingWindow);
                centroidProfile[index] = detectCentroid(correctionResult) - 0.5;
            }

            // 重心座標を抽出して最小二乗法による傾き、切片を算出する。
            LinearFunction<double> regressionLine = Approximation.RegressionLine(extractionCentroidCoordinate(centroidProfile));

            // 求めた傾き、切片を使用して再度重心位置を検出(-0.5 shift for FIR phase)
            for (int rowsIndex = 0; rowsIndex < roiImage.Height; rowsIndex++)
            {
                double coordinateY = regressionLine.Slope * rowsIndex + regressionLine.Intercept;
                hammingWindow = generateHammingWindow(roiImage.Width, coordinateY);
                double[] correctionResult = correctionHammingWindow(convolutionProfiles[rowsIndex], hammingWindow);
                centroidProfile[rowsIndex] = detectCentroid(correctionResult) - 0.5;
            }

            // 再度重心座標を抽出して最小二乗法による傾き、切片を算出する。
            regressionLine = Approximation.RegressionLine(extractionCentroidCoordinate(centroidProfile));

            //角度算出
            var slantedEdgeAngle = Unit.ToDegree(Math.Atan(Math.Abs(regressionLine.Slope)));
            if (slantedEdgeAngle < 3.5)
            {
                throw new WarningException($"High slope warning.\n{slantedEdgeAngle}deg.\nErrors may occur in the measurement results.");
            }

            int binning = 4;
            int binningRoiWidth = roiImage.Width * binning;

            // ESFプロファイル生成
            double[] esfProfile = generateEsfProfile(roiImage, regressionLine, binning);

            // LSFプロファイル生成
            double[] lsfFilter = [(0.5 * contrastResult), 0, (-0.5 * contrastResult)];
            double[] lsfProfile = generateLsfProfile(esfProfile, lsfFilter);

            // LSFのプロファイル位置を中心に移動
            double lsfCentroid = detectCentroid(lsfProfile);
            double[] lsfCenterProfile = shiftCenterPosition(lsfProfile);

            // 窓関数を作成(binning(4) * Roi横幅の平滑化)
            double[] binningHammingWindow = generateHammingWindow(lsfCenterProfile.Length, ((double)(lsfCenterProfile.Length + 1) / 2));
            // LSFにハミング窓関数適用
            double[] lsfHaminng = correctionHammingWindow(lsfCenterProfile, binningHammingWindow);

            // MTF作成(高速フーリエ変換 )
            var lsfImage = new ImageComponent<double>()
            {
                BitDepth = roiImage.BitDepth,
                Height = 1,
                Pixels = lsfHaminng,
                Width = lsfHaminng.Length
            };
            double[] fft = FourierTransform.FFTMagnitude(lsfImage, true).Pixels;

            // MTF正規化
            double[] mtf = new double[(int)Math.Floor((double)binningRoiWidth / 2) + 1];
            for (int index = 0; index < mtf.Length; index++)
            {
                mtf[index] = (fft[index] / fft[0]);
            }

            // FIRフィルタ適用
            double[] firFilter = fir2Fix(mtf.Length);
            for (int index = 0; index < firFilter.Length; index++)
            {
                mtf[index] *= firFilter[index];
            }

            double delfac = Math.Cos(Math.Atan(regressionLine.Slope));

            var result = new MtfEdgeContainer
            {
                EsfProfile = resultEsf(esfProfile),
                LsfPeak = lsfHaminng.Max(),
                LsfProfile = resultLsf(lsfHaminng, roiImage.Height, binning),
                MtfProfile = new PointD[mtf.Length],
                SlantedEdgeAngle = slantedEdgeAngle,
                CentroidProfile = centroidProfile
            };

            // 空間周波数毎とMTFを紐づけ
            for (int index = 0; index < result.MtfProfile.Length; index++)
            {
                result.MtfProfile[index] = new PointD(((binning * (index)) / (delfac * fft.Length)), mtf[index]);
            }

            return result;
        }

        /// <summary>
        /// MTF計算結果から指定した各空間周波数に近いMTF値を出力します。
        /// </summary>
        /// <param name="mtfrofile">MTFプロファイルパラメーター</param>
        /// <param name="spatialFrequency">空間周波数パラメーター</param>
        /// <returns>指定した空間周波数のMTF計算結果</returns>
        public static MtfEdgeContainer SpecifyMtfProfile(MtfEdgeContainer mtfrofile, double[] spatialFrequency)
        {
            // 出力データ生成
            var result = new MtfEdgeContainer
            {
                EsfProfile = mtfrofile.EsfProfile,
                LsfPeak = mtfrofile.LsfPeak,
                LsfProfile = mtfrofile.LsfProfile,
                MtfProfile = new PointD[spatialFrequency.Length],
                SlantedEdgeAngle = mtfrofile.SlantedEdgeAngle,
            };

            // 指定した各空間周波数のMTF
            for (int index = 0; index < (mtfrofile.MtfProfile.Length); index++)
            {
                if (mtfrofile.MtfProfile[index].X < spatialFrequency[0])
                {
                    result.MtfProfile[0] = new PointD(mtfrofile.MtfProfile[index].X, mtfrofile.MtfProfile[index].Y);
                }
            }

            return result;
        }
        #endregion // Methods

        #region Classes
        /// <summary>
        /// MTF計算結果格納クラス
        /// </summary>
        public class MtfContainer
        {
            #region Constructors
            /// <summary>
            /// <see cref="MtfContainer"/>クラスの新しいインスタンスを生成します。
            /// </summary>
            public MtfContainer()
            {
                MtfProfile = (PointD[])Enumerable.Empty<PointD>();
            }
            #endregion // Constructors

            #region Properties
            /// <summary>
            /// MTFプロファイル
            /// </summary>
            /// <remarks>X:空間周波数[cycles/mm], Y:MTF値</remarks>
            public PointD[] MtfProfile { get; set; }
            #endregion // Properties
        }

        /// <summary>
        /// MTF（エッジ法）計算用パラメーター格納クラス
        /// </summary>
        public class MtfEdgeParameter
        {
            #region Constructors
            /// <summary>
            /// <see cref="MtfEdgeParameter"/>クラスの新しいインスタンスを生成します。
            /// </summary>
            public MtfEdgeParameter()
            {
                Image = new ImageContainer();
                Roi = Rectangle.Empty;
                SpatialFrequency = (double[])Enumerable.Empty<double>();
            }
            #endregion // Constructors

            #region Properties
            /// <summary>
            /// 画像データ
            /// </summary>
            public ImageContainer Image { get; }
            /// <summary>
            /// エッジ検出最小しきい値
            /// </summary>
            public int MinThreshold { get; set; }
            /// <summary>
            /// エッジ検出最大しきい値
            /// </summary>
            public int MaxThreshold { get; set; }
            /// <summary>
            /// 計算対象領域[pixel]
            /// </summary>
            /// <remarks>
            /// <para>左上原点。</para>
            /// <para>本プロパティが<see cref="Rectangle.Empty"/>の場合、画像全体が計算対象になります。</para>
            /// </remarks>
            public Rectangle Roi { get; set; }
            /// <summary>
            /// 空間周波数[cycles/pixel]
            /// </summary>
            public double[] SpatialFrequency { get; set; }
            /// <summary>
            /// パラメーター設定が正常かどうかを確認します。
            /// </summary>
            public bool IsValid
            {
                get
                {
                    if (!(SpatialFrequency?.Length > 0))
                    {
                        return false;
                    }

                    if (!(Image.RawData?.Pixels?.Length > 0))
                    {
                        return false;
                    }

                    if (Image.BitDepth < 1)
                    {
                        return false;
                    }

                    if (Image.RawData.Pixels.Length != (Image.Height * Image.Width * Bit.ToByte(Image.BitDepth)))
                    {
                        return false;
                    }

                    if ((Roi.Left < 0) || (Roi.Top < 0))
                    {
                        return false;
                    }

                    if (((Roi.Top + Roi.Height) > Image.Height) ||
                        ((Roi.Left + Roi.Width) > Image.Width))
                    {
                        return false;
                    }

                    return true;
                }
            }
            #endregion // Properties
        }

        /// <summary>
        /// MTF（エッジ法）計算結果格納クラス
        /// </summary>
        /// <seealso cref="MtfContainer"/>
        public class MtfEdgeContainer : MtfContainer
        {
            #region Constructors
            /// <summary>
            /// <see cref="MtfEdgeContainer"/>クラスの新しいインスタンスを生成します。
            /// </summary>
            public MtfEdgeContainer()
            {
                EsfProfile = new List<int[]>();
                LsfProfile = (PointD[])Enumerable.Empty<PointD>();
                CentroidProfile = (double[])Enumerable.Empty<double>();
            }
            #endregion // Constructors

            #region Properties
            /// <summary>
            /// エッジ検出画像
            /// </summary>
            public ImageContainer EdgeImage { get; set; } = new();
            /// <summary>
            /// ESF(Edge Spread Function)プロファイル
            /// </summary>
            public List<int[]> EsfProfile { get; set; }
            /// <summary>
            /// 合成LSFピーク値
            /// </summary>
            public double LsfPeak { get; set; }
            /// <summary>
            /// <para>合成LSF(Line Spread Function)プロファイル</para>
            /// <para>X:距離[pixel]、Y：変化量</para>
            /// </summary>
            public PointD[] LsfProfile { get; set; }
            /// <summary>
            /// 傾斜エッジ角度[deg]
            /// </summary>
            public double SlantedEdgeAngle { get; set; }
            /// <summary>
            /// 重心位置
            /// </summary>
            public double[] CentroidProfile { get; set; }
            #endregion // Properties

            #region Methods
            /// <summary>
            /// <see cref="MtfEdgeContainer"/>のすべての要素を初期化します。
            /// </summary>
            public void Clear()
            {
                EdgeImage?.Clear();
                EsfProfile?.Clear();
                LsfPeak = 0;
                LsfProfile = (PointD[])Enumerable.Empty<PointD>();
                SlantedEdgeAngle = 0;
                CentroidProfile = (double[])Enumerable.Empty<double>();
            }
            #endregion
        }
        #endregion // Classes
    }
}
