using System;
using System.Collections.Generic;
using UnityEditor;
using UnityGameFrame.Runtime;

namespace UnityGameFrame.Editor
{
    [CustomEditor(typeof(EditorResourceManager))]
    internal sealed class EditorResourceManagerInspector : GameFrameworkInspector
    {
        private SerializedProperty m_LoadAssetCountPerFrame = null;
        private SerializedProperty m_MinLoadAssetRandomDelaySeconds = null;
        private SerializedProperty m_MaxLoadAssetRandomDelaySeconds = null;


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorResourceManager t = target as EditorResourceManager;
            if (EditorApplication.isPlaying && IsPrefabInHierarchy(t.gameObject))
                EditorGUILayout.LabelField("Load Waiting Asset Count", t.LoadWaitingAssetCount.ToString());

            EditorGUILayout.PropertyField(m_LoadAssetCountPerFrame);
            EditorGUILayout.PropertyField(m_MinLoadAssetRandomDelaySeconds);
            EditorGUILayout.PropertyField(m_MaxLoadAssetRandomDelaySeconds);

            serializedObject.ApplyModifiedProperties();

            Repaint();
        }

        private void OnEnable()
        {
            m_LoadAssetCountPerFrame = serializedObject.FindProperty("m_LoadAssetCountPerFrame");
            m_MinLoadAssetRandomDelaySeconds = serializedObject.FindProperty("m_MinLoadAssetRandomDelaySeconds");
            m_MaxLoadAssetRandomDelaySeconds = serializedObject.FindProperty("m_MaxLoadAssetRandomDelaySeconds");
        }

        protected override void OnCompileComplete()
        {

        }

        protected override void OnCompileStart()
        {

        }
    }
}
