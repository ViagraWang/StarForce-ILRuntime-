#if ILRuntime
using ILRuntime.CLR.Method;

namespace Game.Runtime
{
    //IL获取的静态方法
	public class ILStaticMethod : StaticMethod
	{
	    private readonly ILRuntime.Runtime.Enviorment.AppDomain m_AppDomain;
	    private readonly IMethod m_Method;    //静态方法
        
        public override bool IsAvalible { get { return m_Method != null; } }

        public ILStaticMethod(string typeFullName, string methodName, int paramsCount)
	    {
            this.m_AppDomain = GameEntry.Hotfix.ILAppDomain;
            this.m_Method = m_AppDomain.GetType(typeFullName).GetMethod(methodName, paramsCount);
            SetParams(paramsCount);
        }

        public override void Run()
        {
            m_AppDomain.Invoke(this.m_Method, null, null);
        }

        public override void Run(object a)
        {
            Param[0] = a;
            m_AppDomain.Invoke(this.m_Method, null, Param);
        }

        public override void Run(object a, object b)
        {
            this.Param[0] = a;
            this.Param[1] = b;
            m_AppDomain.Invoke(this.m_Method, null, Param);
        }

        public override void Run(object a, object b, object c)
        {
            this.Param[0] = a;
            this.Param[1] = b;
            this.Param[2] = c;
            m_AppDomain.Invoke(this.m_Method, null, Param);
        }

        public override T Run<T>()
	    {
            return (T)m_AppDomain.Invoke(this.m_Method, null, null);
	    }
	
	    public override T Run<T>(object a)
	    {
	        this.Param[0] = a;
            return (T)m_AppDomain.Invoke(this.m_Method, null, Param);
	    }
	
	    public override T Run<T>(object a, object b)
	    {
	        this.Param[0] = a;
	        this.Param[1] = b;
            return (T)m_AppDomain.Invoke(this.m_Method, null, Param);
	    }
	
	    public override T Run<T>(object a, object b, object c)
	    {
	        this.Param[0] = a;
	        this.Param[1] = b;
	        this.Param[2] = c;
	        return (T)m_AppDomain.Invoke(this.m_Method, null, Param);
	    }
    }
}
#endif