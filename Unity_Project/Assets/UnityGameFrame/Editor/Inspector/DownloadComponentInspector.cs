using UnityEditor;
using UnityGameFrame.Runtime;

namespace UnityGameFrame.Editor
{
    [CustomEditor(typeof(DownloadComponent))]
    internal sealed class DownloadComponentInspector : GameFrameworkInspector
    {
        private SerializedProperty m_InstanceRoot = null;
        private SerializedProperty m_DownloadAgentHelperCount = null;
        private SerializedProperty m_Timeout = null;
        private SerializedProperty m_FlushSize = null;

        //辅助器信息
        private HelperInfo<DownloadAgentHelperBase> m_DownloadAgentHelperInfo = new HelperInfo<DownloadAgentHelperBase>("DownloadAgent");

        private void OnEnable()
        {
            m_InstanceRoot = serializedObject.FindProperty("m_InstanceRoot");
            m_DownloadAgentHelperCount = serializedObject.FindProperty("m_DownloadAgentHelperCount");
            m_Timeout = serializedObject.FindProperty("m_Timeout");
            m_FlushSize = serializedObject.FindProperty("m_FlushSize");

            m_DownloadAgentHelperInfo.Init(serializedObject);

            RefreshTypeNames();
        }

        private void RefreshTypeNames()
        {
            m_DownloadAgentHelperInfo.Refresh();
            serializedObject.ApplyModifiedProperties();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            DownloadComponent t = target as DownloadComponent;    //下载组件
            EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
            {
                EditorGUILayout.PropertyField(m_InstanceRoot);  //属性字段序列化
                m_DownloadAgentHelperInfo.Draw();   //绘制辅助器
                m_DownloadAgentHelperCount.intValue = EditorGUILayout.IntSlider("Download Agent Helper Count", m_DownloadAgentHelperCount.intValue, 1, 16); //下载器数量
            }
            EditorGUI.EndDisabledGroup();

            float timeout = EditorGUILayout.Slider("Timeout", m_Timeout.floatValue, 0f, 120f);
            if(timeout != m_Timeout.floatValue)
            {
                if (EditorApplication.isPlaying)    //运行中修改组件的值
                    t.Timeout = timeout;
                else
                    m_Timeout.floatValue = timeout; //非运行时修改属性序列化的值
            }

            int flushSize = EditorGUILayout.DelayedIntField("Flush Size", m_FlushSize.intValue);
            if (flushSize != m_FlushSize.intValue)
            {
                if (EditorApplication.isPlaying)
                    t.FlushSize = flushSize;
                else
                    m_FlushSize.intValue = flushSize;
            }

            //运行时显示一些下载器数据
            if (EditorApplication.isPlaying && IsPrefabInHierarchy(t.gameObject))
            {
                EditorGUILayout.LabelField("Total Agent Count", t.TotalAgentCount.ToString());
                EditorGUILayout.LabelField("Free Agent Count", t.FreeAgentCount.ToString());
                EditorGUILayout.LabelField("Working Agent Count", t.WorkingAgentCount.ToString());
                EditorGUILayout.LabelField("Waiting Agent Count", t.WaitingTaskCount.ToString());
                EditorGUILayout.LabelField("Current Speed", t.CurrentSpeed.ToString());
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
