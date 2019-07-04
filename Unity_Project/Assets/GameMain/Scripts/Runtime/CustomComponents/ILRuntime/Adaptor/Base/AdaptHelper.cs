using GameFramework;
using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;
using UnityGameFrame.Runtime;

namespace Game.Runtime {	
	
	public static class AdaptHelper
	{
	    //适配的方法
	    public class AdaptMethod
	    {
	        public string Name;
	        public int ParamCount;
	        public IMethod Method;
            public bool IsGotMethod = false;  //是否获取过方法

            public AdaptMethod(string methodName, int paramCount = -1)
	        {
	            Name = methodName;
	            ParamCount = paramCount;
            }
	    }
	
	    public static IMethod GetMethod(this ILType type, AdaptMethod m)
	    {
	        if (m.IsGotMethod)
	            return m.Method;
	
	        //获取方法
	        m.Method = m.ParamCount == -1 ? type.GetMethod(m.Name) : type.GetMethod(m.Name, m.ParamCount);
            m.IsGotMethod = true;
            if (m.Method == null)
	        {
	            string baseClass = "";
	            if (type.FirstCLRBaseType != null)
	            {
	                baseClass = type.FirstCLRBaseType.FullName;
	            }
	            else if (type.FirstCLRInterface != null)
	            {
	                baseClass = type.FirstCLRInterface.FullName;
	            }
	            Log.Warning(Utility.Text.Format("Can't find the method: {0}.{1}:{2}, paramCount={3}", type.FullName, m.Name, baseClass, m.ParamCount));;
	        }
	
	        return m.Method;
	    }
	
	}
}
