using GameFramework;
using GameFramework.Fsm;
using GameFramework.Procedure;
using UnityEngine;
using UnityGameFrame.Runtime;

namespace Game.Runtime {	
	//检查版本并初始化资源的流程
	public class ProcedureInitResources : ProcedureBase
	{
	    private bool m_InitResourcesComplete = false;   //资源是否初始化完成的标志位
	
	    public override bool UseNativeDialog { get { return true; } }
	
	    protected override void OnInit(IFsm<IProcedureManager> procedureOwner)
	    {
	        base.OnInit(procedureOwner);
	    }
	
	    protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
	    {
	        base.OnEnter(procedureOwner);
	        //订阅事件
	        GameEntry.Event.Subscribe(WebRequestSuccessEventArgs.EventId, OnWebRequestSuccess);
	        GameEntry.Event.Subscribe(WebRequestFailureEventArgs.EventId, OnWebRequestFailure);
	
	        m_InitResourcesComplete = false;
	        //RequestVersion();   //请求版本资源
	        //初始化资源
	        GameEntry.Resource.InitResources(OnInitResourcesComplete);
	    }
	
	    protected override void OnLeave(IFsm<IProcedureManager> procedureOwner, bool isShutdown)
	    {
	        base.OnLeave(procedureOwner, isShutdown);
	        //取消订阅
	        GameEntry.Event.Unsubscribe(WebRequestSuccessEventArgs.EventId, OnWebRequestSuccess);
	        GameEntry.Event.Unsubscribe(WebRequestFailureEventArgs.EventId, OnWebRequestFailure);
	    }
	
	    protected override void OnUpdate(IFsm<IProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
	    {
	        base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
	
	        if (!m_InitResourcesComplete)
	            return;
	
	        ChangeState<ProcedurePreload>(procedureOwner);  //切换流程至预加载
	    }
	
	    //请求版本资源
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
	        //IOS相关配置
	            iOSGeneration = UnityEngine.iOS.Device.generation.ToString();
	            iOSSystemVersion = UnityEngine.iOS.Device.systemVersion;
	            iOSVendorIdentifier = UnityEngine.iOS.Device.vendorIdentifier ?? string.Empty;
#endif
	        string gameVersion = Version.GameVersion;   //游戏版本号
	        string platform = Application.platform.ToString();  //平台
	        string language = GameEntry.Localization.Language.ToString();   //语言包
	        string unityVersion = Application.unityVersion; //Unity版本号
	        string installMode = Application.installMode.ToString();    //安装模式
	        string sandboxType = Application.sandboxType.ToString();    //沙盒运行
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
	
	        GameEntry.WebRequest.AddWebRequest(GameEntry.BuiltinData.BuildInfo.CheckVersionUrl, wwwForm, this);
	    }
	
	    //网络请求成功的回调
	    private void OnWebRequestSuccess(object sender, BaseEventArgs e)
	    {
	        WebRequestSuccessEventArgs args = e as WebRequestSuccessEventArgs;
	        if (args.UserData != this)
	            return;
	
	        string responseJson = Utility.Converter.GetString(args.WebResponseBytes);   //网络返回转为string
	        VersionInfo versionInfo = Utility.Json.ToObject<VersionInfo>(responseJson);
	        if(versionInfo == null)
	        {
	            Log.Error("Parse VersionInfo failure.");
	            return;
	        }
	
	        Log.Info("Latest game version is '{0}', local game version is '{1}'.", versionInfo.LatestGameVersion, Version.GameVersion);
	
	        if (versionInfo.ForceGameUpdate)    //是否需要强制更新
	        {
	            GameEntry.UI.OpenDialog(new DialogParams
	            {
	                Mode = 2,
	                Title = GameEntry.Localization.GetString("ForceUpdate.Title"),
	                Message = GameEntry.Localization.GetString("ForceUpdate.Message"),
	                ConfirmText = GameEntry.Localization.GetString("ForceUpdate.UpdateButton"),
	                OnClickConfirm = delegate (object userData) { Application.OpenURL(versionInfo.GameUpdateUrl); },
	                CancelText = GameEntry.Localization.GetString("ForceUpdate.QuitButton"),
	                OnClickCancel = delegate (object userData) { UnityGameFrame.Runtime.GameEntry.Shutdown(ShutdownType.Quit); },
	            });
	
	            return;
	        }
	
	        //初始化资源
	        GameEntry.Resource.InitResources(OnInitResourcesComplete);
	    }
	
	    //网络请求失败的回调
	    private void OnWebRequestFailure(object sender, BaseEventArgs e)
	    {
	        WebRequestFailureEventArgs ne = (WebRequestFailureEventArgs)e;
	        if (ne.UserData != this)
	            return;
	
	        Log.Warning("Check version failure.");
	        GameEntry.Resource.InitResources(OnInitResourcesComplete);
	    }
	
	    //资源更新完成的回调
	    private void OnInitResourcesComplete()
	    {
	        m_InitResourcesComplete = true;
	
	        Log.Info("Init resources complete.");
	    }
	
	}
}
