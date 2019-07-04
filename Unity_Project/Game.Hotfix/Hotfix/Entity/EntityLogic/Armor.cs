using Game.Runtime;
using GameFramework;
using UnityEngine;
using GameEntry = Game.Runtime.GameEntry;

namespace Game.Hotfix
{	
	/// <summary>
	/// 装甲实体逻辑
	/// </summary>
	public class Armor : EntityLogicBase
	{
	    private const string AttachPointName = "Armor Point";
	
	    private ArmorData m_ArmorData = null;   //装甲数据

        public override void OnInit(object userData)
	    {
	        base.OnInit(userData);
	    }


        public override void OnShow(object userData)
	    {
	        base.OnShow(userData);
	        //装甲数据
	        m_ArmorData = userData as ArmorData;
	        if (m_ArmorData == null)
	        {
                HotLog.Error("Armor data is invalid.");
	            return;
	        }
	
	        //绑定到拥有者实体上
	        GameEntry.Entity.AttachEntity(this, m_ArmorData.OwnerId, AttachPointName);
	    }

        public override void OnAttachTo(EntityLogicBase parentEntity, Transform parentTransform, object userData)
	    {
	        base.OnAttachTo(parentEntity, parentTransform, userData);
	
	        RuntimeEntity.Name = Utility.Text.Format("Armor of {0}", parentEntity.RuntimeEntity.Name);
            RuntimeEntity.LocalPosition = Vector3.zero;
	    }

        public override void OnHide(object userData)
        {

        }
    }
}
