using GameFramework;
using UnityGameFrame.Runtime;

namespace Game.Runtime
{	
	//本地化扩展工具
	public static class LocalizationExtension
	{
	    //加载本地化配置
		public static void LoadDictionary(this LocalizationComponent localizationComponent, string dictionaryName, LoadType loadType, object userData = null)
	    {
	        if (string.IsNullOrEmpty(dictionaryName))
	        {
	            Log.Warning("Dictionary name is invalid.");
	            return;
	        }
	
	        localizationComponent.LoadDictionary(dictionaryName, RuntimeAssetUtility.GetDictionaryAsset(dictionaryName, loadType), loadType, RuntimeConstant.AssetPriority.DictionaryAsset, userData);
	    }
	}
}
