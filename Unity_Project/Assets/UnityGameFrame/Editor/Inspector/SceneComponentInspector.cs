using UnityEditor;
using UnityEngine;
using UnityGameFrame.Runtime;

namespace UnityGameFrame.Editor
{
    [CustomEditor(typeof(SceneComponent))]
    internal sealed class SceneComponentInspector : GameFrameworkInspector
    {
        //是否开启事件的标志位
        private SerializedProperty m_EnableLoadSceneSuccessEvent = null;
        private SerializedProperty m_EnableLoadSceneFailureEvent = null;
        private SerializedProperty m_EnableLoadSceneUpdateEvent = null;
        private SerializedProperty m_EnableLoadSceneDependencyAssetEvent = null;
        private SerializedProperty m_EnableUnloadSceneSuccessEvent = null;
        private SerializedProperty m_EnableUnloadSceneFailureEvent = null;

        private void OnEnable()
        {
            m_EnableLoadSceneSuccessEvent = serializedObject.FindProperty("m_EnableLoadSceneSuccessEvent");
            m_EnableLoadSceneFailureEvent = serializedObject.FindProperty("m_EnableLoadSceneFailureEvent");
            m_EnableLoadSceneUpdateEvent = serializedObject.FindProperty("m_EnableLoadSceneUpdateEvent");
            m_EnableLoadSceneDependencyAssetEvent = serializedObject.FindProperty("m_EnableLoadSceneDependencyAssetEvent");
            m_EnableUnloadSceneSuccessEvent = serializedObject.FindProperty("m_EnableUnloadSceneSuccessEvent");
            m_EnableUnloadSceneFailureEvent = serializedObject.FindProperty("m_EnableUnloadSceneFailureEvent");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            SceneComponent t = target as SceneComponent;

            //相关属性
            m_EnableLoadSceneSuccessEvent.boolValue = EditorGUILayout.ToggleLeft("Enable Load Scene Success Event", m_EnableLoadSceneSuccessEvent.boolValue);
            m_EnableLoadSceneFailureEvent.boolValue = EditorGUILayout.ToggleLeft("Enable Load Scene Failure Event", m_EnableLoadSceneFailureEvent.boolValue);
            m_EnableLoadSceneUpdateEvent.boolValue = EditorGUILayout.ToggleLeft("Enable Load Scene Update Event", m_EnableLoadSceneUpdateEvent.boolValue);
            m_EnableLoadSceneDependencyAssetEvent.boolValue = EditorGUILayout.ToggleLeft("Enable Load Scene Dependency Asset Event", m_EnableLoadSceneDependencyAssetEvent.boolValue);
            m_EnableUnloadSceneSuccessEvent.boolValue = EditorGUILayout.ToggleLeft("Enable Unload Scene Success Event", m_EnableUnloadSceneSuccessEvent.boolValue);
            m_EnableUnloadSceneFailureEvent.boolValue = EditorGUILayout.ToggleLeft("Enable Unload Scene Failure Event", m_EnableUnloadSceneFailureEvent.boolValue);

            serializedObject.ApplyModifiedProperties();

            if (EditorApplication.isPlaying && IsPrefabInHierarchy(t.gameObject))
            {
                EditorGUILayout.LabelField("Loaded Scene Asset Names", GetSceneNameString(t.GetLoadedSceneAssetNames()));
                EditorGUILayout.LabelField("Loading Scene Asset Names", GetSceneNameString(t.GetLoadingSceneAssetNames()));
                EditorGUILayout.LabelField("Unloading Scene Asset Names", GetSceneNameString(t.GetUnloadingSceneAssetNames()));
                EditorGUILayout.ObjectField("Main Camera", t.MainCamera, typeof(Camera), true);

                Repaint();
            }
        }

        //获取场景名称
        private string GetSceneNameString(string[] sceneAssetNames)
        {
            if (sceneAssetNames == null || sceneAssetNames.Length <= 0)
                return "<Empty>";

            string sceneNameString = string.Empty;
            for (int i = 0; i < sceneAssetNames.Length; i++)
            {
                if (!string.IsNullOrEmpty(sceneNameString))
                    sceneNameString += ", ";

                sceneNameString += SceneComponent.GetSceneName(sceneAssetNames[i]);
            }

            return sceneNameString;
        }

        protected override void OnCompileComplete()
        {

        }

        protected override void OnCompileStart()
        {

        }
    }
}
