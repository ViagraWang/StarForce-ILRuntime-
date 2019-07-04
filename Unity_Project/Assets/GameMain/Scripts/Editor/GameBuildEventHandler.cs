using Game.Runtime;
using GameFramework;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityGameFrame.Editor.AssetBundleTools;
using UnityGameFrame.Runtime;

namespace Game.Editor
{	
	//创建发包时事件回调
	public class GameBuildEventHandler : IBuildEventHandler
	{
	    private const string GameUpdateUrl = null; //TODO:游戏资源热更新的url，后期填写资源所在服务器的网址
	
	    //当前平台打包失败，不再继续打包其他平台
	    public bool ContinueOnFailure { get { return false; } }
	
	    /// <summary>
	    /// 所有平台生成结束后的后处理事件。
	    /// </summary>
	    /// <param name="productName">产品名称。</param>
	    /// <param name="companyName">公司名称。</param>
	    /// <param name="gameIdentifier">游戏识别号。</param>
	    /// <param name="applicableGameVersion">适用游戏版本。</param>
	    /// <param name="internalResourceVersion">内部资源版本。</param>
	    /// <param name="unityVersion">Unity 版本。</param>
	    /// <param name="buildOptions">生成选项。</param>
	    /// <param name="zip">是否压缩。</param>
	    /// <param name="outputDirectory">生成目录。</param>
	    /// <param name="workingPath">生成时的工作路径。</param>
	    /// <param name="outputPackageSelected">是否生成单机模式所需的文件。</param>
	    /// <param name="outputPackagePath">为单机模式生成的文件存放于此路径。若游戏是单机游戏，生成结束后将此目录中对应平台的文件拷贝至 StreamingAssets 后打包 App 即可。</param>
	    /// <param name="outputFullSelected">是否生成可更新模式所需的远程文件。</param>
	    /// <param name="outputFullPath">为可更新模式生成的远程文件存放于此路径。若游戏是网络游戏，生成结束后应将此目录上传至 Web 服务器，供玩家下载用。</param>
	    /// <param name="outputPackedSelected">是否生成可更新模式所需的本地文件。</param>
	    /// <param name="outputPackedPath">为可更新模式生成的本地文件存放于此路径。若游戏是网络游戏，生成结束后将此目录中对应平台的文件拷贝至 StreamingAssets 后打包 App 即可。</param>
	    /// <param name="buildReportPath">生成报告路径。</param>
	    public void PostprocessAllPlatforms(string productName, string companyName, string gameIdentifier, 
	        string applicableGameVersion, int internalResourceVersion, string unityVersion, 
	        BuildAssetBundleOptions buildOptions, bool zip, string outputDirectory, string workingPath,
	        bool outputPackageSelected, string outputPackagePath, bool outputFullSelected, string outputFullPath,
	         bool outputPackedSelected, string outputPackedPath, string buildReportPath)
	    {
	        Debug.LogFormat("已完成创建所有平台的资源包。");
	
	    }
	
	    /// <summary>
	    /// 某个平台生成结束后的后处理事件。
	    /// </summary>
	    /// <param name="platform">生成平台。</param>
	    /// <param name="workingPath">生成时的工作路径。</param>
	    /// <param name="outputPackageSelected">是否生成单机模式所需的文件。</param>
	    /// <param name="outputPackagePath">为单机模式生成的文件存放于此路径。若游戏是单机游戏，生成结束后将此目录中对应平台的文件拷贝至 StreamingAssets 后打包 App 即可。</param>
	    /// <param name="outputFullSelected">是否生成可更新模式所需的远程文件。</param>
	    /// <param name="outputFullPath">为可更新模式生成的远程文件存放于此路径。若游戏是网络游戏，生成结束后应将此目录上传至 Web 服务器，供玩家下载用。</param>
	    /// <param name="outputPackedSelected">是否生成可更新模式所需的本地文件。</param>
	    /// <param name="outputPackedPath">为可更新模式生成的本地文件存放于此路径。若游戏是网络游戏，生成结束后将此目录中对应平台的文件拷贝至 StreamingAssets 后打包 App 即可。</param>
	    /// <param name="isSuccess">是否生成成功。</param>
	    public void PostprocessPlatform(Platform platform, string workingPath, bool outputPackageSelected, string outputPackagePath, bool outputFullSelected, string outputFullPath, bool outputPackedSelected, string outputPackedPath, bool isSuccess)
	    {
	        Debug.LogFormat("{0}平台的资源包创建完成。", platform.ToString());
	
	        if (!outputPackageSelected)
	            return;
	        
	        if (IsCurrentBuildPlatform(platform))
	            return;
	
	        string streamingAssetsPath = Utility.Path.GetCombinePath(Application.dataPath, "StreamingAssets");
	        string[] fileNames = Directory.GetFiles(outputPackagePath, "*", SearchOption.AllDirectories);
	        foreach (string fileName in fileNames)
	        {
	            string destFileName = Utility.Path.GetCombinePath(streamingAssetsPath, fileName.Substring(outputPackagePath.Length));
	            FileInfo destFileInfo = new FileInfo(destFileName);
	            if (!destFileInfo.Directory.Exists)
	            {
	                destFileInfo.Directory.Create();
	            }
	
	            File.Copy(fileName, destFileName);
	        }
	    }
	
	    /// <summary>
	    /// 所有平台生成开始前的预处理事件。
	    /// </summary>
	    /// <param name="productName">产品名称。</param>
	    /// <param name="companyName">公司名称。</param>
	    /// <param name="gameIdentifier">游戏识别号。</param>
	    /// <param name="applicableGameVersion">适用游戏版本。</param>
	    /// <param name="internalResourceVersion">内部资源版本。</param>
	    /// <param name="unityVersion">Unity 版本。</param>
	    /// <param name="buildOptions">生成选项。</param>
	    /// <param name="zip">是否压缩。</param>
	    /// <param name="outputDirectory">生成目录。</param>
	    /// <param name="workingPath">生成时的工作路径。</param>
	    /// <param name="outputPackageSelected">是否生成单机模式所需的文件。</param>
	    /// <param name="outputPackagePath">为单机模式生成的文件存放于此路径。若游戏是单机游戏，生成结束后将此目录中对应平台的文件拷贝至 StreamingAssets 后打包 App 即可。</param>
	    /// <param name="outputFullSelected">是否生成可更新模式所需的远程文件。</param>
	    /// <param name="outputFullPath">为可更新模式生成的远程文件存放于此路径。若游戏是网络游戏，生成结束后应将此目录上传至 Web 服务器，供玩家下载用。</param>
	    /// <param name="outputPackedSelected">是否生成可更新模式所需的本地文件。</param>
	    /// <param name="outputPackedPath">为可更新模式生成的本地文件存放于此路径。若游戏是网络游戏，生成结束后将此目录中对应平台的文件拷贝至 StreamingAssets 后打包 App 即可。</param>
	    /// <param name="buildReportPath">生成报告路径。</param>
	    public void PreprocessAllPlatforms(string productName, string companyName, string gameIdentifier, string applicableGameVersion, int internalResourceVersion, string unityVersion, BuildAssetBundleOptions buildOptions, bool zip, string outputDirectory, string workingPath, bool outputPackageSelected, string outputPackagePath, bool outputFullSelected, string outputFullPath, bool outputPackedSelected, string outputPackedPath, string buildReportPath)
	    {
	        Debug.LogFormat("准备创建所有平台的资源包。");
	        string streamingAssetsPath = Utility.Path.GetCombinePath(Application.dataPath, "StreamingAssets");
	        if (!Directory.Exists(streamingAssetsPath))
	            Directory.CreateDirectory(streamingAssetsPath);
	        string[] fileNames = Directory.GetFiles(streamingAssetsPath, "*", SearchOption.AllDirectories);
	        for (int i = 0; i < fileNames.Length; i++)
	        {
	            if (fileNames[i].Contains(".gitkeep"))
	                continue;
	
	            File.Delete(fileNames[i]);  //删除文件
	        }
	
	        //移除空文件夹
	        if (Utility.Path.RemoveEmptyDirectory(streamingAssetsPath))
	        {
	            Debug.Log("移除StreamingAssets下所有文件");
	        }
	    }
	
	    /// <summary>
	    /// 某个平台生成开始前的预处理事件。
	    /// </summary>
	    /// <param name="platform">生成平台。</param>
	    /// <param name="workingPath">生成时的工作路径。</param>
	    /// <param name="outputPackageSelected">是否生成单机模式所需的文件。</param>
	    /// <param name="outputPackagePath">为单机模式生成的文件存放于此路径。若游戏是单机游戏，生成结束后将此目录中对应平台的文件拷贝至 StreamingAssets 后打包 App 即可。</param>
	    /// <param name="outputFullSelected">是否生成可更新模式所需的远程文件。</param>
	    /// <param name="outputFullPath">为可更新模式生成的远程文件存放于此路径。若游戏是网络游戏，生成结束后应将此目录上传至 Web 服务器，供玩家下载用。</param>
	    /// <param name="outputPackedSelected">是否生成可更新模式所需的本地文件。</param>
	    /// <param name="outputPackedPath">为可更新模式生成的本地文件存放于此路径。若游戏是网络游戏，生成结束后将此目录中对应平台的文件拷贝至 StreamingAssets 后打包 App 即可。</param>
	    public void PreprocessPlatform(Platform platform, string workingPath, bool outputPackageSelected, string outputPackagePath, bool outputFullSelected, string outputFullPath, bool outputPackedSelected, string outputPackedPath)
	    {
	        Debug.LogFormat("开始创建{0}平台的资源包。", platform.ToString());
	    }
	
	    /// <summary>
	    /// 处理版本列表信息
	    /// </summary>
	    /// <param name="path">资源包的生成路径</param>
	    /// <param name="latestGameVersion">最新的版本号</param>
	    /// <param name="internalResourceVersion">内部资源的版本号</param>
	    /// <param name="versionListLength">版本列表大小</param>
	    /// <param name="versionListHashCode">版本列表哈希码</param>
	    /// <param name="versionListZipLength">版本列表压缩大小</param>
	    /// <param name="versionListZipHashCode">版本列表压缩哈希码</param>
	    public void OnHandleVersionList(string path, string latestGameVersion, int internalResourceVersion, int versionListLength, int versionListHashCode, int versionListZipLength, int versionListZipHashCode)
	    {
	        Version.SetVersionHelper(new DefaultVersionHelper());   //设置默认版本辅助器
	        DirectoryInfo info = new DirectoryInfo(Path.GetDirectoryName(path));
	        string gameUpdateUrl = string.IsNullOrEmpty(GameUpdateUrl) ? info.Parent.Parent.FullName.Replace('\\', '/') : GameUpdateUrl;  //游戏资源热更新的网址
	        VersionInfo versionInfo = new VersionInfo(false, latestGameVersion, Version.InternalGameVersion, internalResourceVersion, gameUpdateUrl,
	            versionListLength, versionListHashCode, versionListZipLength, versionListZipHashCode, null);
	        Utility.Json.SetJsonHelper(new DefaultJsonHelper());    //设置默认的Json辅助器
	
	        string json = Utility.Json.ToJson(versionInfo);
	
	        string filePath = Path.Combine(path, VersionInfo.VersionListName);
            using(FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
	        {
            using (StreamWriter sw = new StreamWriter(fs))
	            {
	                sw.Write(json);
	            }
	        }
	
	        AssetDatabase.Refresh();    //刷新编辑器
	        AssetDatabase.SaveAssets();
	        Debug.Log("成功创建版本列表文件=>" + filePath);
	    }
	
	    //检查是否是当前目标平台
	    private bool IsCurrentBuildPlatform(Platform platform)
	    {
	        switch (platform)
	        {
	            case Platform.Undefined:
	                break;
	            case Platform.Windows:
	                if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneWindows)
	                    return true;
	                break;
	            case Platform.Windows64:
	                if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneWindows64)
	                    return true;
	                break;
	            case Platform.MacOS:
	                if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneOSX)
	                    return true;
	                break;
	            case Platform.Linux:
	                if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneLinux)
	                    return true;
	                break;
	            case Platform.Linux64:
	                if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneLinux64)
	                    return true;
	                break;
	            case Platform.LinuxUniversal:
	                if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneLinuxUniversal)
	                    return true;
	                break;
	            case Platform.IOS:
	                if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS)
	                    return true;
	                break;
	            case Platform.Android:
	                if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
	                    return true;
	                break;
	            case Platform.WindowsStore:
	                if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneWindows)
	                    return true;
	                break;
	            case Platform.WebGL:
	                if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.WebGL)
	                    return true;
	                break;
	            default:
	                break;
	        }
	
	        return false;
	    }
	}
}
