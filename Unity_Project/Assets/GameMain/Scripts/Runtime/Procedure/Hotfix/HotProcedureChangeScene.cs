
using GameFramework.Fsm;
using GameFramework.Procedure;
using UnityEngine;

namespace Game.Runtime
{
	//切换场景的流程
	public class HotProcedureChangeScene : HotProcedure
	{
	    public override bool UseNativeDialog { get { return false; } }

        private string m_HotProcedureLogicTypeFullName = "ProcedureChangeScene".HotFixTypeFullName();
        public override string HotProcedureLogicTypeFullName { get { return m_HotProcedureLogicTypeFullName; } }

    }
}
