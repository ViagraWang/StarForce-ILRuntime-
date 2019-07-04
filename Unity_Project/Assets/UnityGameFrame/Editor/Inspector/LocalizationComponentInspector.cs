using UnityEditor;
using UnityGameFrame.Runtime;

namespace UnityGameFrame.Editor
{
    [CustomEditor(typeof(LocalizationComponent))]
    internal sealed class LocalizationComponentInspector : GameFrameworkInspector
    {
        //本地序列化属性
        private SerializedProperty m_EnableLoadDictionarySuccessEvent = null;
        private SerializedProperty m_EnableLoadDictionaryFailureEvent = null;
        private SerializedProperty m_EnableLoadDictionaryUpdateEvent = null;
        private SerializedProperty m_EnableLoadDictionaryDependencyAssetEvent = null;

        //辅助器
        private HelperInfo<LocalizationHelperBase> m_LocalizationHelperInfo = new HelperInfo<LocalizationHelperBase>("Localization");

        private void OnEnable()
        {
            m_EnableLoadDictionarySuccessEvent = serializedObject.FindProperty("m_EnableLoadDictionarySuccessEvent");
            m_EnableLoadDictionaryFailureEvent = serializedObject.FindProperty("m_EnableLoadDictionaryFailureEvent");
            m_EnableLoadDictionaryUpdateEvent = serializedObject.FindProperty("m_EnableLoadDictionaryUpdateEvent");
            m_EnableLoadDictionaryDependencyAssetEvent = serializedObject.FindProperty("m_EnableLoadDictionaryDependencyAssetEvent");

            m_LocalizationHelperInfo.Init(serializedObject);

            RefreshTypeNames();
        }

        //刷新类型名称
        private void RefreshTypeNames()
        {
            m_LocalizationHelperInfo.Refresh();
            serializedObject.ApplyModifiedProperties();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            LocalizationComponent t = target as LocalizationComponent;

            m_EnableLoadDictionarySuccessEvent.boolValue = EditorGUILayout.ToggleLeft("Enable Load Dictionary Success Event", m_EnableLoadDictionarySuccessEvent.boolValue);
            m_EnableLoadDictionaryFailureEvent.boolValue = EditorGUILayout.ToggleLeft("Enable Load Dictionary Failure Event", m_EnableLoadDictionaryFailureEvent.boolValue);
            m_EnableLoadDictionaryUpdateEvent.boolValue = EditorGUILayout.ToggleLeft("Enable Load Dictionary Update Event", m_EnableLoadDictionaryUpdateEvent.boolValue);
            m_EnableLoadDictionaryDependencyAssetEvent.boolValue = EditorGUILayout.ToggleLeft("Enable Load Dictionary Dependency Asset Event", m_EnableLoadDictionaryDependencyAssetEvent.boolValue);

            //运行时隐藏
            EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
            {
                m_LocalizationHelperInfo.Draw();
            }
            EditorGUI.EndDisabledGroup();

            //运行时显示
            if (EditorApplication.isPlaying && IsPrefabInHierarchy(t.gameObject))
            {
                EditorGUILayout.LabelField("Language", t.Language.ToString());
                EditorGUILayout.LabelField("System Language", t.SystemLanguage.ToString());
                EditorGUILayout.LabelField("Dictionary Count", t.DictionaryCount.ToString());
            }

            serializedObject.ApplyModifiedProperties();

            Repaint();
        }

        protected override void OnCompileComplete()
        {
            RefreshTypeNames();
        }

        protected override void OnCompileStart()
        {

        }
    }
}
