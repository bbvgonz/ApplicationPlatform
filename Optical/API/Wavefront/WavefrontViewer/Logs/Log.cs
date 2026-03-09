using Optical.API.Library;
using Optical.API.Wavefront;
using Optical.Platform.Types;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WavefrontTester.Logs
{
    public class Log
    {
        #region Fields
        private string[] resultHeaders;

        private ManualResetEventSlim isWriting;
        private ConcurrentQueue<WavefrontContainer> logQueue;
        private int writeCounter;
        #endregion // Fields

        #region Constructors
        public Log()
        {
            initializeParameter();
        }
        #endregion // Constructors

        #region Methods
        private void initializeParameter()
        {
            isWriting = new ManualResetEventSlim(false);
            logQueue = new ConcurrentQueue<WavefrontContainer>();

            resultHeaders = new string[]
            {
                "No.",
                "Date",
                "Time",
                "Product Name",
                "Serial No.",
                "Note",
                ",Tilt Mag.",
                "Tilt Ang.",
                "Defocus",
                "SA",
                "Coma Mag.",
                "Coma Ang.",
                "AS Mag.",
                "AS Ang.",
                ",Incident Angle X",
                "Incident Angle Y",
                "Incident Angle D",
                "Power",
                "RoC X",
                "RoC Y",
                "ρ(rho) X",
                "ρ(rho) Y",
                "PV",
                "RMS",
                "Strehl Ratio"
            };
        }

        private void outputImageFile(ImageComponent imageComponent, DateTime timeStamp, string filePath)
        {
            string imageFolder = Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath) + "_Images");
            if (!Directory.Exists(imageFolder))
            {
                Directory.CreateDirectory(imageFolder);
            }

            Bitmap image = ImageProcessing.RawToBitmap(imageComponent);
            image.Save(imageFolder + $"\\{timeStamp.ToString("yyMMdd_HHmmss_fff")}.png", ImageFormat.Png);
        }

        /// <summary>
        /// 測定結果ファイルを新規作成する
        /// </summary>
        public void CreateResultFile(string filePath, int zernikeIndex, int legendreDegree)
        {
            writeCounter = 1;

            using (var writer = new StreamWriter(filePath, false, Encoding.GetEncoding("utf-8")))
            {
                writer.Write(string.Join(",", resultHeaders));

                // Zernike
                var zernike = new StringBuilder(",,");
                for (int index = 1; index <= zernikeIndex; index++)
                {
                    zernike.Append($"Z{index},");
                }

                writer.Write(zernike);

                // Legendre
                var legendre = new StringBuilder(",");
                for (int n = 0; n <= legendreDegree; n++)
                {
                    for (int m = 0; m <= legendreDegree; m++)
                    {
                        legendre.Append($"\"L({m},{n})\",");
                    }
                }

                writer.WriteLine(legendre);
            }
        }

        /// <summary>
        /// 測定結果を外部ファイルに追記出力する。
        /// </summary>
        public void Write(string filePath, WavefrontContainer wavefront, LogComponents logCaption)
        {
            logQueue.Enqueue(new WavefrontContainer(wavefront));
            Task.Run(() =>
            {
                if (isWriting.Wait(0))
                {
                    return;
                }

                isWriting.Set();

                while (logQueue.TryDequeue(out WavefrontContainer result))
                {
                    int logLength = 10 + resultHeaders.Length + result.Zernike.Length + result.Legendre.Length;
                    var line = new List<string>(logLength);
                    line.Add(writeCounter++.ToString());
                    DateTime now = DateTime.Now;
                    line.Add(now.ToString("yyyy/MM/dd"));
                    line.Add(now.ToString("HH:mm:ss"));
                    line.Add(logCaption.Product);
                    line.Add(logCaption.Serial);
                    line.Add(logCaption.Note);

                    line.Add("");

                    // Seidel
                    line.Add(result.Seidel.Tilt.Rho.ToString("f15"));
                    line.Add(result.Seidel.Tilt.Theta.ToString("f15"));
                    line.Add(result.Seidel.Defocus.ToString("f15"));
                    line.Add(result.Seidel.SphericalAberration.ToString("f15"));
                    line.Add(result.Seidel.Coma.Rho.ToString("f15"));
                    line.Add(result.Seidel.Coma.Rho.ToString("f15"));
                    line.Add(result.Seidel.Astigmatism.Rho.ToString("f15"));
                    line.Add(result.Seidel.Astigmatism.Rho.ToString("f15"));

                    line.Add("");

                    // Total
                    line.Add(result.Total.IncidentAngle.X.ToString("f15"));
                    line.Add(result.Total.IncidentAngle.Y.ToString("f15"));
                    line.Add(result.Total.IncidentAngle.Synthetic.ToString("f15"));
                    line.Add(result.Total.Power.ToString("f15"));
                    line.Add(result.Total.RoCX.ToString("f15"));
                    line.Add(result.Total.RoCY.ToString("f15"));
                    line.Add(result.Total.RhoX.ToString("f15"));
                    line.Add(result.Total.RhoY.ToString("f15"));
                    line.Add(result.Total.PV.ToString("f15"));
                    line.Add(result.Total.RMS.ToString("f15"));
                    line.Add(result.Total.StrehlRatio.ToString("f15"));

                    line.Add("");

                    for (int index = 1; index < result.Zernike.Length; index++)
                    {
                        line.Add(result.Zernike[index].ToString("F15"));
                    }

                    line.Add("");

                    for (int index = 0; index < result.Legendre.Length; index++)
                    {
                        line.Add(result.Legendre[index].ToString("F15"));
                    }

                    using (var streamWriter = new StreamWriter(filePath, true, Encoding.UTF8))
                    {
                        streamWriter.WriteLine(string.Join(",", line));
                    }
                }

                isWriting.Reset();
            });
        }
        #endregion // Methods

        #region Classes
        public struct LogComponents
        {
            public string Product { get; set; }
            public string Serial { get; set; }
            public string Note { get; set; }
        }
        #endregion // Classes
    }
}
