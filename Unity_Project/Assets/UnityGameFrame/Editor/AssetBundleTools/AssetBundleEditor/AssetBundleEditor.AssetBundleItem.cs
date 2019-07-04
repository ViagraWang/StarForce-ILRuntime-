using GameFramework;
using UnityEditor;
using UnityEngine;

namespace UnityGameFrame.Editor.AssetBundleTools
{
    internal sealed partial class AssetBundleEditor
    {
        //Bundle项
        private sealed class AssetBundleItem
        {
            private static Texture s_CachedUnknownIcon = null;
            private static Texture s_CachedAssetIcon = null;
            private static Texture s_CachedSceneIcon = null;

            public AssetBundleItem(string name, AssetBundleInfo assetBundleInfo, AssetBundleFolder folder)
            {
                if (assetBundleInfo == null)
                {
                    throw new GameFrameworkException("AssetBundle is invalid.");
                }

                if (folder == null)
                {
                    throw new GameFrameworkException("AssetBundle folder is invalid.");
                }

                Name = name;
                AssetBundleInfo = assetBundleInfo;
                Folder = folder;
            }

            public string Name
            {
                get;
                private set;
            }

            public AssetBundleInfo AssetBundleInfo
            {
                get;
                private set;
            }

            public AssetBundleFolder Folder
            {
                get;
                private set;
            }

            public string FromRootPath
            {
                get
                {
                    return (Folder.Folder == null ? Name : Utility.Text.Format("{0}/{1}", Folder.FromRootPath, Name));
                }
            }

            public int Depth
            {
                get
                {
                    return Folder != null ? Folder.Depth + 1 : 0;
                }
            }

            public Texture Icon
            {
                get
                {
                    switch (AssetBundleInfo.Type)
                    {
                        case AssetBundleType.Asset:
                            return CachedAssetIcon;

                        case AssetBundleType.Scene:
                            return CachedSceneIcon;

                        default:
                            return CachedUnknownIcon;
                    }
                }
            }

            private static Texture CachedUnknownIcon
            {
                get
                {
                    if (s_CachedUnknownIcon == null)
                    {
                        string iconName = null;
#if UNITY_2018_3_OR_NEWER
                        iconName = "GameObject Icon";
#else
                        iconName = "Prefab Icon";
#endif
                        s_CachedUnknownIcon = GetIcon(iconName);
                    }

                    return s_CachedUnknownIcon;
                }
            }

            private static Texture CachedAssetIcon
            {
                get
                {
                    if (s_CachedAssetIcon == null)
                    {
                        string iconName = null;
#if UNITY_2018_3_OR_NEWER
                        iconName = "Prefab Icon";
#else
                        iconName = "PrefabNormal Icon";
#endif
                        s_CachedAssetIcon = GetIcon(iconName);
                    }

                    return s_CachedAssetIcon;
                }
            }

            private static Texture CachedSceneIcon
            {
                get
                {
                    if (s_CachedSceneIcon == null)
                    {
                        s_CachedSceneIcon = GetIcon("SceneAsset Icon");
                    }

                    return s_CachedSceneIcon;
                }
            }

            private static Texture GetIcon(string iconName)
            {
                return EditorGUIUtility.IconContent(iconName).image;
            }
        }
    }
}
