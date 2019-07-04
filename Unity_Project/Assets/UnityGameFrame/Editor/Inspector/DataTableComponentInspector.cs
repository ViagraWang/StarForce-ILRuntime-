using GameFramework;
using GameFramework.DataTable;
using UnityEditor;
using UnityGameFrame.Runtime;

namespace UnityGameFrame.Editor
{
    [CustomEditor(typeof(DataTableComponent))]
    internal sealed class DataTableComponentInspector : GameFrameworkInspector
    {
        private SerializedProperty m_EnableLoadDataTableSuccessEvent = null;
        private SerializedProperty m_EnableLoadDataTableFailureEvent = null;
        private SerializedProperty m_EnableLoadDataTableUpdateEvent = null;
        private SerializedProperty m_EnableLoadDataTableDependencyAssetEvent = null;

        private HelperInfo<DataTableHelperBase> m_DataTableHelperInfo = new HelperInfo<DataTableHelperBase>("DataTable");

        private void OnEnable()
        {
            m_EnableLoadDataTableSuccessEvent = serializedObject.FindProperty("m_EnableLoadDataTableSuccessEvent");
            m_EnableLoadDataTableFailureEvent = serializedObject.FindProperty("m_EnableLoadDataTableFailureEvent");
            m_EnableLoadDataTableUpdateEvent = serializedObject.FindProperty("m_EnableLoadDataTableUpdateEvent");
            m_EnableLoadDataTableDependencyAssetEvent = serializedObject.FindProperty("m_EnableLoadDataTableDependencyAssetEvent");

            m_DataTableHelperInfo.Init(serializedObject);

            RefreshTypeNames();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            DataTableComponent t = target as DataTableComponent;

            m_EnableLoadDataTableSuccessEvent.boolValue = EditorGUILayout.ToggleLeft("Enable Load Table Success Event", m_EnableLoadDataTableSuccessEvent.boolValue);
            m_EnableLoadDataTableFailureEvent.boolValue = EditorGUILayout.ToggleLeft("Enable Load Table Failure Event", m_EnableLoadDataTableFailureEvent.boolValue);
            m_EnableLoadDataTableUpdateEvent.boolValue = EditorGUILayout.ToggleLeft("Enable Load Table Update Event", m_EnableLoadDataTableUpdateEvent.boolValue);
            m_EnableLoadDataTableDependencyAssetEvent.boolValue = EditorGUILayout.ToggleLeft("Enable Load Table Dependency Asset Event", m_EnableLoadDataTableDependencyAssetEvent.boolValue);

            //运行模式不能绘制辅助器信息
            EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
            {
                m_DataTableHelperInfo.Draw();
            }
            EditorGUI.EndDisabledGroup();

            //运行模式绘制数据表
            if(EditorApplication.isPlaying && IsPrefabInHierarchy(t.gameObject))
            {
                EditorGUILayout.LabelField("Data Table Count", t.Count.ToString()); //数据表数量

                DataTableBase[] dataTables = t.GetAllDataTables();  //获取所有的数据表
                foreach (var dataTable in dataTables)
                {
                    EditorGUILayout.LabelField(Utility.Text.GetFullName(dataTable.Type, dataTable.Name), Utility.Text.Format("{0} Rows", dataTable.Count.ToString()));
                }
            }

            serializedObject.ApplyModifiedProperties();
            Repaint();
        }

        //刷新类名
        private void RefreshTypeNames()
        {
            m_DataTableHelperInfo.Refresh();
            serializedObject.ApplyModifiedProperties();
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
