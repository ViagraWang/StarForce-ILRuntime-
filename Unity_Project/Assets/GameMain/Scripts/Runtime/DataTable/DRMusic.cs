// 此文件由工具自动生成，请勿直接修改。
// 生成时间：2019-07-04 17:22:16.316
//------------------------------------------------------------
using GameFramework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityGameFrame.Runtime;

namespace Game.Runtime
{

	/// <summary>
	/// 音乐配置表
	/// </summary>
	public class DRMusic : DataRowBase
	{
		private int m_Id = 0;

		/// <summary>
		/// 获取音乐编号
		/// </summary>
		public override int Id
		{
			get
			{
				return m_Id;
			}
		}

	    /// <summary>
    /// 获取资源名称
    /// </summary>
    public string AssetName { get; private set; }

	    public override bool ParseDataRow(GameFrameworkSegment<string> dataRowSegment)
    {
        try
        {
            // Star Force 示例代码，正式项目使用时请调整此处的生成代码，以处理 GCAlloc 问题！
            string[] columnTexts = dataRowSegment.Source.Substring(dataRowSegment.Offset, dataRowSegment.Length).Split(DataTableExtension.DataSplitSeparators);
            for (int i = 0; i < columnTexts.Length; i++)
            {
                columnTexts[i] = columnTexts[i].Trim(DataTableExtension.DataTrimSeparators);
            }

            int index = 0;
            index++;
            m_Id = int.Parse(columnTexts[index++]);
            index++;
            AssetName = columnTexts[index++];

            GeneratePropertyArray();
            return true;
        }
        catch (Exception e)
        {
            Log.Error("ParseDataRow is failure, error message is:\n{0}.", e.ToString());
            return false;
        }
    }

	    public override bool ParseDataRow(GameFrameworkSegment<byte[]> dataRowSegment)
    {
        // Star Force 示例代码，正式项目使用时请调整此处的生成代码，以处理 GCAlloc 问题！
        using (MemoryStream memoryStream = new MemoryStream(dataRowSegment.Source, dataRowSegment.Offset, dataRowSegment.Length, false))
        {
            using (BinaryReader binaryReader = new BinaryReader(memoryStream, Encoding.UTF8))
            {
                try
                {
                    m_Id = binaryReader.ReadInt32();
                    AssetName = binaryReader.ReadString();
                }
                catch (Exception e)
                {
                    Log.Error("ParseDataRow is failure, error message is:\n{0}.", e.ToString());
                    return false;
                }
            }
        }

        GeneratePropertyArray();
        return true;
    }

	    public override bool ParseDataRow(GameFrameworkSegment<Stream> dataRowSegment)
    {
        Log.Warning("Not implemented ParseDataRow(GameFrameworkSegment<Stream>)");
        return false;
    }

	    private void GeneratePropertyArray()
    {

    }
	}

}