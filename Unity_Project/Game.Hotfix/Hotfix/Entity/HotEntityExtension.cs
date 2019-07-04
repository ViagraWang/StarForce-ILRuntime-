using Game.Runtime;
using System;
using UnityGameFrame.Runtime;

namespace Game.Hotfix
{	
	/// <summary>
	/// 实体扩展器
	/// </summary>
	public static class HotEntityExtension {
	
	    // 关于 EntityId 的约定：
	    // 0 为无效
	    // 正值用于和服务器通信的实体（如玩家角色、NPC、怪等，服务器只产生正值）
	    // 负值用于本地生成的临时实体（如特效、FakeObject等）
	    private static int s_SerialId = 0;
	
	    /// <summary>
	    /// 生成实体编号
	    /// </summary>
	    /// <param name="entityComponent">实体组件</param>
	    /// <returns>实体编号</returns>
	    public static int GenerateSerialId(this EntityComponent entityComponent)
	    {
	        return --s_SerialId;
	    }
	
	    /// <summary>
	    /// 根据实体id获取实体
	    /// </summary>
	    /// <param name="entityComponent">实体组件</param>
	    /// <param name="entityId">实体编号</param>
	    /// <returns>逻辑实体</returns>
	    public static EntityLogicBase GetGameEntity(this EntityComponent entityComponent, int entityId)
	    {
	        UnityGameFrame.Runtime.Entity entity = entityComponent.GetEntity(entityId);
            HotEntity hotEntity = entity.Logic as HotEntity;
            if (hotEntity == null)
	            return null;
	
	        return hotEntity.HotLogicInstance as EntityLogicBase;
	    }
	
	    //隐藏逻辑实体
	    public static void HideEntity(this EntityComponent entityComponent, EntityLogicBase entity)
	    {
	        entityComponent.HideEntity(entity.RuntimeEntity.Entity);
	    }
	
        //显示实体
        public static void ShowEntity(this EntityComponent entityComponent, Type logicType, string groupName, int priority, EntityData data)
        {
            entityComponent.ShowHotEntity(logicType.Name, groupName, priority, data.Id, data.TypeId, data);
        }

        //附加实体
        public static void AttachEntity(this EntityComponent entityComponent, EntityLogicBase entity, int ownerId, string parentTransformPath = null, object userData = null)
	    {
	        entityComponent.AttachEntity(entity.RuntimeEntity.Entity, ownerId, parentTransformPath, userData);
	    }
	
	    //显示我的战机实体
	    public static void ShowMyAircraft(this EntityComponent entityComponent, MyAircraftData data)
	    {
            entityComponent.ShowEntity(typeof(MyAircraft), "Aircraft", Constant.AssetPriority.MyAircraftAsset, data);
        }

        //显示战机实体
        public static void ShowAircraft(this EntityComponent entityComponent, AircraftData data)
	    {
            entityComponent.ShowEntity(typeof(Aircraft), "Aircraft", Constant.AssetPriority.AircraftAsset, data);
        }

        //显示推进器实体
        public static void ShowThruster(this EntityComponent entityComponent, ThrusterData data)
	    {
            entityComponent.ShowEntity(typeof(Thruster), "Thruster", Constant.AssetPriority.ThrusterAsset, data);
        }

        //显示武器实体
        public static void ShowWeapon(this EntityComponent entityComponent, WeaponData data)
	    {
            entityComponent.ShowEntity(typeof(Weapon), "Weapon", Constant.AssetPriority.WeaponAsset, data);
        }

        //显示装甲实体
        public static void ShowArmor(this EntityComponent entityComponent, ArmorData data)
	    {
            entityComponent.ShowEntity(typeof(Armor), "Armor", Constant.AssetPriority.ArmorAsset, data);
        }

        //显示子弹实体
        public static void ShowBullet(this EntityComponent entityComponent, BulletData data)
	    {
            entityComponent.ShowEntity(typeof(Bullet), "Bullet", Constant.AssetPriority.BulletAsset, data);
        }

        //显示特效
        public static void ShowEffect(this EntityComponent entityComponent, EffectData data)
	    {
	        entityComponent.ShowEntity(typeof(Effect), "Effect", Constant.AssetPriority.EffectAsset, data);
	    }
	
	    //显示陨石实体
	    public static void ShowAsteroid(this EntityComponent entityComponent, AsteroidData data)
	    {
            entityComponent.ShowEntity(typeof(Asteroid), "Asteroid", Constant.AssetPriority.AsteroiAsset, data);
        }
    }
}
