using GameFramework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace UnityGameFrame.Editor.Processor
{
    /// <summary>
    /// 数据表处理器
    /// </summary>
    public sealed partial class DataTableProcessor
    {
        private const string CommentLineSeparator = "#";    //注释符
        private static readonly char[] DataSplitSeparators = new char[] { '\t' };   //分割符
        private static readonly char[] DataTrimSeparators = new char[] { '\"' };

        private readonly string[] m_NameRow;    //每列数值名描述
        private readonly string[] m_TypeRow;    //每列数值类型行
        private readonly string[] m_DefaultValueRow;    //每一列的默认值
        private readonly string[] m_CommentRow; //每列的注释名称数据

        private readonly DataProcessor[] m_DataProcessor;   //所有的数据处理器
        private readonly string[][] m_RawValues;    //行列数值

        private string m_CodeTemplate;  //脚本模板
        private DataTableCodeGenerator m_CodeGenerator; //数据表脚本生成器

        /// <summary>
        /// 行数
        /// </summary>
        public int RawRowCount { get { return m_RawValues.Length; } }

        /// <summary>
        /// 列数
        /// </summary>
        public int RawColumnCount { get { return m_RawValues.Length > 0 ? m_RawValues[0].Length : 0; } }

        /// <summary>
        /// 内容开始行
        /// </summary>
        public int ContentStartRow { get; private set; }

        /// <summary>
        /// id所在列
        /// </summary>
        public int IdColumn { get; private set; }

        /// <summary>
        /// 数据表处理器构造函数
        /// </summary>
        /// <param name="dataTableFilePath">数据表文件路径</param>
        /// <param name="encoding">编码格式</param>
        /// <param name="nameRow">每一列数值命名描述所在行</param>
        /// <param name="typeRow">每一列的数据类型所在行</param>
        /// <param name="defaultValueRow">默认值所在行</param>
        /// <param name="commentRow">每列数值的注释行</param>
        /// <param name="contentStartRow">数据内容开始行</param>
        /// <param name="idColumn">id所在列</param>
        public DataTableProcessor(string dataTableFilePath, Encoding encoding, int nameRow, int typeRow, int? defaultValueRow, int? commentRow, int contentStartRow, int idColumn)
        {
            if (string.IsNullOrEmpty(dataTableFilePath))
                throw new GameFrameworkException("Data table file name is invalid.");

            if (!dataTableFilePath.EndsWith(".csv"))    //只能处理txt文件
                throw new GameFrameworkException(Utility.Text.Format("Data table file '{0}' is not a txt.", dataTableFilePath));

            if (!File.Exists(dataTableFilePath))
                throw new GameFrameworkException(Utility.Text.Format("Data table file '{0}' is not exist.", dataTableFilePath));

            string[] lines = File.ReadAllLines(dataTableFilePath, encoding);    //读取所有行内容
            int rawRowCount = lines.Length; //行数

            int rawColumnCount = 0; //列数
            List<string[]> rawValues = new List<string[]>();    //所有行的内容
            for (int i = 0; i < lines.Length; i++)
            {
                string[] rawValue = lines[i].Split(DataSplitSeparators);    //其中一行内容
                for (int j = 0; j < rawValue.Length; j++)
                {
                    rawValue[j] = rawValue[j].Trim(DataTrimSeparators); //去除引号
                }

                if (i == 0) //第一列肯定是#
                    rawColumnCount = rawValue.Length;   //列数
                else if (rawValue.Length != rawColumnCount)
                    throw new GameFrameworkException(Utility.Text.Format("Raw Column is '{1}', but line '{0}' column is '{2}'.", i.ToString(), rawColumnCount.ToString(), rawValue.Length.ToString()));

                rawValues.Add(rawValue);    //保存所有行的内容
            }

            m_RawValues = rawValues.ToArray();  //行列值的二维数组
            //Debug.Log(Utility.Text.Format("{0}文件的行数:{1}", dataTableFilePath, m_RawValues.Length));

            //检查行参数是否越界
            if (nameRow < 0)
                throw new GameFrameworkException(Utility.Text.Format("Name row '{0}' is invalid.", nameRow.ToString()));
            if (typeRow < 0)
                throw new GameFrameworkException(Utility.Text.Format("Type row '{0}' is invalid.", typeRow.ToString()));
            if (contentStartRow < 0)
                throw new GameFrameworkException(Utility.Text.Format("Content start row '{0}' is invalid.", contentStartRow.ToString()));
            if (idColumn < 0)
                throw new GameFrameworkException(Utility.Text.Format("Id column '{0}' is invalid.", idColumn.ToString()));
            if (nameRow >= rawRowCount)
                throw new GameFrameworkException(Utility.Text.Format("Name row '{0}' >= raw row count '{1}' is not allow.", nameRow.ToString(), rawRowCount.ToString()));
            if (typeRow >= rawRowCount)
                throw new GameFrameworkException(Utility.Text.Format("Type row '{0}' >= raw row count '{1}' is not allow.", typeRow.ToString(), rawRowCount.ToString()));
            if (defaultValueRow.HasValue && defaultValueRow.Value >= rawRowCount)
                throw new GameFrameworkException(Utility.Text.Format("Default value row '{0}' >= raw row count '{1}' is not allow.", defaultValueRow.Value.ToString(), rawRowCount.ToString()));
            if (commentRow.HasValue && commentRow.Value >= rawRowCount)
                throw new GameFrameworkException(Utility.Text.Format("Comment row '{0}' >= raw row count '{1}' is not allow.", commentRow.Value.ToString(), rawRowCount.ToString()));
            if (contentStartRow > rawRowCount)
                throw new GameFrameworkException(Utility.Text.Format("Content start row '{0}' > raw row count '{1}' is not allow.", contentStartRow.ToString(), rawRowCount.ToString()));
            if (idColumn >= rawColumnCount)
                throw new GameFrameworkException(Utility.Text.Format("Id column '{0}' >= raw column count '{1}' is not allow.", idColumn.ToString(), rawColumnCount.ToString()));

            //获取数据
            m_NameRow = m_RawValues[nameRow];
            m_TypeRow = m_RawValues[typeRow];
            m_DefaultValueRow = defaultValueRow.HasValue ? m_RawValues[defaultValueRow.Value] : null;
            m_CommentRow = commentRow.HasValue ? m_RawValues[commentRow.Value] : null;
            ContentStartRow = contentStartRow;    //保存内容开始的行
            IdColumn = idColumn;  //保存id所在的列

            m_DataProcessor = new DataProcessor[rawColumnCount];
            for (int i = 0; i < rawColumnCount; i++)
            {
                if (i == IdColumn)
                    m_DataProcessor[i] = DataProcessorUtility.GetDataProcessor("id");   //获取id的数据处理器
                else
                    m_DataProcessor[i] = DataProcessorUtility.GetDataProcessor(m_TypeRow[i]);   //获取其他类型的
            }

            m_CodeTemplate = null;
            m_CodeGenerator = null;
        }

        //判断是否是id的列
        public bool IsIdColumn(int rawColumn)
        {
            if (rawColumn < 0 || rawColumn >= RawColumnCount)
                throw new GameFrameworkException(Utility.Text.Format("Raw column '{0}' is out of range.", rawColumn.ToString()));

            return m_DataProcessor[rawColumn].IsId;
        }

        //是否是注释行
        public bool IsCommentRow(int rawRow)
        {
            if (rawRow < 0 || rawRow >= RawRowCount)
                throw new GameFrameworkException(Utility.Text.Format("Raw row '{0}' is out of range.", rawRow.ToString()));

            return GetValue(rawRow, 0).StartsWith(CommentLineSeparator);
        }

        //是否是注释列
        public bool IsCommentColumn(int rawColumn)
        {
            if (rawColumn < 0 || rawColumn >= RawColumnCount)
                throw new GameFrameworkException(Utility.Text.Format("Raw column '{0}' is out of range.", rawColumn.ToString()));

            return string.IsNullOrEmpty(GetName(rawColumn)) || m_DataProcessor[rawColumn].IsComment;
        }

        //获取列的名称
        public string GetName(int rawColumn)
        {
            if (rawColumn < 0 || rawColumn >= RawColumnCount)
                throw new GameFrameworkException(Utility.Text.Format("Raw column '{0}' is out of range.", rawColumn.ToString()));

            //这里进行多余的判断？？？？？
            //if (IsIdColumn(rawColumn))
            //    return "Id";

            return m_NameRow[rawColumn];
        }

        //是否是系统类型数值的列
        public bool IsSystem(int rawColumn)
        {
            if (rawColumn < 0 || rawColumn >= RawColumnCount)
                throw new GameFrameworkException(Utility.Text.Format("Raw column '{0}' is out of range.", rawColumn.ToString()));

            return m_DataProcessor[rawColumn].IsSystem;
        }

        //获取列的类型
        public System.Type GetType(int rawColumn)
        {
            if (rawColumn < 0 || rawColumn >= RawColumnCount)
                throw new GameFrameworkException(Utility.Text.Format("Raw column '{0}' is out of range.", rawColumn.ToString()));

            return m_DataProcessor[rawColumn].Type;
        }

        //获取类型名
        public string GetLanguageKeyword(int rawColumn)
        {
            if (rawColumn < 0 || rawColumn >= RawColumnCount)
                throw new GameFrameworkException(Utility.Text.Format("Raw column '{0}' is out of range.", rawColumn.ToString()));

            return m_DataProcessor[rawColumn].LanguageKeyword;
        }

        //获取默认值
        public string GetDefaultValue(int rawColumn)
        {
            if (rawColumn < 0 || rawColumn >= RawColumnCount)
                throw new GameFrameworkException(Utility.Text.Format("Raw column '{0}' is out of range.", rawColumn.ToString()));

            return m_DefaultValueRow != null ? m_DefaultValueRow[rawColumn] : null;
        }

        //获取注释列
        public string GetComment(int rawColumn)
        {
            if (rawColumn < 0 || rawColumn >= RawColumnCount)
                throw new GameFrameworkException(Utility.Text.Format("Raw column '{0}' is out of range.", rawColumn.ToString()));

            return m_CommentRow != null ? m_CommentRow[rawColumn] : null;
        }

        //获取行列值
        public string GetValue(int rawRow, int rawColumn)
        {
            if (rawRow < 0 || rawRow >= RawRowCount)
                throw new GameFrameworkException(Utility.Text.Format("Raw row '{0}' is out of range.", rawRow.ToString()));

            if (rawColumn < 0 || rawColumn >= RawColumnCount)
                throw new GameFrameworkException(Utility.Text.Format("Raw column '{0}' is out of range.", rawColumn.ToString()));

            return m_RawValues[rawRow][rawColumn];
        }

        //创建二进制数据文件
        public bool GenerateDataFile(string outputFileName, Encoding encoding)
        {
            if (string.IsNullOrEmpty(outputFileName))
                throw new GameFrameworkException("Output file name is invalid.");

            try
            {
                string dir = Path.GetDirectoryName(outputFileName);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                using (FileStream fileStream = new FileStream(outputFileName, FileMode.Create))
                {
                    using (BinaryWriter writer = new BinaryWriter(fileStream, encoding))
                    {
                        for (int i = ContentStartRow; i < RawRowCount; i++) //遍历所有的数值行数
                        {
                            if (IsCommentRow(i))    //注释所在行
                                continue;

                            int startPosition = (int)writer.BaseStream.Position;
                            writer.BaseStream.Position += sizeof(int);  //空余4个字节，最后写入数据长度
                            for (int j = 0; j < RawColumnCount; j++)    //遍历所有的数值列数
                            {
                                if (IsCommentColumn(j)) //注释所在列
                                    continue;

                                try
                                {
                                    //m_DataProcessor[j].WriteToStream(writer, GetValue(i, j));   //数据写入流
                                    m_DataProcessor[j].WriteToStream(writer, GetValue(i, j) ?? GetDefaultValue(j));   //数据写入流
                                }
                                catch
                                {
                                    if (m_DataProcessor[j].IsId || string.IsNullOrEmpty(GetDefaultValue(j)))
                                    {
                                        Debug.LogError(Utility.Text.Format("Parse raw value failure. OutputFileName='{0}' RawRow='{1}' RowColumn='{2}' Name='{3}' Type='{4}' RawValue='{5}'", outputFileName, i.ToString(), j.ToString(), GetName(j), GetLanguageKeyword(j), GetValue(i, j)));
                                        return false;
                                    }
                                    else
                                    {
                                        //Debug.LogWarning(Utility.Text.Format("Parse raw value failure, will try default value. OutputFileName='{0}' RawRow='{1}' RowColumn='{2}' Name='{3}' Type='{4}' RawValue='{5}'", outputFileName, i.ToString(), j.ToString(), GetName(j), GetLanguageKeyword(j), GetValue(i, j)));
                                        //try
                                        //{
                                        //    m_DataProcessor[j].WriteToStream(writer, GetDefaultValue(j));   //写入默认值
                                        //}
                                        //catch
                                        //{
                                        //    Debug.LogError(Utility.Text.Format("Parse default value failure. OutputFileName='{0}' RawRow='{1}' RowColumn='{2}' Name='{3}' Type='{4}' RawValue='{5}'", outputFileName, i.ToString(), j.ToString(), GetName(j), GetLanguageKeyword(j), GetComment(j)));
                                        //    return false;
                                        //}
                                        Debug.LogError(Utility.Text.Format("Parse default value failure. OutputFileName='{0}' RawRow='{1}' RowColumn='{2}' Name='{3}' Type='{4}' RawValue='{5}'", outputFileName, i.ToString(), j.ToString(), GetName(j), GetLanguageKeyword(j), GetComment(j)));
                                        return false;
                                    }
                                }
                            }

                            //写入一行成功后，写入该行的数据长度
                            int endPosition = (int)writer.BaseStream.Position;
                            int length = endPosition - startPosition - sizeof(int);
                            writer.BaseStream.Position = startPosition;
                            writer.Write(length);
                            writer.BaseStream.Position = endPosition;
                        }
                    }
                }

                Debug.Log(Utility.Text.Format("Parse data table '{0}' success.", outputFileName));
                return true;
            }
            catch (Exception exception)
            {
                Debug.LogError(Utility.Text.Format("Parse data table '{0}' failure, exception is '{1}'.", outputFileName, exception.Message));
                return false;
            }
        }

        //设置脚本模板
        public bool SetCodeTemplate(string codeTemplateFileName, Encoding encoding)
        {
            try
            {
                m_CodeTemplate = File.ReadAllText(codeTemplateFileName, encoding);
                Debug.Log(Utility.Text.Format("Set code template '{0}' success.", codeTemplateFileName));
                return true;
            }
            catch (Exception exception)
            {
                Debug.LogError(Utility.Text.Format("Set code template '{0}' failure, exception is '{1}'.", codeTemplateFileName, exception.Message));
                return false;
            }
        }

        //设置脚本生成器
        public void SetCodeGenerator(DataTableCodeGenerator codeGenerator)
        {
            m_CodeGenerator = codeGenerator;
        }

        //生成脚本文件
        public bool GenerateCodeFile(string outputFileName, Encoding encoding, object userData = null)
        {
            if (string.IsNullOrEmpty(m_CodeTemplate))
                throw new GameFrameworkException("You must set code template first.");

            if (string.IsNullOrEmpty(outputFileName))
                throw new GameFrameworkException("Output file name is invalid.");

            try
            {
                StringBuilder stringBuilder = new StringBuilder(m_CodeTemplate);
                if (m_CodeGenerator != null)
                {
                    m_CodeGenerator(this, stringBuilder, userData);
                }

                using (FileStream fileStream = new FileStream(outputFileName, FileMode.Create))
                {
                    using (StreamWriter stream = new StreamWriter(fileStream, encoding))
                    {
                        stream.Write(stringBuilder.ToString());
                    }
                }

                Debug.Log(Utility.Text.Format("Generate code file '{0}' success.", outputFileName));
                return true;
            }
            catch (Exception exception)
            {
                Debug.LogError(Utility.Text.Format("Generate code file '{0}' failure, exception is: \n'{1}'.", outputFileName, exception.ToString()));
                return false;
            }
        }
    }
}
