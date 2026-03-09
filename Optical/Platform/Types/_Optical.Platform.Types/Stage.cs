namespace Optical.Platform.Types
{
    namespace Stage
    {
        /// <summary>
        /// ステージの進行方向
        /// </summary>
        public enum DriveDirection
        {
            /// <summary>
            /// 負方向
            /// </summary>
            Minus = -1,
            /// <summary>
            /// 正方向
            /// </summary>
            Plus = 1,
        }

        /// <summary>
        /// エンコーダの逓倍入力形式。
        /// </summary>
        public enum EncoderMultiplyInput
        {
            /// <summary>
            /// 無効
            /// </summary>
            Disable,
            /// <summary>
            /// 1逓倍
            /// </summary>
            MultiplicationBy1,
            /// <summary>
            /// 2逓倍
            /// </summary>
            MultiplicationBy2,
            /// <summary>
            /// 4逓倍
            /// </summary>
            MultiplicationBy4
        }

        /// <summary>
        /// ステージテーブル側から見たモーターの回転方向
        /// </summary>
        public enum MotorRotation
        {
            /// <summary>
            /// 時計回り(Clockwise)
            /// </summary>
            CW,
            /// <summary>
            /// 反時計回り(Counter Clockwise)
            /// </summary>
            CCW,
            /// <summary>
            /// 未指定
            /// </summary>
            None
        };

        /// <summary>
        /// ステージモーターの種類
        /// </summary>
        public enum MotorType
        {
            /// <summary>
            /// 不明
            /// </summary>
            Unknown,
            /// <summary>
            /// ステッピングモーター
            /// </summary>
            Stepper,
            /// <summary>
            /// サーボモーター
            /// </summary>
            Servo
        }

        /// <summary>
        /// 原点復帰タイプ
        /// </summary>
        public enum OriginReturnType
        {
            /// <summary>
            /// 原点復帰を行いません。
            /// </summary>
            Type0,
            /// <summary>
            /// CCW方向に検出を行い、はじめにNORG信号のCW側エッジの検出工程を行い、次にORG信号のCCW側エッジの検出工程を行います。
            /// </summary>
            Type1,
            /// <summary>
            /// CW方向に検出を行い、はじめにNORG信号のCCW側エッジの検出工程を行い、次にORG信号のCW側エッジの検出工程を行います。
            /// </summary>
            Type2,
            /// <summary>
            /// CCW方向に検出を行い、ORG信号のCCW側エッジの検出工程を行います。
            /// </summary>
            Type3,
            /// <summary>
            /// CW方向に検出を行い、ORG信号のCW側エッジの検出工程を行います。
            /// </summary>
            Type4,
            /// <summary>
            /// CCW方向に検出を行い、CCWLS信号のCW側エッジの検出工程を行います。
            /// </summary>
            Type5,
            /// <summary>
            /// CW方向に検出を行い、CWLS信号のCCW側エッジの検出工程を行います。
            /// </summary>
            Type6
        }

        /// <summary>
        /// 原点復帰タイプ拡張クラス
        /// </summary>
        public static class OriginReturnTypeExtension
        {
            /// <summary>
            /// 原点復帰タイプ詳細情報説明
            /// </summary>
            /// <param name="type">原点復帰タイプ</param>
            /// <returns></returns>
            public static string Description(this OriginReturnType type)
            {
                switch (type)
                {
                    case OriginReturnType.Type0:
                        return "原点復帰を行いません。";
                    case OriginReturnType.Type1:
                        return "CCW方向に検出を行い、はじめにNORG信号のCW側エッジの検出工程を行い、次にORG信号のCCW側エッジの検出工程を行います。";
                    case OriginReturnType.Type2:
                        return "CW方向に検出を行い、はじめにNORG信号のCCW側エッジの検出工程を行い、次にORG信号のCW側エッジの検出工程を行います。";
                    case OriginReturnType.Type3:
                        return "CCW方向に検出を行い、ORG信号のCCW側エッジの検出工程を行います。";
                    case OriginReturnType.Type4:
                        return "CW方向に検出を行い、ORG信号のCW側エッジの検出工程を行います。";
                    case OriginReturnType.Type5:
                        return "CCW方向に検出を行い、CCWLS信号のCW側エッジの検出工程を行います。";
                    case OriginReturnType.Type6:
                        return "CW方向に検出を行い、CWLS信号のCCW側エッジの検出工程を行います。";
                    default:
                        return "対応していない原点復帰タイプです。";
                }
            }
        }
    }
}
