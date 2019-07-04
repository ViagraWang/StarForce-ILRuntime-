using GameFramework.Sound;
using UnityEngine;
using UnityEngine.Audio;

namespace UnityGameFrame.Runtime
{
    /// <summary>
    /// 声音组辅助器基类
    /// </summary>
    public abstract class SoundGroupHelperBase : MonoBehaviour, ISoundGroupHelper
    {
        [SerializeField]
        private AudioMixerGroup m_AudioMixerGroup = null;   //混音器组

        /// <summary>
        /// 获取或设置声音组辅助器所在的混音组
        /// </summary>
        public AudioMixerGroup AudioMixerGroup
        {
            get
            {
                return m_AudioMixerGroup;
            }
            set
            {
                m_AudioMixerGroup = value;
            }
        }
    }
}
