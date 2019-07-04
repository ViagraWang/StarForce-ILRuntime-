using GameFramework.Fsm;
using GameFramework.Procedure;
using Game.Runtime;
using UnityGameFrame.Runtime;
using GameEntry = Game.Runtime.GameEntry;
using GameFramework.Event;
using UnityGameFramework.Runtime;

namespace Game.Hotfix
{	
	//主菜单的流程
	public class ProcedureMenu : ProcedureLogic
	{
	    private bool m_StartGame = false;   //开始游戏的标志位
	    private MenuForm m_MenuForm = null; //菜单界面

        /// <summary>
        /// 开始游戏
        /// </summary>
        public void StartGame()
	    {
	        m_StartGame = true;
	    }
	
	
	    public override void OnEnter(IFsm<IProcedureManager> procedureOwner)
	    {	
	        GameEntry.Event.Subscribe(OpenUIFormSuccessEventArgs.EventId, OnOpenUIFormSuccess); //订阅打开UI的回调
	        m_StartGame = false;
	        //执行打开UI操作
	        GameEntry.UI.OpenUIForm(UIFormID.MenuForm, this);
	    }
	
	    public override void OnLeave(IFsm<IProcedureManager> procedureOwner, bool isShutdown)
	    {	
	        GameEntry.Event.Unsubscribe(OpenUIFormSuccessEventArgs.EventId, OnOpenUIFormSuccess);
	        if (m_MenuForm != null && m_MenuForm.RuntimeUIForm != null)
	        {
	            m_MenuForm.RuntimeUIForm.Close(isShutdown);
	            m_MenuForm = null;
	        }
	    }
	
	    public override void OnUpdate(IFsm<IProcedureManager> procedureOwner)
	    {	
	        if (m_StartGame)
	        {
	            procedureOwner.SetData(Constant.ProcedureData.NextSceneId, new VarInt(GameEntry.Config.GetInt("Scene.Main")));
	            procedureOwner.SetData(Constant.ProcedureData.GameMode, new VarInt((int)GameMode.Survival));
	
	            RuntimeProcedure.ChangeProcedure<HotProcedureChangeScene>(procedureOwner);
	        }
	    }
	
	    //打开UI成功的回调
	    private void OnOpenUIFormSuccess(object sender, GameEventArgs e)
	    {
	        OpenUIFormSuccessEventArgs args = e as OpenUIFormSuccessEventArgs;
            UserUIData uiData = args.UserData as UserUIData;

            if (uiData.UserData != this)
	            return;
	
	        m_MenuForm = uiData.RuntimeUIForm.HotLogicInstance as MenuForm;
	    }

        public override void OnDestroy(IFsm<IProcedureManager> procedureManager)
        {

        }
    }
}
