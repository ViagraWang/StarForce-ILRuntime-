using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Runtime {	
	/// <summary>
	/// 版本信息
	/// </summary>
	[Serializable]
	public class VersionInfo
	{
	    public const string VersionListName = "versionList.dat";   //版本列表文件名称
	
	    private const string DefaultEndOfJson = ""; //默认结束标志
	
	    [SerializeField]
	    private bool Force_GameUpdate = false;  //强制更新游戏包的标志位
	    [SerializeField]
	    private string Latest_GameVersion;  //最新的游戏版本号
	    [SerializeField]
	    private int Internal_GameVersion;   //内部游戏版本号
	    [SerializeField]
	    private int Internal_ResourceVersion;   //内部资源版本号
	    [SerializeField]
	    private string Game_UpdateUrl;  //游戏更新地址
	    [SerializeField]
	    private int VersionList_Length;   //版本列表大小
	    [SerializeField]
	    private int VersionList_HashCode;   //版本列表哈希码
	    [SerializeField]
	    private int VersionList_ZipLength;   //版本列表压缩大小
	    [SerializeField]
	    private int VersionList_ZipHashCode;   //版本列表压缩哈希码
	    [SerializeField]
	    private string END_OF_JSON;  //结束部分
	
	    /// <summary>
	    /// 强制游戏更新的标志位
	    /// </summary>
	    public bool ForceGameUpdate { get { return Force_GameUpdate; } }
	
	    /// <summary>
	    /// 最新的游戏版本号
	    /// </summary>
	    public string LatestGameVersion { get { return Latest_GameVersion; } }
	
	    /// <summary>
	    /// 内部游戏版本号
	    /// </summary>
	    public int InternalGameVersion { get { return Internal_GameVersion; } }
	
	    /// <summary>
	    /// 内部资源版本号
	    /// </summary>
	    public int InternalResourceVersion { get { return Internal_ResourceVersion; } }
	
	    /// <summary>
	    /// 游戏更新的Url
	    /// </summary>
	    public string GameUpdateUrl { get { return Game_UpdateUrl; } }
	
	    /// <summary>
	    /// 版本列表大小
	    /// </summary>
	    public int VersionListLength { get { return VersionList_Length; } }
	
	    /// <summary>
	    /// 版本列表哈希码
	    /// </summary>
	    public int VersionListHashCode { get { return VersionList_HashCode; } }
	
	    /// <summary>
	    /// 版本列表压缩大小
	    /// </summary>
	    public int VersionListZipLength { get { return VersionList_ZipLength; } }
	
	    /// <summary>
	    /// 版本列表压缩哈希码
	    /// </summary>
	    public int VersionListZipHashCode { get { return VersionList_ZipHashCode; } }
	
	    /// <summary>
	    /// 结束部分
	    /// </summary>
	    public string ENDOFJSON { get { return END_OF_JSON; } }
	
	    public VersionInfo(bool Force_GameUpdate, string Latest_GameVersion, int Internal_GameVersion, int Internal_ResourceVersion, string Game_UpdateUrl,
	        int VersionList_Length, int VersionList_HashCode, int VersionList_ZipLength, int VersionList_ZipHashCode, string END_OF_JSON)
	    {
	        this.Force_GameUpdate = Force_GameUpdate;
	        this.Latest_GameVersion = Latest_GameVersion;
	        this.Internal_GameVersion = Internal_GameVersion;
	        this.Internal_ResourceVersion = Internal_ResourceVersion;
	        this.Game_UpdateUrl = Game_UpdateUrl;
	        this.VersionList_Length = VersionList_Length;
	        this.VersionList_HashCode = VersionList_HashCode;
	        this.VersionList_ZipLength = VersionList_ZipLength;
	        this.VersionList_ZipHashCode = VersionList_ZipHashCode;
	        this.END_OF_JSON = string.IsNullOrEmpty(END_OF_JSON) ? DefaultEndOfJson : END_OF_JSON;
	    }
	
	}
}
