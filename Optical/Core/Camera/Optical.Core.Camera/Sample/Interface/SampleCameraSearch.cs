using System.Collections.Generic;

namespace Optical.Core.Camera
{
    /// <summary>
    /// 接続されているカメラを検索する機能を提供します。
    /// </summary>
    internal class SampleCameraSearch : ICameraSearch
    {
        /// <summary>
        /// 接続されているカメラのデバイス情報一覧を出力します。
        /// </summary>
        /// <returns>カメラデバイス情報一覧</returns>
        public IReadOnlyList<CameraComponents> EnumerateDevice()
        {
            return SampleCameraWrapper.EnumerateDevice(typeof(SampleCamera).AssemblyQualifiedName ?? string.Empty);
        }
    }
}
