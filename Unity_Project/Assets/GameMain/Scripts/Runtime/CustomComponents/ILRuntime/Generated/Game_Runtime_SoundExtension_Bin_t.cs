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
    unsafe class Game_Runtime_SoundExtension_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            Type[] args;
            Type type = typeof(Game.Runtime.SoundExtension);
            args = new Type[]{typeof(UnityGameFrame.Runtime.SoundComponent), typeof(System.Int32), typeof(Game.Runtime.Entity), typeof(System.Object)};
            method = type.GetMethod("PlaySound", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, PlaySound_0);
            args = new Type[]{typeof(UnityGameFrame.Runtime.SoundComponent), typeof(System.Int32), typeof(System.Object)};
            method = type.GetMethod("PlayMusic", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, PlayMusic_1);
            args = new Type[]{typeof(UnityGameFrame.Runtime.SoundComponent), typeof(System.String)};
            method = type.GetMethod("IsMuted", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, IsMuted_2);
            args = new Type[]{typeof(UnityGameFrame.Runtime.SoundComponent), typeof(System.String)};
            method = type.GetMethod("GetGroupVolume", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetGroupVolume_3);
            args = new Type[]{typeof(UnityGameFrame.Runtime.SoundComponent), typeof(System.String), typeof(System.Boolean)};
            method = type.GetMethod("Mute", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Mute_4);
            args = new Type[]{typeof(UnityGameFrame.Runtime.SoundComponent), typeof(System.String), typeof(System.Single)};
            method = type.GetMethod("SetGroupVolume", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, SetGroupVolume_5);
            args = new Type[]{typeof(UnityGameFrame.Runtime.SoundComponent)};
            method = type.GetMethod("StopMusic", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, StopMusic_6);


        }


        static StackObject* PlaySound_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 4);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Object @userData = (System.Object)typeof(System.Object).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            Game.Runtime.Entity @bindingEntity = (Game.Runtime.Entity)typeof(Game.Runtime.Entity).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.Int32 @soundId = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            UnityGameFrame.Runtime.SoundComponent @soundComponent = (UnityGameFrame.Runtime.SoundComponent)typeof(UnityGameFrame.Runtime.SoundComponent).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            var result_of_this_method = Game.Runtime.SoundExtension.PlaySound(@soundComponent, @soundId, @bindingEntity, @userData);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* PlayMusic_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Object @userData = (System.Object)typeof(System.Object).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Int32 @musicId = ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            UnityGameFrame.Runtime.SoundComponent @soundComponent = (UnityGameFrame.Runtime.SoundComponent)typeof(UnityGameFrame.Runtime.SoundComponent).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            var result_of_this_method = Game.Runtime.SoundExtension.PlayMusic(@soundComponent, @musicId, @userData);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* IsMuted_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @soundGroupName = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            UnityGameFrame.Runtime.SoundComponent @soundComponent = (UnityGameFrame.Runtime.SoundComponent)typeof(UnityGameFrame.Runtime.SoundComponent).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            var result_of_this_method = Game.Runtime.SoundExtension.IsMuted(@soundComponent, @soundGroupName);

            __ret->ObjectType = ObjectTypes.Integer;
            __ret->Value = result_of_this_method ? 1 : 0;
            return __ret + 1;
        }

        static StackObject* GetGroupVolume_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String @soundGroupName = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            UnityGameFrame.Runtime.SoundComponent @soundComponent = (UnityGameFrame.Runtime.SoundComponent)typeof(UnityGameFrame.Runtime.SoundComponent).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            var result_of_this_method = Game.Runtime.SoundExtension.GetGroupVolume(@soundComponent, @soundGroupName);

            __ret->ObjectType = ObjectTypes.Float;
            *(float*)&__ret->Value = result_of_this_method;
            return __ret + 1;
        }

        static StackObject* Mute_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Boolean @mute = ptr_of_this_method->Value == 1;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.String @soundGroupName = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            UnityGameFrame.Runtime.SoundComponent @soundComponent = (UnityGameFrame.Runtime.SoundComponent)typeof(UnityGameFrame.Runtime.SoundComponent).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            Game.Runtime.SoundExtension.Mute(@soundComponent, @soundGroupName, @mute);

            return __ret;
        }

        static StackObject* SetGroupVolume_5(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Single @volume = *(float*)&ptr_of_this_method->Value;

            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.String @soundGroupName = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            UnityGameFrame.Runtime.SoundComponent @soundComponent = (UnityGameFrame.Runtime.SoundComponent)typeof(UnityGameFrame.Runtime.SoundComponent).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            Game.Runtime.SoundExtension.SetGroupVolume(@soundComponent, @soundGroupName, @volume);

            return __ret;
        }

        static StackObject* StopMusic_6(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);

            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityGameFrame.Runtime.SoundComponent @soundComponent = (UnityGameFrame.Runtime.SoundComponent)typeof(UnityGameFrame.Runtime.SoundComponent).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);


            Game.Runtime.SoundExtension.StopMusic(@soundComponent);

            return __ret;
        }



    }
}
