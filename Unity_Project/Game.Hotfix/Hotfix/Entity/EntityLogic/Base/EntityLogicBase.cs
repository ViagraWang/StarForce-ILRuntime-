using Game.Runtime;
using System;
using UnityEngine;

namespace Game.Hotfix
{
    /// <summary>
    /// 实体逻辑的基类
    /// </summary>
    public abstract class EntityLogicBase
    {
        public EntityData EntityData { get; private set; } //实体数据

        //实体逻辑
        public HotEntity RuntimeEntity { get; private set; }

        /// <summary>
        /// 实体初始化
        /// </summary>
        /// <param name="userData">用户自定义数据</param>
        public virtual void OnInit(object userData)
        {
            RuntimeEntity = userData as HotEntity;
        }

        /// <summary>
        /// 实体显示
        /// </summary>
        /// <param name="userData">用户自定义数据</param>
        public virtual void OnShow(object userData)
        {
            EntityData = userData as EntityData;
            if (EntityData == null)
            {
                HotLog.Error("Entity data is invalid -> m_EntityData == null");
                return;
            }
            RuntimeEntity.Position = EntityData.Position;
            RuntimeEntity.Rotation = EntityData.Rotation;
        }

        /// <summary>
        /// 实体隐藏
        /// </summary>
        /// <param name="userData">用户自定义数据</param>
        public abstract void OnHide(object userData);

        /// <summary>
        /// 实体附加子实体
        /// </summary>
        /// <param name="childEntity">附加的子实体</param>
        /// <param name="parentTransform">被附加的父实体</param>
        /// <param name="userData">用户自定义数据</param>
        public virtual void OnAttached(EntityLogicBase childEntity, Transform parentTransform, object userData)
        {

        }

        /// <summary>
        /// 实体解除子实体
        /// </summary>
        /// <param name="childEntity">解除的子实体</param>
        /// <param name="userData">用户自定义数据</param>
        public virtual void OnDetached(EntityLogicBase childEntity, object userData)
        {

        }

        /// <summary>
        /// 实体附加子实体
        /// </summary>
        /// <param name="parentEntity">被附加的父实体</param>
        /// <param name="parentTransform">被附加父实体的位置</param>
        /// <param name="userData">用户自定义数据</param>
        public virtual void OnAttachTo(EntityLogicBase parentEntity, Transform parentTransform, object userData)
        {

        }

        /// <summary>
        /// 实体解除子实体
        /// </summary>
        /// <param name="childEntity">被解除的父实体</param>
        /// <param name="userData">用户自定义数据</param>
        public virtual void OnDetachFrom(EntityLogicBase parentEntity, object userData)
        {

        }

        /// <summary>
        /// 实体轮询
        /// </summary>
        public virtual void OnUpdate()
        {

        }

        /// <summary>
        /// 设置实体的可见性
        /// </summary>
        /// <param name="visible">实体的可见性</param>
        public virtual void InternalSetVisible(bool visible)
        {

        }

        //触发器进入的回调
        public virtual void OnTriggerEnter(HotEntity collider) { }

        //触发器离开的回调
        public virtual void OnTriggerExit(HotEntity collider) { }

    }
}
