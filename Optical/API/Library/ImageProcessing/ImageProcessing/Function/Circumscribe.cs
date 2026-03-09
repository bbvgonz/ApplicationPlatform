using OpenCvSharp;
using Optical.Platform.Types;
using System;
using System.Drawing;

namespace Optical.API.Library
{
    /// <summary>
    /// 画像処理ライブラリクラス
    /// </summary>
    public partial class ImageProcessing
    {
        /// <summary>
        /// 点群に外接する傾いていない矩形を出力します。
        /// </summary>
        /// <param name="points">入力される2次元点の集合</param>
        /// <returns>外接矩形座標。</returns>
        public static Rectangle CircumscribedRectangle(PointD[] points)
        {
            OpenCvSharp.Point[] pointVector = Array.ConvertAll(points, new Converter<PointD, OpenCvSharp.Point>((point) => new OpenCvSharp.Point(point.X, point.Y)));

            Rect boudingRectangle = Cv2.BoundingRect(pointVector);

            return new Rectangle(boudingRectangle.X, boudingRectangle.Y, boudingRectangle.Width, boudingRectangle.Height);
        }

        /// <summary>
        /// 与えられた2次元点集合を囲む最小の円を出力します。
        /// </summary>
        /// <param name="points">入力される2次元点の集合</param>
        /// <returns>外接矩形座標。</returns>
        public static Rectangle CircumscribedCircle(PointD[] points)
        {
            OpenCvSharp.Point[] pointVector = Array.ConvertAll(points, new Converter<PointD, OpenCvSharp.Point>((point) => new OpenCvSharp.Point(point.X, point.Y)));

            Cv2.MinEnclosingCircle(pointVector, out Point2f center, out float radius);

            return new Rectangle((int)(center.X - radius), (int)(center.Y - radius), (int)(radius * 2), (int)(radius * 2));
        }
    }
}
