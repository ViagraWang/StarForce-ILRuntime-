namespace Game.Runtime {	
	//主流程类
	public class HotProcedureMain : HotProcedure
	{
	    public override bool UseNativeDialog { get { return false; } }

        private string m_HotProcedureLogicTypeFullName = "ProcedureMain".HotFixTypeFullName();
        public override string HotProcedureLogicTypeFullName { get { return m_HotProcedureLogicTypeFullName; } }

	}
}
