using Game.Runtime;
using GameFramework;
using UnityEngine;
using GameEntry = Game.Runtime.GameEntry;

namespace Game.Hotfix
{	
	/// <summary>
	/// 推进器类
	/// </summary>
	public class Thruster : EntityLogicBase
	{
	    private const string AttachPointName = "Thruster Point";
	
	    private ThrusterData m_ThrusterData = null; //推进器数据

        public override void OnInit(object userData)
	    {
	        base.OnInit(userData);
	    }


        public override void OnShow(object userData)
	    {
	        base.OnShow(userData);
	
	        m_ThrusterData = userData as ThrusterData;
	        if(m_ThrusterData == null)
	        {
                HotLog.Error("Thruster data is invalid.");
	            return;
	        }
	
	        //挂载到拥有者上
	        GameEntry.Entity.AttachEntity(this, m_ThrusterData.OwnerId, AttachPointName);
	    }

        public override void OnAttachTo(EntityLogicBase parentEntity, Transform parentTransform, object userData)
	    {
	        base.OnAttachTo(parentEntity, parentTransform, userData);
	
	        RuntimeEntity.Name = Utility.Text.Format("Thruster of {0}", parentEntity.RuntimeEntity.Name);
            RuntimeEntity.LocalPosition = Vector3.zero;
	    }

        public override void OnHide(object userData)
        {

        }
    }
}
