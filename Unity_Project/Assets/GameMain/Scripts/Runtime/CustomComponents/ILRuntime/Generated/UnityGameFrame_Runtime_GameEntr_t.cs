using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

using ILRuntime.CLR.TypeSystem;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using ILRuntime.Reflection;
using ILRuntime.CLR.Utils;

namespace ILRuntime.Runtime.Generated
{
    unsafe class UnityGameFrame_Runtime_GameEntry_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(UnityGameFrame.Runtime.GameEntry);
            args = new Type[]{typeof(UnityGameFrame.Runtime.ShutdownType)};
            method = type.GetMethod("Shutdown", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Shutdown_0);


        }


        static StackObject* Shutdown_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityGameFrame.Runtime.ShutdownType @shutdownType = (UnityGameFrame.Runtime.ShutdownType)typeof(UnityGameFrame.Runtime.ShutdownType).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            UnityGameFrame.Runtime.GameEntry.Shutdown(@shutdownType);

            return __ret;
        }



    }
}
