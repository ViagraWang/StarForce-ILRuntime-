using GameFramework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using UnityEditor;
using UnityEngine;

namespace UnityGameFrame.Editor.AssetBundleTools
{
    /// <summary>
    /// 资源包收集器。
    /// </summary>
    public sealed class AssetBundleCollection
    {
        private const string PostfixOfScene = ".unity"; //场景的扩展名
        private static readonly Regex AssetBundleNameRegex = new Regex(@"^([A-Za-z0-9\._-]+/)*[A-Za-z0-9\._-]+$");  //bundle名不能包含的特殊字符
        private static readonly Regex AssetBundleVariantRegex = new Regex(@"^[a-z0-9_-]+$");    //变体名不能包含的特殊字符


        private readonly string m_ConfigurationPath;    //配置路径
        private readonly SortedDictionary<string, AssetBundleInfo> m_AssetBundles;  //AssetBundle资源包信息集合
        private readonly SortedDictionary<string, AssetInfo> m_Assets;  //资源信息集合

        /// <summary>
        /// 获取所有保存的Bundle信息
        /// </summary>
        public AssetBundleInfo[] AllAssetBundleInfos { get { return m_AssetBundles.Values.ToArray(); } }

        /// <summary>
        /// 获取所有保存的资源信息
        /// </summary>
        public AssetInfo[] AllAssetInfos { get { return m_Assets.Values.ToArray(); } }

        /// <summary>
        /// Bundle信息的数量
        /// </summary>
        public int AssetBundleCount { get { return m_AssetBundles.Count; } }

        /// <summary>
        /// 资源信息的数量
        /// </summary>
        public int AssetCount { get { return m_Assets.Count; } }

        public event GameFrameworkAction<int, int> EventOnLoadingAssetBundle = null;    //加载Bundle信息中的回调
        public event GameFrameworkAction<int, int> EventOnLoadingAsset = null;  //加载资源信息中的回调
        public event GameFrameworkAction EventOnLoadCompleted = null;   //加载完成的回调

        public AssetBundleCollection()
        {
            m_ConfigurationPath = Type.GetConfigurationPath<AssetBundleCollectionConfigPathAttribute>() ??
                Utility.Path.GetCombinePath(Application.dataPath, "GameFramework/Configs/AssetBundleCollection.xml");

            m_AssetBundles = new SortedDictionary<string, AssetBundleInfo>();
            m_Assets = new SortedDictionary<string, AssetInfo>();
        }

        //清空
        public void Clear()
        {
            m_AssetBundles.Clear();
            m_Assets.Clear();
        }

        //加载Bundle配置信息
        public bool Load()
        {
            Clear();

            if (!File.Exists(m_ConfigurationPath))
                return false;

            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(m_ConfigurationPath);
                //获取各个节点
                XmlNode xmlRoot = xmlDocument.SelectSingleNode("UnityGameFramework");
                XmlNode xmlCollection = xmlRoot.SelectSingleNode("AssetBundleCollection");
                XmlNode xmlAssetBundles = xmlCollection.SelectSingleNode("AssetBundles");
                XmlNode xmlAssets = xmlCollection.SelectSingleNode("Assets");

                XmlNodeList xmlNodeList = null;
                XmlNode xmlNode = null;
                XmlNode xmlTemp = null;
                int count = 0;

                //遍历 AssetBundle信息
                xmlNodeList = xmlAssetBundles.ChildNodes;
                count = xmlNodeList.Count;
                for (int i = 0; i < count; i++)
                {
                    if (EventOnLoadingAssetBundle != null)
                        EventOnLoadingAssetBundle.Invoke(i, count);

                    //遍历每一个节点
                    xmlNode = xmlNodeList.Item(i);
                    if (xmlNode.Name != "AssetBundle")
                        continue;

                    //bundle名称
                    string assetBundleName = xmlNode.Attributes.GetNamedItem("Name").Value;
                    //变体名称
                    xmlTemp = xmlNode.Attributes.GetNamedItem("Variant");   
                    string assetBundleVariant = xmlTemp != null ? xmlTemp.Value : null;
                    //加载类型
                    int assetBundleLoadType = 0;
                    xmlTemp = xmlNode.Attributes.GetNamedItem("LoadType");
                    if (xmlTemp != null)
                        int.TryParse(xmlTemp.Value, out assetBundleLoadType);
                    //是否打包
                    bool assetBundlePacked = false;
                    xmlTemp = xmlNode.Attributes.GetNamedItem("Packed");
                    if (xmlTemp != null)
                        bool.TryParse(xmlTemp.Value, out assetBundlePacked);
                    //资源组
                    xmlTemp = xmlNode.Attributes.GetNamedItem("ResourceGroups");
                    string[] assetBundleResourceGroups = xmlTemp != null ? xmlTemp.Value.Split(',') : new string[0];
                    //添加
                    if (!AddAssetBundleInfo(assetBundleName, assetBundleVariant, (AssetBundleLoadType)assetBundleLoadType, assetBundlePacked, assetBundleResourceGroups))
                    {
                        string assetBundleFullName = assetBundleVariant != null ? Utility.Text.Format("{0}.{1}", assetBundleName, assetBundleVariant) : assetBundleName;
                        Debug.LogWarning(Utility.Text.Format("Can not add AssetBundle '{0}'.", assetBundleFullName));
                        continue;
                    }
                }

                //遍历Assets信息
                xmlNodeList = xmlAssets.ChildNodes;
                count = xmlNodeList.Count;
                for (int i = 0; i < count; i++)
                {
                    if (EventOnLoadingAsset != null)
                        EventOnLoadingAsset.Invoke(i, count);

                    xmlNode = xmlNodeList.Item(i);
                    if (xmlNode.Name != "Asset")
                        continue;

                    string assetGuid = xmlNode.Attributes.GetNamedItem("Guid").Value;   //资源id
                    string assetBundleName = xmlNode.Attributes.GetNamedItem("AssetBundleName").Value;  //资源Bundle名
                    xmlTemp = xmlNode.Attributes.GetNamedItem("AssetBundleVariant");
                    string assetBundleVariant = xmlTemp != null ? xmlTemp.Value : null;
                    if(!AssignAssetInfo(assetGuid, assetBundleName, assetBundleVariant))
                    {
                        string assetBundleFullName = assetBundleVariant != null ? Utility.Text.Format("{0}.{1}", assetBundleName, assetBundleVariant) : assetBundleName;
                        Debug.LogWarning(Utility.Text.Format("Can not assign asset '{0}' to AssetBundle '{1}'.", assetGuid, assetBundleFullName));
                        continue;
                    }
                }

                if (EventOnLoadCompleted != null)
                    EventOnLoadCompleted.Invoke();

                return true;
            }
            catch
            {
                File.Delete(m_ConfigurationPath);
                if (EventOnLoadCompleted != null)
                    EventOnLoadCompleted.Invoke();

                return false;
            }
        }

        //保存配置信息
        public bool Save()
        {
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.AppendChild(xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null));

                XmlElement xmlRoot = xmlDocument.CreateElement("UnityGameFramework");
                xmlDocument.AppendChild(xmlRoot);

                XmlElement xmlCollection = xmlDocument.CreateElement("AssetBundleCollection");
                xmlRoot.AppendChild(xmlCollection);
                //Bundle信息节点
                XmlElement xmlAssetBundles = xmlDocument.CreateElement("AssetBundles");
                xmlCollection.AppendChild(xmlAssetBundles);
                //资源信息节点
                XmlElement xmlAssets = xmlDocument.CreateElement("Assets");
                xmlCollection.AppendChild(xmlAssets);

                XmlElement xmlElement = null;
                XmlAttribute xmlAttribute = null;

                foreach (AssetBundleInfo assetBundleInfo in m_AssetBundles.Values)
                {
                    xmlElement = xmlDocument.CreateElement("AssetBundle");
                    //名称
                    xmlAttribute = xmlDocument.CreateAttribute("Name");
                    xmlAttribute.Value = assetBundleInfo.Name;
                    xmlElement.Attributes.SetNamedItem(xmlAttribute);
                    //变体
                    if (assetBundleInfo.Variant != null)
                    {
                        xmlAttribute = xmlDocument.CreateAttribute("Variant");
                        xmlAttribute.Value = assetBundleInfo.Variant;
                        xmlElement.Attributes.SetNamedItem(xmlAttribute);
                    }
                    //加载方式
                    xmlAttribute = xmlDocument.CreateAttribute("LoadType");
                    xmlAttribute.Value = ((int)assetBundleInfo.LoadType).ToString();
                    xmlElement.Attributes.SetNamedItem(xmlAttribute);
                    //是否封包
                    xmlAttribute = xmlDocument.CreateAttribute("Packed");
                    xmlAttribute.Value = assetBundleInfo.Packed.ToString();
                    xmlElement.Attributes.SetNamedItem(xmlAttribute);
                    //资源组
                    string[] resourceGroups = assetBundleInfo.GetResourceGroups();
                    if(resourceGroups.Length > 0)
                    {
                        xmlAttribute = xmlDocument.CreateAttribute("ResourceGroups");
                        xmlAttribute.Value = string.Join(",", resourceGroups);  //连接资源组名称
                        xmlElement.Attributes.SetNamedItem(xmlAttribute);
                    }

                    xmlAssetBundles.AppendChild(xmlElement);
                }

                foreach (AssetInfo asset in m_Assets.Values)
                {
                    xmlElement = xmlDocument.CreateElement("Asset");
                    //资源编号
                    xmlAttribute = xmlDocument.CreateAttribute("Guid");
                    xmlAttribute.Value = asset.Guid;
                    xmlElement.Attributes.SetNamedItem(xmlAttribute);
                    //资源的Bundle名
                    xmlAttribute = xmlDocument.CreateAttribute("AssetBundleName");
                    xmlAttribute.Value = asset.AssetBundleInfo.Name;
                    xmlElement.Attributes.SetNamedItem(xmlAttribute);
                    //资源变体名
                    if (asset.AssetBundleInfo.Variant != null)
                    {
                        xmlAttribute = xmlDocument.CreateAttribute("AssetBundleVariant");
                        xmlAttribute.Value = asset.AssetBundleInfo.Variant;
                        xmlElement.Attributes.SetNamedItem(xmlAttribute);
                    }

                    xmlAssets.AppendChild(xmlElement);
                }

                string configurationDirectoryName = Path.GetDirectoryName(m_ConfigurationPath); //获取目录名
                if (!Directory.Exists(configurationDirectoryName))
                    Directory.CreateDirectory(configurationDirectoryName);

                xmlDocument.Save(m_ConfigurationPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                return true;
            }
            catch
            {
                if (File.Exists(m_ConfigurationPath))
                    File.Delete(m_ConfigurationPath);

                return false;
            }
        }

        //获取Bundle信息
        public AssetBundleInfo GetAssetBundleInfo(string assetBundleName, string assetBundleVariant)
        {
            if (!IsValidAssetBundleName(assetBundleName, assetBundleVariant))   //检查是否合法
                return null;

            AssetBundleInfo info = null;
            if (m_AssetBundles.TryGetValue(GetAssetBundleFullName(assetBundleName, assetBundleVariant).ToLower(), out info))
                return info;

            return null;
        }

        public bool HasAssetBundleInfo(string assetBundleName, string assetBundleVariant)
        {
            if (!IsValidAssetBundleName(assetBundleName, assetBundleVariant))
                return false;

            return m_AssetBundles.ContainsKey(GetAssetBundleFullName(assetBundleName, assetBundleVariant).ToLower());
        }

        //添加Bundle
        public bool AddAssetBundleInfo(string assetBundleName, string assetBundleVariant, AssetBundleLoadType assetBundleLoadType, bool assetBundlePacked, string[] resourceGroups)
        {
            if (!IsValidAssetBundleName(assetBundleName, assetBundleVariant))   //检查是否合法
                return false;

            if (!IsAvailableBundleName(assetBundleName, assetBundleVariant, null))
                return false;

            AssetBundleInfo assetBundle = AssetBundleInfo.Create(assetBundleName, assetBundleVariant, assetBundleLoadType, assetBundlePacked, resourceGroups);
            m_AssetBundles.Add(assetBundle.FullName.ToLower(), assetBundle);
            return true;
        }

        //重命名Bundle信息
        public bool RenameAssetBundleInfo(string oldAssetBundleName, string oldAssetBundleVariant, string newAssetBundleName, string newAssetBundleVariant)
        {
            //先检查名称是否合法
            if (!IsValidAssetBundleName(oldAssetBundleName, oldAssetBundleVariant) || !IsValidAssetBundleName(newAssetBundleName, newAssetBundleVariant))
                return false;

            AssetBundleInfo assetBundle = GetAssetBundleInfo(oldAssetBundleName, oldAssetBundleVariant);
            if (assetBundle == null)
                return false;

            if (!IsAvailableBundleName(newAssetBundleName, newAssetBundleVariant, assetBundle))
                return false;

            m_AssetBundles.Remove(assetBundle.FullName.ToLower());  //先移除旧的
            assetBundle.Rename(newAssetBundleName, newAssetBundleVariant);  //重命名
            m_AssetBundles.Add(assetBundle.FullName.ToLower(), assetBundle);    //重新添加key

            return true;
        }

        //移除Bundle信息
        public bool RemoveAssetBundle(string assetBundleName, string assetBundleVariant)
        {
            if (!IsValidAssetBundleName(assetBundleName, assetBundleVariant))
                return false;

            AssetBundleInfo assetBundle = GetAssetBundleInfo(assetBundleName, assetBundleVariant);  //获取Bundle信息
            if (assetBundle == null)
                return false;

            AssetInfo[] assets = assetBundle.AssetInfos;    //BundleInfo关联的资源信息
            assetBundle.Clear();    //清空BundleInfo中的数据
            m_AssetBundles.Remove(assetBundle.FullName.ToLower());  //从列表中移除
            foreach (AssetInfo asset in assets)
            {
                m_Assets.Remove(asset.Guid);    //收集器中移除这些资源信息
            }

            return true;
        }

        //设置Bundle资源的加载模式
        public bool SetAssetBundleLoadType(string assetBundleName, string assetBundleVariant, AssetBundleLoadType assetBundleLoadType)
        {
            if (!IsValidAssetBundleName(assetBundleName, assetBundleVariant))
                return false;

            AssetBundleInfo assetBundle = GetAssetBundleInfo(assetBundleName, assetBundleVariant);  //获取Bundle信息
            if (assetBundle == null)
                return false;

            assetBundle.SetLoadType(assetBundleLoadType);   //修改加载模式

            return true;
        }

        //设置Bundle是否多资源封包
        public bool SetAssetBundlePacked(string assetBundleName, string assetBundleVariant, bool assetBundlePacked)
        {
            if (!IsValidAssetBundleName(assetBundleName, assetBundleVariant))
                return false;

            AssetBundleInfo assetBundle = GetAssetBundleInfo(assetBundleName, assetBundleVariant);
            if (assetBundle == null)
            {
                return false;
            }

            assetBundle.SetPacked(assetBundlePacked);

            return true;
        }

        //检查Bundle名和变体名是否合法
        private bool IsValidAssetBundleName(string assetBundleName, string assetBundleVariant)
        {
            if (string.IsNullOrEmpty(assetBundleName))
                return false;

            if (!AssetBundleNameRegex.IsMatch(assetBundleName))
                return false;

            if (assetBundleVariant != null && !AssetBundleVariantRegex.IsMatch(assetBundleVariant))
                return false;

            return true;
        }

        //检查Bundle是否一致
        private bool IsAvailableBundleName(string assetBundleName, string assetBundleVariant, AssetBundleInfo selfAssetBundle)
        {
            AssetBundleInfo findAssetBundle = GetAssetBundleInfo(assetBundleName, assetBundleVariant);
            if (findAssetBundle != null)
                return findAssetBundle == selfAssetBundle;

            foreach (AssetBundleInfo assetBundle in m_AssetBundles.Values)
            {
                if (selfAssetBundle != null && assetBundle == selfAssetBundle)  //排除自身，因为自身的名称肯定等于自身的
                    continue;

                if (assetBundle.Name == assetBundleName)    //Bundle名一致
                {
                    if (assetBundle.Variant == null && assetBundleVariant != null)
                        return false;

                    if (assetBundle.Variant != null && assetBundleVariant == null)
                        return false;
                }

                if (assetBundle.Name.Length > assetBundleName.Length 
                    && assetBundle.Name.IndexOf(assetBundleName, StringComparison.CurrentCultureIgnoreCase) == 0
                    && assetBundle.Name[assetBundleName.Length] == '/')
                    return false;

                if (assetBundleName.Length > assetBundle.Name.Length
                    && assetBundleName.IndexOf(assetBundle.Name, StringComparison.CurrentCultureIgnoreCase) == 0
                    && assetBundleName[assetBundle.Name.Length] == '/')
                    return false;

            }

            return true;
        }

        //获取指定bundle名和变体名的资源信息
        public AssetInfo[] GetAssetInfos(string assetBundleName, string assetBundleVariant)
        {
            if (!IsValidAssetBundleName(assetBundleName, assetBundleVariant))
                return new AssetInfo[0];

            AssetBundleInfo assetBundle = GetAssetBundleInfo(assetBundleName, assetBundleVariant);
            if (assetBundle == null)
                return new AssetInfo[0];

            return assetBundle.AssetInfos;
        }

        //获取资源信息
        public AssetInfo GetAssetInfo(string assetGuid)
        {
            if (string.IsNullOrEmpty(assetGuid))
                return null;

            AssetInfo asset = null;
            if (m_Assets.TryGetValue(assetGuid, out asset))
                return asset;

            return null;
        }

        //是否包含资源信息
        public bool HasAssetInfo(string assetGuid)
        {
            if (string.IsNullOrEmpty(assetGuid))
            {
                return false;
            }

            return m_Assets.ContainsKey(assetGuid);
        }

        //分配资源信息
        public bool AssignAssetInfo(string assetGuid, string assetBundleName, string assetBundleVariant)
        {
            if (string.IsNullOrEmpty(assetGuid))
                return false;

            if (!IsValidAssetBundleName(assetBundleName, assetBundleVariant))
                return false;

            AssetBundleInfo assetBundle = GetAssetBundleInfo(assetBundleName, assetBundleVariant);  //获取Bundle信息
            if (assetBundle == null)
                return false;

            string assetName = AssetDatabase.GUIDToAssetPath(assetGuid);    //根据资源的guid获取路径
            if (string.IsNullOrEmpty(assetName))
                return false;

            AssetInfo[] assetsInAssetBundle = assetBundle.AssetInfos;   //获取bundle中的所有资源信息
            for (int i = 0; i < assetsInAssetBundle.Length; i++)
            {
                AssetInfo info = assetsInAssetBundle[i];
                if (info.Name == assetName) //已存在
                    continue;

                if (info.Name.ToLower() == assetName.ToLower()) //bundle名可能一致
                    return false;
            }

            bool isScene = assetName.EndsWith(PostfixOfScene);
            if (isScene && assetBundle.Type == AssetBundleType.Asset || !isScene && assetBundle.Type == AssetBundleType.Scene)
                return false;   //场景资源不能喝其他资源打包到一起

            AssetInfo asset = GetAssetInfo(assetGuid);
            if (asset == null)
            {
                asset = AssetInfo.Create(assetGuid);
                m_Assets.Add(asset.Guid, asset);
            }

            assetBundle.AssignAssetInfo(asset, isScene);    //分配到Bundle中

            return true;
        }

        //移除资源信息
        public bool UnassignAssetInfo(string assetGuid)
        {
            if (string.IsNullOrEmpty(assetGuid))
            {
                return false;
            }

            AssetInfo asset = GetAssetInfo(assetGuid);
            if (asset != null)
            {
                asset.AssetBundleInfo.Unassign(asset);
                m_Assets.Remove(asset.Guid);
            }

            return true;
        }

        //获取Bundle全名
        private string GetAssetBundleFullName(string assetBundleName, string assetBundleVariant)
        {
            return !string.IsNullOrEmpty(assetBundleVariant) ? Utility.Text.Format("{0}.{1}", assetBundleName, assetBundleVariant) : assetBundleName;
        }

    }
}
