
namespace Game.Runtime
{
    //静态方法抽象类
    public abstract class StaticMethod : IMethodBase
	{
        protected object[] Param { get; private set; }    //参数

        public abstract bool IsAvalible { get; }    //是否可用

        public void SetParams(int paramCount)
        {
            Param = new object[paramCount];
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
