using UnityEngine;

namespace Game.Hotfix
{	
	/// <summary>
	/// 滚动背景图
	/// </summary>
	public class ScrollableBackground : MonoBehaviour
	{
	
	    [SerializeField]
	    private float m_ScrollSpeed = -0.25f;   //滚动速度
	    [SerializeField]
	    private float m_TileSize = 30f; //平铺大小
	    [SerializeField]
	    private BoxCollider m_VisibleBoundary = null;    //可见区域盒子碰撞器
	    [SerializeField]
	    private BoxCollider m_PlayerMoveBoundary = null;    //玩家移动区域盒子碰撞器
	    [SerializeField]
	    private BoxCollider m_EnemySpawnBoundary = null;    //敌人创建边界的盒子碰撞器
	
	    private Transform m_CachedTransform = null; //缓存
	    private Vector3 m_StartPosition = Vector3.zero; //开始坐标
	
	    public BoxCollider VisibleBoundary { get { return m_VisibleBoundary; } }
	
	    public BoxCollider PlayerMoveBoundary { get { return m_PlayerMoveBoundary; } }
	
	    public BoxCollider EnemySpawnBoundary { get { return m_EnemySpawnBoundary; } }
	
	    void Start ()
	    {
	        m_CachedTransform = transform;
	        m_StartPosition = m_CachedTransform.position;
	    }
		
		void Update ()
	    {
	        float newPosition = Mathf.Repeat(Time.time * m_ScrollSpeed, m_TileSize);
	        m_CachedTransform.position = m_StartPosition + Vector3.forward * newPosition;
		}
	}
}
