//=======================================================
// 作者：
// 描述：调用热更新流程逻辑的基类，因为流程不会频繁增减，一般在大版本更新中增减，所以使用Unity中的流程框架，逻辑调用Hotfix中的。
//也可以在Hotfix中单独创建一个流程管理器

//这里说明下为何没有在Hotfix程序中使用继承ProcedureBase的方式：
//因为每一个流程都是继承自FsmState，其实就是有限状态机的一个状态，所有的流程状态是需要在流程管理器初始化时全部添加进去。
//而状态的存储使用的是Dictionary，其中Type.GetType().FullName作为key，那么只有保存不同class的Type，不然会存在相同的key而报异常。
//Hotfix中继承Unity的class，需要写适配器Adaptor，那么Hotfix中所有的流程保存到ProcedureManager中时，其实保存的是适配器（因为适配器也要继承ProcedureBase）
//那么肯定只有一种Type，保存到Dictionary中时肯定会报Key重复
//=======================================================

using GameFramework;
using GameFramework.Fsm;
using GameFramework.Procedure;

namespace Game.Runtime
{	
    /// <summary>
    /// 热更新的流程
    /// </summary>
	public abstract class HotProcedure : GameFramework.Procedure.ProcedureBase
    {
        /// <summary>
        /// 是否使用本地对话框
        /// </summary>
        public abstract bool UseNativeDialog { get; }

        public object HotLogicInstance { get; private set; }    //热更的逻辑实例

        //实例方法，因为只有update方法是频繁调用的，所以只缓存Update的方法，其他方法只在调用时处理即可
        private InstanceMethod OnUpdateMethod = null;

        /// <summary>
        /// 热更新流程逻辑的类型名称
        /// </summary>
        public abstract string HotProcedureLogicTypeFullName { get; }

        /// <summary>
        /// 流程初始化时调用
        /// </summary>
        /// <param name="procedureOwner">流程持有者</param>
        protected override void OnInit(IFsm<IProcedureManager> procedureOwner)
        {
            base.OnInit(procedureOwner);
        }

        /// <summary>
        /// 进入流程时调用
        /// </summary>
        /// <param name="procedureOwner">流程持有者</param>
        protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
        {
            base.OnEnter(procedureOwner);

            if(HotLogicInstance == null)
            {
                HotLogicInstance = GameEntry.Hotfix.CreateInstance(HotProcedureLogicTypeFullName, null);  //创建热更逻辑实例
                GameEntry.Hotfix.InvokeOne(HotProcedureLogicTypeFullName, "OnInit", HotLogicInstance, procedureOwner, this); //调用初始化方法

                //获取更新方法
#if ILRuntime

            OnUpdateMethod = ReferencePool.Acquire<ILInstanceMethod>().Fill(HotLogicInstance, HotProcedureLogicTypeFullName, "OnUpdate", 1);

#else

                OnUpdateMethod = ReferencePool.Acquire<ReflectInstanceMethod>().Fill(HotLogicInstance, HotProcedureLogicTypeFullName, "OnUpdate");

#endif
            }

            //调用进入流程的方法
            GameEntry.Hotfix.InvokeOne(HotProcedureLogicTypeFullName, "OnEnter", HotLogicInstance, procedureOwner);

        }

        /// <summary>
        /// 流程轮询更新
        /// </summary>
        /// <param name="procedureOwner">流程持有者</param>
        /// <param name="elapseSeconds">逻辑帧时间</param>
        /// <param name="realElapseSeconds">真实流逝时间</param>
        protected override void OnUpdate(IFsm<IProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            if (OnUpdateMethod.IsAvalible)
                OnUpdateMethod.Run(procedureOwner);
        }

        /// <summary>
        /// 流程离开时调用
        /// </summary>
        /// <param name="procedureOwner">流程持有者</param>
        /// <param name="isShutdown">是否关闭状态机</param>
        protected override void OnLeave(IFsm<IProcedureManager> procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);

            //调用离开流程的方法
            GameEntry.Hotfix.InvokeOne(HotProcedureLogicTypeFullName, "OnLeave", HotLogicInstance, procedureOwner, isShutdown);
        }

        /// <summary>
        /// 销毁时调用
        /// </summary>
        /// <param name="procedureOwner">流程持有者</param>
        protected override void OnDestroy(IFsm<IProcedureManager> procedureOwner)
        {
            base.OnDestroy(procedureOwner);
            GameEntry.Hotfix.InvokeOne(HotProcedureLogicTypeFullName, "OnDestroy", HotLogicInstance, procedureOwner); //调用销毁方法

            if(OnUpdateMethod != null)
            {
                ReferencePool.Release(OnUpdateMethod);  //回收
                OnUpdateMethod = null;
            }

        }

        /// <summary>
        /// 切换流程，主要供Hotfix中ProcedureLogic使用
        /// </summary>
        /// <typeparam name="TProcedure">要切换的流程</typeparam>
        /// <param name="procedureOwner">流程管理器</param>
        public void ChangeProcedure<TProcedure>(IFsm<IProcedureManager> procedureOwner) where TProcedure : HotProcedure
        {
            ChangeState<TProcedure>(procedureOwner);
        }

    }
}