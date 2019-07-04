using GameFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Runtime
{
    //实例方法抽象类
    public abstract class InstanceMethod : IMethodBase, IReference
    {
        public object Instance { get; private set; } //实例对象

        protected object[] Param { get; private set; }    //参数

        public abstract bool IsAvalible { get; }    //是否可用

        public void SetInstance(object instance, int paramCount)
        {
            Instance = instance;
            Param = new object[paramCount];
        }

        public virtual void Clear()
        {
            Instance = default;
            Param = null;
        }

        public abstract void Run();

        public abstract void Run(object a);

        public abstract void Run(object a, object b);

        public abstract void Run(object a, object b, object c);

        public abstract T Run<T>();

        public abstract T Run<T>(object a);

        public abstract T Run<T>(object a, object b);

        public abstract T Run<T>(object a, object b, object c);

    }
}
