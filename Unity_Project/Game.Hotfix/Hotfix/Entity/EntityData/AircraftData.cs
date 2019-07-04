using GameFramework.DataTable;
using Game.Runtime;
using System.Collections.Generic;

namespace Game.Hotfix
{	
	/// <summary>
	/// 战机数据
	/// </summary>
	public class AircraftData : TargetableObjectData
	{
        private int m_MaxHP = 0;    //最大生命值

        /// <summary>
        /// 推进器数据
        /// </summary>
        public ThrusterData ThrusterData { get; } = null;

        /// <summary>
        /// 武器数据列表
        /// </summary>
        public List<WeaponData> AllWeaponData { get; } = new List<WeaponData>();

        /// <summary>
        /// 装甲数据列表
        /// </summary>
        public List<ArmorData> AllArmorData { get; } = new List<ArmorData>();

        /// <summary>
        /// 最大生命值
        /// </summary>
        public override int MaxHP { get { return m_MaxHP; } }

        /// <summary>
        /// 防御。
        /// </summary>
        public int Defense { get; private set; } = 0;

        /// <summary>
        /// 速度。
        /// </summary>
        public float Speed { get { return ThrusterData.Speed; } }

        /// <summary>
        /// 死亡特效id
        /// </summary>
        public int DeadEffectId { get; } = 0;

        /// <summary>
        /// 死亡声音id
        /// </summary>
        public int DeadSoundId { get; } = 0;

        public AircraftData(int entityId, int typeId, CampType camp)
	        :base(entityId, typeId, camp)
	    {
	        IDataTable<DRAircraft> dtAircraft = GameEntry.DataTable.GetDataTable<DRAircraft>();
	        DRAircraft drAircraft = dtAircraft.GetDataRow(TypeId);
	        if (drAircraft == null)
	            return;
	
	        //创建推进器数据
	        ThrusterData = new ThrusterData(GameEntry.Entity.GenerateSerialId(), drAircraft.ThrusterId, Id, Camp);
	        //附加武器数据
	        for(int index = 0, weaponId = 0; (weaponId = drAircraft.GetWeaponIdAt(index)) > 0; index++)
	        {
	            AttachWeaponData(new WeaponData(GameEntry.Entity.GenerateSerialId(), weaponId, Id, camp));
	        }
	        //附加装甲数据
	        for (int index = 0, armorId = 0; (armorId = drAircraft.GetArmorIdAt(index)) > 0; index++)
	        {
	            AttachArmorData(new ArmorData(GameEntry.Entity.GenerateSerialId(), armorId, Id, camp));
	        }
	
	        DeadEffectId = drAircraft.DeadEffectId;   //死亡特效id
	        DeadSoundId = drAircraft.DeadSoundId; //死亡声音id
	
	        HP = m_MaxHP;
	    }
	
	    //附加武器数据
	    public void AttachWeaponData(WeaponData weaponData)
	    {
	        if (weaponData == null)
	            return;
	
	        if (AllWeaponData.Contains(weaponData))
	            return;
	
	        AllWeaponData.Add(weaponData);
	    }
	
	    //解除武器数据
	    public void DetachWeaponData(WeaponData weaponData)
	    {
	        if (weaponData == null)
	            return;
	
	        AllWeaponData.Remove(weaponData);
	    }
	
	    //附加装甲数据
	    public void AttachArmorData(ArmorData armorData)
	    {
	        if (armorData == null)
	            return;
	
	        if (AllArmorData.Contains(armorData))
	            return;
	
	        AllArmorData.Add(armorData);
	        RefreshData();  //刷新数据
	    }
	
	    //解除装甲数据
	    public void DetachArmorData(ArmorData armorData)
	    {
	        if (armorData == null)
	            return;
	
	        AllArmorData.Remove(armorData);
	        RefreshData();
	    }
	
	    //刷新数据
	    private void RefreshData()
	    {
	        m_MaxHP = 0;
	        Defense = 0;
	        for (int i = 0; i < AllArmorData.Count; i++)
	        {
	            m_MaxHP += AllArmorData[i].MaxHP;
	            Defense += AllArmorData[i].Defense;
	        }
	
	        if (HP > m_MaxHP)
	            HP = m_MaxHP;
	    }
	
	}
}
