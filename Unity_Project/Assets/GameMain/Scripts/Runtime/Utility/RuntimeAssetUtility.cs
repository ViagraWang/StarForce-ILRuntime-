using GameFramework;

namespace Game.Runtime
{
	//资源扩展工具
	public static class RuntimeAssetUtility
	{
        //路径
	    public const string ConfigPath = "Assets/GameMain/Configs"; //配置表路径
	    public const string DataTablePath = "Assets/GameMain/DataTables";   //数据表路径
	    public const string LocalizationPath = "Assets/GameMain/Localization";  //本地化路径
	    public const string FontPath = "Assets/GameMain/Fonts";  //字体路径
	    public const string ScenePath = "Assets/GameMain/Scenes";  //场景路径
	    public const string MusicPath = "Assets/GameMain/Music";  //音乐路径
	    public const string SoundPath = "Assets/GameMain/Sounds";  //音效路径
	    public const string EntityPath = "Assets/GameMain/Entities";  //实体路径
	    public const string UIFormPath = "Assets/GameMain/UI/UIForms";  //UI预设路径
	    public const string UISoundPath = "Assets/GameMain/UI/UISounds";  //UI音效路径
	    public const string HotfixPath = "Assets/GameMain/HotfixDll";  //热更新路径

        //热更新程序文件
        public const string HotfixDllName = "Game.Hotfix.dll.bytes";
        public const string HotfixPdbName = "Game.Hotfix.pdb.bytes";

        //扩展名称
        public const string CsvFolder = "Csv";    //csv文件夹
	    public const string BytesFolder = "Bytes";    //二进制文件夹
	    public const string csvExtension = ".csv";   //csv文件扩展名
	    public const string bytesExtension = ".bytes";   //二进制文件扩展名
	
	    //根据加载类型，获取配置表和数据表文件的后缀
	    public static string GetDataTableAndConfigLoadTypeSuffix(this LoadType loadType)
	    {
	        return loadType == LoadType.Text ? csvExtension : bytesExtension;
	    }
	
	    //根据加载类型，获取本地化语言包文件的后缀
	    public static string GetDictionaryLoadTypeSuffix(this LoadType loadType)
	    {
	        return loadType == LoadType.Text ? csvExtension : bytesExtension;
	    }
	
	    //获取不同文件所在的文件夹
	    public static string GetFolderName(this LoadType loadType)
	    {
	        return loadType == LoadType.Text ? CsvFolder : BytesFolder;
	    }
	
	    //获取配置资源内置路径
	    public static string GetConfigAsset(string assetName, LoadType loadType)
	    {
	        return Utility.Text.Format("{0}/{1}{2}", ConfigPath, assetName, loadType.GetDataTableAndConfigLoadTypeSuffix());
	    }
	
	    //获取数据表资源内置路径
	    public static string GetDataTableAsset(string assetName, LoadType loadType)
	    {
	        return Utility.Text.Format("{0}/{1}/{2}{3}", DataTablePath, loadType.GetFolderName(), assetName, loadType.GetDataTableAndConfigLoadTypeSuffix());
	    }
	
	    //获取本地化语言资源内置路径
	    public static string GetDictionaryAsset(string assetName, LoadType loadType)
	    {
	        return Utility.Text.Format("{0}/{1}/{2}{3}", LocalizationPath, loadType.GetFolderName(), assetName, loadType.GetDictionaryLoadTypeSuffix());
	    }
	
	    //获取本地化字体资源内置路径
	    public static string GetFontAsset(string assetName)
	    {
	        return Utility.Text.Format("{0}/{1}.ttf", FontPath, assetName);
	    }
	
	    //获取场景资源内置路径
	    public static string GetSceneAsset(string assetName)
	    {
	        return Utility.Text.Format("{0}/{1}.unity", ScenePath, assetName);
	    }
	
	    //获取音乐资源内置路径
	    public static string GetMusicAsset(string assetName)
	    {
	        return Utility.Text.Format("{0}/{1}.mp3", MusicPath, assetName);
	    }
	
	    //获取音效资源内置路径
	    public static string GetSoundAsset(string assetName)
	    {
	        return Utility.Text.Format("{0}/{1}.wav", SoundPath, assetName);
	    }
	
	    //获取实体资源内置路径
	    public static string GetEntityAsset(string assetName)
	    {
	        return Utility.Text.Format("{0}/{1}.prefab", EntityPath, assetName);
	    }
	
	    //获取UI界面资源内置路径
	    public static string GetUIFormAsset(string assetName)
	    {
	        return Utility.Text.Format("{0}/{1}.prefab", UIFormPath, assetName);
	    }
	
	    //获取UI声音资源内置路径
	    public static string GetUISoundAsset(string assetName)
	    {
	        return Utility.Text.Format("{0}/{1}.wav", UISoundPath, assetName);
	    }
	
        //获取热更新资源内置路径
        public static string GetHotfixAsset(string assetName)
        {
            return Utility.Text.Format("{0}/{1}", HotfixPath, assetName);
        }

    }
}
