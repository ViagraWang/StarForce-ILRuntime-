using GameFramework;
using GameFramework.Scene;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SceneManager = UnityEngine.SceneManagement.SceneManager;

namespace UnityGameFrame.Runtime
{
    /// <summary>
    /// 场景组件
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Game Framework/Scene")]
    public sealed class SceneComponent : GameFrameworkComponent
    {
        private ISceneManager m_SceneManager = null;    //场景管理器
        private EventComponent m_EventComponent = null; //事件组件
        private Camera m_MainCamera = null; //主相机
        private Scene m_GFScene = default(Scene);    //框架所在的场景

        //是否开启事件的标志位
        [SerializeField]
        private bool m_EnableLoadSceneSuccessEvent = true;
        [SerializeField]
        private bool m_EnableLoadSceneFailureEvent = true;
        [SerializeField]
        private bool m_EnableLoadSceneUpdateEvent = true;
        [SerializeField]
        private bool m_EnableLoadSceneDependencyAssetEvent = true;
        [SerializeField]
        private bool m_EnableUnloadSceneSuccessEvent = true;
        [SerializeField]
        private bool m_EnableUnloadSceneFailureEvent = true;

        /// <summary>
        /// 获取当前场景主相机
        /// </summary>
        public Camera MainCamera { get { return m_MainCamera; } }

        protected override void Awake()
        {
            base.Awake();
            m_SceneManager = GameFrameworkEntry.GetModule<ISceneManager>();
            if (m_SceneManager == null)
            {
                Log.Fatal("[SceneComponent.Awake] Scene manager is invalid.");
                return;
            }

            //注册事件
            m_SceneManager.LoadSceneSuccess += OnLoadSceneSuccess;
            m_SceneManager.LoadSceneFailure += OnLoadSceneFailure;
            m_SceneManager.LoadSceneUpdate += OnLoadSceneUpdate;
            m_SceneManager.LoadSceneDependencyAsset += OnLoadSceneDependencyAsset;
            m_SceneManager.UnloadSceneSuccess += OnUnloadSceneSuccess;
            m_SceneManager.UnloadSceneFailure += OnUnloadSceneFailure;

            //获取框架场景
            m_GFScene = SceneManager.GetSceneAt(GameEntry.GFSceneId);
            if (!m_GFScene.IsValid())
            {
                Log.Fatal("[SceneComponent.Awake] Game Framework scene is invalid.");
                return;
            }
        }

        private void Start()
        {
            //基础组件
            BaseComponent baseComponent = GameEntry.GetComponent<BaseComponent>();
            if(baseComponent == null)
            {
                Log.Fatal("[SceneComponent.Start] Base component is invalid.");
                return;
            }

            //事件组件
            m_EventComponent = GameEntry.GetComponent<EventComponent>();
            if (m_EventComponent == null)
            {
                Log.Fatal("[SceneComponent.Start] Event component is invalid.");
                return;
            }
            //设置资源管理器
            m_SceneManager.SetResourceManager(baseComponent.ResourceManager);
        }

        /// <summary>
        /// 获取场景是否已加载
        /// </summary>
        /// <param name="sceneAssetName">场景资源名称</param>
        /// <returns>场景是否已加载</returns>
        public bool SceneIsLoaded(string sceneAssetName)
        {
            return m_SceneManager.SceneIsLoaded(sceneAssetName);
        }

        /// <summary>
        /// 获取已加载场景的资源名称
        /// </summary>
        /// <returns>已加载场景的资源名称</returns>
        public string[] GetLoadedSceneAssetNames()
        {
            return m_SceneManager.GetLoadedSceneAssetNames();
        }

        /// <summary>
        /// 获取已加载场景的资源名称
        /// </summary>
        /// <param name="results">已加载场景的资源名称</param>
        public void GetLoadedSceneAssetNames(List<string> results)
        {
            m_SceneManager.GetLoadedSceneAssetNames(results);
        }

        /// <summary>
        /// 获取场景是否正在加载
        /// </summary>
        /// <param name="sceneAssetName">场景资源名称</param>
        /// <returns>场景是否正在加载</returns>
        public bool SceneIsLoading(string sceneAssetName)
        {
            return m_SceneManager.SceneIsLoading(sceneAssetName);
        }

        /// <summary>
        /// 获取正在加载场景的资源名称
        /// </summary>
        /// <returns>正在加载场景的资源名称</returns>
        public string[] GetLoadingSceneAssetNames()
        {
            return m_SceneManager.GetLoadingSceneAssetNames();
        }

        /// <summary>
        /// 获取正在加载场景的资源名称
        /// </summary>
        /// <param name="results">正在加载场景的资源名称</param>
        public void GetLoadingSceneAssetNames(List<string> results)
        {
            m_SceneManager.GetLoadingSceneAssetNames(results);
        }

        /// <summary>
        /// 判断场景是否正在卸载
        /// </summary>
        /// <param name="sceneAssetName">场景资源名称</param>
        /// <returns>场景是否正在卸载</returns>
        public bool SceneIsUnloading(string sceneAssetName)
        {
            return m_SceneManager.SceneIsUnloading(sceneAssetName);
        }

        /// <summary>
        /// 获取正在卸载场景的资源名称
        /// </summary>
        /// <returns>正在卸载场景的资源名称</returns>
        public string[] GetUnloadingSceneAssetNames()
        {
            return m_SceneManager.GetUnloadingSceneAssetNames();
        }

        /// <summary>
        /// 获取正在卸载场景的资源名称
        /// </summary>
        /// <param name="results">正在卸载场景的资源名称</param>
        public void GetUnloadingSceneAssetNames(List<string> results)
        {
            m_SceneManager.GetUnloadingSceneAssetNames(results);
        }

        /// <summary>
        /// 加载场景
        /// </summary>
        /// <param name="sceneAssetName">场景资源名称</param>
        /// <param name="userData">用户自定义数据</param>
        public void LoadScene(string sceneAssetName, object userData = null)
        {
            LoadScene(sceneAssetName, 0, userData);
        }

        /// <summary>
        /// 加载场景
        /// </summary>
        /// <param name="sceneAssetName">场景资源名称</param>
        /// <param name="priority">加载场景资源的优先级</param>
        /// <param name="userData">用户自定义数据</param>
        public void LoadScene(string sceneAssetName, int priority, object userData = null)
        {
            if (string.IsNullOrEmpty(sceneAssetName))
            {
                Log.Error("[SceneComponent.LoadScene] Scene asset name is invalid.");
                return;
            }

            if (!sceneAssetName.StartsWith("Assets/") || !sceneAssetName.EndsWith(".unity"))
            {
                Log.Error("[SceneComponent.LoadScene] Scene asset name '{0}' is invalid.", sceneAssetName);
                return;
            }
            m_SceneManager.LoadScene(sceneAssetName, priority, userData);
        }

        /// <summary>
        /// 卸载场景
        /// </summary>
        /// <param name="sceneAssetName">场景资源名称</param>
        /// <param name="userData">用户自定义数据</param>
        public void UnloadScene(string sceneAssetName, object userData = null)
        {
            if (string.IsNullOrEmpty(sceneAssetName))
            {
                Log.Error("[SceneComponent.UnloadScene] Scene asset name is invalid.");
                return;
            }

            if (!sceneAssetName.StartsWith("Assets/") || !sceneAssetName.EndsWith(".unity"))
            {
                Log.Error("[SceneComponent.UnloadScene] Scene asset name '{0}' is invalid.", sceneAssetName);
                return;
            }
            m_SceneManager.UnloadScene(sceneAssetName, userData);
        }

        //加载场景成功的回调
        private void OnLoadSceneSuccess(object sender, GameFramework.Scene.LoadSceneSuccessEventArgs e)
        {
            m_MainCamera = Camera.main; //获取主相机
            if(SceneManager.GetActiveScene() == m_GFScene)   //如果当前是框架所在场景
            {
                Scene scene = SceneManager.GetSceneByName(GetSceneName(e.SceneAssetName));  //获取加载成功的场景
                if (!scene.IsValid())   //加载的场景不合法
                {
                    Log.Error("[SceneComponent.OnLoadSceneSuccess] Loaded scene '{0}' is invalid.", e.SceneAssetName);
                    return;
                }

                SceneManager.SetActiveScene(scene); //激活场景
            }

            if (m_EnableLoadSceneSuccessEvent)
                m_EventComponent.Fire(this, ReferencePool.Acquire<LoadSceneSuccessEventArgs>().Fill(e));

        }

        //加载场景失败的回调
        private void OnLoadSceneFailure(object sender, GameFramework.Scene.LoadSceneFailureEventArgs e)
        {
            Log.Warning("[SceneComponent.OnLoadSceneFailure] Load scene failure, scene asset name '{0}', error message '{1}'.", e.SceneAssetName, e.ErrorMessage);
            if (m_EnableLoadSceneFailureEvent)
                m_EventComponent.Fire(this, ReferencePool.Acquire<LoadSceneFailureEventArgs>().Fill(e));
        }

        //加载场景更新的回调
        private void OnLoadSceneUpdate(object sender, GameFramework.Scene.LoadSceneUpdateEventArgs e)
        {
            if (m_EnableLoadSceneUpdateEvent)
                m_EventComponent.Fire(this, ReferencePool.Acquire<LoadSceneUpdateEventArgs>().Fill(e));
        }

        //加载场景依赖资源的回调
        private void OnLoadSceneDependencyAsset(object sender, GameFramework.Scene.LoadSceneDependencyAssetEventArgs e)
        {
            if (m_EnableLoadSceneDependencyAssetEvent)
                m_EventComponent.Fire(this, ReferencePool.Acquire<LoadSceneDependencyAssetEventArgs>().Fill(e));
        }

        //卸载场景成功的回调
        private void OnUnloadSceneSuccess(object sender, GameFramework.Scene.UnloadSceneSuccessEventArgs e)
        {
            m_MainCamera = Camera.main;
            if (m_EnableUnloadSceneSuccessEvent)
                m_EventComponent.Fire(this, ReferencePool.Acquire<UnloadSceneSuccessEventArgs>().Fill(e));
        }

        //卸载场景失败的回调
        private void OnUnloadSceneFailure(object sender, GameFramework.Scene.UnloadSceneFailureEventArgs e)
        {
            Log.Warning("[SceneComponent.OnUnloadSceneFailure] Unload scene failure, scene asset name '{0}'.", e.SceneAssetName);
            if (m_EnableUnloadSceneFailureEvent)
                m_EventComponent.Fire(this, ReferencePool.Acquire<UnloadSceneFailureEventArgs>().Fill(e));
        }

        /// <summary>
        /// 获取场景名称
        /// </summary>
        /// <param name="sceneAssetName">场景资源名称</param>
        /// <returns>场景名称</returns>
        public static string GetSceneName(string sceneAssetName)
        {
            if (string.IsNullOrEmpty(sceneAssetName))
            {
                Log.Error("[SceneComponent.GetSceneName] Scene asset name is invalid.");
                return null;
            }

            int sceneNamePosition = sceneAssetName.LastIndexOf('/');
            if(sceneNamePosition +1 >= sceneAssetName.Length)
            {
                Log.Error("[SceneComponent.GetSceneName] Scene asset name '{0}' is invalid.", sceneAssetName);
                return null;
            }

            string sceneName = sceneAssetName.Substring(sceneNamePosition + 1);
            sceneNamePosition = sceneName.LastIndexOf(".unity");
            if (sceneNamePosition > 0)
                sceneName = sceneName.Substring(0, sceneNamePosition);

            return sceneName;
        }

    }
}
