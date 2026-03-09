using Optical.API.Library;
using Optical.API.Library.Optics;
using Optical.API.Wavefront;
using Optical.Platform.Backup;
using Optical.Platform.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WavefrontTester
{
    public class WavefrontCalibration : IDisposable
    {
        #region Fields
        private static readonly string configKey = "WavefrontTester.Calibration.userCalibrationData";

        private ConfigFile configFile;
        private CalibrationComponents userCalibrationData;
        private IWavefront wavefront;
        #endregion // Fields

        #region Constructors
        public WavefrontCalibration(IWavefront wavefront)
        {
            this.wavefront = wavefront;

            initializeConfigFile();
        }
        #endregion // Constructors

        #region Methods
        private CalibrationComponents generateCalibrationComponentsBase(string deviceId)
        {
            var calibrationComponents = new CalibrationComponents()
            {
                DeviceId = deviceId,
                MicroLensArrayFocalLength = wavefront.MicroLensArrayFocalLength,
                MicroLensArrayPitch = wavefront.MicroLensArrayPitch,
                OpticalMagnification = wavefront.OpticalMagnification,
                PixelPitch = wavefront.PixelPitch,
                PropagationDistance = wavefront.PropagationDistance,
                Wavelength = wavefront.Wavelength
            };
            calibrationComponents.SensorSize.Width = wavefront.SensorSize.Width;
            calibrationComponents.SensorSize.Height = wavefront.SensorSize.Height;
            calibrationComponents.SystemId = string.Empty;

            return calibrationComponents;
        }

        private List<PointD> calculateDefaultReferencePoints(Size<int> sensorSize)
        {
            double microLensArrayPitchPixels = wavefront.MicroLensArrayPitch / wavefront.PixelPitch;
            int rows = ((int)(sensorSize.Height / microLensArrayPitchPixels) + 1) / 2 * 2 + 1;
            int columns = ((int)(sensorSize.Width / microLensArrayPitchPixels) + 1) / 2 * 2 + 1;
            int diagonalCount = ((int)(Math.Sqrt(2.0) * Math.Max(rows, columns))) / 2 * 2 + 1;

            double diagonalMax = microLensArrayPitchPixels * (diagonalCount - 1) / 2.0;
            double diagonalMin = -diagonalMax;

            double endX = microLensArrayPitchPixels * (columns / 2.0);
            double startX = -endX;
            double endY = microLensArrayPitchPixels * (rows / 2.0);
            double startY = -endY;

            var referencePoints = new List<PointD>();
            double x;
            double y;
            double rotatedX;
            double rotatedY;
            double rotateAngle = 45.0 * (Math.PI / 180.0);
            for (int i = 0; i < diagonalCount; i++)
            {
                y = (diagonalMin * (diagonalCount - 1 - i) + diagonalMax * i) / (diagonalCount - 1);
                for (int j = 0; j < diagonalCount; j++)
                {
                    x = (diagonalMin * (diagonalCount - 1 - j) + diagonalMax * j) / (diagonalCount - 1);
                    rotatedX = Math.Cos(rotateAngle) * x + Math.Sin(rotateAngle) * y;
                    if ((rotatedX >= startX) && (rotatedX <= endX))
                    {
                        rotatedY = -Math.Sin(rotateAngle) * x + Math.Cos(rotateAngle) * y;
                        if ((rotatedY >= startY) && (rotatedY <= endY))
                        {
                            PointD imageCoordinate = convertToImageCoordinate(rotatedX, rotatedY, sensorSize);
                            referencePoints.Add(imageCoordinate);
                        }
                    }
                }
            }

            return referencePoints;
        }

        private List<PointD> calculateUserReferencePoints(ImageComponent image, int noiseLevel)
        {
            // ラベリング
            ImageComponent noiseCutImage = ImageProcessing.ToZero(image, noiseLevel);
            ImageComponent binaryImage = ImageProcessing.Binarize(noiseCutImage, noiseLevel);
            ImageProcessing.LabelingContainer labelingStats = ImageProcessing.LabelingStats(binaryImage);

            // 輝度重心
            var referencePoints = new List<PointD>(labelingStats.Components.Length);
            for (int index = 1; index < labelingStats.Components.Length; index++)
            {
                if ((labelingStats[index].Area > wavefront.SpotSizeRange.Maximum) ||
                    (labelingStats[index].Area < wavefront.SpotSizeRange.Minimum))
                {
                    continue;
                }

                ImageComponent trimImage = ImageProcessing.Trim(noiseCutImage, labelingStats[index].Circumscribed, ApertureShape.Rectangle);
                ImageProcessing.ImageMoments moments = ImageProcessing.Moment(trimImage);
                referencePoints.Add(new PointD((moments.Raw10 / moments.Raw00) + labelingStats[index].Circumscribed.X,
                                               (moments.Raw01 / moments.Raw00) + labelingStats[index].Circumscribed.Y));
            }

            return referencePoints.OrderBy(point => point.DistanceTo(image.Width / 2, image.Height / 2)).ToList();
        }

        private PointD convertToImageCoordinate(double x, double y, Size<int> sensorSize)
        {
            return new PointD(x + (sensorSize.Width / 2.0), -y + (sensorSize.Height / 2.0));
        }

        private void initializeConfigFile()
        {
            userCalibrationData = new CalibrationComponents();
            configFile = new ConfigFile();
            configFile.Register(configKey, userCalibrationData);
        }

        private void updateUserCalibrationData(CalibrationComponents calibrationData)
        {
            userCalibrationData.DeviceId = calibrationData.DeviceId;
            userCalibrationData.MicroLensArrayFocalLength = calibrationData.MicroLensArrayFocalLength;
            userCalibrationData.MicroLensArrayPitch = calibrationData.MicroLensArrayPitch;
            userCalibrationData.OpticalMagnification = calibrationData.OpticalMagnification;
            userCalibrationData.PixelPitch = calibrationData.PixelPitch;
            userCalibrationData.PropagationDistance = calibrationData.PropagationDistance;
            userCalibrationData.ReferencePoints = new List<PointD>(calibrationData.ReferencePoints.Count);
            for (int index = 0; index < calibrationData.ReferencePoints.Count; index++)
            {
                userCalibrationData.ReferencePoints.Add(new PointD(calibrationData.ReferencePoints[index]));
            }

            userCalibrationData.SensorSize.Width = calibrationData.SensorSize.Width;
            userCalibrationData.SensorSize.Height = calibrationData.SensorSize.Height;
            userCalibrationData.SystemId = calibrationData.SystemId;
            userCalibrationData.Wavelength = calibrationData.Wavelength;
        }

        public CalibrationComponents GenerateDefault(Size<int> sensorSize, string deviceId)
        {
            CalibrationComponents calibrationComponents = generateCalibrationComponentsBase(deviceId);
            calibrationComponents.ReferencePoints = calculateDefaultReferencePoints(sensorSize);

            return calibrationComponents;
        }

        public CalibrationComponents Generate(ImageComponent imageComponent, int noiseLevel, string deviceId)
        {
            CalibrationComponents calibrationComponents = generateCalibrationComponentsBase(deviceId);
            calibrationComponents.ReferencePoints = calculateUserReferencePoints(imageComponent, noiseLevel);

            return calibrationComponents;
        }

        public CalibrationComponents Load(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return null;
            }

            configFile.Load(filePath);
            CalibrationComponents loadedData = configFile.Refer<CalibrationComponents>(configKey);

            updateUserCalibrationData(loadedData);

            return userCalibrationData;
        }

        public void Save(CalibrationComponents calibrationComponents, string calibrationFilePath)
        {
            string directoryName = Path.GetDirectoryName(calibrationFilePath);
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }

            updateUserCalibrationData(calibrationComponents);

            configFile.Save(calibrationFilePath);
        }

        #region IDisposable Support
        private bool disposedValue = false; // 重複する呼び出しを検出するには

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: マネージド状態を破棄します (マネージド オブジェクト)。
                    wavefront = null;
                }

                // TODO: アンマネージド リソース (アンマネージド オブジェクト) を解放し、下のファイナライザーをオーバーライドします。
                // TODO: 大きなフィールドを null に設定します。

                disposedValue = true;
            }
        }

        // TODO: 上の Dispose(bool disposing) にアンマネージド リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします。
        // ~WavefrontCalibration() {
        //   // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
        //   Dispose(false);
        // }

        // このコードは、破棄可能なパターンを正しく実装できるように追加されました。
        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
            Dispose(true);
            // TODO: 上のファイナライザーがオーバーライドされる場合は、次の行のコメントを解除してください。
            // GC.SuppressFinalize(this);
        }
        #endregion
        #endregion // Methods
    }
}
