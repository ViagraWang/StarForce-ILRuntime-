using GameFramework.DataTable;
using Game.Runtime;
using System;

namespace Game.Hotfix
{	
	/// <summary>
	/// 推进器数据
	/// </summary>
	public class ThrusterData : AccessoryObjectData
	{
        /// <summary>
        /// 速度
        /// </summary>
        public float Speed { get; } = 0f;

        public ThrusterData(int entityId, int typeId, int ownerId, CampType ownerCamp)
	        : base(entityId, typeId, ownerId, ownerCamp)
	    {
	        IDataTable<DRThruster> dtThruster = GameEntry.DataTable.GetDataTable<DRThruster>(); //获取数据表
	        DRThruster drThruster = dtThruster.GetDataRow(TypeId);  //获取其中一条数据
	        if (drThruster == null)
	            return;
	
	        Speed = drThruster.Speed;
	    }
	
	}
}
