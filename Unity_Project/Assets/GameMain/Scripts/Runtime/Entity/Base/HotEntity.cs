//=======================================================
// 作者：
// 描述：热更新的实体逻辑中介
//=======================================================
using GameFramework;
using UnityEngine;
using UnityGameFrame.Runtime;

namespace Game.Runtime
{
    [DisallowMultipleComponent]
    public class HotEntity : Entity
    {
        /// <summary>
        /// 控制世界位置变化
        /// </summary>
        public Vector3 Position
        {
            get { return CachedTransform.position; }
            set { CachedTransform.position = value; }
        }

        /// <summary>
        /// 控制局部位置变化
        /// </summary>
        public Vector3 LocalPosition
        {
            get { return CachedTransform.localPosition; }
            set { CachedTransform.localPosition = value; }
        }

        /// <summary>
        /// 控制世界旋转
        /// </summary>
        public Quaternion Rotation
        {
            get { return CachedTransform.rotation; }
            set { CachedTransform.rotation = value; }
        }

        /// <summary>
        /// 控制局部旋转
        /// </summary>
        public Quaternion LocalRotation
        {
            get { return CachedTransform.localRotation; }
            set { CachedTransform.localRotation = value; }
        }

        public ReferenceCollector ReferenceCollector { get; private set; }

        //自定义实体数据
        public UserEntityData UserEntityData { get; private set; }

        public object HotLogicInstance { get; private set; }    //热更的逻辑实例

        [SerializeField]
        private string HotLogicScript;  //保存当前的热更脚本

        //实例方法
        private InstanceMethod OnInitMethod = null;
        private InstanceMethod OnShowMethod = null;
        private InstanceMethod OnHideMethod = null;
        private InstanceMethod OnAttachedMethod = null;
        private InstanceMethod OnDetachedMethod = null;
        private InstanceMethod OnAttachToMethod = null;
        private InstanceMethod OnDetachFromMethod = null;
        private InstanceMethod OnUpdateMethod = null;
        private InstanceMethod InternalSetVisibleMethod = null;
        private InstanceMethod OnTriggerEnterMethod = null;
        private InstanceMethod OnTriggerExitMethod = null;

        private void Awake()
        {
            ReferenceCollector = GetComponent<ReferenceCollector>();
            if (ReferenceCollector != null)
                ReferenceCollector.ComponentView.Component = this;
        }

        //初始化逻辑
        private void InitLogicData(UserEntityData data)
        {
            HotLogicInstance = GameEntry.Hotfix.CreateInstance(data.HotLogicTypeFullName, null);  //创建实例
            HotLogicScript = data.HotLogicTypeFullName;

#if ILRuntime

            //获取方法
            OnInitMethod = ReferencePool.Acquire<ILInstanceMethod>().Fill(HotLogicInstance, data.HotLogicTypeFullName, "OnInit", 1);
            OnShowMethod = ReferencePool.Acquire<ILInstanceMethod>().Fill(HotLogicInstance, data.HotLogicTypeFullName, "OnShow", 1);
            OnHideMethod = ReferencePool.Acquire<ILInstanceMethod>().Fill(HotLogicInstance, data.HotLogicTypeFullName, "OnHide", 1);
            OnUpdateMethod = ReferencePool.Acquire<ILInstanceMethod>().Fill(HotLogicInstance, data.HotLogicTypeFullName, "OnUpdate", 0);
            OnAttachedMethod = ReferencePool.Acquire<ILInstanceMethod>().Fill(HotLogicInstance, data.HotLogicTypeFullName, "OnAttached", 3);
            OnDetachedMethod = ReferencePool.Acquire<ILInstanceMethod>().Fill(HotLogicInstance, data.HotLogicTypeFullName, "OnDetached", 2);
            OnAttachToMethod = ReferencePool.Acquire<ILInstanceMethod>().Fill(HotLogicInstance, data.HotLogicTypeFullName, "OnAttachTo", 3);
            OnDetachFromMethod = ReferencePool.Acquire<ILInstanceMethod>().Fill(HotLogicInstance, data.HotLogicTypeFullName, "OnDetachFrom", 2);
            InternalSetVisibleMethod = ReferencePool.Acquire<ILInstanceMethod>().Fill(HotLogicInstance, data.HotLogicTypeFullName, "InternalSetVisible", 1);
            OnTriggerEnterMethod = ReferencePool.Acquire<ILInstanceMethod>().Fill(HotLogicInstance, data.HotLogicTypeFullName, "OnTriggerEnter", 1);
            OnTriggerExitMethod = ReferencePool.Acquire<ILInstanceMethod>().Fill(HotLogicInstance, data.HotLogicTypeFullName, "OnTriggerExit", 1);

#else

            //获取方法
            OnInitMethod = ReferencePool.Acquire<ReflectInstanceMethod>().Fill(HotLogicInstance, data.HotLogicTypeFullName, "OnInit");
            OnShowMethod = ReferencePool.Acquire<ReflectInstanceMethod>().Fill(HotLogicInstance, data.HotLogicTypeFullName, "OnShow");
            OnHideMethod = ReferencePool.Acquire<ReflectInstanceMethod>().Fill(HotLogicInstance, data.HotLogicTypeFullName, "OnHide");
            OnUpdateMethod = ReferencePool.Acquire<ReflectInstanceMethod>().Fill(HotLogicInstance, data.HotLogicTypeFullName, "OnUpdate");
            OnAttachedMethod = ReferencePool.Acquire<ReflectInstanceMethod>().Fill(HotLogicInstance, data.HotLogicTypeFullName, "OnAttached");
            OnDetachedMethod = ReferencePool.Acquire<ReflectInstanceMethod>().Fill(HotLogicInstance, data.HotLogicTypeFullName, "OnDetached");
            OnAttachToMethod = ReferencePool.Acquire<ReflectInstanceMethod>().Fill(HotLogicInstance, data.HotLogicTypeFullName, "OnAttachTo");
            OnDetachFromMethod = ReferencePool.Acquire<ReflectInstanceMethod>().Fill(HotLogicInstance, data.HotLogicTypeFullName, "OnDetachFrom");
            InternalSetVisibleMethod = ReferencePool.Acquire<ReflectInstanceMethod>().Fill(HotLogicInstance, data.HotLogicTypeFullName, "InternalSetVisible");
            OnTriggerEnterMethod = ReferencePool.Acquire<ReflectInstanceMethod>().Fill(HotLogicInstance, data.HotLogicTypeFullName, "OnTriggerEnter");
            OnTriggerExitMethod = ReferencePool.Acquire<ReflectInstanceMethod>().Fill(HotLogicInstance, data.HotLogicTypeFullName, "OnTriggerExit");

#endif

            if (OnInitMethod.IsAvalible)
                OnInitMethod.Run(this);
        }

        //释放资源
        private void ReleaseLogicData()
        {
            if (HotLogicInstance != null)
            {
                //Log.Info(Utility.Text.Format("实体逻辑Release -> {0}", HotLogicScript));
                if (UserEntityData != null)
                {
                    //ReferencePool.Release(UserEntityData);  //回收
                    UserEntityData = null;
                }
                ReferencePool.Release((IReference)OnInitMethod);
                ReferencePool.Release((IReference)OnShowMethod);
                ReferencePool.Release((IReference)OnHideMethod);
                ReferencePool.Release((IReference)OnAttachedMethod);
                ReferencePool.Release((IReference)OnDetachedMethod);
                ReferencePool.Release((IReference)OnAttachToMethod);
                ReferencePool.Release((IReference)OnDetachFromMethod);
                ReferencePool.Release((IReference)OnUpdateMethod);
                ReferencePool.Release((IReference)InternalSetVisibleMethod);
                ReferencePool.Release((IReference)OnTriggerEnterMethod);
                ReferencePool.Release((IReference)OnTriggerExitMethod);

                HotLogicInstance = null;

                OnInitMethod = null;
                OnShowMethod = null;
                OnHideMethod = null;
                OnAttachedMethod = null;
                OnDetachedMethod = null;
                OnAttachToMethod = null;
                OnDetachFromMethod = null;
                OnUpdateMethod = null;
                InternalSetVisibleMethod = null;
                OnTriggerEnterMethod = null;
                OnTriggerExitMethod = null;
            }

        }

        /// <summary>
        /// 初始化回调
        /// </summary>
        /// <param name="userData">用户自定义数据</param>
        protected override void OnInit(UnityGameFrame.Runtime.Entity entity, object userData)
        {
            base.OnInit(entity, userData);

            UserEntityData = userData as UserEntityData;
            UserEntityData.RuntimeEntity = this;
            InitLogicData(UserEntityData);

            //Log.Info(Utility.Text.Format("实体逻辑Init -> {0}", HotLogicScript));

        }

        /// <summary>
        /// 隐藏时回调
        /// </summary>
        /// <param name="userData">用户自定义数据</param>
        protected override void OnHide(object userData)
        {
            base.OnHide(userData);

            if (OnHideMethod.IsAvalible)
                OnHideMethod.Run(userData);

            if(UserEntityData != null)
            {
                //ReferencePool.Release(UserEntityData);  //回收
                UserEntityData = null;
            }
        }

        /// <summary>
        /// 实体显示
        /// </summary>
        /// <param name="userData">用户自定义数据</param>
        protected override void OnShow(object userData)
        {
            base.OnShow(userData);

            UserEntityData data = userData as UserEntityData;   //临时缓存，避免下面ReleaseLogicData时Release
            //Log.Info($"显示实体OnShow -> {data.HotLogicTypeFullName} ={HotLogicScript}");
            if(UserEntityData != data)
            {
                data.RuntimeEntity = this;
                if (data.HotLogicTypeFullName != HotLogicScript)   //检查逻辑类名是否一样
                {
                    ReleaseLogicData(); //释放保存的Hot逻辑
                    InitLogicData(data); //重新初始化Hot逻辑，并调用Init方法
                }

                UserEntityData = data;
            }

            if (OnShowMethod.IsAvalible)
                OnShowMethod.Run(UserEntityData.UserData);
        }

        /// <summary>
        /// 实体附加子实体
        /// </summary>
        /// <param name="childEntity">附加的子实体</param>
        /// <param name="parentTransform">被附加的父实体</param>
        /// <param name="userData">用户自定义数据</param>
        protected override void OnAttached(EntityLogic childEntity, Transform parentTransform, object userData)
        {
            base.OnAttached(childEntity, parentTransform, userData);
            if (OnAttachedMethod.IsAvalible)
                OnAttachedMethod.Run((childEntity as HotEntity).HotLogicInstance, parentTransform, userData);
        }

        /// <summary>
        /// 实体解除子实体
        /// </summary>
        /// <param name="childEntity">解除的子实体</param>
        /// <param name="userData">用户自定义数据</param>
        protected override void OnDetached(EntityLogic childEntity, object userData)
        {
            base.OnDetached(childEntity, userData);
            if (OnDetachedMethod.IsAvalible)
                OnDetachedMethod.Run((childEntity as HotEntity).HotLogicInstance, userData);
        }

        /// <summary>
        /// 实体附加子实体
        /// </summary>
        /// <param name="parentEntity">被附加的父实体</param>
        /// <param name="parentTransform">被附加父实体的位置</param>
        /// <param name="userData">用户自定义数据</param>
        protected override void OnAttachTo(EntityLogic parentEntity, Transform parentTransform, object userData)
        {
            base.OnAttachTo(parentEntity, parentTransform, userData);
            if (OnAttachToMethod.IsAvalible)
                OnAttachToMethod.Run((parentEntity as HotEntity).HotLogicInstance, parentTransform, userData);
        }

        /// <summary>
        /// 实体解除子实体
        /// </summary>
        /// <param name="childEntity">被解除的父实体</param>
        /// <param name="userData">用户自定义数据</param>
        protected override void OnDetachFrom(EntityLogic parentEntity, object userData)
        {
            base.OnDetachFrom(parentEntity, userData);
            if (OnDetachFromMethod.IsAvalible)
                OnDetachFromMethod.Run((parentEntity as HotEntity).HotLogicInstance, userData);
        }

        /// <summary>
        /// 实体轮询
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位</param>
        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            if (OnUpdateMethod.IsAvalible)
                OnUpdateMethod.Run();
        }

        /// <summary>
        /// 设置实体的可见性
        /// </summary>
        /// <param name="visible">实体的可见性</param> 
        protected override void InternalSetVisible(bool visible)
        {
            base.InternalSetVisible(visible);
            if (InternalSetVisibleMethod.IsAvalible)
                InternalSetVisibleMethod.Run(visible);
        }


        private void OnTriggerEnter(Collider other)
        {
            if (OnTriggerEnterMethod.IsAvalible)
                OnTriggerEnterMethod.Run(other.GetComponent<HotEntity>());
        }

        private void OnTriggerExit(Collider other)
        {
            if (OnTriggerExitMethod.IsAvalible)
                OnTriggerExitMethod.Run(other.GetComponent<HotEntity>());
        }

        protected virtual void OnDestroy()
        {
            ReleaseLogicData();
        }

    }
}
