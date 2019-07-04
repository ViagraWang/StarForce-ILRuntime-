using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Runtime
{
    //方法接口
    public interface IMethodBase
    {
        bool IsAvalible { get; }    //是否可用

        void Run();
        void Run(object a);
        void Run(object a, object b);
        void Run(object a, object b, object c);

        T Run<T>();
        T Run<T>(object a);
        T Run<T>(object a, object b);
        T Run<T>(object a, object b, object c);
    }
}
