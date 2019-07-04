using Game.Runtime;
using UnityEditor;
using UnityEngine;

namespace Game.Editor
{	
	//脚本化显示的自定义编辑
	[CustomEditor(typeof(DeviceModelConfig))]
	public class DeviceModelConfigInspector : UnityEditor.Editor
    {
	    public override void OnInspectorGUI()
	    {
	        if (GUILayout.Button("Open Device Model Config Editor"))
	            DeviceModelConfigEditorWindow.OpenWindow((DeviceModelConfig)target);
	    }
	}
}
