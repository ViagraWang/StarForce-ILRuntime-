using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityGameFrame.Runtime
{
    /// <summary>
    /// 播放声音的信息
    /// </summary>
    internal sealed class PlaySoundInfo
    {
        private readonly Entity m_BindingEntity;    //绑定的实体
        private readonly Vector3 m_WorldPosition;   //世界位置坐标
        private readonly object m_UserData; //用户自定义数据

        public Entity BindingEntity { get { return m_BindingEntity; } }

        public Vector3 WorldPosition { get { return m_WorldPosition; } }

        public object UserData { get { return m_UserData; } }

        public PlaySoundInfo(Entity bindingEntity, Vector3 worldPosition, object userData)
        {
            m_BindingEntity = bindingEntity;
            m_WorldPosition = worldPosition;
            m_UserData = userData;
        }

    }
}
