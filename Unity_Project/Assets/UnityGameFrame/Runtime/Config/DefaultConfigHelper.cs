using GameFramework;
using GameFramework.Config;
using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace UnityGameFrame.Runtime
{
    /// <summary>
    /// 默认配置辅助器
    /// </summary>
    public sealed class DefaultConfigHelper : ConfigHelperBase
    {
        private static readonly string[] RowSplitSeparator = new string[] { "\r\n", "\r", "\n" };
        private static readonly string[] ColumnSplitSeparator = new string[] { "\t" };   //分离每一列的标识符
        private const int ColumnCount = 4;  //列数

        private ResourceComponent m_ResourceComponent = null;   //资源组件
        private IConfigManager m_ConfigManager = null;  //配置管理器

        private void Start()
        {
            //获取资源组件
            m_ResourceComponent = GameEntry.GetComponent<ResourceComponent>();
            if (m_ResourceComponent == null)
            {
                Log.Fatal("[DefaultConfigHelper.Start] Resource component is invalid.");
                return;
            }

            //获取配置管理器模块
            m_ConfigManager = GameFrameworkEntry.GetModule<IConfigManager>();
            if (m_ConfigManager == null)
            {
                Log.Fatal("[DefaultConfigHelper.Start] Config manager is invalid.");
                return;
            }
        }

        /// <summary>
        /// 解析配置
        /// </summary>
        /// <param name="text">要解析的配置文本</param>
        /// <param name="userData">用户自定义数据</param>
        /// <returns>是否解析配置成功</returns>
        public override bool ParseConfig(string text, object userData)
        {
            try
            {
                string[] rowTexts = text.Split(RowSplitSeparator, StringSplitOptions.None);    //文本按行分割
                for (int i = 0; i < rowTexts.Length; i++)
                {
                    if (rowTexts[i].Length <= 0 || rowTexts[i][0] == '#')   //第一个字符是#表示注释行
                        continue;

                    string[] splitLine = rowTexts[i].Split(ColumnSplitSeparator, StringSplitOptions.None);   //跳格分割
                    if(splitLine.Length != ColumnCount)
                    {
                        Log.Warning("[DefaultConfigHelper.ParseConfig] Can not parse config '{0}'.", text);
                        return false;
                    }

                    string configName = splitLine[1].Trim('\"');   //配置名
                    string configValue = splitLine[3].Trim('\"');  //字符串值，第二行是备注，不需要读取

                    if (!AddConfig(configName, configValue))
                    {
                        Log.Warning("[DefaultConfigHelper.ParseConfig] Can not add raw string with config name '{0}' which may be invalid or duplicate.", configName);
                        return false;
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                Log.Warning("[DefaultConfigHelper.ParseConfig] Can not parse config '{0}' with exception '{1}'.", text, e.ToString());
                return false;
            }
        }

        /// <summary>
        /// 解析配置
        /// </summary>
        /// <param name="bytes">要解析的配置二进制流</param>
        /// <param name="userData">用户自定义数据</param>
        /// <returns>是否解析配置成功</returns>
        public override bool ParseConfig(byte[] bytes, object userData)
        {
            using(MemoryStream ms = new MemoryStream(bytes, false))
            {
                return ParseConfig(ms, userData);
            }
        }

        /// <summary>
        /// 解析配置
        /// </summary>
        /// <param name="stream">要解析的配置二进制流</param>
        /// <param name="userData">用户自定义数据</param>
        /// <returns>是否解析配置成功</returns>
        public override bool ParseConfig(Stream stream, object userData)
        {
            try
            {
                using(BinaryReader br = new BinaryReader(stream, Encoding.UTF8))
                {
                    while(br.BaseStream.Position < br.BaseStream.Length)
                    {
                        string configName = br.ReadString();
                        string configValue = br.ReadString();
                        if (!AddConfig(configName, configValue))
                        {
                            Log.Warning("[DefaultConfigHelper.ParseConfig] Can not add raw string with config name '{0}' which may be invalid or duplicate.", configName);
                            return false;
                        }
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                Log.Warning("Can not parse config with exception '{0}'.", e.ToString());
                return false;
            }
        }

        /// <summary>
        /// 释放配置资源
        /// </summary>
        /// <param name="configAsset">要释放的配置资源</param>
        public override void ReleaseConfigAsset(object configAsset)
        {
            m_ResourceComponent.UnloadAsset(configAsset);
        }

        /// <summary>
        /// 加载配置
        /// </summary>
        /// <param name="configName">配置名称</param>
        /// <param name="configAsset">配置资源</param>
        /// <param name="loadType">配置加载方式</param>
        /// <param name="userData">用户自定义数据</param>
        /// <returns>加载是否成功</returns>
        protected override bool LoadConfig(string configName, object configAsset, LoadType loadType, object userData)
        {
            TextAsset textAsset = configAsset as TextAsset;
            if (textAsset == null)
            {
                Log.Warning("[DefaultConfigHelper.ParseConfig] Config asset '{0}' is invalid.", configName);
                return false;
            }

            bool res = false;
            switch (loadType)
            {
                case LoadType.Text:
                    res = m_ConfigManager.ParseConfig(textAsset.text, userData);
                    break;
                case LoadType.Bytes:
                    res = m_ConfigManager.ParseConfig(textAsset.bytes, userData);
                    break;
                case LoadType.Stream:
                    using(MemoryStream ms = new MemoryStream(textAsset.bytes, false))
                    {
                        res = m_ConfigManager.ParseConfig(ms, userData);
                    }
                    break;
                default:
                    Log.Warning("[DefaultConfigHelper.ParseConfig] Unknown load type.");
                    break;
            }
            
            if(!res)
                Log.Warning("[DefaultConfigHelper.ParseConfig] Config asset '{0}' parse failure.", configName);

            return res;
        }

        /// <summary>
        /// 增加指定配置项
        /// </summary>
        /// <param name="configName">要增加配置项的名称</param>
        /// <param name="configValue">要增加配置项的值</param>
        /// <returns>是否增加配置项成功</returns>
        private bool AddConfig(string configName, string configValue)
        {
            bool boolValue;
            bool.TryParse(configValue, out boolValue);

            int intValue;
            int.TryParse(configValue, out intValue);

            float floatValue;
            float.TryParse(configValue, out floatValue);

            return m_ConfigManager.AddConfig(configName, boolValue, intValue, floatValue, configValue);
        }
    }
}
