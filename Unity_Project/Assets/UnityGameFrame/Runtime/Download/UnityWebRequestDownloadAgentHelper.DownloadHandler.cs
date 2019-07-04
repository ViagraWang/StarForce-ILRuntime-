#if UNITY_5_4_OR_NEWER
using GameFramework.Download;
using UnityEngine.Networking;
#else
using UnityEngine.Experimental.Networking;
#endif

namespace UnityGameFrame.Runtime
{
    public partial class UnityWebRequestDownloadAgentHelper
    {

        private sealed class DownloadHandler : DownloadHandlerScript
        {
            private readonly UnityWebRequestDownloadAgentHelper m_Owner;

            public DownloadHandler(UnityWebRequestDownloadAgentHelper owner) : base(owner.m_DownloadCache)
            {
                m_Owner = owner;
            }

            //接收到网络数据
            protected override bool ReceiveData(byte[] data, int dataLength)
            {
                if(m_Owner != null && dataLength > 0)
                {
                    m_Owner.m_DownloadAgentHelperUpdateBytesEventHandler.Invoke(this, new DownloadAgentHelperUpdateBytesEventArgs(data, 0, dataLength));
                    m_Owner.m_DownloadAgentHelperUpdateLengthEventHandler.Invoke(this, new DownloadAgentHelperUpdateLengthEventArgs(dataLength));
                }
                return base.ReceiveData(data, dataLength);
            }

        }
    }
}
