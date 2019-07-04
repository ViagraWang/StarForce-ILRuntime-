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
    unsafe class UnityGameFrame_Runtime_EntityComponent_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(UnityGameFrame.Runtime.EntityComponent);
            args = new Type[]{typeof(System.Int32)};
            method = type.GetMethod("GetEntity", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetEntity_0);
            args = new Type[]{typeof(UnityGameFrame.Runtime.Entity), typeof(System.Object)};
            method = type.GetMethod("HideEntity", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, HideEntity_1);
            args = new Type[]{typeof(UnityGameFrame.Runtime.Entity), typeof(System.Int32), typeof(System.String), typeof(System.Object)};
            method = type.GetMethod("AttachEntity", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, AttachEntity_2);
            args = new Type[]{};
            method = type.GetMethod("HideAllLoadingEntities", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, HideAllLoadingEntities_3);
            args = new Type[]{typeof(System.Object)};
            method = type.GetMethod("HideAllLoadedEntities", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, HideAllLoadedEntities_4);


        }


        static StackObject* GetEntity_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Int32 @entityId = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            UnityGameFrame.Runtime.EntityComponent instance_of_this_method = (UnityGameFrame.Runtime.EntityComponent)typeof(UnityGameFrame.Runtime.EntityComponent).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.GetEntity(@entityId);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* HideEntity_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Object @userData = (System.Object)typeof(System.Object).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            UnityGameFrame.Runtime.Entity @entity = (UnityGameFrame.Runtime.Entity)typeof(UnityGameFrame.Runtime.Entity).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            UnityGameFrame.Runtime.EntityComponent instance_of_this_method = (UnityGameFrame.Runtime.EntityComponent)typeof(UnityGameFrame.Runtime.EntityComponent).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.HideEntity(@entity, @userData);

            return __ret;
        }

        static StackObject* AttachEntity_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 5);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Object @userData = (System.Object)typeof(System.Object).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.String @parentTransformPath = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.Int32 @parentEntityId = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            UnityGameFrame.Runtime.Entity @childEntity = (UnityGameFrame.Runtime.Entity)typeof(UnityGameFrame.Runtime.Entity).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 5);
            UnityGameFrame.Runtime.EntityComponent instance_of_this_method = (UnityGameFrame.Runtime.EntityComponent)typeof(UnityGameFrame.Runtime.EntityComponent).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.AttachEntity(@childEntity, @parentEntityId, @parentTransformPath, @userData);

            return __ret;
        }

        static StackObject* HideAllLoadingEntities_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityGameFrame.Runtime.EntityComponent instance_of_this_method = (UnityGameFrame.Runtime.EntityComponent)typeof(UnityGameFrame.Runtime.EntityComponent).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.HideAllLoadingEntities();

            return __ret;
        }

        static StackObject* HideAllLoadedEntities_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Object @userData = (System.Object)typeof(System.Object).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            UnityGameFrame.Runtime.EntityComponent instance_of_this_method = (UnityGameFrame.Runtime.EntityComponent)typeof(UnityGameFrame.Runtime.EntityComponent).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.HideAllLoadedEntities(@userData);

            return __ret;
        }



    }
}
