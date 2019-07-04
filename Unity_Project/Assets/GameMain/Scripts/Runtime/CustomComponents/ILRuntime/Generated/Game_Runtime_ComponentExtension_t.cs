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
    unsafe class Game_Runtime_ComponentExtension_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(Game.Runtime.ComponentExtension);
            args = new Type[]{typeof(UnityGameFrame.Runtime.ResourceComponent), typeof(System.String), typeof(System.Int32), typeof(System.Action<System.String, System.Object, System.Single, System.Object>), typeof(System.Action<System.String, System.String, System.String, System.Object>), typeof(System.Object)};
            method = type.GetMethod("LoadAsset", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, LoadAsset_0);
            args = new Type[]{typeof(Game.Runtime.CommonButton), typeof(UnityEngine.Events.UnityAction)};
            method = type.GetMethod("ComButtonAddClick", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ComButtonAddClick_1);
            args = new Type[]{typeof(UnityEngine.UI.Toggle), typeof(UnityEngine.Events.UnityAction<System.Boolean>)};
            method = type.GetMethod("ToggleAddChanged", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ToggleAddChanged_2);
            args = new Type[]{typeof(UnityEngine.UI.Slider), typeof(UnityEngine.Events.UnityAction<System.Single>)};
            method = type.GetMethod("SliderAddChanged", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SliderAddChanged_3);


        }


        static StackObject* LoadAsset_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 6);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Object @userData = (System.Object)typeof(System.Object).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Action<System.String, System.String, System.String, System.Object> @loadAssetFailureCallbacks = (System.Action<System.String, System.String, System.String, System.Object>)typeof(System.Action<System.String, System.String, System.String, System.Object>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.Action<System.String, System.Object, System.Single, System.Object> @loadAssetSuccessCallbacks = (System.Action<System.String, System.Object, System.Single, System.Object>)typeof(System.Action<System.String, System.Object, System.Single, System.Object>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            System.Int32 @priority = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 5);
            System.String @assetName = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 6);
            UnityGameFrame.Runtime.ResourceComponent @resource = (UnityGameFrame.Runtime.ResourceComponent)typeof(UnityGameFrame.Runtime.ResourceComponent).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            Game.Runtime.ComponentExtension.LoadAsset(@resource, @assetName, @priority, @loadAssetSuccessCallbacks, @loadAssetFailureCallbacks, @userData);

            return __ret;
        }

        static StackObject* ComButtonAddClick_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.Events.UnityAction @action = (UnityEngine.Events.UnityAction)typeof(UnityEngine.Events.UnityAction).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Game.Runtime.CommonButton @button = (Game.Runtime.CommonButton)typeof(Game.Runtime.CommonButton).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            Game.Runtime.ComponentExtension.ComButtonAddClick(@button, @action);

            return __ret;
        }

        static StackObject* ToggleAddChanged_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.Events.UnityAction<System.Boolean> @callback = (UnityEngine.Events.UnityAction<System.Boolean>)typeof(UnityEngine.Events.UnityAction<System.Boolean>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            UnityEngine.UI.Toggle @toggle = (UnityEngine.UI.Toggle)typeof(UnityEngine.UI.Toggle).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            Game.Runtime.ComponentExtension.ToggleAddChanged(@toggle, @callback);

            return __ret;
        }

        static StackObject* SliderAddChanged_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.Events.UnityAction<System.Single> @callback = (UnityEngine.Events.UnityAction<System.Single>)typeof(UnityEngine.Events.UnityAction<System.Single>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            UnityEngine.UI.Slider @slider = (UnityEngine.UI.Slider)typeof(UnityEngine.UI.Slider).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            Game.Runtime.ComponentExtension.SliderAddChanged(@slider, @callback);

            return __ret;
        }



    }
}
