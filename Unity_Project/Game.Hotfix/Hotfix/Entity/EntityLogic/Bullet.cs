using Game.Runtime;
using UnityEngine;

namespace Game.Hotfix
{	
	/// <summary>
	/// 子弹类
	/// </summary>
	public class Bullet : EntityLogicBase
	{
	    private BulletData m_BulletData = null; //子弹数据
	
	    //获取撞击数据
	    public ImpactData GetImpactData()
	    {
            return ReferencePool.Acquire<ImpactData>().Fill(m_BulletData.OwnerCamp, 0, m_BulletData.Attack, 0);
	    }

        public override void OnHide(object userData)
        {

        }

        public override void OnInit(object userData)
	    {
	        base.OnInit(userData);
	    }

        public override void OnShow(object userData)
	    {
	        base.OnShow(userData);
            m_BulletData = EntityData as BulletData;
	        if(m_BulletData == null)
	        {
                HotLog.Error("Bullet data is invalid.");
	            return;
	        }
	    }

        public override void OnUpdate()
	    {
	        base.OnUpdate();
	        RuntimeEntity.CachedTransform.Translate(Vector3.forward * m_BulletData.Speed * HotfixEntry.deltaTime, Space.World);
	    }
	
	}
}
