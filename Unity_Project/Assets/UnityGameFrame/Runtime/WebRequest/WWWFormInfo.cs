using UnityEngine;

namespace UnityGameFrame.Runtime
{
    internal sealed class WWWFormInfo
    {
        private readonly WWWForm m_WWWForm; //表单信息
        private readonly object m_UserData; //用户自定义数据

        public WWWForm WWWForm { get { return m_WWWForm; } }

        public object UserData { get { return m_UserData; } }

        public WWWFormInfo(WWWForm wwwForm, object userData)
        {
            m_WWWForm = wwwForm;
            m_UserData = userData;
        }
    }
}
