using GameFramework;
using Game.Runtime;
using System;
using UnityEngine;
using UnityEngine.UI;
using GameEntry = Game.Runtime.GameEntry;

namespace Game.Hotfix
{	
	/// <summary>
	/// 对话界面
	/// </summary>
	public class DialogForm : UIFormBase
    {
	    private Text m_TitleText = null;    //标题文本
	    private Text m_MessageText = null;  //信息文本
	    private GameObject[] m_ModeObjects = null;  //模型对象
	    private Text[] m_ConfirmTexts = null;   //确认文本
	    private Text[] m_CancelTexts = null;    //取消文本
        private Text[] m_OtherTexts = null; //其他文本

        private Action<object> m_OnClickConfirm = null;   //点击确认按钮的事件
	    private Action<object> m_OnClickCancel = null;    //点击取消按钮的事件
	    private Action<object> m_OnClickOther = null;     //点击其他按钮的事件

        public int DialogMode { get; private set; } = 1;

        public bool IsPauseGame { get; private set; } = false;

        public object UserData { get; private set; } = null;

        //点击确认按钮
        public void OnConfirmButtonClick()
	    {
	        RuntimeUIForm.Close();
	
	        if (m_OnClickConfirm != null)
	            m_OnClickConfirm.Invoke(UserData);
	    }
	
	    //点击取消按钮
	    public void OnCancelButtonClick()
	    {
            RuntimeUIForm.Close();
	
	        if (m_OnClickCancel != null)
	            m_OnClickCancel.Invoke(UserData);
	    }
	
	    //点击其他按钮
	    public void OnOtherButtonClick()
	    {
            RuntimeUIForm. Close();
	
	        if (m_OnClickOther != null)
	            m_OnClickOther.Invoke(UserData);
	    }

        //初始化
        public override void OnInit(object userData)
        {
            base.OnInit(userData);

            ReferenceCollector collector = RuntimeUIForm.ReferenceCollector;
            m_TitleText = collector.Get("t_Title", typeof(Text)) as Text;
            m_MessageText = collector.Get("t_Message", typeof(Text)) as Text;

            m_ModeObjects = new GameObject[3];
            m_ConfirmTexts = new Text[3];
            for (int i = 0; i < 3; i++)
            {
                m_ModeObjects[i] = collector.GetGO(Utility.Text.Format("ModeObject{0}", i + 1));
                m_ConfirmTexts[i] = collector.Get(Utility.Text.Format("t_Confirm{0}", i + 1), typeof(Text)) as Text;
                (collector.Get(Utility.Text.Format("bt_Confirm{0}", i + 1), typeof(CommonButton)) as CommonButton).ComButtonAddClick(OnConfirmButtonClick);
            }

            m_CancelTexts = new Text[2];
            for (int i = 0; i < 2; i++)
            {
                m_CancelTexts[i] = collector.Get(Utility.Text.Format("t_Cancel{0}", i + 1), typeof(Text)) as Text;
                (collector.Get(Utility.Text.Format("bt_Cancel{0}", i + 1), typeof(CommonButton)) as CommonButton).ComButtonAddClick(OnCancelButtonClick);
            }

            m_OtherTexts = new Text[1];
            m_OtherTexts[0] = collector.Get("t_Other", typeof(Text)) as Text;
            (collector.Get("bt_Other", typeof(CommonButton)) as CommonButton).ComButtonAddClick(OnOtherButtonClick);

        }

        public override void OnOpen(object userData)
	    {	
	        //对话框数据
	        DialogParams dialogParams = (DialogParams)userData;
	        if (dialogParams == null)
	        {
                HotLog.Warning("DialogParams is invalid.");
	            return;
	        }
	
	        DialogMode = dialogParams.Mode;
	        RefreshDialogMode();
	        //信息
	        m_TitleText.text = dialogParams.Title;
	        m_MessageText.text = dialogParams.Message;
	        //暂停游戏
	        IsPauseGame = dialogParams.PauseGame;
	        RefreshPauseGame();
	
	        UserData = dialogParams.UserData;
	
	        //按钮文本和事件
	        RefreshConfirmText(dialogParams.ConfirmText);
	        m_OnClickConfirm = dialogParams.OnClickConfirm;
	        RefreshCancelText(dialogParams.CancelText);
	        m_OnClickCancel = dialogParams.OnClickCancel;
	        RefreshOtherText(dialogParams.OtherText);
	        m_OnClickOther = dialogParams.OnClickOther;
	    }
	
	    public override void OnClose(object userData)
	    {	
	        //恢复游戏
	        if (IsPauseGame)
	            GameEntry.Base.ResumeGame();
	
	        DialogMode = 1;
	        m_TitleText.text = string.Empty;
	        m_MessageText.text = string.Empty;
	        IsPauseGame = false;
	        UserData = null;
	
	        RefreshConfirmText(string.Empty);
	        m_OnClickConfirm = null;
	
	        RefreshCancelText(string.Empty);
	        m_OnClickCancel = null;
	
	        RefreshOtherText(string.Empty);
	        m_OnClickOther = null;
	    }
	
	    //刷新对话框模式
	    private void RefreshDialogMode()
	    {
	        for (int i = 1; i < m_ModeObjects.Length; i++)
	        {
	            m_ModeObjects[i - 1].SetActive(i == DialogMode);
	        }
	    }
	
	    //刷新暂停游戏
	    private void RefreshPauseGame()
	    {
	        if (IsPauseGame)
	            GameEntry.Base.PauseGame();
	    }
	
	    //刷新确认文本
	    private void RefreshConfirmText(string confirmText)
	    {
	        if (string.IsNullOrEmpty(confirmText))
	            confirmText = GameEntry.Localization.GetString("Dialog.ConfirmButton");
	
	        for (int i = 0; i < m_ConfirmTexts.Length; i++)
	        {
	            m_ConfirmTexts[i].text = confirmText;
	        }
	    }
	
	    //刷新取消按钮文本
	    private void RefreshCancelText(string cancelText)
	    {
	        if (string.IsNullOrEmpty(cancelText))
	        {
	            cancelText = GameEntry.Localization.GetString("Dialog.CancelButton");
	        }
	
	        for (int i = 0; i < m_CancelTexts.Length; i++)
	        {
	            m_CancelTexts[i].text = cancelText;
	        }
	    }
	
	    //刷新其他按钮文本
	    private void RefreshOtherText(string otherText)
	    {
	        if (string.IsNullOrEmpty(otherText))
	        {
	            otherText = GameEntry.Localization.GetString("Dialog.OtherButton");
	        }
	
	        for (int i = 0; i < m_OtherTexts.Length; i++)
	        {
	            m_OtherTexts[i].text = otherText;
	        }
	    }
	}
}
