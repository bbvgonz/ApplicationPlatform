using System;
using System.Collections.Generic;
using System.Text;

namespace Optical.Core.Camera
{
    /// <summary>
    /// 接続されているカメラを検索する機能を提供します。
    /// </summary>
    public static class CameraSearch
    {
        /// <summary>
        /// 製造元毎のカメラ検索機能テーブル
        /// </summary>
        private static Dictionary<VendorIdentifier, ICameraSearch> vendorTable;

        static CameraSearch()
        {
            vendorTable = new Dictionary<VendorIdentifier, ICameraSearch>
            {
                [VendorIdentifier.Sample] = new SampleCameraSearch(),
            };
        }

        /// <summary>
        /// 接続されているカメラのデバイス情報一覧を出力します。
        /// </summary>
        /// <returns>カメラデバイス情報一覧</returns>
        public static IReadOnlyList<CameraComponents> EnumerateDevice()
        {
            var devices = new List<CameraComponents>();
            foreach (ICameraSearch finder in vendorTable.Values)
            {
                IReadOnlyList<CameraComponents> list = finder.EnumerateDevice();
                if (list.Count > 0)
                {
                    devices.AddRange(list);
                }
            }

            return devices.AsReadOnly();
        }
    }

    /// <summary>
    /// カメラデバイス識別情報
    /// </summary>
    /// <seealso cref="Connection.UsbComponents"/>
    /// <seealso cref="Connection.CameraLinkComponents"/>
    /// <seealso cref="Connection.EtherComponents"/>
    public class CameraComponents
    {
        #region Constructors
        /// <summary>
        /// カメラデバイス識別情報の新しいインスタンスを生成します。
        /// </summary>
        /// <param name="className">カメラインスタンスを生成するクラスの<see cref="Type.AssemblyQualifiedName"/></param>
        internal CameraComponents(string className)
        {
            Identifier = className;
            Usb = new Connection.UsbComponents();
            Ether = new Connection.EtherComponents();
            CameraLink = new Connection.CameraLinkComponents();
        }
        #endregion // Constructors

        #region Properties
        /// <summary>
        /// カメラ識別子
        /// </summary>
        /// <remarks>カメラインスタンスを生成するクラスの<see cref="Type.AssemblyQualifiedName"/>を設定する。</remarks>
        public string Identifier { get; }

        /// <summary>
        /// ベンダー名称
        /// </summary>
        public string VendorName { get; internal set; } = string.Empty;

        /// <summary>
        /// モデル型式
        /// </summary>
        public string ModelName { get; internal set; } = string.Empty;

        /// <summary>
        /// デバイスID
        /// </summary>
        public string DeviceId { get; internal set; } = string.Empty;

        /// <summary>
        /// 接続形式
        /// </summary>
        public Connection.Terminal ConnectionType { get; internal set; }

        /// <summary>
        ///  シリアルナンバー
        /// </summary>
        public string SerialNumber { get; internal set; } = string.Empty;

        /// <summary>
        /// USB接続情報
        /// </summary>
        public Connection.UsbComponents Usb { get; }

        /// <summary>
        /// イーサネット接続情報
        /// </summary>
        public Connection.EtherComponents Ether { get; }

        /// <summary>
        /// カメラリンク接続情報
        /// </summary>
        public Connection.CameraLinkComponents CameraLink { get; }
        #endregion // Properties

        #region Methods
        /// <summary>
        /// カメラデバイス管理キー生成。
        /// </summary>
        /// <returns></returns>
        public string GenerateKey()
        {
            return Convert.ToBase64String(Encoding.GetEncoding("UTF-8").GetBytes($"{VendorName},{ModelName},{DeviceId},{SerialNumber}"));
        }
        #endregion // Methods
    }

    namespace Connection
    {
        #region Enums
        /// <summary>
        /// 接続端子種別
        /// </summary>
        public enum Terminal
        {
            /// <summary>
            /// カメラリンク
            /// </summary>
            CameraLink,
            /// <summary>
            /// イーサネット
            /// </summary>
            GigE,
            /// <summary>
            /// IEEE1394
            /// </summary>
            IEEE1394,
            /// <summary>
            /// USB
            /// </summary>
            Usb,
            /// <summary>
            /// 不明
            /// </summary>
            Unknown
        }
        #endregion // Enums

        #region Classes
        /// <summary>
        /// USB接続情報
        /// </summary>
        public class UsbComponents
        {
            internal UsbComponents() { }

            /// <summary>
            /// Globally Unique Identifier
            /// </summary>
            public string GUID { get; internal set; } = string.Empty;
            /// <summary>
            /// デバイスインデックス
            /// </summary>
            public string Index { get; internal set; } = string.Empty;
            /// <summary>
            /// ドライバーキー名称
            /// </summary>
            public string DriverKey { get; internal set; } = string.Empty;
            /// <summary>
            /// デバイス製造元
            /// </summary>
            public string Manufacture { get; internal set; } = string.Empty;
            /// <summary>
            /// 製品識別子
            /// </summary>
            public string ProductId { get; internal set; } = string.Empty;
            /// <summary>
            /// USBポートバージョン
            /// </summary>
            public string PortVersion { get; internal set; } = string.Empty;
            /// <summary>
            /// ベンダー識別子
            /// </summary>
            public string VendorId { get; internal set; } = string.Empty;
        }

        /// <summary>
        /// イーサネット接続情報
        /// </summary>
        public class EtherComponents
        {
            internal EtherComponents() { }

            /// <summary>
            /// デフォルトゲートウェイ
            /// </summary>
            public string DefaultGateway { get; internal set; } = string.Empty;
            /// <summary>
            /// IPアドレス
            /// </summary>
            public string IpAddress { get; internal set; } = string.Empty;
            /// <summary>
            /// 物理アドレス
            /// </summary>
            public string MacAddress { get; internal set; } = string.Empty;
            /// <summary>
            /// ポート番号
            /// </summary>
            public string PortNumber { get; internal set; } = string.Empty;
            /// <summary>
            /// サブネットマスク
            /// </summary>
            public string SubnetMask { get; internal set; } = string.Empty;
        }

        /// <summary>
        /// カメラリンク接続情報
        /// </summary>
        public class CameraLinkComponents
        {
            internal CameraLinkComponents() { }

            /// <summary>
            /// デバイスID
            /// </summary>
            public string DeviceId { get; internal set; } = string.Empty;
            /// <summary>
            /// ボーレート
            /// </summary>
            public string BaudRate { get; internal set; } = string.Empty;
            /// <summary>
            /// ポート識別子
            /// </summary>
            public string PortId { get; internal set; } = string.Empty;
        }
        #endregion // Classes
    }
}
