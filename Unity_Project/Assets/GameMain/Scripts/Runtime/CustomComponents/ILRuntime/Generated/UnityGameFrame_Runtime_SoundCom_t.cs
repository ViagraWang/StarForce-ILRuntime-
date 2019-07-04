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
    unsafe class UnityGameFrame_Runtime_SoundComponent_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(UnityGameFrame.Runtime.SoundComponent);
            args = new Type[]{};
            method = type.GetMethod("StopAllLoadingSounds", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, StopAllLoadingSounds_0);
            args = new Type[]{typeof(System.Single)};
            method = type.GetMethod("StopAllLoadedSounds", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, StopAllLoadedSounds_1);


        }


        static StackObject* StopAllLoadingSounds_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityGameFrame.Runtime.SoundComponent instance_of_this_method = (UnityGameFrame.Runtime.SoundComponent)typeof(UnityGameFrame.Runtime.SoundComponent).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.StopAllLoadingSounds();

            return __ret;
        }

        static StackObject* StopAllLoadedSounds_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Single @fadeOutSeconds = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            UnityGameFrame.Runtime.SoundComponent instance_of_this_method = (UnityGameFrame.Runtime.SoundComponent)typeof(UnityGameFrame.Runtime.SoundComponent).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.StopAllLoadedSounds(@fadeOutSeconds);

            return __ret;
        }



    }
}
