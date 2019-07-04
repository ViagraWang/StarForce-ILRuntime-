using UnityEditor;
using UnityGameFrame.Runtime;

namespace UnityGameFrame.Editor
{
    [CustomEditor(typeof(WebRequestComponent))]
    internal sealed class WebRequestComponentInspector : GameFrameworkInspector
    {
        private SerializedProperty m_InstanceRoot = null;   //根对象
        private SerializedProperty m_WebRequestAgentHelperCount = null; //代理辅助器数量
        private SerializedProperty m_Timeout = null;    //超时时间

        private HelperInfo<WebRequestAgentHelperBase> m_WebRequestAgentHelperInfo = new HelperInfo<WebRequestAgentHelperBase>("WebRequestAgent");

        private void OnEnable()
        {
            m_InstanceRoot = serializedObject.FindProperty("m_InstanceRoot");
            m_WebRequestAgentHelperCount = serializedObject.FindProperty("m_WebRequestAgentHelperCount");
            m_Timeout = serializedObject.FindProperty("m_Timeout");

            m_WebRequestAgentHelperInfo.Init(serializedObject);

            RefreshTypeNames();
        }

        private void RefreshTypeNames()
        {
            m_WebRequestAgentHelperInfo.Refresh();
            serializedObject.ApplyModifiedProperties();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            WebRequestComponent t = target as WebRequestComponent;

            EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
            {
                EditorGUILayout.PropertyField(m_InstanceRoot);
                m_WebRequestAgentHelperInfo.Draw();
                m_WebRequestAgentHelperCount.intValue = EditorGUILayout.IntSlider("Web Request Agent Helper Count", m_WebRequestAgentHelperCount.intValue, 1, 16);
            }
            EditorGUI.EndDisabledGroup();

            //超时时间
            float timeout = EditorGUILayout.Slider("Timeout", m_Timeout.floatValue, 0f, 120f);
            if (timeout != m_Timeout.floatValue)
            {
                if (EditorApplication.isPlaying)
                    t.Timeout = timeout;
                else
                    m_Timeout.floatValue = timeout;
            }

            //运行时显示一些信息
            if (EditorApplication.isPlaying && IsPrefabInHierarchy(t.gameObject))
            {
                EditorGUILayout.LabelField("Total Agent Count", t.TotalAgentCount.ToString());
                EditorGUILayout.LabelField("Free Agent Count", t.FreeAgentCount.ToString());
                EditorGUILayout.LabelField("Working Agent Count", t.WorkingAgentCount.ToString());
                EditorGUILayout.LabelField("Waiting Agent Count", t.WaitingTaskCount.ToString());
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
