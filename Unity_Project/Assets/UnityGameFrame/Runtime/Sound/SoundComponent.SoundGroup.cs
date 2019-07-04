using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityGameFrame.Runtime
{
    public sealed partial class SoundComponent
    {
        /// <summary>
        /// 声音组，主要用在配置信息的加载
        /// </summary>
        [Serializable]
        private sealed class SoundGroup
        {
            [SerializeField]
            private string m_Name = null;   //声音组名
            [SerializeField]
            private bool m_IsAvoidBeingReplacedBySamePriority = false;    //能否被同优先级的声音替代
            [SerializeField]
            private bool m_IsMute = false;    //是否静音的标志位
            [SerializeField, Range(0f, 1f)]
            private float m_Volume = 1f;    //音量
            [SerializeField]
            private int m_AgentHelperCount = 1; //代理辅助器数量

            public string Name { get { return m_Name; } }

            public bool AvoidBeingReplacedBySamePriority { get { return m_IsAvoidBeingReplacedBySamePriority; } }

            public bool Mute { get { return m_IsMute; } }

            public float Volume { get { return m_Volume; } }

            public int AgentHelperCount { get { return m_AgentHelperCount; } }
        }
    }

}
