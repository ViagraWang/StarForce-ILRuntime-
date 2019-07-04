using GameFramework;
using GameFramework.DataTable;
using Game.Runtime;
using UnityEngine;

namespace Game.Hotfix
{	
	//生存模式游戏
	public class SurvivalGame : GameBase
	{
	    private const float m_AsteroidInterval = 1f;    //创建陨石的频率
	    private float m_ElapseSeconds = 0f; //计算创建陨石时间
	
	    public override GameMode GameMode { get { return GameMode.Survival; } }
	
	    public override void Update(float elapseSeconds, float realElapseSeconds)
	    {
	        base.Update(elapseSeconds, realElapseSeconds);
	
	        m_ElapseSeconds += elapseSeconds;
	        if(m_ElapseSeconds >= m_AsteroidInterval)
	        {
	            m_ElapseSeconds = 0f;
	            IDataTable<DRAsteroid> dtAsteroid = GameEntry.DataTable.GetDataTable<DRAsteroid>();
	            float randomPositionX = SceneBackground.EnemySpawnBoundary.bounds.min.x + SceneBackground.EnemySpawnBoundary.bounds.size.x * (float)Utility.Random.GetRandomDouble();
	            float randomPositionZ = SceneBackground.EnemySpawnBoundary.bounds.min.z + SceneBackground.EnemySpawnBoundary.bounds.size.z * (float)Utility.Random.GetRandomDouble();
	            GameEntry.Entity.ShowAsteroid(new AsteroidData(GameEntry.Entity.GenerateSerialId(), 60000 + Utility.Random.GetRandom(dtAsteroid.Count))
	            {
	                Position = new Vector3(randomPositionX, 0f, randomPositionZ)    //位置坐标
	            });
	
	        }
	
	    }
	
	}
}
