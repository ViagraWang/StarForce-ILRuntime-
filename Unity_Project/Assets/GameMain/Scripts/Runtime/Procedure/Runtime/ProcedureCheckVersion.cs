using GameFramework;
using GameFramework.Resource;
using UnityEngine;
using UnityGameFrame.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace Game.Runtime {	
	//检查版本的流程
	public class ProcedureCheckVersion : ProcedureBase
	{
	    private bool m_LatestVersionComplete = false;   //最新版本检查完成的标志位
	    private VersionInfo m_VersionInfo = null;   //版本信息
	    private UpdateVersionListCallbacks m_UpdateVersionListCallbacks = null; //更新版本列表的回调
	
	    public override bool UseNativeDialog { get { return true; } }
	
	    protected override void OnInit(ProcedureOwner procedureOwner)
	    {
	        base.OnInit(procedureOwner);
	
	        m_UpdateVersionListCallbacks = new UpdateVersionListCallbacks(OnUpdateVersionListSuccess, OnUpdateVersionListFailure);
	    }
	
	    protected override void OnEnter(ProcedureOwner procedureOwner)
	    {
	        base.OnEnter(procedureOwner);
	
	        m_LatestVersionComplete = false;
	        m_VersionInfo = null;
	
	        GameEntry.Event.Subscribe(WebRequestSuccessEventArgs.EventId, OnWebRequestSuccess);
	        GameEntry.Event.Subscribe(WebRequestFailureEventArgs.EventId, OnWebRequestFailure);
	
	        RequestVersion();   //请求版本列表
	    }
	
	    protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
	    {
	        GameEntry.Event.Unsubscribe(WebRequestSuccessEventArgs.EventId, OnWebRequestSuccess);
	        GameEntry.Event.Unsubscribe(WebRequestFailureEventArgs.EventId, OnWebRequestFailure);
	
	        base.OnLeave(procedureOwner, isShutdown);
	    }
	
	    protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
	    {
	        base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
	
	        if (!m_LatestVersionComplete)
	            return;
	
	        ChangeState<ProcedureUpdateResource>(procedureOwner);   //切换到更新资源的流程
	    }
	
	    //前往下载更新app
	    private void GotoUpdateApp(object userData)
	    {
	        string url = null;
#if UNITY_EDITOR
	        url = GameEntry.BuiltinData.BuildInfo.StandaloneAppUrl;
#elif UNITY_IOS
	            url = GameEntry.BuiltinData.BuildInfo.IosAppUrl;
#elif UNITY_ANDROID
	            url = GameEntry.BuiltinData.BuildInfo.AndroidAppUrl;
#else
	            url = GameEntry.BuiltinData.BuildInfo.StandaloneAppUrl;
#endif
	        Application.OpenURL(url);
	    }
	
	    //请求版本信息
	    private void RequestVersion()
	    {
	        string deviceId = SystemInfo.deviceUniqueIdentifier;
	        string deviceName = SystemInfo.deviceName;
	        string deviceModel = SystemInfo.deviceModel;
	        string processorType = SystemInfo.processorType;
	        string processorCount = SystemInfo.processorCount.ToString();
	        string memorySize = SystemInfo.systemMemorySize.ToString();
	        string operatingSystem = SystemInfo.operatingSystem;
	        string iOSGeneration = string.Empty;
	        string iOSSystemVersion = string.Empty;
	        string iOSVendorIdentifier = string.Empty;
#if UNITY_IOS && !UNITY_EDITOR
	            iOSGeneration = UnityEngine.iOS.Device.generation.ToString();
	            iOSSystemVersion = UnityEngine.iOS.Device.systemVersion;
	            iOSVendorIdentifier = UnityEngine.iOS.Device.vendorIdentifier ?? string.Empty;
#endif
	        string gameVersion = Version.GameVersion;
	        string platform = Application.platform.ToString();
	        string language = GameEntry.Localization.Language.ToString();
	        string unityVersion = Application.unityVersion;
	        string installMode = Application.installMode.ToString();
	        string sandboxType = Application.sandboxType.ToString();
	        string screenWidth = Screen.width.ToString();
	        string screenHeight = Screen.height.ToString();
	        string screenDpi = Screen.dpi.ToString();
	        string screenOrientation = Screen.orientation.ToString();
	        string screenResolution = Utility.Text.Format("{0} x {1} @ {2}Hz", Screen.currentResolution.width.ToString(), Screen.currentResolution.height.ToString(), Screen.currentResolution.refreshRate.ToString());
	        string useWifi = (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork).ToString();
	
	        WWWForm wwwForm = new WWWForm();
	        wwwForm.AddField("DeviceId", WebUtility.EscapeString(deviceId));
	        wwwForm.AddField("DeviceName", WebUtility.EscapeString(deviceName));
	        wwwForm.AddField("DeviceModel", WebUtility.EscapeString(deviceModel));
	        wwwForm.AddField("ProcessorType", WebUtility.EscapeString(processorType));
	        wwwForm.AddField("ProcessorCount", WebUtility.EscapeString(processorCount));
	        wwwForm.AddField("MemorySize", WebUtility.EscapeString(memorySize));
	        wwwForm.AddField("OperatingSystem", WebUtility.EscapeString(operatingSystem));
	        wwwForm.AddField("IOSGeneration", WebUtility.EscapeString(iOSGeneration));
	        wwwForm.AddField("IOSSystemVersion", WebUtility.EscapeString(iOSSystemVersion));
	        wwwForm.AddField("IOSVendorIdentifier", WebUtility.EscapeString(iOSVendorIdentifier));
	        wwwForm.AddField("GameVersion", WebUtility.EscapeString(gameVersion));
	        wwwForm.AddField("Platform", WebUtility.EscapeString(platform));
	        wwwForm.AddField("Language", WebUtility.EscapeString(language));
	        wwwForm.AddField("UnityVersion", WebUtility.EscapeString(unityVersion));
	        wwwForm.AddField("InstallMode", WebUtility.EscapeString(installMode));
	        wwwForm.AddField("SandboxType", WebUtility.EscapeString(sandboxType));
	        wwwForm.AddField("ScreenWidth", WebUtility.EscapeString(screenWidth));
	        wwwForm.AddField("ScreenHeight", WebUtility.EscapeString(screenHeight));
	        wwwForm.AddField("ScreenDPI", WebUtility.EscapeString(screenDpi));
	        wwwForm.AddField("ScreenOrientation", WebUtility.EscapeString(screenOrientation));
	        wwwForm.AddField("ScreenResolution", WebUtility.EscapeString(screenResolution));
	        wwwForm.AddField("UseWifi", WebUtility.EscapeString(useWifi));
	
	        //TODO:后期可能会需要提交数据，检查版本列表
	        //GameEntry.WebRequest.AddWebRequest(GameEntry.BuiltinData.BuildInfo.CheckVersionUrl, wwwForm, this);
	        //这里本地检查版本列表，不需要提交数据
	        string versionListPath = Utility.Path.GetCombinePath(GameEntry.BuiltinData.BuildInfo.CheckVersionUrl, GetPlatformPath(), VersionInfo.VersionListName);
	        GameEntry.WebRequest.AddWebRequest(versionListPath, this);
	    }
	
	    //更新版本信息
	    private void UpdateVersion()
	    {
	        if (GameEntry.Resource.CheckVersionList(m_VersionInfo.InternalResourceVersion) == CheckVersionListResult.Updated)   //已是最新
	        {
	            m_LatestVersionComplete = true;
	        }
	        else
	        {
	            //需要更新
	            GameEntry.Resource.UpdateVersionList(m_VersionInfo.VersionListLength, m_VersionInfo.VersionListHashCode, m_VersionInfo.VersionListZipLength, m_VersionInfo.VersionListZipHashCode, m_UpdateVersionListCallbacks);
	        }
	    }
	
	    //请求版本信息成功的回调
	    private void OnWebRequestSuccess(object sender, BaseEventArgs e)
	    {
	        WebRequestSuccessEventArgs ne = (WebRequestSuccessEventArgs)e;
	        if (ne.UserData != this)
	            return;
	
	        m_VersionInfo = Utility.Json.ToObject<VersionInfo>(ne.WebResponseBytes);
	        if (m_VersionInfo == null)
	        {
	            Log.Error("Parse VersionInfo failure.");
	            return;
	        }
	
	        Log.Info("Latest game version is '{0}', local game version is '{1}'.", m_VersionInfo.LatestGameVersion, Version.GameVersion);
	
	        if (m_VersionInfo.ForceGameUpdate)
	        {
	            GameEntry.UI.OpenDialog(new DialogParams
	            {
	                Mode = 2,
	                Title = GameEntry.Localization.GetString("ForceUpdate.Title"),
	                Message = GameEntry.Localization.GetString("ForceUpdate.Message"),
	                ConfirmText = GameEntry.Localization.GetString("ForceUpdate.UpdateButton"),
	                OnClickConfirm = GotoUpdateApp,
	                CancelText = GameEntry.Localization.GetString("ForceUpdate.QuitButton"),
	                OnClickCancel = delegate (object userData) { UnityGameFrame.Runtime.GameEntry.Shutdown(ShutdownType.Quit); },
	            });
	
	            return;
	        }
	
	        //TODO:设置更新url，后期改为平台+版本号的uri
	        GameEntry.Resource.UpdatePrefixUri = Utility.Path.GetCombinePath(m_VersionInfo.GameUpdateUrl, GetResourceVersionName(), GetPlatformPath());
	
	        UpdateVersion();    //更新版本列表
	    }
	
	    private void OnWebRequestFailure(object sender, BaseEventArgs e)
	    {
	        WebRequestFailureEventArgs ne = (WebRequestFailureEventArgs)e;
	        if (ne.UserData != this)
	        {
	            return;
	        }
	
	        Log.Warning("Check version failure, error message '{0}'.", ne.ErrorMessage);
	    }
	
	    //更新版本列表成功
	    private void OnUpdateVersionListSuccess(string downloadPath, string downloadUri)
	    {
	        m_LatestVersionComplete = true;
	        Log.Info("Update latest version list from '{0}' success.", downloadUri);
	    }
	
	    //更新版本列表失败
	    private void OnUpdateVersionListFailure(string downloadUri, string errorMessage)
	    {
	        Log.Warning("Update latest version list from '{0}' failure, error message '{1}'.", downloadUri, errorMessage);
	    }
	
	    //获取资源版本名称
	    private string GetResourceVersionName()
	    {
	        string[] splitApplicableGameVersion = Version.GameVersion.Split('.');
	        string format = "";
	        for (int i = 0; i < splitApplicableGameVersion.Length; i++)
	        {
	            format = Utility.Text.Format(string.IsNullOrEmpty(format) ? "{0}{1}" : "{0}_{1}", format, splitApplicableGameVersion[i]);
	        }
	
	        return string.Format("{0}_{1}", format, m_VersionInfo.InternalResourceVersion);
	    }
	
	    //获取平台路径
	    private string GetPlatformPath()
	    {
	        switch (Application.platform)
	        {
	            case RuntimePlatform.WindowsEditor:
	            case RuntimePlatform.WindowsPlayer:
	                return "Windows64";
	            case RuntimePlatform.OSXEditor:
	            case RuntimePlatform.OSXPlayer:
	            case RuntimePlatform.IPhonePlayer:
	                return "IOS";
	            case RuntimePlatform.Android:
	                return "Android";
	            case RuntimePlatform.WSAPlayerX86:
	            case RuntimePlatform.WSAPlayerX64:
	            case RuntimePlatform.WSAPlayerARM:
	                return "WindowsStore";
	            default:
	                return "Windows64";
	        }
	    }
	}
}
