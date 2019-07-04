using System;

namespace Game.Hotfix
{	
	public class EffectData : EntityData
	{
        /// <summary>
        /// 存在时间
        /// </summary>
        public float KeepTime { get; } = 0f;

        public EffectData(int entityId, int typeId)
	    : base(entityId, typeId)
	    {
	        KeepTime = 3f;
	    }
	
	}
}
