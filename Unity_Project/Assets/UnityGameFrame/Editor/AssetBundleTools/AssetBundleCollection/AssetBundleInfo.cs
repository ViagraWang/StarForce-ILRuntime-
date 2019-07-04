using GameFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityGameFrame.Editor.AssetBundleTools
{
    /// <summary>
    /// 资源包信息
    /// </summary>
    public sealed class AssetBundleInfo
    {
        private readonly List<AssetInfo> m_Assets;  //Bundle关联的资源列表
        private readonly List<string> m_ResourceGroups; //资源组

        /// <summary>
        /// 资源信息数组
        /// </summary>
        public AssetInfo[] AssetInfos { get { return m_Assets.ToArray(); } }

        /// <summary>
        /// Bundle名称
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        ///变体名称
        /// </summary>
        public string Variant { get; private set; }

        /// <summary>
        /// Bundle全名
        /// </summary>
        public string FullName
        {
            get { return Variant != null ? Utility.Text.Format("{0}.{1}", Name, Variant) : Name; }
        }

        /// <summary>
        /// 资源包类型
        /// </summary>
        public AssetBundleType Type { get; private set; }

        /// <summary>
        /// 资源加载方式
        /// </summary>
        public AssetBundleLoadType LoadType { get; private set; }

        /// <summary>
        /// 是否打包
        /// </summary>
        public bool Packed { get; private set; }

        private AssetBundleInfo(string name, string variant, AssetBundleLoadType loadType, bool packed, string[] resourceGroups)
        {
            m_Assets = new List<AssetInfo>();
            m_ResourceGroups = new List<string>();

            Name = name;
            Variant = variant;
            Type = AssetBundleType.Unknown;
            LoadType = loadType;
            Packed = packed;

            for (int i = 0; i < resourceGroups.Length; i++)
            {
                AddResourceGroup(resourceGroups[i]);
            }
        }

        /// <summary>
        /// 重命名
        /// </summary>
        /// <param name="name">新名称</param>
        /// <param name="variant">新变体名</param>
        public void Rename(string name, string variant)
        {
            Name = name;
            Variant = variant;
        }

        /// <summary>
        /// 设置bundle加载方式
        /// </summary>
        /// <param name="loadType"></param>
        public void SetLoadType(AssetBundleLoadType loadType)
        {
            LoadType = loadType;
        }

        /// <summary>
        /// 设置是否打包
        /// </summary>
        /// <param name="packed">打包的标志位</param>
        public void SetPacked(bool packed)
        {
            Packed = packed;
        }

        /// <summary>
        /// 分配资源
        /// </summary>
        /// <param name="assetInfo"></param>
        /// <param name="isScene"></param>
        public void AssignAssetInfo(AssetInfo assetInfo, bool isScene)
        {
            if (assetInfo.AssetBundleInfo != null)
                assetInfo.AssetBundleInfo.Unassign(assetInfo);  //先移除

            Type = isScene ? AssetBundleType.Scene : AssetBundleType.Asset;
            assetInfo.SetAssetBundle(this);
            m_Assets.Add(assetInfo);
            m_Assets.Sort(AssetComparer);   //排序
        }

        //移除
        public void Unassign(AssetInfo assetInfo)
        {
            assetInfo.SetAssetBundle(null);
            m_Assets.Remove(assetInfo);
            //检查数量是否为0
            if (m_Assets.Count <= 0)
                Type = AssetBundleType.Unknown;
        }

        //获取资源组名称数组
        public string[] GetResourceGroups()
        {
            return m_ResourceGroups.ToArray();
        }

        //检查是否存在资源组
        public bool HasResourceGroup(string resourceGroup)
        {
            if (string.IsNullOrEmpty(resourceGroup))
                return false;

            return m_ResourceGroups.Contains(resourceGroup);
        }

        //添加资源组
        public void AddResourceGroup(string resourceGroup)
        {
            if (string.IsNullOrEmpty(resourceGroup))
                return;

            if (m_ResourceGroups.Contains(resourceGroup))
                return;

            m_ResourceGroups.Add(resourceGroup);
            m_ResourceGroups.Sort();
        }

        //移除资源组
        public bool RemoveResourceGroup(string resourceGroup)
        {
            if (string.IsNullOrEmpty(resourceGroup))
                return false;

            return m_ResourceGroups.Remove(resourceGroup);
        }

        //清除
        public void Clear()
        {
            foreach (var asset in m_Assets)
            {
                asset.SetAssetBundle(null);
            }

            m_Assets.Clear();
            m_ResourceGroups.Clear();
            Type = AssetBundleType.Unknown;
        }

        private int AssetComparer(AssetInfo a, AssetInfo b)
        {
            return a.Guid.CompareTo(b.Guid);
        }

        public static AssetBundleInfo Create(string name, string variant, AssetBundleLoadType loadType, bool packed, string[] resourceGroups)
        {
            return new AssetBundleInfo(name, variant, loadType, packed, resourceGroups);
        }
        
    }
}
