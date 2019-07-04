using ILRuntime.Runtime.Enviorment;
using System;
using System.Collections.Generic;
using UnityEngine.Events;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;

namespace Game.Runtime
{	
	//ILRuntime扩展工具包
	public static class ILRuntimeUtility
	{
	    //初始化
	    public static void InitILRuntime(AppDomain appDomain)
	    {

            //注册委托参数
            //GF用
            appDomain.DelegateManager.RegisterMethodDelegate<float>();
	        appDomain.DelegateManager.RegisterMethodDelegate<object, GameFramework.Event.GameEventArgs>();
	        appDomain.DelegateManager.RegisterMethodDelegate<float, float>();
	        appDomain.DelegateManager.RegisterMethodDelegate<bool>();
	        appDomain.DelegateManager.RegisterMethodDelegate<object>();
	        appDomain.DelegateManager.RegisterMethodDelegate<string, string, string, object>(); //加载资源失败的回调
	        appDomain.DelegateManager.RegisterMethodDelegate<string, object, float, object>(); //加载资源成功的回调
	
	        //ET用
	        appDomain.DelegateManager.RegisterMethodDelegate<List<object>>();
	        //appDomain.DelegateManager.RegisterMethodDelegate<AChannel, System.Net.Sockets.SocketError>();
	        appDomain.DelegateManager.RegisterMethodDelegate<byte[], int, int>();
	        //appDomain.DelegateManager.RegisterMethodDelegate<IResponse>();
	        //appDomain.DelegateManager.RegisterMethodDelegate<Session, object>();
	        //appDomain.DelegateManager.RegisterMethodDelegate<Session, ushort, MemoryStream>();
	        //appDomain.DelegateManager.RegisterMethodDelegate<Session>();
	        //appDomain.DelegateManager.RegisterMethodDelegate<ILTypeInstance>();

            //PB用
            //appDomain.DelegateManager.RegisterFunctionDelegate<IMessageAdaptor.Adaptor>();
            //appDomain.DelegateManager.RegisterMethodDelegate<IMessageAdaptor.Adaptor>();

            //注册委托转换器
            appDomain.DelegateManager.RegisterDelegateConvertor<UnityAction>((action) =>
	        {
	            return new UnityAction(() =>
	            {
	                ((Action)action).Invoke();
	            });
	        });
	
	        appDomain.DelegateManager.RegisterDelegateConvertor<UnityAction<float>>((action) =>
	        {
	            return new UnityAction<float>((a) =>
	            {
	                ((Action<float>)action).Invoke(a);
	            });
	        });

            appDomain.DelegateManager.RegisterDelegateConvertor<UnityAction<bool>>((action) =>
            {
                return new UnityAction<bool>((a) =>
                {
                    ((Action<bool>)action).Invoke(a);
                });
            });

            appDomain.DelegateManager.RegisterDelegateConvertor<EventHandler<GameFramework.Event.GameEventArgs>>((action) =>
            {
                return new EventHandler<GameFramework.Event.GameEventArgs>((sender, e) =>
                {
                    ((Action<object, GameFramework.Event.GameEventArgs>)action).Invoke(sender, e);
                });
            });

            //注册CLR绑定代码
            ILRuntime.Runtime.Generated.CLRBindings.Initialize(appDomain);

            //注册跨域继承适配器
            //Type[] types = Utility.Assembly.GetTypes();   //这里是工程中所有的类型，其实没必要
            Type[] types = typeof(HotfixComponent).Assembly.GetTypes();
            for (int i = 0; i < types.Length; i++)
            {
                Type type = types[i];
                object[] attrs = type.GetCustomAttributes(typeof(ILAdapterAttribute), false);
                if (attrs.Length == 0)
                {
                    continue;
                }
                object obj = Activator.CreateInstance(type);
                CrossBindingAdaptor adaptor = obj as CrossBindingAdaptor;
                if (adaptor == null)
                {
                    continue;
                }
                appDomain.RegisterCrossBindingAdaptor(adaptor);
            }
	
	        //注册LitJson
	        LitJson.JsonMapper.RegisterILRuntimeCLRRedirection(appDomain);
	
	    }
	
	}
}
