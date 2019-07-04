using System;
namespace UnityGameFrame.Runtime
{
    internal sealed class LoadConfigInfo
    {
        private readonly string m_ConfigName;   //配置名称
        private readonly object m_UserData; //用户自定义数据

        /// <summary>
        /// 配置名称
        /// </summary>
        public string ConfigName { get { return m_ConfigName; } }

        /// <summary>
        /// 用户自定义数据
        /// </summary>
        public object UserData { get { return m_UserData; } }

        public LoadConfigInfo(string configName, object userData)
        {
            m_ConfigName = configName;
            m_UserData = userData;
        }

    }
}
