using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UnityGameFrame.Editor.AssetBundleTools
{
    /// <summary>
    /// 依赖资源数据
    /// </summary>
    public sealed class DependencyData
    {
        private List<AssetBundleInfo> m_DependencyAssetBundleInfos; //依赖Bundle信息列表
        private List<AssetInfo> m_DependencyAssetInfos; //依赖资源信息列表
        private List<string> m_ScatteredDependencyAssetNames;   //零散的依赖资源名

        public int DependencyAssetBundleInfoCount { get { return m_DependencyAssetBundleInfos.Count; } }

        public int DependencyAssetInfoCount { get { return m_DependencyAssetInfos.Count; } }

        public int ScatteredDependencyAssetCount { get { return m_ScatteredDependencyAssetNames.Count; } }

        //所有依赖Bundle资源包信息
        public AssetBundleInfo[] AllDependencyAssetBundleInfos { get { return m_DependencyAssetBundleInfos.ToArray(); } }

        //所有依赖资源信息
        public AssetInfo[] AllDependencyAssetInfos { get { return m_DependencyAssetInfos.ToArray(); } }

        //所有零散依赖资源包名
        public string[] AllScatteredDependencyAssetNames { get { return m_ScatteredDependencyAssetNames.ToArray(); } }

        public DependencyData()
        {
            m_DependencyAssetBundleInfos = new List<AssetBundleInfo>();
            m_DependencyAssetInfos = new List<AssetInfo>();
            m_ScatteredDependencyAssetNames = new List<string>();
        }

        //添加依赖资源
        public void AddDependencyAsset(AssetInfo asset)
        {
            if (!m_DependencyAssetBundleInfos.Contains(asset.AssetBundleInfo))
            {
                m_DependencyAssetBundleInfos.Add(asset.AssetBundleInfo);
            }

            m_DependencyAssetInfos.Add(asset);
        }

        //添加零散的依赖资源包名
        public void AddScatteredDependencyAsset(string dependencyAssetName)
        {
            m_ScatteredDependencyAssetNames.Add(dependencyAssetName);
        }

        //刷新，排序
        public void RefreshData()
        {
            m_DependencyAssetBundleInfos.Sort(DependencyAssetBundlesComparer);
            m_DependencyAssetInfos.Sort(DependencyAssetsComparer);
            m_ScatteredDependencyAssetNames.Sort();
        }

        private int DependencyAssetBundlesComparer(AssetBundleInfo a, AssetBundleInfo b)
        {
            return a.FullName.CompareTo(b.FullName);
        }

        private int DependencyAssetsComparer(AssetInfo a, AssetInfo b)
        {
            return a.Name.CompareTo(b.Name);
        }
    }
}
