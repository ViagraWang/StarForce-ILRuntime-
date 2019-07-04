using System;
using UnityEngine;

namespace Game.Runtime {	
	/// <summary>
	/// 构建的发包信息
	/// </summary>
	[Serializable]
	public class BuildInfo
	{
	    [SerializeField]
	    private string Game_Version;
	    [SerializeField]
	    private int Internal_Game_Version;
	    [SerializeField]
	    private string Check_Version_Url;   //检查版本地址
	    [SerializeField]
	    private string Standalone_App_Url;  //独立应用地址
	    [SerializeField]
	    private string Ios_App_Url; //IOS应用地址
	    [SerializeField]
	    private string Android_App_Url; //Android应用地址
	    [SerializeField]
	    private string END_OF_JSON; //结束语
	
	    /// <summary>
	    /// 游戏版本
	    /// </summary>
	    public string GameVersion { get { return Game_Version; } }
	
	    /// <summary>
	    /// 内置版本
	    /// </summary>
	    public int InternalGameVersion { get { return Internal_Game_Version; } }
	
	    /// <summary>
	    /// 检查版本号的Url
	    /// </summary>
	    public string CheckVersionUrl { get { return Check_Version_Url; } }
	
	    /// <summary>
	    /// 独立应用的Url
	    /// </summary>
	    public string StandaloneAppUrl { get { return Standalone_App_Url; } }
	
	    /// <summary>
	    /// IOS应用的Url
	    /// </summary>
	    public string IosAppUrl { get { return Ios_App_Url; } }
	
	    /// <summary>
	    /// Android应用的Url
	    /// </summary>
	    public string AndroidAppUrl { get { return Android_App_Url; } }
	
	    /// <summary>
	    /// 结束语
	    /// </summary>
	    public string ENDOFJSON { get { return END_OF_JSON; } }
	
	    public BuildInfo(string gameVersion, int internalGameVersion, string checkVersionUrl, string standaloneAppUrl, string iosAppUrl, string androidAppUrl, string endOfJson)
	    {
	        Game_Version = gameVersion;
	        Internal_Game_Version = internalGameVersion;
	        Check_Version_Url = checkVersionUrl;
	        Standalone_App_Url = standaloneAppUrl;
	        Ios_App_Url = iosAppUrl;
	        Android_App_Url = androidAppUrl;
	        END_OF_JSON = endOfJson;
	    }
	
	}
}
