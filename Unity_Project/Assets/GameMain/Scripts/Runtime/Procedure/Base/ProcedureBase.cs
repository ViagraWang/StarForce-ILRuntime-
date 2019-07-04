
namespace Game.Runtime
{
	//流程基类
	public abstract class ProcedureBase : GameFramework.Procedure.ProcedureBase
	{
	    /// <summary>
	    /// 是否使用本地对话框
	    /// </summary>
	    public abstract bool UseNativeDialog { get; }


	}
}
