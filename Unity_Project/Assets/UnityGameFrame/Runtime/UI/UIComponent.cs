using GameFramework;
using GameFramework.ObjectPool;
using GameFramework.UI;
using System.Collections.Generic;
using UnityEngine;

namespace UnityGameFrame.Runtime
{
    /// <summary>
    /// 界面组件
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Game Framework/UI")]
    public sealed partial class UIComponent : GameFrameworkComponent
    {
        private IUIManager m_UIManager = null;  //UI管理器
        private EventComponent m_EventComponent = null; //事件组件
        private readonly List<IUIForm> m_InternalUIFormResultsCache = new List<IUIForm>();  //内部界面缓存

        //是否开启事件的标志位
        [SerializeField]
        private bool m_EnableOpenUIFormSuccessEvent = true;
        [SerializeField]
        private bool m_EnableOpenUIFormFailureEvent = true;
        [SerializeField]
        private bool m_EnableOpenUIFormUpdateEvent = false;
        [SerializeField]
        private bool m_EnableOpenUIFormDependencyAssetEvent = false;
        [SerializeField]
        private bool m_EnableCloseUIFormCompleteEvent = true;

        //对象池参数
        [SerializeField]
        private float m_InstanceAutoReleaseInterval = 60f;
        [SerializeField]
        private int m_InstanceCapacity = 16;
        [SerializeField]
        private float m_InstanceExpireTime = 60f;
        [SerializeField]
        private int m_InstancePriority = 0;

        [SerializeField]
        private Transform m_InstanceRoot = null;   //根对象

        [SerializeField]
        private string m_UIFormHelperTypeName = "UnityGameFrame.Runtime.DefaultUIFormHelper";   //默认界面辅助器
        [SerializeField]
        private UIFormHelperBase m_CustomUIFormHelper = null;   //自定义界面辅助器
        [SerializeField]
        private string m_UIGroupHelperTypeName = "UnityGameFrame.Runtime.DefaultUIGroupHelper"; //默认界面组辅助器
        [SerializeField]
        private UIGroupHelperBase m_CustomUIGroupHelper = null; //自定义界面组辅助器

        [SerializeField]
        private UIGroup[] m_UIGroups = null;    //界面组，配置用

        /// <summary>
        /// 界面组数量
        /// </summary>
        public int UIGroupCount { get { return m_UIManager.UIGroupCount; } }

        //UI相机
        public Camera UICamera { get; private set; }

        /// <summary>
        /// 获取或设置界面实例对象池自动释放可释放对象的间隔秒数
        /// </summary>
        public float InstanceAutoReleaseInterval
        {
            get { return m_UIManager.InstanceAutoReleaseInterval; }
            set { m_UIManager.InstanceAutoReleaseInterval = m_InstanceAutoReleaseInterval = value; }
        }

        /// <summary>
        /// 获取或设置界面实例对象池的容量
        /// </summary>
        public int InstanceCapacity
        {
            get { return m_UIManager.InstanceCapacity; }
            set { m_UIManager.InstanceCapacity = m_InstanceCapacity = value; }
        }

        /// <summary>
        /// 获取或设置界面实例对象池对象过期秒数
        /// </summary>
        public float InstanceExpireTime
        {
            get { return m_UIManager.InstanceExpireTime; }
            set { m_UIManager.InstanceExpireTime = m_InstanceExpireTime = value; }
        }

        /// <summary>
        /// 获取或设置界面实例对象池的优先级
        /// </summary>
        public int InstancePriority
        {
            get { return m_UIManager.InstancePriority; }
            set { m_UIManager.InstancePriority = m_InstancePriority = value; }
        }


        protected override void Awake()
        {
            base.Awake();
            //获取界面管理器
            m_UIManager = GameFrameworkEntry.GetModule<IUIManager>();
            if (m_UIManager == null)
            {
                Log.Fatal("[UIComponent.Awake] UI manager is invalid.");
                return;
            }

            //注册事件
            m_UIManager.OpenUIFormSuccess += OnOpenUIFormSuccess;
            m_UIManager.OpenUIFormFailure += OnOpenUIFormFailure;
            m_UIManager.OpenUIFormUpdate += OnOpenUIFormUpdate;
            m_UIManager.OpenUIFormDependencyAsset += OnOpenUIFormDependencyAsset;
            m_UIManager.CloseUIFormComplete += OnCloseUIFormComplete;
        }

        private void Start()
        {
            //基础组件
            BaseComponent baseComponent = GameEntry.GetComponent<BaseComponent>();
            if (baseComponent == null)
            {
                Log.Fatal("[UIComponent.Start] Base component is invalid.");
                return;
            }
            m_UIManager.SetResourceManager(baseComponent.ResourceManager);  //设置资源管理器

            //事件
            m_EventComponent = GameEntry.GetComponent<EventComponent>();
            if (m_EventComponent == null)
            {
                Log.Fatal("[UIComponent.Start] Event component is invalid.");
                return;
            }

            //设置对象池
            m_UIManager.SetObjectPoolManager(GameFrameworkEntry.GetModule<IObjectPoolManager>());
            m_UIManager.InstanceAutoReleaseInterval = m_InstanceAutoReleaseInterval;
            m_UIManager.InstanceCapacity = m_InstanceCapacity;
            m_UIManager.InstanceExpireTime = m_InstanceExpireTime;
            m_UIManager.InstancePriority = m_InstancePriority;

            //界面辅助器
            UIFormHelperBase uiFormHelper = Helper.CreateHelper(m_UIFormHelperTypeName, m_CustomUIFormHelper);
            if (uiFormHelper == null)
            {
                Log.Error("[UIComponent.Start] Can not create UI form helper.");
                return;
            }
            uiFormHelper.name = "UI Form Helper";
            Transform trans = uiFormHelper.transform;
            trans.SetParent(this.transform);
            trans.localScale = Vector3.one;
            m_UIManager.SetUIFormHelper(uiFormHelper);  //设置界面辅助器

            //根对象
            if (m_InstanceRoot == null)
            {
                m_InstanceRoot = (new GameObject("UI Form Instances")).transform;
                m_InstanceRoot.SetParent(gameObject.transform);
                m_InstanceRoot.localScale = Vector3.one;
            }

            m_InstanceRoot.gameObject.layer = LayerMask.NameToLayer("UI");  //设置层

            //添加配置的界面组
            for (int i = 0; i < m_UIGroups.Length; i++)
            {
                if (!AddUIGroup(m_UIGroups[i].Name, m_UIGroups[i].Depth))
                {
                    Log.Warning("[UIComponent.Start] Add UI group '{0}' failure.", m_UIGroups[i].Name);
                    continue;
                }
            }

            UICamera = GetComponentInChildren<Camera>();    //获取UI相机
        }

        /// <summary>
        /// 是否存在界面组
        /// </summary>
        /// <param name="uiGroupName">界面组名称</param>
        /// <returns>是否存在界面组</returns>
        public bool HasUIGroup(string uiGroupName)
        {
            return m_UIManager.HasUIGroup(uiGroupName);
        }

        /// <summary>
        /// 获取界面组
        /// </summary>
        /// <param name="uiGroupName">界面组名称</param>
        /// <returns>要获取的界面组</returns>
        public IUIGroup GetUIGroup(string uiGroupName)
        {
            return m_UIManager.GetUIGroup(uiGroupName);
        }

        /// <summary>
        /// 获取所有界面组
        /// </summary>
        /// <returns>所有界面组</returns>
        public IUIGroup[] GetAllUIGroups()
        {
            return m_UIManager.GetAllUIGroups();
        }

        /// <summary>
        /// 获取所有界面组
        /// </summary>
        /// <param name="results">所有界面组</param>
        public void GetAllUIGroups(List<IUIGroup> results)
        {
            m_UIManager.GetAllUIGroups(results);
        }

        /// <summary>
        /// 增加界面组
        /// </summary>
        /// <param name="uiGroupName">界面组名称</param>
        /// <param name="depth">界面组深度</param>
        /// <returns>是否增加界面组成功</returns>
        public bool AddUIGroup(string uiGroupName, int depth = 0)
        {
            if (m_UIManager.HasUIGroup(uiGroupName))
                return false;

            UIGroupHelperBase uiGroupHelper = Helper.CreateHelper(m_UIGroupHelperTypeName, m_CustomUIGroupHelper, UIGroupCount);
            if(uiGroupHelper == null)
            {
                Log.Error("[UIComponent.AddUIGroup] Can not create ui group helper.");
                return false;
            }

            uiGroupHelper.name = Utility.Text.Format("UI Group - {0}", uiGroupName);
            uiGroupHelper.gameObject.layer = LayerMask.NameToLayer("UI");
            Transform trans = uiGroupHelper.transform;
            trans.SetParent(m_InstanceRoot);
            trans.localScale = Vector3.one;

            return m_UIManager.AddUIGroup(uiGroupName, uiGroupHelper);
        }

        /// <summary>
        /// 是否存在界面
        /// </summary>
        /// <param name="serialId">界面序列编号</param>
        /// <returns>是否存在界面</returns>
        public bool HasUIForm(int serialId)
        {
            return m_UIManager.HasUIForm(serialId);
        }

        /// <summary>
        /// 是否存在界面
        /// </summary>
        /// <param name="uiFormAssetName">界面资源名称</param>
        /// <returns>是否存在界面</returns>
        public bool HasUIForm(string uiFormAssetName)
        {
            return m_UIManager.HasUIForm(uiFormAssetName);
        }

        /// <summary>
        /// 获取界面
        /// </summary>
        /// <param name="serialId">界面序列编号</param>
        /// <returns>要获取的界面</returns>
        public UIForm GetUIForm(int serialId)
        {
            return (UIForm)m_UIManager.GetUIForm(serialId);
        }

        /// <summary>
        /// 获取界面
        /// </summary>
        /// <param name="uiFormAssetName">界面资源名称</param>
        /// <returns>要获取的界面</returns>
        public UIForm GetUIForm(string uiFormAssetName)
        {
            return (UIForm)m_UIManager.GetUIForm(uiFormAssetName);
        }

        /// <summary>
        /// 获取界面
        /// </summary>
        /// <param name="uiFormAssetName">界面资源名称</param>
        /// <returns>要获取的界面</returns>
        public UIForm[] GetUIForms(string uiFormAssetName)
        {
            IUIForm[] uiForms = m_UIManager.GetUIForms(uiFormAssetName);
            UIForm[] results = new UIForm[uiForms.Length];
            for (int i = 0; i < uiForms.Length; i++)
            {
                results[i] = (UIForm)uiForms[i];
            }

            return results;
        }

        /// <summary>
        /// 获取界面
        /// </summary>
        /// <param name="uiFormAssetName">界面资源名称</param>
        /// <param name="results">要获取的界面</param>
        public void GetUIForms(string uiFormAssetName, List<UIForm> results)
        {
            if (results == null)
            {
                Log.Error("[UIComponent.GetUIForms] Results is invalid.");
                return;
            }

            results.Clear();
            m_UIManager.GetUIForms(uiFormAssetName, m_InternalUIFormResultsCache);
            foreach (IUIForm uiForm in m_InternalUIFormResultsCache)
            {
                results.Add((UIForm)uiForm);
            }
        }

        /// <summary>
        /// 获取所有已加载的界面
        /// </summary>
        /// <returns>所有已加载的界面</returns>
        public UIForm[] GetAllLoadedUIForms()
        {
            IUIForm[] uiForms = m_UIManager.GetAllLoadedUIForms();
            UIForm[] results = new UIForm[uiForms.Length];
            for (int i = 0; i < uiForms.Length; i++)
            {
                results[i] = (UIForm)uiForms[i];
            }

            return results;
        }

        /// <summary>
        /// 获取所有已加载的界面
        /// </summary>
        /// <param name="results">所有已加载的界面</param>
        public void GetAllLoadedUIForms(List<UIForm> results)
        {
            if (results == null)
            {
                Log.Error("[UIComponent.GetAllLoadedUIForms] Results is invalid.");
                return;
            }

            results.Clear();
            m_UIManager.GetAllLoadedUIForms(m_InternalUIFormResultsCache);
            foreach (IUIForm uiForm in m_InternalUIFormResultsCache)
            {
                results.Add((UIForm)uiForm);
            }
        }

        /// <summary>
        /// 获取所有正在加载界面的序列编号
        /// </summary>
        /// <returns>所有正在加载界面的序列编号</returns>
        public int[] GetAllLoadingUIFormSerialIds()
        {
            return m_UIManager.GetAllLoadingUIFormSerialIds();
        }

        /// <summary>
        /// 获取所有正在加载界面的序列编号
        /// </summary>
        /// <param name="results">所有正在加载界面的序列编号</param>
        public void GetAllLoadingUIFormSerialIds(List<int> results)
        {
            m_UIManager.GetAllLoadingUIFormSerialIds(results);
        }

        /// <summary>
        /// 是否正在加载界面
        /// </summary>
        /// <param name="serialId">界面序列编号</param>
        /// <returns>是否正在加载界面</returns>
        public bool IsLoadingUIForm(int serialId)
        {
            return m_UIManager.IsLoadingUIForm(serialId);
        }

        /// <summary>
        /// 是否正在加载界面
        /// </summary>
        /// <param name="uiFormAssetName">界面资源名称</param>
        /// <returns>是否正在加载界面</returns>
        public bool IsLoadingUIForm(string uiFormAssetName)
        {
            return m_UIManager.IsLoadingUIForm(uiFormAssetName);
        }

        /// <summary>
        /// 是否是合法的界面
        /// </summary>
        /// <param name="uiForm">界面</param>
        /// <returns>界面是否合法</returns>
        public bool IsValidUIForm(UIForm uiForm)
        {
            return m_UIManager.IsValidUIForm(uiForm);
        }

        /// <summary>
        /// 打开界面
        /// </summary>
        /// <param name="uiFormAssetName">界面资源名称</param>
        /// <param name="uiGroupName">界面组名称</param>
        /// <param name="userData">用户自定义数据</param>
        /// <returns>界面的序列编号</returns>
        public int OpenUIForm(string uiFormAssetName, string uiGroupName, object userData = null)
        {
            return OpenUIForm(uiFormAssetName, uiGroupName, 0, false, userData);
        }

        /// <summary>
        /// 打开界面
        /// </summary>
        /// <param name="uiFormAssetName">界面资源名称</param>
        /// <param name="uiGroupName">界面组名称</param>
        /// <param name="priority">加载界面资源的优先级</param>
        /// <param name="userData">用户自定义数据</param>
        /// <returns>界面的序列编号</returns>
        public int OpenUIForm(string uiFormAssetName, string uiGroupName, int priority, object userData = null)
        {
            return OpenUIForm(uiFormAssetName, uiGroupName, priority, false, userData);
        }


        /// <summary>
        /// 打开界面
        /// </summary>
        /// <param name="uiFormAssetName">界面资源名称</param>
        /// <param name="uiGroupName">界面组名称</param>
        /// <param name="pauseCoveredUIForm">是否暂停被覆盖的界面</param>
        /// <param name="userData">用户自定义数据</param>
        /// <returns>界面的序列编号</returns>
        public int OpenUIForm(string uiFormAssetName, string uiGroupName, bool pauseCoveredUIForm, object userData = null)
        {
            return OpenUIForm(uiFormAssetName, uiGroupName, 0, pauseCoveredUIForm, userData);
        }

        /// <summary>
        /// 打开界面
        /// </summary>
        /// <param name="uiFormAssetName">界面资源名称</param>
        /// <param name="uiGroupName">界面组名称</param>
        /// <param name="priority">加载界面资源的优先级</param>
        /// <param name="pauseCoveredUIForm">是否暂停被覆盖的界面</param>
        /// <param name="userData">用户自定义数据</param>
        /// <returns>界面的序列编号</returns>
        public int OpenUIForm(string uiFormAssetName, string uiGroupName, int priority, bool pauseCoveredUIForm, object userData = null)
        {
            return m_UIManager.OpenUIForm(uiFormAssetName, uiGroupName, priority, pauseCoveredUIForm, userData);
        }

        /// <summary>
        /// 关闭界面
        /// </summary>
        /// <param name="serialId">要关闭界面的序列编号</param>
        /// <param name="userData">用户自定义数据</param>
        public void CloseUIForm(int serialId, object userData = null)
        {
            m_UIManager.CloseUIForm(serialId, userData);
        }

        /// <summary>
        /// 关闭界面
        /// </summary>
        /// <param name="uiForm">要关闭的界面</param>
        /// <param name="userData">用户自定义数据</param>
        public void CloseUIForm(UIForm uiForm, object userData = null)
        {
            m_UIManager.CloseUIForm(uiForm, userData);
        }

        /// <summary>
        /// 关闭所有已加载的界面
        /// </summary>
        public void CloseAllLoadedUIForms()
        {
            m_UIManager.CloseAllLoadedUIForms();
        }

        /// <summary>
        /// 关闭所有已加载的界面
        /// </summary>
        /// <param name="userData">用户自定义数据</param>
        public void CloseAllLoadedUIForms(object userData)
        {
            m_UIManager.CloseAllLoadedUIForms(userData);
        }

        /// <summary>
        /// 关闭所有正在加载的界面
        /// </summary>
        public void CloseAllLoadingUIForms()
        {
            m_UIManager.CloseAllLoadingUIForms();
        }

        /// <summary>
        /// 激活界面
        /// </summary>
        /// <param name="uiForm">要激活的界面</param>
        /// <param name="userData">用户自定义数据</param>
        public void RefocusUIForm(UIForm uiForm, object userData = null)
        {
            m_UIManager.RefocusUIForm(uiForm, userData);
        }

        /// <summary>
        /// 设置界面是否被加锁
        /// </summary>
        /// <param name="uiForm">要设置是否被加锁的界面</param>
        /// <param name="locked">界面是否被加锁</param>
        public void SetUIFormInstanceLocked(UIForm uiForm, bool locked)
        {
            if (uiForm == null)
            {
                Log.Warning("[UIComponent.SetUIFormInstanceLocked] UI form is invalid.");
                return;
            }

            m_UIManager.SetUIFormInstanceLocked(uiForm.gameObject, locked);
        }

        /// <summary>
        /// 设置界面的优先级
        /// </summary>
        /// <param name="uiForm">要设置优先级的界面</param>
        /// <param name="priority">界面优先级</param>
        public void SetUIFormInstancePriority(UIForm uiForm, int priority)
        {
            if (uiForm == null)
            {
                Log.Warning("[UIComponent.SetUIFormInstancePriority] UI form is invalid.");
                return;
            }

            m_UIManager.SetUIFormInstancePriority(uiForm.gameObject, priority);
        }

        //打开界面成功的回调
        private void OnOpenUIFormSuccess(object sender, GameFramework.UI.OpenUIFormSuccessEventArgs e)
        {
            if (m_EnableOpenUIFormSuccessEvent)
            {
                m_EventComponent.Fire(this, ReferencePool.Acquire<OpenUIFormSuccessEventArgs>().Fill(e));
            }
        }

        //打开界面失败的回调
        private void OnOpenUIFormFailure(object sender, GameFramework.UI.OpenUIFormFailureEventArgs e)
        {
            Log.Warning("Open UI form failure, asset name '{0}', UI group name '{1}', pause covered UI form '{2}', error message '{3}'.", e.UIFormAssetName, e.UIGroupName, e.PauseCoveredUIForm.ToString(), e.ErrorMessage);
            if (m_EnableOpenUIFormFailureEvent)
            {
                m_EventComponent.Fire(this, ReferencePool.Acquire<OpenUIFormFailureEventArgs>().Fill(e));
            }
        }

        //打开界面更新的回调
        private void OnOpenUIFormUpdate(object sender, GameFramework.UI.OpenUIFormUpdateEventArgs e)
        {
            if (m_EnableOpenUIFormUpdateEvent)
            {
                m_EventComponent.Fire(this, ReferencePool.Acquire<OpenUIFormUpdateEventArgs>().Fill(e));
            }
        }

        //打开界面时加载依赖资源的会滴
        private void OnOpenUIFormDependencyAsset(object sender, GameFramework.UI.OpenUIFormDependencyAssetEventArgs e)
        {
            if (m_EnableOpenUIFormDependencyAssetEvent)
            {
                m_EventComponent.Fire(this, ReferencePool.Acquire<OpenUIFormDependencyAssetEventArgs>().Fill(e));
            }
        }

        private void OnCloseUIFormComplete(object sender, GameFramework.UI.CloseUIFormCompleteEventArgs e)
        {
            if (m_EnableCloseUIFormCompleteEvent)
            {
                m_EventComponent.Fire(this, ReferencePool.Acquire<CloseUIFormCompleteEventArgs>().Fill(e));
            }
        }

    }
}
