using UnityGameFrame.Runtime;

namespace Game.Runtime
{	
	/// <summary>
	/// 游戏入口，内置框架组件
	/// </summary>
	public partial class GameEntry
	{
	    /// <summary>
	    /// 获取游戏基础组件
	    /// </summary>
	    public static BaseComponent Base { get; private set; }
	
	
	    /// <summary>
	    /// 获取配置组件。
	    /// </summary>
	    public static ConfigComponent Config { get; private set; }
	
	    /// <summary>
	    /// 获取数据结点组件。
	    /// </summary>
	    public static DataNodeComponent DataNode { get; private set; }
	
	    /// <summary>
	    /// 获取数据表组件。
	    /// </summary>
	    public static DataTableComponent DataTable { get; private set; }
	
	    /// <summary>
	    /// 获取调试组件。
	    /// </summary>
	    public static DebuggerComponent Debugger { get; private set; }
	
	    /// <summary>
	    /// 获取下载组件。
	    /// </summary>
	    public static DownloadComponent Download { get; private set; }
	
	    /// <summary>
	    /// 获取实体组件。
	    /// </summary>
	    public static EntityComponent Entity { get; private set; }
	
	    /// <summary>
	    /// 获取事件组件。
	    /// </summary>
	    public static EventComponent Event { get; private set; }
	
	    /// <summary>
	    /// 获取有限状态机组件。
	    /// </summary>
	    public static FsmComponent Fsm { get; private set; }
	
	    /// <summary>
	    /// 获取本地化组件。
	    /// </summary>
	    public static LocalizationComponent Localization { get; private set; }
	
	    /// <summary>
	    /// 获取网络组件。
	    /// </summary>
	    public static NetworkComponent Network { get; private set; }
	
	    /// <summary>
	    /// 获取对象池组件。
	    /// </summary>
	    public static ObjectPoolComponent ObjectPool { get; private set; }
	
	    /// <summary>
	    /// 获取流程组件。
	    /// </summary>
	    public static ProcedureComponent Procedure { get; private set; }
	
	    /// <summary>
	    /// 获取资源组件。
	    /// </summary>
	    public static ResourceComponent Resource { get; private set; }
	
	    /// <summary>
	    /// 获取场景组件。
	    /// </summary>
	    public static SceneComponent Scene { get; private set; }
	
	    /// <summary>
	    /// 获取配置组件。
	    /// </summary>
	    public static SettingComponent Setting { get; private set; }
	
	    /// <summary>
	    /// 获取声音组件。
	    /// </summary>
	    public static SoundComponent Sound { get; private set; }
	
	    /// <summary>
	    /// 获取界面组件。
	    /// </summary>
	    public static UIComponent UI { get; private set; }
	
	    /// <summary>
	    /// 获取网络组件。
	    /// </summary>
	    public static WebRequestComponent WebRequest { get; private set; }
	
	    //初始化内置组件
	    private static void InitBuiltinComponent()
	    {
	        Base = UnityGameFrame.Runtime.GameEntry.GetComponent<BaseComponent>();
	        Config = UnityGameFrame.Runtime.GameEntry.GetComponent<ConfigComponent>();
	        DataNode = UnityGameFrame.Runtime.GameEntry.GetComponent<DataNodeComponent>();
	        DataTable = UnityGameFrame.Runtime.GameEntry.GetComponent<DataTableComponent>();
	        Debugger = UnityGameFrame.Runtime.GameEntry.GetComponent<DebuggerComponent>();
	        Download = UnityGameFrame.Runtime.GameEntry.GetComponent<DownloadComponent>();
	        Entity = UnityGameFrame.Runtime.GameEntry.GetComponent<EntityComponent>();
	        Event = UnityGameFrame.Runtime.GameEntry.GetComponent<EventComponent>();
	        Fsm = UnityGameFrame.Runtime.GameEntry.GetComponent<FsmComponent>();
	        Localization = UnityGameFrame.Runtime.GameEntry.GetComponent<LocalizationComponent>();
	        Network = UnityGameFrame.Runtime.GameEntry.GetComponent<NetworkComponent>();
	        ObjectPool = UnityGameFrame.Runtime.GameEntry.GetComponent<ObjectPoolComponent>();
	        Procedure = UnityGameFrame.Runtime.GameEntry.GetComponent<ProcedureComponent>();
	        Resource = UnityGameFrame.Runtime.GameEntry.GetComponent<ResourceComponent>();
	        Scene = UnityGameFrame.Runtime.GameEntry.GetComponent<SceneComponent>();
	        Setting = UnityGameFrame.Runtime.GameEntry.GetComponent<SettingComponent>();
	        Sound = UnityGameFrame.Runtime.GameEntry.GetComponent<SoundComponent>();
	        UI = UnityGameFrame.Runtime.GameEntry.GetComponent<UIComponent>();
	        WebRequest = UnityGameFrame.Runtime.GameEntry.GetComponent<WebRequestComponent>();
	    }
	
	}
}
