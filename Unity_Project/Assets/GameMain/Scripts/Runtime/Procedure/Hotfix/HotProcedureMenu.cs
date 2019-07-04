
namespace Game.Runtime {	
	//主菜单的流程
	public class HotProcedureMenu : HotProcedure
	{
	    public override bool UseNativeDialog { get { return false; } }

        private string m_HotProcedureLogicTypeFullName = "ProcedureMenu".HotFixTypeFullName();
        public override string HotProcedureLogicTypeFullName { get { return m_HotProcedureLogicTypeFullName; } }

	}
}
