using Game.Runtime;
using GameFramework;
using UnityEngine;
using GameEntry = Game.Runtime.GameEntry;

namespace Game.Hotfix
{	
	/// <summary>
	/// 武器实体逻辑类
	/// </summary>
	public class Weapon : EntityLogicBase
	{
	    private const string AttachPointName = "Weapon Point";
	
	    private WeaponData m_WeaponData = null; //武器数据
	
	    private float m_NextAttackTime = 0f;    //下次攻击时间

        public override void OnInit(object userData)
	    {
	        base.OnInit(userData);
	    }

        public override void OnShow(object userData)
	    {
	        base.OnShow(userData);
	
	        m_WeaponData = userData as WeaponData;
	        if (m_WeaponData == null)
	        {
                HotLog.Error("Weapon data is invalid.");
	            return;
	        }
	
	        //挂载到拥有者实体上
	        GameEntry.Entity.AttachEntity(this, m_WeaponData.OwnerId, AttachPointName);
	    }
	
	    public override void OnAttachTo(EntityLogicBase parentEntity, Transform parentTransform, object userData)
	    {
	        base.OnAttachTo(parentEntity, parentTransform, userData);
	
	        RuntimeEntity.Name = Utility.Text.Format("Weapon of {0}", parentEntity.RuntimeEntity.Name);
            RuntimeEntity.LocalPosition = Vector3.zero;
	    }
	
	    //尝试攻击
	    public void TryAttack()
	    {
            float time = Time.time;

            if (time < m_NextAttackTime)
	            return;
	
	        m_NextAttackTime = time + m_WeaponData.AttackInterval;

            //显示子弹实体
            GameEntry.Entity.ShowBullet(new BulletData(GameEntry.Entity.GenerateSerialId(), m_WeaponData.BulletId, m_WeaponData.OwnerId,
	            m_WeaponData.OwnerCamp, m_WeaponData.Attack, m_WeaponData.BulletSpeed)
	        {
                Position = RuntimeEntity.Position //赋值位置
	        });
	        //播放音效
	        GameEntry.Sound.PlaySound(m_WeaponData.BulletSoundId);
	    }

        public override void OnHide(object userData)
        {

        }
    }
}
