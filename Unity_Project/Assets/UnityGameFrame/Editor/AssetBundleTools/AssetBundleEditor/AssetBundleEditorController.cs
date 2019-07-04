using GameFramework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEditor;
using UnityEngine;

namespace UnityGameFrame.Editor.AssetBundleTools
{
    public sealed class AssetBundleEditorController
    {
        private const string DefaultSourceAssetRootPath = "Assets";

        private readonly string m_ConfigurationPath;
        private readonly AssetBundleCollection m_AssetBundleCollection;
        //要搜索资源的所有路径，可为根路径，可为所有配置的子目录
        private readonly List<string> m_SourceAssetSearchPaths;
        //配置资源搜索的子目录（相对于根目录的路径），每个子目录填写一行 SourceAssetSearchPath，若不填则搜索所有子目录。
        private readonly List<string> m_SourceAssetSearchRelativePaths;
        //资源文件信息列表
        private readonly Dictionary<string, SourceAsset> m_SourceAssets;
        private string m_SourceAssetRootPath;
        //资源类型过滤
        private string m_SourceAssetUnionTypeFilter;
        //要筛选并包含的标签类型，默认会包含带有 AssetBundleInclusive 标签（l:AssetBundleInclusive）的资源。
        private string m_SourceAssetUnionLabelFilter;
        //要筛选并排除的资源类型，默认会排除脚本（t:Script）资源
        private string m_SourceAssetExceptTypeFilter;
        //要筛选并排除的标签类型，默认会排除带有 AssetBundleExclusive 标签（l:AssetBundleExclusive）的资源。
        private string m_SourceAssetExceptLabelFilter;
        //编辑器内资源列表排序顺序，可以是 Name（资源文件名）、Path（资源全路径）或者 Guid（资源GUID）。
        //private AssetSorterType m_AssetSorter;

        public AssetBundleEditorController()
        {
            m_ConfigurationPath = Type.GetConfigurationPath<AssetBundleEditorConfigPathAttribute>() ?? Utility.Path.GetCombinePath(Application.dataPath, "GameFramework/Configs/AssetBundleEditor.xml");

            m_AssetBundleCollection = new AssetBundleCollection();

            m_AssetBundleCollection.EventOnLoadingAssetBundle += delegate (int index, int count)
            {
                if (EventOnLoadingAssetBundle != null)
                {
                    EventOnLoadingAssetBundle(index, count);
                }
            };

            m_AssetBundleCollection.EventOnLoadingAsset += delegate (int index, int count)
            {
                if (EventOnLoadingAsset != null)
                {
                    EventOnLoadingAsset(index, count);
                }
            };

            m_AssetBundleCollection.EventOnLoadCompleted += delegate ()
            {
                if (EventOnLoadCompleted != null)
                {
                    EventOnLoadCompleted();
                }
            };

            m_SourceAssetSearchPaths = new List<string>();
            m_SourceAssetSearchRelativePaths = new List<string>();
            m_SourceAssets = new Dictionary<string, SourceAsset>();
            SourceAssetRoot = null;
            m_SourceAssetRootPath = null;
            m_SourceAssetUnionTypeFilter = null;
            m_SourceAssetUnionLabelFilter = null;
            m_SourceAssetExceptTypeFilter = null;
            m_SourceAssetExceptLabelFilter = null;
            AssetSorter = AssetSorterType.Path;

            SourceAssetRootPath = DefaultSourceAssetRootPath;
        }

        public int AssetBundleCount
        {
            get
            {
                return m_AssetBundleCollection.AssetBundleCount;
            }
        }

        public int AssetCount
        {
            get
            {
                return m_AssetBundleCollection.AssetCount;
            }
        }

        //资源根文件夹
        public SourceFolder SourceAssetRoot { get; private set; }

        //编辑器内资源列表排序顺序，可以是 Name（资源文件名）、Path（资源全路径）或者 Guid（资源GUID）。
        public AssetSorterType AssetSorter { get; set; }

        //资源文件根路径
        public string SourceAssetRootPath
        {
            get { return m_SourceAssetRootPath; }
            set
            {
                if (m_SourceAssetRootPath == value)
                    return;

                m_SourceAssetRootPath = value.Replace('\\', '/');
                SourceAssetRoot = new SourceFolder(m_SourceAssetRootPath, null);
                RefreshSourceAssetSearchPaths();
            }
        }

        public event GameFrameworkAction<int, int> EventOnLoadingAssetBundle = null;
        public event GameFrameworkAction<int, int> EventOnLoadingAsset = null;
        public event GameFrameworkAction EventOnLoadCompleted = null;
        public event GameFrameworkAction<SourceAsset[]> EventOnAssetAssigned = null;    //分配资源的事件
        public event GameFrameworkAction<SourceAsset[]> EventOnAssetUnassigned = null;  //取消分配资源的事件

        public bool Load()
        {
            if (!File.Exists(m_ConfigurationPath))
            {
                return false;
            }

            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(m_ConfigurationPath);
                XmlNode xmlRoot = xmlDocument.SelectSingleNode("UnityGameFramework");
                XmlNode xmlEditor = xmlRoot.SelectSingleNode("AssetBundleEditor");
                XmlNode xmlSettings = xmlEditor.SelectSingleNode("Settings");

                XmlNodeList xmlNodeList = null;
                XmlNode xmlNode = null;

                xmlNodeList = xmlSettings.ChildNodes;
                for (int i = 0; i < xmlNodeList.Count; i++)
                {
                    xmlNode = xmlNodeList.Item(i);
                    switch (xmlNode.Name)
                    {
                        case "SourceAssetRootPath": //资源放置的根路径
                            SourceAssetRootPath = xmlNode.InnerText;
                            break;
                        case "SourceAssetSearchPaths":  //相对根目录的子目录
                            m_SourceAssetSearchRelativePaths.Clear();
                            XmlNodeList xmlNodeListInner = xmlNode.ChildNodes;
                            XmlNode xmlNodeInner = null;
                            for (int j = 0; j < xmlNodeListInner.Count; j++)
                            {
                                xmlNodeInner = xmlNodeListInner.Item(j);
                                if (xmlNodeInner.Name != "SourceAssetSearchPath")
                                {
                                    continue;
                                }

                                m_SourceAssetSearchRelativePaths.Add(xmlNodeInner.Attributes.GetNamedItem("RelativePath").Value);
                            }
                            break;
                        case "SourceAssetUnionTypeFilter":  //要筛选并包含的资源类型
                            m_SourceAssetUnionTypeFilter = xmlNode.InnerText;
                            break;
                        case "SourceAssetUnionLabelFilter": //要筛选并包含的标签类型，默认会包含带有 AssetBundleInclusive 标签（l:AssetBundleInclusive）的资源。
                            m_SourceAssetUnionLabelFilter = xmlNode.InnerText;
                            break;
                        case "SourceAssetExceptTypeFilter": //要筛选并排除的资源类型，默认会排除脚本（t:Script）资源
                            m_SourceAssetExceptTypeFilter = xmlNode.InnerText;
                            break;
                        case "SourceAssetExceptLabelFilter":    //要筛选并排除的标签类型，默认会排除带有 AssetBundleExclusive 标签（l:AssetBundleExclusive）的资源。
                            m_SourceAssetExceptLabelFilter = xmlNode.InnerText;
                            break;
                        case "AssetSorter":
                            AssetSorter = (AssetSorterType)Enum.Parse(typeof(AssetSorterType), xmlNode.InnerText);
                            break;
                    }
                }

                RefreshSourceAssetSearchPaths();
            }
            catch
            {
                File.Delete(m_ConfigurationPath);
                return false;
            }

            ScanSourceAssets();

            m_AssetBundleCollection.Load();

            return true;
        }

        //保存配置
        public bool Save()
        {
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.AppendChild(xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null));

                XmlElement xmlRoot = xmlDocument.CreateElement("UnityGameFramework");
                xmlDocument.AppendChild(xmlRoot);

                XmlElement xmlEditor = xmlDocument.CreateElement("AssetBundleEditor");
                xmlRoot.AppendChild(xmlEditor);

                XmlElement xmlSettings = xmlDocument.CreateElement("Settings");
                xmlEditor.AppendChild(xmlSettings);

                XmlElement xmlElement = null;
                XmlAttribute xmlAttribute = null;

                xmlElement = xmlDocument.CreateElement("SourceAssetRootPath");
                xmlElement.InnerText = SourceAssetRootPath.ToString();
                xmlSettings.AppendChild(xmlElement);

                xmlElement = xmlDocument.CreateElement("SourceAssetSearchPaths");
                xmlSettings.AppendChild(xmlElement);

                foreach (string sourceAssetSearchRelativePath in m_SourceAssetSearchRelativePaths)
                {
                    XmlElement xmlElementInner = xmlDocument.CreateElement("SourceAssetSearchPath");
                    xmlAttribute = xmlDocument.CreateAttribute("RelativePath");
                    xmlAttribute.Value = sourceAssetSearchRelativePath;
                    xmlElementInner.Attributes.SetNamedItem(xmlAttribute);
                    xmlElement.AppendChild(xmlElementInner);
                }

                xmlElement = xmlDocument.CreateElement("SourceAssetUnionTypeFilter");
                xmlElement.InnerText = m_SourceAssetUnionTypeFilter ?? string.Empty;
                xmlSettings.AppendChild(xmlElement);
                xmlElement = xmlDocument.CreateElement("SourceAssetUnionLabelFilter");
                xmlElement.InnerText = m_SourceAssetUnionLabelFilter ?? string.Empty;
                xmlSettings.AppendChild(xmlElement);
                xmlElement = xmlDocument.CreateElement("SourceAssetExceptTypeFilter");
                xmlElement.InnerText = m_SourceAssetExceptTypeFilter ?? string.Empty;
                xmlSettings.AppendChild(xmlElement);
                xmlElement = xmlDocument.CreateElement("SourceAssetExceptLabelFilter");
                xmlElement.InnerText = m_SourceAssetExceptLabelFilter ?? string.Empty;
                xmlSettings.AppendChild(xmlElement);
                xmlElement = xmlDocument.CreateElement("AssetSorter");
                xmlElement.InnerText = AssetSorter.ToString();
                xmlSettings.AppendChild(xmlElement);

                string configurationDirectoryName = Path.GetDirectoryName(m_ConfigurationPath);
                if (!Directory.Exists(configurationDirectoryName))
                {
                    Directory.CreateDirectory(configurationDirectoryName);
                }

                xmlDocument.Save(m_ConfigurationPath);
                AssetDatabase.Refresh();
            }
            catch
            {
                if (File.Exists(m_ConfigurationPath))
                {
                    File.Delete(m_ConfigurationPath);
                }

                return false;
            }

            return m_AssetBundleCollection.Save();
        }

        public AssetBundleInfo[] GetAssetBundleInfos()
        {
            return m_AssetBundleCollection.AllAssetBundleInfos;
        }

        public AssetBundleInfo GetAssetBundleInfo(string assetBundleName, string assetBundleVariant)
        {
            return m_AssetBundleCollection.GetAssetBundleInfo(assetBundleName, assetBundleVariant);
        }

        public bool HasAssetBundleInfo(string assetBundleName, string assetBundleVariant)
        {
            return m_AssetBundleCollection.HasAssetBundleInfo(assetBundleName, assetBundleVariant);
        }

        //添加BundleInfo
        public bool AddAssetBundleInfo(string assetBundleName, string assetBundleVariant, AssetBundleLoadType assetBundleLoadType, bool assetBundlePacked)
        {
            return m_AssetBundleCollection.AddAssetBundleInfo(assetBundleName, assetBundleVariant, assetBundleLoadType, assetBundlePacked, new string[0]);
        }

        //重命名BundleInfo
        public bool RenameAssetBundleInfo(string oldAssetBundleName, string oldAssetBundleVariant, string newAssetBundleName, string newAssetBundleVariant)
        {
            return m_AssetBundleCollection.RenameAssetBundleInfo(oldAssetBundleName, oldAssetBundleVariant, newAssetBundleName, newAssetBundleVariant);
        }

        //移除BundleInfo
        public bool RemoveAssetBundleInfo(string assetBundleName, string assetBundleVariant)
        {
            AssetInfo[] assetsToRemove = m_AssetBundleCollection.GetAssetInfos(assetBundleName, assetBundleVariant);    //获取BundleInfo关联的所有资源信息
            if (m_AssetBundleCollection.RemoveAssetBundle(assetBundleName, assetBundleVariant))
            {
                List<SourceAsset> unassignedSourceAssets = new List<SourceAsset>(); //取消分配的资源列表
                foreach (AssetInfo asset in assetsToRemove)
                {
                    SourceAsset sourceAsset = GetSourceAsset(asset.Guid);
                    if (sourceAsset != null)
                        unassignedSourceAssets.Add(sourceAsset);
                }

                if (EventOnAssetUnassigned != null)
                    EventOnAssetUnassigned.Invoke(unassignedSourceAssets.ToArray());

                return true;
            }

            return false;
        }

        //设置Bundle的加载方式
        public bool SetAssetBundleLoadType(string assetBundleName, string assetBundleVariant, AssetBundleLoadType assetBundleLoadType)
        {
            return m_AssetBundleCollection.SetAssetBundleLoadType(assetBundleName, assetBundleVariant, assetBundleLoadType);
        }

        //设置Bundle打包的标志位
        public bool SetAssetBundlePacked(string assetBundleName, string assetBundleVariant, bool assetBundlePacked)
        {
            return m_AssetBundleCollection.SetAssetBundlePacked(assetBundleName, assetBundleVariant, assetBundlePacked);
        }

        //移除无效的bundle资源包
        public int RemoveUnusedAssetBundles()
        {
            List<AssetBundleInfo> assetBundleInfos = new List<AssetBundleInfo>(m_AssetBundleCollection.AllAssetBundleInfos);
            List<AssetBundleInfo> removeAssetBundles = assetBundleInfos.FindAll(assetBundle => GetAssetInfos(assetBundle.Name, assetBundle.Variant).Length <= 0);
            foreach (AssetBundleInfo assetBundleInfo in removeAssetBundles)
            {
                m_AssetBundleCollection.RemoveAssetBundle(assetBundleInfo.Name, assetBundleInfo.Variant);
            }

            return removeAssetBundles.Count;
        }

        public AssetInfo[] GetAssetInfos(string assetBundleName, string assetBundleVariant)
        {
            List<AssetInfo> assets = new List<AssetInfo>(m_AssetBundleCollection.GetAssetInfos(assetBundleName, assetBundleVariant));
            switch (AssetSorter)
            {
                case AssetSorterType.Path:
                    assets.Sort(AssetPathComparer);
                    break;
                case AssetSorterType.Name:
                    assets.Sort(AssetNameComparer);
                    break;
                case AssetSorterType.Guid:
                    assets.Sort(AssetGuidComparer);
                    break;
            }

            return assets.ToArray();
        }

        //获取资源信息
        public AssetInfo GetAsset(string assetGuid)
        {
            return m_AssetBundleCollection.GetAssetInfo(assetGuid);
        }

        //分配资源
        public bool AssignAsset(string assetGuid, string assetBundleName, string assetBundleVariant)
        {
            if (m_AssetBundleCollection.AssignAssetInfo(assetGuid, assetBundleName, assetBundleVariant))
            {
                if (EventOnAssetAssigned != null)
                    EventOnAssetAssigned.Invoke(new SourceAsset[] { GetSourceAsset(assetGuid) });

                return true;
            }

            return false;
        }

        //取消分配资源
        public bool UnassignAssetInfo(string assetGuid)
        {
            if (m_AssetBundleCollection.UnassignAssetInfo(assetGuid))
            {
                SourceAsset sourceAsset = GetSourceAsset(assetGuid);
                if (sourceAsset != null)
                {
                    if (EventOnAssetUnassigned != null)
                        EventOnAssetUnassigned.Invoke(new SourceAsset[] { sourceAsset });
                }

                return true;
            }

            return false;
        }

        //移除没用的资源信息
        public int RemoveUnknownAssetInfos()
        {
            List<AssetInfo> assets = new List<AssetInfo>(m_AssetBundleCollection.AllAssetInfos);
            List<AssetInfo> removeAssets = assets.FindAll(asset => GetSourceAsset(asset.Guid) == null);
            foreach (AssetInfo asset in removeAssets)
            {
                m_AssetBundleCollection.UnassignAssetInfo(asset.Guid);
            }

            return removeAssets.Count;
        }

        public SourceAsset[] GetSourceAssets()
        {
            int count = 0;
            SourceAsset[] sourceAssets = new SourceAsset[m_SourceAssets.Count];
            foreach (KeyValuePair<string, SourceAsset> sourceAsset in m_SourceAssets)
            {
                sourceAssets[count++] = sourceAsset.Value;
            }

            return sourceAssets;
        }

        //获取资源文件信息
        public SourceAsset GetSourceAsset(string assetGuid)
        {
            if (string.IsNullOrEmpty(assetGuid))
                return null;

            SourceAsset sourceAsset = null;
            if (m_SourceAssets.TryGetValue(assetGuid, out sourceAsset))
                return sourceAsset;
            
            return null;
        }

        //搜索资源
        public void ScanSourceAssets()
        {
            m_SourceAssets.Clear();
            SourceAssetRoot.Clear();

            string[] sourceAssetSearchPaths = m_SourceAssetSearchPaths.ToArray();
            HashSet<string> tempGuids = new HashSet<string>();
            tempGuids.UnionWith(AssetDatabase.FindAssets(m_SourceAssetUnionTypeFilter, sourceAssetSearchPaths));    //并集运算
            tempGuids.UnionWith(AssetDatabase.FindAssets(m_SourceAssetUnionLabelFilter, sourceAssetSearchPaths));   //并集运算
            tempGuids.ExceptWith(AssetDatabase.FindAssets(m_SourceAssetExceptTypeFilter, sourceAssetSearchPaths));    //排除运算
            tempGuids.ExceptWith(AssetDatabase.FindAssets(m_SourceAssetExceptLabelFilter, sourceAssetSearchPaths));   //排除运算

            string[] assetGuids = new List<string>(tempGuids).ToArray();
            foreach (string assetGuid in assetGuids)
            {
                string fullPath = AssetDatabase.GUIDToAssetPath(assetGuid);
                if (AssetDatabase.IsValidFolder(fullPath))
                {
                    // Skip folder.
                    continue;
                }

                string assetPath = fullPath.Substring(SourceAssetRootPath.Length + 1);
                string[] splitPath = assetPath.Split('/');
                SourceFolder folder = SourceAssetRoot;
                for (int i = 0; i < splitPath.Length - 1; i++)
                {
                    SourceFolder subFolder = folder.GetFolder(splitPath[i]);
                    folder = subFolder == null ? folder.AddFolder(splitPath[i]) : subFolder;
                }

                SourceAsset asset = folder.AddAsset(assetGuid, fullPath, splitPath[splitPath.Length - 1]);
                m_SourceAssets.Add(asset.Guid, asset);
            }
        }

        //刷新资源查找路径
        private void RefreshSourceAssetSearchPaths()
        {
            m_SourceAssetSearchPaths.Clear();

            if (string.IsNullOrEmpty(m_SourceAssetRootPath))
                SourceAssetRootPath = DefaultSourceAssetRootPath;   //默认路径为Assets

            if (m_SourceAssetSearchRelativePaths.Count > 0)
            {
                foreach (string sourceAssetSearchRelativePath in m_SourceAssetSearchRelativePaths)
                {
                    m_SourceAssetSearchPaths.Add(Utility.Path.GetCombinePath(m_SourceAssetRootPath, sourceAssetSearchRelativePath));
                }
            }
            else
            {
                m_SourceAssetSearchPaths.Add(m_SourceAssetRootPath);
            }
        }

        private int AssetPathComparer(AssetInfo a, AssetInfo b)
        {
            SourceAsset sourceAssetA = GetSourceAsset(a.Guid);
            SourceAsset sourceAssetB = GetSourceAsset(b.Guid);

            if (sourceAssetA != null && sourceAssetB != null)
            {
                return sourceAssetA.Path.CompareTo(sourceAssetB.Path);
            }

            if (sourceAssetA == null && sourceAssetB == null)
            {
                return a.Guid.CompareTo(b.Guid);
            }

            if (sourceAssetA == null)
            {
                return -1;
            }

            if (sourceAssetB == null)
            {
                return 1;
            }

            return 0;
        }

        private int AssetNameComparer(AssetInfo a, AssetInfo b)
        {
            SourceAsset sourceAssetA = GetSourceAsset(a.Guid);
            SourceAsset sourceAssetB = GetSourceAsset(b.Guid);

            if (sourceAssetA != null && sourceAssetB != null)
            {
                return sourceAssetA.Name.CompareTo(sourceAssetB.Name);
            }

            if (sourceAssetA == null && sourceAssetB == null)
            {
                return a.Guid.CompareTo(b.Guid);
            }

            if (sourceAssetA == null)
            {
                return -1;
            }

            if (sourceAssetB == null)
            {
                return 1;
            }

            return 0;
        }

        private int AssetGuidComparer(AssetInfo a, AssetInfo b)
        {
            SourceAsset sourceAssetA = GetSourceAsset(a.Guid);
            SourceAsset sourceAssetB = GetSourceAsset(b.Guid);

            if (sourceAssetA != null && sourceAssetB != null || sourceAssetA == null && sourceAssetB == null)
            {
                return a.Guid.CompareTo(b.Guid);
            }

            if (sourceAssetA == null)
            {
                return -1;
            }

            if (sourceAssetB == null)
            {
                return 1;
            }

            return 0;
        }
    }
}
