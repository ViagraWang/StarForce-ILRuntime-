//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2019 Jiang Yin. All rights reserved.
// Homepage: http://gameframework.cn/
// Feedback: mailto:jiangyin@gameframework.cn
//------------------------------------------------------------

using GameFramework;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace UnityGameFrame.Editor.Processor
{
    public sealed partial class DataTableProcessor
    {
        /// <summary>
        /// 数据处理器的扩展
        /// </summary>
        private static class DataProcessorUtility
        {
            //所有数据处理器的字典
            private static readonly IDictionary<string, DataProcessor> s_DataProcessors = new SortedDictionary<string, DataProcessor>();

            static DataProcessorUtility()
            {
                System.Type dataProcessorBaseType = typeof(DataProcessor);
                Assembly assembly = Assembly.GetExecutingAssembly();
                System.Type[] types = assembly.GetTypes();
                for (int i = 0; i < types.Length; i++)
                {
                    if (!types[i].IsClass || types[i].IsAbstract)   //排除非类或抽象类
                        continue;

                    if (dataProcessorBaseType.IsAssignableFrom(types[i]))   //DataProcessor的子类
                    {
                        DataProcessor dataProcessor = (DataProcessor)Activator.CreateInstance(types[i]);
                        foreach (string typeString in dataProcessor.GetTypeStrings())
                        {
                            s_DataProcessors.Add(typeString.ToLower(), dataProcessor);
                        }
                    }
                }
            }

            //获取数据处理器
            public static DataProcessor GetDataProcessor(string type)
            {
                if (type == null)
                {
                    type = string.Empty;
                }

                DataProcessor dataProcessor = null;
                if (s_DataProcessors.TryGetValue(type.ToLower(), out dataProcessor))
                {
                    return dataProcessor;
                }

                throw new GameFrameworkException(Utility.Text.Format("Not supported data processor type '{0}'.", type));
            }
        }
    }
}
