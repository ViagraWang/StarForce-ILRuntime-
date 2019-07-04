
using GameFramework;
using GameFramework.Download;
using UnityEngine;

namespace UnityGameFrame.Runtime
{
    /// <summary>
    /// 下载组件
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Game Framework/Download")]
    public sealed class DownloadComponent : GameFrameworkComponent
    {
        private IDownloadManager m_DownloadManager = null;  //下载管理器
        private EventComponent m_EventComponent = null; //事件组件

        [SerializeField]
        private Transform m_InstanceRoot = null;    //下载器根对象

        [SerializeField]
        private string m_DownloadAgentHelperTypeName = "UnityGameFrame.Runtime.UnityWebRequestDownloadAgentHelper";

        [SerializeField]
        private DownloadAgentHelperBase m_CustomDownloadAgentHelper = null; //用户自定义的下载代理辅助器

        [SerializeField]
        private int m_DownloadAgentHelperCount = 3; //下载代理辅助器数量

        [SerializeField]
        private float m_Timeout = 30f;  //超时时长

        [SerializeField]
        private int m_FlushSize = 1024 * 1024;  //获取或设置将缓冲区写入磁盘的临界大小，仅当开启断点续传时有效

        /// <summary>
        /// 获取下载代理总数量
        /// </summary>
        public int TotalAgentCount { get { return m_DownloadManager.TotalAgentCount; } }

        /// <summary>
        /// 获取可用下载代理数量
        /// </summary>
        public int FreeAgentCount { get { return m_DownloadManager.FreeAgentCount; } }

        /// <summary>
        /// 获取工作中下载代理数量
        /// </summary>
        public int WorkingAgentCount { get { return m_DownloadManager.WorkingAgentCount; } }

        /// <summary>
        /// 获取等待下载任务数量
        /// </summary>
        public int WaitingTaskCount { get { return m_DownloadManager.WaitingTaskCount; } }

        /// <summary>
        /// 获取或设置下载超时时长，以秒为单位
        /// </summary>
        public float Timeout
        {
            get { return m_DownloadManager.Timeout; }
            set { m_DownloadManager.Timeout = m_Timeout = value; }
        }

        /// <summary>
        /// 获取或设置将缓冲区写入磁盘的临界大小，仅当开启断点续传时有效
        /// </summary>
        public int FlushSize
        {
            get { return m_DownloadManager.FlushSize; }
            set { m_DownloadManager.FlushSize = m_FlushSize = value; }
        }

        /// <summary>
        /// 获取当前下载速度
        /// </summary>
        public float CurrentSpeed { get { return m_DownloadManager.CurrentSpeed; } }

        protected override void Awake()
        {
            base.Awake();
            //获取下载管理器模块
            m_DownloadManager = GameFrameworkEntry.GetModule<IDownloadManager>();
            if (m_DownloadManager == null)
            {
                Log.Fatal("[DownloadComponent.Awake] Download manager is invalid.");
                return;
            }

            //初始化配置下载管理器
            m_DownloadManager.DownloadStart += OnDownloadStart;
            m_DownloadManager.DownloadUpdate += OnDownloadUpdate;
            m_DownloadManager.DownloadSuccess += OnDownloadSuccess;
            m_DownloadManager.DownloadFailure += OnDownloadFailure;
            m_DownloadManager.FlushSize = m_FlushSize;
            m_DownloadManager.Timeout = m_Timeout;
        }

        private void Start()
        {
            //事件组件
            m_EventComponent = GameEntry.GetComponent<EventComponent>();
            if (m_EventComponent == null)
            {
                Log.Fatal("[DownloadComponent.Start] Event component is invalid.");
                return;
            }

            //辅助器根对象
            if(m_InstanceRoot == null)
            {
                m_InstanceRoot = new GameObject("Download Agent Instances").transform;
                m_InstanceRoot.SetParent(transform);
                m_InstanceRoot.localScale = Vector3.one;
            }
            
            //添加配置数量的下载代理辅助器
            for (int i = 0; i < m_DownloadAgentHelperCount; i++)
            {
                AddDownloadAgentHelper(i);
            }
        }

        /// <summary>
        /// 增加下载任务
        /// </summary>
        /// <param name="downloadPath">下载后存放路径</param>
        /// <param name="downloadUri">原始下载地址</param>
        /// <param name="userData">用户自定义数据</param>
        /// <returns>新增下载任务的序列编号</returns>
        public int AddDownload(string downloadPath, string downloadUri, object userData = null)
        {
            return m_DownloadManager.AddDownload(downloadPath, downloadUri, userData);
        }

        /// <summary>
        /// 增加下载任务
        /// </summary>
        /// <param name="downloadPath">下载后存放路径</param>
        /// <param name="downloadUri">原始下载地址</param>
        /// <param name="priority">下载任务的优先级</param>
        /// <param name="userData">用户自定义数据</param>
        /// <returns>新增下载任务的序列编号</returns>
        public int AddDownload(string downloadPath, string downloadUri, int priority, object userData = null)
        {
            return AddDownload(downloadPath, downloadUri, priority, userData);
        }

        /// <summary>
        /// 移除下载任务
        /// </summary>
        /// <param name="serialId">要移除下载任务的序列编号</param>
        public void RemoveDownload(int serialId)
        {
            m_DownloadManager.RemoveDownload(serialId);
        }

        /// <summary>
        /// 移除所有下载任务
        /// </summary>
        public void RemoveAllDownload()
        {
            m_DownloadManager.RemoveAllDownload();
        }

        //添加下载代理辅助器
        private void AddDownloadAgentHelper(int index)
        {
            //实例化代理辅助器
            DownloadAgentHelperBase downloadAgentHelper = Helper.CreateHelper(m_DownloadAgentHelperTypeName, m_CustomDownloadAgentHelper, index);
            if (downloadAgentHelper == null)
            {
                Log.Error("[DownloadComponent.AddDownloadAgentHelper] Can not create download agent helper.");
                return;
            }
            //辅助器操作
            downloadAgentHelper.name = Utility.Text.Format("Download Agent Helper - {0}", index);
            Transform transform = downloadAgentHelper.transform;
            transform.SetParent(m_InstanceRoot);
            transform.localScale = Vector3.one;

            m_DownloadManager.AddDownloadAgentHelper(downloadAgentHelper);  //添加带下载管理器
        }

        //下载开始回调
        private void OnDownloadStart(object sender, GameFramework.Download.DownloadStartEventArgs e)
        {
            m_EventComponent.Fire(this, ReferencePool.Acquire<DownloadStartEventArgs>().Fill(e));
        }

        //下载更新回调
        private void OnDownloadUpdate(object sender, GameFramework.Download.DownloadUpdateEventArgs e)
        {
            m_EventComponent.Fire(this, ReferencePool.Acquire<DownloadUpdateEventArgs>().Fill(e));
        }

        //下载成功回调
        private void OnDownloadSuccess(object sender, GameFramework.Download.DownloadSuccessEventArgs e)
        {
            m_EventComponent.Fire(this, ReferencePool.Acquire<DownloadSuccessEventArgs>().Fill(e));
        }

        //下载失败回调
        private void OnDownloadFailure(object sender, GameFramework.Download.DownloadFailureEventArgs e)
        {
            Log.Warning("Download failure, download serial id '{0}', download path '{1}', download uri '{2}', error message '{3}'.", e.SerialId.ToString(), e.DownloadPath, e.DownloadUri, e.ErrorMessage);
            m_EventComponent.Fire(this, ReferencePool.Acquire<DownloadFailureEventArgs>().Fill(e));
        }
    }
}
