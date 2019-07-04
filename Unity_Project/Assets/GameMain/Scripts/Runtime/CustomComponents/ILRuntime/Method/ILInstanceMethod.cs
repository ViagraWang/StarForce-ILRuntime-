#if ILRuntime
using ILRuntime.CLR.Method;

namespace Game.Runtime
{
    //ILRuntime获取的实例方法
    public class ILInstanceMethod : InstanceMethod
    {
        private ILRuntime.Runtime.Enviorment.AppDomain m_AppDomain;
        private IMethod m_Method;    //方法

        public override bool IsAvalible { get { return m_Method != null; } }

        public ILInstanceMethod() { }

        public ILInstanceMethod Fill(object instance, string typeFullName, string methodName, int paramCount)
        {
            m_AppDomain = GameEntry.Hotfix.ILAppDomain;
            this.m_Method = m_AppDomain.GetType(typeFullName).GetMethod(methodName, paramCount);
            SetInstance(instance, paramCount);
            return this;
        }

        public override void Clear()
        {
            base.Clear();
            m_Method = null;
            m_AppDomain = null;
        }

        public override void Run()
        {
            m_AppDomain.Invoke(this.m_Method, Instance, Param);
        }

        public override void Run(object a)
        {
            Param[0] = a;
            m_AppDomain.Invoke(this.m_Method, Instance, Param);
        }

        public override void Run(object a, object b)
        {
            this.Param[0] = a;
            this.Param[1] = b;
            m_AppDomain.Invoke(this.m_Method, Instance, Param);
        }

        public override void Run(object a, object b, object c)
        {
            this.Param[0] = a;
            this.Param[1] = b;
            this.Param[2] = c;
            m_AppDomain.Invoke(this.m_Method, Instance, Param);
        }

        public override T Run<T>()
        {
            return (T)m_AppDomain.Invoke(this.m_Method, Instance, Param);
        }

        public override T Run<T>(object a)
        {
            this.Param[0] = a;
            return (T)m_AppDomain.Invoke(this.m_Method, Instance, Param);
        }

        public override T Run<T>(object a, object b)
        {
            this.Param[0] = a;
            this.Param[1] = b;
            return (T)m_AppDomain.Invoke(this.m_Method, Instance, Param);
        }

        public override T Run<T>(object a, object b, object c)
        {
            this.Param[0] = a;
            this.Param[1] = b;
            this.Param[2] = c;
            return (T)m_AppDomain.Invoke(this.m_Method, Instance, Param);
        }

    }
}

#endif