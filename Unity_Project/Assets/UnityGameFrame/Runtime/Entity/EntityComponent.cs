using GameFramework;
using GameFramework.Entity;
using GameFramework.ObjectPool;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityGameFrame.Runtime
{
    /// <summary>
    /// 实体组件
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Game Framework/Entity")]
    public sealed partial class EntityComponent : GameFrameworkComponent
    {
        private IEntityManager m_EntityManager = null;  //实体管理器
        private EventComponent m_EventComponent = null; //事件组件

        private readonly List<IEntity> m_InternalEntityResultsCache = new List<IEntity>();  //实体缓存

        //开始回调事件的标志位
        [SerializeField]
        private bool m_EnableShowEntitySuccessEvent = true;
        [SerializeField]
        private bool m_EnableShowEntityFailureEvent = true;
        [SerializeField]
        private bool m_EnableShowEntityUpdateEvent = false;
        [SerializeField]
        private bool m_EnableShowEntityDependencyAssetEvent = false;
        [SerializeField]
        private bool m_EnableHideEntityCompleteEvent = true;

        [SerializeField]
        private Transform m_InstanceRoot = null;    //根对象

        [SerializeField]
        private string m_EntityHelperTypeName = "UnityGameFrame.Runtime.DefaultEntityHelper";   //实体辅助器类名
        [SerializeField]
        private EntityHelperBase m_CustomEntityHelper = null;   //自定义的实体辅助器


        [SerializeField]
        private string m_EntityGroupHelperTypeName = "UnityGameFrame.Runtime.DefaultEntityGroupHelper"; //实体组辅助器类名
        [SerializeField]
        private EntityGroupHelperBase m_CustomEntityGroupHelper = null; //自定义的实体辅助器

        [SerializeField]
        private EntityGroup[] m_EntityGroups = null;    //实体组

        /// <summary>
        /// 获取实体数量
        /// </summary>
        public int EntityCount { get { return m_EntityManager.EntityCount; } }

        /// <summary>
        /// 获取实体组数量
        /// </summary>
        public int EntityGroupCount { get { return m_EntityManager.EntityGroupCount; } }


        protected override void Awake()
        {
            base.Awake();

            m_EntityManager = GameFrameworkEntry.GetModule<IEntityManager>();
            if (m_EntityManager == null)
            {
                Log.Fatal("[EntityComponent.Awake] Entity manager is invalid.");
                return;
            }
            //注册事件
            m_EntityManager.ShowEntitySuccess += OnShowEntitySuccess;
            m_EntityManager.ShowEntityFailure += OnShowEntityFailure;
            m_EntityManager.ShowEntityUpdate += OnShowEntityUpdate;
            m_EntityManager.ShowEntityDependencyAsset += OnShowEntityDependencyAsset;
            m_EntityManager.HideEntityComplete += OnHideEntityComplete;
        }


        private void Start()
        {
            //基础组件
            BaseComponent baseComponent = GameEntry.GetComponent<BaseComponent>();
            if (baseComponent == null)
            {
                Log.Fatal("[EntityComponent.Start] Base component is invalid.");
                return;
            }
            //设置资源管理器
            m_EntityManager.SetResourceManager(baseComponent.ResourceManager);

            //事件组件
            m_EventComponent = GameEntry.GetComponent<EventComponent>();
            if (m_EventComponent == null)
            {
                Log.Fatal("[EntityComponent.Start] Event component is invalid.");
                return;
            }
            
            //设置对象池
            m_EntityManager.SetObjectPoolManager(GameFrameworkEntry.GetModule<IObjectPoolManager>());

            //实体辅助器
            EntityHelperBase entityHelper = Helper.CreateHelper(m_EntityHelperTypeName, m_CustomEntityHelper);
            if (entityHelper == null)
            {
                Log.Error("[EntityComponent.Start] Can not create entity helper.");
                return;
            }
            entityHelper.name = "Entity Helper";
            Transform trans = entityHelper.transform;
            trans.SetParent(this.transform);
            trans.localScale = Vector3.one;

            m_EntityManager.SetEntityHelper(entityHelper);  //设置实体辅助器

            if (m_InstanceRoot == null)
            {
                m_InstanceRoot = (new GameObject("Entity Instances")).transform;
                m_InstanceRoot.SetParent(trans);
                m_InstanceRoot.localScale = Vector3.one;
            }

            for (int i = 0; i < m_EntityGroups.Length; i++)
            {
                if (!AddEntityGroup(m_EntityGroups[i].Name, m_EntityGroups[i].InstanceAutoReleaseInterval, m_EntityGroups[i].InstanceCapacity, m_EntityGroups[i].InstanceExpireTime, m_EntityGroups[i].InstancePriority))
                {
                    Log.Warning("[EntityComponent.Start] Add entity group '{0}' failure.", m_EntityGroups[i].Name);
                    continue;
                }
            }
        }

        /// <summary>
        /// 是否存在实体组
        /// </summary>
        /// <param name="entityGroupName">实体组名称</param>
        /// <returns>是否存在实体组</returns>
        public bool HasEntityGroup(string entityGroupName)
        {
            return m_EntityManager.HasEntityGroup(entityGroupName);
        }

        /// <summary>
        /// 获取实体组
        /// </summary>
        /// <param name="entityGroupName">实体组名称</param>
        /// <returns>要获取的实体组</returns>
        public IEntityGroup GetEntityGroup(string entityGroupName)
        {
            return m_EntityManager.GetEntityGroup(entityGroupName);
        }

        /// <summary>
        /// 获取所有实体组
        /// </summary>
        /// <returns>所有实体组</returns>
        public IEntityGroup[] GetAllEntityGroups()
        {
            return m_EntityManager.GetAllEntityGroups();
        }

        /// <summary>
        /// 获取所有实体组
        /// </summary>
        /// <param name="results">所有实体组</param>
        public void GetAllEntityGroups(List<IEntityGroup> results)
        {
            m_EntityManager.GetAllEntityGroups(results);
        }

        /// <summary>
        /// 增加实体组
        /// </summary>
        /// <param name="entityGroupName">实体组名称</param>
        /// <param name="instanceAutoReleaseInterval">实体实例对象池自动释放可释放对象的间隔秒数</param>
        /// <param name="instanceCapacity">实体实例对象池容量</param>
        /// <param name="instanceExpireTime">实体实例对象池对象过期秒数</param>
        /// <param name="instancePriority">实体实例对象池的优先级</param>
        /// <returns>是否增加实体组成功</returns>
        public bool AddEntityGroup(string entityGroupName, float instanceAutoReleaseInterval, int instanceCapacity, float instanceExpireTime, int instancePriority)
        {
            if (m_EntityManager.HasEntityGroup(entityGroupName))    //先检查是否存在实体组
                return false;

            //创建实体组辅助器
            EntityGroupHelperBase entityGroupHelper = Helper.CreateHelper(m_EntityGroupHelperTypeName, m_CustomEntityGroupHelper, EntityGroupCount);
            if (entityGroupHelper == null)
            {
                Log.Error("[EntityComponent.AddEntityGroup] Can not create entity group helper.");
                return false;
            }
            entityGroupHelper.name = Utility.Text.Format("Entity Group - {0}", entityGroupName);
            Transform trans = entityGroupHelper.transform;
            trans.SetParent(m_InstanceRoot);
            trans.localScale = Vector3.one;

            return m_EntityManager.AddEntityGroup(entityGroupName, instanceAutoReleaseInterval, instanceCapacity, instanceExpireTime, instancePriority, entityGroupHelper);
        }

        /// <summary>
        /// 是否存在实体
        /// </summary>
        /// <param name="entityId">实体编号</param>
        /// <returns>是否存在实体</returns>
        public bool HasEntity(int entityId)
        {
            return m_EntityManager.HasEntity(entityId);
        }

        /// <summary>
        /// 是否存在实体
        /// </summary>
        /// <param name="entityAssetName">实体资源名称</param>
        /// <returns>是否存在实体</returns>
        public bool HasEntity(string entityAssetName)
        {
            return m_EntityManager.HasEntity(entityAssetName);
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="entityId">实体编号</param>
        /// <returns>实体</returns>
        public Entity GetEntity(int entityId)
        {
            return (Entity)m_EntityManager.GetEntity(entityId);
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="entityAssetName">实体资源名称</param>
        /// <returns>要获取的实体</returns>
        public Entity GetEntity(string entityAssetName)
        {
            return (Entity)m_EntityManager.GetEntity(entityAssetName);
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="entityAssetName">实体资源名称</param>
        /// <returns>要获取的实体</returns>
        public Entity[] GetEntities(string entityAssetName)
        {
            IEntity[] entities = m_EntityManager.GetEntities(entityAssetName);
            Entity[] results = new Entity[entities.Length];
            for (int i = 0; i < results.Length; i++)
            {
                results[i] = entities[i] as Entity;
            }

            return results;
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="entityAssetName">实体资源名称</param>
        /// <param name="results">要获取的实体</param>
        public void GetEntities(string entityAssetName, List<Entity> results)
        {
            if (results == null)
            {
                Log.Error("[EntityComponent.GetEntities] Results is invalid.");
                return;
            }

            results.Clear();
            m_EntityManager.GetEntities(entityAssetName, m_InternalEntityResultsCache);
            for (int i = 0; i < m_InternalEntityResultsCache.Count; i++)
            {
                results.Add(m_InternalEntityResultsCache[i] as Entity);
            }
        }

        /// <summary>
        /// 获取所有已加载的实体
        /// </summary>
        /// <returns>所有已加载的实体</returns>
        public Entity[] GetAllLoadedEntities()
        {
            IEntity[] entities = m_EntityManager.GetAllLoadedEntities();
            Entity[] results = new Entity[entities.Length];
            for (int i = 0; i < entities.Length; i++)
            {
                results[i] = entities[i] as Entity;
            }

            return results;
        }

        /// <summary>
        /// 获取所有已加载的实体
        /// </summary>
        /// <param name="results">所有已加载的实体</param>
        public void GetAllLoadedEntities(List<Entity> results)
        {
            if (results == null)
            {
                Log.Error("[EntityComponent.GetAllLoadedEntities]Results is invalid.");
                return;
            }

            results.Clear();
            m_EntityManager.GetAllLoadedEntities(m_InternalEntityResultsCache);
            for (int i = 0; i < m_InternalEntityResultsCache.Count; i++)
            {
                results.Add(m_InternalEntityResultsCache[i] as Entity);
            }
        }

        /// <summary>
        /// 获取所有正在加载实体的编号
        /// </summary>
        /// <returns>所有正在加载实体的编号</returns>
        public int[] GetAllLoadingEntityIds()
        {
            return m_EntityManager.GetAllLoadingEntityIds();
        }

        /// <summary>
        /// 获取所有正在加载实体的编号
        /// </summary>
        /// <param name="results">所有正在加载实体的编号</param>
        public void GetAllLoadingEntityIds(List<int> results)
        {
            m_EntityManager.GetAllLoadingEntityIds(results);
        }

        /// <summary>
        /// 是否正在加载实体
        /// </summary>
        /// <param name="entityId">实体编号</param>
        /// <returns>是否正在加载实体</returns>
        public bool IsLoadingEntity(int entityId)
        {
            return m_EntityManager.IsLoadingEntity(entityId);
        }

        /// <summary>
        /// 是否是合法的实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>实体是否合法</returns>
        public bool IsValidEntity(Entity entity)
        {
            return m_EntityManager.IsValidEntity(entity);
        }

        /// <summary>
        /// 显示实体
        /// </summary>
        /// <typeparam name="T">实体逻辑类型</typeparam>
        /// <param name="entityId">实体编号</param>
        /// <param name="entityAssetName">实体资源名称</param>
        /// <param name="entityGroupName">实体组名称</param>
        /// <param name="userData">用户自定义数据</param>
        public void ShowEntity<T>(int entityId, string entityAssetName, string entityGroupName, object userData = null) where T : EntityLogic
        {
            ShowEntity(entityId, typeof(T), entityAssetName, entityGroupName, 0, userData);
        }

        /// <summary>
        /// 显示实体
        /// </summary>
        /// <param name="entityId">实体编号</param>
        /// <param name="entityLogicType">实体逻辑类型</param>
        /// <param name="entityAssetName">实体资源名称</param>
        /// <param name="entityGroupName">实体组名称</param>
        /// <param name="userData">用户自定义数据</param>
        public void ShowEntity(int entityId, Type entityLogicType, string entityAssetName, string entityGroupName, object userData = null)
        {
            ShowEntity(entityId, entityLogicType, entityAssetName, entityGroupName, 0, userData);
        }

        /// <summary>
        /// 显示实体
        /// </summary>
        /// <typeparam name="T">实体逻辑类型</typeparam>
        /// <param name="entityId">实体编号</param>
        /// <param name="entityAssetName">实体资源名称</param>
        /// <param name="entityGroupName">实体组名称</param>
        /// <param name="priority">加载实体资源的优先级</param>
        /// <param name="userData">用户自定义数据</param>
        public void ShowEntity<T>(int entityId, string entityAssetName, string entityGroupName, int priority, object userData = null) where T : EntityLogic
        {
            ShowEntity(entityId, typeof(T), entityAssetName, entityGroupName, priority, userData);
        }

        /// <summary>
        /// 显示实体
        /// </summary>
        /// <param name="entityId">实体编号</param>
        /// <param name="entityLogicType">实体逻辑类型</param>
        /// <param name="entityAssetName">实体资源名称</param>
        /// <param name="entityGroupName">实体组名称</param>
        /// <param name="priority">加载实体资源的优先级</param>
        /// <param name="userData">用户自定义数据</param>
        public void ShowEntity(int entityId, Type entityLogicType, string entityAssetName, string entityGroupName, int priority, object userData = null)
        {
            if (entityLogicType == null)
            {
                Log.Error("[EntityComponent.ShowEntity] Entity type is invalid.");
                return;
            }
            m_EntityManager.ShowEntity(entityId, entityAssetName, entityGroupName, priority, ReferencePool.Acquire<ShowEntityInfo>().Fill(entityLogicType, userData));
        }

        /// <summary>
        /// 隐藏实体
        /// </summary>
        /// <param name="entityId">实体编号</param>
        /// <param name="userData">用户自定义数据</param>
        public void HideEntity(int entityId, object userData = null)
        {
            m_EntityManager.HideEntity(entityId, userData);
        }

        /// <summary>
        /// 隐藏实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="userData">用户自定义数据</param>
        public void HideEntity(Entity entity, object userData = null)
        {
            m_EntityManager.HideEntity(entity, userData);
        }

        /// <summary>
        /// 隐藏所有已加载的实体
        /// </summary>
        /// <param name="userData">用户自定义数据</param>
        public void HideAllLoadedEntities(object userData = null)
        {
            m_EntityManager.HideAllLoadedEntities(userData);
        }

        /// <summary>
        /// 隐藏所有正在加载的实体
        /// </summary>
        public void HideAllLoadingEntities()
        {
            m_EntityManager.HideAllLoadingEntities();
        }

        /// <summary>
        /// 获取父实体
        /// </summary>
        /// <param name="childEntityId">要获取父实体的子实体的实体编号</param>
        /// <returns>子实体的父实体</returns>
        public Entity GetParentEntity(int childEntityId)
        {
            return (Entity)m_EntityManager.GetParentEntity(childEntityId);
        }

        /// <summary>
        /// 获取父实体
        /// </summary>
        /// <param name="childEntity">要获取父实体的子实体</param>
        /// <returns>子实体的父实体</returns>
        public Entity GetParentEntity(Entity childEntity)
        {
            return (Entity)m_EntityManager.GetParentEntity(childEntity);
        }

        /// <summary>
        /// 获取子实体
        /// </summary>
        /// <param name="parentEntityId">要获取子实体的父实体的实体编号</param>
        /// <returns>子实体数组</returns>
        public Entity[] GetChildEntities(int parentEntityId)
        {
            IEntity[] entities = m_EntityManager.GetChildEntities(parentEntityId);
            Entity[] results = new Entity[entities.Length];
            for (int i = 0; i < entities.Length; i++)
            {
                results[i] = entities[i] as Entity;
            }

            return results;
        }

        /// <summary>
        /// 获取子实体
        /// </summary>
        /// <param name="parentEntityId">要获取子实体的父实体的实体编号</param>
        /// <param name="results">子实体数组</param>
        public void GetChildEntities(int parentEntityId, List<IEntity> results)
        {
            if (results == null)
            {
                Log.Error("[EntityComponent.GetChildEntities] Results is invalid.");
                return;
            }

            results.Clear();
            m_EntityManager.GetChildEntities(parentEntityId, m_InternalEntityResultsCache);
            for (int i = 0; i < m_InternalEntityResultsCache.Count; i++)
            {
                results.Add(m_InternalEntityResultsCache[i] as Entity);
            }
        }

        /// <summary>
        /// 获取子实体
        /// </summary>
        /// <param name="parentEntity">要获取子实体的父实体</param>
        /// <returns>子实体数组</returns>
        public Entity[] GetChildEntities(Entity parentEntity)
        {
            IEntity[] entities = m_EntityManager.GetChildEntities(parentEntity);
            Entity[] results = new Entity[entities.Length];
            for (int i = 0; i < entities.Length; i++)
            {
                results[i] = entities[i] as Entity;
            }

            return results;
        }

        /// <summary>
        /// 获取子实体
        /// </summary>
        /// <param name="parentEntity">要获取子实体的父实体</param>
        /// <param name="results">子实体数组</param>
        public void GetChildEntities(IEntity parentEntity, List<IEntity> results)
        {
            if (results == null)
            {
                Log.Error("[EntityComponent.GetChildEntities] Results is invalid.");
                return;
            }

            results.Clear();
            m_EntityManager.GetChildEntities(parentEntity, m_InternalEntityResultsCache);
            for (int i = 0; i < m_InternalEntityResultsCache.Count; i++)
            {
                results.Add(m_InternalEntityResultsCache[i] as Entity);
            }
        }

        /// <summary>
        /// 附加子实体
        /// </summary>
        /// <param name="childEntityId">要附加的子实体的实体编号</param>
        /// <param name="parentEntityId">被附加的父实体的实体编号</param>
        /// <param name="userData">用户自定义数据</param>
        public void AttachEntity(int childEntityId, int parentEntityId, object userData = null)
        {
            AttachEntity(GetEntity(childEntityId), GetEntity(parentEntityId), string.Empty, userData);
        }

        /// <summary>
        /// 附加子实体
        /// </summary>
        /// <param name="childEntityId">要附加的子实体的实体编号</param>
        /// <param name="parentEntity">被附加的父实体</param>
        /// <param name="userData">用户自定义数据</param>
        public void AttachEntity(int childEntityId, Entity parentEntity, object userData = null)
        {
            AttachEntity(GetEntity(childEntityId), parentEntity, string.Empty, userData);
        }

        /// <summary>
        /// 附加子实体
        /// </summary>
        /// <param name="childEntity">要附加的子实体</param>
        /// <param name="parentEntityId">被附加的父实体的实体编号</param>
        /// <param name="userData">用户自定义数据</param>
        public void AttachEntity(Entity childEntity, int parentEntityId, object userData = null)
        {
            AttachEntity(childEntity, GetEntity(parentEntityId), string.Empty, userData);
        }

        /// <summary>
        /// 附加子实体
        /// </summary>
        /// <param name="childEntity">要附加的子实体</param>
        /// <param name="parentEntity">被附加的父实体</param>
        /// <param name="userData">用户自定义数据</param>
        public void AttachEntity(Entity childEntity, Entity parentEntity, object userData = null)
        {
            AttachEntity(childEntity, parentEntity, string.Empty, userData);
        }

        /// <summary>
        /// 附加子实体
        /// </summary>
        /// <param name="childEntityId">要附加的子实体的实体编号</param>
        /// <param name="parentEntityId">被附加的父实体的实体编号</param>
        /// <param name="parentTransformPath">相对于被附加父实体的位置</param>
        /// <param name="userData">用户自定义数据</param>
        public void AttachEntity(int childEntityId, int parentEntityId, string parentTransformPath, object userData = null)
        {
            AttachEntity(GetEntity(childEntityId), GetEntity(parentEntityId), parentTransformPath, userData);
        }

        /// <summary>
        /// 附加子实体
        /// </summary>
        /// <param name="childEntityId">要附加的子实体的实体编号</param>
        /// <param name="parentEntity">被附加的父实体</param>
        /// <param name="parentTransformPath">相对于被附加父实体的位置</param>
        /// <param name="userData">用户自定义数据</param>
        public void AttachEntity(int childEntityId, Entity parentEntity, string parentTransformPath, object userData = null)
        {
            AttachEntity(GetEntity(childEntityId), parentEntity, parentTransformPath, userData);
        }

        /// <summary>
        /// 附加子实体
        /// </summary>
        /// <param name="childEntity">要附加的子实体</param>
        /// <param name="parentEntityId">被附加的父实体的实体编号</param>
        /// <param name="parentTransformPath">相对于被附加父实体的位置</param>
        /// <param name="userData">用户自定义数据</param>
        public void AttachEntity(Entity childEntity, int parentEntityId, string parentTransformPath, object userData = null)
        {
            AttachEntity(childEntity, GetEntity(parentEntityId), parentTransformPath, userData);
        }


        /// <summary>
        /// 附加子实体
        /// </summary>
        /// <param name="childEntityId">要附加的子实体的实体编号</param>
        /// <param name="parentEntityId">被附加的父实体的实体编号</param>
        /// <param name="parentTransform">相对于被附加父实体的位置</param>
        /// <param name="userData">用户自定义数据</param>
        public void AttachEntity(int childEntityId, int parentEntityId, Transform parentTransform, object userData = null)
        {
            AttachEntity(GetEntity(childEntityId), GetEntity(parentEntityId), parentTransform, userData);
        }

        /// <summary>
        /// 附加子实体
        /// </summary>
        /// <param name="childEntityId">要附加的子实体的实体编号</param>
        /// <param name="parentEntity">被附加的父实体</param>
        /// <param name="parentTransform">相对于被附加父实体的位置</param>
        /// <param name="userData">用户自定义数据</param>
        public void AttachEntity(int childEntityId, Entity parentEntity, Transform parentTransform, object userData = null)
        {
            AttachEntity(GetEntity(childEntityId), parentEntity, parentTransform, userData);
        }

        /// <summary>
        /// 附加子实体
        /// </summary>
        /// <param name="childEntity">要附加的子实体</param>
        /// <param name="parentEntityId">被附加的父实体的实体编号</param>
        /// <param name="parentTransform">相对于被附加父实体的位置</param>
        /// <param name="userData">用户自定义数据</param>
        public void AttachEntity(Entity childEntity, int parentEntityId, Transform parentTransform, object userData = null)
        {
            AttachEntity(childEntity, GetEntity(parentEntityId), parentTransform, userData);
        }

        /// <summary>
        /// 附加子实体
        /// </summary>
        /// <param name="childEntity">要附加的子实体</param>
        /// <param name="parentEntity">被附加的父实体</param>
        /// <param name="parentTransformPath">相对于被附加父实体的位置</param>
        /// <param name="userData">用户自定义数据</param>
        public void AttachEntity(Entity childEntity, Entity parentEntity, string parentTransformPath, object userData = null)
        {
            if (childEntity == null)
            {
                Log.Warning("[EntityComponent.AttachEntity] Child entity is invalid.");
                return;
            }

            if (parentEntity == null)
            {
                Log.Warning("[EntityComponent.AttachEntity] Parent entity is invalid.");
                return;
            }

            Transform parentTransform = null;
            if (string.IsNullOrEmpty(parentTransformPath))  //父对象路径名为空，则直接用父对象
            {
                parentTransform = parentEntity.Logic.CachedTransform;
            }
            else
            {
                parentTransform = parentEntity.Logic.CachedTransform.Find(parentTransformPath);
                if (parentTransform == null)
                {
                    Log.Warning("[EntityComponent.AttachEntity] Can not find transform path '{0}' from parent entity '{1}'.", parentTransformPath, parentEntity.Logic.Name);
                    parentTransform = parentEntity.Logic.CachedTransform;
                }
            }

            AttachEntity(childEntity, parentEntity, parentTransform, userData);
        }

        /// <summary>
        /// 附加子实体
        /// </summary>
        /// <param name="childEntity">要附加的子实体</param>
        /// <param name="parentEntity">被附加的父实体</param>
        /// <param name="parentTransform">相对于被附加父实体的位置</param>
        /// <param name="userData">用户自定义数据</param>
        public void AttachEntity(Entity childEntity, Entity parentEntity, Transform parentTransform, object userData = null)
        {
            if (childEntity == null)
            {
                Log.Warning("[EntityComponent.AttachEntity] Child entity is invalid.");
                return;
            }

            if (parentEntity == null)
            {
                Log.Warning("[EntityComponent.AttachEntity] Parent entity is invalid.");
                return;
            }

            if (parentTransform == null)
            {
                parentTransform = parentEntity.Logic.CachedTransform;
            }

            m_EntityManager.AttachEntity(childEntity, parentEntity, new AttachEntityInfo(parentTransform, userData));
        }

        /// <summary>
        /// 解除子实体
        /// </summary>
        /// <param name="childEntityId">要解除的子实体的实体编号</param>
        /// <param name="userData">用户自定义数据</param>
        public void DetachEntity(int childEntityId, object userData = null)
        {
            m_EntityManager.DetachEntity(childEntityId, userData);
        }

        /// <summary>
        /// 解除子实体
        /// </summary>
        /// <param name="childEntity">要解除的子实体</param>
        /// <param name="userData">用户自定义数据</param>
        public void DetachEntity(Entity childEntity, object userData = null)
        {
            m_EntityManager.DetachEntity(childEntity, userData);
        }

        /// <summary>
        /// 解除所有子实体
        /// </summary>
        /// <param name="parentEntityId">被解除的父实体的实体编号</param>
        /// <param name="userData">用户自定义数据</param>
        public void DetachChildEntities(int parentEntityId, object userData = null)
        {
            m_EntityManager.DetachChildEntities(parentEntityId, userData);
        }

        /// <summary>
        /// 解除所有子实体
        /// </summary>
        /// <param name="parentEntity">被解除的父实体</param>
        /// <param name="userData">用户自定义数据</param>
        public void DetachChildEntities(Entity parentEntity, object userData = null)
        {
            m_EntityManager.DetachChildEntities(parentEntity, userData);
        }

        /// <summary>
        /// 设置实体是否被加锁
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="locked">实体是否被加锁</param>
        public void SetEntityInstanceLocked(Entity entity, bool locked)
        {
            if (entity == null)
            {
                Log.Warning("[EntityComponent.SetEntityInstanceLocked] Entity is invalid.");
                return;
            }

            IEntityGroup entityGroup = entity.EntityGroup;
            if (entityGroup == null)
            {
                Log.Warning("[EntityComponent.SetEntityInstanceLocked] Entity group is invalid.");
                return;
            }

            entityGroup.SetEntityInstanceLocked(entity.gameObject, locked);
        }

        /// <summary>
        /// 设置实体的优先级
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="priority">实体优先级</param>
        public void SetInstancePriority(Entity entity, int priority)
        {
            if (entity == null)
            {
                Log.Warning("[EntityComponent.SetInstancePriority] Entity is invalid.");
                return;
            }

            IEntityGroup entityGroup = entity.EntityGroup;
            if (entityGroup == null)
            {
                Log.Warning("[EntityComponent.SetInstancePriority] Entity group is invalid.");
                return;
            }

            entityGroup.SetEntityInstancePriority(entity.gameObject, priority);
        }

        //显示实体成功的回调
        private void OnShowEntitySuccess(object sender, GameFramework.Entity.ShowEntitySuccessEventArgs e)
        {
            if (m_EnableShowEntitySuccessEvent)
                m_EventComponent.Fire(this, ReferencePool.Acquire<ShowEntitySuccessEventArgs>().Fill(e));
        }

        //显示实体失败的回调
        private void OnShowEntityFailure(object sender, GameFramework.Entity.ShowEntityFailureEventArgs e)
        {
            Log.Error("[EntityComponent.OnShowEntityFailure] Show entity failure, entity id '{0}', asset name '{1}', entity group name '{2}', error message '{3}'.", e.EntityId.ToString(), e.EntityAssetName, e.EntityGroupName, e.ErrorMessage);
            if (m_EnableShowEntityFailureEvent)
                m_EventComponent.Fire(this, ReferencePool.Acquire<ShowEntityFailureEventArgs>().Fill(e));
        }

        //显示实体更新的回调
        private void OnShowEntityUpdate(object sender, GameFramework.Entity.ShowEntityUpdateEventArgs e)
        {
            if (m_EnableShowEntityUpdateEvent)
                m_EventComponent.Fire(this, ReferencePool.Acquire<ShowEntityUpdateEventArgs>().Fill(e));
        }

        //显示实体时加载依赖资源的回调
        private void OnShowEntityDependencyAsset(object sender, GameFramework.Entity.ShowEntityDependencyAssetEventArgs e)
        {
            if (m_EnableShowEntityDependencyAssetEvent)
                m_EventComponent.Fire(this, ReferencePool.Acquire<ShowEntityDependencyAssetEventArgs>().Fill(e));
        }

        //隐藏实体完成的回调
        private void OnHideEntityComplete(object sender, GameFramework.Entity.HideEntityCompleteEventArgs e)
        {
            if (m_EnableHideEntityCompleteEvent)
                m_EventComponent.Fire(this, ReferencePool.Acquire<HideEntityCompleteEventArgs>().Fill(e));
        }
    }
}
