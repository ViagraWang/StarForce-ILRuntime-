//=======================================================
// 作者：
// 描述：热更新的UI界面逻辑
//=======================================================
using GameFramework;
using UnityEngine;
using UnityGameFrame.Runtime;

namespace Game.Runtime
{
    [DisallowMultipleComponent]
    public sealed partial class HotUIForm : UGUIForm
    {
        public ReferenceCollector ReferenceCollector { get; private set; } = null; //组件对象容器

        [SerializeField]
        private string HotLogicScript;    //热更新的逻辑

        //UI界面数据
        public UserUIData UserUIData { get; private set; }

        public object HotLogicInstance { get; private set; }    //热更的逻辑实例

        //实例方法
        private InstanceMethod OnInitMethod = null;
        private InstanceMethod OnOpenMethod = null;
        private InstanceMethod OnUpdateMethod = null;
        private InstanceMethod OnRefocusMethod = null;
        private InstanceMethod OnResumeMethod = null;
        private InstanceMethod OnPauseMethod = null;
        private InstanceMethod OnCoverMethod = null;
        private InstanceMethod OnRevealMethod = null;
        private InstanceMethod OnCloseMethod = null;
        private InstanceMethod OnDepthChangedMethod = null;

        private void Awake()
        {
            ReferenceCollector = GetComponent<ReferenceCollector>();
            if (ReferenceCollector != null)
                ReferenceCollector.ComponentView.Component = this;
        }

        //初始化逻辑数据
        private void InitLogicData(UserUIData data)
        {
            HotLogicInstance = GameEntry.Hotfix.CreateInstance(UserUIData.HotLogicTypeFullName, null);  //创建实例
            HotLogicScript = UserUIData.HotLogicTypeFullName;

#if ILRuntime

            //获取方法
            OnInitMethod = ReferencePool.Acquire<ILInstanceMethod>().Fill(HotLogicInstance, UserUIData.HotLogicTypeFullName, "OnInit", 1);
            OnOpenMethod = ReferencePool.Acquire<ILInstanceMethod>().Fill(HotLogicInstance, UserUIData.HotLogicTypeFullName, "OnOpen", 1);
            OnUpdateMethod = ReferencePool.Acquire<ILInstanceMethod>().Fill(HotLogicInstance, UserUIData.HotLogicTypeFullName, "OnUpdate", 0);
            OnRefocusMethod = ReferencePool.Acquire<ILInstanceMethod>().Fill(HotLogicInstance, UserUIData.HotLogicTypeFullName, "OnRefocus", 1);
            OnResumeMethod = ReferencePool.Acquire<ILInstanceMethod>().Fill(HotLogicInstance, UserUIData.HotLogicTypeFullName, "OnResume", 0);
            OnPauseMethod = ReferencePool.Acquire<ILInstanceMethod>().Fill(HotLogicInstance, UserUIData.HotLogicTypeFullName, "OnPause", 0);
            OnCoverMethod = ReferencePool.Acquire<ILInstanceMethod>().Fill(HotLogicInstance, UserUIData.HotLogicTypeFullName, "OnCover", 0);
            OnRevealMethod = ReferencePool.Acquire<ILInstanceMethod>().Fill(HotLogicInstance, UserUIData.HotLogicTypeFullName, "OnReveal", 0);
            OnCloseMethod = ReferencePool.Acquire<ILInstanceMethod>().Fill(HotLogicInstance, UserUIData.HotLogicTypeFullName, "OnClose", 1);
            OnDepthChangedMethod = ReferencePool.Acquire<ILInstanceMethod>().Fill(HotLogicInstance, UserUIData.HotLogicTypeFullName, "OnDepthChanged", 2);

#else

            //获取方法
            OnInitMethod = ReferencePool.Acquire<ReflectInstanceMethod>().Fill(HotLogicInstance, UserUIData.HotLogicTypeFullName, "OnInit");
            OnOpenMethod = ReferencePool.Acquire<ReflectInstanceMethod>().Fill(HotLogicInstance, UserUIData.HotLogicTypeFullName, "OnOpen");
            OnUpdateMethod = ReferencePool.Acquire<ReflectInstanceMethod>().Fill(HotLogicInstance, UserUIData.HotLogicTypeFullName, "OnUpdate");
            OnRefocusMethod = ReferencePool.Acquire<ReflectInstanceMethod>().Fill(HotLogicInstance, UserUIData.HotLogicTypeFullName, "OnRefocus");
            OnResumeMethod = ReferencePool.Acquire<ReflectInstanceMethod>().Fill(HotLogicInstance, UserUIData.HotLogicTypeFullName, "OnResume");
            OnPauseMethod = ReferencePool.Acquire<ReflectInstanceMethod>().Fill(HotLogicInstance, UserUIData.HotLogicTypeFullName, "OnPause");
            OnCoverMethod = ReferencePool.Acquire<ReflectInstanceMethod>().Fill(HotLogicInstance, UserUIData.HotLogicTypeFullName, "OnCover");
            OnRevealMethod = ReferencePool.Acquire<ReflectInstanceMethod>().Fill(HotLogicInstance, UserUIData.HotLogicTypeFullName, "OnReveal");
            OnCloseMethod = ReferencePool.Acquire<ReflectInstanceMethod>().Fill(HotLogicInstance, UserUIData.HotLogicTypeFullName, "OnClose");
            OnDepthChangedMethod = ReferencePool.Acquire<ReflectInstanceMethod>().Fill(HotLogicInstance, UserUIData.HotLogicTypeFullName, "OnDepthChanged");

#endif

            if (OnInitMethod.IsAvalible)
                OnInitMethod.Run(UserUIData);
        }

        //释放逻辑数据
        private void ReleaseLogicData()
        {
            if(HotLogicInstance != null)
            {
                HotLogicInstance = null;
                if (UserUIData != null)
                {
                    //ReferencePool.Release(UserUIData);  //回收
                    UserUIData = null;
                }

                ReferencePool.Release((IReference)OnInitMethod);
                ReferencePool.Release((IReference)OnOpenMethod);
                ReferencePool.Release((IReference)OnUpdateMethod);
                ReferencePool.Release((IReference)OnRefocusMethod);
                ReferencePool.Release((IReference)OnResumeMethod);
                ReferencePool.Release((IReference)OnPauseMethod);
                ReferencePool.Release((IReference)OnCoverMethod);
                ReferencePool.Release((IReference)OnRevealMethod);
                ReferencePool.Release((IReference)OnCloseMethod);
                ReferencePool.Release((IReference)OnDepthChangedMethod);

                OnInitMethod = null;
                OnOpenMethod = null;
                OnUpdateMethod = null;
                OnRefocusMethod = null;
                OnResumeMethod = null;
                OnPauseMethod = null;
                OnCoverMethod = null;
                OnRevealMethod = null;
                OnCloseMethod = null;
                OnDepthChangedMethod = null;
            }
        }

        //界面初始化
        protected override void OnInit(UIForm uiform, object userData)
        {
            base.OnInit(uiform, userData);

            UserUIData = userData as UserUIData;
            UserUIData.RuntimeUIForm = this;
            InitLogicData(UserUIData);
        }

        //界面打开
        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            UserUIData data = userData as UserUIData;   //临时缓存，避免下面ReleaseLogicData时Release
            if (UserUIData != data)
            {
                data.RuntimeUIForm = this;
                if (data.HotLogicTypeFullName != HotLogicScript)   //检查逻辑类名是否一样
                {
                    ReleaseLogicData(); //释放保存的Hot逻辑
                    InitLogicData(data); //重新初始化Hot逻辑，并调用Init方法
                }

                UserUIData = data;
            }

            if (OnOpenMethod.IsAvalible)
                OnOpenMethod.Run(UserUIData.UserData);

        }

        //界面更新
        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);

            if (OnUpdateMethod.IsAvalible)
                OnUpdateMethod.Run();
        }

        //界面重新激活
        protected override void OnRefocus(object userData)
        {
            base.OnRefocus(userData);

            if (OnRefocusMethod.IsAvalible)
                OnRefocusMethod.Run(userData);
        }

        //界面暂停恢复
        protected override void OnResume()
        {
            base.OnResume();

            if (OnResumeMethod.IsAvalible)
                OnResumeMethod.Run();
        }

        //界面暂停
        protected override void OnPause()
        {
            base.OnPause();

            if (OnPauseMethod.IsAvalible)
                OnPauseMethod.Run();
        }

        //界面被遮挡覆盖
        protected override void OnCover()
        {
            base.OnCover();

            if (OnCoverMethod.IsAvalible)
                OnCoverMethod.Run();
        }

        //界面遮挡恢复
        protected override void OnReveal()
        {
            base.OnReveal();

            if (OnRevealMethod.IsAvalible)
                OnRevealMethod.Run();
        }

        //界面关闭的回调
        protected override void OnClose(object userData)
        {
            base.OnClose(userData);
            //Log.Info(Utility.Text.Format("UI界面OnClose ->{0}", UserUIData.HotLogicTypeFullName));

            if (OnCloseMethod.IsAvalible)
                OnCloseMethod.Run(userData);

            if (UserUIData != null)
            {
                //ReferencePool.Release(UserUIData);  //回收
                UserUIData = null;
            }
        }

        //界面深度改变
        protected override void OnDepthChanged(int uiGroupDepth, int depthInUIGroup)
        {
            base.OnDepthChanged(uiGroupDepth, depthInUIGroup);

            if (OnDepthChangedMethod.IsAvalible)
                OnDepthChangedMethod.Run(uiGroupDepth, depthInUIGroup);
        }

        private void OnDestroy()
        {
            ReleaseLogicData();
        }
    }
}