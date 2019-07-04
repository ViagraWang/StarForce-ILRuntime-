using UnityEditor;
using UnityGameFrame.Runtime;

namespace UnityGameFrame.Editor
{
    /// <summary>
    /// 配置编辑
    /// </summary>
    [CustomEditor(typeof(ConfigComponent))]
    internal sealed class ConfigComponentInspector : GameFrameworkInspector
    {
        //是否开始相关事件的属性
        private SerializedProperty m_EnableLoadConfigSuccessEvent = null;
        private SerializedProperty m_EnableLoadConfigFailureEvent = null;
        private SerializedProperty m_EnableLoadConfigUpdateEvent = null;
        private SerializedProperty m_EnableLoadConfigDependencyAssetEvent = null;

        private HelperInfo<ConfigHelperBase> m_ConfigHelperInfo = new HelperInfo<ConfigHelperBase>("Config");

        private void OnEnable()
        {
            m_EnableLoadConfigSuccessEvent = serializedObject.FindProperty("m_EnableLoadConfigSuccessEvent");
            m_EnableLoadConfigFailureEvent = serializedObject.FindProperty("m_EnableLoadConfigFailureEvent");
            m_EnableLoadConfigUpdateEvent = serializedObject.FindProperty("m_EnableLoadConfigUpdateEvent");
            m_EnableLoadConfigDependencyAssetEvent = serializedObject.FindProperty("m_EnableLoadConfigDependencyAssetEvent");

            m_ConfigHelperInfo.Init(serializedObject);

            RefreshTypeNames();
        }

        //刷新类型名称
        private void RefreshTypeNames()
        {
            m_ConfigHelperInfo.Refresh();
            serializedObject.ApplyModifiedProperties();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            ConfigComponent t = target as ConfigComponent;    //目标脚本

            m_EnableLoadConfigSuccessEvent.boolValue = EditorGUILayout.ToggleLeft("Enable Load Config Success Event", m_EnableLoadConfigSuccessEvent.boolValue);
            m_EnableLoadConfigFailureEvent.boolValue = EditorGUILayout.ToggleLeft("Enable Load Config Failure Event", m_EnableLoadConfigFailureEvent.boolValue);
            m_EnableLoadConfigUpdateEvent.boolValue = EditorGUILayout.ToggleLeft("Enable Load Config Update Event", m_EnableLoadConfigUpdateEvent.boolValue);
            m_EnableLoadConfigDependencyAssetEvent.boolValue = EditorGUILayout.ToggleLeft("Enable Load Config Dependency Asset Event", m_EnableLoadConfigDependencyAssetEvent.boolValue);

            EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
            {
                m_ConfigHelperInfo.Draw();
            }
            EditorGUI.EndDisabledGroup();

            if (EditorApplication.isPlaying && IsPrefabInHierarchy(t.gameObject))
            {
                EditorGUILayout.LabelField("Config Count", t.ConfigCount.ToString());
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
