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
    unsafe class Game_Runtime_HotProcedure_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(Game.Runtime.HotProcedure);
            Dictionary<string, List<MethodInfo>> genericMethods = new Dictionary<string, List<MethodInfo>>();
            List<MethodInfo> lst = null;                    
            foreach(var m in type.GetMethods())
            {
                if(m.IsGenericMethodDefinition)
                {
                    if (!genericMethods.TryGetValue(m.Name, out lst))
                    {
                        lst = new List<MethodInfo>();
                        genericMethods[m.Name] = lst;
                    }
                    lst.Add(m);
                }
            }
            args = new Type[]{typeof(Game.Runtime.HotProcedureMenu)};
            if (genericMethods.TryGetValue("ChangeProcedure", out lst))
            {
                foreach(var m in lst)
                {
                    if(m.GetParameters().Length == 1)
                    {
                        method = m.MakeGenericMethod(args);
                        app.RegisterCLRMethodRedirection(method, ChangeProcedure_0);

                        break;
                    }
                }
            }
            args = new Type[]{typeof(Game.Runtime.HotProcedureMain)};
            if (genericMethods.TryGetValue("ChangeProcedure", out lst))
            {
                foreach(var m in lst)
                {
                    if(m.GetParameters().Length == 1)
                    {
                        method = m.MakeGenericMethod(args);
                        app.RegisterCLRMethodRedirection(method, ChangeProcedure_1);

                        break;
                    }
                }
            }
            args = new Type[]{typeof(Game.Runtime.HotProcedureChangeScene)};
            if (genericMethods.TryGetValue("ChangeProcedure", out lst))
            {
                foreach(var m in lst)
                {
                    if(m.GetParameters().Length == 1)
                    {
                        method = m.MakeGenericMethod(args);
                        app.RegisterCLRMethodRedirection(method, ChangeProcedure_2);

                        break;
                    }
                }
            }
            args = new Type[]{};
            method = type.GetMethod("get_UseNativeDialog", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_UseNativeDialog_3);


        }


        static StackObject* ChangeProcedure_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager> @procedureOwner = (GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>)typeof(GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Game.Runtime.HotProcedure instance_of_this_method = (Game.Runtime.HotProcedure)typeof(Game.Runtime.HotProcedure).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.ChangeProcedure<Game.Runtime.HotProcedureMenu>(@procedureOwner);

            return __ret;
        }

        static StackObject* ChangeProcedure_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager> @procedureOwner = (GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>)typeof(GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Game.Runtime.HotProcedure instance_of_this_method = (Game.Runtime.HotProcedure)typeof(Game.Runtime.HotProcedure).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.ChangeProcedure<Game.Runtime.HotProcedureMain>(@procedureOwner);

            return __ret;
        }

        static StackObject* ChangeProcedure_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager> @procedureOwner = (GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>)typeof(GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Game.Runtime.HotProcedure instance_of_this_method = (Game.Runtime.HotProcedure)typeof(Game.Runtime.HotProcedure).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.ChangeProcedure<Game.Runtime.HotProcedureChangeScene>(@procedureOwner);

            return __ret;
        }

        static StackObject* get_UseNativeDialog_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            Game.Runtime.HotProcedure instance_of_this_method = (Game.Runtime.HotProcedure)typeof(Game.Runtime.HotProcedure).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.UseNativeDialog;

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }



    }
}
