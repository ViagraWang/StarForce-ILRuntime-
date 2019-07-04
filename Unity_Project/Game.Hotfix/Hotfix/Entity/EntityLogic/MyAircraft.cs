using Game.Runtime;
using UnityEngine;

namespace Game.Hotfix
{	
	/// <summary>
	/// 我的战机实体逻辑类
	/// </summary>
	public class MyAircraft : Aircraft
	{
	    private MyAircraftData m_MyAircraftData = null; //我的战机数据
	
	    private Rect m_PlayerMoveBoundary = default(Rect);  //移动边框
	    private Vector3 m_TargetPosition = Vector3.zero;    //移动的目标位置

        public override void OnInit(object userData)
	    {
	        base.OnInit(userData);
	    }

        public override void OnShow(object userData)
	    {
	        base.OnShow(userData);
	
	        //缓存我的战机数据
	        m_MyAircraftData = userData as MyAircraftData;
	        if (m_MyAircraftData == null)
	        {
                HotLog.Error("My aircraft data is invalid.");
	            return;
	        }
	        
	        ScrollableBackground sceneBackground = (ScrollableBackground)Object.FindObjectOfType(typeof(ScrollableBackground));
	        if(sceneBackground == null)
	        {
                HotLog.Warning("Can not find scene background.");
	            return;
	        }
	        //创建移动区域
	        m_PlayerMoveBoundary = new Rect(sceneBackground.PlayerMoveBoundary.bounds.min.x, sceneBackground.PlayerMoveBoundary.bounds.min.z,
	            sceneBackground.PlayerMoveBoundary.bounds.size.x, sceneBackground.PlayerMoveBoundary.bounds.size.z);
	    }

        public override void OnUpdate()
	    {
	        base.OnUpdate();
	
	        if (Input.GetMouseButton(0))
	        {
	            Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
	            m_TargetPosition = new Vector3(point.x, 0f, point.z);
	
	            for (int i = 0; i < m_ListWeapon.Count; i++)
	            {
	                m_ListWeapon[i].TryAttack();
	            }
	        }

            Vector3 pos = RuntimeEntity.Position;
            Vector3 direction = m_TargetPosition - pos;
	        if (direction.sqrMagnitude <= Vector3.kEpsilon)
	            return;
	
	        Vector3 speed = Vector3.ClampMagnitude(direction.normalized * m_MyAircraftData.Speed * HotfixEntry.deltaTime, direction.magnitude);
            RuntimeEntity.Position = new Vector3
	            (
	            Mathf.Clamp(pos.x + speed.x, m_PlayerMoveBoundary.xMin, m_PlayerMoveBoundary.xMax),
	            0f,
	            Mathf.Clamp(pos.z + speed.z, m_PlayerMoveBoundary.yMin, m_PlayerMoveBoundary.yMax)
	            );
	    }
	
	}
}
