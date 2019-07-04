using Game.Runtime;
using GameFramework.Event;
using UnityEngine;
using UnityGameFrame.Runtime;
using GameEntry = Game.Runtime.GameEntry;

namespace Game.Hotfix
{	
	//游戏基类
	public abstract class GameBase
	{
	    /// <summary>
	    /// 游戏模式
	    /// </summary>
		public abstract GameMode GameMode { get; }
	
	    /// <summary>
	    /// 滚动背景
	    /// </summary>
	    protected ScrollableBackground SceneBackground { get; private set; }
	
	    /// <summary>
	    /// 是否游戏结束的标志位
	    /// </summary>
	    public bool IsGameOver { get; protected set; }
	
	    private MyAircraft m_MyAircraft = null; //我的战机
	
	    //初始化
	    public virtual void Initialize()
	    {
	        //注册事件
	        GameEntry.Event.Subscribe(ShowEntitySuccessEventArgs.EventId, OnShowEntitySuccess);
	        GameEntry.Event.Subscribe(ShowEntityFailureEventArgs.EventId, OnShowEntityFailure);
	        //滚动背景
	        SceneBackground = Object.FindObjectOfType<ScrollableBackground>();
	        if(SceneBackground == null)
	        {
                HotLog.Warning("Can not find scene background.");
	            return;
	        }
	        SceneBackground.VisibleBoundary.gameObject.GetOrAddComponent<HideByBoundary>(); //添加触发离开时隐藏实体
	        GameEntry.Entity.ShowMyAircraft(new MyAircraftData(GameEntry.Entity.GenerateSerialId(), 10000)
	        {
	            Name = "My Aircraft",
	            Position = Vector3.zero
	        });
	
	        IsGameOver = false;
	        m_MyAircraft = null;
	    }
	
	    //关闭并清理
	    public virtual void Shutdown()
	    {
	        GameEntry.Event.Unsubscribe(ShowEntitySuccessEventArgs.EventId, OnShowEntitySuccess);
	        GameEntry.Event.Unsubscribe(ShowEntityFailureEventArgs.EventId, OnShowEntityFailure);
	    }
	
	    //更新
	    public virtual void Update(float elapseSeconds, float realElapseSeconds)
	    {
	        if(!IsGameOver && m_MyAircraft != null && m_MyAircraft.IsDead)
	        {
	            IsGameOver = true;
                HotLog.Debug("玩家死亡，游戏结束");
	            return;
	        }
	    }
	
	    //显示实体成功的回调
	    protected virtual void OnShowEntitySuccess(object sender, GameEventArgs e)
	    {
	        ShowEntitySuccessEventArgs args = e as ShowEntitySuccessEventArgs;
            UserEntityData entityData = args.UserData as UserEntityData;
            if(entityData != null && entityData.HotLogicTypeName == typeof(MyAircraft).Name)
	            m_MyAircraft = entityData.RuntimeEntity.HotLogicInstance as MyAircraft;
	    }
	
	
	    protected virtual void OnShowEntityFailure(object sender, GameEventArgs e)
	    {
	        ShowEntityFailureEventArgs ne = e as ShowEntityFailureEventArgs;
            HotLog.Warning("Show entity failure with error message '{0}'.", ne.ErrorMessage);
	    }
	
	}
}
