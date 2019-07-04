using GameFramework.DataNode;
using UnityEditor;
using UnityGameFrame.Runtime;

namespace UnityGameFrame.Editor
{
    [CustomEditor(typeof(DataNodeComponent))]
    internal sealed class DataNodeComponentInspector : GameFrameworkInspector
    {

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (!EditorApplication.isPlaying)
            {
                EditorGUILayout.HelpBox("Available during runtime only.", MessageType.Info);
                return;
            }

            DataNodeComponent t = target as DataNodeComponent;
            if(IsPrefabInHierarchy(t.gameObject))  //非预设才显示数据节点
            {
                DrawDataNode(t.Root);
            }
            Repaint();
        }

        //绘制数据节点
        private void DrawDataNode(IDataNode dataNode)
        {
            EditorGUILayout.LabelField(dataNode.FullName, dataNode.ToDataString()); //显示节点信息
            IDataNode[] child = dataNode.GetAllChild(); //子节点
            for (int i = 0; i < child.Length; i++)
            {
                DrawDataNode(child[i]);
            }
        }

        protected override void OnCompileComplete()
        {

        }

        protected override void OnCompileStart()
        {

        }
    }
}
