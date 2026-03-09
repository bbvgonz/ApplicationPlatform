using System;
using System.Drawing;
using System.Linq;

namespace Optical.Platform.Types
{
    /// <summary>
    /// ビニングピクセル値算出形式
    /// </summary>
    public enum BinningPixelFormat
    {
        /// <summary>
        /// 平均値
        /// </summary>
        Average,
        /// <summary>
        /// 積算値
        /// </summary>
        Sum,
        /// <summary>
        /// 無効
        /// </summary>
        Invalid
    }

    /// <summary>
    /// 画像構成要素クラス
    /// </summary>
    public class ImageComponent
    {
        #region Constructors
        /// <summary>
        /// <see cref="ImageComponent"/>クラスの新しいインスタンスを生成します。
        /// </summary>
        public ImageComponent()
        {
            initializeParameter();
        }

        /// <summary>
        /// 指定されたパラメーターで、<see cref="ImageComponent"/>クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="height">画像高さ</param>
        /// <param name="width">画像幅</param>
        /// <param name="bitDepth">ビット深度</param>
        public ImageComponent(int height, int width, int bitDepth)
        {
            BitDepth = bitDepth;
            Height = height;
            Pixels = new byte[Bit.ToByte(bitDepth) * height * width];
            Width = width;
        }

        /// <summary>
        /// 指定された画像構成要素で、<see cref="ImageComponent"/>クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="image">画像構成要素</param>
        public ImageComponent(ImageComponent image)
        {
            image.copyTo(this);
        }
        #endregion // Constructors

        #region Properties
        /// <summary>
        /// ビット深度(1-16)
        /// </summary>
        public int BitDepth { get; set; }

        /// <summary>
        /// 画素データ
        /// </summary>
        public byte[] Pixels { get; set; }

        /// <summary>
        /// 画像高さ[pixel]
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// 画像幅[pixel]
        /// </summary>
        public int Width { get; set; }
        #endregion // Properties

        #region Methods
        private void copyTo(ImageComponent image)
        {
            image.BitDepth = BitDepth;
            image.Pixels = new byte[Pixels.Length];
            Buffer.BlockCopy(Pixels, 0, image.Pixels, 0, image.Pixels.Length);
            image.Height = Height;
            image.Width = Width;
        }

        private void initializeParameter()
        {
            BitDepth = 0;
            Pixels = Array.Empty<byte>();
            Height = 0;
            Width = 0;
        }

        /// <summary>
        /// <see cref="ImageComponent"/>クラスの各パラメーターを初期化します。
        /// </summary>
        public void Clear()
        {
            initializeParameter();
        }

        /// <summary>
        /// 現在のインスタンスの簡易コピーを作成します。
        /// </summary>
        /// <returns>このインスタンスの簡易コピー。</returns>
        public ImageComponent Clone()
        {
            return (ImageComponent)MemberwiseClone();
        }

        /// <summary>
        /// 全ての要素を指定された画像構成要素にコピーします。
        /// </summary>
        /// <param name="image">画像構成要素</param>
        public void CopyTo(ImageComponent image)
        {
            copyTo(image);
        }
        #endregion // Methods
    }

    /// <summary>
    /// 画像構成要素クラス
    /// </summary>
    /// <typeparam name="TNumeric">数値型</typeparam>
    public class ImageComponent<TNumeric> where TNumeric : struct
    {
        #region Constructors
        /// <summary>
        /// <see cref="ImageComponent{TNumeric}"/>クラスの新しいインスタンスを生成します。
        /// </summary>
        public ImageComponent()
        {
            Pixels = (TNumeric[])Enumerable.Empty<TNumeric>();
        }

        /// <summary>
        /// 指定された画像構成要素で、<see cref="ImageComponent{TNumeric}"/>クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="image">画像構成要素</param>
        public ImageComponent(ImageComponent<TNumeric> image)
        {
            BitDepth = image.BitDepth;
            Pixels = new TNumeric[image.Pixels.Length];
            Buffer.BlockCopy(image.Pixels, 0, Pixels, 0, Pixels.Length * Bit.ToByte(image.BitDepth));
            Height = image.Height;
            Width = image.Width;
        }

        /// <summary>
        /// 指定したサイズを使用して、<see cref="ImageComponent{TNumeric}"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="width">新しい <see cref="ImageComponent{TNumeric}"/> の幅 [pixel]。</param>
        /// <param name="height">新しい <see cref="ImageComponent{TNumeric}"/> の高さ [pixel]。</param>
        /// <param name="bitDepth">新しい <see cref="ImageComponent{TNumeric}"/> のビット深度 [bit]。</param>
        /// <remarks><see cref="Pixels"/>のサイズは <see cref="Width"/> * <see cref="Height"/> * <see cref="BitDepth"/>.ToByte。</remarks>
        public ImageComponent(int width, int height, int bitDepth)
        {
            BitDepth = bitDepth;
            Pixels = new TNumeric[width * height];
            Height = height;
            Width = width;
        }
        #endregion // Constructors

        #region Properties
        /// <summary>
        /// ビット深度(<typeparamref name="TNumeric"/> のサイズ)
        /// </summary>
        public int BitDepth { get; set; }

        /// <summary>
        /// 画素データ
        /// </summary>
        public TNumeric[] Pixels { get; set; }

        /// <summary>
        /// 画像高さ[pixel]
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// 画像幅[pixel]
        /// </summary>
        public int Width { get; set; }
        #endregion // Properties

        #region Methods
        private void copyTo(ImageComponent<TNumeric> image)
        {
            BitDepth = image.BitDepth;
            Pixels = new TNumeric[image.Pixels.Length];
            Buffer.BlockCopy(image.Pixels, 0, Pixels, 0, Pixels.Length * Bit.ToByte(image.BitDepth));
            Height = image.Height;
            Width = image.Width;
        }

        private void initializeParameter()
        {
            BitDepth = 0;
            Pixels = Array.Empty<TNumeric>();
            Height = 0;
            Width = 0;
        }

        /// <summary>
        /// 全ての要素を指定された画像構成要素にコピーします。
        /// </summary>
        /// <param name="image">画像構成要素</param>
        public void CopyTo(ImageComponent<TNumeric> image)
        {
            copyTo(image);
        }
        #endregion // Methods
    }

    /// <summary>
    /// 測定対象画像の各種情報を格納します。
    /// </summary>
    public class ImageContainer
    {
        #region Constructors
        /// <summary>
        /// <see cref="ImageContainer"/>クラスの新しいインスタンスを生成します。
        /// </summary>
        public ImageContainer()
        {
            initialize();
        }

        /// <summary>
        /// 指定された画像データを元に、<see cref="ImageContainer"/>クラスの新しいインスタンスを生成します。
        /// </summary>
        /// <param name="image"></param>
        public ImageContainer(ImageContainer image)
        {
            updateFrame(image);
        }
        #endregion

        #region Events
        /// <summary>
        /// 画像データが更新されたときに発生します。
        /// </summary>
        /// <remarks>引数:画像識別子</remarks>
        public event EventHandler<string> Updated;
        #endregion

        #region Properties
        /// <summary>
        /// フレーム識別子
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// タイムスタンプ
        /// </summary>
        public TimeSpan Timestamp { get; set; }
        /// <summary>
        /// フレーム番号。測定開始毎にリセットされる。
        /// </summary>
        public int FrameNumber { get; set; }
        /// <summary>
        /// 画像高さ[pixels]
        /// </summary>
        public int Height { get; set; }
        /// <summary>
        /// 画像幅[pixels]
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// ビット深度[bpp]
        /// </summary>
        public int BitDepth { get; set; }
        /// <summary>
        /// RAW画像データ
        /// </summary>
        public ImageComponent RawData { get; set; }
        /// <summary>
        /// ビットマップ画像
        /// </summary>
        public Bitmap BitmapImage { get; set; }
        #endregion

        #region Methods
        private void initialize()
        {
            Id = string.Empty;
            Timestamp = new TimeSpan();
            FrameNumber = 0;
            Height = 0;
            Width = 0;
            BitDepth = 0;
            RawData = null;
            BitmapImage = null;
        }

        private void updateFrame(ImageContainer frame, bool deepCopy = false)
        {
            BitDepth = frame.BitDepth;
            BitmapImage = deepCopy ? (Bitmap)frame.BitmapImage?.Clone() : frame.BitmapImage;
            FrameNumber = frame.FrameNumber;
            Id = frame.Id;
            Height = frame.Height;
            Width = frame.Width;

            if (deepCopy)
            {
                if (RawData == null)
                {
                    RawData = new ImageComponent();
                }

                int length = Height * Width * Bit.ToByte(BitDepth);
                if (RawData.Pixels.Length != length)
                {
                    RawData.Pixels = new byte[length];
                }

                RawData.BitDepth = frame.RawData.BitDepth;
                RawData.Height = frame.RawData.Height;
                RawData.Width = frame.RawData.Width;
                Buffer.BlockCopy(frame.RawData.Pixels, 0, RawData.Pixels, 0, length);
            }
            else
            {
                RawData = frame.RawData;
            }

            Timestamp = frame.Timestamp;
        }

        /// <summary>
        /// <see cref="Updated"/>イベントの発生を通知します。
        /// </summary>
        /// <param name="id">フレーム識別子</param>
        protected void OnUpdated(string id)
        {
            Updated?.Invoke(this, id);
        }

        /// <summary>
        /// 現在の画像データ情報の全ての要素を、指定した画像データ情報にコピーします。
        /// </summary>
        /// <param name="frame">画像データ</param>
        public void CopyTo(ImageContainer frame)
        {
            frame.Id = Id;
            frame.Timestamp = Timestamp;
            frame.FrameNumber = FrameNumber;
            frame.Height = Height;
            frame.Width = Width;
            frame.BitDepth = BitDepth;

            if (frame.RawData == null)
            {
                frame.RawData = new ImageComponent();
            }

            int length = RawData.Height * RawData.Width * Bit.ToByte(RawData.BitDepth);
            if (frame.RawData.Pixels.Length != length)
            {
                frame.RawData.Pixels = new byte[length];
            }

            frame.RawData.BitDepth = RawData.BitDepth;
            frame.RawData.Height = RawData.Height;
            Buffer.BlockCopy(frame.RawData.Pixels, 0, RawData.Pixels, 0, length);
            frame.RawData.Width = RawData.Width;

            frame.BitmapImage = (Bitmap)BitmapImage?.Clone();
        }

        /// <summary>
        /// 指定した画像データで各パラメーターを更新します。
        /// </summary>
        /// <param name="frame">画像データ</param>
        /// <param name="deepCopy">ディープコピーフラグ</param>
        public void Update(ImageContainer frame, bool deepCopy = false)
        {
            updateFrame(frame, deepCopy);
            OnUpdated(Id);
        }

        /// <summary>
        /// <see cref="ImageContainer"/>クラスの各パラメーターを初期化します。
        /// </summary>
        public void Clear()
        {
            initialize();
        }

        /// <summary>
        /// 現在のインスタンスの簡易コピーを作成します。
        /// </summary>
        /// <returns>このインスタンスの簡易コピー。</returns>
        public ImageContainer Clone()
        {
            return (ImageContainer)MemberwiseClone();
        }
        #endregion
    }
}
