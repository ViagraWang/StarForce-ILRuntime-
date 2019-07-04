#if !ILRuntime

using System;
using System.Reflection;

namespace Game.Runtime
{
    //反射获取的静态方法
	public class ReflectStaticMethod : StaticMethod
	{
	    private readonly MethodInfo m_MethodInfo; //静态方法
		
        public override bool IsAvalible { get { return m_MethodInfo != null; } }

	    public ReflectStaticMethod(string typeFullName, string methodName)
	    {
            Type type = GameEntry.Hotfix.ReflectAssembly.GetType(typeFullName);
            this.m_MethodInfo = type.GetMethod(methodName);
            SetParams(m_MethodInfo.GetParameters().Length);
        }

        public override void Run()
        {
            m_MethodInfo.Invoke(null, Param);
        }

        public override void Run(object a)
        {
            Param[0] = a;
            m_MethodInfo.Invoke(null, Param);
        }

        public override void Run(object a, object b)
        {
            this.Param[0] = a;
            this.Param[1] = b;
            m_MethodInfo.Invoke(null, Param);
        }

        public override void Run(object a, object b, object c)
        {
            this.Param[0] = a;
            this.Param[1] = b;
            this.Param[2] = c;
            m_MethodInfo.Invoke(null, Param);
        }

        public override T Run<T>()
	    {
            return (T)m_MethodInfo.Invoke(null, Param);
	    }
	
	    public override T Run<T>(object a)
	    {
	        this.Param[0] = a;
            return (T)m_MethodInfo.Invoke(null, Param);
	    }
	
	    public override T Run<T>(object a, object b)
	    {
	        this.Param[0] = a;
	        this.Param[1] = b;
            return (T)m_MethodInfo.Invoke(null, Param);
	    }
	
	    public override T Run<T>(object a, object b, object c)
	    {
	        this.Param[0] = a;
	        this.Param[1] = b;
	        this.Param[2] = c;
	        return (T)m_MethodInfo.Invoke(null, Param);
	    }
	}
}

#endif