using System.IO;

namespace UnityGameFrame.Editor.Processor
{
    public sealed partial class DataTableProcessor
    {
        /// <summary>
        /// 数据处理器
        /// </summary>
        public abstract class DataProcessor
        {
            /// <summary>
            /// 类型
            /// </summary>
            public abstract System.Type Type { get; }

            /// <summary>
            /// 是否是Id
            /// </summary>
            public abstract bool IsId { get; }

            /// <summary>
            /// 是否是注释行
            /// </summary>
            public abstract bool IsComment { get; }

            /// <summary>
            /// 是否是系统值
            /// </summary>
            public abstract bool IsSystem { get; }

            /// <summary>
            /// 语言Key
            /// </summary>
            public abstract string LanguageKeyword { get; }

            /// <summary>
            /// 获取类型名称字符串数组
            /// </summary>
            /// <returns></returns>
            public abstract string[] GetTypeStrings();

            /// <summary>
            /// 写入流中
            /// </summary>
            /// <param name="stream">药写入的流</param>
            /// <param name="value">写入的值</param>
            public abstract void WriteToStream(BinaryWriter stream, string value);
        }
    }
}
