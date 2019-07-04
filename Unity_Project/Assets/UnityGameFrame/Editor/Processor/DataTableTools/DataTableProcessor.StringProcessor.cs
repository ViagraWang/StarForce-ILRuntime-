//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2019 Jiang Yin. All rights reserved.
// Homepage: http://gameframework.cn/
// Feedback: mailto:jiangyin@gameframework.cn
//------------------------------------------------------------

using System.IO;

namespace UnityGameFrame.Editor.Processor
{
    public sealed partial class DataTableProcessor
    {
        private sealed class StringProcessor : GenericDataProcessor<string>
        {
            public override bool IsSystem
            {
                get
                {
                    return true;
                }
            }

            public override string LanguageKeyword
            {
                get
                {
                    return "string";
                }
            }

            public override string[] GetTypeStrings()
            {
                return new string[]
                {
                    "string",
                    "system.string"
                };
            }

            public override string Parse(string value)
            {
                return value;
            }

            public override void WriteToStream(BinaryWriter stream, string value)
            {
                stream.Write(Parse(value));
            }
        }
    }
}
