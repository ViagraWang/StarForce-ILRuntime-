using GameFramework;
using GameFramework.WebRequest;
using UnityEngine;

namespace UnityGameFrame.Runtime
{
    /// <summary>
    /// Web 请求组件
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Game Framework/Web Request")]
    public sealed class WebRequestComponent : GameFrameworkComponent
    {
        private IWebRequestManager m_WebRequestManager = null;  //Web请求管理器
        private EventComponent m_EventComponent = null;

        [SerializeField]
        private Transform m_InstanceRoot = null;    //根对象
        [SerializeField]
        private string m_WebRequestAgentHelperTypeName = "UnityGameFrame.Runtime.UnityWebRequestAgentHelper";   //默认Web请求代理辅助器类型名称

        [SerializeField]
        private WebRequestAgentHelperBase m_CustomWebRequestAgentHelper = null; //用户自定义的辅助器

        [SerializeField]
        private int m_WebRequestAgentHelperCount = 1;   //配置代理辅助器的数量

        [SerializeField]
        private float m_Timeout = 30f;  //超时时长

        /// <summary>
        /// 获取 Web 请求代理总数量
        /// </summary>
        public int TotalAgentCount { get { return m_WebRequestManager.TotalAgentCount; } }

        /// <summary>
        /// 获取可用 Web 请求代理数量
        /// </summary>
        public int FreeAgentCount { get { return m_WebRequestManager.FreeAgentCount; } }

        /// <summary>
        /// 获取工作中 Web 请求代理数量
        /// </summary>
        public int WorkingAgentCount { get { return m_WebRequestManager.WorkingAgentCount; } }

        /// <summary>
        /// 获取等待 Web 请求数量
        /// </summary>
        public int WaitingTaskCount { get { return m_WebRequestManager.WaitingTaskCount; } }

        /// <summary>
        /// 获取或设置 Web 请求超时时长，以秒为单位
        /// </summary>
        public float Timeout
        {
            get { return m_WebRequestManager.Timeout; }
            set { m_WebRequestManager.Timeout = m_Timeout = value; }
        }

        protected override void Awake()
        {
            base.Awake();
            m_WebRequestManager = GameFrameworkEntry.GetModule<IWebRequestManager>();
            if (m_WebRequestManager == null)
            {
                Log.Fatal("[WebRequestComponent.Awake] Web request manager is invalid.");
                return;
            }

            //配置
            m_WebRequestManager.Timeout = m_Timeout;
            //注册事件
            m_WebRequestManager.WebRequestStart += OnWebRequestStart;
            m_WebRequestManager.WebRequestSuccess += OnWebRequestSuccess;
            m_WebRequestManager.WebRequestFailure += OnWebRequestFailure;
        }

        private void Start()
        {
            m_EventComponent = GameEntry.GetComponent<EventComponent>();
            if (m_EventComponent == null)
            {
                Log.Fatal("[WebRequestComponent.Start] Event component is invalid.");
                return;
            }
            //配置根对象
            if (m_InstanceRoot == null)
            {
                m_InstanceRoot = (new GameObject("Web Request Agent Instances")).transform;
                m_InstanceRoot.SetParent(gameObject.transform);
                m_InstanceRoot.localScale = Vector3.one;
            }
            //添加代理辅助器
            for (int i = 0; i < m_WebRequestAgentHelperCount; i++)
            {
                AddWebRequestAgentHelper(i);
            }
        }

        /// <summary>
        /// 增加 Web 请求任务
        /// </summary>
        /// <param name="webRequestUri">Web 请求地址</param>
        /// <param name="userData">用户自定义数据</param>
        /// <returns>新增 Web 请求任务的序列编号</returns>
        public int AddWebRequest(string webRequestUri, object userData = null)
        {
            return AddWebRequest(webRequestUri, null, null, 0, userData);
        }

        /// <summary>
        /// 增加 Web 请求任务
        /// </summary>
        /// <param name="webRequestUri">Web 请求地址</param>
        /// <param name="postData">要发送的数据流</param>
        /// <param name="userData">用户自定义数据</param>
        /// <returns>新增 Web 请求任务的序列编号</returns>
        public int AddWebRequest(string webRequestUri, byte[] postData, object userData = null)
        {
            return AddWebRequest(webRequestUri, postData, null, 0, userData);
        }

        /// <summary>
        /// 增加 Web 请求任务
        /// </summary>
        /// <param name="webRequestUri">Web 请求地址</param>
        /// <param name="wwwForm">WWW 表单</param>
        /// <param name="userData">用户自定义数据</param>
        /// <returns>新增 Web 请求任务的序列编号</returns>
        public int AddWebRequest(string webRequestUri, WWWForm wwwForm, object userData = null)
        {
            return AddWebRequest(webRequestUri, null, wwwForm, 0, userData);
        }

        /// <summary>
        /// 增加 Web 请求任务
        /// </summary>
        /// <param name="webRequestUri">Web 请求地址</param>
        /// <param name="priority">Web 请求任务的优先级</param>
        /// <param name="userData">用户自定义数据</param>
        /// <returns>新增 Web 请求任务的序列编号</returns>
        public int AddWebRequest(string webRequestUri, int priority, object userData = null)
        {
            return AddWebRequest(webRequestUri, null, null, priority, userData);
        }

        /// <summary>
        /// 增加 Web 请求任务
        /// </summary>
        /// <param name="webRequestUri">Web 请求地址</param>
        /// <param name="postData">要发送的数据流</param>
        /// <param name="priority">Web 请求任务的优先级</param>
        /// <param name="userData">用户自定义数据</param>
        /// <returns>新增 Web 请求任务的序列编号</returns>
        public int AddWebRequest(string webRequestUri, byte[] postData, int priority, object userData = null)
        {
            return AddWebRequest(webRequestUri, postData, null, priority, userData);
        }

        /// <summary>
        /// 增加 Web 请求任务
        /// </summary>
        /// <param name="webRequestUri">Web 请求地址</param>
        /// <param name="wwwForm">WWW 表单</param>
        /// <param name="priority">Web 请求任务的优先级</param>
        /// <param name="userData">用户自定义数据</param>
        /// <returns>新增 Web 请求任务的序列编号</returns>
        public int AddWebRequest(string webRequestUri, WWWForm wwwForm, int priority, object userData = null)
        {
            return AddWebRequest(webRequestUri, null, wwwForm, priority, userData);
        }

        /// <summary>
        /// 增加 Web 请求任务
        /// </summary>
        /// <param name="webRequestUri">Web 请求地址</param>
        /// <param name="postData">要发送的数据流</param>
        /// <param name="wwwForm">WWW 表单</param>
        /// <param name="priority">Web 请求任务的优先级</param>
        /// <param name="userData">用户自定义数据</param>
        /// <returns>新增 Web 请求任务的序列编号</returns>
        private int AddWebRequest(string webRequestUri, byte[] postData, WWWForm wwwForm, int priority, object userData)
        {
            return m_WebRequestManager.AddWebRequest(webRequestUri, postData, priority, new WWWFormInfo(wwwForm, userData));
        }

        /// <summary>
        /// 移除 Web 请求任务
        /// </summary>
        /// <param name="serialId">要移除 Web 请求任务的序列编号</param>
        /// <returns>是否移除 Web 请求任务成功</returns>
        public bool RemoveWebRequest(int serialId)
        {
            return m_WebRequestManager.RemoveWebRequest(serialId);
        }

        /// <summary>
        /// 移除所有 Web 请求任务
        /// </summary>
        public void RemoveAllWebRequests()
        {
            m_WebRequestManager.RemoveAllWebRequests();
        }

        /// <summary>
        /// 增加 Web 请求代理辅助器
        /// </summary>
        /// <param name="index">Web 请求代理辅助器索引</param>
        private void AddWebRequestAgentHelper(int index)
        {
            WebRequestAgentHelperBase webRequestAgentHelper = Helper.CreateHelper(m_WebRequestAgentHelperTypeName, m_CustomWebRequestAgentHelper, index);
            if (webRequestAgentHelper == null)
            {
                Log.Error("Can not create web request agent helper.");
                return;
            }

            webRequestAgentHelper.name = Utility.Text.Format("Web Request Agent Helper - {0}", index.ToString());
            Transform trans = webRequestAgentHelper.transform;
            trans.SetParent(m_InstanceRoot);
            trans.localScale = Vector3.one;

            m_WebRequestManager.AddWebRequestAgentHelper(webRequestAgentHelper);
        }

        //Web请求开始的回调
        private void OnWebRequestStart(object sender, GameFramework.WebRequest.WebRequestStartEventArgs e)
        {
            m_EventComponent.Fire(this, ReferencePool.Acquire<WebRequestStartEventArgs>().Fill(e));
        }

        //Web请求成功的回调
        private void OnWebRequestSuccess(object sender, GameFramework.WebRequest.WebRequestSuccessEventArgs e)
        {
            m_EventComponent.Fire(this, ReferencePool.Acquire<WebRequestSuccessEventArgs>().Fill(e));
        }

        //Web请求失败的回调
        private void OnWebRequestFailure(object sender, GameFramework.WebRequest.WebRequestFailureEventArgs e)
        {
            Log.Warning("[WebRequestComponent.OnWebRequestFailure] Web request failure, web request serial id '{0}', web request uri '{1}', error message '{2}'.", e.SerialId.ToString(), e.WebRequestUri, e.ErrorMessage);
            m_EventComponent.Fire(this, ReferencePool.Acquire<WebRequestFailureEventArgs>().Fill(e));
        }

    }
}
