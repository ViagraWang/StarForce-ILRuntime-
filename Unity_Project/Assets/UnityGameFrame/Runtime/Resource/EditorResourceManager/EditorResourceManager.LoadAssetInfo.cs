using GameFramework.Resource;
using System;

namespace UnityGameFrame.Runtime
{
    public partial class EditorResourceManager
    {
        //加载资源信息
        private sealed class LoadAssetInfo
        {
            private readonly string m_AssetName;    //资源名
            private readonly Type m_AssetType;  //资源类型
            private readonly int m_Priority;    //优先级
            private readonly DateTime m_StartTime;  //开始时间
            private readonly float m_DelaySeconds;  //延迟秒数
            private readonly LoadAssetCallbacks m_LoadAssetCallbacks;   //加载资源回调函数集
            private readonly object m_UserData; //用户自定义数据

            public string AssetName { get { return m_AssetName; } }

            public Type AssetType { get { return m_AssetType; } }

            public int Priority { get { return m_Priority; } }

            public DateTime StartTime { get { return m_StartTime; } }

            public float DelaySeconds { get { return m_DelaySeconds; } }

            public LoadAssetCallbacks LoadAssetCallbacks { get { return m_LoadAssetCallbacks; } }

            public object UserData { get { return m_UserData; } }

            public LoadAssetInfo(string assetName, Type assetType, int priority, DateTime startTime, float delaySeconds, LoadAssetCallbacks loadAssetCallbacks, object userData)
            {
                m_AssetName = assetName;
                m_AssetType = assetType;
                m_Priority = priority;
                m_StartTime = startTime;
                m_DelaySeconds = delaySeconds;
                m_LoadAssetCallbacks = loadAssetCallbacks;
                m_UserData = userData;
            }

        }
    }
}
