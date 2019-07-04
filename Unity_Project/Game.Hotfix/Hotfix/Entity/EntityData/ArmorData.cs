using GameFramework.DataTable;
using Game.Runtime;
using System;

namespace Game.Hotfix
{	
	/// <summary>
	/// 装甲数据
	/// </summary>
	public class ArmorData : AccessoryObjectData
	{

        /// <summary>
        /// 最大生命。
        /// </summary>
        public int MaxHP { get; } = 0;

        /// <summary>
        /// 防御力。
        /// </summary>
        public int Defense { get; } = 0;

        public ArmorData(int entityId, int typeId, int ownerId, CampType ownerCamp)
	        :base(entityId, typeId, ownerId, ownerCamp)
	    {
	        IDataTable<DRArmor> dtArmor = GameEntry.DataTable.GetDataTable<DRArmor>();
	        DRArmor drArmor = dtArmor.GetDataRow(TypeId);
	        if (drArmor == null)
	            return;
	
	        MaxHP = drArmor.MaxHP;
	        Defense = drArmor.Defense;
	    }
	
	}
}
