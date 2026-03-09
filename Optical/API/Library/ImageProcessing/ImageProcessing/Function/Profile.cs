using Optical.Platform.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Optical.API.Library
{
    /// <summary>
    /// 画像処理ライブラリクラス
    /// </summary>
    public partial class ImageProcessing
    {
        /// <summary>
        /// 指定画像から各行の微分プロファイルを生成します。
        /// </summary>
        /// <param name="image">画像データ</param>
        /// <param name="roi">
        /// <para>プロファイル生成対象領域。</para>
        /// <para><see cref="Rectangle.Empty"/>の場合は、画像全体が対象になります。</para> 
        /// </param>
        /// <returns>微分プロファイル。</returns>
        /// <remarks>画像左端は微分が取れないため、プロファイルから除外されます。</remarks>
        public static List<int[]> DifferentialHorizontalProfiles(ImageContainer image, Rectangle roi)
        {
            if ((roi.Left < 0) || (roi.Top < 0))
            {
                throw new ArgumentOutOfRangeException(nameof(roi), "対象領域が画像の範囲外です。");
            }

            if ((roi.Height > image.Height) || (roi.Width > image.Width))
            {
                throw new ArgumentOutOfRangeException(nameof(roi), "対象領域が画像の大きさを超えています。");
            }

            int startingRow = 0;
            int startingColumn = 1;
            int targetWidth = image.Width - 1;
            int rowCount = image.Height;
            if (!roi.IsEmpty)
            {
                startingRow = roi.Top;
                startingColumn = (roi.Left == 0) ? 1 : roi.Left;
                targetWidth = (roi.Left == 0) ? (roi.Width - 1) : roi.Width;
                rowCount = roi.Height;
            }

            var differentialProfiles = new List<int[]>(rowCount);
            Func<byte[], int, uint> bitConverter = Bit.SelectConverter(image.BitDepth);
            for (int rows = startingRow; rows < (startingRow + rowCount); rows++)
            {
                int[] lineProfile = new int[targetWidth];
                for (int columns = startingColumn; columns < (startingColumn + targetWidth); columns++)
                {
                    // リトルエンディアン専用
                    int targetIndex = rows * image.Width + columns;
                    int currentPixel = (int)bitConverter(image.RawData.Pixels, targetIndex);
                    int previousPixel = (int)bitConverter(image.RawData.Pixels, targetIndex - 1);

                    lineProfile[columns - startingColumn] = currentPixel - previousPixel;
                }

                differentialProfiles.Add(lineProfile);
            }

            return differentialProfiles;
        }

        /// <summary>
        /// 画像データからEncircled Energy Profileを生成します。
        /// </summary>
        /// <param name="image">2次元画像データ</param>
        /// <param name="centroid">画像の重心位置</param>
        /// <param name="baseline">輝度基準値</param>
        /// <returns>Encircled Energy Profile計算結果(index:Pixel距離, value:輝度積算値)</returns>
        public static List<double> EncircledEnergyProfile(ImageComponent image, PointD centroid, double baseline = 0)
        {
            int bytesPerPixel = Bit.ToByte(image.BitDepth);
            if ((bytesPerPixel == 3) || (bytesPerPixel > 4))
            {
                throw new ArgumentOutOfRangeException(nameof(image), image.BitDepth, $"不正なビット深度です。({nameof(image.BitDepth)})");
            }

            // Encircled Energy Profile生成
            Func<byte[], int, uint> bitConverter = Bit.SelectConverter(image.BitDepth);
            var pixelTable = new List<(double distance, double luminance)>(image.Width * image.Height);
            double maxDistance = 0;
            for (int row = 0; row < image.Height; row++)
            {
                for (int column = 0; column < image.Width; column++)
                {
                    // 重心からの距離に対する輝度値を算出する
                    double x = column - centroid.X;
                    double y = centroid.Y - row;
                    double distance = Math.Sqrt((x * x) + (y * y));
                    if (distance > maxDistance)
                    {
                        maxDistance = distance;
                    }

                    double luminance = bitConverter(image.Pixels, (row * image.Width) + column) - baseline;
                    pixelTable.Add((distance, luminance));
                }
            }

            if (pixelTable.Count < 1)
            {
                return [];
            }

            // Pixel Pitchで輝度値を積分する。
            var profile = new double[(int)Math.Ceiling(maxDistance) + 1].ToList();
            for (int index = 0; index < pixelTable.Count; index++)
            {
                profile[(int)pixelTable[index].distance + 1] += pixelTable[index].luminance;
            }

            for (int index = 1; index < profile.Count; index++)
            {
                profile[index] += profile[index - 1];
            }

            // 上限1.0に正規化
            double sum = profile[profile.Count - 1];
            for (int index = 0; index < profile.Count; index++)
            {
                profile[index] /= sum;
            }

            return profile;
        }

        /// <summary>
        /// 画像データからEncircled Energy Profileを生成します。
        /// </summary>
        /// <param name="image">2次元画像データ</param>
        /// <param name="centroid">画像の重心位置</param>
        /// <returns>Encircled Energy Profile計算結果(index:Pixel距離, value:輝度積算値)</returns>
        public static List<double> EncircledEnergyProfile(ImageComponent<double> image, PointD centroid)
        {
            // Encircled Energy Profile生成
            var pixelTable = new List<(double distance, double luminance)>(image.Width * image.Height);
            double maxDistance = 0;
            for (int row = 0; row < image.Height; row++)
            {
                for (int column = 0; column < image.Width; column++)
                {
                    // 重心からの距離に対する輝度値を算出する
                    double x = column - centroid.X;
                    double y = centroid.Y - row;
                    double distance = Math.Sqrt((x * x) + (y * y));
                    if (distance > maxDistance)
                    {
                        maxDistance = distance;
                    }

                    pixelTable.Add((distance, image.Pixels[(row * image.Width) + column]));
                }
            }

            if (pixelTable.Count < 1)
            {
                return [];
            }

            // Pixel Pitchで輝度値を積分する。
            var profile = new double[(int)Math.Ceiling(maxDistance) + 1].ToList();
            if (profile.Count < 2)
            {
                return profile;
            }

            for (int index = 0; index < pixelTable.Count; index++)
            {
                profile[(int)pixelTable[index].distance + 1] += pixelTable[index].luminance;
            }

            for (int index = 1; index < profile.Count; index++)
            {
                profile[index] += profile[index - 1];
            }

            // 上限1.0に正規化
            double sum = profile[profile.Count - 1];
            for (int index = 0; index < profile.Count; index++)
            {
                profile[index] /= sum;
            }

            return profile;
        }
    }
}
