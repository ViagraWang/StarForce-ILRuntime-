using Game.Runtime;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFrame.Runtime;
using GameEntry = Game.Runtime.GameEntry;

namespace Game.Hotfix
{	
	/// <summary>
	/// 关于界面
	/// </summary>
	public class AboutForm : UIFormBase
	{
	    [SerializeField]
	    private RectTransform m_RectTransform = null;
	    [SerializeField]
	    private float m_ScrollSpeed = 1f;   //滚动速度
	    private float m_InitPosition = 0f;  //初始位置
	
	    public override void OnInit(object userData)
	    {
	        base.OnInit(userData);
	        //画布大小
	        CanvasScaler canvasScaler = (CanvasScaler)RuntimeUIForm.GetComponentInParent(typeof(CanvasScaler));
	        if (canvasScaler == null)
	        {
                HotLog.Warning("Can not find CanvasScaler component.");
	            return;
	        }
	
	        //初始位置设置为高度的一半
	        m_InitPosition = -0.5f * canvasScaler.referenceResolution.x * Screen.height / Screen.width;

            ReferenceCollector collector = RuntimeUIForm.ReferenceCollector;
            m_RectTransform = (RectTransform)collector.Get("trans_Content", typeof(RectTransform));
            (collector.Get("bt_Back", typeof(CommonButton)) as CommonButton).ComButtonAddClick(OnClickClose);

        }

        public override void OnOpen(object userData)
	    {
	        GameEntry.Sound.PlayMusic(3);   //换个音乐
            m_RectTransform = RuntimeUIForm.CachedTransform as RectTransform;

        }

        public override void OnClose(object userData)
	    {
	        GameEntry.Sound.PlayMusic(1);   //还原音乐
	    }

        public override void OnUpdate()
	    {
	        base.OnUpdate();
	
	        m_RectTransform.AddLocalPositionY(m_ScrollSpeed * HotfixEntry.deltaTime);   //累加Y坐标
	        if (m_RectTransform.localPosition.y > m_RectTransform.sizeDelta.y - m_InitPosition)
	            m_RectTransform.SetLocalPositionY(m_InitPosition);  //复位
	    }
	

        private void OnClickClose()
        {
            RuntimeUIForm.Close();
        }

	}
}
