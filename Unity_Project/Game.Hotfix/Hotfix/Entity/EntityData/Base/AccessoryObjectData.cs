using System;

namespace Game.Hotfix
{	
	/// <summary>
	/// 配件对象数据
	/// </summary>
	public abstract class AccessoryObjectData : EntityData
	{
        /// <summary>
        /// 拥有者编号。
        /// </summary>
        public int OwnerId { get; } = 0;

        /// <summary>
        /// 拥有者阵营。
        /// </summary>
        public CampType OwnerCamp { get; } = CampType.Unknown;

        public AccessoryObjectData(int entityId, int typeId, int ownerId, CampType ownerCamp)
	        :base(entityId, typeId)
	    {
	        OwnerId = ownerId;
	        OwnerCamp = ownerCamp;
	    }
	
	}
}
