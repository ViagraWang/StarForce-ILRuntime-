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
    unsafe class UnityEngine_UI_CanvasScaler_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(UnityEngine.UI.CanvasScaler);
            args = new Type[]{typeof(UnityEngine.UI.CanvasScaler.ScaleMode)};
            method = type.GetMethod("set_uiScaleMode", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_uiScaleMode_0);
            args = new Type[]{typeof(UnityEngine.Vector2)};
            method = type.GetMethod("set_referenceResolution", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_referenceResolution_1);
            args = new Type[]{typeof(UnityEngine.UI.CanvasScaler.ScreenMatchMode)};
            method = type.GetMethod("set_screenMatchMode", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_screenMatchMode_2);
            args = new Type[]{typeof(System.Single)};
            method = type.GetMethod("set_matchWidthOrHeight", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_matchWidthOrHeight_3);
            args = new Type[]{};
            method = type.GetMethod("get_referenceResolution", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_referenceResolution_4);


        }


        static StackObject* set_uiScaleMode_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.UI.CanvasScaler.ScaleMode @value = (UnityEngine.UI.CanvasScaler.ScaleMode)typeof(UnityEngine.UI.CanvasScaler.ScaleMode).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            UnityEngine.UI.CanvasScaler instance_of_this_method = (UnityEngine.UI.CanvasScaler)typeof(UnityEngine.UI.CanvasScaler).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.uiScaleMode = value;

            return __ret;
        }

        static StackObject* set_referenceResolution_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.Vector2 @value = (UnityEngine.Vector2)typeof(UnityEngine.Vector2).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            UnityEngine.UI.CanvasScaler instance_of_this_method = (UnityEngine.UI.CanvasScaler)typeof(UnityEngine.UI.CanvasScaler).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.referenceResolution = value;

            return __ret;
        }

        static StackObject* set_screenMatchMode_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.UI.CanvasScaler.ScreenMatchMode @value = (UnityEngine.UI.CanvasScaler.ScreenMatchMode)typeof(UnityEngine.UI.CanvasScaler.ScreenMatchMode).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            UnityEngine.UI.CanvasScaler instance_of_this_method = (UnityEngine.UI.CanvasScaler)typeof(UnityEngine.UI.CanvasScaler).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.screenMatchMode = value;

            return __ret;
        }

        static StackObject* set_matchWidthOrHeight_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Single @value = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            UnityEngine.UI.CanvasScaler instance_of_this_method = (UnityEngine.UI.CanvasScaler)typeof(UnityEngine.UI.CanvasScaler).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.matchWidthOrHeight = value;

            return __ret;
        }

        static StackObject* get_referenceResolution_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.UI.CanvasScaler instance_of_this_method = (UnityEngine.UI.CanvasScaler)typeof(UnityEngine.UI.CanvasScaler).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.referenceResolution;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }



    }
}
