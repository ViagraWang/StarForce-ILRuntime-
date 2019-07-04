using Game.Runtime;
using UnityEngine;
using UnityGameFrame.Runtime;
using GameEntry = Game.Runtime.GameEntry;

namespace Game.Hotfix
{	
	/// <summary>
	/// 可命中的实体对象
	/// </summary>
	public abstract class TargetableObject : EntityLogicBase
    {
	    private TargetableObjectData m_TargetableObjectData = null;
	
	    /// <summary>
	    /// 是否死亡的标志位
	    /// </summary>
	    public bool IsDead { get { return m_TargetableObjectData.HP <= 0; } }

        /// <summary>
        /// 获取撞击数据
        /// </summary>
        /// <returns></returns>
        public abstract ImpactData GetImpactData();
	
	    /// <summary>
	    /// 实体初始化
	    /// </summary>
	    /// <param name="userData">用户自定义数据</param>
	    public override void OnInit(object userData)
	    {
	        base.OnInit(userData);
            RuntimeEntity.CachedTransform.gameObject.SetLayerRecursively(Constant.Layer.TargetableObjectLayerId);
	    }

        /// <summary>
        /// 实体显示
        /// </summary>
        /// <param name="userData">用户自定义数据</param>
        public override void OnShow(object userData)
	    {
	        base.OnShow(userData);
	        m_TargetableObjectData = EntityData as TargetableObjectData;
	        if(m_TargetableObjectData == null)
	        {
                HotLog.Error("Targetable object data is invalid -> m_TargetableObjectData == null");
	            return;
	        }
	    }
	
	    //应用伤害
	    public void ApplyDamage(EntityLogicBase attacker, int damageHP)
	    {
	        float fromHPRatio = m_TargetableObjectData.HPRatio;
	        m_TargetableObjectData.HP -= damageHP;
	        float toHPRatio = m_TargetableObjectData.HPRatio;
	            //显示血条动画
	        if (fromHPRatio > toHPRatio)
	            HotfixEntry.HPBar.ShowHPBar(RuntimeEntity, fromHPRatio, toHPRatio);
	
	        if (m_TargetableObjectData.HP <= 0)
	            OnDead(attacker);
	    }
	
	    /// <summary>
	    /// 死亡
	    /// </summary>
	    /// <param name="attacker">攻击者</param>
	    protected virtual void OnDead(EntityLogicBase attacker)
	    {
	        GameEntry.Entity.HideEntity(RuntimeEntity.Entity);
	    }
	
	    //触发检测
	    public override void OnTriggerEnter(HotEntity other)
	    {
	        if (other == null)
	            return;

            EntityLogicBase otherTarget = other.HotLogicInstance as EntityLogicBase;

            if (otherTarget != null && other.Id >= RuntimeEntity.Id)
	        {
                // 碰撞事件由 Id 小的一方处理，避免重复处理
	            return;
	        }

            AIUtility.PerformCollision(this, otherTarget);
	    }
	
	}
}
