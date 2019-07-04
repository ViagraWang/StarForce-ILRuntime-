using GameFramework;
using GameFramework.Localization;
using GameFramework.Resource;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Version = GameFramework.Version;

namespace UnityGameFrame.Runtime
{
    /// <summary>
    /// 基础组件
    /// </summary>
    [DefaultExecutionOrder(-100)]
    [DisallowMultipleComponent] //不允许同事存在多个
    [AddComponentMenu("Game Framework/Base")]   //组件列表菜单
    public sealed class BaseComponent : GameFrameworkComponent
    {
        private const int DefaultDpi = 96;  //窗口每英寸像素数
        private float m_GameSpeedBeforePause = 1f;  //游戏暂停前的播放速度
        [SerializeField]
        private bool m_IsEditorResourceMode = true;   //使用编辑器资源模式的标志位
        [SerializeField]
        private Language m_EditorLanguage = Language.Unspecified;   //本地化语言
        [SerializeField]
        private float m_MinLoadAssetRandomDelaySeconds = 0;   //最小延迟秒数
        [SerializeField]
        private float m_MaxLoadAssetRandomDelaySeconds = 1f;   //最大延迟秒数

        [SerializeField]
        private string m_VersionHelperTypeName = "UnityGameFrame.Runtime.DefaultVersionHelper"; //默认版本辅助器脚本名
        [SerializeField]
        private string m_LogHelperTypeName = "UnityGameFrame.Runtime.DefaultLogHelper"; //默认打印Log辅助器
        [SerializeField]
        private string m_ZipHelperTypeName = "UnityGameFrame.Runtime.DefaultZipHelper"; //默认压缩辅助器
        [SerializeField]
        private string m_JsonHelperTypeName = "UnityGameFrame.Runtime.DefaultJsonHelper"; //默认Json文件辅助器
        [SerializeField]
        private string m_ProfilerHelperTypeName = "UnityGameFrame.Runtime.DefaultProfilerHelper"; //默认调试辅助器
        [SerializeField]
        private int m_FrameRate = 60;   //目标帧率
        [SerializeField]
        private float m_GameSpeed = 1f;   //游戏速度
        [SerializeField]
        private bool m_IsRunInBackground = true;  //后台运行标志位
        [SerializeField]
        private bool m_NeverSleep = true;   //不休眠的标志位

        /// <summary>
        /// 获取或设置是否使用编辑器资源模式（仅编辑器内有效）
        /// </summary>
        public bool IsEditorResourceMode
        {
            get { return m_IsEditorResourceMode; }
            set { m_IsEditorResourceMode = value; }
        }

        /// <summary>
        /// 获取或设置编辑器语言（仅编辑器内有效）
        /// </summary>
        public Language EditorLanguage
        {
            get { return m_EditorLanguage; }
            private set { m_EditorLanguage = value; }
        }

        /// <summary>
        /// 编辑器模拟资源加载最小延迟秒数
        /// </summary>
        public float MinLoadAssetRandomDelaySeconds { get { return m_MinLoadAssetRandomDelaySeconds; } }

        /// <summary>
        /// 编辑器模拟资源加载最大延迟秒数
        /// </summary>
        public float MaxLoadAssetRandomDelaySeconds { get { return m_MaxLoadAssetRandomDelaySeconds; } }

        /// <summary>
        /// 资源管理器
        /// </summary>
        public IResourceManager ResourceManager { get; private set; }

        /// <summary>
        /// 获取或设置游戏帧率
        /// </summary>
        public int FrameRate
        {
            get { return m_FrameRate; }
            set { Application.targetFrameRate = m_FrameRate = value; }
        }

        /// <summary>
        /// 获取或设置游戏速度
        /// </summary>
        public float GameSpeed
        {
            get { return m_GameSpeed; }
            set { Time.timeScale = m_GameSpeed = (value >= 0f ? value : 0f); }
        }

        /// <summary>
        /// 获取游戏是否暂停
        /// </summary>
        public bool IsGamePaused
        {
            get { return m_GameSpeed <= 0f; }
        }

        /// <summary>
        /// 获取是否正常游戏速度
        /// </summary>
        public bool IsNormalGameSpeed
        {
            get { return m_GameSpeed == 1f; }
        }

        /// <summary>
        /// 获取或设置是否允许后台运行
        /// </summary>
        public bool IsRunInBackground
        {
            get { return m_IsRunInBackground; }
            set { Application.runInBackground = m_IsRunInBackground = value; }
        }

        /// <summary>
        /// 获取或设置是否禁止休眠
        /// </summary>
        public bool IsNeverSleep
        {
            get { return m_NeverSleep; }
            set
            {
                m_NeverSleep = value;
                Screen.sleepTimeout = value ? SleepTimeout.NeverSleep : SleepTimeout.SystemSetting;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            InitVersionHelper();    //初始化版本辅助器
            InitLogHelper();    //初始化Log辅助器
            Log.Info("Game Framework Version: {0}", Version.GameFrameworkVersion);
            Log.Info("Game Version: {0} ({1})", Version.GameVersion, Version.InternalGameVersion.ToString());
            Log.Info("Unity Version: {0}", Application.unityVersion);

#if UNITY_5_3_OR_NEWER
            InitZipHelper();    //初始化压缩辅助器
            InitJsonHelper();   //初始化Json辅助器
            InitProfilerHelper();   //初始化调试辅助器

            Utility.Converter.ScreenDpi = Screen.dpi;
            if (Utility.Converter.ScreenDpi <= 0)
                Utility.Converter.ScreenDpi = DefaultDpi;

            m_IsEditorResourceMode &= Application.isEditor;   //进一步判断是否是在Unity编辑器下
            //设置资源管理器
            ResourceManager = m_IsEditorResourceMode ? gameObject.AddComponent<EditorResourceManager>() : GameFrameworkEntry.GetModule<IResourceManager>();

            Application.targetFrameRate = m_FrameRate;  //帧率
            Time.timeScale = m_GameSpeed;   //设置游戏速度
            Application.runInBackground = m_IsRunInBackground;    //后台运行
            Screen.sleepTimeout = m_NeverSleep ? SleepTimeout.NeverSleep : SleepTimeout.SystemSetting;
#else
            //Unity5.3以前的版本不能用
            Log.Error("Game Framework only applies with Unity 5.3 and above, but current Unity version is {0}.", Application.unityVersion);
            GameEntry.Shutdown(ShutdownType.Quit);
#endif
#if UNITY_5_6_OR_NEWER
            Application.lowMemory += OnLowMemory;
#endif
        }

        // Update is called once per frame
        private void Update()
        {
            GameFrameworkEntry.Update(Time.deltaTime, Time.unscaledDeltaTime);
        }

        private void OnDestroy()
        {
#if UNITY_5_6_OR_NEWER
            Application.lowMemory -= OnLowMemory;   //低内存回调
#endif
            GameFrameworkEntry.Shutdown();
        }

        /// <summary>
        /// 暂停游戏
        /// </summary>
        public void PauseGame()
        {
            if (IsGamePaused) return;

            m_GameSpeedBeforePause = GameSpeed; //保存当前的游戏速度
            GameSpeed = 0f;
        }

        /// <summary>
        /// 恢复游戏运行
        /// </summary>
        public void ResumeGame()
        {
            if (!IsGamePaused) return;
            GameSpeed = m_GameSpeedBeforePause;
        }

        /// <summary>
        /// 重置为正常游戏速度
        /// </summary>
        public void ResetNormalGameSpeed()
        {
            if (IsNormalGameSpeed) return;

            GameSpeed = 1f;
        }

        //关闭框架
        internal void Shutdown()
        {
            Destroy(gameObject);
        }

        //初始化版本辅助器
        private void InitVersionHelper()
        {
            if (string.IsNullOrEmpty(m_VersionHelperTypeName))
                return;

            Type versionHelperType = Utility.Assembly.GetType(m_VersionHelperTypeName);
            if (versionHelperType == null)
                throw new GameFrameworkException(Utility.Text.Format("[BaseComponent.InitVersionHelper] Can not find version helper type '{0}'.", m_VersionHelperTypeName));

            Version.IVersionHelper versionHelper = Activator.CreateInstance(versionHelperType) as Version.IVersionHelper;   //实例化版本辅助器
            if (versionHelper == null)
                throw new GameFrameworkException(Utility.Text.Format("[BaseComponent.InitVersionHelper] Can not create version helper instance '{0}'.", m_VersionHelperTypeName));

            Version.SetVersionHelper(versionHelper);    //设置版本辅助器
        }

        //初始化调试日志辅助器
        private void InitLogHelper()
        {
            if (string.IsNullOrEmpty(m_LogHelperTypeName))
                return;

            Type logHelperType = Utility.Assembly.GetType(m_LogHelperTypeName);
            if (logHelperType == null)
                throw new GameFrameworkException(Utility.Text.Format("[BaseComponent.InitLogHelper] Can not find log helper type '{0}'.", m_LogHelperTypeName));

            GameFrameworkLog.ILogHelper logHelper = Activator.CreateInstance(logHelperType) as GameFrameworkLog.ILogHelper;   //实例化日志辅助器
            if (logHelper == null)
                throw new GameFrameworkException(Utility.Text.Format("[BaseComponent.InitLogHelper] Can not create log helper instance '{0}'.", m_LogHelperTypeName));

            GameFrameworkLog.SetLogHelper(logHelper);    //设置日志辅助器
        }

        //初始化压缩辅助器
        private void InitZipHelper()
        {
            if (string.IsNullOrEmpty(m_ZipHelperTypeName))
                return;

            Type zipHelperType = Utility.Assembly.GetType(m_ZipHelperTypeName);
            if (zipHelperType == null)
                throw new GameFrameworkException(Utility.Text.Format("[BaseComponent.InitZipHelper] Can not find zip helper type '{0}'.", m_ZipHelperTypeName));

            Utility.Zip.IZipHelper zipHelper = Activator.CreateInstance(zipHelperType) as Utility.Zip.IZipHelper;   //实例化压缩辅助器
            if (zipHelper == null)
                throw new GameFrameworkException(Utility.Text.Format("[BaseComponent.InitZipHelper] Can not create zip helper instance '{0}'.", m_ZipHelperTypeName));

            Utility.Zip.SetZipHelper(zipHelper);    //设置压缩辅助器
        }

        //初始化Json辅助器
        private void InitJsonHelper()
        {
            if (string.IsNullOrEmpty(m_JsonHelperTypeName))
                return;

            Type jsonHelperType = Utility.Assembly.GetType(m_JsonHelperTypeName);
            if (jsonHelperType == null)
                throw new GameFrameworkException(Utility.Text.Format("[BaseComponent.InitJsonHelper] Can not find json helper type '{0}'.", m_JsonHelperTypeName));

            Utility.Json.IJsonHelper jsonHelper = Activator.CreateInstance(jsonHelperType) as Utility.Json.IJsonHelper;   //实例化Json辅助器
            if (jsonHelper == null)
                throw new GameFrameworkException(Utility.Text.Format("[BaseComponent.InitJsonHelper] Can not create json helper instance '{0}'.", m_JsonHelperTypeName));

            Utility.Json.SetJsonHelper(jsonHelper);    //设置Json辅助器
        }

        //初始化调试辅助器
        private void InitProfilerHelper()
        {
            if (string.IsNullOrEmpty(m_ProfilerHelperTypeName))
                return;

            Type profilerHelperType = Utility.Assembly.GetType(m_ProfilerHelperTypeName);
            if (profilerHelperType == null)
                throw new GameFrameworkException(Utility.Text.Format("[BaseComponent.InitProfilerHelper] Can not find profiler helper type '{0}'.", m_ProfilerHelperTypeName));

            Utility.Profiler.IProfilerHelper profilerHelper = Activator.CreateInstance(profilerHelperType, Thread.CurrentThread) as Utility.Profiler.IProfilerHelper;   //实例化Profiler辅助器
            if (profilerHelper == null)
                throw new GameFrameworkException(Utility.Text.Format("[BaseComponent.InitProfilerHelper] Can not create profiler helper instance '{0}'.", m_ProfilerHelperTypeName));

            Utility.Profiler.SetProfilerHelper(profilerHelper);    //设置Profiler辅助器
        }

        //低内存回调
        private void OnLowMemory()
        {
            Log.Info("[BaseComponent.OnLowMemory] Low memory reported...");
            ObjectPoolComponent objectPoolComponent = GameEntry.GetComponent<ObjectPoolComponent>();
            if (objectPoolComponent != null)
                objectPoolComponent.ReleaseAllUnused(); //释放所有未使用的对象

            ResourceComponent resourceCompoent = GameEntry.GetComponent<ResourceComponent>();
            if (resourceCompoent != null)
                resourceCompoent.ForceUnloadUnusedAssets(true); //强制卸载未使用的资源

        }

    }
}

