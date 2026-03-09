using Optical.API.Library;
using Optical.API.Library.Device;
using Optical.API.Library.Optics;
using Optical.Core.Camera;
using Optical.Platform.Mathematics;
using Optical.Platform.Structs;
using Optical.Platform.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Optical.Enums;

namespace Optical.API.Autocollimator.Sample
{
    internal class LaserAutocollimatorLogic
    {
        #region Fields
        /// <summary>
        /// 象限数
        /// </summary>
        private const int quadrantCount = 4;

        private const int defaultDivisionPoints = 33;

        /// <summary>
        /// バックアップ用Key
        /// </summary>
        private readonly string backupKey;

        private static Dictionary<string, CameraComponents> sensorTable;

        private int bitDepth;
        private int bufferIndex;
        private List<ManualResetEventSlim> bufferLockEvent = [];
        private readonly ISensorCamera camera;
        private int divisionPoints = defaultDivisionPoints;
        private List<TiltContainer> frameBuffers = [];
        private List<Rectangle> roiList = [];
        private double tiltRange;

        /// <summary>
        /// 角度補正データ初期値
        /// </summary>
        private CalibrationComponent defaultTiltCorrection;
        /// <summary>
        /// 角度補正データ
        /// </summary>
        private CalibrationComponent? tiltCorrection;
        /// <summary>
        /// 最近傍校正点探索用ツリー
        /// </summary>
        private CalibrationElement pointMapIndexTree;
        #endregion //Fields

        #region Constructors
        static LaserAutocollimatorLogic()
        {
            sensorTable = [];
        }

        /// <summary>
        /// <see cref="LaserAutocollimatorLogic"/>クラスの新しいインスタンスを生成します。
        /// </summary>
        /// <param name="component">センサー識別情報</param>
        public LaserAutocollimatorLogic(SensorComponents component)
        {
            backupKey = $"{typeof(LaserAutocollimatorLogic).Namespace}.{nameof(LaserAutocollimatorLogic)}.{nameof(tiltCorrection)}";

            bufferLockEvent = [new ManualResetEventSlim(false)];
            frameBuffers = [new TiltContainer()];
            roiList = [];
            SensorSize = new();

            // 現在接続されているセンサーに対象のセンサーがあるかどうか確認する。
            if (sensorTable.Count == 0)
            {
                IReadOnlyList<CameraComponents> cameraList = CameraSearch.EnumerateDevice();
                foreach (CameraComponents camera in cameraList)
                {
                    sensorTable[camera.GenerateKey()] = camera;
                }
            }

            if (!sensorTable.TryGetValue(component.Key, out CameraComponents? value))
            {
                throw new ArgumentOutOfRangeException(nameof(component), component.Key, "存在しないデバイスです。");
            }

            // 対象センサーのインスタンス生成。
            var classType = Type.GetType((value).Identifier) ?? throw new ArgumentNullException(nameof(component));
            camera = Activator.CreateInstance(classType, value) as ISensorCamera ?? throw new ArgumentNullException(nameof(component));

            camera.Open();
            camera.NewFrame += Camera_NewFrame;
            SensorSize.Height = camera.Height;
            SensorSize.Width = camera.Width;
            DeviceId = component.DeviceID;

            defaultTiltCorrection = generateDefaultTiltCorrection();
            pointMapIndexTree = generateCorrectionTree(defaultTiltCorrection);
        }
        #endregion // Constructors

        #region Events
        /// <summary>
        /// 画像データが更新された場合にイベントが発生します。
        /// </summary>
        public event EventHandler<int>? NewFrame;

        /// <summary>
        /// 角度の測定結果が更新された場合に発生します。
        /// </summary>
        public event EventHandler<int>? NewResult;
        #endregion

        #region Properties
        /// <summary>
        /// 自動露光時間調整の動作可否を取得・設定します。
        /// </summary>
        public bool AutoExposure
        {
            get => camera.AutoExposure;
            set => camera.AutoExposure = value;
        }

        /// <summary>
        /// 自動ゲイン調整調整の動作可否を取得・設定します。
        /// </summary>
        public bool AutoGain
        {
            get => camera.AutoGain;
            set => camera.AutoGain = value;
        }

        /// <summary>
        /// 移動平均の平均化回数を取得・設定します。
        /// </summary>
        public int AveragingTimes { get; set; }

        /// <summary>
        /// ビニングする一辺のピクセル数を取得・設定します。
        /// </summary>
        public int Binning
        {
            get => camera.Binning;
            set => camera.Binning = value;
        }

        /// <summary>
        /// ビニング時のピクセル値算出方法を取得・設定します。
        /// </summary>
        public BinningPixelFormat BinningMode
        {
            get => camera.BinningMode;
            set => camera.BinningMode = value;
        }

        /// <summary>
        /// 画像データのビット深度を取得・設定します。
        /// </summary>
        public int BitDepth
        {
            get
            {
                if (bitDepth == 0)
                {
                    bitDepth = camera.BitDepth;
                }

                return bitDepth;
            }

            set
            {
                camera.BitDepth = value;
                bitDepth = camera.BitDepth;
            }
        }

        /// <summary>
        /// イメージセンサーの黒レベルを取得・設定します。
        /// </summary>
        public double BlackLevel
        {
            get => camera.BlackLevel;
            set => camera.BlackLevel = value;
        }

        /// <summary>
        /// 測定結果バッファの参照先インデックスを取得・設定します。
        /// </summary>
        public int BufferIndex
        {
            get => bufferIndex;
            set => bufferIndex = value % frameBuffers.Count;
        }

        /// <summary>
        /// 測定結果バッファのサイズを取得・設定します。
        /// </summary>
        /// <remarks>初期サイズ：1
        /// <para>測定停止中のみ変更可能です。</para></remarks>
        public int BufferSize
        {
            get => frameBuffers.Count;
            set
            {
                if (frameBuffers.Count > value)
                {
                    int delta = FrameBuffers.Count - value;
                    FrameBuffers.RemoveRange(value, delta);
                    frameBuffers.TrimExcess();

                    bufferLockEvent.RemoveRange(value, delta);
                    bufferLockEvent.TrimExcess();
                    bufferLockEvent.ForEach(lockEvent => lockEvent.Reset());
                }
                else
                {
                    for (int index = frameBuffers.Count; index < value; index++)
                    {
                        bufferLockEvent.Add(new ManualResetEventSlim(false));
                        frameBuffers.Add(new TiltContainer());
                    }
                }
            }
        }

        /// <summary>
        /// 重心計算方法を取得・設定します。
        /// </summary>
        public CentroidMethod CentroidType { get; set; }

        /// <summary>
        /// 初期校正値の分割点数
        /// </summary>
        public int DivisionPoints
        {
            get => divisionPoints;
            set
            {
                divisionPoints = value;
                defaultTiltCorrection = generateDefaultTiltCorrection();
            }
        }

        /// <summary>
        /// ビーム検出閾値を取得・設定します。
        /// </summary>
        public int DetectionThreshold { get; set; }

        /// <summary>
        /// デバイスIDを取得します。
        /// </summary>
        public string DeviceId { get; private set; }

        /// <summary>
        /// イメージセンサーの露光時間[ms]を取得・設定します。
        /// </summary>
        public double ExposureTime
        {
            get => camera.ExposureTime;
            set => camera.ExposureTime = value;
        }

        /// <summary>
        /// 露光時間の設定範囲[ms]
        /// </summary>
        public Limit<double> ExposureTimeRange => camera.ExposureTimeRange;

        /// <summary>
        /// 画像の水平方向のミラーリングを有効にします。
        /// </summary>
        /// <remarks>キャプチャした画像の各ラインのピクセル値は、ラインの中心を軸に端と端が入れ替わります。</remarks>
        public bool FlipHorizontal
        {
            get => camera.FlipHorizontal;
            set => camera.FlipHorizontal = value;
        }

        /// <summary>
        /// 画像の垂直方向のミラーリングを有効にします。
        /// </summary>
        /// <remarks>撮影した画像の各行のピクセル値が、その行の中心を軸に端から端まで入れ替わります。</remarks>
        public bool FlipVertical
        {
            get => camera.FlipVertical;
            set => camera.FlipVertical = value;
        }

        /// <summary>
        /// 画像データの更新周期[Hz]を取得・設定します。
        /// </summary>
        public double FrameRate
        {
            get => camera.FrameRate;
            set => camera.FrameRate = value;
        }

        /// <summary>
        /// 画像データの更新周期の設定範囲[Hz]
        /// </summary>
        public Limit<double> FrameRateRange => camera.FrameRateRange;

        /// <summary>
        /// 測定結果バッファを参照します。
        /// </summary>
        public List<TiltContainer> FrameBuffers => frameBuffers;

        /// <summary>
        /// 指定されたインデックスのフレームバッファを参照します。
        /// <seealso cref="BufferIndex"/>
        /// </summary>
        public TiltContainer FrameBuffer => frameBuffers[bufferIndex];

        /// <summary>
        /// イメージセンサーのゲイン[dB]を取得・設定します。
        /// </summary>
        public double Gain
        {
            get => camera.Gain;
            set => camera.Gain = value;
        }

        /// <summary>
        /// ゲインの設定範囲[dB]
        /// </summary>
        public Limit<double> GainRange => camera.GainRange;

        /// <summary>
        /// 内部光源を使用するかどうかを示す値を取得・設定します。
        /// </summary>
        public bool InternalLightSource { get; set; }

        public bool IsCalibrated => (tiltCorrection != null);

        /// <summary>
        /// デバイスが測定中かどうかを確認します。
        /// </summary>
        public bool IsMeasuring => camera.IsGrabbing;

        /// <summary>
        /// デバイスが使用可能な状態かどうかを確認します。
        /// </summary>
        public bool IsOpened => camera.IsOpened;

        /// <summary>
        /// イメージセンサーのノイズレベルを取得・設定します。
        /// </summary>
        public int NoiseLevel { get; set; }

        /// <summary>
        /// 対象領域の範囲[pixel]を取得・設定します。
        /// </summary>
        public List<Rectangle> RoiList => roiList;

        /// <summary>
        /// イメージセンサーの画面解像度[pixel]を取得します。
        /// </summary>
        public Size<int> SensorSize { get; private set; }

        /// <summary>
        /// 最新の測定角度[deg]を取得・設定します。
        /// </summary>
        public AngleD Tilt { get; set; } = new();

        /// <summary>
        /// 測定範囲を取得・設定します。±[deg]
        /// </summary>
        public double TiltRange
        {
            get => tiltRange;
            set
            {
                tiltRange = value;
                defaultTiltCorrection = generateDefaultTiltCorrection();
            }
        }

        /// <summary>
        /// トリガーモードで動作するどうかを示す値を取得・設定します。
        /// </summary>
        public bool TriggerMode
        {
            get => camera.TriggerMode;
            set => camera.TriggerMode = value;
        }

        /// <summary>
        /// トリガーモード動作時の入力タイプを取得・設定します。
        /// </summary>
        public TriggerInput TriggerType
        {
            get => camera.TriggerType;
            set => camera.TriggerType = value;
        }

        /// <summary>
        /// ワークディスタンスを取得・設定します。[um]
        /// </summary>
        public double WorkDistance { get; set; }
        #endregion // Properties

        #region Methods
        private void Camera_NewFrame(object? sender, ImageContainer e)
        {
            if (frameBuffers.Count == 0)
            {
                return;
            }

            int latestIndex = (bufferIndex + 1) % frameBuffers.Count;
            if (bufferLockEvent[latestIndex].IsSet)
            {
                return;
            }

            frameBuffers[latestIndex].Clear();
            FrameBuffers[latestIndex].Image.Update(e, true);
            bufferIndex = latestIndex;

            Task createTask(int index)
            {
                bufferLockEvent[index].Set();
                return new Task(() =>
                {
                    calculateTilt(index);
                    raiseNewResult(index);

                    bufferLockEvent[index].Reset();
                });
            }

            createTask(latestIndex).Start();

            Task.Run(() => raiseNewFrame(latestIndex));
        }

        /// <summary>
        /// 角度を計算します。
        /// </summary>
        /// <param name="bufferIndex">測定結果バッファの参照先インデックス</param>
        private void calculateTilt(int bufferIndex)
        {
            var targetRoi = new List<Rectangle>();
            if (roiList.Count > 0)
            {
                targetRoi.AddRange(roiList);
            }
            else
            {
                targetRoi.Add(new Rectangle(0, 0, frameBuffers[bufferIndex].Image.Width, frameBuffers[bufferIndex].Image.Height));
            }

            var tiltList = new List<TiltSpotContainer>(targetRoi.Count);
            for (int roiIndex = 0; roiIndex < targetRoi.Count; roiIndex++)
            {
                tiltList.Add(new TiltSpotContainer());
                tiltList[roiIndex].RoiNumber = roiIndex + 1;
                tiltList[roiIndex].Roi = targetRoi[roiIndex];
                tiltList[roiIndex].RoiShape = ApertureShape.Rectangle;
                tiltList[roiIndex].RoiImage.BitDepth = frameBuffers[bufferIndex].Image.BitDepth;
                tiltList[roiIndex].Centroid.X = double.NaN;
                tiltList[roiIndex].Centroid.Y = double.NaN;
                tiltList[roiIndex].Tilt = new AngleD(double.NaN, double.NaN);
            }

            try
            {
                Parallel.For(0, targetRoi.Count, roiIndex =>
                {
                    // ROI内のデータを1次元配列に格納
                    int bytesPerPixel = Bit.ToByte(tiltList[roiIndex].RoiImage.BitDepth);
                    tiltList[roiIndex].RoiImage.Height = tiltList[roiIndex].Roi.Height;
                    tiltList[roiIndex].RoiImage.Width = tiltList[roiIndex].Roi.Width;
                    tiltList[roiIndex].RoiImage.Pixels = new byte[tiltList[roiIndex].Roi.Height * tiltList[roiIndex].Roi.Width * bytesPerPixel];
                    for (int row = 0; row < tiltList[roiIndex].Roi.Height; row++)
                    {
                        Buffer.BlockCopy(frameBuffers[bufferIndex].Image.RawData.Pixels,
                                         ((tiltList[roiIndex].Roi.Top + row) * frameBuffers[bufferIndex].Image.RawData.Width + tiltList[roiIndex].Roi.Left) * bytesPerPixel,
                                         tiltList[roiIndex].RoiImage.Pixels,
                                         row * tiltList[roiIndex].Roi.Width * bytesPerPixel,
                                         tiltList[roiIndex].Roi.Width * bytesPerPixel);
                    }

                    #region 重心算出
                    ImageComponent binaryImage = ImageProcessing.Binarize(tiltList[roiIndex].RoiImage, NoiseLevel);
                    ImageProcessing.LabelingContainer labelStatus = ImageProcessing.LabelingStats(binaryImage);

                    // Components[0]には全体情報が入る為要素数2以上でなければならない
                    if (labelStatus.Components.Length < 2)
                    {
                        throw new InvalidOperationException("ラベリング要素が不足しています。");
                    }

                    int maxArea = -1;
                    int maxAreaIndex = -1;
                    for (int index = 1; index < labelStatus.Components.Length; index++)
                    {
                        if (maxArea < labelStatus.Components[index].Area)
                        {
                            maxArea = labelStatus.Components[index].Area;
                            maxAreaIndex = index;
                        }
                    }

                    int peak;
                    PointD roiCentroid;
                    if (CentroidType == CentroidMethod.Area)
                    {
                        roiCentroid = labelStatus[maxAreaIndex].Centroid;

                        peak = ImageProcessing.LuminancePeakValue(tiltList[roiIndex].RoiImage,
                                                                  labelStatus[maxAreaIndex].Circumscribed,
                                                                  NoiseLevel);
                    }
                    else
                    {
                        roiCentroid = ImageProcessing.LuminanceCentroid(tiltList[roiIndex].RoiImage,
                                                                        NoiseLevel,
                                                                        labelStatus[maxAreaIndex].Circumscribed,
                                                                        out peak);
                    }

                    tiltList[roiIndex].Centroid.X = roiCentroid.X + tiltList[roiIndex].Roi.Left;
                    tiltList[roiIndex].Centroid.Y = roiCentroid.Y + tiltList[roiIndex].Roi.Top;
                    tiltList[roiIndex].Peak = peak;
                    #endregion 重心算出

                    // 重心座標と校正値から角度を算出します。
                    if (double.IsNaN(tiltList[roiIndex].Centroid.X) || double.IsNaN(tiltList[roiIndex].Centroid.Y))
                    {
                        return;
                    }

                    var rawCentroid = new PointD(tiltList[roiIndex].Centroid.X, tiltList[roiIndex].Centroid.Y);

                    CalibrationComponent component = tiltCorrection ?? defaultTiltCorrection;

                    int nearestIndex = nearestCalibrationPointIndex(component.PointMap, pointMapIndexTree, rawCentroid);
                    List<CorrectionPair<AngleD>> correctionPairs = targetCalibrationPoints(component, nearestIndex, rawCentroid);

                    if (correctionPairs.Count != quadrantCount)
                    {
                        return;
                    }

                    AngleD tiltResult = Interpolation.Bilinear(correctionPairs, rawCentroid);

                    // 校正時とAutocollimator使用時のの光源タイプに応じてTilt結果を修正します。
                    if (component.LightSource == LightSourcePosition.Internal)
                    {
                        if (!InternalLightSource)
                        {
                            tiltResult.X *= 2;
                            tiltResult.Y *= 2;
                        }
                    }
                    else
                    {
                        if (InternalLightSource)
                        {
                            tiltResult.X /= 2;
                            tiltResult.Y /= 2;
                        }
                    }

                    tiltList[roiIndex].Tilt = tiltResult;
                });
            }
            finally
            {
                frameBuffers[bufferIndex].UpdateTiltList(tiltList);
            }
        }

        /// <summary>
        /// 最近傍校正点探索用ツリー生成
        /// </summary>
        /// <param name="tiltCorrection">校正データ</param>
        private CalibrationElement generateCorrectionTree(CalibrationComponent tiltCorrection)
        {
            var targetTree = new CalibrationElement("root");

            if (tiltCorrection == null)
            {
                return targetTree;
            }

            // 校正点の数からツリー構造の深さを決定する。
            int treeDepth = (int)Math.Log(tiltCorrection.PointMap.Count, quadrantCount);
            if (treeDepth > 1)
            {
                treeDepth--;
            }

            // 各校正点を象限ごとに分類してツリーへ追加する。
            targetTree.Center.XY(tiltCorrection.SensorSize.Width / 2, tiltCorrection.SensorSize.Height / 2);
            for (int index = 0; index < tiltCorrection.PointMap.Count; index++)
            {
                CalibrationElement parent = targetTree;
                for (int depth = 0; depth < treeDepth; depth++)
                {
                    int quadrant = parent.TargetQuadrantIndex(tiltCorrection.PointMap[index].Reference);
                    if (!parent.Child.ContainsKey(quadrant))
                    {
                        parent.Add(new CalibrationElement(quadrant));
                    }

                    var child = (CalibrationElement)parent.Child[quadrant];
                    int widthDelta = (tiltCorrection.SensorSize.Width / 2) / (int)Math.Pow(2, (depth + 1));
                    int heightDelta = (tiltCorrection.SensorSize.Height / 2) / (int)Math.Pow(2, (depth + 1));
                    child.Center.XY(parent.Center.X + (Euclidean.QuadrantSign[quadrant].X * widthDelta),
                                    parent.Center.Y + (Euclidean.QuadrantSign[quadrant].Y * heightDelta));
                    parent = child;
                }

                parent.Value.Add(index);
            }

            return targetTree;
        }

        /// <summary>
        /// 初期校正値を設定します。
        /// </summary>
        private CalibrationComponent generateDefaultTiltCorrection()
        {
            // センサーサイズの小さい方に合わせる。
            int smallerSensorSize = (SensorSize.Height < SensorSize.Width) ? SensorSize.Height : SensorSize.Width;

            // 校正マップの点間隔[pixel]
            double pitch = smallerSensorSize / (divisionPoints - 1);

            // 1ピクセル辺りの角度を算出[deg]
            double anglePerPixel = tiltRange * 2 / smallerSensorSize;

            // 校正点マップ開始座標を算出 [pixel]
            int left = (SensorSize.Width - smallerSensorSize) / 2;
            int top = (SensorSize.Height - smallerSensorSize) / 2;

            // 校正ポイントマップ生成
            var pointMap = new List<CorrectionPair<AngleD>>();
            for (int row = 0; row < divisionPoints; row++)
            {
                for (int column = 0; column < divisionPoints; column++)
                {
                    var correctionPair = new CorrectionPair<AngleD>();
                    correctionPair.Reference.X = left + (pitch * column);
                    correctionPair.Reference.Y = top + (pitch * row);
                    correctionPair.Target.X = -tiltRange + (anglePerPixel * column * pitch);
                    correctionPair.Target.Y = tiltRange - (anglePerPixel * row * pitch);
                    pointMap.Add(correctionPair);
                }
            }

            var calibrationComponent = new CalibrationComponent
            {
                CentroidType = CentroidMethod.Luminance,
                DeviceId = DeviceId,
                LightSource = LightSourcePosition.External,
                PointMap = pointMap,
                PointMapHorizontalCount = divisionPoints,
                PointMapVerticalCount = divisionPoints,
                SensorSize = SensorSize,
                WaveLength = double.NaN
            };

            return calibrationComponent;
        }

        private void raiseNewFrame(int bufferIndex)
        {
            NewFrame?.Invoke(this, bufferIndex);
        }

        private void raiseNewResult(int bufferIndex)
        {
            NewResult?.Invoke(this, bufferIndex);
        }

        /// <summary>
        /// 指定された座標に対して、最も近い位置にある校正点のインデックスを出力します。
        /// </summary>
        /// <param name="calibrationMap">校正点マップ</param>
        /// <param name="indexTree">近傍探索用校正点インデックスツリー</param>
        /// <param name="targetPoint">対象座標</param>
        /// <returns>最近傍校正点のインデックス</returns>
        private int nearestCalibrationPointIndex(List<CorrectionPair<AngleD>> calibrationMap, CalibrationElement indexTree, PointD targetPoint)
        {
            if (calibrationMap.Count < 1)
            {
                throw new ArgumentException("校正点マップにデータが存在しません。", nameof(calibrationMap));
            }

            if (!(indexTree?.HasChild ?? false))
            {
                return -1;
            }

            CalibrationElement parent = indexTree;

            var indexList = new List<int>();
            while (parent.HasChild)
            {
                int key = parent.TargetQuadrantIndex(targetPoint);
                if (parent.Child.ContainsKey(key))
                {
                    parent = (CalibrationElement)parent.Child[key];
                    indexList = parent.Value;
                }
                else
                {
                    indexList.Clear();
                    var parents = new List<CalibrationElement> { parent };
                    while (parents.Count > 0)
                    {
                        var childs = new List<CalibrationElement>();
                        foreach (CalibrationElement node in parents)
                        {
                            // 子ノードを持つ場合、子ノードをchildsに集める
                            if (node.HasChild)
                            {
                                foreach (KeyValuePair<object, TreeElement<int>> child in node.Child)
                                {
                                    childs.Add((CalibrationElement)child.Value);
                                }
                            }
                            else
                            {
                                indexList.AddRange(node.Value);
                            }
                        }

                        parents = childs;
                    }

                    break;
                }
            }

            int targetIndex = -1;
            double minDistance = double.MaxValue;
            for (int index = 0; index < indexList.Count; index++)
            {
                double deltaX = targetPoint.X - calibrationMap[indexList[index]].Reference.X;
                double deltaY = targetPoint.Y - calibrationMap[indexList[index]].Reference.Y;
                double distance = (deltaX * deltaX) + (deltaY * deltaY);
                if (minDistance > distance)
                {
                    minDistance = distance;
                    targetIndex = indexList[index];
                }
            }

            return targetIndex;
        }

        /// <summary>
        /// 指定された座標を補正するための4つの校正点のデータを出力します。
        /// </summary>
        /// <param name="calibration">校正点マップ</param>
        /// <param name="nearestIndex">最近傍校正点のインデックス</param>
        /// <param name="targetPoint">補正対象座標</param>
        /// <returns></returns>
        private List<CorrectionPair<AngleD>> targetCalibrationPoints(CalibrationComponent calibration, int nearestIndex, PointD targetPoint)
        {
            if (calibration == null)
            {
                throw new ArgumentNullException("校正データが存在しません。", nameof(calibration));
            }

            if ((nearestIndex < 0) || (nearestIndex > calibration.PointMap.Count - 1))
            {
                throw new ArgumentOutOfRangeException("最近傍校正点のインデックスが校正マップの範囲外です。", nameof(nearestIndex));
            }

            int quadrantIndex = Euclidean.QuadrantIndex(calibration.PointMap[nearestIndex].Reference, targetPoint);

            // 校正点４つ分のインデックスを算出。
            int[] quadrant = new int[4];
            switch (quadrantIndex)
            {
                case 0:
                    // 第2象限
                    quadrant[0] = nearestIndex - calibration.PointMapHorizontalCount - 1;
                    quadrant[1] = nearestIndex - calibration.PointMapHorizontalCount;
                    quadrant[2] = nearestIndex - 1;
                    quadrant[3] = nearestIndex;
                    break;
                case 1:
                    // 第1象限
                    quadrant[0] = nearestIndex - calibration.PointMapHorizontalCount;
                    quadrant[1] = nearestIndex - calibration.PointMapHorizontalCount + 1;
                    quadrant[2] = nearestIndex;
                    quadrant[3] = nearestIndex + 1;
                    break;
                case 2:
                    // 第3象限
                    quadrant[0] = nearestIndex - 1;
                    quadrant[1] = nearestIndex;
                    quadrant[2] = nearestIndex + calibration.PointMapHorizontalCount - 1;
                    quadrant[3] = nearestIndex + calibration.PointMapHorizontalCount;
                    break;
                case 3:
                    // 第4象限
                    quadrant[0] = nearestIndex;
                    quadrant[1] = nearestIndex + 1;
                    quadrant[2] = nearestIndex + calibration.PointMapHorizontalCount;
                    quadrant[3] = nearestIndex + calibration.PointMapHorizontalCount + 1;
                    break;
                default:
                    return new List<CorrectionPair<AngleD>>();
            }

            var points = new List<CorrectionPair<AngleD>>();
            for (int index = 0; index < quadrant.Length; index++)
            {
                // 校正マップのインデックス範囲内かを判定
                if ((quadrant[index] < 0) || (quadrant[index] > calibration.PointMap.Count - 1))
                {
                    continue;
                }

                // 校正点の位置関係を判定
                switch (index)
                {
                    // 左上の校正点のインデックス"quadrant[0]"が、ポイントマップの左端に無いことを判定
                    case 0:
                        if ((quadrant[index] + 1) % calibration.PointMapHorizontalCount == 0)
                        {
                            continue;
                        }
                        break;
                    // 右上の校正点のインデックス"quadrant[1]"が、ポイントマップの左端に無いことを判定
                    case 1:
                        if (quadrant[index] % calibration.PointMapHorizontalCount == 0)
                        {
                            continue;
                        }
                        break;
                    // 左下の校正点のインデックス"quadrant[2]"が、ポイントマップの左端に無いことを判定
                    case 2:
                        if ((quadrant[index] + 1) % calibration.PointMapHorizontalCount == 0)
                        {
                            continue;
                        }
                        break;
                    // 右下の校正点のインデックス"quadrant[3]"が、ポイントマップの左端に無いことを判定
                    case 3:
                        if (quadrant[index] % calibration.PointMapHorizontalCount == 0)
                        {
                            continue;
                        }
                        break;
                }

                points.Add(calibration.PointMap[quadrant[index]]);
            }

            return points;
        }

        /// <summary>
        /// センサーデバイス一覧を列挙します。
        /// </summary>
        /// <returns>センサーデバイス一覧</returns>
        public static IReadOnlyList<SensorComponents> EnumerateDevice()
        {
            IReadOnlyList<CameraComponents> cameraList = CameraSearch.EnumerateDevice();

            sensorTable.Clear();
            var sensorList = new List<SensorComponents>();
            foreach (CameraComponents camera in cameraList)
            {
                string key = camera.GenerateKey();
                sensorTable[key] = camera;

                var sensor = new SensorComponents
                {
                    Key = key,
                    VendorName = camera.VendorName,
                    ModelName = camera.ModelName,
                    DeviceID = camera.DeviceId,
                    ConnectionType = camera.ConnectionType.ToString(),
                    SerialNumber = camera.SerialNumber
                };
                sensorList.Add(sensor);
            }

            return sensorList.AsReadOnly();
        }

        /// <summary>
        /// 校正データを削除します。
        /// </summary>
        public void ClearCalibration()
        {
            pointMapIndexTree = generateCorrectionTree(defaultTiltCorrection);
            tiltCorrection = null;
        }

        /// <summary>
        /// デバイスの使用を停止します。
        /// </summary>
        public void Close()
        {
            if (!camera.IsOpened)
            {
                return;
            }

            if (camera.IsGrabbing)
            {
                camera.Stop();
            }

            camera.Close();
        }

        /// <summary>
        /// バッファに格納されている情報を削除します。
        /// </summary>
        public void ClearBuffer()
        {
            foreach (TiltContainer frame in frameBuffers)
            {
                frame.Clear();
            }
        }

        /// <summary>
        /// デバイスの使用を開始します。
        /// </summary>
        public void Open()
        {
            if (camera.IsOpened)
            {
                return;
            }

            camera.Open();
        }

        /// <summary>
        /// 校正ファイルを読み込みます。
        /// </summary>
        /// <param name="filePath">校正ファイルパス</param>
        public void ReadCalibrationFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("校正ファイルが見つかりません。", nameof(filePath));
            }

            var xml = new DataContractSerializer(typeof(CalibrationComponent));
            CalibrationComponent? component;
            using (var optionFile = XmlReader.Create(filePath))
            {
                component = (CalibrationComponent?)xml.ReadObject(optionFile);
            }

            if (component is null)
            {
                throw new XmlException("校正ファイルのフォーマットが不正です。");
            }

            if (component.DeviceId == DeviceId)
            {
                tiltCorrection = component;
                pointMapIndexTree = generateCorrectionTree(tiltCorrection);
            }
        }

        /// <summary>
        /// デバイスを測定開始状態にします。
        /// </summary>
        public void Start()
        {
            camera.Start();
        }

        /// <summary>
        /// デバイスの測定を停止します。
        /// </summary>
        public void Stop()
        {
            camera.Stop();
        }

        /// <summary>
        /// ソフトウェアトリガーによるキャプチャー処理を行います。
        /// </summary>
        public void TakeSnapshot()
        {
            camera.TakeSnapshot();
        }
        #endregion // Methods

        #region Classes
        private class CalibrationElement(object name, TreeElement<int>? parent = null) : TreeElement<int>(name, parent)
        {
            #region Properties
            /// <summary>
            /// 中心座標[deg] (※左上原点)
            /// </summary>
            public PointD Center { get; set; } = new PointD();
            #endregion // Properties

            #region Methods
            public void Add(CalibrationElement element)
            {
                TreeElement<int> treeElement = element;
                Add(treeElement);
            }

            /// <summary>
            /// 指定された座標から、中心座標に対する象限を出力する。
            /// </summary>
            /// <returns>対象象限のインデックス。（0～3）</returns>
            public int TargetQuadrantIndex(PointD coordinate)
            {
                return Euclidean.QuadrantIndex(Center, coordinate);
            }
            #endregion // Methods
        }
        #endregion // Classes
    }
}
