using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityGameFrame.Runtime;
#if ILRuntime
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;
#endif

namespace Game.Runtime
{

	//热更新的组件
	public class HotfixComponent : GameFrameworkComponent
	{

#if ILRuntime

        /// <summary>
        /// ILRuntime入口
        /// </summary>
        public AppDomain ILAppDomain { get; private set; }

#else

        /// <summary>
        /// 热更新的程序集
        /// </summary>
        public Assembly ReflectAssembly { get; private set; }

#endif

        //热更新的命名空间
        public const string c_HotfixNamespace = "Game.Hotfix";

        //以下方法在hotfix中注册
        public Action<float, float> OnUpdate = null; //更新回调
	    public Action OnLateUpdate = null;   //延迟更新
	    public Action OnApplication = null;    //应用退出回调
	
	    private List<Type> hotfixTypes; //热更新的所有类型

        //加载热更新脚本，可动态改变热更新模式：ILRuntime热更新/Mono程序集反射热更新(非IOS)。加载脚本数据的操作放入预加载流程中
        public void LoadHotfixAssembly(byte[] dllBytes, byte[] pdbBytes = null)
	    {
	        StaticMethod start = null;    //逻辑入口
            string hotfixTypeName = "HotfixEntry";
            string hotfixStartMethodName = "Start";

#if ILRuntime

            Log.Debug($"当前使用的是ILRuntime模式");
            ILAppDomain = new AppDomain();

            using (MemoryStream fdllStreams = new MemoryStream(dllBytes))
            {
                using (MemoryStream pdbStream = new MemoryStream(pdbBytes))
                {
                    ILAppDomain.LoadAssembly(fdllStreams, null, new Mono.Cecil.Pdb.PdbReaderProvider());
                }
            }

            //启动调试服务器
            //AppDomain.DebugService.StartDebugService(56000);

            //做一些ILRuntime的注册，比如重定向函数、注册委托变量、适配器等
            ILRuntimeUtility.InitILRuntime(ILAppDomain);

            hotfixTypes = ILAppDomain.LoadedTypes.Values.Select(x => x.ReflectionType).ToList();

            start = new ILStaticMethod(hotfixTypeName.HotFixTypeFullName(), hotfixStartMethodName, 1);

#elif Reflect

            Log.Debug($"当前使用的是Mono模式");
            ReflectAssembly = Assembly.Load(dllBytes, pdbBytes);
            this.hotfixTypes = ReflectAssembly.GetTypes().ToList();
            start = new ReflectStaticMethod(hotfixTypeName.HotFixTypeFullName(), hotfixStartMethodName);


#else

            ReflectAssembly = Assembly.Load(c_HotfixNamespace);
            this.hotfixTypes = ReflectAssembly.GetTypes().ToList();

            start = new ReflectStaticMethod(hotfixTypeName.HotFixTypeFullName(), hotfixStartMethodName);

#endif

            //启动热更新的程序
            start.Run(GameEntry._Instance.gameObject);
        }
	
	    //获取类型
        public object GetHotType(string typeName)
        {
            object type = null;
#if ILRuntime

            type = ILAppDomain.LoadedTypes[typeName];

#else

            type = ReflectAssembly.GetType(typeName);

#endif

            return type;
        }

        //创建实例对象
        public object CreateInstance(string typeFullName, params object[] args)
        {
            object instance;

#if ILRuntime

            instance = ILAppDomain.Instantiate(typeFullName, args);

#else

            Type type = ReflectAssembly.GetType(typeFullName);
            instance = Activator.CreateInstance(type, args);

#endif
            return instance;
        }

        //调用一次的方法
        public object InvokeOne(string typeFullName, string methodName, object instance, params object[] args)
        {
            if(instance != null)
            {
#if ILRuntime
                if (ILAppDomain != null)
                    return ILAppDomain.Invoke(typeFullName, methodName, instance, args);

#else
                if(ReflectAssembly != null)
                {
                    Type type = ReflectAssembly.GetType(typeFullName);
                    return type.GetMethod(methodName).Invoke(instance, args);
                }

#endif
            }

            return null;

        }


        private void Update()
	    {
	        OnUpdate?.Invoke(Time.deltaTime, Time.unscaledDeltaTime);
	    }
	
	    private void LateUpdate()
	    {
	        OnLateUpdate?.Invoke();
	    }
	
	    private void OnApplicationQuit()
	    {
	        OnApplication?.Invoke();
	    }
	
	}
}
