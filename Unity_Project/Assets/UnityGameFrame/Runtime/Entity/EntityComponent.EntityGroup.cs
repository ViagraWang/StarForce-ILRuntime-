using System;
using UnityEngine;

namespace UnityGameFrame.Runtime
{
    public sealed partial class EntityComponent
    {
        /// <summary>
        /// 实体组，主要用在初始配置
        /// </summary>
        [Serializable]
        private sealed class EntityGroup
        {
            [SerializeField]
            private string m_Name = null;   //实体组名
            [SerializeField]
            private float m_InstanceAutoReleaseInterval = 60f;  //实体组实例对象池自动释放可释放对象的间隔秒数
            [SerializeField]
            private int m_InstanceCapacity = 16;  //实体组实例对象池的容量
            [SerializeField]
            private float m_InstanceExpireTime = 60f;  //实体组实例对象池对象过期秒数
            [SerializeField]
            private int m_InstancePriority = 0;  //实体组实例对象池的优先级

            public string Name { get { return m_Name; } }

            public float InstanceAutoReleaseInterval { get { return m_InstanceAutoReleaseInterval; } }

            public int InstanceCapacity { get { return m_InstanceCapacity; } }

            public float InstanceExpireTime { get { return m_InstanceExpireTime; } }

            public int InstancePriority { get { return m_InstancePriority; } }
        }
    }
}
