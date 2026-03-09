using System;
using System.ComponentModel;

namespace Optical.Platform.Types
{
    /// <summary>
    /// 2次元平面内の点を定義する浮動小数点のx座標とy座標の順序付けられたペアを表します。
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class BindingPointD : PointD, INotifyPropertyChanged
    {
        #region Fields
        /// <summary>
        /// 点の水平位置
        /// </summary>
        private double x;

        /// <summary>
        /// 点の垂直位置
        /// </summary>
        private double y;
        #endregion // Fields

        #region Constructors
        /// <summary>
        /// 新しいインスタンスを初期化します。
        /// </summary>
        public BindingPointD() { }

        /// <summary>
        /// 指定された座標で、<see cref="BindingPointD"/>構造体の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="x">点の水平位置</param>
        /// <param name="y">点の垂直位置</param>
		public BindingPointD(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// 指定された<see cref="BindingPointD"/>構造体で、<see cref="BindingPointD"/>構造体の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="point">点の2次元座標</param>
		public BindingPointD(BindingPointD point)
        {
            x = point.x;
            y = point.y;
        }

        /// <summary>
        /// 指定された<see cref="PointD"/>構造体で、<see cref="BindingPointD"/>構造体の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="point">点の2次元座標</param>
        public BindingPointD(PointD point)
        {
            x = point.X;
            y = point.Y;
        }
        #endregion // Constructors

        #region Events
        /// <summary>
        /// このオブジェクトのプロパティが変更されるたびに通知されます。
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion // Events

        #region Properties
        /// <summary>
        /// この<see cref="BindingPointD"/>のx座標を取得または設定します。
        /// </summary>
        public new double X
        {
            get => x;

            set
            {
                x = value;
                OnPropertyChanged(nameof(X));
            }
        }
        /// <summary>
        /// この<see cref="BindingPointD"/>のy座標を取得または設定します。
        /// </summary>
        public new double Y
        {
            get => y;

            set
            {
                y = value;
                OnPropertyChanged(nameof(Y));
            }
        }
        #endregion // Properties

        #region Methods
        /// <summary>
        /// プロパティの変更通知
        /// </summary>
        /// <param name="name">プロパティ名称</param>
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        /// <summary>
        /// 新しい2次元座標を設定します。
        /// </summary>
        /// <param name="x">点の水平位置</param>
        /// <param name="y">点の垂直位置</param>
        public new void XY(double x, double y)
        {
            this.x = x;
            this.y = y;
            OnPropertyChanged(string.Empty);
        }

        /// <summary>
        /// 新しい2次元座標を設定します。
        /// </summary>
        /// <param name="point">点の2次元座標</param>
        public void XY(BindingPointD point)
        {
            x = point.x;
            y = point.y;
            OnPropertyChanged(string.Empty);
        }

        /// <summary>
        /// 新しい2次元座標を設定します。
        /// </summary>
        /// <param name="point">点の2次元座標</param>
        public new void XY(PointD point)
        {
            x = point.X;
            y = point.Y;
            OnPropertyChanged(string.Empty);
        }
        #endregion // Methods
    }

    /// <summary>
    /// 2次元平面内の点を定義する浮動小数点のx座標とy座標の順序付けられたペアを表します。
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class PointD
    {
        #region Fields
        private static PointD empty = new PointD();
        #endregion // Fields

        #region Constructors
        /// <summary>
        /// <see cref="PointD"/>クラスの新しいインスタンスを生成します。
        /// </summary>
        public PointD() { }

        /// <summary>
        /// 指定された座標で、<see cref="PointD"/>クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="x">点の水平位置</param>
        /// <param name="y">点の垂直位置</param>
		public PointD(double x, double y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="point">座標</param>
        public PointD(PointD point)
        {
            X = point?.X ?? 0;
            Y = point?.Y ?? 0;
        }
        #endregion // Constructors

        #region Properties
        /// <summary>
        /// プロパティが初期化されていない状態の<see cref="PointD"/>クラスを表します。
        /// </summary>
        public static PointD Empty => empty;

        /// <summary>
        /// 原点からの距離
        /// </summary>
        public double Distance => Math.Sqrt((X * X) + (Y * Y));

        /// <summary>
        /// この<see cref="PointD"/>が空かどうかを示す値を取得します。
        /// </summary>
        /// <remarks>この<see cref="PointD"/>が<see cref="Empty"/>と等しい場合は<see langword="true"/>。それ以外の場合は<see langword="false"/>。</remarks>
        public bool IsEmpty => this == empty;

        /// <summary>
        /// この<see cref="PointD"/>のx座標を取得または設定します。
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// この<see cref="PointD"/>のy座標を取得または設定します。
        /// </summary>
        public double Y { get; set; }
        #endregion // Properties

        #region Methods
        /// <summary>
        /// 座標を初期化します。
        /// </summary>
        public void Clear()
        {
            X = 0;
            Y = 0;
        }

        /// <summary>
        /// 指定された座標までの距離を算出します。
        /// </summary>
        /// <param name="x">対象x座標</param>
        /// <param name="y">対象y座標</param>
        /// <returns>指定された座標までの距離。</returns>
        public double DistanceTo(double x, double y)
        {
            double deltaX = x - X;
            double deltaY = y - Y;
            return Math.Sqrt((deltaX * deltaX) + (deltaY * deltaY));
        }

        /// <summary>
        /// 指定された座標までの距離を算出します。
        /// </summary>
        /// <param name="point">点の2次元座標</param>
        /// <returns>指定された座標までの距離。</returns>
        public double DistanceTo(PointD point)
        {
            double deltaX = point.X - X;
            double deltaY = point.Y - Y;
            return Math.Sqrt((deltaX * deltaX) + (deltaY * deltaY));
        }

        /// <summary>
        /// 新しい2次元座標を設定します。
        /// </summary>
        /// <param name="x">点の水平位置</param>
        /// <param name="y">点の垂直位置</param>
        public void XY(double x, double y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// 新しい2次元座標を設定します。
        /// </summary>
        /// <param name="point">点の2次元座標</param>
        public void XY(PointD point)
        {
            X = point.X;
            Y = point.Y;
        }
        #endregion // Methods
    }

    /// <summary>
    /// 2次元平面内の点を定義するx座標とy座標の順序付けられたペアを表します。
    /// </summary>
    /// <typeparam name="TNumeric">数値型</typeparam>
    public class Point<TNumeric> where TNumeric : struct
    {
        #region Constructors
        /// <summary>
        /// 指定された座標で、<see cref="Point{TNumeric}"/>クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="x">点の水平位置</param>
        /// <param name="y">点の垂直位置</param>
        public Point(TNumeric x, TNumeric y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="point">座標</param>
        public Point(Point<TNumeric> point)
        {
            X = point?.X ?? (dynamic)0;
            Y = point?.Y ?? (dynamic)0;
        }
        #endregion // Constructors

        #region Properties
        /// <summary>
        /// プロパティが初期化されていない状態の<see cref="PointD"/>クラスを表します。
        /// </summary>
        public static Point<TNumeric> Empty { get; } = new Point<TNumeric>((dynamic)0, (dynamic)0);

        /// <summary>
        /// 原点からの距離
        /// </summary>
        public TNumeric Distance => Math.Sqrt(((dynamic)X * X) + ((dynamic)Y * Y));

        /// <summary>
        /// この<see cref="Point{TNumeric}"/>が空かどうかを示す値を取得します。
        /// </summary>
        /// <remarks>この<see cref="Point{TNumeric}"/>が<see cref="Empty"/>と等しい場合は<see langword="true"/>。それ以外の場合は<see langword="false"/>。</remarks>
        public bool IsEmpty => this == Empty;

        /// <summary>
        /// この<see cref="Point{TNumeric}"/>のx軸成分を取得または設定します。
        /// </summary>
        public TNumeric X { get; set; }

        /// <summary>
        /// この<see cref="Point{TNumeric}"/>のy軸成分を取得または設定します。
        /// </summary>
        public TNumeric Y { get; set; }
        #endregion // Properties

        #region Methods
        /// <summary>
        /// 指定されたポイントが <see langword="null"/> または<see cref="X"/> と <see cref="Y"/> の両方が 0 であるかどうかを示します。
        /// </summary>
        /// <param name="point">テストするポイント</param>
        /// <returns><see cref="Point{TNumeric}"/> パラメーターが <see langword="null"/> または <see cref="X"/> と <see cref="Y"/> の両方が 0 の場合は <see langword="true"/>。それ以外の場合は <see langword="false"/>。</returns>
        public static bool IsNullOrEmpty(Point<TNumeric> point)
        {
            if (point == null)
            {
                return true;
            }

            if (((dynamic)point.X == 0) && ((dynamic)point.Y == 0))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 新しい2次元座標を設定します。
        /// </summary>
        /// <param name="x">点の水平位置</param>
        /// <param name="y">点の垂直位置</param>
        public void XY(TNumeric x, TNumeric y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// 指定された座標までの距離を算出します。
        /// </summary>
        /// <param name="x">対象x座標</param>
        /// <param name="y">対象y座標</param>
        /// <returns>指定された座標までの距離。</returns>
        public double DistanceTo(TNumeric x, TNumeric y)
        {
            double deltaX = (dynamic)x - X;
            double deltaY = (dynamic)y - Y;
            return Math.Sqrt((deltaX * deltaX) + (deltaY * deltaY));
        }
        #endregion // Methods
    }
}
