using Game.Runtime;
using UnityEngine;
using GameEntry = Game.Runtime.GameEntry;

namespace Game.Hotfix
{	
	/// <summary>
	///小行星实体逻辑
	/// </summary>
	public class Asteroid : TargetableObject
	{
	    private AsteroidData m_AsteroidData = null; //小行星数据
	
	    private Vector3 m_RotateSphere = Vector3.zero;
	
	    //获取撞击数据
	    public override ImpactData GetImpactData()
	    {
            return ReferencePool.Acquire<ImpactData>().Fill(m_AsteroidData.Camp, m_AsteroidData.HP, m_AsteroidData.Attack, 0);
	    }

        public override void OnInit(object userData)
	    {
	        base.OnInit(userData);
	    }

        public override void OnShow(object userData)
	    {
	        base.OnShow(userData);
	
	        m_AsteroidData = EntityData as AsteroidData;
	        if (m_AsteroidData == null)
	        {
                HotLog.Error("Asteroid data is invalid.");
	            return;
	        }
	
	        m_RotateSphere = Random.insideUnitSphere;
	
	    }

        public override void OnUpdate()
	    {
	        base.OnUpdate();
	
	        RuntimeEntity.CachedTransform.Translate(Vector3.back * m_AsteroidData.Speed * HotfixEntry.deltaTime, Space.World);
            RuntimeEntity.CachedTransform.Rotate(m_RotateSphere * m_AsteroidData.AngularSpeed * HotfixEntry.deltaTime, Space.Self);
	    }

        protected override void OnDead(EntityLogicBase attacker)
	    {
	        base.OnDead(attacker);
	
	        //显示特效
	        GameEntry.Entity.ShowEffect(new EffectData(GameEntry.Entity.GenerateSerialId(), m_AsteroidData.DeadEffectId)
	        {
	            Position = RuntimeEntity.Position
	        });
	
	        //播放声音
	        GameEntry.Sound.PlaySound(m_AsteroidData.DeadSoundId);
	    }

        public override void OnHide(object userData)
        {

        }

    }
}
