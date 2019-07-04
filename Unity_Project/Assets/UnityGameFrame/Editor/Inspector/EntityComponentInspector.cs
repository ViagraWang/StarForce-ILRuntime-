using GameFramework;
using GameFramework.Entity;
using UnityEditor;
using UnityGameFrame.Runtime;

namespace UnityGameFrame.Editor
{
    [CustomEditor(typeof(EntityComponent))]
    internal sealed class EntityComponentInspector : GameFrameworkInspector
    {
        //相关事件的标志位
        private SerializedProperty m_EnableShowEntitySuccessEvent = null;
        private SerializedProperty m_EnableShowEntityFailureEvent = null;
        private SerializedProperty m_EnableShowEntityUpdateEvent = null;
        private SerializedProperty m_EnableShowEntityDependencyAssetEvent = null;
        private SerializedProperty m_EnableHideEntityCompleteEvent = null;

        private SerializedProperty m_InstanceRoot = null;
        private SerializedProperty m_EntityGroups = null;

        //辅助器
        private HelperInfo<EntityHelperBase> m_EntityHelperInfo = new HelperInfo<EntityHelperBase>("Entity");
        private HelperInfo<EntityGroupHelperBase> m_EntityGroupHelperInfo = new HelperInfo<EntityGroupHelperBase>("EntityGroup");

        private void OnEnable()
        {
            m_EnableShowEntitySuccessEvent = serializedObject.FindProperty("m_EnableShowEntitySuccessEvent");
            m_EnableShowEntityFailureEvent = serializedObject.FindProperty("m_EnableShowEntityFailureEvent");
            m_EnableShowEntityUpdateEvent = serializedObject.FindProperty("m_EnableShowEntityUpdateEvent");
            m_EnableShowEntityDependencyAssetEvent = serializedObject.FindProperty("m_EnableShowEntityDependencyAssetEvent");
            m_EnableHideEntityCompleteEvent = serializedObject.FindProperty("m_EnableHideEntityCompleteEvent");
            m_InstanceRoot = serializedObject.FindProperty("m_InstanceRoot");
            m_EntityGroups = serializedObject.FindProperty("m_EntityGroups");

            m_EntityHelperInfo.Init(serializedObject);
            m_EntityGroupHelperInfo.Init(serializedObject);

            RefreshTypeNames();
        }

        private void RefreshTypeNames()
        {
            m_EntityHelperInfo.Refresh();
            m_EntityGroupHelperInfo.Refresh();
            serializedObject.ApplyModifiedProperties();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EntityComponent t = target as EntityComponent;

            m_EnableShowEntitySuccessEvent.boolValue = EditorGUILayout.ToggleLeft("Enable Show Entity Success Event", m_EnableShowEntitySuccessEvent.boolValue);
            m_EnableShowEntityFailureEvent.boolValue = EditorGUILayout.ToggleLeft("Enable Show Entity Failure Event", m_EnableShowEntityFailureEvent.boolValue);
            m_EnableShowEntityUpdateEvent.boolValue = EditorGUILayout.ToggleLeft("Enable Show Entity Update Event", m_EnableShowEntityUpdateEvent.boolValue);
            m_EnableShowEntityDependencyAssetEvent.boolValue = EditorGUILayout.ToggleLeft("Enable Show Entity Dependency Asset Event", m_EnableShowEntityDependencyAssetEvent.boolValue);
            m_EnableHideEntityCompleteEvent.boolValue = EditorGUILayout.ToggleLeft("Enable Hide Entity Complete Event", m_EnableHideEntityCompleteEvent.boolValue);

            EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
            {
                EditorGUILayout.PropertyField(m_InstanceRoot);
                m_EntityHelperInfo.Draw();
                m_EntityGroupHelperInfo.Draw();
                EditorGUILayout.PropertyField(m_EntityGroups, true);
            }
            EditorGUI.EndDisabledGroup();

            if (EditorApplication.isPlaying && IsPrefabInHierarchy(t.gameObject))
            {
                EditorGUILayout.LabelField("Entity Group Count", t.EntityGroupCount.ToString());    //实体组数量
                EditorGUILayout.LabelField("Entity Count (Total)", t.EntityCount.ToString());   //实体数量
                IEntityGroup[] entityGroups = t.GetAllEntityGroups();
                foreach (IEntityGroup entityGroup in entityGroups)  //显示每个实体组的实体数量
                {
                    EditorGUILayout.LabelField(Utility.Text.Format("Entity Count ({0})", entityGroup.Name), entityGroup.EntityCount.ToString());
                }
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
