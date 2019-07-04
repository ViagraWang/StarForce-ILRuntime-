﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityGameFrame.Runtime
{
    /// <summary>
    /// 实体逻辑基类
    /// </summary>
    public abstract class EntityLogic : MonoBehaviour
    {
        private bool m_Available = false;
        private bool m_Visible = false;
        private int m_OriginalLayer = 0;    //初始层级
        private Transform m_OriginalTransform = null;   //初始根对象
        private Entity m_Entity = null; //绑定的实体

        /// <summary>
        /// 获取实体
        /// </summary>
        public Entity Entity { get { return m_Entity; } }

        /// <summary>
        /// 获取或设置实体名称
        /// </summary>
        public string Name
        {
            get { return gameObject.name; }
            set { gameObject.name = value; }
        }

        /// <summary>
        /// 获取实体是否可用
        /// </summary>
        public bool IsAvailable { get { return m_Available; } }

        /// <summary>
        /// 获取或设置实体是否可见
        /// </summary>
        public bool Visible
        {
            get { return m_Available && m_Visible; }
            set
            {
                if (!m_Available)
                {
                    Log.Warning("Entity '{0}' is not available.", Name);
                    return;
                }
                if (m_Visible == value)
                    return;

                m_Visible = value;
                InternalSetVisible(value);

            }
        }

        /// <summary>
        /// 缓存的Transform
        /// </summary>
        public Transform CachedTransform { get; private set; }

        /// <summary>
        /// 实体初始化
        /// </summary>
        /// <param name="userData">传递的实体</param>
        /// <param name="userData">用户自定义数据</param>
        protected internal virtual void OnInit(Entity entity, object userData)
        {
            if (CachedTransform == null)
                CachedTransform = transform;
            m_Entity = entity;
            m_OriginalLayer = gameObject.layer;
            m_OriginalTransform = CachedTransform.parent;
            
        }

        /// <summary>
        /// 实体显示
        /// </summary>
        /// <param name="userData">用户自定义数据</param>
        protected internal virtual void OnShow(object userData)
        {
            m_Available = true;
            Visible = true;
        }

        /// <summary>
        /// 实体隐藏
        /// </summary>
        /// <param name="userData">用户自定义数据</param>
        protected internal virtual void OnHide(object userData)
        {
            gameObject.SetLayerRecursively(m_OriginalLayer);
            Visible = false;
            m_Available = false;
        }

        /// <summary>
        /// 实体附加子实体
        /// </summary>
        /// <param name="childEntity">附加的子实体</param>
        /// <param name="parentTransform">被附加的父实体</param>
        /// <param name="userData">用户自定义数据</param>
        protected internal virtual void OnAttached(EntityLogic childEntity, Transform parentTransform, object userData)
        {

        }

        /// <summary>
        /// 实体解除子实体
        /// </summary>
        /// <param name="childEntity">解除的子实体</param>
        /// <param name="userData">用户自定义数据</param>
        protected internal virtual void OnDetached(EntityLogic childEntity, object userData)
        {

        }

        /// <summary>
        /// 实体附加子实体
        /// </summary>
        /// <param name="parentEntity">被附加的父实体</param>
        /// <param name="parentTransform">被附加父实体的位置</param>
        /// <param name="userData">用户自定义数据</param>
        protected internal virtual void OnAttachTo(EntityLogic parentEntity, Transform parentTransform, object userData)
        {
            CachedTransform.SetParent(parentTransform);
        }

        /// <summary>
        /// 实体解除子实体
        /// </summary>
        /// <param name="childEntity">被解除的父实体</param>
        /// <param name="userData">用户自定义数据</param>
        protected internal virtual void OnDetachFrom(EntityLogic parentEntity, object userData)
        {
            CachedTransform.SetParent(m_OriginalTransform);
        }

        /// <summary>
        /// 实体轮询
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位</param>
        protected internal virtual void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {

        }

        /// <summary>
        /// 设置实体的可见性
        /// </summary>
        /// <param name="visible">实体的可见性</param>
        protected virtual void InternalSetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }

    }
}
