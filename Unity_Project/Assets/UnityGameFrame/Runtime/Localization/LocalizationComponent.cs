using GameFramework;
using GameFramework.Localization;
using System.IO;
using UnityEngine;

namespace UnityGameFrame.Runtime
{
    /// <summary>
    /// 本地化组件
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Game Framework/Localization")]
    public sealed class LocalizationComponent : GameFrameworkComponent
    {
        private ILocalizationManager m_LocalizationManager = null;  //本地化管理器
        private EventComponent m_EventComponent = null; //事件组件

        //是否启用相关的事件
        [SerializeField]
        private bool m_EnableLoadDictionarySuccessEvent = true;
        [SerializeField]
        private bool m_EnableLoadDictionaryFailureEvent = true;
        [SerializeField]
        private bool m_EnableLoadDictionaryUpdateEvent = false;
        [SerializeField]
        private bool m_EnableLoadDictionaryDependencyAssetEvent = false;

        [SerializeField]
        private string m_LocalizationHelperTypeName = "UnityGameFrame.Runtime.DefaultLocalizationHelper";   //默认辅助器类型

        [SerializeField]
        private LocalizationHelperBase m_CustomLocalizationHelper = null;   //自定义辅助器

        /// <summary>
        /// 获取或设置本地化语言
        /// </summary>
        public Language Language
        {
            get { return m_LocalizationManager.Language; }
            set { m_LocalizationManager.Language = value; }
        }

        /// <summary>
        /// 获取系统语言
        /// </summary>
        public Language SystemLanguage { get { return m_LocalizationManager.SystemLanguage; } }

        /// <summary>
        /// 获取字典数量
        /// </summary>
        public int DictionaryCount { get { return m_LocalizationManager.DictionaryCount; } }

        protected override void Awake()
        {
            base.Awake();
            m_LocalizationManager = GameFrameworkEntry.GetModule<ILocalizationManager>();
            if (m_LocalizationManager == null)
            {
                Log.Fatal("[LocalizationComponent.Awake] Localization manager is invalid.");
                return;
            }

            //注册事件
            m_LocalizationManager.LoadDictionarySuccess += OnLoadDictionarySuccess;
            m_LocalizationManager.LoadDictionaryFailure += OnLoadDictionaryFailure;
            m_LocalizationManager.LoadDictionaryUpdate += OnLoadDictionaryUpdate;
            m_LocalizationManager.LoadDictionaryDependencyAsset += OnLoadDictionaryDependencyAsset;
        }

        private void Start()
        {
            BaseComponent baseComponent = GameEntry.GetComponent<BaseComponent>();
            if (baseComponent == null)
            {
                Log.Fatal("[LocalizationComponent.Start] Base component is invalid.");
                return;
            }

            m_EventComponent = GameEntry.GetComponent<EventComponent>();
            if (m_EventComponent == null)
            {
                Log.Fatal("[LocalizationComponent.Start] Event component is invalid.");
                return;
            }

            m_LocalizationManager.SetResourceManager(baseComponent.ResourceManager);    //设置资源加载管理器
            LocalizationHelperBase localizationHelper = Helper.CreateHelper(m_LocalizationHelperTypeName, m_CustomLocalizationHelper);
            if (localizationHelper == null)
            {
                Log.Error("[LocalizationComponent.Start] Can not create localization helper.");
                return;
            }
            localizationHelper.name = "Localization Helper";
            Transform trans = localizationHelper.transform;
            trans.SetParent(this.transform);
            trans.localScale = Vector3.one;

            //设置辅助器
            m_LocalizationManager.SetLocalizationHelper(localizationHelper);
            //设置语言
            m_LocalizationManager.Language = baseComponent.IsEditorResourceMode && baseComponent.EditorLanguage != Language.Unspecified ? baseComponent.EditorLanguage : m_LocalizationManager.SystemLanguage;
        }

        /// <summary>
        /// 加载字典
        /// </summary>
        /// <param name="dictionaryName">字典名称</param>
        /// <param name="dictionaryAssetName">字典资源名称</param>
        /// <param name="loadType">字典加载方式</param>
        /// <param name="userData">用户自定义数据</param>
        public void LoadDictionary(string dictionaryName, string dictionaryAssetName, LoadType loadType, object userData = null)
        {
            LoadDictionary(dictionaryName, dictionaryAssetName, loadType, 0, userData);
        }

        /// <summary>
        /// 加载字典
        /// </summary>
        /// <param name="dictionaryName">字典名称</param>
        /// <param name="dictionaryAssetName">字典资源名称</param>
        /// <param name="loadType">字典加载方式</param>
        /// <param name="priority">加载字典资源的优先级</param>
        /// <param name="userData">用户自定义数据</param>
        public void LoadDictionary(string dictionaryName, string dictionaryAssetName, LoadType loadType, int priority, object userData = null)
        {
            if (string.IsNullOrEmpty(dictionaryName))
            {
                Log.Error("[LocalizationComponent.LoadDictionary] Dictionary name is invalid.");
                return;
            }
            m_LocalizationManager.LoadDictionary(dictionaryAssetName, loadType, priority, new LoadDictionaryInfo(dictionaryName, userData));
        }

        /// <summary>
        /// 解析字典
        /// </summary>
        /// <param name="text">要解析的字典文本</param>
        /// <param name="userData">用户自定义数据</param>
        /// <returns>是否解析字典成功</returns>
        public bool ParseDictionary(string text, object userData = null)
        {
            return m_LocalizationManager.ParseDictionary(text, userData);
        }

        /// <summary>
        /// 解析字典
        /// </summary>
        /// <param name="bytes">要解析的字典二进制流</param>
        /// <param name="userData">用户自定义数据</param>
        /// <returns>是否解析字典成功</returns>
        public bool ParseDictionary(byte[] bytes, object userData = null)
        {
            return m_LocalizationManager.ParseDictionary(bytes, userData);
        }

        /// <summary>
        /// 解析字典
        /// </summary>
        /// <param name="stream">要解析的字典二进制流</param>
        /// <param name="userData">用户自定义数据</param>
        /// <returns>是否解析字典成功</returns>
        public bool ParseDictionary(Stream stream, object userData = null)
        {
            return m_LocalizationManager.ParseDictionary(stream, userData);
        }

        /// <summary>
        /// 根据字典主键获取字典内容字符串
        /// </summary>
        /// <param name="key">字典主键</param>
        /// <returns>要获取的字典内容字符串</returns>
        public string GetString(string key)
        {
            return m_LocalizationManager.GetString(key);
        }

        /// <summary>
        /// 根据字典主键获取字典内容字符串
        /// </summary>
        /// <param name="key">字典主键</param>
        /// <param name="args">字典参数</param>
        /// <returns>要获取的字典内容字符串</returns>
        public string GetString(string key, params object[] args)
        {
            return m_LocalizationManager.GetString(key, args);
        }

        /// <summary>
        /// 是否存在字典
        /// </summary>
        /// <param name="key">字典主键</param>
        /// <returns>是否存在字典</returns>
        public bool HasString(string key)
        {
            return m_LocalizationManager.HasRawString(key);
        }

        /// <summary>
        /// 移除字典
        /// </summary>
        /// <param name="key">字典主键</param>
        /// <returns>是否移除字典成功</returns>
        public bool RemoveString(string key)
        {
            return m_LocalizationManager.RemoveRawString(key);
        }

        //加载资源成功的回调
        private void OnLoadDictionarySuccess(object sender, GameFramework.Localization.LoadDictionarySuccessEventArgs e)
        {
            if (m_EnableLoadDictionarySuccessEvent)
            {
                m_EventComponent.Fire(this, ReferencePool.Acquire<LoadDictionarySuccessEventArgs>().Fill(e));
            }
        }

        //加载资源成功的回调
        private void OnLoadDictionaryFailure(object sender, GameFramework.Localization.LoadDictionaryFailureEventArgs e)
        {
            if (m_EnableLoadDictionaryFailureEvent)
            {
                m_EventComponent.Fire(this, ReferencePool.Acquire<LoadDictionaryFailureEventArgs>().Fill(e));
            }
        }

        //加载资源成功的回调
        private void OnLoadDictionaryUpdate(object sender, GameFramework.Localization.LoadDictionaryUpdateEventArgs e)
        {
            if (m_EnableLoadDictionaryUpdateEvent)
            {
                m_EventComponent.Fire(this, ReferencePool.Acquire<LoadDictionaryUpdateEventArgs>().Fill(e));
            }
        }

        //加载资源成功的回调
        private void OnLoadDictionaryDependencyAsset(object sender, GameFramework.Localization.LoadDictionaryDependencyAssetEventArgs e)
        {
            if (m_EnableLoadDictionaryDependencyAssetEvent)
            {
                m_EventComponent.Fire(this, ReferencePool.Acquire<LoadDictionaryDependencyAssetEventArgs>().Fill(e));
            }
        }
    }
}
