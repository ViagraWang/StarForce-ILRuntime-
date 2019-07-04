using GameFramework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace UnityGameFrame.Editor.Processor
{
    /// <summary>
    /// 配置表处理器
    /// </summary>
    public sealed class ConfigProcessor
    {
        private const string CommentLineSeparator = "#";    //注释符
        private static readonly char[] DataSplitSeparators = new char[] { '\t' };   //分割符
        private static readonly char[] DataTrimSeparators = new char[] { '\"' };

        private readonly string[][] m_RawValues;    //行列数值
        private readonly string[] m_CommentRow; //每列的注释名称数据

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
        /// 配置表处理器构造函数
        /// </summary>
        /// <param name="configFilePath">配置表文件路径</param>
        /// <param name="encoding">编码格式</param>
        /// <param name="commentRow">每列数值的注释行</param>
        /// <param name="contentStartRow">数据内容开始行</param>
        public ConfigProcessor(string configFilePath, Encoding encoding, int? commentRow, int contentStartRow)
        {
            if (string.IsNullOrEmpty(configFilePath))
                throw new GameFrameworkException("Config file name is invalid.");

            if (!configFilePath.EndsWith(".csv"))    //只能处理txt文件
                throw new GameFrameworkException(Utility.Text.Format("Config file '{0}' is not a txt.", configFilePath));

            if (!File.Exists(configFilePath))
                throw new GameFrameworkException(Utility.Text.Format("Config file '{0}' is not exist.", configFilePath));

            string[] lines = File.ReadAllLines(configFilePath, encoding);    //读取所有行内容
            int rawRowCount = lines.Length; //行数

            int rawColumnCount = 0; //列数
            List<string[]> rawValues = new List<string[]>();    //所有行的内容
            for (int i = 0; i < lines.Length; i++)
            {
                string[] rawValue = lines[i].Split(DataSplitSeparators);    //其中一行内容
                for (int j = 0; j < rawValue.Length; j++)
                {
                    rawValue[j] = rawValue[j].Trim(DataTrimSeparators); //去除结尾符
                }

                if (i == 0) //第一行肯定是#开头
                    rawColumnCount = rawValue.Length;   //列数
                else if (rawValue.Length != rawColumnCount)
                    throw new GameFrameworkException(Utility.Text.Format("Raw Column is '{1}', but line '{0}' column is '{2}'.", i.ToString(), rawColumnCount.ToString(), rawValue.Length.ToString()));

                rawValues.Add(rawValue);    //保存所有行的内容
            }

            m_RawValues = rawValues.ToArray();  //行列值的二维数组
            Debug.Log(Utility.Text.Format("{0}文件的行数:{1}", configFilePath, m_RawValues.Length));

            //检查行参数是否越界
            if (contentStartRow < 0)
                throw new GameFrameworkException(Utility.Text.Format("Content start row '{0}' is invalid.", contentStartRow.ToString()));
            if (commentRow.HasValue && commentRow.Value >= rawRowCount)
                throw new GameFrameworkException(Utility.Text.Format("Comment row '{0}' >= raw row count '{1}' is not allow.", commentRow.Value.ToString(), rawRowCount.ToString()));

            //获取数据
            ContentStartRow = contentStartRow;    //保存内容开始的行
            m_CommentRow = commentRow.HasValue ? m_RawValues[commentRow.Value] : null;  //保存每列的注释名称

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

        //是否是注释行
        public bool IsCommentRow(int rawRow)
        {
            if (rawRow < 0 || rawRow >= RawRowCount)
                throw new GameFrameworkException(Utility.Text.Format("Raw row '{0}' is out of range.", rawRow.ToString()));

            return GetValue(rawRow, 0).StartsWith(CommentLineSeparator);
        }

        //是否是注释列，肯定是带#的列
        public bool IsCommentColumn(int rawColumn)
        {
            if (rawColumn < 0 || rawColumn >= RawColumnCount)
                throw new GameFrameworkException(Utility.Text.Format("Raw column '{0}' is out of range.", rawColumn.ToString()));
            string comment = GetComment(rawColumn);
            return string.IsNullOrEmpty(comment) || comment.StartsWith(CommentLineSeparator);
        }

        //获取列的注释
        public string GetComment(int rawColumn)
        {
            if (rawColumn < 0 || rawColumn >= RawColumnCount)
                throw new GameFrameworkException(Utility.Text.Format("Raw column '{0}' is out of range.", rawColumn.ToString()));

            return m_CommentRow != null ? m_CommentRow[rawColumn] : null;
        }

        //创建二进制数据文件
        public bool GenerateConfigFile(string outputFileName, Encoding encoding)
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

                            for (int j = 0; j < RawColumnCount; j++)    //遍历所有的数值列数
                            {
                                if (IsCommentColumn(j)) //注释所在列，肯定是第一列
                                    continue;

                                try
                                {
                                    string value = GetValue(i, j);
                                    writer.Write(value);
                                }
                                catch
                                {
                                    Debug.LogError(Utility.Text.Format("Generate config file failure. OutputFileName='{0}' RawRow='{1}' RowColumn='{2}'.", outputFileName, i.ToString(), j.ToString()));
                                    return false;
                                }
                            }
                        }
                    }
                }

                Debug.Log(Utility.Text.Format("Parse Config '{0}' success.", outputFileName));
                return true;
            }
            catch (Exception exception)
            {
                Debug.LogError(Utility.Text.Format("Parse Config '{0}' failure, exception is '{1}'.", outputFileName, exception.Message));
                return false;
            }
        }
    }
}
