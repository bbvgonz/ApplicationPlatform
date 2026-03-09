using System;
using System.Collections.Generic;

namespace Optical.Platform.Types
{
    /// <summary>
    /// ビット演算クラス
    /// </summary>
    public static class Bit
    {
        /// <summary>
        /// byte長変換テーブル
        /// </summary>
        private static readonly Dictionary<int, Func<byte[], int, uint>> bitConvertTable = new Dictionary<int, Func<byte[], int, uint>>
        {
            [1] = (byteArray, index) => byteArray[index],
            [2] = (byteArray, index) => BitConverter.ToUInt16(byteArray, index << 1),
            [4] = (byteArray, index) => BitConverter.ToUInt32(byteArray, index << 2)
        };

        /// <summary>
        /// Bit Flag(64bit)
        /// </summary>
        [Flags]
        public enum Flags : long
        {
            /// <summary>
            /// 0x0000000000000001
            /// </summary>
            Bit0 = 1,
            /// <summary>
            /// 0x0000000000000002
            /// </summary>
            Bit1 = 1 << 1,
            /// <summary>
            /// 0x0000000000000004
            /// </summary>
            Bit2 = 1 << 2,
            /// <summary>
            /// 0x0000000000000008
            /// </summary>
            Bit3 = 1 << 3,
            /// <summary>
            /// 0x0000000000000010
            /// </summary>
            Bit4 = 1 << 4,
            /// <summary>
            /// 0x0000000000000020
            /// </summary>
            Bit5 = 1 << 5,
            /// <summary>
            /// 0x0000000000000040
            /// </summary>
            Bit6 = 1 << 6,
            /// <summary>
            /// 0x0000000000000080
            /// </summary>
            Bit7 = 1 << 7,
            /// <summary>
            /// 0x0000000000000100
            /// </summary>
            Bit8 = 1 << 8,
            /// <summary>
            /// 0x0000000000000200
            /// </summary>
            Bit9 = 1 << 9,
            /// <summary>
            /// 0x0000000000000400
            /// </summary>
            Bit10 = 1 << 10,
            /// <summary>
            /// 0x0000000000000800
            /// </summary>
            Bit11 = 1 << 11,
            /// <summary>
            /// 0x0000000000001000
            /// </summary>
            Bit12 = 1 << 12,
            /// <summary>
            /// 0x0000000000002000
            /// </summary>
            Bit13 = 1 << 13,
            /// <summary>
            /// 0x0000000000004000
            /// </summary>
            Bit14 = 1 << 14,
            /// <summary>
            /// 0x0000000000008000
            /// </summary>
            Bit15 = 1 << 15,
            /// <summary>
            /// 0x0000000000010000
            /// </summary>
            Bit16 = 1 << 16,
            /// <summary>
            /// 0x0000000000020000
            /// </summary>
            Bit17 = 1 << 17,
            /// <summary>
            /// 0x0000000000040000
            /// </summary>
            Bit18 = 1 << 18,
            /// <summary>
            /// 0x0000000000080000
            /// </summary>
            Bit19 = 1 << 19,
            /// <summary>
            /// 0x0000000000100000
            /// </summary>
            Bit20 = 1 << 20,
            /// <summary>
            /// 0x0000000000200000
            /// </summary>
            Bit21 = 1 << 21,
            /// <summary>
            /// 0x0000000000400000
            /// </summary>
            Bit22 = 1 << 22,
            /// <summary>
            /// 0x0000000000800000
            /// </summary>
            Bit23 = 1 << 23,
            /// <summary>
            /// 0x0000000001000000
            /// </summary>
            Bit24 = 1 << 24,
            /// <summary>
            /// 0x0000000002000000
            /// </summary>
            Bit25 = 1 << 25,
            /// <summary>
            /// 0x0000000004000000
            /// </summary>
            Bit26 = 1 << 26,
            /// <summary>
            /// 0x0000000008000000
            /// </summary>
            Bit27 = 1 << 27,
            /// <summary>
            /// 0x0000000010000000
            /// </summary>
            Bit28 = 1 << 28,
            /// <summary>
            /// 0x0000000020000000
            /// </summary>
            Bit29 = 1 << 29,
            /// <summary>
            /// 0x0000000040000000
            /// </summary>
            Bit30 = 1L << 30,
            /// <summary>
            /// 0x0000000080000000
            /// </summary>
            Bit31 = 1L << 31,
            /// <summary>
            /// 0x0000000100000000
            /// </summary>
            Bit32 = 1L << 32,
            /// <summary>
            /// 0x0000000200000000
            /// </summary>
            Bit33 = 1L << 33,
            /// <summary>
            /// 0x0000000400000000
            /// </summary>
            Bit34 = 1L << 34,
            /// <summary>
            /// 0x0000000800000000
            /// </summary>
            Bit35 = 1L << 35,
            /// <summary>
            /// 0x0000001000000000
            /// </summary>
            Bit36 = 1L << 36,
            /// <summary>
            /// 0x0000002000000000
            /// </summary>
            Bit37 = 1L << 37,
            /// <summary>
            /// 0x0000004000000000
            /// </summary>
            Bit38 = 1L << 38,
            /// <summary>
            /// 0x0000008000000000
            /// </summary>
            Bit39 = 1L << 39,
            /// <summary>
            /// 0x0000010000000000
            /// </summary>
            Bit40 = 1L << 40,
            /// <summary>
            /// 0x0000020000000000
            /// </summary>
            Bit41 = 1L << 41,
            /// <summary>
            /// 0x0000040000000000
            /// </summary>
            Bit42 = 1L << 42,
            /// <summary>
            /// 0x0000080000000000
            /// </summary>
            Bit43 = 1L << 43,
            /// <summary>
            /// 0x0000100000000000
            /// </summary>
            Bit44 = 1L << 44,
            /// <summary>
            /// 0x0000200000000000
            /// </summary>
            Bit45 = 1L << 45,
            /// <summary>
            /// 0x0000400000000000
            /// </summary>
            Bit46 = 1L << 46,
            /// <summary>
            /// 0x0000800000000000
            /// </summary>
            Bit47 = 1L << 47,
            /// <summary>
            /// 0x0001000000000000
            /// </summary>
            Bit48 = 1L << 48,
            /// <summary>
            /// 0x0002000000000000
            /// </summary>
            Bit49 = 1L << 49,
            /// <summary>
            /// 0x0004000000000000
            /// </summary>
            Bit50 = 1L << 50,
            /// <summary>
            /// 0x0008000000000000
            /// </summary>
            Bit51 = 1L << 51,
            /// <summary>
            /// 0x0010000000000000
            /// </summary>
            Bit52 = 1L << 52,
            /// <summary>
            /// 0x0020000000000000
            /// </summary>
            Bit53 = 1L << 53,
            /// <summary>
            /// 0x0040000000000000
            /// </summary>
            Bit54 = 1L << 54,
            /// <summary>
            /// 0x0080000000000000
            /// </summary>
            Bit55 = 1L << 55,
            /// <summary>
            /// 0x0100000000000000
            /// </summary>
            Bit56 = 1L << 56,
            /// <summary>
            /// 0x0200000000000000
            /// </summary>
            Bit57 = 1L << 57,
            /// <summary>
            /// 0x0400000000000000
            /// </summary>
            Bit58 = 1L << 58,
            /// <summary>
            /// 0x0800000000000000
            /// </summary>
            Bit59 = 1L << 59,
            /// <summary>
            /// 0x1000000000000000
            /// </summary>
            Bit60 = 1L << 60,
            /// <summary>
            /// 0x2000000000000000
            /// </summary>
            Bit61 = 1L << 61,
            /// <summary>
            /// 0x4000000000000000
            /// </summary>
            Bit62 = 1L << 62,
            /// <summary>
            /// 0x8000000000000000
            /// </summary>
            Bit63 = 1L << 63
        }

        private static int toByte(int bits)
        {
            if (bits < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(bits), bits, "The number of bits is out of range.");
            }

            return ((bits - 1) >> 3) + 1;
        }

        /// <summary>
        /// ビット深度に対応した数値変換機能を選択する。
        /// </summary>
        /// <param name="bitDepth">ビット深度</param>
        /// <returns>数値変換関数(ByteArray, Index)</returns>
        public static Func<byte[], int, uint> SelectConverter(int bitDepth)
        {
            int byteLength = toByte(bitDepth);

            return bitConvertTable[byteLength];
        }

        /// <summary>
        /// Bit->Byte数変換
        /// </summary>
        /// <param name="bits">ビット数</param>
        /// <returns>バイト数</returns>
        /// <exception cref="ArgumentOutOfRangeException">ビット数が1未満の場合に発生。</exception>
        public static int ToByte(int bits)
        {
            return toByte(bits);
        }
    }
}
