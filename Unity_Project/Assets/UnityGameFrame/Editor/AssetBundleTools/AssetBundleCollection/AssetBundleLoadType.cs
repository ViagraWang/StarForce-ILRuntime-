
namespace UnityGameFrame.Editor.AssetBundleTools
{
    /// <summary>
    /// 资源加载方式类型
    /// </summary>
    public enum AssetBundleLoadType
    {
        /// <summary>
        /// 从文件加载。
        /// </summary>
        LoadFromFile = 0,

        /// <summary>
        /// 从内存加载。
        /// </summary>
        LoadFromMemory,

        /// <summary>
        /// 从内存快速解密加载。
        /// </summary>
        LoadFromMemoryAndQuickDecrypt,

        /// <summary>
        /// 从内存解密加载。
        /// </summary>
        LoadFromMemoryAndDecrypt,
    }
}
