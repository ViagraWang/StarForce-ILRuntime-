using System.Text;

namespace UnityGameFrame.Editor.Processor
{
    /// <summary>
    /// 数据表创建代码的生成器回调
    /// </summary>
    /// <param name="dataTableProcessor"></param>
    /// <param name="codeContent"></param>
    /// <param name="userData"></param>
    public delegate void DataTableCodeGenerator(DataTableProcessor dataTableProcessor, StringBuilder codeContent, object userData);
}
