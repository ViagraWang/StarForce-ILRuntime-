using GameFramework;
using GameFramework.Download;
using GameFramework.ObjectPool;
using GameFramework.Resource;
using System;
using UnityEngine;

namespace UnityGameFrame.Runtime
{
    /// <summary>
    /// 资源组件
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Game Framework/Resource")]
    public sealed class ResourceComponent : GameFrameworkComponent
    {
        private const int OneMegaBytes = 1024 * 1024;
        private EventComponent m_EventComponent = null; //事件组件
        private bool m_ForceUnloadUnusedAssets = false; //强制卸载没用资源
        private bool m_PreorderUnloadUnusedAssets = false;  //预卸载未被使用资源
        private bool m_PerformGCCollect = false; //垃圾回收标志位
        private AsyncOperation m_AsyncOperation = null; //卸载未使用资源的异步操作
        private float m_LastOperationElapse = 0f;   //记录上次卸载未使用资源操作时间
        private ResourceHelperBase m_ResourceHelper = null;    //资源辅助器
#if UNITY_EDITOR
        private bool m_IsEditorResourceMode = false;    //是否是编辑器资源模式的标志位，这里主要供Inspector编辑器使用
#endif
        [SerializeField]
        private ResourceMode m_ResourceMode = ResourceMode.Package; //资源模式

        [SerializeField]
        private ReadWritePathType m_ReadWritePathType = ReadWritePathType.Unspecified;  //读写资源路径类型

        [SerializeField]
        private float m_UnloadUnusedAssetsInterval = 60f;   //预卸载未使用资源的频率

        [SerializeField]
        private float m_AssetAutoReleaseInterval = 60f;   //内置资源对象池自动释放可释放对象的间隔秒数

        [SerializeField]
        private int m_AssetCapacity = 64;   //内置资源对象池容量

        [SerializeField]
        private float m_AssetExpireTime = 60f;   //内置资源对象池过期秒数

        [SerializeField]
        private int m_AssetPriority = 0;   //内置资源对象池优先级

        [SerializeField]
        private float m_ResourceAutoReleaseInterval = 60f;   //资源文件对象池自动释放可释放对象的间隔秒数

        [SerializeField]
        private int m_ResourceCapacity = 16;   //资源文件对象池容量

        [SerializeField]
        private float m_ResourceExpireTime = 60f;   //资源文件对象池过期秒数

        [SerializeField]
        private int m_ResourcePriority = 0;   //资源文件对象池优先级

        [SerializeField]
        private string m_UpdatePrefixUri = null;    //热更新Uri

        [SerializeField]
        private int m_UpdateFileCacheLength = OneMegaBytes;    //更新文件缓冲区长度，即写入文件刷新频率

        [SerializeField]
        private int m_GenerateReadWriteListLength = OneMegaBytes;  //每下载多少字节的资源，刷新一次资源列表

        [SerializeField]
        private int m_UpdateRetryCount = 3;  //更新尝试次数

        [SerializeField]
        private Transform m_InstanceRoot = null;    //资源组件根对象

        [SerializeField]
        private string m_ResourceHelperTypeName = "UnityGameFrame.Runtime.DefaultResourceHelper";   //默认资源辅助器

        [SerializeField]
        private ResourceHelperBase m_CustomResourceHelper = null;   //自定义资源辅助器

        [SerializeField]
        private string m_LoadResourceAgentHelperTypeName = "UnityGameFrame.Runtime.DefaultLoadResourceAgentHelper"; //默认加载资源代理辅助器

        [SerializeField]
        private LoadResourceAgentHelperBase m_CustomLoadResourceAgentHelper = null; //自定义加载资源代理辅助器

        [SerializeField]
        private int m_LoadResourceAgentHelperCount = 3; //加载资源代理辅助器数量

        /// <summary>
        /// 资源管理器
        /// </summary>
        public IResourceManager ResourceManager { get; private set; } = null;

        /// <summary>
        /// 获取资源只读路径
        /// </summary>
        public string ReadOnlyPath { get { return ResourceManager.ReadOnlyPath; } }

        /// <summary>
        /// 获取资源读写路径
        /// </summary>
        public string ReadWritePath { get { return ResourceManager.ReadWritePath; } }

        /// <summary>
        /// 获取资源模式
        /// </summary>
        public ResourceMode ResourceMode { get { return ResourceManager.ResourceMode; } }

        /// <summary>
        /// 获取资源读写路径类型
        /// </summary>
        public ReadWritePathType ReadWritePathType { get { return m_ReadWritePathType; } }

        /// <summary>
        /// 获取当前变体
        /// </summary>
        public string CurrentVariant { get { return ResourceManager.CurrentVariant; } }

        /// <summary>
        /// 获取或设置无用资源释放间隔时间
        /// </summary>
        public float UnloadUnusedAssetsInterval
        {
            get { return m_UnloadUnusedAssetsInterval; }
            set { m_UnloadUnusedAssetsInterval = value; }
        }

        /// <summary>
        /// 获取当前资源适用的游戏版本号
        /// </summary>
        public string ApplicableGameVersion { get { return ResourceManager.ApplicableGameVersion; } }

        /// <summary>
        /// 获取当前内部资源版本号
        /// </summary>
        public int InternalResourceVersion { get { return ResourceManager.InternalResourceVersion; } }

        /// <summary>
        /// 获取已准备完毕内置资源数量
        /// </summary>
        public int AssetCount { get { return ResourceManager.AssetCount; } }

        /// <summary>
        /// 获取已准备完毕资源文件数量
        /// </summary>
        public int ResourceCount { get { return ResourceManager.ResourceCount; } }

        /// <summary>
        /// 获取资源组数量。
        /// </summary>
        public int ResourceGroupCount { get { return ResourceManager.ResourceGroupCount; } }

        /// <summary>
        /// 获取或设置资源更新下载地址
        /// </summary>
        public string UpdatePrefixUri
        {
            get { return ResourceManager.UpdatePrefixUri; }
            set { ResourceManager.UpdatePrefixUri = m_UpdatePrefixUri = value; }
        }

        /// <summary>
        /// 获取或设置更新文件缓存大小
        /// </summary>
        public int UpdateFileCacheLength
        {
            get { return ResourceManager.UpdateFileCacheLength; }
            set { ResourceManager.UpdateFileCacheLength = m_UpdateFileCacheLength = value; }
        }

        /// <summary>
        /// 获取或设置每下载多少字节的资源，刷新一次资源列表。
        /// </summary>
        public int GenerateReadWriteListLength
        {
            get { return ResourceManager.GenerateReadWriteListLength; }
            set { ResourceManager.GenerateReadWriteListLength = m_GenerateReadWriteListLength = value; }
        }

        /// <summary>
        /// 获取或设置资源更新重试次数
        /// </summary>
        public int UpdateRetryCount
        {
            get { return ResourceManager.UpdateRetryCount; }
            set { ResourceManager.UpdateRetryCount = m_UpdateRetryCount = value; }
        }

        /// <summary>
        /// 获取正在更新的资源组。
        /// </summary>
        public IResourceGroup UpdatingResourceGroup { get { return ResourceManager.UpdatingResourceGroup; } }

        /// <summary>
        /// 获取等待更新资源数量
        /// </summary>
        public int UpdateWaitingCount { get { return ResourceManager.UpdateWaitingCount; } }

        /// <summary>
        /// 获取候选更新资源数量
        /// </summary>
        public int UpdateCandidateCount { get { return ResourceManager.UpdateCandidateCount; } }

        /// <summary>
        /// 获取正在更新资源数量
        /// </summary>
        public int UpdatingCount { get { return ResourceManager.UpdatingCount; } }

        /// <summary>
        /// 获取加载资源代理总数量
        /// </summary>
        public int LoadTotalAgentCount { get { return ResourceManager.LoadTotalAgentCount; } }

        /// <summary>
        /// 获取可用加载资源代理数量
        /// </summary>
        public int LoadFreeAgentCount { get { return ResourceManager.LoadFreeAgentCount; } }

        /// <summary>
        /// 获取工作中加载资源代理数量
        /// </summary>
        public int LoadWorkingAgentCount { get { return ResourceManager.LoadWorkingAgentCount; } }

        /// <summary>
        /// 获取等待加载资源任务数量
        /// </summary>
        public int LoadWaitingTaskCount { get { return ResourceManager.LoadWaitingTaskCount; } }

        /// <summary>
        /// 获取或设置资源对象池自动释放可释放对象的间隔秒数
        /// </summary>
        public float AssetAutoReleaseInterval
        {
            get { return ResourceManager.AssetAutoReleaseInterval; }
            set { ResourceManager.AssetAutoReleaseInterval = m_AssetAutoReleaseInterval = value; }
        }

        /// <summary>
        /// 获取或设置资源对象池的容量
        /// </summary>
        public int AssetCapacity
        {
            get { return ResourceManager.AssetCapacity; }
            set { ResourceManager.AssetCapacity = m_AssetCapacity = value; }
        }

        /// <summary>
        /// 获取或设置资源对象池对象过期秒数
        /// </summary>
        public float AssetExpireTime
        {
            get { return ResourceManager.AssetExpireTime; }
            set { ResourceManager.AssetExpireTime = m_AssetExpireTime = value; }
        }

        /// <summary>
        /// 获取或设置资源对象池的优先级
        /// </summary>
        public int AssetPriority
        {
            get { return ResourceManager.AssetPriority; }
            set { ResourceManager.AssetPriority = m_AssetPriority = value; }
        }

        /// <summary>
        /// 获取或设置资源对象池自动释放可释放对象的间隔秒数
        /// </summary>
        public float ResourceAutoReleaseInterval
        {
            get { return ResourceManager.ResourceAutoReleaseInterval; }
            set { ResourceManager.ResourceAutoReleaseInterval = m_ResourceAutoReleaseInterval = value; }
        }

        /// <summary>
        /// 获取或设置资源对象池的容量
        /// </summary>
        public int ResourceCapacity
        {
            get { return ResourceManager.ResourceCapacity; }
            set { ResourceManager.ResourceCapacity = m_ResourceCapacity = value; }
        }

        /// <summary>
        /// 获取或设置资源对象池对象过期秒数
        /// </summary>
        public float ResourceExpireTime
        {
            get { return ResourceManager.ResourceExpireTime; }
            set { ResourceManager.ResourceExpireTime = m_ResourceExpireTime = value; }
        }

        /// <summary>
        /// 获取或设置资源对象池的优先级
        /// </summary>
        public int ResourcePriority
        {
            get { return ResourceManager.ResourcePriority; }
            set { ResourceManager.ResourcePriority = m_ResourcePriority = value; }
        }

        private void Start()
        {
            //基础组件
            BaseComponent baseComponent = GameEntry.GetComponent<BaseComponent>();
            if(baseComponent == null)
            {
                Log.Fatal("[ResourceComponent.Start] Base component is invalid -> baseComponent == null.");
                return;
            }
            //事件组件
            m_EventComponent = GameEntry.GetComponent<EventComponent>();
            if(m_EventComponent == null)
            {
                Log.Fatal("[ResourceComponent.Start] Event component is invalid -> m_EventComponent == null.");
                return;
            }
            //资源管理器

            ResourceManager = baseComponent.ResourceManager;
            if (ResourceManager == null)
            {
                Log.Fatal("[ResourceComponent.Start] Resource manager is invalid -> m_ResourceManager == null.");
                return;
            }

            //绑定事件
            ResourceManager.ResourceUpdateStart += OnResourceUpdateStart; 
            ResourceManager.ResourceUpdateChanged += OnResourceUpdateChanged; 
            ResourceManager.ResourceUpdateSuccess += OnResourceUpdateSuccess; 
            ResourceManager.ResourceUpdateFailure += OnResourceUpdateFailure;

            ResourceManager.SetReadOnlyPath(Application.streamingAssetsPath); //设置只读路径
            if (m_ReadWritePathType == ReadWritePathType.TemporaryCache)
                ResourceManager.SetReadWritePath(Application.temporaryCachePath);
            else
            {
                if (m_ReadWritePathType == ReadWritePathType.Unspecified)
                    m_ReadWritePathType = ReadWritePathType.PersistentData;
                ResourceManager.SetReadWritePath(Application.persistentDataPath);
            }

#if UNITY_EDITOR
            m_IsEditorResourceMode = baseComponent.IsEditorResourceMode;
            if (m_IsEditorResourceMode) //编辑器模式直接返回即可
                return;
#endif

            SetResourceMode(m_ResourceMode);
            ResourceManager.SetDownloadManager(GameFrameworkEntry.GetModule<IDownloadManager>());    //设置下载管理器
            ResourceManager.SetObjectPoolManager(GameFrameworkEntry.GetModule<IObjectPoolManager>());    //设置对象池管理器
            ResourceManager.AssetAutoReleaseInterval = m_AssetAutoReleaseInterval;
            ResourceManager.AssetCapacity = m_AssetCapacity;
            ResourceManager.AssetExpireTime = m_AssetExpireTime;
            ResourceManager.AssetPriority = m_AssetPriority;
            ResourceManager.ResourceAutoReleaseInterval = m_ResourceAutoReleaseInterval;
            ResourceManager.ResourceCapacity = m_ResourceCapacity;
            ResourceManager.ResourceExpireTime = m_ResourceExpireTime;
            ResourceManager.ResourcePriority = m_ResourcePriority;

            if(m_ResourceMode == ResourceMode.Updatable)
            {
                ResourceManager.UpdatePrefixUri = m_UpdatePrefixUri;
                ResourceManager.UpdateFileCacheLength = m_UpdateFileCacheLength;
                ResourceManager.GenerateReadWriteListLength = m_GenerateReadWriteListLength;
                ResourceManager.UpdateRetryCount = m_UpdateRetryCount;
            }

            //资源辅助器
            m_ResourceHelper = Helper.CreateHelper(m_ResourceHelperTypeName, m_CustomResourceHelper);
            if(m_ResourceHelper == null)
            {
                Log.Error("[ResourceComponent.Start] Can not create resource helper -> m_ResourceHelper == null.");
                return;
            }
            m_ResourceHelper.name = "Resource Helper";
            Transform trans = m_ResourceHelper.transform;
            trans.SetParent(transform);
            transform.localScale = Vector3.one;
            ResourceManager.SetResourceHelper(m_ResourceHelper);

            if(m_InstanceRoot == null)
            {
                m_InstanceRoot = new GameObject("Load Resource Agent Instances").transform;
                m_InstanceRoot.SetParent(transform);
                m_InstanceRoot.localScale = Vector3.one;
            }

            for (int i = 0; i < m_LoadResourceAgentHelperCount; i++)
            {
                AddLoadResourceAgentHelper(i);
            }
        }

        public void Update()
        {
            //卸载未被使用资源
            m_LastOperationElapse += Time.unscaledDeltaTime;
            if(m_AsyncOperation == null && (m_ForceUnloadUnusedAssets || m_PreorderUnloadUnusedAssets && m_LastOperationElapse >= m_UnloadUnusedAssetsInterval))
            {
                Log.Info("Unload unused assets...");
                m_ForceUnloadUnusedAssets = false;
                m_PreorderUnloadUnusedAssets = false;
                m_LastOperationElapse = 0f;
                m_AsyncOperation = Resources.UnloadUnusedAssets();
            }

            //检测卸载完成时，进行GC回收
            if(m_AsyncOperation != null && m_AsyncOperation.isDone)
            {
                m_AsyncOperation = null;
                if (m_PerformGCCollect)
                {
                    Log.Info("GC.Collect...");
                    GC.Collect();
                    m_PerformGCCollect = false;
                }
            }
        }

        /// <summary>
        /// 设置资源模式
        /// </summary>
        /// <param name="resourceMode">资源模式</param>
        public void SetResourceMode(ResourceMode resourceMode)
        {
            ResourceManager.SetResourceMode(resourceMode);
        }

        /// <summary>
        /// 设置当前变体
        /// </summary>
        /// <param name="currentVariant">当前变体</param>
        public void SetCurrentVariant(string currentVariant)
        {
            ResourceManager.SetCurrentVariant(!string.IsNullOrEmpty(currentVariant) ? currentVariant : null);
        }

        /// <summary>
        /// 设置解密资源回调函数
        /// </summary>
        /// <param name="decryptResourceCallback">要设置的解密资源回调函数</param>
        /// <remarks>如果不设置，将使用默认的解密资源回调函数</remarks>
        public void SetDecryptResourceCallback(DecryptResourceCallback decryptResourceCallback)
        {
            ResourceManager.SetDecryptResourceCallback(decryptResourceCallback);
        }

        /// <summary>
        /// 预定执行释放未被使用的资源
        /// </summary>
        /// <param name="performGCCollect">是否使用垃圾回收</param>
        public void PreUnloadUnusedAssets(bool performGCCollect)
        {
            m_PreorderUnloadUnusedAssets = true;
            if (performGCCollect)
                m_PerformGCCollect = performGCCollect;
        }

        /// <summary>
        /// 强制执行释放未被使用的资源
        /// </summary>
        /// <param name="performGCCollect">是否使用垃圾回收</param>
        public void ForceUnloadUnusedAssets(bool performGCCollect)
        {
            m_ForceUnloadUnusedAssets = true;
            if (performGCCollect)
                m_PerformGCCollect = performGCCollect;
        }

        /// <summary>
        /// 使用单机模式并初始化资源
        /// </summary>
        /// <param name="initResourcesCompleteCallback">使用单机模式并初始化资源完成时的回调函数</param>
        public void InitResources(InitResourcesCompleteCallback initResourcesCompleteCallback)
        {
            ResourceManager.InitResources(initResourcesCompleteCallback);
        }

        /// <summary>
        /// 使用可更新模式并检查版本资源列表
        /// </summary>
        /// <param name="latestInternalResourceVersion">最新的内部资源版本号</param>
        /// <returns>检查版本资源列表结果</returns>
        public CheckVersionListResult CheckVersionList(int latestInternalResourceVersion)
        {
            return ResourceManager.CheckVersionList(latestInternalResourceVersion);
        }

        /// <summary>
        /// 使用可更新模式并更新版本资源列表
        /// </summary>
        /// <param name="versionListLength">版本资源列表大小</param>
        /// <param name="versionListHashCode">版本资源列表哈希值</param>
        /// <param name="versionListZipLength">版本资源列表压缩后大小</param>
        /// <param name="versionListZipHashCode">版本资源列表压缩后哈希值</param>
        /// <param name="updateVersionListCallbacks">版本资源列表更新回调函数集</param>
        public void UpdateVersionList(int versionListLength, int versionListHashCode, int versionListZipLength, int versionListZipHashCode, UpdateVersionListCallbacks updateVersionListCallbacks)
        {
            ResourceManager.UpdateVersionList(versionListLength, versionListHashCode, versionListZipLength, versionListZipHashCode, updateVersionListCallbacks);
        }

        /// <summary>
        /// 使用可更新模式并检查资源
        /// </summary>
        /// <param name="checkResourcesCompleteCallback">使用可更新模式并检查资源完成时的回调函数</param>
        public void CheckResources(CheckResourcesCompleteCallback checkResourcesCompleteCallback)
        {
            ResourceManager.CheckResources(checkResourcesCompleteCallback);
        }

        /// <summary>
        /// 使用可更新模式并更新全部资源
        /// </summary>
        /// <param name="updateResourcesCompleteCallback">使用可更新模式并更新默认资源组完成时的回调函数</param>
        public void UpdateResources(UpdateResourcesCompleteCallback updateResourcesCompleteCallback)
        {
            ResourceManager.UpdateResources(updateResourcesCompleteCallback);
        }

        /// <summary>
        /// 使用可更新模式并更新指定资源组的资源
        /// </summary>
        /// <param name="resourceGroupName">要更新的资源组名称</param>
        /// <param name="updateResourcesCompleteCallback">使用可更新模式并更新指定资源组完成时的回调函数</param>
        public void UpdateResources(string resourceGroupName, UpdateResourcesCompleteCallback updateResourcesCompleteCallback)
        {
            ResourceManager.UpdateResources(resourceGroupName, updateResourcesCompleteCallback);
        }

        /// <summary>
        /// 检查资源是否存在
        /// </summary>
        /// <param name="assetName">要检查资源的名称</param>
        /// <returns>资源是否存在</returns>
        public bool HasAsset(string assetName)
        {
            return ResourceManager.HasAsset(assetName);
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <param name="assetName">要加载资源的名称</param>
        /// <param name="loadAssetCallbacks">加载资源回调函数集</param>
        /// <param name="userData">用户自定义数据</param>
        public void LoadAsset(string assetName, LoadAssetCallbacks loadAssetCallbacks, object userData = null)
        {
            LoadAsset(assetName, null, 0, loadAssetCallbacks, userData);
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <param name="assetName">要加载资源的名称</param>
        /// <param name="assetType">要加载资源的类型</param>
        /// <param name="loadAssetCallbacks">加载资源回调函数集</param>
        /// <param name="userData">用户自定义数据</param>
        public void LoadAsset(string assetName, Type assetType, LoadAssetCallbacks loadAssetCallbacks, object userData = null)
        {
            LoadAsset(assetName, assetType, 0, loadAssetCallbacks, userData);
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <param name="assetName">要加载资源的名称</param>
        /// <param name="priority">加载资源的优先级</param>
        /// <param name="loadAssetCallbacks">加载资源回调函数集</param>
        /// <param name="userData">用户自定义数据</param>
        public void LoadAsset(string assetName, int priority, LoadAssetCallbacks loadAssetCallbacks, object userData = null)
        {
            LoadAsset(assetName, null, priority, loadAssetCallbacks, userData);
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <param name="assetName">要加载资源的名称</param>
        /// <param name="assetType">要加载资源的类型</param>
        /// <param name="priority">加载资源的优先级</param>
        /// <param name="loadAssetCallbacks">加载资源回调函数集</param>
        /// <param name="userData">用户自定义数据</param>
        public void LoadAsset(string assetName, Type assetType, int priority, LoadAssetCallbacks loadAssetCallbacks, object userData)
        {
            if(string.IsNullOrEmpty(assetName))
            {
                Log.Error("[ResourceComponent.LoadAsset] Asset name is invalid.");
                return;
            }

            if(!assetName.StartsWith("Assets/"))
            {
                Log.Error("[ResourceComponent.LoadAsset] Asset name '{0}' is invalid.", assetName);
                return;
            }

            ResourceManager.LoadAsset(assetName, assetType, priority, loadAssetCallbacks, userData);
        }

        /// <summary>
        /// 卸载资源
        /// </summary>
        /// <param name="asset">要卸载的资源</param>
        public void UnloadAsset(object asset)
        {
            ResourceManager.UnloadAsset(asset);
        }

        /// <summary>
        /// 检查资源组是否存在
        /// </summary>
        /// <param name="resourceGroupName">要检查资源组的名称</param>
        /// <returns>资源组是否存在</returns>
        public bool HasResourceGroup(string resourceGroupName)
        {
            return ResourceManager.HasResourceGroup(resourceGroupName);
        }

        /// <summary>
        /// 获取资源组
        /// </summary>
        /// <param name="resourceGroupName">要获取的资源组名称</param>
        /// <returns>要获取的资源组</returns>
        public IResourceGroup GetResourceGroup(string resourceGroupName)
        {
            return ResourceManager.GetResourceGroup(resourceGroupName);
        }

        /// <summary>
        /// 增加加载资源代理辅助器
        /// </summary>
        /// <param name="index">加载资源代理辅助器索引</param>
        private void AddLoadResourceAgentHelper(int index)
        {
            LoadResourceAgentHelperBase loadResourceAgentHelper = Helper.CreateHelper(m_LoadResourceAgentHelperTypeName, m_CustomLoadResourceAgentHelper, index);
            if(loadResourceAgentHelper == null)
            {
                Log.Error("[ResourceComponent.AddLoadResourceAgentHelper] Can not create load resource agent helper.");
                return;
            }

            loadResourceAgentHelper.name = Utility.Text.Format("Load Resource Agent Helper - {0}", index);
            Transform trans = loadResourceAgentHelper.transform;
            trans.SetParent(m_InstanceRoot);
            trans.localScale = Vector3.one;

            ResourceManager.AddLoadResourceAgentHelper(loadResourceAgentHelper);
        }

        //资源更新开始事件
        private void OnResourceUpdateStart(object sender, GameFramework.Resource.ResourceUpdateStartEventArgs e)
        {
            m_EventComponent.Fire(this, ReferencePool.Acquire<ResourceUpdateStartEventArgs>().Fill(e));
        }

        //资源更新改变事件
        private void OnResourceUpdateChanged(object sender, GameFramework.Resource.ResourceUpdateChangedEventArgs e)
        {
            m_EventComponent.Fire(this, ReferencePool.Acquire<ResourceUpdateChangedEventArgs>().Fill(e));
        }
        
        //资源更新成功事件
        private void OnResourceUpdateSuccess(object sender, GameFramework.Resource.ResourceUpdateSuccessEventArgs e)
        {
            m_EventComponent.Fire(this, ReferencePool.Acquire<ResourceUpdateSuccessEventArgs>().Fill(e));
        }

        //资源更新失败事件
        private void OnResourceUpdateFailure(object sender, GameFramework.Resource.ResourceUpdateFailureEventArgs e)
        {
            m_EventComponent.Fire(this, ReferencePool.Acquire<ResourceUpdateFailureEventArgs>().Fill(e));
        }

    }
}
