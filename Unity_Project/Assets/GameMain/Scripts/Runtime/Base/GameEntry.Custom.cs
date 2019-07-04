using UnityEngine;

namespace Game.Runtime
{	
	/// <summary>
	/// 游戏入口，自定义组件
	/// </summary>
	public partial class GameEntry
	{
	    /// <summary>
	    /// 内置数据组件
	    /// </summary>
	    public static BuiltinDataComponent BuiltinData { get; private set; }
	
	    /// <summary>
	    /// 血条组件
	    /// </summary>
	    //public static HPBarComponent HPBar { get; private set; }
	
	    /// <summary>
	    /// 热更新组件
	    /// </summary>
	    public static HotfixComponent Hotfix { get; private set; }
	
	    //初始化自动以组件
	    private static void InitCustomComponents()
	    {
	        BuiltinData = UnityGameFrame.Runtime.GameEntry.GetComponent<BuiltinDataComponent>();
	        //HPBar = UnityGameFrame.Runtime.GameEntry.GetComponent<HPBarComponent>();
	        Hotfix = UnityGameFrame.Runtime.GameEntry.GetComponent<HotfixComponent>();
	    }
	
	}
}
