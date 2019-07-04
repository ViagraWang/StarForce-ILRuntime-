using GameFramework.Event;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using System;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;

namespace Game.Runtime
{
	[ILAdapter]
	public class BaseEventArgsAdaptor : CrossBindingAdaptor
	{
	    public override Type BaseCLRType
	    {
	        get { return typeof(GameEventArgs); }
	    }
	
	    public override Type AdaptorType { get { return typeof(Adaptor); } }
	
	    public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
	    {
	        return new Adaptor(appdomain, instance);
	    }
	
	    //适配器
	    public class Adaptor : GameEventArgs, CrossBindingAdaptorType
	    {
	        private AdaptorProperty property = null;    //适配器属性
	        public ILTypeInstance ILInstance { get { return property.ILInstance; } }
	
	        //事件id
	        public override int Id
	        {
	            get { return (int)property.Invoke(0, null); }
	        }
	
            public Adaptor() { }

            public Adaptor(AppDomain appdomain, ILTypeInstance instance)
	        {
	            //创建适配器包含的方法
	            AdaptHelper.AdaptMethod[] methods =
	            {
	                new AdaptHelper.AdaptMethod("get_Id",  0),
	                new AdaptHelper.AdaptMethod("Clear",  0),
	            };
	            property = new AdaptorProperty(appdomain, instance, methods);
	        }
	
	        //清理资源
	        public override void Clear()
	        {
	            property.Invoke(1, null);
	        }
	    }
	
	}
}
