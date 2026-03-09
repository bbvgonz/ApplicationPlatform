using Optical.Platform.Mathematics;
using Optical.Platform.Types;
using System;
using System.ComponentModel;

namespace Optical.API.Wavefront
{
    /// <summary>
    /// 総合波面収差格納クラス
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class TotalAberration
    {
        #region Constructors
        /// <summary>
        /// 総合波面収差の測定結果を格納するための新しいインスタンスを生成します。
        /// </summary>
        public TotalAberration()
        {
            initialize();
        }

        /// <summary>
        /// 総合波面収差の測定結果を格納するための新しいインスタンスを生成します。
        /// </summary>
        /// <param name="zernike">フリンジ・ゼルニケ係数（[index] = フリンジ指数）</param>
        /// <param name="apertureSize">開口径[um]</param>
        /// <param name="wavelength">光源波長[um]</param>
        public TotalAberration(double[] zernike, Size<double> apertureSize, double wavelength)
        {
            if (zernike == null)
            {
                throw new ArgumentNullException(nameof(zernike));
            }

            if (apertureSize == null)
            {
                throw new ArgumentNullException(nameof(apertureSize));
            }

            initialize();

            compute(zernike, apertureSize, wavelength);
        }
        #endregion // Constructors

        #region Properties
        /// <summary>
        /// 入射角[deg]
        /// </summary>
        public AngleD IncidentAngle { get; set; }

        /// <summary>
        /// 屈折力
        /// </summary>
        public double Power { get; set; }

        /// <summary>
        /// Peak to Valley[um]
        /// </summary>
        public double PV { get; set; }

        /// <summary>
        /// 水平方向曲率
        /// </summary>
        public double RhoX { get; set; }

        /// <summary>
        /// 垂直方向曲率
        /// </summary>
        public double RhoY { get; set; }

        /// <summary>
        /// Root Mean Square
        /// </summary>
        public double RMS { get; set; }

        /// <summary>
        /// 水平方向曲率半径（radius of curvature）[m]
        /// </summary>
        public double RoCX { get; set; }

        /// <summary>
        /// 垂直方向曲率半径（radius of curvature）[m]
        /// </summary>
        public double RoCY { get; set; }

        /// <summary>
        /// ストレール比
        /// </summary>
        public double StrehlRatio { get; set; }
        #endregion // Properties

        #region Methods
        private void initialize()
        {
            IncidentAngle = new AngleD();
        }

        private void compute(double[] zernike, Size<double> aperture, double wavelength)
        {
            if (zernike.Length < 4)
            {
                return;
            }

            double radiusWidth = (aperture.Width / 2);
            double radiusHeight = (aperture.Height / 2);
            IncidentAngle.X = Math.Atan(zernike[2] / radiusWidth);
            IncidentAngle.Y = Math.Atan(zernike[3] / radiusHeight);

            double power = 2 * zernike[4];
            Power = power;

            double rocX = ((power * power) + (radiusWidth * radiusWidth)) / (2 * power);
            RoCX = Unit.MiliToStandard(Unit.MicroToMili(rocX));
            RhoX = 1 / RoCX;

            double rocY = ((power * power) + (radiusHeight * radiusHeight)) / (2 * power);
            RoCY = Unit.MiliToStandard(Unit.MicroToMili(rocY));
            RhoY = 1 / RoCY;

            double pvZernike = 0;
            for (int index = 2; index < zernike.Length; index++)
            {
                pvZernike += (zernike[index] * zernike[index]);
            }

            PV = 2 * Math.Sqrt(pvZernike);

            double rmsZernike = 0;
            for (int index = 2; index < zernike.Length; index++)
            {
                rmsZernike += (ZernikeRMS.Coefficients[index] * ZernikeRMS.Coefficients[index] * zernike[index] * zernike[index]);
            }

            double rms = Math.Sqrt(rmsZernike);
            RMS = rms;

            double strehl = 2 * Math.PI * rms / wavelength;
            StrehlRatio = 1 - (strehl * strehl);
        }

        /// <summary>
        /// 総合波面収差を初期化します。
        /// </summary>
        public void Clear()
        {
            IncidentAngle.Clear();
            Power = 0;
            PV = 0;
            RhoX = 0;
            RhoY = 0;
            RMS = 0;
            RoCX = 0;
            RoCY = 0;
            StrehlRatio = 0;
        }

        /// <summary>
        /// 指定されたゼルニケ係数から総合波面収差を算出する。
        /// </summary>
        /// <param name="zernike">フリンジ・ゼルニケ係数（[index] = フリンジ指数）</param>
        /// <param name="apertureSize">開口径[um]</param>
        /// <param name="wavelength">光源波長[um]</param>
        public void Compute(double[] zernike, Size<double> apertureSize, double wavelength)
        {
            if (zernike == null)
            {
                throw new ArgumentNullException(nameof(zernike));
            }

            if (Size<double>.IsNullOrEmpty(apertureSize))
            {
                throw new ArgumentNullException(nameof(apertureSize));
            }

            if (wavelength <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(wavelength));
            }

            compute(zernike, apertureSize, wavelength);
        }
        #endregion // Methods
    }
}
