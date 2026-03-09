using System;

namespace Optical.API.Wavefront
{
    internal sealed class ZernikeRMS
    {
        #region Fields
        private static readonly double[] coefficients =
        {
            0,
            1,
            0.5,
            0.5,
            1 / Math.Sqrt(3),
            1 / Math.Sqrt(6),
            1 / Math.Sqrt(6),
            1 / Math.Sqrt(8),
            1 / Math.Sqrt(8),
            1 / Math.Sqrt(5),
            1 / Math.Sqrt(8),
            1 / Math.Sqrt(8),
            1 / Math.Sqrt(10),
            1 / Math.Sqrt(10),
            1 / Math.Sqrt(12),
            1 / Math.Sqrt(12),
            1 / Math.Sqrt(7),
            1 / Math.Sqrt(10),
            1 / Math.Sqrt(10),
            1 / Math.Sqrt(12),
            1 / Math.Sqrt(12),
            1 / Math.Sqrt(14),
            1 / Math.Sqrt(14),
            0.25,
            0.25,
            1 / 3,
            1 / Math.Sqrt(12),
            1 / Math.Sqrt(12),
            1 / Math.Sqrt(14),
            1 / Math.Sqrt(14),
            0.25,
            0.25,
            1 / Math.Sqrt(18),
            1 / Math.Sqrt(18),
            1 / Math.Sqrt(20),
            1 / Math.Sqrt(20),
            1 / Math.Sqrt(11),
        };
        #endregion // Fields

        #region Constructors
        private ZernikeRMS() { }

        static ZernikeRMS()
        {
            Coefficients = new ZernikeRMS();
        }
        #endregion // Constructors

        #region Properties
        public static ZernikeRMS Coefficients { get; }
        #endregion // Properties

        #region Indexers
        public double this[int index]
        {
            get
            {
                if ((index < 0) || (index > 36))
                {
                    throw new IndexOutOfRangeException($"{nameof(index)}:{index}");
                }

                return coefficients[index];
            }
        }
        #endregion // Indexers
    }
}
