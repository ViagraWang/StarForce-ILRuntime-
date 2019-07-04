using Game.Runtime;
using System;
using System.Collections.Generic;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace Game.Hotfix
{
    /// <summary>
    /// 流程逻辑
    /// </summary>
    public abstract class ProcedureLogic
    {
        /// <summary>
        /// 所绑定的Unity中HotProcedure
        /// </summary>
        public HotProcedure RuntimeProcedure { get; private set; }

        /// <summary>
        /// 流程初始化时调用
        /// </summary>
        /// <param name="procedureOwner">流程持有者</param>
        /// <param name="procedureBind">绑定的Runtime流程</param>
        public virtual void OnInit(ProcedureOwner procedureOwner, HotProcedure procedureBind)
        {
            RuntimeProcedure = procedureBind;
        }

        /// <summary>
        /// 进入状态时调用
        /// </summary>
        /// <param name="procedureOwner">流程持有者</param>
        public abstract void OnEnter(ProcedureOwner procedureOwner);

        /// <summary>
        /// 状态轮询更新
        /// </summary>
        /// <param name="procedureOwner">流程持有者</param>
        public abstract void OnUpdate(ProcedureOwner procedureOwner);

        /// <summary>
        /// 状态离开时调用
        /// </summary>
        /// <param name="procedureOwner">流程持有者</param>
        /// <param name="isShutdown">是否关闭状态机</param>
        public abstract void OnLeave(ProcedureOwner procedureOwner, bool isShutdown);

        /// <summary>
        /// 销毁时调用
        /// </summary>
        /// <param name="procedureManager">流程持有者</param>
        public abstract void OnDestroy(ProcedureOwner procedureManager);

    }
}
