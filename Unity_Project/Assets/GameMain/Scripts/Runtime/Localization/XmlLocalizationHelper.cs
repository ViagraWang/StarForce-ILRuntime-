using System;
using System.Xml;
using UnityGameFrame.Runtime;

namespace Game.Runtime
{	
	/// <summary>
	/// XML 格式的本地化辅助器
	/// </summary>
	public class XmlLocalizationHelper : DefaultLocalizationHelper
	{
	
	    public override bool ParseDictionary(string text, object userData)
	    {
	        try
	        {
	            string currentLanguage = GameEntry.Localization.Language.ToString();    //获取本地语言类型
	            XmlDocument xmlDocument = new XmlDocument();
	            xmlDocument.LoadXml(text);  //直接从文本中转换xml
	            XmlNode xmlRoot = xmlDocument.SelectSingleNode("Dictionaries"); //根节点
	            XmlNodeList xmlNodeDictionaryList = xmlRoot.ChildNodes;
	            for (int i = 0; i < xmlNodeDictionaryList.Count; i++)
	            {
	                XmlNode xmlNodeDictionary = xmlNodeDictionaryList.Item(i);
	                if (xmlNodeDictionary.Name != "Dictionary") //一级节点
	                    continue;
	
	                string language = xmlNodeDictionary.Attributes.GetNamedItem("Language").Value;  //获取语言类型
	                if (language != currentLanguage)
	                    continue;   //不相等则继续查找
	
	                XmlNodeList xmlNodeStringList = xmlNodeDictionary.ChildNodes;   //所有子节点列表
	                for (int j = 0; j < xmlNodeStringList.Count; j++)
	                {
	                    XmlNode xmlNodeString = xmlNodeStringList.Item(j);
	                    if (xmlNodeString.Name != "String") //子节点的名全为String
	                        continue;
	
	                    string key = xmlNodeString.Attributes.GetNamedItem("Key").Value;
	                    string value = xmlNodeString.Attributes.GetNamedItem("Value").Value;
	                    if (!AddString(key, value))  //添加一行本地化
	                    {
	                        Log.Warning("Can not add raw string with key '{0}' which may be invalid or duplicate.", key);
	                        return false;
	                    }
	                }
	            }
	
	            return true;
	        }
	        catch (Exception e)
	        {
	            Log.Warning("Can not parse dictionary '{0}' with exception '{1}'.", text, e.ToString());
	            return false;
	        }
	    }
	}
}
