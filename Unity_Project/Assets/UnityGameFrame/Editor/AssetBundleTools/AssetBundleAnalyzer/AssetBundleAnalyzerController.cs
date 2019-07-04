using GameFramework;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UnityGameFrame.Editor.AssetBundleTools
{
    public sealed partial class AssetBundleAnalyzerController
    {
        private readonly AssetBundleCollection m_AssetBundleCollection; //资源包的容器

        private readonly Dictionary<string, DependencyData> m_DependencyDatas;  //依赖资源数据
        private readonly Dictionary<string, List<AssetInfo>> m_ScatteredAssetInfos; //零散资源信息
        private readonly List<string[]> m_CircularDependencyDatas;    //间接依赖数据
        private readonly HashSet<Stamp> m_AnalyzedStamps;

        public event GameFrameworkAction<int, int> EventOnLoadingAssetBundle = null;    //加载Bundle中回调
        public event GameFrameworkAction<int, int> EventOnLoadingAsset = null;  //加载资源中回调
        public event GameFrameworkAction EventOnLoadCompleted = null;   //加载完成回调
        public event GameFrameworkAction<int, int> EventOnAnalyzingAsset = null;    //分析资源回调
        public event GameFrameworkAction EventOnAnalyzeCompleted = null;    //分析完成回调

        public AssetBundleAnalyzerController() : this(null) { }

        public AssetBundleAnalyzerController(AssetBundleCollection assetBundleCollection)
        {
            m_AssetBundleCollection = assetBundleCollection != null ? assetBundleCollection : new AssetBundleCollection();

            //注册事件
            m_AssetBundleCollection.EventOnLoadingAssetBundle += OnLoadingAssetBundle;
            m_AssetBundleCollection.EventOnLoadingAsset += OnLoadingAsset;
            m_AssetBundleCollection.EventOnLoadCompleted += OnLoadCompleted;

            m_DependencyDatas = new Dictionary<string, DependencyData>();
            m_ScatteredAssetInfos = new Dictionary<string, List<AssetInfo>>();
            m_AnalyzedStamps = new HashSet<Stamp>();
            m_CircularDependencyDatas = new List<string[]>();
        }

        //加载Bundle的回调
        private void OnLoadingAssetBundle(int index, int count)
        {
            if (EventOnLoadingAssetBundle != null)
                EventOnLoadingAssetBundle.Invoke(index, count);
        }

        //加载资源的回调
        private void OnLoadingAsset(int index, int count)
        {
            if (EventOnLoadingAsset != null)
                EventOnLoadingAsset.Invoke(index, count);
        }

        //加载完成的回调
        private void OnLoadCompleted()
        {
            if (EventOnLoadCompleted != null)
                EventOnLoadCompleted.Invoke();
        }

        //清空
        public void Clear()
        {
            m_AssetBundleCollection.Clear();
            m_DependencyDatas.Clear();
            m_ScatteredAssetInfos.Clear();
            m_CircularDependencyDatas.Clear();
            m_AnalyzedStamps.Clear();
        }

        //准备资源
        public bool Prepare()
        {
            //m_AssetBundleCollection.Clear();  //无需清空，Load方法内已情况
            return m_AssetBundleCollection.Load();
        }

        //分析
        public void Analyze()
        {
            m_DependencyDatas.Clear();
            m_ScatteredAssetInfos.Clear();
            m_CircularDependencyDatas.Clear();
            m_AnalyzedStamps.Clear();


            HashSet<string> scriptAssetNames = GetFilteredAssetNames("t:Script");   //过滤脚本资源
            AssetInfo[] assets = m_AssetBundleCollection.AllAssetInfos; //所有资源
            int count = assets.Length;
            for (int i = 0; i < count; i++)
            {
                if (EventOnAnalyzingAsset != null)
                    EventOnAnalyzingAsset.Invoke(i, count);

                string assetName = assets[i].Name;
                if (string.IsNullOrEmpty(assetName))
                {
                    Debug.LogWarning(Utility.Text.Format("Can not find asset by guid '{0}'.", assets[i].Guid));
                    continue;
                }

                DependencyData dependencyData = new DependencyData();
                AnalyzeAsset(assetName, assets[i], dependencyData, scriptAssetNames);   //分析资源
                dependencyData.RefreshData();
                m_DependencyDatas.Add(assetName, dependencyData);
            }

            //零散资源排序
            foreach (List<AssetInfo> scatteredAsset in m_ScatteredAssetInfos.Values)
            {
                scatteredAsset.Sort((a, b) => a.Name.CompareTo(b.Name));
            }

            m_CircularDependencyDatas.AddRange(new CircularDependencyChecker(m_AnalyzedStamps.ToArray()).Check());

            if (EventOnAnalyzeCompleted != null)
                EventOnAnalyzeCompleted.Invoke();
        }

        //分析资源
        private void AnalyzeAsset(string assetName ,AssetInfo hostAsset, DependencyData dependencyData, HashSet<string> scriptAssetNames)
        {
            string[] dependencyAssetNames = AssetDatabase.GetDependencies(assetName, false);    //获取依赖资源
            foreach (string dependencyAssetName in dependencyAssetNames)
            {
                if (scriptAssetNames.Contains(dependencyAssetName)) //去除脚本资源
                    continue;

                if (dependencyAssetName == assetName)   //去除资源本身
                    continue;

                if (dependencyAssetName.EndsWith(".unity")) // 忽略对场景的依赖
                    continue;

                Stamp stamp = new Stamp(hostAsset.Name, dependencyAssetName);
                if (m_AnalyzedStamps.Contains(stamp))
                    continue;

                m_AnalyzedStamps.Add(stamp);
                //依赖资源处理
                string guid = AssetDatabase.AssetPathToGUID(dependencyAssetName);
                if (string.IsNullOrEmpty(guid))
                {
                    Debug.LogWarning(Utility.Text.Format("Can not find guid by asset '{0}'.", dependencyAssetName));
                    continue;
                }

                AssetInfo asset = m_AssetBundleCollection.GetAssetInfo(guid);
                if (asset != null)
                {
                    dependencyData.AddDependencyAsset(asset);
                }
                else
                {
                    dependencyData.AddScatteredDependencyAsset(dependencyAssetName);    //设置零散资源

                    List<AssetInfo> scatteredAssets = null;
                    if (!m_ScatteredAssetInfos.TryGetValue(dependencyAssetName, out scatteredAssets))
                    {
                        scatteredAssets = new List<AssetInfo>();
                        m_ScatteredAssetInfos.Add(dependencyAssetName, scatteredAssets);
                    }

                    scatteredAssets.Add(hostAsset);
                    //递归处理
                    AnalyzeAsset(dependencyAssetName, hostAsset, dependencyData, scriptAssetNames);
                }
            }

        }

        //获取资源信息
        public AssetInfo GetAssetInfo(string assetName)
        {
            return m_AssetBundleCollection.GetAssetInfo(AssetDatabase.AssetPathToGUID(assetName));
        }

        //获取资源名
        public string[] GetAssetNames()
        {
            return GetAssetNames(AssetsOrder.AssetNameAsc, null);
        }

        public string[] GetAssetNames(AssetsOrder order, string filter)
        {
            HashSet<string> filteredAssetNames = GetFilteredAssetNames(filter);
            IEnumerable<KeyValuePair<string, DependencyData>> filteredResult = m_DependencyDatas.Where(pair => filteredAssetNames.Contains(pair.Key));
            IEnumerable<KeyValuePair<string, DependencyData>> orderedResult = null;
            switch (order)
            {
                case AssetsOrder.AssetNameAsc:
                    orderedResult = filteredResult.OrderBy(pair => pair.Key);
                    break;
                case AssetsOrder.AssetNameDesc:
                    orderedResult = filteredResult.OrderByDescending(pair => pair.Key);
                    break;
                case AssetsOrder.DependencyAssetBundleCountAsc:
                    orderedResult = filteredResult.OrderBy(pair => pair.Value.DependencyAssetBundleInfoCount);
                    break;
                case AssetsOrder.DependencyAssetBundleCountDesc:
                    orderedResult = filteredResult.OrderByDescending(pair => pair.Value.DependencyAssetBundleInfoCount);
                    break;
                case AssetsOrder.DependencyAssetCountAsc:
                    orderedResult = filteredResult.OrderBy(pair => pair.Value.DependencyAssetInfoCount);
                    break;
                case AssetsOrder.DependencyAssetCountDesc:
                    orderedResult = filteredResult.OrderByDescending(pair => pair.Value.DependencyAssetInfoCount);
                    break;
                case AssetsOrder.ScatteredDependencyAssetCountAsc:
                    orderedResult = filteredResult.OrderBy(pair => pair.Value.ScatteredDependencyAssetCount);
                    break;
                case AssetsOrder.ScatteredDependencyAssetCountDesc:
                    orderedResult = filteredResult.OrderByDescending(pair => pair.Value.ScatteredDependencyAssetCount);
                    break;
                default:
                    orderedResult = filteredResult;
                    break;
            }

            return orderedResult.Select(pair => pair.Key).ToArray();
        }

        //获取依赖资源数据
        public DependencyData GetDependencyData(string assetName)
        {
            DependencyData dependencyData = null;
            if (m_DependencyDatas.TryGetValue(assetName, out dependencyData))
            {
                return dependencyData;
            }

            return dependencyData;
        }

        //获取零散依赖资源名
        public string[] GetScatteredAssetNames()
        {
            return GetScatteredAssetNames(ScatteredAssetsOrder.HostAssetCountDesc, null);
        }

        public string[] GetScatteredAssetNames(ScatteredAssetsOrder order, string filter)
        {
            HashSet<string> filterAssetNames = GetFilteredAssetNames(filter);
            IEnumerable<KeyValuePair<string, List<AssetInfo>>> filteredResult = m_ScatteredAssetInfos.Where(pair => filterAssetNames.Contains(pair.Key) && pair.Value.Count > 1);
            IEnumerable<KeyValuePair<string, List<AssetInfo>>> orderedResult = null;
            switch (order)
            {
                case ScatteredAssetsOrder.AssetNameAsc:
                    orderedResult = filteredResult.OrderBy(pair => pair.Key);
                    break;
                case ScatteredAssetsOrder.AssetNameDesc:
                    orderedResult = filteredResult.OrderByDescending(pair => pair.Key);
                    break;
                case ScatteredAssetsOrder.HostAssetCountAsc:
                    orderedResult = filteredResult.OrderBy(pair => pair.Value.Count);
                    break;
                case ScatteredAssetsOrder.HostAssetCountDesc:
                    orderedResult = filteredResult.OrderByDescending(pair => pair.Value.Count);
                    break;
                default:
                    orderedResult = filteredResult;
                    break;
            }

            return orderedResult.Select(pair => pair.Key).ToArray();
        }

        //获取依赖零散资源的主资源数组
        public AssetInfo[] GetHostAssets(string scatteredAssetName)
        {
            List<AssetInfo> assets = null;
            if (m_ScatteredAssetInfos.TryGetValue(scatteredAssetName, out assets))
            {
                return assets.ToArray();
            }

            return null;
        }

        //获取间接依赖资源数据
        public string[][] GetCircularDependencyDatas()
        {
            return m_CircularDependencyDatas.ToArray();
        }

        //获取过滤的资源名称
        public HashSet<string> GetFilteredAssetNames(string filter)
        {
            string[] filterAssetGuids = AssetDatabase.FindAssets(filter);
            HashSet<string> filterAssetNames = new HashSet<string>();
            foreach (string filterAssetGuid in filterAssetGuids)
            {
                filterAssetNames.Add(AssetDatabase.GUIDToAssetPath(filterAssetGuid));
            }

            return filterAssetNames;
        }

    }
}
