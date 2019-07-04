#if !ILRuntime

using System;
using System.Reflection;

namespace Game.Runtime
{
    //反射获取的实例方法
    public class ReflectInstanceMethod : InstanceMethod
    {
        private MethodInfo m_MethodInfo; //静态方法

        public override bool IsAvalible { get { return m_MethodInfo != null; } }

        public ReflectInstanceMethod() { }

        public ReflectInstanceMethod Fill(object instance, string typeFullName, string methodName)
        {
            Type type = GameEntry.Hotfix.ReflectAssembly.GetType(typeFullName);
            this.m_MethodInfo = type.GetMethod(methodName);
            SetInstance(instance, m_MethodInfo.GetParameters().Length);
            return this;
        }

        public override void Clear()
        {
            base.Clear();
            m_MethodInfo = default;
        }

        public override void Run()
        {
            m_MethodInfo.Invoke(Instance, Param);
        }

        public override void Run(object a)
        {
            Param[0] = a;
            m_MethodInfo.Invoke(Instance, Param);
        }

        public override void Run(object a, object b)
        {
            this.Param[0] = a;
            this.Param[1] = b;
            m_MethodInfo.Invoke(Instance, Param);
        }

        public override void Run(object a, object b, object c)
        {
            this.Param[0] = a;
            this.Param[1] = b;
            this.Param[2] = c;
            m_MethodInfo.Invoke(Instance, Param);
        }

        public override T Run<T>()
        {
            return (T)m_MethodInfo.Invoke(Instance, Param);
        }

        public override T Run<T>(object a)
        {
            this.Param[0] = a;
            return (T)m_MethodInfo.Invoke(Instance, Param);
        }

        public override T Run<T>(object a, object b)
        {
            this.Param[0] = a;
            this.Param[1] = b;
            return (T)m_MethodInfo.Invoke(Instance, Param);
        }

        public override T Run<T>(object a, object b, object c)
        {
            this.Param[0] = a;
            this.Param[1] = b;
            this.Param[2] = c;
            return (T)m_MethodInfo.Invoke(Instance, Param);
        }
    }
}

#endif