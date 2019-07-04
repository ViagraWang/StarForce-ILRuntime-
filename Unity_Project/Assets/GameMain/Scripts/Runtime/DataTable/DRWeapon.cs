﻿// 此文件由工具自动生成，请勿直接修改。
// 生成时间：2019-07-04 17:22:18.441
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
	/// 武器表
	/// </summary>
	public class DRWeapon : DataRowBase
	{
		private int m_Id = 0;

		/// <summary>
		/// 获取武器编号
		/// </summary>
		public override int Id
		{
			get
			{
				return m_Id;
			}
		}

	    /// <summary>
    /// 获取攻击力
    /// </summary>
    public int Attack { get; private set; }

    /// <summary>
    /// 获取攻击间隔
    /// </summary>
    public float AttackInterval { get; private set; }

    /// <summary>
    /// 获取子弹编号
    /// </summary>
    public int BulletId { get; private set; }

    /// <summary>
    /// 获取子弹速度
    /// </summary>
    public float BulletSpeed { get; private set; }

    /// <summary>
    /// 获取子弹声音编号
    /// </summary>
    public int BulletSoundId { get; private set; }

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
            Attack = int.Parse(columnTexts[index++]);
            AttackInterval = float.Parse(columnTexts[index++]);
            BulletId = int.Parse(columnTexts[index++]);
            BulletSpeed = float.Parse(columnTexts[index++]);
            BulletSoundId = int.Parse(columnTexts[index++]);

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
                    Attack = binaryReader.ReadInt32();
                    AttackInterval = binaryReader.ReadSingle();
                    BulletId = binaryReader.ReadInt32();
                    BulletSpeed = binaryReader.ReadSingle();
                    BulletSoundId = binaryReader.ReadInt32();
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