using OpenCvSharp;
using Optical.Platform.Mathematics;
using Optical.Platform.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Numerics;
using System.Text;
using System.Windows.Forms;

namespace Optical.API.Library
{
    public partial class TestForm
    {
        #region Fields
        private UserOptions userOption;
        private Bitmap imageCanvas;
        private Bitmap roiCanvas;
        private Image loadImage;
        /// <summary>
        /// ROI表示オブジェクト
        /// </summary>
        private PictureBox roiPictureBox;
        /// <summary>
        /// 描画中画像のサイズ
        /// </summary>
        private ImageSize drawImageSize;
        private System.Drawing.Point startingDragRoi;
        private MTF.MtfEdgeContainer latestMtfProfile;
        private readonly int flameToRectangleGap = 10;
        private readonly int flameToProfileGap = 25;
        private readonly int flameToScaleOffset = 40;
        private readonly int flameToProfileOffset = 25;
        private readonly int scaleInterval = 5;
        private readonly string[] scaleValue = new string[] { "0.0", "0.2", "0.4", "0.6", "0.8", "1.0" };
        private readonly double mtf30Rate = 0.3;
        private readonly double mtf50Rate = 0.5;
        private readonly double nyquistFrequency = 0.5;
        private static slantedEdgeMethod mtfMethod;
        #endregion // Fields

        #region Enums
        public enum slantedEdgeMethod
        {
            /// <summary>
            /// Canny方式によるMTF計算
            /// </summary>
            Canny,
            /// <summary>
            /// Peter.BurnsによるMTF計算
            /// </summary>
            PeterBurns,
        }
        #endregion Enums

        #region Methods
        private void calculateMTF()
        {
            var parameter = new MTF.MtfEdgeParameter()
            {
                SpatialFrequency = [userOption.SpatialFrequency],
                MinThreshold = userOption.MinThreshold,
                MaxThreshold = userOption.MaxThreshold,
                Roi = userOption.ROI
            };
            var stream = new MemoryStream();
            loadImage.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);

            byte[] buffer = new byte[loadImage.Width * loadImage.Height * Bit.ToByte(userOption.BitDepth)];
            stream.Seek(1078, SeekOrigin.Begin);
            stream.Read(buffer, 0, buffer.Length);
            parameter.Image.RawData = new ImageComponent()
            {
                BitDepth = userOption.BitDepth,
                Height = loadImage.Height,
                Pixels = buffer,
                Width = loadImage.Width
            };
            parameter.Image.Width = loadImage.Width;
            parameter.Image.Height = loadImage.Height;
            parameter.Image.BitDepth = userOption.BitDepth;

            #region 取得画像確認用
            // Format8bppIndexedを指定してBitmapオブジェクトを作成
            var rawImage = new Bitmap(parameter.Image.Width, parameter.Image.Height, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);

            // カラーパレットを設定
            System.Drawing.Imaging.ColorPalette pal = rawImage.Palette;
            for (int index = 0; index < 256; ++index)
            {
                pal.Entries[index] = Color.FromArgb(index, index, index);
            }

            rawImage.Palette = pal;

            // 垂直反転画像を生成する。
            using Mat originalImage = new(parameter.Image.RawData.Height, parameter.Image.RawData.Width, MatType.CV_8UC1);
            originalImage.SetArray(parameter.Image.RawData.Pixels);
            var flipImage = new Mat();
            Cv2.Flip(originalImage, flipImage, FlipMode.X);
            System.Runtime.InteropServices.Marshal.Copy(flipImage.Data, parameter.Image.RawData.Pixels, 0, parameter.Image.RawData.Pixels.Length);
            flipImage.Dispose();
            

            // BitmapDataに用意したbyte配列を一気に書き込む
            System.Drawing.Imaging.BitmapData bmpdata = rawImage.LockBits(
                new Rectangle(0, 0, parameter.Image.Width, parameter.Image.Height),
                System.Drawing.Imaging.ImageLockMode.WriteOnly,
                System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
            System.Runtime.InteropServices.Marshal.Copy(parameter.Image.RawData.Pixels, 0, bmpdata.Scan0, parameter.Image.RawData.Pixels.Length);
            rawImage.UnlockBits(bmpdata);

            // RAW画像表示
            var stretchRaw = new Bitmap(rawImagePictureBox.Width, rawImagePictureBox.Height);
            var rawGraphic = Graphics.FromImage(stretchRaw);
            rawGraphic.DrawImage(rawImage, 0, 0, stretchRaw.Width, stretchRaw.Height);

            // ROI画像表示。
            var stretchRoi = new Bitmap(roiImagePictureBox.Width, roiImagePictureBox.Height);
            var roiGraphic = Graphics.FromImage(stretchRoi);
            roiGraphic.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            roiGraphic.DrawImage(rawImage.Clone(userOption.ROI, rawImage.PixelFormat), 0, 0, stretchRoi.Width, stretchRoi.Height);

            roiGraphic.Dispose();
            rawImage.Dispose();
            rawGraphic.Dispose();

            roiImagePictureBox.Image = stretchRoi;
            rawImagePictureBox.Image = stretchRaw;
            #endregion // 取得画像確認用

            // Parameterで指定された空間周波数のMTFを計算する。
            var mtf = new MTF.MtfEdgeContainer();
            if (mtfMethod == slantedEdgeMethod.Canny)
            {
                mtf = MTF.SlantedEdgeCanny(parameter);
                drawEdgeImage(mtf.EdgeImage);
            }
            else
            {
                var mtfProfileProfile = MTF.SlantedEdgePeterBurns(parameter);
                mtf = MTF.SpecifyMtfProfile(mtfProfileProfile, parameter.SpatialFrequency);
                edgeImagePictureBox.Image = null;
            }

            mtfValue.Text = mtf.MtfProfile[0].Y.ToString("F6");
            lsfPeakValue.Text = mtf.LsfPeak.ToString();
            slantedEdgeAngleValue.Text = mtf.SlantedEdgeAngle.ToString("F4");
            drawEsfProfile(mtf.EsfProfile, (1 << parameter.Image.BitDepth) - 1);
            drawLsfProfile(mtf.LsfProfile);

            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            // 指定された空間周波数毎のMTFを計算する。
            var mtfProfile = new MTF.MtfEdgeContainer();
            if (mtfMethod == slantedEdgeMethod.Canny)
            {
                // MTFプロファイル用の計算
                double[] pitch = new double[2048];
                for (int index = 0; index < pitch.Length; index++)
                {
                    pitch[index] = (double)(index + 1) / pitch.Length;
                }
                parameter.SpatialFrequency = pitch;
                mtfProfile = MTF.SlantedEdgeCanny(parameter);
                drawMtfProfile(mtfProfile.MtfProfile);
            }
            else
            {
                mtfProfile = MTF.SlantedEdgePeterBurns(parameter);
                //PeterBurnsMTFプロファイル
                int displayRatio = 2;
                PointD[] profile = new PointD[(int)(Math.Round(((double)mtfProfile.MtfProfile.Length / displayRatio), MidpointRounding.AwayFromZero))];
                for (int index = 0; index < profile.Length; index++)
                {
                    profile[index] = mtfProfile.MtfProfile[index];
                }
                drawMtfProfile(profile);
            }
            watch.Stop();
            stopwatchLabel.Text = $"{watch.ElapsedMilliseconds}[ms]";

            // ナイキスト周波数のMTFを計算する
            parameter.SpatialFrequency = [nyquistFrequency];
            var mtfNyquistProfile = new MTF.MtfEdgeContainer();
            if (mtfMethod == slantedEdgeMethod.Canny)
            {
                mtfNyquistProfile = MTF.SlantedEdgeCanny(parameter);
            }
            else
            {
                var nyquistProfile = MTF.SlantedEdgePeterBurns(parameter);
                mtfNyquistProfile = MTF.SpecifyMtfProfile(nyquistProfile, parameter.SpatialFrequency);
            }
            string mtfNyquistData = mtfNyquistProfile.MtfProfile[0].Y.ToString("F4"); ;
            //計算結果画面出力
            drawResultData(mtfProfile.MtfProfile, mtfNyquistData);

            // 計算結果保持。
            latestMtfProfile = mtf;
            latestMtfProfile.MtfProfile = mtfProfile.MtfProfile;
        }

        private void drawEdgeImage(ImageContainer edgeImage)
        {
            var canvas = new Bitmap(edgeImagePictureBox.Width, edgeImagePictureBox.Height);
            var graphic = Graphics.FromImage(canvas);
            graphic.InterpolationMode = InterpolationMode.NearestNeighbor;

            var image = new Bitmap(edgeImage.Width, edgeImage.Height, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
            System.Drawing.Imaging.ColorPalette palette = image.Palette;
            for (int index = 0; index < 256; index++)
            {
                palette.Entries[index] = Color.FromArgb(index, index, index);
            }

            image.Palette = palette;

            System.Drawing.Imaging.BitmapData bitmapData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height),
                                                                          System.Drawing.Imaging.ImageLockMode.WriteOnly,
                                                                          image.PixelFormat);
            System.Runtime.InteropServices.Marshal.Copy(edgeImage.RawData.Pixels, 0, bitmapData.Scan0, edgeImage.RawData.Pixels.Length);
            image.UnlockBits(bitmapData);

            graphic.DrawImage(image, 0, 0, canvas.Width, canvas.Height);

            image.Dispose();
            graphic.Dispose();

            edgeImagePictureBox.Image = canvas;
            edgeImagePictureBox.Invalidate();
        }

        private void drawEsfProfile(List<int[]> esfProfile, double peak)
        {
            if (peak == 0)
            {
                return;
            }

            var bitmap = new Bitmap(esfProfilePictureBox.Width, esfProfilePictureBox.Height);
            var graphic = Graphics.FromImage(bitmap);
            graphic.FillRectangle(Brushes.Black, new Rectangle(new System.Drawing.Point(0, 0), bitmap.Size));

            int drawHeight = esfProfilePictureBox.Height - 2;
            int offset = 1;
            double drawWidth = (double)esfProfilePictureBox.Width / (esfProfile.Count * esfProfile[0].Length);
            for (int rows = 0; rows < esfProfile[0].Length; rows++)
            {
                for (int columns = 0; columns < (esfProfile.Count - 1); columns++)
                {
                    graphic.DrawLine(Pens.White,
                                     new System.Drawing.Point((int)Math.Round((rows * esfProfile.Count + (columns)) * drawWidth), drawHeight + offset - (int)(esfProfile[columns][rows] * drawHeight / peak)),
                                     new System.Drawing.Point((int)Math.Round((rows * esfProfile.Count + (columns + 1)) * drawWidth), drawHeight + offset - (int)(esfProfile[columns + 1][rows] * drawHeight / peak)));
                }

                if (rows < (esfProfile[0].Length - 1))
                {
                    graphic.DrawLine(Pens.White,
                                     new System.Drawing.Point((int)Math.Round((rows * esfProfile.Count + (esfProfile.Count - 1)) * drawWidth), drawHeight + offset - (int)(esfProfile[esfProfile.Count - 1][rows] * drawHeight / peak)),
                                     new System.Drawing.Point((int)Math.Round(((rows + 1) * esfProfile.Count) * drawWidth), drawHeight + offset - (int)(esfProfile[0][rows + 1] * drawHeight / peak)));
                }
            }

            graphic.Dispose();
            esfProfilePictureBox.Image = bitmap;
            esfProfilePictureBox.Invalidate();
        }

        private void drawLsfProfile(PointD[] lsfProfile)
        {
            var profileImage = new Bitmap(syntheticLsfProfilePictureBox.Width, syntheticLsfProfilePictureBox.Height);
            var profileGraphic = Graphics.FromImage(profileImage);
            profileGraphic.FillRectangle(Brushes.Black, new Rectangle(new System.Drawing.Point(0, 0), profileImage.Size));

            int drawRange = syntheticLsfProfilePictureBox.Height * 4 / 5;
            double drawWidth = (double)syntheticLsfProfilePictureBox.Width / lsfProfile.Length;
            for (int index = 1; index < lsfProfile.Length; index++)
            {
                profileGraphic.DrawLine(Pens.White,
                                        new System.Drawing.Point((int)Math.Round((index - 1) * drawWidth), (int)((lsfProfile[index - 1].Y + 1) * drawRange) * (-1) + (drawRange * 2)),
                                        new System.Drawing.Point((int)Math.Round(index * drawWidth), (int)((lsfProfile[index].Y + 1) * drawRange) * (-1) + (drawRange * 2)));
            }

            profileGraphic.Dispose();

            syntheticLsfProfilePictureBox.Image = profileImage;
            syntheticLsfProfilePictureBox.Invalidate();
        }

        private void drawMtfBackGround()
        {
            var backGroundImage = new Bitmap(mtfBackGroundPictureBox.Width, mtfBackGroundPictureBox.Height);
            var backGroundGraphic = Graphics.FromImage(backGroundImage);
            backGroundGraphic.FillRectangle(Brushes.White, new Rectangle(new System.Drawing.Point(0, 0), backGroundImage.Size));

            var backGroundRectangle = new Rectangle(flameToScaleOffset, flameToRectangleGap,
                                                mtfBackGroundPictureBox.Width - (flameToRectangleGap + flameToScaleOffset),
                                                mtfBackGroundPictureBox.Height - (flameToRectangleGap + flameToScaleOffset));
            backGroundGraphic.DrawRectangle(Pens.Black, backGroundRectangle);

            int frequencyScaleInterval = (mtfBackGroundPictureBox.Width - flameToScaleOffset - flameToProfileGap) / scaleInterval;
            int mtfScaleInterval = (backGroundRectangle.Bottom - flameToProfileGap) / scaleInterval;

            //罫線の描画
            var ruledScaleLine = new Pen(Color.Black);
            var ruledLine = new Pen(Color.LightGray);
            ruledLine.DashStyle = DashStyle.Dot;
            for (int horizontalAxis = flameToProfileGap; horizontalAxis < backGroundRectangle.Bottom; horizontalAxis += mtfScaleInterval)
            {
                backGroundGraphic.DrawLine(ruledLine, new System.Drawing.Point(backGroundRectangle.Left, horizontalAxis), new System.Drawing.Point(backGroundRectangle.Right, horizontalAxis));
                backGroundGraphic.DrawLine(ruledScaleLine, new System.Drawing.Point(backGroundRectangle.Left, horizontalAxis), new System.Drawing.Point(backGroundRectangle.Left + 5, horizontalAxis));
                backGroundGraphic.DrawLine(ruledScaleLine, new System.Drawing.Point(backGroundRectangle.Right - 5, horizontalAxis), new System.Drawing.Point(backGroundRectangle.Right, horizontalAxis));
            }

            for (int verticalAxis = backGroundRectangle.Left + frequencyScaleInterval; verticalAxis < backGroundRectangle.Right; verticalAxis += frequencyScaleInterval)
            {
                backGroundGraphic.DrawLine(ruledLine, new System.Drawing.Point(verticalAxis, backGroundRectangle.Top), new System.Drawing.Point(verticalAxis, backGroundRectangle.Bottom));
                backGroundGraphic.DrawLine(ruledScaleLine, new System.Drawing.Point(verticalAxis, backGroundRectangle.Top), new System.Drawing.Point(verticalAxis, backGroundRectangle.Top + 5));
                backGroundGraphic.DrawLine(ruledScaleLine, new System.Drawing.Point(verticalAxis, backGroundRectangle.Bottom - 5), new System.Drawing.Point(verticalAxis, backGroundRectangle.Bottom));
            }

            // ナイキスト周波数の罫線
            var nyquistFrequencyLine = new Pen(Color.MediumSlateBlue, 2);
            backGroundGraphic.DrawLine(nyquistFrequencyLine,
                                       new PointF(backGroundRectangle.Left + ((float)backGroundRectangle.Width - (flameToProfileGap - flameToRectangleGap)) / 2, backGroundRectangle.Bottom),
                                       new PointF(backGroundRectangle.Left + ((float)backGroundRectangle.Width - (flameToProfileGap - flameToRectangleGap)) / 2, backGroundRectangle.Bottom - mtfScaleInterval));
            var nyquistFont = new Font("MS UI Gothic", 8);
            backGroundGraphic.DrawString("Nyquist f", nyquistFont, Brushes.MediumSlateBlue, 315, 300);

            //目盛の描画
            var scaleFont = new Font("MS UI Gothic", 9);
            int frequencyValuePoint = backGroundRectangle.Left;
            int mtfValuePoint = backGroundRectangle.Bottom;
            for (int scaleIndex = 0; scaleIndex <= scaleInterval; scaleIndex++)
            {
                backGroundGraphic.DrawString(scaleValue[scaleIndex], scaleFont, Brushes.Black, frequencyValuePoint - 7, backGroundRectangle.Bottom + 5);
                backGroundGraphic.DrawString(scaleValue[scaleIndex], scaleFont, Brushes.Black, backGroundRectangle.Left - 20, mtfValuePoint - 6);
                frequencyValuePoint += frequencyScaleInterval;
                mtfValuePoint -= mtfScaleInterval;
            }

            //項目名の描画
            var itemNameFont = new Font("MS UI Gothic", 9, FontStyle.Bold);
            var itemName = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Far
            };
            var pictureBoxRectangle = new Rectangle(0, 0, mtfBackGroundPictureBox.Width, mtfBackGroundPictureBox.Height);

            backGroundGraphic.DrawString("Frequency (cycles/pixel)", itemNameFont, Brushes.MediumPurple, pictureBoxRectangle, itemName);

            backGroundGraphic.RotateTransform(-90);
            backGroundGraphic.DrawString("MTF", itemNameFont, Brushes.MediumPurple, -pictureBoxRectangle.Height / 2 - 7, 5);
            backGroundGraphic.ResetTransform();

            //空間周波数データ表示位置の描画
            var dataFont = new Font("MS UI Gothic", 9, FontStyle.Bold);
            backGroundGraphic.DrawString("MTF50 = ", dataFont, Brushes.Black, 260, 45);
            backGroundGraphic.DrawString("MTF30 = ", dataFont, Brushes.Black, 260, 68);
            backGroundGraphic.DrawString("Cy/Pxl", dataFont, Brushes.Black, 360, 45);
            backGroundGraphic.DrawString("Cy/Pxl", dataFont, Brushes.Black, 360, 68);

            // ナイキスト周波数のデータ表示位置の描画
            var nyquistDataFont = new Font("MS UI Gothic", 8);
            backGroundGraphic.DrawString("MTF at Nyquist = ", nyquistDataFont, Brushes.Gray, 270, 90);

            backGroundGraphic.Dispose();

            mtfBackGroundPictureBox.Image = backGroundImage;
            mtfBackGroundPictureBox.Invalidate();
        }

        private void drawMtfProfile(PointD[] mtfProfile)
        {
            var profileImage = new Bitmap(mtfProfilePictureBox.Width, mtfProfilePictureBox.Height);
            var profileGraphic = Graphics.FromImage(profileImage);

            if (double.IsNaN(mtfProfile[0].Y))
            {
                mtfProfilePictureBox.Image = profileImage;
                mtfProfilePictureBox.Invalidate();
                profileGraphic.Dispose();
                return;
            }

            float drawHeight = mtfProfilePictureBox.Height - flameToScaleOffset - flameToProfileOffset;
            float drawWidth = (float)(mtfProfilePictureBox.Width - flameToScaleOffset - flameToProfileOffset) / mtfProfile.Length;


            var profilePen = new Pen(Color.Black, 2);
            profileGraphic.DrawLine(profilePen,
                                    new PointF(flameToScaleOffset, flameToProfileOffset),
                                    new PointF(drawWidth + flameToScaleOffset, (float)(mtfProfile[0].Y * drawHeight) * (-1) + drawHeight + flameToProfileOffset));

            for (int index = 1; index < mtfProfile.Length; index++)
            {
                profileGraphic.DrawLine(profilePen,
                                        new PointF(index * drawWidth + flameToScaleOffset, (float)((mtfProfile[index - 1].Y * drawHeight) * (-1)) + drawHeight + flameToProfileOffset),
                                        new PointF((index + 1) * drawWidth + flameToScaleOffset, (float)(mtfProfile[index].Y * drawHeight) * (-1) + drawHeight + flameToProfileOffset));
            }

            profileGraphic.Dispose();

            mtfProfilePictureBox.Image = profileImage;
            mtfProfilePictureBox.Invalidate();
        }

        private void drawResultData(PointD[] mtfProfile, string mtfNyquistData)
        {
            var mtfProfileList = new List<PointD>(mtfProfile);
            mtfProfileList.Insert(0, new PointD(0, 1));

            double[] mtfPointY = new double[mtfProfileList.Count];

            for (int index = 0; index < mtfProfileList.Count; index++)
            {
                mtfPointY[index] = mtfProfileList[index].Y;
            }

            // プロファイルから、指定された割合を超える位置のインデックスを取得
            int mtf30RateIndex = 0;
            int mtf50RateIndex = 0;
            for (int index = 0; index < mtfPointY.Length; index++)
            {
                if ((mtf30RateIndex == 0) && (mtfPointY[index] < mtf30Rate))
                {
                    mtf30RateIndex = index - 1;
                }
                if ((mtf50RateIndex == 0) && (mtfPointY[index] < mtf50Rate))
                {
                    mtf50RateIndex = index - 1;
                }
            }

            mtf30RateIndex = (mtf30RateIndex == -1) ? 0 : mtf30RateIndex;
            mtf50RateIndex = (mtf50RateIndex == -1) ? 0 : mtf50RateIndex;


            // 3次スプライン補間係数の算出
            List<Interpolation.SplineCoefficient> coefficients = Interpolation.CubicSplineCurve(mtfPointY);


            // 【MTF30%】の空間周波数を算出
            Complex[] solution30 = Equation.SolveCubic(coefficients[mtf30RateIndex].a, coefficients[mtf30RateIndex].b, coefficients[mtf30RateIndex].c, coefficients[mtf30RateIndex].d - mtf30Rate);

            double solution30X = 0;
            for (int index = 0; index < solution30.Length; index++)
            {
                // 虚数部は浮動小数点の演算誤差未満を無視する(machine.epsilon:2.22044604925031e-16)
                if ((Math.Abs(solution30[index].Imaginary) < 1e-14) && solution30[index].Real >= 0 && solution30[index].Real <= 1)
                {
                    solution30X = solution30[index].Real;
                    break;
                }
            }
            // 「cycles/pixel」の単位に変換
            double spatialFrequency30 = Math.Round((mtf30RateIndex + solution30X) / mtfProfile.Length, 4);


            // 【MTF50%】の空間周波数を算出
            Complex[] solution50 = Equation.SolveCubic(coefficients[mtf50RateIndex].a, coefficients[mtf50RateIndex].b, coefficients[mtf50RateIndex].c, coefficients[mtf50RateIndex].d - mtf50Rate);
            double solution50X = 0;
            for (int index = 0; index < solution50.Length; index++)
            {
                // 虚数部は浮動小数点の演算誤差未満を無視する(machine.epsilon:2.22044604925031e-16)
                if ((Math.Abs(solution50[index].Imaginary) < 1e-14) && solution50[index].Real >= 0 && solution50[index].Real <= 1)
                {
                    solution50X = solution50[index].Real;
                    break;
                }
            }
            // 「cycles/pixel」の単位に変換
            double spatialFrequency50 = Math.Round((mtf50RateIndex + solution50X) / mtfProfile.Length, 4);


            var drawDataImage = new Bitmap(resultDataPictureBox.Width, resultDataPictureBox.Height);
            var drawDataGraphic = Graphics.FromImage(drawDataImage);
            //空間周波数描画
            var spatialFrequencydataFont = new Font("MS UI Gothic", 9, FontStyle.Bold);
            drawDataGraphic.DrawString(Convert.ToString(spatialFrequency30), spatialFrequencydataFont, Brushes.Black, 313, 68);
            drawDataGraphic.DrawString(Convert.ToString(spatialFrequency50), spatialFrequencydataFont, Brushes.Black, 313, 45);

            //ナイキスト周波数描画
            var mtfnyquistDataFont = new Font("MS UI Gothic", 8);
            drawDataGraphic.DrawString(mtfNyquistData, mtfnyquistDataFont, Brushes.Gray, 353, 90);


            drawDataGraphic.Dispose();

            resultDataPictureBox.Image = drawDataImage;
            resultDataPictureBox.Invalidate();
        }

        private void drawImage(string filePath)
        {
            var graphic = Graphics.FromImage(imageCanvas);
            loadImage?.Dispose();
            loadImage = Image.FromFile(filePath);

            graphic.DrawImage(loadImage, 0, 0, imageCanvas.Width, imageCanvas.Height);
            //imageCanvas.RotateFlip(RotateFlipType.RotateNoneFlipY);
            drawImageSize.Width = loadImage.Width;
            drawImageSize.Height = loadImage.Height;

            graphic.Dispose();

            imagePictureBox.Invalidate();
        }

        private void drawRoi()
        {
            var roiGraphic = Graphics.FromImage(roiCanvas);
            roiGraphic.Clear(Color.Transparent);
            var stretchRoi = new Rectangle
            {
                X = userOption.ROI.X * roiCanvas.Width / drawImageSize.Width,
                Y = userOption.ROI.Y * roiCanvas.Height / drawImageSize.Height,
                Width = userOption.ROI.Width * roiCanvas.Width / drawImageSize.Width,
                Height = userOption.ROI.Height * roiCanvas.Height / drawImageSize.Height
            };
            roiGraphic.DrawRectangle(Pens.HotPink, stretchRoi);
            roiGraphic.Dispose();

            roiPictureBox.Invalidate();
        }

        private void drawDragRoi(System.Drawing.Point location)
        {
            var roiGraphic = Graphics.FromImage(roiCanvas);
            roiGraphic.Clear(Color.Transparent);

            int left = (startingDragRoi.X > location.X) ? location.X : startingDragRoi.X;
            int top = (startingDragRoi.Y > location.Y) ? location.Y : startingDragRoi.Y;
            var region = new Rectangle
            {
                X = left,
                Y = top,
                Width = Math.Abs(location.X - startingDragRoi.X),
                Height = Math.Abs(location.Y - startingDragRoi.Y)
            };
            roiGraphic.DrawRectangle(Pens.RoyalBlue, region);
            roiGraphic.Dispose();

            roiPictureBox.Invalidate();
        }

        private void endDragRoi()
        {
            startingDragRoi = System.Drawing.Point.Empty;
        }

        private void exportMtfProfile()
        {
            var dialog = new SaveFileDialog
            {
                Filter = "CSV Files(*.csv)|*.csv",
                Title = "Export MTF profile."
            };

            if (dialog.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

            using (var streamWriter = new StreamWriter(dialog.FileName))
            {
                var profile = new StringBuilder();
                profile.AppendLine("Spatial Frequency[cycles/pixel],MTF Value");
                profile.AppendLine("0,1");
                for (int index = 0; index < latestMtfProfile.MtfProfile.Length; index++)
                {
                    profile.AppendLine($"{latestMtfProfile.MtfProfile[index].X},{latestMtfProfile.MtfProfile[index].Y}");
                }

                streamWriter.Write(profile.ToString());
            }

        }

        private void exportSyntheticLsfProfile()
        {
            var dialog = new SaveFileDialog
            {
                Filter = "CSV Files(*.csv)|*.csv",
                Title = "Export synthetic LSF profile."
            };

            if (dialog.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

            using (var streamWriter = new StreamWriter(dialog.FileName))
            {
                var profile = new StringBuilder();
                profile.AppendLine("Distance[pixel],Differential value");
                for (int index = 0; index < latestMtfProfile.LsfProfile.Length; index++)
                {
                    profile.AppendLine($"{latestMtfProfile.LsfProfile[index].X},{latestMtfProfile.LsfProfile[index].Y * latestMtfProfile.LsfPeak}");
                }

                streamWriter.Write(profile.ToString());
            }
        }

        private void exportEsfProfile()
        {
            var dialog = new SaveFileDialog
            {
                Filter = "CSV Files(*.csv)|*.csv",
                Title = "Export ESF profile."
            };

            if (dialog.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

            using var streamWriter = new StreamWriter(dialog.FileName);
            var profile = new StringBuilder();
            profile.AppendLine("Luminance value");
            for (int column = 0; column < latestMtfProfile.EsfProfile[0].Length; column++)
            {
                for (int row = 0; row < latestMtfProfile.EsfProfile.Count; row++)
                {
                    profile.AppendLine($"{latestMtfProfile.EsfProfile[row][column]}");
                }
            }

            streamWriter.Write(profile.ToString());
        }

        private void initializeParameter()
        {
            userOption = new UserOptions();
            userOptionPropertyGrid.SelectedObject = userOption;

            imageCanvas = new Bitmap(imagePictureBox.Width, imagePictureBox.Height);
            imagePictureBox.Image = imageCanvas;

            roiCanvas = new Bitmap(imagePictureBox.Width, imagePictureBox.Height);
            roiPictureBox = new PictureBox
            {
                BackColor = Color.Transparent,
                Image = roiCanvas,
                Location = imagePictureBox.Location,
                Parent = imagePictureBox,
                Size = imagePictureBox.Size
            };
            roiPictureBox.MouseDown += roiPictureBox_MouseDown;
            roiPictureBox.MouseUp += roiPictureBox_MouseUp;
            roiPictureBox.MouseMove += roiPictureBox_MouseMove;

            drawImageSize = new ImageSize()
            {
                Width = imagePictureBox.Width,
                Height = imagePictureBox.Height
            };
            drawImageWidthValue.DataBindings.Add(nameof(drawImageWidthValue.Text), drawImageSize, nameof(drawImageSize.Width), false, DataSourceUpdateMode.OnPropertyChanged);
            drawImageHeightValue.DataBindings.Add(nameof(drawImageHeightValue.Text), drawImageSize, nameof(drawImageSize.Height), false, DataSourceUpdateMode.OnPropertyChanged);

            startingDragRoi = System.Drawing.Point.Empty;

            mtfProfilePictureBox.Parent = mtfBackGroundPictureBox;
            resultDataPictureBox.Parent = mtfProfilePictureBox;
            drawMtfBackGround();

            mtfMethod = slantedEdgeMethod.PeterBurns;
            peterBurnsMethodRadioButton.Checked = true;
        }

        private void updateRoi(System.Drawing.Point location)
        {
            int left = (startingDragRoi.X > location.X) ? location.X : startingDragRoi.X;
            int top = (startingDragRoi.Y > location.Y) ? location.Y : startingDragRoi.Y;
            var stretchRoi = new Rectangle
            {
                X = (int)Math.Round((double)left * drawImageSize.Width / roiCanvas.Width),
                Y = (int)Math.Round((double)top * drawImageSize.Height / roiCanvas.Height),
                Width = Math.Abs((int)Math.Round((double)(location.X - startingDragRoi.X) * drawImageSize.Width / roiCanvas.Width)),
                Height = Math.Abs((int)Math.Round((double)(location.Y - startingDragRoi.Y) * drawImageSize.Height / roiCanvas.Height))
            };
            // Bitmapの4バイト境界対応。
            stretchRoi.Width = ((stretchRoi.Width - 1) / 4 + 1) * 4;

            userOption.ROI = stretchRoi;
            userOptionPropertyGrid.Refresh();
        }

        private void startDragRoi(System.Drawing.Point location)
        {
            startingDragRoi.X = location.X;
            startingDragRoi.Y = location.Y;
        }
        #endregion // Methods

        #region Classes
        public class ImageSize : INotifyPropertyChanged
        {
            private int width;
            private int height;

            public bool IsEmpty => (width == 0) && (height == 0);

            public int Width
            {
                get => width;
                set
                {
                    width = value;
                    raisePropertyChanged(nameof(Width));
                }
            }

            public int Height
            {
                get => height;
                set
                {
                    height = value;
                    raisePropertyChanged(nameof(Height));
                }
            }

            public event PropertyChangedEventHandler? PropertyChanged;

            protected void raisePropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion // Classes
    }
}
