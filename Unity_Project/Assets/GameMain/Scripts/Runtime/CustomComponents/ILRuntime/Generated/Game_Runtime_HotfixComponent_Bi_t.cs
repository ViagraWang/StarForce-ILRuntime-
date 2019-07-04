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
    unsafe class Game_Runtime_HotfixComponent_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            FieldInfo field;
            Type[] args;
            Type type = typeof(Game.Runtime.HotfixComponent);

            field = type.GetField("OnUpdate", flag);
            app.RegisterCLRFieldGetter(field, get_OnUpdate_0);
            app.RegisterCLRFieldSetter(field, set_OnUpdate_0);
            field = type.GetField("OnLateUpdate", flag);
            app.RegisterCLRFieldGetter(field, get_OnLateUpdate_1);
            app.RegisterCLRFieldSetter(field, set_OnLateUpdate_1);
            field = type.GetField("OnApplication", flag);
            app.RegisterCLRFieldGetter(field, get_OnApplication_2);
            app.RegisterCLRFieldSetter(field, set_OnApplication_2);


        }



        static object get_OnUpdate_0(ref object o)
        {
            return ((Game.Runtime.HotfixComponent)o).OnUpdate;
        }
        static void set_OnUpdate_0(ref object o, object v)
        {
            ((Game.Runtime.HotfixComponent)o).OnUpdate = (System.Action<System.Single, System.Single>)v;
        }
        static object get_OnLateUpdate_1(ref object o)
        {
            return ((Game.Runtime.HotfixComponent)o).OnLateUpdate;
        }
        static void set_OnLateUpdate_1(ref object o, object v)
        {
            ((Game.Runtime.HotfixComponent)o).OnLateUpdate = (System.Action)v;
        }
        static object get_OnApplication_2(ref object o)
        {
            return ((Game.Runtime.HotfixComponent)o).OnApplication;
        }
        static void set_OnApplication_2(ref object o, object v)
        {
            ((Game.Runtime.HotfixComponent)o).OnApplication = (System.Action)v;
        }


    }
}
