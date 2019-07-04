using GameFramework;
using GameFramework.Fsm;
using UnityEditor;
using UnityGameFrame.Runtime;

namespace UnityGameFrame.Editor
{
    [CustomEditor(typeof(FsmComponent))]
    internal sealed class FsmComponentInspector : GameFrameworkInspector
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!EditorApplication.isPlaying)
            {
                EditorGUILayout.HelpBox("Available during runtime only.", MessageType.Info);
                return;
            }

            FsmComponent t = target as FsmComponent;
            if(IsPrefabInHierarchy(t.gameObject))
            {
                EditorGUILayout.LabelField("FSM Count", t.Count.ToString());    //状态机数量

                FsmBase[] fsms = t.GetAllFsms();
                for (int i = 0; i < fsms.Length; i++)
                {
                    var fsm = fsms[i];
                    //打印状态机运行信息
                    EditorGUILayout.LabelField(Utility.Text.GetFullName(fsm.OwnerType, fsm.Name), fsm.IsRunning ? Utility.Text.Format("{0}, {1} s", fsm.CurrentStateName, fsm.CurrentStateTime.ToString("F1")) : (fsm.IsDestroyed ? "Destroyed" : "Not Running"));
                }
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
