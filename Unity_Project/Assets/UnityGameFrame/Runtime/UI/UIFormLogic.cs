using UnityEngine;

namespace UnityGameFrame.Runtime
{
    /// <summary>
    /// 界面逻辑基类
    /// </summary>
    public abstract class UIFormLogic : MonoBehaviour
    {
        private bool m_Visible = false;
        private int m_OriginalLayer = 0;    //初始层级

        /// <summary>
        /// UI界面
        /// </summary>
        public UIForm UIForm { get; private set; }

        /// <summary>
        /// 对象名称
        /// </summary>
        public string Name { get { return CachedTransform.name; } set { CachedTransform.name = value; } }

        /// <summary>
        /// 是否显示
        /// </summary>
        public bool IsAvailable { get; private set; } = false;

        public bool Visible
        {
            get { return IsAvailable && m_Visible; }
            set
            {
                if (!IsAvailable)
                {
                    Log.Warning("UI form '{0}' is not available.", Name);
                    return;
                }

                if (m_Visible == value)
                    return;

                m_Visible = value;
                InternalSetVisible(value);
            }
        }

        /// <summary>
        /// 缓存的Transform
        /// </summary>
        public Transform CachedTransform { get; private set; }

        /// <summary>
        /// 缓存的GameObject
        /// </summary>
        public GameObject CachedGameObject { get; private set; }

        /// <summary>
        /// 界面初始化
        /// </summary>
        /// <param name="uiForm">关联的界面</param>
        /// <param name="userData">用户自定义数据</param>
        protected internal virtual void OnInit(UIForm uiForm, object userData)
        {
            UIForm = uiForm;
            if (CachedTransform == null)
                CachedTransform = uiForm.transform;
            if (CachedGameObject == null)
                CachedGameObject = CachedTransform.gameObject;

            m_OriginalLayer = CachedTransform.gameObject.layer;
        }

        /// <summary>
        /// 界面打开
        /// </summary>
        /// <param name="userData">用户自定义数据</param>
        protected internal virtual void OnOpen(object userData)
        {
            IsAvailable = true;
            Visible = true;
        }

        /// <summary>
        /// 界面关闭
        /// </summary>
        /// <param name="userData">用户自定义数据</param>
        protected internal virtual void OnClose(object userData)
        {
            CachedTransform.gameObject.SetLayerRecursively(m_OriginalLayer);
            Visible = false;
            IsAvailable = false;
        }

        /// <summary>
        /// 界面暂停
        /// </summary>
        protected internal virtual void OnPause()
        {
            Visible = false;
        }

        /// <summary>
        /// 界面暂停恢复
        /// </summary>
        protected internal virtual void OnResume()
        {
            Visible = true;
        }

        /// <summary>
        /// 界面覆盖
        /// </summary>
        protected internal virtual void OnCover() { } 

        /// <summary>
        /// 界面恢复覆盖
        /// </summary>
        protected internal virtual void OnReveal() { }

        /// <summary>
        /// 界面激活
        /// </summary>
        /// <param name="userData">用户自定义数据</param>
        protected internal virtual void OnRefocus(object userData) { }

        /// <summary>
        /// 界面轮询
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位</param>
        protected internal virtual void OnUpdate(float elapseSeconds, float realElapseSeconds) { }

        /// <summary>
        /// 界面深度改变
        /// </summary>
        /// <param name="uiGroupDepth">界面组深度</param>
        /// <param name="depthInUIGroup">界面在界面组中的深度</param>
        protected internal virtual void OnDepthChanged(int uiGroupDepth, int depthInUIGroup) { }

        /// <summary>
        /// 设置界面的可见性
        /// </summary>
        /// <param name="visible">界面的可见性</param>
        protected virtual void InternalSetVisible(bool visible)
        {
            CachedTransform.gameObject.SetActive(visible);
        }
    }
}
