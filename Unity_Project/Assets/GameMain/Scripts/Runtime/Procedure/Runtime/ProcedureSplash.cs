using GameFramework.Fsm;
using GameFramework.Procedure;

namespace Game.Runtime {	
	public class ProcedureSplash : ProcedureBase
	{
	    public override bool UseNativeDialog { get { return true; } }
	
	    protected override void OnUpdate(IFsm<IProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
	    {
	        base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            //TODO:增加一个Splash动画，这里先跳过
            //编辑器模式下，直接进入预加载流程，否则检查版本
            ChangeState(procedureOwner, GameEntry.Base.IsEditorResourceMode ? typeof(ProcedurePreload) : typeof(ProcedureCheckVersion));
            //ChangeState(procedureOwner, GameEntry.Base.IsEditorResourceMode ? typeof(ProcedurePreload) : typeof(ProcedureInitResources));
	    }
	}
}
