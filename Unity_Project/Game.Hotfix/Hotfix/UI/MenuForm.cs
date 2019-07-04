using Game.Runtime;
using UnityEngine;
using UnityGameFrame.Runtime;
using GameEntry = Game.Runtime.GameEntry;

namespace Game.Hotfix
{	
	/// <summary>
	/// 菜单界面
	/// </summary>
	public class MenuForm : UIFormBase
	{
	    [SerializeField]
	    private GameObject m_QuitButton = null; //退出按钮
	
	    private ProcedureMenu m_ProcedureMenu = null;   //菜单流程管理
	
	    //点击开始游戏按钮
	    public void OnStartButtonClick()
	    {
            HotLog.Info("On Click Start Button");
            m_ProcedureMenu.StartGame();
	    }
	
	    //点击设置按钮
	    public void OnSettingButtonClick()
	    {
            HotLog.Info("On Click Setting Button");
            GameEntry.UI.OpenUIForm(UIFormID.SettingForm);
	    }
	
	    //点击关于按钮
	    public void OnAboutButtonClick()
	    {
            HotLog.Info("On Click About Button");
            GameEntry.UI.OpenUIForm(UIFormID.AboutForm);
	    }
	
	    //点击退出按钮
	    public void OnQuitButtonClick()
	    {
            HotLog.Info("On Click Quit Button");
	        //打开对话框
	        GameEntry.UI.OpenDialog(new DialogParams()
	        {
	            Mode = 2,
	            Title = GameEntry.Localization.GetString("AskQuitGame.Title"),
	            Message = GameEntry.Localization.GetString("AskQuitGame.Message"),
	            OnClickConfirm = delegate (object userData) { UnityGameFrame.Runtime.GameEntry.Shutdown(ShutdownType.Quit); },
	
	        });
	    }

        public override void OnInit(object userData)
        {
            base.OnInit(userData);

            ReferenceCollector collector = RuntimeUIForm.ReferenceCollector;
            m_QuitButton = collector.GetGO("bt_Quit");
            (m_QuitButton.GetComponent(typeof(CommonButton)) as CommonButton).ComButtonAddClick(OnQuitButtonClick);
            m_QuitButton.SetActive(Application.platform != RuntimePlatform.IPhonePlayer);
            (collector.Get("bt_About", typeof(CommonButton)) as CommonButton).ComButtonAddClick(OnAboutButtonClick);
            (collector.Get("bt_Setting", typeof(CommonButton)) as CommonButton).ComButtonAddClick(OnSettingButtonClick);
            (collector.Get("bt_Start", typeof(CommonButton)) as CommonButton).ComButtonAddClick(OnStartButtonClick);
        }

        public override void OnOpen(object userData)
	    {	
	        m_ProcedureMenu = userData as ProcedureMenu;
	        if(m_ProcedureMenu == null)
	        {
                HotLog.Warning("ProcedureMenu is invalid when open MenuForm.");
	            return;
	        }

        }

        public override void OnClose(object userData)
	    {
	        m_ProcedureMenu = null;
	    }
	
	}
}
