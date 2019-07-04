
namespace UnityGameFrame.Editor.Processor
{
    public sealed partial class DataTableProcessor
    {
        public abstract class GenericDataProcessor<T> : DataProcessor
        {
            public override System.Type Type { get { return typeof(T); } }

            public override bool IsId { get { return false;} }

            public override bool IsComment { get { return false; } }

            /// <summary>
            /// 解析数据
            /// </summary>
            /// <param name="value">数据内容</param>
            /// <returns>解析厚返回的类型</returns>
            public abstract T Parse(string value);
        }
    }
}
