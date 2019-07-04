using UnityEditor;

namespace UnityGameFrame.Editor.AssetBundleTools
{
    /// <summary>
    /// 资源信息
    /// </summary>
    public sealed class AssetInfo
    {
        public string Guid { get; private set; }

        public string Name { get { return AssetDatabase.GUIDToAssetPath(Guid); } }

        /// <summary>
        /// 所属的Bundle
        /// </summary>
        public AssetBundleInfo AssetBundleInfo { get; private set; }

        private AssetInfo(string guid, AssetBundleInfo assetBundleInfo)
        {
            Guid = guid;
            AssetBundleInfo = assetBundleInfo;
        }

        public void SetAssetBundle(AssetBundleInfo assetBundleInfo)
        {
            AssetBundleInfo = assetBundleInfo;
        }

        public static AssetInfo Create(string guid)
        {
            return new AssetInfo(guid, null);
        }

        public static AssetInfo Create(string guid, AssetBundleInfo assetBundleInfo)
        {
            return new AssetInfo(guid, assetBundleInfo);
        }
    }
}
