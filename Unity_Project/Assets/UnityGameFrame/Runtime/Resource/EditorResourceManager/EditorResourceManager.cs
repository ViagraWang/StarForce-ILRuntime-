using GameFramework;
using GameFramework.Download;
using GameFramework.ObjectPool;
using GameFramework.Resource;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityGameFrame.Runtime
{
    /// <summary>
    /// 编辑器资源管理器
    /// </summary>
    [DisallowMultipleComponent]
    public sealed partial class EditorResourceManager : MonoBehaviour, IResourceManager
    {
        private readonly int AssetsSubstringLength = "Assets".Length;

        [SerializeField]
        private int m_LoadAssetCountPerFrame = 1;   //每帧加载的资源数量

        [SerializeField]
        private float m_MinLoadAssetRandomDelaySeconds = 0f;

        [SerializeField]
        private float m_MaxLoadAssetRandomDelaySeconds = 0f;

        private string m_ReadOnlyPath = null;
        private string m_ReadWritePath = null;
        private List<LoadAssetInfo> m_LoadAssetInfos = null;    //加载资源信息的列表
        private List<LoadSceneInfo> m_LoadSceneInfos = null;    //加载场景信息的列表
        private List<UnloadSceneInfo> m_UnloadSceneInfos = null;    //卸载场景信息的列表

        /// <summary>
        /// 获取资源只读取路径
        /// </summary>
        public string ReadOnlyPath { get { return m_ReadOnlyPath; } }

        /// <summary>
        /// 获取资源读写区路径
        /// </summary>
        public string ReadWritePath { get { return m_ReadWritePath; } }

        /// <summary>
        /// 获取资源模式
        /// </summary>
        public ResourceMode ResourceMode { get { return ResourceMode.Unspecified; } }

        /// <summary>
        /// 获取当前变体
        /// </summary>
        public string CurrentVariant { get { return null; } }

        /// <summary>
        /// 获取当前资源使用的游戏版本号
        /// </summary>
        public string ApplicableGameVersion { get { throw new NotSupportedException("ApplicableGameVersion"); } }

        /// <summary>
        /// 获取当前内部资源版本号
        /// </summary>
        public int InternalResourceVersion { get { throw new NotSupportedException("InternalResourceVersion"); } }

        /// <summary>
        /// 获取已准备完毕资源数量
        /// </summary>
        public int AssetCount { get { throw new NotSupportedException("AssetCount"); } }

        /// <summary>
        /// 获取已准备完毕资源数量
        /// </summary>
        public int ResourceCount { get { throw new NotSupportedException("ResourceCount"); } }

        /// <summary>
        /// 获取资源组数量
        /// </summary>
        public int ResourceGroupCount { get { throw new NotSupportedException("ResourceGroupCount"); } }

        /// <summary>
        /// 获取或设置资源更新下载地址
        /// </summary>
        public string UpdatePrefixUri
        {
            get { throw new NotSupportedException("UpdatePrefixUri"); }
            set { throw new NotSupportedException("UpdatePrefixUri"); }
        }

        /// <summary>
        /// 获取或设置资源更新重试次数
        /// </summary>
        public int UpdateRetryCount
        {
            get { throw new NotSupportedException("UpdateRetryCount"); }
            set { throw new NotSupportedException("UpdateRetryCount"); }
        }


        /// <summary>
        /// 获取或设置更新文件缓存大小。
        /// </summary>
        public int UpdateFileCacheLength
        {
            get { throw new NotSupportedException("UpdateFileCacheLength"); }
            set { throw new NotSupportedException("UpdateFileCacheLength"); }
        }

        /// <summary>
        /// 获取或设置每下载多少字节的资源，刷新一次资源列表。
        /// </summary>
        public int GenerateReadWriteListLength
        {
            get { throw new NotSupportedException("GenerateReadWriteListLength"); }
            set { throw new NotSupportedException("GenerateReadWriteListLength"); }
        }

        /// <summary>
        /// 获取等待更新资源数量
        /// </summary>
        public int UpdateWaitingCount { get { throw new NotSupportedException("UpdateWaitingCount"); } }

        /// <summary>
        /// 获取候选更新资源数量
        /// </summary>
        public int UpdateCandidateCount { get { throw new NotSupportedException("UpdateCandidateCount"); } }

        /// <summary>
        /// 获取正在更新资源数量
        /// </summary>
        public int UpdatingCount { get { throw new NotSupportedException("UpdatingCount"); } }

        /// <summary>
        /// 获取加载资源代理总数量
        /// </summary>
        public int LoadTotalAgentCount { get { throw new NotSupportedException("LoadTotalAgentCount"); } }

        /// <summary>
        /// 获取可用加载资源代理数量
        /// </summary>
        public int LoadFreeAgentCount { get { throw new NotSupportedException("LoadFreeAgentCount"); } }

        /// <summary>
        /// 获取工作中加载资源代理数量
        /// </summary>
        public int LoadWorkingAgentCount { get { throw new NotSupportedException("LoadWorkingAgentCount"); } }

        /// <summary>
        /// 获取等待加载资源任务数量
        /// </summary>
        public int LoadWaitingTaskCount { get { throw new NotSupportedException("LoadWaitingTaskCount"); } }

        /// <summary>
        /// 获取或设置资源对象池自动释放可释放对象的间隔秒数
        /// </summary>
        public float AssetAutoReleaseInterval
        {
            get { throw new NotSupportedException("AssetAutoReleaseInterval"); }
            set { throw new NotSupportedException("AssetAutoReleaseInterval"); }
        }

        /// <summary>
        /// 获取或设置资源对象池的容量
        /// </summary>
        public int AssetCapacity
        {
            get { throw new NotSupportedException("AssetCapacity"); }
            set { throw new NotSupportedException("AssetCapacity"); }
        }

        /// <summary>
        /// 获取或设置资源对象池对象过期秒数
        /// </summary>
        public float AssetExpireTime
        {
            get { throw new NotSupportedException("AssetExpireTime"); }
            set { throw new NotSupportedException("AssetExpireTime"); }
        }

        /// <summary>
        /// 获取或设置资源对象池的优先级
        /// </summary>
        public int AssetPriority
        {
            get { throw new NotSupportedException("AssetPriority"); }
            set { throw new NotSupportedException("AssetPriority"); }
        }

        /// <summary>
        /// 获取或设置资源对象池自动释放可释放对象的间隔秒数
        /// </summary>
        public float ResourceAutoReleaseInterval
        {
            get { throw new NotSupportedException("ResourceAutoReleaseInterval"); }
            set { throw new NotSupportedException("ResourceAutoReleaseInterval"); }
        }

        /// <summary>
        /// 获取或设置资源对象池的容量
        /// </summary>
        public int ResourceCapacity
        {
            get { throw new NotSupportedException("ResourceCapacity"); }
            set { throw new NotSupportedException("ResourceCapacity"); }
        }

        /// <summary>
        /// 获取或设置资源对象池对象过期秒数
        /// </summary>
        public float ResourceExpireTime
        {
            get { throw new NotSupportedException("ResourceExpireTime"); }
            set { throw new NotSupportedException("ResourceExpireTime"); }
        }

        /// <summary>
        /// 获取或设置资源对象池的优先级
        /// </summary>
        public int ResourcePriority
        {
            get { throw new NotSupportedException("ResourcePriority"); }
            set { throw new NotSupportedException("ResourcePriority"); }
        }

        /// <summary>
        /// 获取正在更新的资源组
        /// </summary>
        public IResourceGroup UpdatingResourceGroup
        {
            get { throw new NotSupportedException("UpdatingResourceGroup"); }
        }

        /// <summary>
        /// 获取等待编辑器加载的资源数量
        /// </summary>
        public int LoadWaitingAssetCount { get { return m_LoadAssetInfos.Count; } }

        public int UpdateFailureCount => throw new NotImplementedException();

#pragma warning disable 0067, 0414

        /// <summary>
        /// 资源更新开始事件
        /// </summary>
        public event EventHandler<GameFramework.Resource.ResourceUpdateStartEventArgs> ResourceUpdateStart = null;

        /// <summary>
        /// 资源更新改变事件
        /// </summary>
        public event EventHandler<GameFramework.Resource.ResourceUpdateChangedEventArgs> ResourceUpdateChanged = null;

        /// <summary>
        /// 资源更新成功事件
        /// </summary>
        public event EventHandler<GameFramework.Resource.ResourceUpdateSuccessEventArgs> ResourceUpdateSuccess = null;

        /// <summary>
        /// 资源更新失败事件
        /// </summary>
        public event EventHandler<GameFramework.Resource.ResourceUpdateFailureEventArgs> ResourceUpdateFailure = null;

#pragma warning restore 0067, 0414

        //初始化
        private void Awake()
        {
            m_ReadOnlyPath = null;
            m_ReadWritePath = null;
            m_LoadAssetInfos = new List<LoadAssetInfo>();
            m_LoadSceneInfos = new List<LoadSceneInfo>();
            m_UnloadSceneInfos = new List<UnloadSceneInfo>();
        }

        void Start()
        {
            //基础组件
            BaseComponent baseComponent = GameEntry.GetComponent<BaseComponent>();
            if (baseComponent == null)
            {
                Log.Fatal("[EditorResourceManager.Start] Base component is invalid -> baseComponent == null.");
                return;
            }

            m_MinLoadAssetRandomDelaySeconds = baseComponent.MinLoadAssetRandomDelaySeconds;
            m_MaxLoadAssetRandomDelaySeconds = baseComponent.MaxLoadAssetRandomDelaySeconds;
        }

        int index = 0;
        private void Update()
        {
            UpdateLoadAssetInfos();
            UpdateLoadSceneInfos();
            UpdateUnloadSceneInfos();
        }

        //更新加载资源
        private void UpdateLoadAssetInfos()
        {
            if (m_LoadAssetInfos.Count > 0)
            {
                int count = 0;
                for (index = 0; index < m_LoadAssetInfos.Count && count < m_LoadAssetCountPerFrame;)
                {
                    LoadAssetInfo loadAssetInfo = m_LoadAssetInfos[index];
                    float elapseSeconds = (float)(DateTime.Now - loadAssetInfo.StartTime).TotalSeconds;
                    if (elapseSeconds >= loadAssetInfo.DelaySeconds)
                    {
                        UnityEngine.Object asset = null;
#if UNITY_EDITOR
                        if (loadAssetInfo.AssetType != null)
                        {
                            asset = UnityEditor.AssetDatabase.LoadAssetAtPath(loadAssetInfo.AssetName, loadAssetInfo.AssetType);
                        }
                        else
                        {
                            asset = UnityEditor.AssetDatabase.LoadMainAssetAtPath(loadAssetInfo.AssetName);
                        }
#endif

                        if (asset != null)
                        {
                            if (loadAssetInfo.LoadAssetCallbacks.LoadAssetSuccessCallback != null)
                                loadAssetInfo.LoadAssetCallbacks.LoadAssetSuccessCallback.Invoke(loadAssetInfo.AssetName, asset, elapseSeconds, loadAssetInfo.UserData);
                        }
                        else
                        {
                            if (loadAssetInfo.LoadAssetCallbacks.LoadAssetFailureCallback != null)
                                loadAssetInfo.LoadAssetCallbacks.LoadAssetFailureCallback.Invoke(loadAssetInfo.AssetName, LoadResourceStatus.NotExist, "Can not load this asset from asset database.", loadAssetInfo.UserData);
                        }

                        m_LoadAssetInfos.RemoveAt(index);   //移除
                        count++;    //加载的数量+1
                    }
                    else
                    {
                        index++;    //进行下一次循环
                        if (loadAssetInfo.LoadAssetCallbacks.LoadAssetUpdateCallback != null)
                            loadAssetInfo.LoadAssetCallbacks.LoadAssetUpdateCallback.Invoke(loadAssetInfo.AssetName, elapseSeconds / loadAssetInfo.DelaySeconds, loadAssetInfo.UserData);
                    }
                }
            }
        }

        //更新加载场景
        private void UpdateLoadSceneInfos()
        {
            if (m_LoadSceneInfos.Count > 0)
            {
                for (index = 0; index < m_LoadSceneInfos.Count;)
                {
                    LoadSceneInfo loadSceneInfo = m_LoadSceneInfos[index];
                    if (loadSceneInfo.AsyncOperation.isDone)    //判断是否异步完成
                    {
                        if (loadSceneInfo.AsyncOperation.allowSceneActivation)
                        {
                            if (loadSceneInfo.LoadSceneCallbacks.LoadSceneSuccessCallback != null)
                                loadSceneInfo.LoadSceneCallbacks.LoadSceneSuccessCallback.Invoke(loadSceneInfo.SceneAssetName, (float)(DateTime.Now - loadSceneInfo.StartTime).TotalSeconds, loadSceneInfo.UserData);
                        }
                        else
                        {
                            if (loadSceneInfo.LoadSceneCallbacks.LoadSceneFailureCallback != null)
                                loadSceneInfo.LoadSceneCallbacks.LoadSceneFailureCallback.Invoke(loadSceneInfo.SceneAssetName, LoadResourceStatus.NotExist, "Can not load this scene from asset database.", loadSceneInfo.UserData);
                        }

                        m_LoadSceneInfos.RemoveAt(index);   //移除
                    }
                    else
                    {
                        index++;
                        if (loadSceneInfo.LoadSceneCallbacks.LoadSceneUpdateCallback != null)
                            loadSceneInfo.LoadSceneCallbacks.LoadSceneUpdateCallback.Invoke(loadSceneInfo.SceneAssetName, loadSceneInfo.AsyncOperation.progress, loadSceneInfo.UserData);
                    }
                }
            }
        }

        //更新卸载场景
        private void UpdateUnloadSceneInfos()
        {
            if (m_UnloadSceneInfos.Count > 0)
            {
                for (index = 0; index < m_UnloadSceneInfos.Count;)
                {
                    UnloadSceneInfo unloadSceneInfo = m_UnloadSceneInfos[index];
                    if (unloadSceneInfo.AsyncOperation.isDone)    //判断是否异步完成
                    {
                        if (unloadSceneInfo.AsyncOperation.allowSceneActivation)
                        {
                            if (unloadSceneInfo.UnloadSceneCallbacks.UnloadSceneSuccessCallback != null)
                                unloadSceneInfo.UnloadSceneCallbacks.UnloadSceneSuccessCallback.Invoke(unloadSceneInfo.SceneAssetName, unloadSceneInfo.UserData);
                        }
                        else
                        {
                            if (unloadSceneInfo.UnloadSceneCallbacks.UnloadSceneFailureCallback != null)
                                unloadSceneInfo.UnloadSceneCallbacks.UnloadSceneFailureCallback.Invoke(unloadSceneInfo.SceneAssetName, unloadSceneInfo.UserData);
                        }

                        m_UnloadSceneInfos.RemoveAt(index);   //移除
                    }
                    else
                    {
                        index++;
                    }
                }
            }
        }

        /// <summary>
        /// 增加加载资源代理辅助器
        /// </summary>
        /// <param name="loadResourceAgentHelper">要增加的加载资源代理辅助器</param>
        public void AddLoadResourceAgentHelper(ILoadResourceAgentHelper loadResourceAgentHelper)
        {
            throw new NotSupportedException("AddLoadResourceAgentHelper");
        }

        /// <summary>
        /// 使用可更新模式并检查资源
        /// </summary>
        /// <param name="checkResourcesCompleteCallback">使用可更新模式并检查资源完成时的回调函数</param>
        public void CheckResources(CheckResourcesCompleteCallback checkResourcesCompleteCallback)
        {
            throw new NotSupportedException("CheckResources");
        }

        /// <summary>
        /// 使用可更新模式并检查版本资源列表
        /// </summary>
        /// <param name="latestInternalResourceVersion">最新的内部资源版本号</param>
        /// <returns>检查版本资源列表结果</returns>
        public CheckVersionListResult CheckVersionList(int latestInternalResourceVersion)
        {
            throw new NotSupportedException("CheckVersionList");
        }

        /// <summary>
        /// 检查资源是否存在
        /// </summary>
        /// <param name="assetName">资源名称</param>
        /// <returns>资源是否存在</returns>
        public bool HasAsset(string assetName)
        {
#if UNITY_EDITOR
            return UnityEditor.AssetDatabase.LoadMainAssetAtPath(assetName) != null;
#else
            return false;
#endif
        }

        /// <summary>
        /// 使用单机模式并初始化资源
        /// </summary>
        /// <param name="initResourceCompleteCallback">使用单机模式并初始化资源完成时的回调函数</param>
        public void InitResources(InitResourcesCompleteCallback initResourceCompleteCallback)
        {
            throw new NotSupportedException("InitResources");
        }

        /// <summary>
        /// 获取资源组是否准备完毕
        /// </summary>
        /// <param name="resourceGroupName">资源组名称</param>
        /// <returns>是否准备完毕</returns>
        public bool IsResourceGroupReady(string resourceGroupName)
        {
            throw new NotSupportedException("IsResourceGroupReady");
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <param name="assetName">资源名称</param>
        /// <param name="loadAssetCallbacks">加载资源回调函数集</param>
        public void LoadAsset(string assetName, LoadAssetCallbacks loadAssetCallbacks)
        {
            LoadAsset(assetName, null, 0, loadAssetCallbacks, null);
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <param name="assetName">资源名称</param>
        /// <param name="assetType">资源类型</param>
        /// <param name="loadAssetCallbacks">加载资源回调函数集</param>
        public void LoadAsset(string assetName, Type assetType, LoadAssetCallbacks loadAssetCallbacks)
        {
            LoadAsset(assetName, assetType, 0, loadAssetCallbacks, null);
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <param name="assetName">资源名称</param>
        /// <param name="priority">加载资源优先级</param>
        /// <param name="loadAssetCallbacks">加载资源回调函数集</param>
        public void LoadAsset(string assetName, int priority, LoadAssetCallbacks loadAssetCallbacks)
        {
            LoadAsset(assetName, null, priority, loadAssetCallbacks, null);
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <param name="assetName">资源名称</param>
        /// <param name="assetType">资源类型</param>
        /// <param name="priority">加载资源优先级</param>
        /// <param name="loadAssetCallbacks">加载资源回调函数集</param>
        public void LoadAsset(string assetName, Type assetType, int priority, LoadAssetCallbacks loadAssetCallbacks)
        {
            LoadAsset(assetName, assetType, priority, loadAssetCallbacks, null);
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <param name="assetName">资源名称</param>
        /// <param name="loadAssetCallbacks">加载资源回调函数集</param>
        /// <param name="userData">用户自定义数据</param>
        public void LoadAsset(string assetName, LoadAssetCallbacks loadAssetCallbacks, object userData)
        {
            LoadAsset(assetName, null, 0, loadAssetCallbacks, userData);
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <param name="assetName">资源名称</param>
        /// <param name="assetType">资源类型</param>
        /// <param name="loadAssetCallbacks">加载资源回调函数集</param>
        /// <param name="userData">用户自定义数据</param>
        public void LoadAsset(string assetName, Type assetType, LoadAssetCallbacks loadAssetCallbacks, object userData)
        {
            LoadAsset(assetName, assetType, 0, loadAssetCallbacks, userData);
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <param name="assetName">资源名称</param>
        /// <param name="priority">加载资源优先级</param>
        /// <param name="loadAssetCallbacks">加载资源回调函数集</param>
        /// <param name="userData">用户自定义数据</param>
        public void LoadAsset(string assetName, int priority, LoadAssetCallbacks loadAssetCallbacks, object userData)
        {
            LoadAsset(assetName, null, priority, loadAssetCallbacks, userData);
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <param name="assetName">资源名称</param>
        /// <param name="assetType">资源类型</param>
        /// <param name="priority">加载资源优先级</param>
        /// <param name="loadAssetCallbacks">加载资源回调函数集</param>
        /// <param name="userData">用户自定义数据</param>
        public void LoadAsset(string assetName, Type assetType, int priority, LoadAssetCallbacks loadAssetCallbacks, object userData)
        {
            if (string.IsNullOrEmpty(assetName))
            {
                Log.Error("[EditorResourceManager.LoadAsset] Asset name is invalid.");
                return;
            }

            if (!assetName.StartsWith("Assets/"))
            {
                Log.Error("[EditorResourceManager.LoadAsset] Asset name '{0}' is invalid.", assetName);
                return;
            }

            if (loadAssetCallbacks == null)
            {
                Log.Error("[EditorResourceManager.LoadAsset] Load asset callbacks is invalid.");
                return;
            }

            if (!HasFile(assetName))
            {
                Log.Error("[EditorResourceManager.LoadAsset] Asset '{0}' is not exist.", assetName);
                return;
            }

            m_LoadAssetInfos.Add(new LoadAssetInfo(assetName, assetType, priority, DateTime.Now, m_MinLoadAssetRandomDelaySeconds + (float)Utility.Random.GetRandomDouble() * (m_MaxLoadAssetRandomDelaySeconds - m_MinLoadAssetRandomDelaySeconds), loadAssetCallbacks, userData));
        }


        /// <summary>
        /// 异步加载场景
        /// </summary>
        /// <param name="sceneAssetName">要加载场景资源的名称</param>
        /// <param name="loadSceneCallbacks">加载场景回调函数集</param>
        public void LoadScene(string sceneAssetName, LoadSceneCallbacks loadSceneCallbacks)
        {
            LoadScene(sceneAssetName, 0, loadSceneCallbacks, null);
        }

        /// <summary>
        /// 异步加载场景
        /// </summary>
        /// <param name="sceneAssetName">要加载场景资源的名称</param>
        /// <param name="priority">加载场景资源的优先级</param>
        /// <param name="loadSceneCallbacks">加载场景回调函数集</param>
        public void LoadScene(string sceneAssetName, int priority, LoadSceneCallbacks loadSceneCallbacks)
        {
            LoadScene(sceneAssetName, priority, loadSceneCallbacks, null);
        }

        /// <summary>
        /// 异步加载场景
        /// </summary>
        /// <param name="sceneAssetName">要加载场景资源的名称</param>
        /// <param name="loadSceneCallbacks">加载场景回调函数集</param>
        /// <param name="userData">用户自定义数据</param>
        public void LoadScene(string sceneAssetName, LoadSceneCallbacks loadSceneCallbacks, object userData)
        {
            LoadScene(sceneAssetName, 0, loadSceneCallbacks, userData);
        }

        /// <summary>
        /// 异步加载场景
        /// </summary>
        /// <param name="sceneAssetName">要加载场景资源的名称</param>
        /// <param name="priority">加载场景资源的优先级</param>
        /// <param name="loadSceneCallbacks">加载场景回调函数集</param>
        /// <param name="userData">用户自定义数据</param>
        public void LoadScene(string sceneAssetName, int priority, LoadSceneCallbacks loadSceneCallbacks, object userData)
        {
            if (string.IsNullOrEmpty(sceneAssetName))
            {
                Log.Error("[EditorResourceManager.LoadScene] Scene asset name is invalid.");
                return;
            }

            if (!sceneAssetName.StartsWith("Assets/") || !sceneAssetName.EndsWith(".unity"))
            {
                Log.Error("[EditorResourceManager.LoadScene] Scene asset name '{0}' is invalid.", sceneAssetName);
                return;
            }

            if (loadSceneCallbacks == null)
            {
                Log.Error("[EditorResourceManager.LoadScene] Load scene callbacks is invalid.");
                return;
            }

            if (!HasFile(sceneAssetName))
            {
                Log.Error("[EditorResourceManager.LoadScene] Scene '{0}' is not exist.", sceneAssetName);
                return;
            }

#if UNITY_5_5_OR_NEWER
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneAssetName, LoadSceneMode.Additive);
#else
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(SceneComponent.GetSceneName(sceneAssetName), LoadSceneMode.Additive);
#endif
            if (asyncOperation == null)
                return;

            m_LoadSceneInfos.Add(new LoadSceneInfo(asyncOperation, sceneAssetName, priority, DateTime.Now, loadSceneCallbacks, userData));
        }


        /// <summary>
        /// 设置当前变体
        /// </summary>
        /// <param name="currentVariant"></param>
        public void SetCurrentVariant(string currentVariant)
        {
            throw new NotSupportedException("SetCurrentVariant");
        }

        /// <summary>
        /// 设置界面资源回调函数
        /// </summary>
        /// <param name="decryptResourceCallback">要设置的解密资源回调函数</param>
        /// <remarks>如果不设置，将使用默认的解密资源回调函数</remarks>
        public void SetDecryptResourceCallback(DecryptResourceCallback decryptResourceCallback)
        {
            throw new NotSupportedException("SetDecryptResourceCallback");
        }

        /// <summary>
        /// 设置下载管理器
        /// </summary>
        /// <param name="downloadManager">下载管理器</param>
        public void SetDownloadManager(IDownloadManager downloadManager)
        {
            throw new NotSupportedException("SetDownloadManager");
        }

        /// <summary>
        /// 设置对象池管理器
        /// </summary>
        /// <param name="objectPoolManager">对象池管理器</param>
        public void SetObjectPoolManager(IObjectPoolManager objectPoolManager)
        {
            throw new NotSupportedException("SetObjectPoolManager");
        }

        /// <summary>
        /// 设置资源只读区路径
        /// </summary>
        /// <param name="readOnlyPath">资源只读区路径</param>
        public void SetReadOnlyPath(string readOnlyPath)
        {
            if (string.IsNullOrEmpty(readOnlyPath))
            {
                Log.Error("[EditorResourceManager.SetReadOnlyPath] Readonly path is invalid.");
                return;
            }

            m_ReadOnlyPath = readOnlyPath;
        }

        /// <summary>
        /// 设置资源读写区路径
        /// </summary>
        /// <param name="readWritePath">资源读写区路径</param>
        public void SetReadWritePath(string readWritePath)
        {
            if (string.IsNullOrEmpty(readWritePath))
            {
                Log.Error("[EditorResourceManager.SetReadOnlyPath] Read-write path is invalid.");
                return;
            }

            m_ReadWritePath = readWritePath;
        }

        /// <summary>
        /// 设置资源辅助器
        /// </summary>
        /// <param name="resourceHelper">资源辅助器</param>
        public void SetResourceHelper(IResourceHelper resourceHelper)
        {
            throw new NotSupportedException("SetResourceHelper");
        }

        /// <summary>
        /// 设置资源模式
        /// </summary>
        /// <param name="resourceMode">资源模式</param>
        public void SetResourceMode(ResourceMode resourceMode)
        {
            throw new NotSupportedException("SetResourceMode");
        }

        /// <summary>
        /// 卸载资源
        /// </summary>
        /// <param name="asset">资源名称</param>
        public void UnloadAsset(object asset)
        {

        }

        /// <summary>
        /// 异步卸载场景
        /// </summary>
        /// <param name="sceneAssetName">要卸载场景资源的名称</param>
        /// <param name="unloadSceneCallbacks">卸载场景回调函数集</param>
        public void UnloadScene(string sceneAssetName, UnloadSceneCallbacks unloadSceneCallbacks)
        {
            UnloadScene(sceneAssetName, unloadSceneCallbacks, null);
        }

        /// <summary>
        /// 异步卸载场景
        /// </summary>
        /// <param name="sceneAssetName">要卸载场景资源的名称</param>
        /// <param name="unloadSceneCallbacks">卸载场景回调函数集</param>
        /// <param name="userData">用户自定义数据</param>
        public void UnloadScene(string sceneAssetName, UnloadSceneCallbacks unloadSceneCallbacks, object userData)
        {
            if (string.IsNullOrEmpty(sceneAssetName))
            {
                Log.Error("[EditorResourceManager.UnloadScene] Scene asset name is invalid.");
                return;
            }

            if (!sceneAssetName.StartsWith("Assets/") || !sceneAssetName.EndsWith(".unity"))
            {
                Log.Error("[EditorResourceManager.UnloadScene] Scene asset name '{0}' is invalid.", sceneAssetName);
                return;
            }

            if (unloadSceneCallbacks == null)
            {
                Log.Error("[EditorResourceManager.UnloadScene] Unload scene callbacks is invalid.");
                return;
            }

            if (!HasFile(sceneAssetName))
            {
                Log.Error("[EditorResourceManager.UnloadScene] Scene '{0}' is not exist.", sceneAssetName);
                return;
            }
#if UNITY_5_5_OR_NEWER
            AsyncOperation asyncOperation = SceneManager.UnloadSceneAsync(sceneAssetName);
            if (asyncOperation == null)
            {
                return;
            }

            m_UnloadSceneInfos.Add(new UnloadSceneInfo(asyncOperation, sceneAssetName, unloadSceneCallbacks, userData));
#else
            if (SceneManager.UnloadScene(SceneComponent.GetSceneName(sceneAssetName)))
            {
                if (unloadSceneCallbacks.UnloadSceneSuccessCallback != null)
                {
                    unloadSceneCallbacks.UnloadSceneSuccessCallback(sceneAssetName, userData);
                }
            }
            else
            {
                if (unloadSceneCallbacks.UnloadSceneFailureCallback != null)
                {
                    unloadSceneCallbacks.UnloadSceneFailureCallback(sceneAssetName, userData);
                }
            }
#endif
        }

        /// <summary>
        /// 使用可更新模式并更新全部资源
        /// </summary>
        /// <param name="updateResourcesCompleteCallback">使用可更新模式并更新默认资源组完成时的回调函数</param>
        public void UpdateResources(UpdateResourcesCompleteCallback updateResourcesCompleteCallback)
        {
            throw new NotSupportedException("UpdateResources");
        }

        /// <summary>
        /// 使用可更新模式并更新指定资源组的资源
        /// </summary>
        /// <param name="resourceGroupName">要更新的资源组名称</param>
        /// <param name="updateResourcesCompleteCallback">使用可更新模式并更新指定资源组完成时的回调函数</param>
        public void UpdateResources(string resourceGroupName, UpdateResourcesCompleteCallback updateResourcesCompleteCallback)
        {
            throw new NotSupportedException("UpdateResources");
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
            throw new NotSupportedException("UpdateVersionList");
        }

        //检查是否存在文件
        private bool HasFile(string assetName)
        {
            if (string.IsNullOrEmpty(assetName))
                return false;

            string assetFullName = Application.dataPath.Substring(0, Application.dataPath.Length - AssetsSubstringLength) + assetName;
            if (string.IsNullOrEmpty(assetFullName))
                return false;

            string[] splitAssetFullName = assetFullName.Split('/');
            string currentPath = Path.GetPathRoot(assetFullName);
            for (int i = 1; i < splitAssetFullName.Length - 1; i++)
            {
                string[] directoryNames = Directory.GetDirectories(currentPath, splitAssetFullName[i]);
                if (directoryNames.Length != 1)
                    return false;

                currentPath = directoryNames[0];
            }

            string[] fileNames = Directory.GetFiles(currentPath, splitAssetFullName[splitAssetFullName.Length - 1]);
            if (fileNames.Length != 1)
            {
                return false;
            }

            string fileFullName = Utility.Path.GetRegularPath(fileNames[0]);
            if (fileFullName == null)
                return false;

            if (assetFullName != fileFullName)
            {
                if (assetFullName.ToLower() == fileFullName.ToLower())
                    Log.Warning("The real path of the specific asset '{0}' is '{1}'. Check the case of letters in the path.", assetName, "Assets" + fileFullName.Substring(Application.dataPath.Length));

                return false;
            }

            return true;
        }

        /// <summary>
        /// 检查资源组是否存在
        /// </summary>
        /// <param name="resourceGroupName">要检查资源组的名称</param>
        /// <returns>资源组是否存在</returns>
        public bool HasResourceGroup(string resourceGroupName)
        {
            throw new NotSupportedException("HasResourceGroup");
        }

        /// <summary>
        /// 获取资源组
        /// </summary>
        /// <param name="resourceGroupName">要获取的资源组名称</param>
        /// <returns>要获取的资源组</returns>
        public IResourceGroup GetResourceGroup(string resourceGroupName)
        {
            throw new NotSupportedException("GetResourceGroup");
        }
    }

}
