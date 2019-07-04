using UnityEngine;
using UnityEngine.UI;
using UnityGameFrame.Runtime;

namespace Game.Runtime {	
	/// <summary>
	/// UGUI界面组辅助器
	/// </summary>
	public class UGUIGroupHelper : UIGroupHelperBase
	{
	    public const int DepthFactor = 10000;   //UI组的深度因子
	
	    private int m_Depth = 0;    //当前深度
	    private Canvas m_CachedCanvas = null;   //画布
	
	    private void Awake()
	    {
	        m_CachedCanvas = gameObject.GetOrAddComponent<Canvas>();
	        gameObject.GetOrAddComponent<GraphicRaycaster>();   //添加射线检测机制
	    }
	
	    private void Start()
	    {
	        m_CachedCanvas.overrideSorting = true;
	        m_CachedCanvas.sortingOrder = DepthFactor * m_Depth;
	
	        //设置界面组的布局参数
	        RectTransform rectTrans = transform as RectTransform;
	        rectTrans.anchorMin = Vector2.zero;
	        rectTrans.anchorMax = Vector2.one;
	        rectTrans.anchoredPosition = Vector2.zero;
	        rectTrans.sizeDelta = Vector2.zero;
	
	    }
	
	    /// <summary>
	    /// 设置界面组深度
	    /// </summary>
	    /// <param name="depth">界面组深度</param>
	    public override void SetDepth(int depth)
	    {
	        m_Depth = depth;
	        m_CachedCanvas.overrideSorting = true;
	        m_CachedCanvas.sortingOrder = DepthFactor * depth;
	    }
	}
}
