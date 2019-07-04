using System.Collections.Generic;
using GameFramework.Fsm;
using GameFramework.Procedure;
using Game.Runtime;
using UnityGameFramework.Runtime;

namespace Game.Hotfix
{	
	//主流程类
	public class ProcedureMain : ProcedureLogic
	{
	    private const float GameOverDelayedSeconds = 2f;    //等待返回主菜单的时间
	
	    private readonly Dictionary<GameMode, GameBase> m_Games = new Dictionary<GameMode, GameBase>(); //所有游戏逻辑处理类
	    private GameBase m_CurrentGame = null;  //当前游戏逻辑的处理类
	    private bool m_GotoMenu = false;    //返回菜单的标志位
	    private float m_GotoMenuDelaySeconds = 0f;  //记录返回菜单的延迟时间
		
	    public void GotoMenu()
	    {
	        m_GotoMenu = true;
	    }
	
	    //初始化流程，框架启动时调用
	    public override void OnInit(IFsm<IProcedureManager> procedureOwner, HotProcedure procedureBind)
	    {
            base.OnInit(procedureOwner, procedureBind);

            //添加游戏逻辑控制器
            m_Games.Add(GameMode.Survival, new SurvivalGame());
	    }

        //销毁流程，框架关闭时调用
        public override void OnDestroy(IFsm<IProcedureManager> procedureOwner)
	    {
	        m_Games.Clear();
	    }
	
	    //进入流程
	    public override void OnEnter(IFsm<IProcedureManager> procedureOwner)
	    {
	        m_GotoMenu = false; //返回菜单标志位false
	        //获取游戏类型并初始化
	        GameMode gameMode = (GameMode)procedureOwner.GetData<VarInt>(Constant.ProcedureData.GameMode).Value;
	        m_CurrentGame = m_Games[gameMode];
	        m_CurrentGame.Initialize();
	    }
	
	    public override void OnLeave(IFsm<IProcedureManager> procedureOwner, bool isShutdown)
	    {
	        //清理当前游戏
	        if(m_CurrentGame != null)
	        {
	            m_CurrentGame.Shutdown();
	            m_CurrentGame = null;
	        }
	    }
	
	
	    public override void OnUpdate(IFsm<IProcedureManager> procedureOwner)
	    {	
	        if(m_CurrentGame != null && !m_CurrentGame.IsGameOver)
	        {
	            m_CurrentGame.Update(HotfixEntry.deltaTime, HotfixEntry.unscaleDeltaTime);
	            return;
	        }
	
	        if (!m_GotoMenu)
	        {
	            m_GotoMenu = true;
	            m_GotoMenuDelaySeconds = 0f;
	        }
	
	        m_GotoMenuDelaySeconds += HotfixEntry.deltaTime;
            if (m_GotoMenuDelaySeconds >= GameOverDelayedSeconds)
	        {
	            procedureOwner.SetData<VarInt>(Constant.ProcedureData.NextSceneId, new VarInt(GameEntry.Config.GetInt("Scene.Menu")));
	            RuntimeProcedure.ChangeProcedure<HotProcedureChangeScene>(procedureOwner);
	        }
	
	    }
    }
}
