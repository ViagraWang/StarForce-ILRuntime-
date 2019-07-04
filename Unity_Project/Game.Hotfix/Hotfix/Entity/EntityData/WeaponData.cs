using GameFramework.DataTable;
using Game.Runtime;
using System;

namespace Game.Hotfix
{	
	/// <summary>
	/// 武器数据
	/// </summary>
	public class WeaponData : AccessoryObjectData
	{
        /// <summary>
        /// 攻击力。
        /// </summary>
        public int Attack { get; } = 0;

        /// <summary>
        /// 攻击间隔。
        /// </summary>
        public float AttackInterval { get; } = 0f;

        /// <summary>
        /// 子弹编号。
        /// </summary>
        public int BulletId { get; } = 0;

        /// <summary>
        /// 子弹速度。
        /// </summary>
        public float BulletSpeed { get; } = 0f;

        /// <summary>
        /// 子弹声音编号。
        /// </summary>
        public int BulletSoundId { get; } = 0;

        public WeaponData(int entityId, int typeId, int ownerId, CampType ownerCamp)
	        :base(entityId, typeId, ownerId, ownerCamp)
	    {
	        IDataTable<DRWeapon> dtWeapon = GameEntry.DataTable.GetDataTable<DRWeapon>();
	        DRWeapon drWeapon = dtWeapon.GetDataRow(TypeId);
	        if (drWeapon == null)
	            return;
	
	        //武器信息
	        Attack = drWeapon.Attack;
	        AttackInterval = drWeapon.AttackInterval;
	        BulletId = drWeapon.BulletId;
	        BulletSpeed = drWeapon.BulletSpeed;
	        BulletSoundId = drWeapon.BulletSoundId;
	    }
	
	}
}
