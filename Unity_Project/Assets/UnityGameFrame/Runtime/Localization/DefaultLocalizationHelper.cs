using GameFramework;
using GameFramework.Localization;
using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace UnityGameFrame.Runtime
{
    /// <summary>
    /// 默认本地化辅助器
    /// </summary>
    public class DefaultLocalizationHelper : LocalizationHelperBase
    {
        private static readonly string[] RowSplitSeparator = new string[] { "\r\n", "\r", "\n" };
        private readonly string[] ColumnSplitSeparator = new string[] { "\t" };   //分离字符
        //private const int ColumnCount = 4;  //列数

        private ResourceComponent m_ResourceComponent = null;
        private ILocalizationManager m_LocalizationManager = null;

        /// <summary>
        /// 获取系统语言
        /// </summary>
        public override Language SystemLanguage
        {
            get
            {
                switch (Application.systemLanguage)
                {
                    case UnityEngine.SystemLanguage.Afrikaans: return Language.Afrikaans;
                    case UnityEngine.SystemLanguage.Arabic: return Language.Arabic;
                    case UnityEngine.SystemLanguage.Basque: return Language.Basque;
                    case UnityEngine.SystemLanguage.Belarusian: return Language.Belarusian;
                    case UnityEngine.SystemLanguage.Bulgarian: return Language.Bulgarian;
                    case UnityEngine.SystemLanguage.Catalan: return Language.Catalan;
                    case UnityEngine.SystemLanguage.Chinese: return Language.ChineseSimplified;
                    case UnityEngine.SystemLanguage.ChineseSimplified: return Language.ChineseSimplified;
                    case UnityEngine.SystemLanguage.ChineseTraditional: return Language.ChineseTraditional;
                    case UnityEngine.SystemLanguage.Czech: return Language.Czech;
                    case UnityEngine.SystemLanguage.Danish: return Language.Danish;
                    case UnityEngine.SystemLanguage.Dutch: return Language.Dutch;
                    case UnityEngine.SystemLanguage.English: return Language.English;
                    case UnityEngine.SystemLanguage.Estonian: return Language.Estonian;
                    case UnityEngine.SystemLanguage.Faroese: return Language.Faroese;
                    case UnityEngine.SystemLanguage.Finnish: return Language.Finnish;
                    case UnityEngine.SystemLanguage.French: return Language.French;
                    case UnityEngine.SystemLanguage.German: return Language.German;
                    case UnityEngine.SystemLanguage.Greek: return Language.Greek;
                    case UnityEngine.SystemLanguage.Hebrew: return Language.Hebrew;
                    case UnityEngine.SystemLanguage.Hungarian: return Language.Hungarian;
                    case UnityEngine.SystemLanguage.Icelandic: return Language.Icelandic;
                    case UnityEngine.SystemLanguage.Indonesian: return Language.Indonesian;
                    case UnityEngine.SystemLanguage.Italian: return Language.Italian;
                    case UnityEngine.SystemLanguage.Japanese: return Language.Japanese;
                    case UnityEngine.SystemLanguage.Korean: return Language.Korean;
                    case UnityEngine.SystemLanguage.Latvian: return Language.Latvian;
                    case UnityEngine.SystemLanguage.Lithuanian: return Language.Lithuanian;
                    case UnityEngine.SystemLanguage.Norwegian: return Language.Norwegian;
                    case UnityEngine.SystemLanguage.Polish: return Language.Polish;
                    case UnityEngine.SystemLanguage.Portuguese: return Language.PortuguesePortugal;
                    case UnityEngine.SystemLanguage.Romanian: return Language.Romanian;
                    case UnityEngine.SystemLanguage.Russian: return Language.Russian;
                    case UnityEngine.SystemLanguage.SerboCroatian: return Language.SerboCroatian;
                    case UnityEngine.SystemLanguage.Slovak: return Language.Slovak;
                    case UnityEngine.SystemLanguage.Slovenian: return Language.Slovenian;
                    case UnityEngine.SystemLanguage.Spanish: return Language.Spanish;
                    case UnityEngine.SystemLanguage.Swedish: return Language.Swedish;
                    case UnityEngine.SystemLanguage.Thai: return Language.Thai;
                    case UnityEngine.SystemLanguage.Turkish: return Language.Turkish;
                    case UnityEngine.SystemLanguage.Ukrainian: return Language.Ukrainian;
                    case UnityEngine.SystemLanguage.Unknown: return Language.Unspecified;
                    case UnityEngine.SystemLanguage.Vietnamese: return Language.Vietnamese;
                    default: return Language.Unspecified;
                }
            }
        }

        private void Start()
        {
            m_ResourceComponent = GameEntry.GetComponent<ResourceComponent>();
            if (m_ResourceComponent == null)
            {
                Log.Fatal("[DefaultLocalizationHelper.Start] Resource component is invalid.");
                return;
            }
            m_LocalizationManager = GameFrameworkEntry.GetModule<ILocalizationManager>();
            if (m_LocalizationManager == null)
            {
                Log.Fatal("[DefaultLocalizationHelper.Start] Localization manager is invalid.");
                return;
            }
        }

        /// <summary>
        /// 解析字典
        /// </summary>
        /// <param name="text">要解析的字典文本</param>
        /// <param name="userData">用户自定义数据</param>
        /// <returns>是否解析字典成功</returns>
        public override bool ParseDictionary(string text, object userData)
        {
            try
            {
                string currentLanguage = m_LocalizationManager.Language.ToString();    //获取本地语言类型
                int valueColumnIndex = 1;   //所取的语言类型所在列
                string[] rowTexts = text.Split(RowSplitSeparator, StringSplitOptions.None);    //分离行
                for (int i = 0; i < rowTexts.Length; i++)
                {
                    string[] splitLine = rowTexts[i].Split(ColumnSplitSeparator, StringSplitOptions.None);
                    //if (splitLine.Length != ColumnCount)
                    //{
                    //    Log.Warning("[DefaultLocalizationHelper.ParseDictionary] Can not parse dictionary '{0}'.", text);
                    //    return false;
                    //}

                    if (rowTexts[i].Length <= 0 || splitLine[0].Trim('"') == "#")
                    {
                        if(i == 0)  //选择语言下标
                        {
                            for (int j = 0; j < splitLine.Length; j++)
                            {
                                if (!string.IsNullOrEmpty(splitLine[j].Trim('\"')) && splitLine[j].Trim('\"').Equals(currentLanguage))
                                {
                                    valueColumnIndex = j;   //指定选取语言所在的列
                                    break;
                                }
                            }
                        }
                        continue;
                    }

                    string dictionaryName = splitLine[1].Trim('\"');
                    string dictionaryValue = splitLine[valueColumnIndex].Trim('\"');
                    if(!AddString(dictionaryName, dictionaryValue))
                    {
                        Log.Warning("[DefaultLocalizationHelper.ParseDictionary] Can not add raw string with key '{0}' which may be invalid or duplicate.", dictionaryName);
                        return false;
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                Log.Warning("[DefaultLocalizationHelper.ParseDictionary] Can not parse dictionary '{0}' with exception '{1}'.", text, e.ToString());
                return false;
            }
        }

        /// <summary>
        /// 解析字典
        /// </summary>
        /// <param name="bytes">要解析的字典二进制流</param>
        /// <param name="userData">用户自定义数据</param>
        /// <returns>是否解析字典成功</returns>
        public override bool ParseDictionary(byte[] bytes, object userData)
        {
            using(MemoryStream stream = new MemoryStream(bytes, false))
            {
                return ParseDictionary(stream, userData);
            }
        }

        /// <summary>
        /// 解析字典
        /// </summary>
        /// <param name="stream">要解析的字典二进制流</param>
        /// <param name="userData">用户自定义数据</param>
        /// <returns>是否解析字典成功</returns>
        public override bool ParseDictionary(Stream stream, object userData)
        {
            try
            {
                using(BinaryReader br = new BinaryReader(stream, Encoding.UTF8))
                {
                    while(br.BaseStream.Position < br.BaseStream.Length)
                    {
                        string dictionaryName = br.ReadString();
                        string dictionaryValue = br.ReadString();
                        if(!AddString(dictionaryName, dictionaryValue))
                        {
                            Log.Warning("[DefaultLocalizationHelper.ParseDictionary] Can not add raw string with key '{0}' which may be invalid or duplicate.", dictionaryName);
                            return false;
                        }
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                Log.Warning("[DefaultLocalizationHelper.ParseDictionary] Can not parse dictionary with exception '{0}'.", e.ToString());
                return false;
            }
        }

        /// <summary>
        /// 释放字典资源
        /// </summary>
        /// <param name="dictionaryAsset">要释放的字典资源</param>
        public override void ReleaseDictionaryAsset(object dictionaryAsset)
        {
            m_ResourceComponent.UnloadAsset(dictionaryAsset);
        }

        /// <summary>
        /// 加载字典
        /// </summary>
        /// <param name="dictionaryName">字典名称</param>
        /// <param name="dictionaryAsset">字典资源</param>
        /// <param name="loadType">字典加载方式。</param>
        /// <param name="userData">用户自定义数据</param>
        /// <returns>加载是否成功</returns>
        protected override bool LoadDictionary(string dictionaryName, object dictionaryAsset, LoadType loadType, object userData)
        {
            TextAsset textAsset = dictionaryAsset as TextAsset;
            if (textAsset == null)
            {
                Log.Warning("[DefaultLocalizationHelper.LoadDictionary] Dictionary asset '{0}' is invalid.", dictionaryName);
                return false;
            }

            bool retVal = false;
            switch (loadType)
            {
                case LoadType.Text:
                    retVal = m_LocalizationManager.ParseDictionary(textAsset.text, userData);
                    break;
                case LoadType.Bytes:
                    retVal = m_LocalizationManager.ParseDictionary(textAsset.bytes, userData);
                    break;
                case LoadType.Stream:
                    using(MemoryStream stream = new MemoryStream(textAsset.bytes, false))
                    {
                        retVal = m_LocalizationManager.ParseDictionary(stream, userData);
                    }
                    break;
                default:
                    Log.Warning("[DefaultLocalizationHelper.LoadDictionary] Unknown load type.");
                    break;
            }

            if (!retVal)
            {
                Log.Warning("[DefaultLocalizationHelper.LoadDictionary] Dictionary asset '{0}' parse failure.", dictionaryName);
            }

            return retVal;
        }

        /// <summary>
        /// 增加字典
        /// </summary>
        /// <param name="key">字典主键</param>
        /// <param name="value">字典内容</param>
        /// <returns>是否增加字典成功</returns>
        protected bool AddString(string key, string value)
        {
            return m_LocalizationManager.AddRawString(key, value);
        }
    }
}
