using Game.Runtime;
using GameEntry = Game.Runtime.GameEntry;

namespace Game.Hotfix
{	
	/// <summary>
	/// 特效类
	/// </summary>
	public class Effect : EntityLogicBase
	{
	    private EffectData m_EffectData = null; //特效数据
	
	    private float m_ElapseSeconds = 0f; //记录时间

        public override void OnShow(object userData)
	    {
	        base.OnShow(userData);
	
	        m_EffectData = userData as EffectData;
	        if (m_EffectData == null)
	        {
                HotLog.Error("Effect data is invalid.");
	            return;
	        }
	
	        m_ElapseSeconds = 0f;
	    }

        public override void OnUpdate()
	    {
	        base.OnUpdate();
	
	        m_ElapseSeconds += HotfixEntry.deltaTime;
	        if(m_ElapseSeconds >= m_EffectData.KeepTime)
	        {
	            GameEntry.Entity.HideEntity(this);  //隐藏
	        }
	    }


        public override void OnHide(object userData)
        {

        }

    }
}
