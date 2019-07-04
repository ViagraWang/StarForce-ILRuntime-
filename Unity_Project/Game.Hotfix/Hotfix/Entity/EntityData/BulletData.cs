
namespace Game.Hotfix
{	
	/// <summary>
	/// 子弹数据
	/// </summary>
	public class BulletData : EntityData
	{
        public int OwnerId { get; } = 0;

        public CampType OwnerCamp { get; } = CampType.Unknown;

        public int Attack { get; } = 0;

        public float Speed { get; } = 0f;

        public BulletData(int entityId, int typeId, int ownerId, CampType ownerCamp, int attack, float speed)
	        :base(entityId, typeId)
	    {
	        OwnerId = ownerId;
	        OwnerCamp = ownerCamp;
	        Attack = attack;
	        Speed = speed;
	    }
	
	}
}
