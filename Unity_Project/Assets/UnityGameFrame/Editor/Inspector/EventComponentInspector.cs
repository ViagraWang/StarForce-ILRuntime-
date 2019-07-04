using UnityEditor;
using UnityGameFrame.Runtime;

namespace UnityGameFrame.Editor
{
    [CustomEditor(typeof(EventComponent))]
    internal sealed class EventComponentInspector : GameFrameworkInspector
    {

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!EditorApplication.isPlaying)
            {
                EditorGUILayout.HelpBox("Available during runtime only.", MessageType.Info);
                return;
            }

            EventComponent t = target as EventComponent;
            if (IsPrefabInHierarchy(t.gameObject))
            {
                EditorGUILayout.LabelField("Event Handler Count", t.EventHandlerCount.ToString());
                EditorGUILayout.LabelField("Event Count", t.EventCount.ToString());
            }

            Repaint();
        }

        protected override void OnCompileComplete()
        {

        }

        protected override void OnCompileStart()
        {

        }
    }
}
