using System;
using System.Collections.Generic;

namespace UnityGameFrame.Runtime
{
    /// <summary>
    /// 默认声音辅助器
    /// </summary>
    public class DefaultSoundHelper : SoundHelperBase
    {
        private ResourceComponent m_ResourceComponent = null;

        private void Start()
        {
            m_ResourceComponent = GameEntry.GetComponent<ResourceComponent>();
            if (m_ResourceComponent == null)
            {
                Log.Fatal("[DefaultSoundHelper.Start] Resource component is invalid.");
                return;
            }
        }

        public override void ReleaseSoundAsset(object soundAsset)
        {
            m_ResourceComponent.UnloadAsset(soundAsset);
        }
    }
}
