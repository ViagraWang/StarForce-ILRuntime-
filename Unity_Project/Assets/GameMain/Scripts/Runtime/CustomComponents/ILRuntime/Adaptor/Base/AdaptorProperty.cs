using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Intepreter;
using System;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;

namespace Game.Runtime {	
	
	//自定义的适配器属性类，包含通用数据
	public class AdaptorProperty
	{
	    public AppDomain AppDomain { get; private set; }
	    private AdaptHelper.AdaptMethod[] _methods; //适配的方法
	
	    public ILTypeInstance ILInstance { get; private set; }  //类型实例
	
	    //构造函数
	    public AdaptorProperty(AppDomain appdomain, ILTypeInstance instance, AdaptHelper.AdaptMethod[] methods)
	    {
	        AppDomain = appdomain;
	        ILInstance = instance;
	        _methods = methods;
	    }
	
	    //获取方法
	    public IMethod GetMethod(int index)
	    {
	        if (_methods == null)
	            throw new Exception("[AdaptorProperty.GetMethod] Methods is invalid -> _methods == null");
	
	        return ILInstance.Type.GetMethod(_methods[index]);
	    }
	
	    //调用方法，如果是抽象方法或者虚方法，尽量不要用这个，因为会有不存在重写的情况，需要调用父类的方法
	    public object Invoke(int index, params object[] p)
	    {
	        var method = GetMethod(index);
	        return AppDomain.Invoke(method, ILInstance, p);
	    }
	
	    //执行ToString的方法
	    public string DoToString()
	    {
	        IMethod mToString = this.AppDomain.ObjectType.GetMethod("ToString", 0);
	        mToString = ILInstance.Type.GetVirtualMethod(mToString);
	
	        if (mToString == null || mToString is ILMethod)
	        {
	            return ILInstance.ToString();
	        }
	
	        return ILInstance.Type.FullName;
	    }
	
	}
}
