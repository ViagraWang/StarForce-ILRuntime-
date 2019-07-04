using GameFramework.Resource;
using UnityEngine;

namespace UnityGameFrame.Runtime
{
    public partial class EditorResourceManager
    {
        //加载资源信息
        private sealed class UnloadSceneInfo
        {
            private readonly AsyncOperation m_AsyncOperation;
            private readonly string m_SceneAssetName;
            private readonly UnloadSceneCallbacks m_UnloadSceneCallbacks;
            private readonly object m_UserData;

            public AsyncOperation AsyncOperation { get { return m_AsyncOperation; } }

            public string SceneAssetName { get { return m_SceneAssetName; } }

            public UnloadSceneCallbacks UnloadSceneCallbacks { get { return m_UnloadSceneCallbacks; } }

            public object UserData { get { return m_UserData; } }

            public UnloadSceneInfo(AsyncOperation asyncOperation, string sceneAssetName, UnloadSceneCallbacks loadSceneCallbacks, object userData)
            {
                m_AsyncOperation = asyncOperation;
                m_SceneAssetName = sceneAssetName;
                m_UnloadSceneCallbacks = loadSceneCallbacks;
                m_UserData = userData;
            }

        }
    }
}