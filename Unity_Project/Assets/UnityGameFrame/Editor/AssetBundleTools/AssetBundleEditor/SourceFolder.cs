using GameFramework;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityGameFrame.Editor.AssetBundleTools
{
    //资源文件夹
    public sealed class SourceFolder
    {
        private static Texture s_CachedIcon = null; //缓存图标

        private readonly List<SourceFolder> m_Folders;  //文件夹列表
        private readonly List<SourceAsset> m_Assets;    //文件列表

        public SourceFolder(string name, SourceFolder folder)
        {
            m_Folders = new List<SourceFolder>();
            m_Assets = new List<SourceAsset>();

            Name = name;
            Folder = folder;
        }

        public string Name
        {
            get;
            private set;
        }

        //上级文件夹
        public SourceFolder Folder { get; private set; }

        public string FromRootPath
        {
            get
            {
                return Folder == null ? string.Empty : (Folder.Folder == null ? Name : Utility.Text.Format("{0}/{1}", Folder.FromRootPath, Name));
            }
        }

        public int Depth
        {
            get
            {
                return Folder != null ? Folder.Depth + 1 : 0;
            }
        }

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
            m_Assets.Clear();
        }

        //获取所有文件夹数组
        public SourceFolder[] GetFolders()
        {
            return m_Folders.ToArray();
        }

        //获取文件夹
        public SourceFolder GetFolder(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new GameFrameworkException("Source folder name is invalid.");

            foreach (SourceFolder folder in m_Folders)
            {
                if (folder.Name == name)
                    return folder;
            }

            return null;
        }

        //添加文件夹
        public SourceFolder AddFolder(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new GameFrameworkException("Source folder name is invalid.");

            SourceFolder folder = GetFolder(name);
            if (folder != null)
                throw new GameFrameworkException("Source folder is already exist.");

            folder = new SourceFolder(name, this);
            m_Folders.Add(folder);

            return folder;
        }

        //获取所有资源
        public SourceAsset[] GetAssets()
        {
            return m_Assets.ToArray();
        }

        //根据资源名获取资源
        public SourceAsset GetAsset(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new GameFrameworkException("Source asset name is invalid.");

            foreach (SourceAsset asset in m_Assets)
            {
                if (asset.Name == name)
                {
                    return asset;
                }
            }

            return null;
        }

        //添加资源
        public SourceAsset AddAsset(string guid, string path, string name)
        {
            if (string.IsNullOrEmpty(guid))
                throw new GameFrameworkException("Source asset guid is invalid.");

            if (string.IsNullOrEmpty(path))
                throw new GameFrameworkException("Source asset path is invalid.");

            if (string.IsNullOrEmpty(name))
                throw new GameFrameworkException("Source asset name is invalid.");

            SourceAsset asset = GetAsset(name); //已存在
            if (asset != null)
                throw new GameFrameworkException(Utility.Text.Format("Source asset '{0}' is already exist.", name));

            asset = new SourceAsset(guid, path, name, this);
            m_Assets.Add(asset);

            return asset;
        }
    }
}
