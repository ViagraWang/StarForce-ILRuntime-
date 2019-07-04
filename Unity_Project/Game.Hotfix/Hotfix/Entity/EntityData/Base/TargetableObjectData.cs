using System;

namespace Game.Hotfix
{	
	/// <summary>
	/// 可命中的实体数据
	/// </summary>
	public abstract class TargetableObjectData : EntityData
	{

        /// <summary>
        /// 角色阵营
        /// </summary>
        public CampType Camp { get; } = CampType.Unknown;

        /// <summary>
        /// 当前生命值
        /// </summary>
        public int HP { get; set; } = 0;

        /// <summary>
        /// 最大生命值
        /// </summary>
        public abstract int MaxHP { get; }
	
	    /// <summary>
	    /// 生命百分比
	    /// </summary>
	    public float HPRatio { get { return MaxHP > 0 ? (float)HP / MaxHP : 0f; } }
	
	    public TargetableObjectData(int entityId, int typeId, CampType camp)
	        :base(entityId, typeId)
	    {
	        Camp = camp;
	        HP = 0;
	    }
	
	}
}
