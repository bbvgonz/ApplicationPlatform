using System;
using System.ComponentModel;

namespace Optical.Platform.Types
{
    /// <summary>
    /// 順序付けられた数値のペア（通常は長方形の幅と高さ）を格納します。
    /// </summary>
    /// <typeparam name="TNumeric">数値型</typeparam>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class Size<TNumeric> where TNumeric : struct
    {
        #region Constructors
        /// <summary>
        /// <see cref="Size{TNumeric}"/>クラスの新しいインスタンスを生成します。
        /// </summary>
        public Size() { }

        /// <summary>
        /// 指定された寸法で、<see cref="Size{T}"/>構造体の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="width">新しい<see cref="Size{T}"/>構造体の横幅成分</param>
        /// <param name="height">新しい<see cref="Size{T}"/>構造体の高さ成分</param>
        public Size(TNumeric width, TNumeric height)
        {
            Width = width;
            Height = height;
        }
        #endregion // Constructors

        #region Properties
        /// <summary>
        /// 指定されたサイズが <see langword="null"/> または幅および高さが 0 であるかどうかを示します。
        /// </summary>
        /// <param name="size">テストするサイズ</param>
        /// <returns><see cref="Size{TNumeric}"/> パラメーターが <see langword="null"/> または幅および高さが 0 の場合は true。それ以外の場合は false。</returns>
        public static bool IsNullOrEmpty(Size<TNumeric> size)
        {
            if (size == null)
            {
                return true;
            }

            if (((dynamic)size.Height == 0) && ((dynamic)size.Width == 0))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// <see cref="Size{T}"/>構造体の対角成分を取得します。
        /// </summary>
        public double Diagonal => Math.Sqrt(((dynamic)Width * Width) + ((dynamic)Height * Height));

        // ref readonlyが使用可能になる(C#7.2以降)まで実装不可。
        //public static ref readonly Size<TNumeric> Empty { get; } = new Size<TNumeric>();

        /// <summary>
        /// <see cref="Size{TNumeric}"/>構造体の垂直成分を取得または設定します。
        /// </summary>
        public TNumeric Height { get; set; }

        /// <summary>
        /// <see cref="Size{TNumeric}"/>構造体の水平成分を取得または設定します。
        /// </summary>
        public TNumeric Width { get; set; }
        #endregion // Properties

        #region Methods
        /// <summary>
        /// すべての要素を初期化します。
        /// </summary>
        public void Clear()
        {
            Height = (dynamic)0;
            Width = (dynamic)0;
        }
        #endregion // Methods
    }

    /// <summary>
    /// 順序付けられた寸法のペア（通常は長方形の幅と高さ）を格納します。
    /// </summary>
    /// <typeparam name="TNumeric">数値型</typeparam>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class BindingSize<TNumeric> : Size<TNumeric>, INotifyPropertyChanged where TNumeric : struct
    {
        #region Fields
        /// <summary>
        /// 水平成分
        /// </summary>
        private TNumeric width;

        /// <summary>
        /// 垂直成分
        /// </summary>
        private TNumeric height;
        #endregion

        #region Constructors
        /// <summary>
        /// 新しいインスタンスを初期化します。
        /// </summary>
        public BindingSize()
        {
        }

        /// <summary>
        /// 指定された寸法で、<see cref="BindingSize{T}"/>構造体の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="width">新しい<see cref="BindingSize{T}"/>構造体の横幅成分</param>
        /// <param name="height">新しい<see cref="BindingSize{T}"/>構造体の高さ成分</param>
        public BindingSize(TNumeric width, TNumeric height)
        {
            this.width = width;
            this.height = height;
        }

        /// <summary>
        /// 指定された<see cref="BindingSize{T}"/>構造体で、<see cref="BindingSize{T}"/>構造体の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="size">新しい<see cref="BindingSize{T}"/>構造体を生成するための<see cref="BindingSize{T}"/>構造体</param>
        public BindingSize(BindingSize<TNumeric> size)
        {
            width = size.width;
            height = size.height;
        }
        #endregion // Constructors

        #region Events
        /// <summary>
        /// プロパティが更新されたときに発生するイベント
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;
        #endregion

        #region Properties
        /// <summary>
        /// <see cref="BindingSize{T}"/>構造体の垂直成分を取得または設定します。
        /// </summary>
        public new TNumeric Height
        {
            get => height;
            set
            {
                height = value;
                OnPropertyChanged(nameof(Height));
            }
        }

        /// <summary>
        /// <see cref="BindingSize{T}"/>構造体の水平成分を取得または設定します。
        /// </summary>
        public new TNumeric Width
        {
            get => width;
            set
            {
                width = value;
                OnPropertyChanged(nameof(Width));
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
        /// データフィールドの内容をコピーします。
        /// </summary>
        /// <param name="size">コピー先データ</param>
        public void CopyTo(BindingSize<TNumeric> size)
        {
            size.width = width;
            size.height = height;
        }

        /// <summary>
        /// 新しい寸法を設定します。
        /// </summary>
        /// <param name="size">設定元データ</param>
        public void Set(BindingSize<TNumeric> size)
        {
            width = size.width;
            height = size.height;
            OnPropertyChanged(string.Empty);
        }

        /// <summary>
        /// 新しい寸法を設定します。
        /// </summary>
        /// <param name="size">設定元データ</param>
        public void Set(Size<TNumeric> size)
        {
            width = size.Width;
            height = size.Height;
            OnPropertyChanged(string.Empty);
        }
        #endregion
    }
}
