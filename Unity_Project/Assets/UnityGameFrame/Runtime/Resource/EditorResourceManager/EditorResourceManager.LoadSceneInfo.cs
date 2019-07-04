using GameFramework.Resource;
using System;
using UnityEngine;

namespace UnityGameFrame.Runtime
{
    public partial class EditorResourceManager
    {
        //加载资源信息
        private sealed class LoadSceneInfo
        {
            private readonly AsyncOperation m_AsyncOperation;
            private readonly string m_SceneAssetName;
            private readonly int m_Priority;
            private readonly DateTime m_StartTime;
            private readonly LoadSceneCallbacks m_LoadSceneCallbacks;
            private readonly object m_UserData;

            public AsyncOperation AsyncOperation { get { return m_AsyncOperation; } }

            public string SceneAssetName { get { return m_SceneAssetName; } }

            public int Priority { get { return m_Priority; } }

            public DateTime StartTime { get { return m_StartTime; } }

            public LoadSceneCallbacks LoadSceneCallbacks { get { return m_LoadSceneCallbacks; } }

            public object UserData { get { return m_UserData; } }

            public LoadSceneInfo(AsyncOperation asyncOperation, string sceneAssetName, int priority, DateTime startTime, LoadSceneCallbacks loadSceneCallbacks, object userData)
            {
                m_AsyncOperation = asyncOperation;
                m_SceneAssetName = sceneAssetName;
                m_Priority = priority;
                m_StartTime = startTime;
                m_LoadSceneCallbacks = loadSceneCallbacks;
                m_UserData = userData;
            }

        }
    }
}