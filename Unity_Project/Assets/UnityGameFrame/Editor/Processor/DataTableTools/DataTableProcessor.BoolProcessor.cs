using System.IO;

namespace UnityGameFrame.Editor.Processor
{
    public sealed partial class DataTableProcessor
    {
        /// <summary>
        /// 布尔类型处理器
        /// </summary>
        private sealed class BoolProcessor : GenericDataProcessor<bool>
        {

            public override bool IsSystem { get { return true; } }

            public override string LanguageKeyword { get { return "bool"; } }

            public override string[] GetTypeStrings()
            {
                return new string[] { "bool", "boolean", "system.boolean" };
            }

            public override bool Parse(string value)
            {
                return bool.Parse(value);
            }

            public override void WriteToStream(BinaryWriter stream, string value)
            {
                stream.Write(Parse(value));
            }

        }
    }
}
