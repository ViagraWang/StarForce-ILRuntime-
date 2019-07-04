using GameFramework;
using UnityGameFrame.Runtime;

namespace Game.Runtime
{	
	public static class ConfigExtension
	{
	    //加载配置
	    public static void LoadConfig(this ConfigComponent configComponent, string configName, LoadType loadType, object userData = null)
	    {
	        if (string.IsNullOrEmpty(configName))
	        {
	            Log.Warning("Config name is invalid.");
	            return;
	        }
	
	        //加载配置
	        configComponent.LoadConfig(configName, RuntimeAssetUtility.GetConfigAsset(configName, loadType), loadType, RuntimeConstant.AssetPriority.ConfigAsset, userData);
	    }
	
	}
}
