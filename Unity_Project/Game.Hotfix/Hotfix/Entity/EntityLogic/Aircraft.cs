using Game.Runtime;
using GameFramework;
using System.Collections.Generic;
using UnityEngine;
using GameEntry = Game.Runtime.GameEntry;

namespace Game.Hotfix
{	
	/// <summary>
	/// 战机逻辑类
	/// </summary>
	public abstract class Aircraft : TargetableObject
	{
	    private AircraftData m_AircraftData = null; //战机数据
	
	    protected Thruster m_Thruster = null;   //推进器
	
	    protected List<Weapon> m_ListWeapon = new List<Weapon>(); //武器列表
	
	    protected List<Armor> m_ListArmor = new List<Armor>();
	
	    public override ImpactData GetImpactData()
	    {
	        return ReferencePool.Acquire<ImpactData>().Fill(m_AircraftData.Camp, m_AircraftData.HP, 0, m_AircraftData.Defense);
	    }
	
	    public override void OnShow(object userData)
	    {
	        base.OnShow(userData);
	        m_AircraftData = EntityData as AircraftData;
	        if (m_AircraftData == null)
	        {
                HotLog.Error("Aircraft data is invalid.");
	            return;
	        }
	
	        RuntimeEntity.Name = Utility.Text.Format("Aircraft ({0})", RuntimeEntity.Id.ToString());
	        GameEntry.Entity.ShowThruster(m_AircraftData.ThrusterData); //显示推进器
	
	        //显示所有武器
	        List<WeaponData> weaponDatas = m_AircraftData.AllWeaponData;
	        for (int i = 0; i < weaponDatas.Count; i++)
	        {
	            GameEntry.Entity.ShowWeapon(weaponDatas[i]);
	        }
	
	        //显示所有装甲
	        List<ArmorData> armorDatas = m_AircraftData.AllArmorData;
	        for (int i = 0; i < armorDatas.Count; i++)
	        {
	            GameEntry.Entity.ShowArmor(armorDatas[i]);
	        }
	    }


        public override void OnHide(object userData)
	    {

        }

        //附加实体的回调
        public override void OnAttached(EntityLogicBase childEntity, Transform parentTransform, object userData)
	    {
	        base.OnAttached(childEntity, parentTransform, userData);

            if (childEntity.GetType() == typeof(Thruster))    //附加推进器
	        {
	            m_Thruster = childEntity as Thruster;
	            return;
	        }
	
	        if (childEntity.GetType() == typeof(Weapon))  //附加武器
	        {
	            m_ListWeapon.Add(childEntity as Weapon);
	            return;
	        }
	
	        if (childEntity.GetType() == typeof(Armor))   //附加装甲
	        {
	            m_ListArmor.Add(childEntity as Armor);
	            return;
	        }
	    }

        //解除实体
        public override void OnDetached(EntityLogicBase childEntity, object userData)
	    {
	        base.OnDetached(childEntity, userData);

            if (childEntity.GetType() == typeof(Thruster))    //推进器
            {
                m_Thruster = null;
	            return;
	        }

            if (childEntity.GetType() == typeof(Weapon))  //武器
            {
                m_ListWeapon.Remove(childEntity as Weapon);
	            return;
	        }

            if (childEntity.GetType() == typeof(Armor))   //装甲
            {
                m_ListArmor.Remove(childEntity as Armor);
	            return;
	        }
	    }

        protected override void OnDead(EntityLogicBase attacker)
	    {
	        base.OnDead(attacker);
	        //显示特效
	        GameEntry.Entity.ShowEffect(new EffectData(GameEntry.Entity.GenerateSerialId(), m_AircraftData.DeadEffectId)
	        {
	            Position = RuntimeEntity.Position
	        });
	
	        GameEntry.Sound.PlaySound(m_AircraftData.DeadSoundId); //播放死亡声音
	    }
	
	}
}
