using GameFramework.Entity;
using UnityEngine;

namespace UnityGameFrame.Runtime
{
    /// <summary>
    /// 默认实体辅助器
    /// </summary>
    public class DefaultEntityHelper : EntityHelperBase
    {
        private ResourceComponent m_ResourceComponent = null;   //资源组件

        private void Start()
        {
            m_ResourceComponent = GameEntry.GetComponent<ResourceComponent>();
            if (m_ResourceComponent == null)
            {
                Log.Fatal("[DefaultEntityHelper.Start] Resource component is invalid.");
                return;
            }
        }

        /// <summary>
        /// 创建实体
        /// </summary>
        /// <param name="entityInstance">实体实例</param>
        /// <param name="entityGroup">实体所属的实体组</param>
        /// <param name="userData">用户自定义数据</param>
        /// <returns>实体</returns>
        public override IEntity CreateEntity(object entityInstance, IEntityGroup entityGroup, object userData)
        {
            GameObject gameObject = entityInstance as GameObject;
            if (gameObject == null)
            {
                Log.Error("[DefaultEntityHelper.CreateEntity] Entity instance is invalid.");
                return null;
            }

            gameObject.transform.SetParent((entityGroup.Helper as MonoBehaviour).transform);
            return gameObject.GetOrAddComponent<Entity>();
        }

        /// <summary>
        /// 实例化实体
        /// </summary>
        /// <param name="entityAsset">要实例化的实体资源</param>
        /// <returns>实例化后的实体</returns>
        public override object InstantiateEntity(object entityAsset)
        {
            return Instantiate(entityAsset as Object);
        }

        /// <summary>
        /// 释放实体
        /// </summary>
        /// <param name="entityAsset">要释放的实体资源</param>
        /// <param name="entityInstance">要释放的实体实例</param>
        public override void ReleaseEntity(object entityAsset, object entityInstance)
        {
            m_ResourceComponent.UnloadAsset(entityAsset);   //卸载
            Destroy((Object)entityInstance);    //删除实体实例
        }
    }
}
