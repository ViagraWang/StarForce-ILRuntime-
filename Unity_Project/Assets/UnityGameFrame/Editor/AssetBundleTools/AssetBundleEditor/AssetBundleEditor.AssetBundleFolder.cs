using GameFramework;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityGameFrame.Editor.AssetBundleTools
{
    internal sealed partial class AssetBundleEditor
    {
        //Bundle文件夹
        private sealed class AssetBundleFolder
        {
            private static Texture s_CachedIcon = null; //缓存图标

            private readonly List<AssetBundleFolder> m_Folders;
            private readonly List<AssetBundleItem> m_Items;

            public AssetBundleFolder(string name, AssetBundleFolder folder)
            {
                m_Folders = new List<AssetBundleFolder>();
                m_Items = new List<AssetBundleItem>();

                Name = name;
                Folder = folder;
            }

            //文件夹名称
            public string Name { get; private set; }

            //父级Bundle文件夹
            public AssetBundleFolder Folder { get; private set; }

            //从根路径开始的路径
            public string FromRootPath
            {
                get
                {
                    return Folder == null ? string.Empty : (Folder.Folder == null ? Name : Utility.Text.Format("{0}/{1}", Folder.FromRootPath, Name));
                }
            }

            //深度
            public int Depth
            {
                get
                {
                    return Folder != null ? Folder.Depth + 1 : 0;
                }
            }

            //图标
            public static Texture Icon
            {
                get
                {
                    if (s_CachedIcon == null)
                    {
                        s_CachedIcon = AssetDatabase.GetCachedIcon("Assets");
                    }

                    return s_CachedIcon;
                }
            }

            public void Clear()
            {
                m_Folders.Clear();
                m_Items.Clear();
            }

            //获取所有的Bundle文件夹
            public AssetBundleFolder[] GetFolders()
            {
                return m_Folders.ToArray();
            }

            //获取一个子Bundle文件夹
            public AssetBundleFolder GetFolder(string name)
            {
                if (string.IsNullOrEmpty(name))
                    throw new GameFrameworkException("AssetBundle folder name is invalid.");

                foreach (AssetBundleFolder folder in m_Folders)
                {
                    if (folder.Name == name)
                    {
                        return folder;
                    }
                }

                return null;
            }

            //增加一个子Bundle文件夹
            public AssetBundleFolder AddFolder(string name)
            {
                if (string.IsNullOrEmpty(name))
                    throw new GameFrameworkException("AssetBundle folder name is invalid.");

                AssetBundleFolder folder = GetFolder(name);
                if (folder != null)
                    throw new GameFrameworkException("AssetBundle folder is already exist.");

                folder = new AssetBundleFolder(name, this);
                m_Folders.Add(folder);

                return folder;
            }

            //获取所有的Bundle资源项
            public AssetBundleItem[] GetItems()
            {
                return m_Items.ToArray();
            }

            //获取一个Bundle资源项
            public AssetBundleItem GetItem(string name)
            {
                if (string.IsNullOrEmpty(name))
                    throw new GameFrameworkException("AssetBundle item name is invalid.");

                foreach (AssetBundleItem item in m_Items)
                {
                    if (item.Name == name)
                    {
                        return item;
                    }
                }

                return null;
            }

            //添加一个Bundle资源项
            public void AddItem(string name, AssetBundleInfo assetBundleInfo)
            {
                AssetBundleItem item = GetItem(name);
                if (item != null)
                    throw new GameFrameworkException("AssetBundle item is already exist.");

                item = new AssetBundleItem(name, assetBundleInfo, this);
                m_Items.Add(item);
                m_Items.Sort(AssetBundleItemComparer);
            }

            private int AssetBundleItemComparer(AssetBundleItem a, AssetBundleItem b)
            {
                return a.Name.CompareTo(b.Name);
            }
        }
    }
}
