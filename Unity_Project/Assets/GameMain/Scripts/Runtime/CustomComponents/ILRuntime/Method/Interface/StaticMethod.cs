
namespace Game.Runtime
{
    //��̬����������
    public abstract class StaticMethod : IMethodBase
	{
        protected object[] Param { get; private set; }    //����

        public abstract bool IsAvalible { get; }    //�Ƿ����

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
