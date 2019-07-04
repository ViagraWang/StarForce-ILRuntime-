using GameFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFrame.Runtime;

namespace Game.Runtime
{

	/// <summary>
	/// UGUI界面
	/// </summary>
	public abstract class UGUIForm : UIFormLogic
	{
	    public const int DepthFactor = 100; //界面的深度因子
	    private const float FadeTime = 0.3f;    //淡化时间
	
	    private static Font s_MainFont = null;  //主字体
	    private Canvas m_CachedCanvas = null;   //缓存的画布
	    private CanvasGroup m_CanvasGroup = null;   //画布组
	
	    /// <summary>
	    /// 初始深度
	    /// </summary>
	    public int OriginalDepth { get; private set; }
	
	    /// <summary>
	    /// 深度
	    /// </summary>
	    public int Depth { get { return m_CachedCanvas.sortingOrder; } }
	
	    //播放UI音效
	    public void PlayUISound(int uiSoundId)
	    {
	        GameEntry.Sound.PlayUISound(uiSoundId);
	    }
	
	    //设置主字体
	    public static void SetMainFont(Font mainFont)
	    {
	        if (mainFont == null)
	        {
	            Log.Error("Main font is invalid.");
	            return;
	        }
	
	        s_MainFont = mainFont;
	
	        //这里是做什么用的？
	        //GameObject go = new GameObject();
	        //go.AddComponent<Text>().font = mainFont;
	        //Destroy(go);
	    }
	
	    protected override void OnInit(UIForm uiform, object userData)
	    {
	        base.OnInit(uiform, userData);
	        m_CachedCanvas = CachedGameObject.GetOrAddComponent<Canvas>();
	        m_CachedCanvas.worldCamera = GameEntry.UI.UICamera; //设置UI相机
	        m_CachedCanvas.overrideSorting = true;
	        OriginalDepth = m_CachedCanvas.sortingOrder;    //初始深度
	
	        m_CanvasGroup = CachedGameObject.GetOrAddComponent<CanvasGroup>();
	
	        RectTransform rectTrans = CachedTransform as RectTransform;
	        rectTrans.anchorMin = Vector2.zero;
	        rectTrans.anchorMax = Vector2.one;
	        rectTrans.anchoredPosition = Vector2.zero;
	        rectTrans.sizeDelta = Vector2.zero;

            CachedGameObject.GetOrAddComponent<GraphicRaycaster>();
	
	        //设置所有字体
	        Text[] texts = GetComponentsInChildren<Text>(true);
	        for (int i = 0; i < texts.Length; i++)
	        {
	            texts[i].font = s_MainFont;
	            if (!string.IsNullOrEmpty(texts[i].text))
	                texts[i].text = GameEntry.Localization.GetString(texts[i].text);
	        }
	    }
	
	    //界面打开时的回调
	    protected override void OnOpen(object userData)
	    {
	        base.OnOpen(userData);
	        m_CanvasGroup.alpha = 0f;
            StopAllCoroutines();
	        StartCoroutine(m_CanvasGroup.FadeToAlpha(1f, FadeTime));
	    }
	
	    //界面关闭的回调
	    protected override void OnClose(object userData)
	    {
	        base.OnClose(userData);
	    }
	
	    //界面暂停的回调
	    protected override void OnPause()
	    {
	        base.OnPause();
	    }
	
	    //界面恢复的回调
	    protected override void OnResume()
	    {
	        base.OnResume();
	
	        m_CanvasGroup.alpha = 0f;
            StopAllCoroutines();
            StartCoroutine(m_CanvasGroup.FadeToAlpha(1f, FadeTime));
	    }
	
	    //界面被遮挡覆盖的回调
	    protected override void OnCover()
	    {
	        base.OnCover();
	    }
	
	    //界面遮挡恢复的回调
	    protected override void OnReveal()
	    {
	        base.OnReveal();
	    }
	
	    //界面重新激活的回调
	    protected override void OnRefocus(object userData)
	    {
	        base.OnRefocus(userData);
	    }
	
	    //深度改变的回调
	    protected override void OnDepthChanged(int uiGroupDepth, int depthInUIGroup)
	    {
	        base.OnDepthChanged(uiGroupDepth, depthInUIGroup);
	
	        int oldDepth = Depth;
	        int deltaDepth = UGUIGroupHelper.DepthFactor * uiGroupDepth + DepthFactor * depthInUIGroup - oldDepth + OriginalDepth;
	        Canvas[] canvases = GetComponentsInChildren<Canvas>(true);
	        for (int i = 0; i < canvases.Length; i++)
	        {
	            canvases[i].sortingOrder += deltaDepth;
	        }
	    }
	
	    /// <summary>
	    /// 关闭界面
	    /// </summary>
	    /// <param name="ignoreFade">忽略淡化效果</param>
	    public void Close(bool ignoreFade = false)
	    {
            StopAllCoroutines();
	
	        if (ignoreFade)
	            GameEntry.UI.CloseUIForm(UIForm);
	        else
                StartCoroutine(CloseCo(FadeTime));
	    }
	
	    //延迟关闭界面
	    private IEnumerator CloseCo(float duration)
	    {
	        yield return m_CanvasGroup.FadeToAlpha(0f, duration);
	        GameEntry.UI.CloseUIForm(UIForm);
	    }

    }
}
