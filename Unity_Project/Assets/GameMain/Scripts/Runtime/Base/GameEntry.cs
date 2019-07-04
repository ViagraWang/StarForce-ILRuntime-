using UnityEngine;
using UnityGameFrame.Runtime;

namespace Game.Runtime {	
	/// <summary>
	/// 游戏入口，自定义扩展
	/// </summary>
	[DefaultExecutionOrder(-100)]
	public partial class GameEntry : MonoBehaviour
	{
        public static GameEntry _Instance { get; private set; }

        private void Start()
	    {
            Log.Info("<color=red>GameEntry Start</color>");
            _Instance = this;

	        InitBuiltinComponent(); //初始化框架自带的基础组件
	        InitCustomComponents(); //初始化自定义组件
	    }
	}
}
