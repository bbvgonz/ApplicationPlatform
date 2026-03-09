using Optical.Platform.Mathematics;
using Optical.Platform.Types;
using System;
using System.ComponentModel;

namespace Optical.API.Wavefront
{
    /// <summary>
    /// ザイデル収差格納クラス
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class SeidelAberration
    {
        #region Constructors
        /// <summary>
        /// ザイデル収差の測定結果を格納するための新しいインスタンスを生成します。
        /// </summary>
        public SeidelAberration()
        {
            initialize();
        }

        /// <summary>
        /// ザイデル収差の測定結果を格納するための新しいインスタンスを生成します。
        /// </summary>
        /// <param name="zernike">フリンジ・ゼルニケ係数（[index] = フリンジ指数）</param>
        public SeidelAberration(double[] zernike)
        {
            initialize();

            compute(zernike);
        }
        #endregion // Constructors

        #region Properties
        /// <summary>
        /// 非点収差
        /// </summary>
        /// <remarks>大きさ：[um], 偏角：[deg]</remarks>
        public Circular Astigmatism { get; set; }

        /// <summary>
        /// コマ収差
        /// </summary>
        /// <remarks>大きさ：[um], 偏角：[deg]</remarks>
        public Circular Coma { get; set; }

        /// <summary>
        /// 焦点ぼけ
        /// </summary>
        public double Defocus { get; set; }

        /// <summary>
        /// 球面収差
        /// </summary>
        public double SphericalAberration { get; set; }

        /// <summary>
        /// 波面傾き
        /// </summary>
        /// <remarks>大きさ：[deg], 偏角：[deg]</remarks>
        public Circular Tilt { get; set; }
        #endregion // Properties

        #region Methods
        private void initialize()
        {
            Tilt = new Circular();
            Coma = new Circular();
            Astigmatism = new Circular();
        }

        private void compute(double[] zernike)
        {
            if (zernike.Length < 9)
            {
                return;
            }

            double x = zernike[2] - (2 * zernike[7]);
            double y = zernike[3] - (2 * zernike[8]);
            Tilt.Rho = Math.Sqrt((x * x) + (y * y));
            Tilt.Theta = Unit.ToDegree(Math.Atan(y / x));

            double AS = Math.Sqrt((zernike[5] * zernike[5]) + (zernike[6] * zernike[6]));
            double plus = (2 * zernike[4]) - (6 * zernike[9]) + AS;
            double minus = (2 * zernike[4]) - (6 * zernike[9]) - AS;
            if (Math.Abs(plus) > Math.Abs(minus))
            {
                Defocus = minus;
                Astigmatism.Rho = 2 * AS;
            }
            else
            {
                Defocus = plus;
                Astigmatism.Rho = -2 * AS;
            }

            Astigmatism.Theta = Unit.ToDegree(Math.Atan(zernike[6] / zernike[5]) / 2);

            SphericalAberration = 6 * zernike[9];

            Coma.Rho = 3 * Math.Sqrt((zernike[7] * zernike[7]) + (zernike[8] * zernike[8]));
            Coma.Theta = Unit.ToDegree(Math.Atan(zernike[8] / zernike[7]));
        }

        /// <summary>
        /// ザイデル収差を初期化します。
        /// </summary>
        public void Clear()
        {
            Tilt.Clear();
            Defocus = 0;
            SphericalAberration = 0;
            Coma.Clear();
            Astigmatism.Clear();
        }

        /// <summary>
        /// 指定されたゼルニケ係数からザイデル収差を算出する。
        /// </summary>
        /// <param name="zernike">フリンジ・ゼルニケ係数（[index] = フリンジ指数）</param>
        public void Compute(double[] zernike)
        {
            compute(zernike);
        }
        #endregion // Methods
    }
}
