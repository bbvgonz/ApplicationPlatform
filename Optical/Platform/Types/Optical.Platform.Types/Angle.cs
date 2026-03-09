using System;
using System.ComponentModel;

namespace Optical.Platform.Types
{
    /// <summary>
    /// 2次元平面で定義する角度の倍精度浮動小数点のx軸成分とy軸成分の順序付けられたペアを表します。
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class AngleD
    {
        #region Constructors
        /// <summary>
        /// <see cref="AngleD"/>クラスの新しいインスタンスを生成します。
        /// </summary>
        public AngleD() { }

        /// <summary>
        /// 指定された角度で、<see cref="AngleD"/>構造体の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="x">新しい<see cref="AngleD"/>構造体の<paramref name="x"/>軸成分</param>
        /// <param name="y">新しい<see cref="AngleD"/>構造体の<paramref name="y"/>軸成分</param>
        public AngleD(double x, double y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// 指定された<see cref="AngleD"/>構造体で、<see cref="AngleD"/>の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="angle">新しい<see cref="AngleD"/>構造体を生成するための<see cref="AngleD"/>構造体</param>
        public AngleD(AngleD angle)
        {
            X = angle.X;
            Y = angle.Y;
        }
        #endregion // Constructors

        #region Properties
        /// <summary>
        /// この<see cref="AngleD"/>のx軸成分を取得または設定します。
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// この<see cref="AngleD"/>のy軸成分を取得または設定します。
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// <see cref="X"/>と<see cref="Y"/>の合成角度を取得します。
        /// </summary>
        public double Synthetic => Math.Sqrt((X * X) + (Y * Y));
        #endregion // Properties

        #region Methods
        /// <summary>
        /// すべての要素を初期化します。
        /// </summary>
        public void Clear()
        {
            X = 0;
            Y = 0;
        }

        /// <summary>
        /// この<see cref="AngleD"/>の属性をラジアンに変換します。
        /// </summary>
        /// <returns>この<see cref="AngleD"/>構造体の<see cref="X"/>軸成分と<see cref="Y"/>軸成分をラジアンに変換した、新しい<see cref="AngleD"/>のインスタンス</returns>
        public AngleD ToRadian()
        {
            return new AngleD((X * Math.PI / 180), (Y * Math.PI / 180));
        }

        /// <summary>
        /// この<see cref="AngleD"/>の属性を度に変換します。
        /// </summary>
        /// <returns>この<see cref="AngleD"/>構造体の<see cref="X"/>軸成分と<see cref="Y"/>軸成分を度に変換した、新しい<see cref="AngleD"/>のインスタンス</returns>
        public AngleD ToDegree()
        {
            return new AngleD((X * 180 / Math.PI), (Y * 180 / Math.PI));
        }
        #endregion // Methods
    }

    /// <summary>
    /// 2次元平面で定義する角度の倍精度浮動小数点のx軸成分とy軸成分の順序付けられたペアを表します。
    /// </summary>
    /// <typeparam name="TNumeric">数値型</typeparam>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class Angle<TNumeric> where TNumeric : struct
    {
        #region Constructors
        /// <summary>
        /// <see cref="AngleD"/>クラスの新しいインスタンスを生成します。
        /// </summary>
        public Angle() { }

        /// <summary>
        /// 指定された角度で、<see cref="Angle{TNumeric}"/>構造体の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="x">新しい<see cref="Angle{TNumeric}"/>構造体の<paramref name="x"/>軸成分</param>
        /// <param name="y">新しい<see cref="Angle{TNumeric}"/>構造体の<paramref name="y"/>軸成分</param>
        public Angle(TNumeric x, TNumeric y)
        {
            X = x;
            Y = y;
        }
        #endregion // Constructors

        #region Properties
        /// <summary>
        /// この<see cref="AngleD"/>のx軸成分を取得または設定します。
        /// </summary>
        public TNumeric X { get; set; }

        /// <summary>
        /// この<see cref="AngleD"/>のy軸成分を取得または設定します。
        /// </summary>
        public TNumeric Y { get; set; }

        /// <summary>
        /// <see cref="X"/>と<see cref="Y"/>の合成角度を取得します。
        /// </summary>
        public TNumeric Synthetic => Math.Sqrt(((dynamic)X * X) + ((dynamic)Y * Y));
        #endregion // Properties

        #region Methods
        /// <summary>
        /// この<see cref="AngleD"/>の属性をラジアンに変換します。
        /// </summary>
        /// <returns>この<see cref="AngleD"/>構造体の<see cref="X"/>軸成分と<see cref="Y"/>軸成分をラジアンに変換した、新しい<see cref="AngleD"/>構造体のインスタンス</returns>
        public AngleD ToRadian()
        {
            return new AngleD(((dynamic)X * Math.PI / 180), ((dynamic)Y * Math.PI / 180));
        }

        /// <summary>
        /// この<see cref="AngleD"/>の属性を度に変換します。
        /// </summary>
        /// <returns>この<see cref="AngleD"/>構造体の<see cref="X"/>軸成分と<see cref="Y"/>軸成分を度に変換した、新しい<see cref="AngleD"/>構造体のインスタンス</returns>
        public AngleD ToDegree()
        {
            return new AngleD(((dynamic)X * 180 / Math.PI), ((dynamic)Y * 180 / Math.PI));
        }
        #endregion // Methods
    }
}
