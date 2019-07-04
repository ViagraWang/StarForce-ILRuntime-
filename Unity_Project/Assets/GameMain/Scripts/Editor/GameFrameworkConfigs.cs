using GameFramework;
using UnityEngine;
using UnityGameFrame.Editor;
using UnityGameFrame.Editor.AssetBundleTools;

namespace Game.Editor
{	
	//游戏配置
	public static class GameFrameworkConfigs
	{
	    public static string s_ConfigFolderPath = "GameMain/Configs";  //配置文件夹路径
	
	    [BuildSettingsConfigPath]
	    public static string BuildSettingsConfig = Utility.Path.GetCombinePath(Application.dataPath, s_ConfigFolderPath, "BuildSettings.xml");
	
	    [AssetBundleBuilderConfigPath]
	    public static string AssetBundleBuilderConfig = Utility.Path.GetCombinePath(Application.dataPath, s_ConfigFolderPath, "AssetBundleBuilder.xml");
	
	    [AssetBundleEditorConfigPath]
	    public static string AssetBundleEditorConfig = Utility.Path.GetCombinePath(Application.dataPath, s_ConfigFolderPath, "AssetBundleEditor.xml");
	
	    [AssetBundleCollectionConfigPath]
	    public static string AssetBundleCollectionConfig = Utility.Path.GetCombinePath(Application.dataPath, s_ConfigFolderPath, "AssetBundleCollection.xml");
	}
}
