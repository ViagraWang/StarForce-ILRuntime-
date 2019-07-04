using Game.Runtime;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFrame.Runtime;
using Entity = Game.Runtime.Entity;
using GameEntry = Game.Runtime.GameEntry;

namespace Game.Hotfix
{	
	//血条
	public class HPBarItem
	{
        public ReferenceCollector ReferenceCollector { get; private set; }

        private const float AnimationSeconds = 0.3f;    //动画时间
	    private const float KeepSeconds = 0.4f; //存在时间
	    private const float FadeOutSeconds = 0.3f;  //淡出时间
	    private WaitForSeconds keepSeconds = new WaitForSeconds(KeepSeconds);
	
	    [SerializeField]
	    private Slider m_HPBar = null;
	
	    private Canvas m_ParentCanvas = null;
	    private RectTransform m_CachedTransform = null;
	    private CanvasGroup m_CachedCanvasGroup = null;
	    private Entity m_Owner = null;  //所属的实体
	    private int m_OwnerId = 0;
	
	    public Entity Owner { get { return m_Owner; } }
	
	    public HPBarItem(ReferenceCollector referenceCollector)
	    {
            ReferenceCollector = referenceCollector;
            ReferenceCollector.ComponentView.Component = this;

            m_HPBar = ReferenceCollector.Get("slider_HP", typeof(Slider)) as Slider;

            //获取组件
            m_CachedTransform = ReferenceCollector.CachedTransform as RectTransform;
	        if (m_CachedTransform == null)
	        {
	            Log.Error("RectTransform is invalid.");
	            return;
	        }
	
	        m_CachedCanvasGroup = ReferenceCollector.GetComponent(typeof(CanvasGroup)) as CanvasGroup;
	        if (m_CachedCanvasGroup == null)
	        {
	            Log.Error("CanvasGroup is invalid.");
	            return;
	        }
	    }
	
	    public void Init(Entity owner, Canvas parentCanvas, float fromHPRatio, float toHPRatio)
	    {
	        if (owner == null)
	        {
	            Log.Error("Owner is invalid.");
	            return;
	        }
	
	        m_ParentCanvas = parentCanvas;

            ReferenceCollector.gameObject.SetActive(true);
            ReferenceCollector.StopAllCoroutines();
	
	        m_CachedCanvasGroup.alpha = 1f;
	
	        //检查所属实体
	        if (m_Owner != owner || m_OwnerId != owner.Id)
	        {
	            m_HPBar.value = fromHPRatio;
	            m_Owner = owner;
	            m_OwnerId = owner.Id;
	        }
	
	        Refresh();  //刷新

            ReferenceCollector.StartCoroutine(HPBarCo(toHPRatio, AnimationSeconds, KeepSeconds, FadeOutSeconds));
	    }
	
	    //刷新
	    public bool Refresh()
	    {
	        if (m_CachedCanvasGroup.alpha <= 0f)
                return false;
	
	        if(m_Owner != null && m_Owner.IsAvailable && m_Owner.Id == m_OwnerId)
	        {
	            Vector3 worldPosition = m_Owner.CachedTransform.position;
	            Vector3 screenPosition = GameEntry.Scene.MainCamera.WorldToScreenPoint(worldPosition);
	
	            Vector2 position;
	            //屏幕坐标转UI坐标
	            if(RectTransformUtility.ScreenPointToLocalPointInRectangle(m_ParentCanvas.transform as RectTransform, screenPosition, m_ParentCanvas.worldCamera, out position))
	            {
	                m_CachedTransform.anchoredPosition = position;
	            }
	        }

	        return true;
	    }
	
	    //血条动画
	    private IEnumerator HPBarCo(float value, float animationDuration, float keepDuration, float fadeOutDuration)
	    {
	        yield return m_HPBar.SmoothValue(value, animationDuration);
	        yield return keepSeconds;
	        yield return m_CachedCanvasGroup.FadeToAlpha(0f, fadeOutDuration);
	    }
	
	    public void Reset()
	    {
            ReferenceCollector.StopAllCoroutines();    //停止所有协程
	        m_CachedCanvasGroup.alpha = 1f;
	        m_HPBar.value = 1f;
	        m_Owner = null;
            ReferenceCollector.gameObject.SetActive(false);
	    }
	
	}
}
