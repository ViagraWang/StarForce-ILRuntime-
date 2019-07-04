using GameFramework;
using UnityEditor;
using UnityGameFrame.Runtime;

namespace UnityGameFrame.Editor
{
    [CustomEditor(typeof(ReferencePoolComponent))]
    internal sealed class ReferencePoolComponentInspector : GameFrameworkInspector
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!EditorApplication.isPlaying)
            {
                EditorGUILayout.HelpBox("Available during runtime only.", MessageType.Info);
                return;
            }

            ReferencePoolComponent t = target as ReferencePoolComponent;

            if(IsPrefabInHierarchy(t.gameObject))
            {
                EditorGUILayout.LabelField("Reference Pool Count", t.Count.ToString());

                ReferencePoolInfo[] referencePoolInfos = t.GetAllReferencePoolInfos();  //引用信息
                for (int i = 0; i < referencePoolInfos.Length; i++)
                {
                    DrawReferencePoolInfo(referencePoolInfos[i]);
                }
            }

            Repaint();
        }

        private void DrawReferencePoolInfo(ReferencePoolInfo referencePoolInfo)
        {
            EditorGUILayout.LabelField(referencePoolInfo.TypeName, Utility.Text.Format("[Unused]{0} [Using]{1} [Acquire]{2} [Release]{3} [Add]{4} [Remove]{5}", referencePoolInfo.UnusedReferenceCount.ToString(), referencePoolInfo.UsingReferenceCount.ToString(), referencePoolInfo.AcquireReferenceCount.ToString(), referencePoolInfo.ReleaseReferenceCount.ToString(), referencePoolInfo.AddReferenceCount.ToString(), referencePoolInfo.RemoveReferenceCount.ToString()));
        }

        protected override void OnCompileComplete()
        {

        }

        protected override void OnCompileStart()
        {

        }
    }
}
