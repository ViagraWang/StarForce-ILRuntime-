using GameFramework.DataTable;
using Game.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Game.Hotfix
{	
	/// <summary>
	/// 小行星数据
	/// </summary>
	public class AsteroidData : TargetableObjectData
	{
	    private int m_MaxHP = 0;    //最大生命值

        /// <summary>
        /// 最大生命值
        /// </summary>
        public override int MaxHP { get { return m_MaxHP; } }

        /// <summary>
        /// 攻击力
        /// </summary>
        public int Attack { get; } = 0;

        /// <summary>
        /// 移动速度
        /// </summary>
        public float Speed { get; } = 0f;

        /// <summary>
        /// 欧拉旋转速度
        /// </summary>
        public float AngularSpeed { get; } = 0f;

        /// <summary>
        /// 死亡特效id
        /// </summary>
        public int DeadEffectId { get; } = 0;

        /// <summary>
        /// 死亡声音id
        /// </summary>
        public int DeadSoundId { get; } = 0;


        public AsteroidData(int entityId, int typeId)
	        :base(entityId, typeId, CampType.Neutral)
	    {
	        IDataTable<DRAsteroid> dtAsteroid = GameEntry.DataTable.GetDataTable<DRAsteroid>();
	        DRAsteroid drAsteroid = dtAsteroid.GetDataRow(TypeId);
	        if (drAsteroid == null)
	            return;
	
	        //获取数据
	        HP = m_MaxHP = drAsteroid.MaxHP;
	        Attack = drAsteroid.Attack;
	        Speed = drAsteroid.Speed;
	        AngularSpeed = drAsteroid.AngularSpeed;
	        DeadEffectId = drAsteroid.DeadEffectId;
	        DeadSoundId = drAsteroid.DeadSoundId;
	    }
	
	}
}
