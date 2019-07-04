using GameFramework;
using GameFramework.Config;
using System.IO;
using UnityEngine;

namespace UnityGameFrame.Runtime
{
    /// <summary>
    /// 配置组件
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Game Framework/Config")]
    public class ConfigComponent : GameFrameworkComponent
    {

        private IConfigManager m_ConfigManager = null;  //配置管理器接口
        private EventComponent m_EventComponent = null; //事件组件

        [SerializeField]
        private bool m_EnableLoadConfigSuccessEvent = true; //是否启用加载配置成功的事件

        [SerializeField]
        private bool m_EnableLoadConfigFailureEvent = true; //是否启用加载配置失败的事件

        [SerializeField]
        private bool m_EnableLoadConfigUpdateEvent = true; //是否启用加载配置更新的事件

        [SerializeField]
        private bool m_EnableLoadConfigDependencyAssetEvent = true; //是否启用加载配置依赖资源的事件

        [SerializeField]
        private string m_ConfigHelperTypeName = "UnityGameFrame.Runtime.DefaultConfigHelper"; //配置辅助器的名称

        [SerializeField]
        private ConfigHelperBase m_CustomConfigHelper = null;   //自定义的配置辅助器

        /// <summary>
        /// 获取配置数量
        /// </summary>
        public int ConfigCount { get { return m_ConfigManager.ConfigCount; } }


        protected override void Awake()
        {
            base.Awake();
            //通过反射获取配置管理器
            m_ConfigManager = GameFrameworkEntry.GetModule<IConfigManager>();
            if(m_ConfigManager == null)
            {
                Log.Fatal("[ConfigComponent.Awake] Config manager is invalid -> m_ConfigManager == null.");
                return;
            }

            //配置事件
            m_ConfigManager.LoadConfigSuccess += OnLoadConfigSuccess;
            m_ConfigManager.LoadConfigFailure += OnLoadConfigFailure;
            m_ConfigManager.LoadConfigUpdate += OnLoadConfigUpdate;
            m_ConfigManager.LoadConfigDependencyAsset += OnLoadConfigDependencyAsset;
        }

        private void Start()
        {
            //获取基础组件
            BaseComponent baseComponent = GameEntry.GetComponent<BaseComponent>();
            if(baseComponent == null)
            {
                Log.Fatal("[ConfigComponent.Start] Base component is invalid -> baseComponent == null.");
                return;
            }

            //获取事件组件
            m_EventComponent = GameEntry.GetComponent<EventComponent>();
            if (m_EventComponent == null)
            {
                Log.Fatal("[ConfigComponent.Start] Event component is invalid -> m_EventComponent == null.");
                return;
            }

            //设置资源管理器
            m_ConfigManager.SetResourceManager(baseComponent.ResourceManager);

            //设置配置辅助器
            ConfigHelperBase configHelper = Helper.CreateHelper<ConfigHelperBase>(m_ConfigHelperTypeName, m_CustomConfigHelper);
            if(configHelper == null)
            {
                Log.Error("[ConfigComponent.Start] Config helper is invalid -> configHelper == null.");
                return;
            }
            configHelper.name = "Config Helper";
            Transform trans = configHelper.transform;
            trans.SetParent(transform);
            trans.localScale = Vector3.one;
            m_ConfigManager.SetConfigHelper(configHelper);

        }

        /// <summary>
        /// 加载配置
        /// </summary>
        /// <param name="configName">配置名称</param>
        /// <param name="configAssetName">配置资源名称</param>
        /// <param name="loadType">配置加载方式</param>
        public void LoadConfig(string configName, string configAssetName, LoadType loadType)
        {
            LoadConfig(configName, configAssetName, loadType, 0, null);
        }

        /// <summary>
        /// 加载配置
        /// </summary>
        /// <param name="configName">配置名称</param>
        /// <param name="configAssetName">配置资源名称</param>
        /// <param name="loadType">配置加载方式</param>
        /// <param name="priority">加载配置资源的优先级</param>
        public void LoadConfig(string configName, string configAssetName, LoadType loadType, int priority)
        {
            LoadConfig(configName, configAssetName, loadType, priority, null);
        }

        /// <summary>
        /// 加载配置
        /// </summary>
        /// <param name="configName">配置名称</param>
        /// <param name="configAssetName">配置资源名称</param>
        /// <param name="loadType">配置加载方式</param>
        /// <param name="userData">用户自定义数据</param>
        public void LoadConfig(string configName, string configAssetName, LoadType loadType, object userData)
        {
            LoadConfig(configName, configAssetName, loadType, 0, userData);
        }

        /// <summary>
        /// 加载配置
        /// </summary>
        /// <param name="configName">配置名称</param>
        /// <param name="configAssetName">配置资源名称</param>
        /// <param name="loadType">配置加载方式</param>
        /// <param name="priority">加载配置资源的优先级</param>
        /// <param name="userData">用户自定义数据</param>
        public void LoadConfig(string configName, string configAssetName, LoadType loadType, int priority, object userData)
        {
            if (string.IsNullOrEmpty(configName))
            {
                Log.Error("[ConfigComponent.LoadConfig] Config name is invalid -> configName == null.");
                return;
            }

            m_ConfigManager.LoadConfig(configAssetName, loadType, priority, new LoadConfigInfo(configName, userData));
        }

        /// <summary>
        /// 解析配置
        /// </summary>
        /// <param name="text">要解析的配置文本</param>
        /// <param name="userData">用户自定义数据</param>
        /// <returns>是否解析配置成功</returns>
        public bool ParseConfig(string text, object userData = null)
        {
            return m_ConfigManager.ParseConfig(text, userData);
        }

        /// <summary>
        /// 解析配置
        /// </summary>
        /// <param name="bytes">要解析的配置二进制流</param>
        /// <param name="userData">用户自定义数据</param>
        /// <returns>是否解析配置成功</returns>
        public bool ParseConfig(byte[] bytes, object userData = null)
        {
            return m_ConfigManager.ParseConfig(bytes, userData);
        }

        /// <summary>
        /// 解析配置
        /// </summary>
        /// <param name="stream">要解析的配置二进制流</param>
        /// <param name="userData">用户自定义数据</param>
        /// <returns>是否解析配置成功</returns>
        public bool ParseConfig(Stream stream, object userData)
        {
            return m_ConfigManager.ParseConfig(stream, userData);
        }

        /// <summary>
        /// 检查是否存在指定配置项
        /// </summary>
        /// <param name="configName">要检查配置项的名称</param>
        /// <returns>指定的配置项是否存在</returns>
        public bool HasConfig(string configName)
        {
            return m_ConfigManager.HasConfig(configName);
        }

        /// <summary>
        /// 移除指定配置项
        /// </summary>
        /// <param name="configName">要移除配置项的名称</param>
        public void RemoveConfig(string configName)
        {
            m_ConfigManager.RemoveConfig(configName);
        }

        /// <summary>
        /// 清空所有配置项
        /// </summary>
        public void RemoveAllConfigs()
        {
            m_ConfigManager.RemoveAllConfigs();
        }

        /// <summary>
        /// 从指定配置项中读取布尔值
        /// </summary>
        /// <param name="configName">要获取配置项的名称</param>
        /// <param name="defaultValue">当指定的配置项不存在时，返回此默认值</param>
        /// <returns>读取的布尔值</returns>
        public bool GetBool(string configName, bool defaultValue = false)
        {
            return m_ConfigManager.GetBool(configName, defaultValue);
        }

        /// <summary>
        /// 从指定配置项中读取整数值
        /// </summary>
        /// <param name="configName">要获取配置项的名称</param>
        /// <param name="defaultValue">当指定的配置项不存在时，返回此默认值</param>
        /// <returns>读取的整数值</returns>
        public int GetInt(string configName, int defaultValue = 0)
        {
            return m_ConfigManager.GetInt(configName, defaultValue);
        }

        /// <summary>
        /// 从指定配置项中读取浮点数值
        /// </summary>
        /// <param name="configName">要获取配置项的名称</param>
        /// <param name="defaultValue">当指定的配置项不存在时，返回此默认值</param>
        /// <returns>读取的浮点数值</returns>
        public float GetFloat(string configName, float defaultValue = 0f)
        {
            return m_ConfigManager.GetFloat(configName, defaultValue);
        }

        /// <summary>
        /// 从指定配置项中读取字符串值
        /// </summary>
        /// <param name="configName">要获取配置项的名称</param>
        /// <param name="defaultValue">当指定的配置项不存在时，返回此默认值</param>
        /// <returns>读取的字符串值</returns>
        public string GetString(string configName, string defaultValue = null)
        {
            return m_ConfigManager.GetString(configName, defaultValue);
        }

        //加载配置成功的回调
        private void OnLoadConfigSuccess(object sender, GameFramework.Config.LoadConfigSuccessEventArgs e)
        {
            if (m_EnableLoadConfigSuccessEvent)
                m_EventComponent.Fire(this, ReferencePool.Acquire<LoadConfigSuccessEventArgs>().Fill(e));
        }

        //加载配置失败回调
        private void OnLoadConfigFailure(object sender, GameFramework.Config.LoadConfigFailureEventArgs e)
        {
            if (m_EnableLoadConfigFailureEvent)
                m_EventComponent.Fire(this, ReferencePool.Acquire<LoadConfigFailureEventArgs>().Fill(e));
        }

        //加载配置更新的回调
        private void OnLoadConfigUpdate(object sender, GameFramework.Config.LoadConfigUpdateEventArgs e)
        {
            if (m_EnableLoadConfigUpdateEvent)
                m_EventComponent.Fire(this, ReferencePool.Acquire<LoadConfigUpdateEventArgs>().Fill(e));
        }

        //加载配置时加载依赖资源的回调
        private void OnLoadConfigDependencyAsset(object sender, GameFramework.Config.LoadConfigDependencyAssetEventArgs e)
        {
            if (m_EnableLoadConfigDependencyAssetEvent)
                m_EventComponent.Fire(this, ReferencePool.Acquire<LoadConfigDependencyAssetEventArgs>().Fill(e));
        }
    }
}

