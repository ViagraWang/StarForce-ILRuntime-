using Game.Runtime;
using GameFramework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityGameFrame.Editor.Processor;

namespace Game.Editor
{	
	public sealed class DataTableGenerator
	{
	    //属性容器
	    private sealed class PropertyCollection
	    {
	        private readonly string m_Name; //属性名称
	        private readonly string m_LanguageKeyword;  //属性类型名称
	        private readonly List<KeyValuePair<int, string>> m_Items;   //属性内容
	
	        //构造器
	        public PropertyCollection(string name, string languageKeyword)
	        {
	            m_Name = name;
	            m_LanguageKeyword = languageKeyword;
	            m_Items = new List<KeyValuePair<int, string>>();
	        }
	
	        public string Name { get { return m_Name; } }
	
	        public string LanguageKeyword { get { return m_LanguageKeyword; } }
	
	        public int ItemCount { get { return m_Items.Count; } }
	
	        public KeyValuePair<int, string> GetItem(int index)
	        {
	            if (index < 0 || index >= m_Items.Count)
	            {
	                throw new GameFrameworkException(Utility.Text.Format("GetItem with invalid index '{0}'.", index.ToString()));
	            }
	
	            return m_Items[index];
	        }
	
	        public void AddItem(int id, string propertyName)
	        {
	            m_Items.Add(new KeyValuePair<int, string>(id, propertyName));
	        }
	    }
	
	    private const string CSharpCodePath = "Assets/GameMain/Scripts/Runtime/DataTable";  //生成的数据结构脚本路径
	    private const string CSharpCodeTemplateFileName = "Assets/GameMain/Configs/DataTableCodeTemplate.txt";  //数据结构模板路径
	    private static readonly Regex EndWithNumberRegex = new Regex(@"\d+$");  //数字结尾的正则表达式
	    private static readonly Regex NameRegex = new Regex(@"^[A-Z][A-Za-z0-9_]*$");   //名称的正则表达式
	
	    //创建数据表处理器
	    public static DataTableProcessor CreateDataTableProcessor(string dataTableName)
	    {
	        return new DataTableProcessor(Utility.Path.GetCombinePath(RuntimeAssetUtility.DataTablePath, RuntimeAssetUtility.CsvFolder, dataTableName + RuntimeAssetUtility.csvExtension), Encoding.UTF8, 1, 2, null, 3, 4, 1);
	    }
	
	    //检查数据表的数据列命名是否符合规范，因为数据列的命名要写入数据结构的命名字段中
	    public static bool CheckRawData(DataTableProcessor dataTableProcessor, string dataTableName)
	    {
	        //遍历列
	        for (int i = 0; i < dataTableProcessor.RawColumnCount; i++)
	        {
	            string name = dataTableProcessor.GetName(i);    //获取数据列的命名名称
            if (string.IsNullOrEmpty(name) || name == "#")  //第一个一般都是#，表示注释行
	                continue;
	
	            if (!NameRegex.IsMatch(name))   //正则检测
	            {
	                Debug.LogWarning(Utility.Text.Format("Check raw data failure. DataTableName='{0}' Name='{1}'", dataTableName, name));
	                return false;
	            }
	        }
	
	        return true;
	    }
	
	    //生成数据文件
	    public static void GenerateDataFile(DataTableProcessor dataTableProcessor, string dataTableName)
	    {
	        string binaryDataFileName = Utility.Path.GetCombinePath(RuntimeAssetUtility.DataTablePath, RuntimeAssetUtility.BytesFolder, dataTableName + RuntimeAssetUtility.bytesExtension);   //二进制的数据文件名
	        if (!dataTableProcessor.GenerateDataFile(binaryDataFileName, Encoding.UTF8) && File.Exists(binaryDataFileName))
	        {
	            //创建失败，并且文件存在，则删除
	            File.Delete(binaryDataFileName);
	            Debug.LogError(Utility.Text.Format("失败:生成数据表文件 -> {0}", dataTableName));
	        }
	        AssetDatabase.Refresh();
	    }
	
	    //生成脚本文件
	    public static void GenerateCodeFile(DataTableProcessor dataTableProcessor, string dataTableName)
	    {
	        dataTableProcessor.SetCodeTemplate(CSharpCodeTemplateFileName, Encoding.UTF8);
	        dataTableProcessor.SetCodeGenerator(CustomDataTableCodeGenerator);
	
	        string csharpCodeFileName = Utility.Path.GetCombinePath(CSharpCodePath, DataTableExtension.DataRowClassPrefixName + dataTableName + ".cs");
	        if (!dataTableProcessor.GenerateCodeFile(csharpCodeFileName, Encoding.UTF8, dataTableName) && File.Exists(csharpCodeFileName))
	        {
	            //创建失败，并且文件存在，则删除
	            File.Delete(csharpCodeFileName);
	            Debug.LogError(Utility.Text.Format("失败:生成数据表结构 -> {0}", dataTableName));
	        }
	
	        AssetDatabase.Refresh();
	    }
	
	    //数据表脚本生成器
	    private static void CustomDataTableCodeGenerator(DataTableProcessor dataTableProcessor, StringBuilder codeContent, object userData)
	    {
	        string dataTableName = (string)userData;
	
	        codeContent.Replace("__DATA_TABLE_CREATE_TIME__", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));    //替换数据表创建时间
	        codeContent.Replace("__DATA_TABLE_NAME_SPACE__", "Game.Runtime");  //替换数据表所在命名空间，这里去掉命名空间
	        codeContent.Replace("__DATA_TABLE_CLASS_NAME__", DataTableExtension.DataRowClassPrefixName + dataTableName);  //类名
	        codeContent.Replace("__DATA_TABLE_COMMENT__", dataTableProcessor.GetValue(0, 1)); //脚本注释名
	        codeContent.Replace("__DATA_TABLE_ID_COMMENT__", "获取" + dataTableProcessor.GetComment(dataTableProcessor.IdColumn));    //id注释
	        codeContent.Replace("__DATA_TABLE_PROPERTIES__", GenerateDataTableProperties(dataTableProcessor));
	        codeContent.Replace("__DATA_TABLE_STRING_PARSER__", GenerateDataTableStringParser(dataTableProcessor));
	        codeContent.Replace("__DATA_TABLE_BYTES_PARSER__", GenerateDataTableBytesParser(dataTableProcessor));
	        codeContent.Replace("__DATA_TABLE_STREAM_PARSER__", GenerateDataTableStreamParser(dataTableProcessor));
	        codeContent.Replace("__DATA_TABLE_PROPERTY_ARRAY__", GenerateDataTablePropertyArray(dataTableProcessor));
	    }
	
	    //生成属性
	    private static string GenerateDataTableProperties(DataTableProcessor dataTableProcessor)
	    {
	        StringBuilder stringBuilder = new StringBuilder();
	        bool firstProperty = true;  //是否是第一条属性的标志位
	        for (int i = 0; i < dataTableProcessor.RawColumnCount; i++)
	        {
	            if (dataTableProcessor.IsCommentColumn(i))  // 注释列
	                continue;
	
	            if (dataTableProcessor.IsIdColumn(i))   // 编号列
	                continue;
	
	            if (firstProperty)
	            {
	                firstProperty = false;
	            }
	            else
	            {
	                stringBuilder.AppendLine().AppendLine();    //非第一条属性，要另起两行，保持与上衣属性的间距
	            }
	
	            stringBuilder
	                .AppendLine("    /// <summary>")
	                .AppendFormat("    /// 获取{0}", dataTableProcessor.GetComment(i)).AppendLine()   //列的注释
	                .AppendLine("    /// </summary>")
	                .AppendFormat("    public {0} {1} ", dataTableProcessor.GetLanguageKeyword(i), dataTableProcessor.GetName(i))
	                .Append("{ get; private set; }");
	            //.Append(" {")
	            //.Append(" get;")
	            //.Append(" private set;")
	            //.Append(" }");
	        }
	
	        return stringBuilder.ToString();
	    }
	
	    //生成重写解析string数据的方法
	    private static string GenerateDataTableStringParser(DataTableProcessor dataTableProcessor)
	    {
	        StringBuilder stringBuilder = new StringBuilder();
	        stringBuilder
	            .AppendLine("    public override bool ParseDataRow(GameFrameworkSegment<string> dataRowSegment)")
	            .AppendLine("    {")
	            .AppendLine("        try")
	            .AppendLine("        {")
	            .AppendLine("            // Star Force 示例代码，正式项目使用时请调整此处的生成代码，以处理 GCAlloc 问题！")
	            .AppendLine("            string[] columnTexts = dataRowSegment.Source.Substring(dataRowSegment.Offset, dataRowSegment.Length).Split(DataTableExtension.DataSplitSeparators);")
	            .AppendLine("            for (int i = 0; i < columnTexts.Length; i++)")
	            .AppendLine("            {")
	            .AppendLine("                columnTexts[i] = columnTexts[i].Trim(DataTableExtension.DataTrimSeparators);")
	            .AppendLine("            }")
	            .AppendLine()
	            .AppendLine("            int index = 0;");
	
	        for (int i = 0; i < dataTableProcessor.RawColumnCount; i++)
	        {
	            if (dataTableProcessor.IsCommentColumn(i))
	            {
	                // 注释列
	                stringBuilder.AppendLine("            index++;");
	                continue;
	            }
	
	            if (dataTableProcessor.IsIdColumn(i))
	            {
	                // 编号列
	                stringBuilder.AppendLine("            m_Id = int.Parse(columnTexts[index++]);");
	                continue;
	            }
	
	            if (dataTableProcessor.IsSystem(i)) //系统类型
	            {
	                string languageKeyword = dataTableProcessor.GetLanguageKeyword(i);  //系统类型名
	                if (languageKeyword == "string")
	                {
	                    //字符串处理
	                    stringBuilder.AppendFormat("            {0} = columnTexts[index++];", dataTableProcessor.GetName(i)).AppendLine();
	                }
	                else
	                {
	                    //解析转换
	                    stringBuilder.AppendFormat("            {0} = {1}.Parse(columnTexts[index++]);", dataTableProcessor.GetName(i), languageKeyword).AppendLine();
	                }
	            }
	            else
	            {
	                //转换类型
	                stringBuilder.AppendFormat("            {0} = DataTableExtension.Parse{1}(columnTexts[index++]);", dataTableProcessor.GetName(i), dataTableProcessor.GetType(i).Name).AppendLine();
	            }
	        }
	
	        stringBuilder
	            .AppendLine()
	            .AppendLine("            GeneratePropertyArray();")
	            .AppendLine("            return true;")
	            .AppendLine("        }")
	            .AppendLine("        catch (Exception e)")
	            .AppendLine("        {")
	            .AppendLine("            Log.Error(\"ParseDataRow is failure, error message is:\\n{0}.\", e.ToString());")
	            .AppendLine("            return false;")
	            .AppendLine("        }")
	            .Append("    }");
	
	        return stringBuilder.ToString();
	    }
	
	    //生成重写解析字节流数据的方法
	    private static string GenerateDataTableBytesParser(DataTableProcessor dataTableProcessor)
	    {
	        StringBuilder stringBuilder = new StringBuilder();
	        stringBuilder
	            .AppendLine("    public override bool ParseDataRow(GameFrameworkSegment<byte[]> dataRowSegment)")
	            .AppendLine("    {")
	            .AppendLine("        // Star Force 示例代码，正式项目使用时请调整此处的生成代码，以处理 GCAlloc 问题！")
            .AppendLine("        using (MemoryStream memoryStream = new MemoryStream(dataRowSegment.Source, dataRowSegment.Offset, dataRowSegment.Length, false))")
	            .AppendLine("        {")
            .AppendLine("            using (BinaryReader binaryReader = new BinaryReader(memoryStream, Encoding.UTF8))")
	            .AppendLine("            {")
	            .AppendLine("                try")
	            .AppendLine("                {");
	
	        for (int i = 0; i < dataTableProcessor.RawColumnCount; i++)
	        {
	            if (dataTableProcessor.IsCommentColumn(i))
	            {
	                // 注释列
	                continue;
	            }
	
	            if (dataTableProcessor.IsIdColumn(i))
	            {
	                // 编号列
	                stringBuilder.AppendLine("                    m_Id = binaryReader.ReadInt32();");
	                continue;
	            }
	
	            stringBuilder.AppendFormat("                    {0} = binaryReader.Read{1}();", dataTableProcessor.GetName(i), dataTableProcessor.GetType(i).Name).AppendLine();
	        }
	
	        stringBuilder
	            .AppendLine("                }")
	            .AppendLine("                catch (Exception e)")
	            .AppendLine("                {")
	            .AppendLine("                    Log.Error(\"ParseDataRow is failure, error message is:\\n{0}.\", e.ToString());")
	            .AppendLine("                    return false;")
	            .AppendLine("                }")
	            .AppendLine("            }")
	            .AppendLine("        }")
	            .AppendLine()
	            .AppendLine("        GeneratePropertyArray();")
	            .AppendLine("        return true;")
	            .Append("    }");
	
	        return stringBuilder.ToString();
	    }
	
	    //生成重写解析内存流数据的方法
	    private static string GenerateDataTableStreamParser(DataTableProcessor dataTableProcessor)
	    {
	        StringBuilder stringBuilder = new StringBuilder();
	        stringBuilder
	            .AppendLine("    public override bool ParseDataRow(GameFrameworkSegment<Stream> dataRowSegment)")
	            .AppendLine("    {")
	            .AppendLine("        Log.Warning(\"Not implemented ParseDataRow(GameFrameworkSegment<Stream>)\");")
	            .AppendLine("        return false;")
	            .Append("    }");
	
	        return stringBuilder.ToString();
	    }
	
	
	    private static string GenerateDataTablePropertyArray(DataTableProcessor dataTableProcessor)
	    {
	        List<PropertyCollection> propertyCollections = new List<PropertyCollection>();
	        for (int i = 0; i < dataTableProcessor.RawColumnCount; i++)
	        {
	            if (dataTableProcessor.IsCommentColumn(i))
	            {
	                // 注释列
	                continue;
	            }
	
	            if (dataTableProcessor.IsIdColumn(i))
	            {
	                // 编号列
	                continue;
	            }
	
	            string name = dataTableProcessor.GetName(i);    //列名
	            if (!EndWithNumberRegex.IsMatch(name))
	                continue;
	
	            string propertyCollectionName = EndWithNumberRegex.Replace(name, string.Empty);
	            int id = int.Parse(EndWithNumberRegex.Match(name).Value);
	
	            PropertyCollection propertyCollection = null;
	            foreach (PropertyCollection pc in propertyCollections)
	            {
	                if (pc.Name == propertyCollectionName)
	                {
	                    propertyCollection = pc;
	                    break;
	                }
	            }
	
	            if (propertyCollection == null)
	            {
	                propertyCollection = new PropertyCollection(propertyCollectionName, dataTableProcessor.GetLanguageKeyword(i));
	                propertyCollections.Add(propertyCollection);
	            }
	
	            propertyCollection.AddItem(id, name);
	        }
	
	        StringBuilder stringBuilder = new StringBuilder();
	        bool firstProperty = true;
	        foreach (PropertyCollection propertyCollection in propertyCollections)
	        {
	            if (firstProperty)
	            {
	                firstProperty = false;
	            }
	            else
	            {
	                stringBuilder.AppendLine().AppendLine();
	            }
	
	            stringBuilder
	                .AppendFormat("    private KeyValuePair<int, {1}>[] m_{0} = null;", propertyCollection.Name, propertyCollection.LanguageKeyword).AppendLine()
	                .AppendLine()
	                .AppendFormat("    public int {0}Count", propertyCollection.Name).AppendLine()
	                .AppendLine("    {")
	                .AppendLine("        get")
	                .AppendLine("        {")
	                .AppendFormat("            return m_{0}.Length;", propertyCollection.Name).AppendLine()
	                .AppendLine("        }")
	                .AppendLine("    }")
	                .AppendLine()
	                .AppendFormat("    public {1} Get{0}(int id)", propertyCollection.Name, propertyCollection.LanguageKeyword).AppendLine()
	                .AppendLine("    {")
	                .AppendFormat("        foreach (KeyValuePair<int, {1}> i in m_{0})", propertyCollection.Name, propertyCollection.LanguageKeyword).AppendLine()
	                .AppendLine("        {")
	                .AppendLine("            if (i.Key == id)")
	                .AppendLine("            {")
	                .AppendLine("                return i.Value;")
	                .AppendLine("            }")
	                .AppendLine("        }")
	                .AppendLine()
	                .AppendFormat("        throw new GameFrameworkException(Utility.Text.Format(\"Get{0} with invalid id '{{0}}'.\", id));", propertyCollection.Name).AppendLine()
	                .AppendLine("    }")
	                .AppendLine()
	                .AppendFormat("    public {1} Get{0}At(int index)", propertyCollection.Name, propertyCollection.LanguageKeyword).AppendLine()
	                .AppendLine("    {")
	                .AppendFormat("        if (index < 0 || index >= m_{0}.Length)", propertyCollection.Name).AppendLine()
	                .AppendLine("        {")
	                .AppendFormat("            throw new GameFrameworkException(Utility.Text.Format(\"Get{0}At with invalid index '{{0}}'.\", index));", propertyCollection.Name).AppendLine()
	                .AppendLine("        }")
	                .AppendLine()
	                .AppendFormat("        return m_{0}[index].Value;", propertyCollection.Name).AppendLine()
	                .Append("    }");
	        }
	
	        if (propertyCollections.Count > 0)
	        {
	            stringBuilder.AppendLine().AppendLine();
	        }
	
	        stringBuilder
	            .AppendLine("    private void GeneratePropertyArray()")
	            .AppendLine("    {");
	
	        firstProperty = true;
	        foreach (PropertyCollection propertyCollection in propertyCollections)
	        {
	            if (firstProperty)
	            {
	                firstProperty = false;
	            }
	            else
	            {
	                stringBuilder.AppendLine().AppendLine();
	            }
	
	            stringBuilder
	                .AppendFormat("        m_{0} = new KeyValuePair<int, {1}>[]", propertyCollection.Name, propertyCollection.LanguageKeyword).AppendLine()
	                .AppendLine("        {");
	
	            int itemCount = propertyCollection.ItemCount;
	            for (int i = 0; i < itemCount; i++)
	            {
	                KeyValuePair<int, string> item = propertyCollection.GetItem(i);
	                stringBuilder.AppendFormat("            new KeyValuePair<int, {0}>({1}, {2}),", propertyCollection.LanguageKeyword, item.Key.ToString(), item.Value).AppendLine();
	            }
	
	            stringBuilder.Append("        };");
	        }
	
	        stringBuilder
	            .AppendLine()
	            .Append("    }");
	
	        return stringBuilder.ToString();
	    }
	    
	}
}
