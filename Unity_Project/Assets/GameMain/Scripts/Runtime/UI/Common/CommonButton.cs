using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityGameFrame.Runtime;

namespace Game.Runtime
{	
	/// <summary>
	/// 通用按钮
	/// </summary>
	public class CommonButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
	{
	    private const float FadeTime = 0.3f;
	    private const float OnHoverAlpha = 0.7f;
	    private const float OnClickAlpha = 0.6f;
	
	    [SerializeField]
	    private UnityEvent m_OnPointerEnter = null;

        [SerializeField]
        private UnityEvent m_OnPointerExit = null;

        [SerializeField]
	    private UnityEvent m_OnPointerDown = null;

        [SerializeField]
        private UnityEvent m_OnPointerUp = null;

        [SerializeField]
        private UnityEvent m_OnClick = null;

        private CanvasGroup m_CanvasGroup = null;
	
	    protected virtual void Awake()
	    {
	        m_CanvasGroup = gameObject.GetOrAddComponent<CanvasGroup>();
	    }

        protected virtual void OnDisable()
	    {
	        m_CanvasGroup.alpha = 1f;
	    }
	
	    public virtual void OnPointerEnter(PointerEventData eventData)
	    {
            if (eventData.button != PointerEventData.InputButton.Left)
	        {
	            return;
	        }
	
	        StopAllCoroutines();
	        StartCoroutine(m_CanvasGroup.FadeToAlpha(OnHoverAlpha, FadeTime));
            if (m_OnPointerEnter != null)
                m_OnPointerEnter.Invoke();
	    }
	
	    public virtual void OnPointerExit(PointerEventData eventData)
	    {
            if (eventData.button != PointerEventData.InputButton.Left)
	        {
	            return;
	        }
	
	        StopAllCoroutines();
	        StartCoroutine(m_CanvasGroup.FadeToAlpha(1f, FadeTime));
            if (m_OnPointerExit != null)
                m_OnPointerExit.Invoke();

        }

        public virtual void OnPointerDown(PointerEventData eventData)
	    {

            if (eventData.button != PointerEventData.InputButton.Left)
	        {
	            return;
	        }
	
	        m_CanvasGroup.alpha = OnClickAlpha;
            if (m_OnPointerDown != null)
                m_OnPointerDown.Invoke();
	    }
	
	    public virtual void OnPointerUp(PointerEventData eventData)
	    {

            if (eventData.button != PointerEventData.InputButton.Left)
	        {
	            return;
	        }
	
	        m_CanvasGroup.alpha = OnHoverAlpha;
            if (m_OnPointerUp != null)
                m_OnPointerUp.Invoke();
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (m_OnClick != null)
                m_OnClick.Invoke();
        }

        //添加进入事件
        internal void EnterAddListener(UnityAction action)
        {
            m_OnPointerEnter.AddListener(action);
        }

        //移除进入事件
        internal void EnterRemoveListener(UnityAction action)
        {
            m_OnPointerEnter.RemoveListener(action);
        }

        //移除所有进入事件
        internal void EnterRemoveAllListeners()
        {
            m_OnPointerEnter.RemoveAllListeners();
        }

        //添加离开事件
        internal void ExitAddListener(UnityAction action)
        {
            m_OnPointerExit.AddListener(action);
        }

        //移除离开事件
        internal void ExitRemoveListener(UnityAction action)
        {
            m_OnPointerExit.RemoveListener(action);
        }

        //移除所有离开事件
        internal void ExitRemoveAllListeners()
        {
            m_OnPointerExit.RemoveAllListeners();
        }

        //添加按下事件
        internal void DownAddListener(UnityAction action)
        {
            m_OnPointerDown.AddListener(action);
        }

        //移除按下事件
        internal void DownRemoveListener(UnityAction action)
        {
            m_OnPointerDown.RemoveListener(action);
        }

        //移除所有按下事件
        internal void DownRemoveAllListeners()
        {
            m_OnPointerDown.RemoveAllListeners();
        }

        //添加抬起事件
        internal void UpAddListener(UnityAction action)
        {
            m_OnPointerUp.AddListener(action);
        }

        //移除抬起事件
        internal void UpRemoveListener(UnityAction action)
        {
            m_OnPointerUp.RemoveListener(action);
        }

        //移除所有抬起事件
        internal void UpRemoveAllListeners()
        {
            m_OnPointerUp.RemoveAllListeners();
        }

        //添加点击事件
        internal void ClickAddListener(UnityAction action)
        {
            m_OnClick.AddListener(action);
        }

        //移除点击事件
        internal void ClickRemoveListener(UnityAction action)
        {
            m_OnClick.RemoveListener(action);
        }

        //移除所有点击事件
        internal void ClickRemoveAllListeners()
        {
            m_OnClick.RemoveAllListeners();
        }

    }
}
