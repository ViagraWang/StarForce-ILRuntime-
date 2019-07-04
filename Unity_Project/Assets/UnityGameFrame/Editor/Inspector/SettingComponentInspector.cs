using UnityEditor;
using UnityEngine;
using UnityGameFrame.Runtime;

namespace UnityGameFrame.Editor
{
    [CustomEditor(typeof(SettingComponent))]
    internal sealed class SettingComponentInspector : GameFrameworkInspector
    {
        private HelperInfo<SettingHelperBase> m_SettingHelperInfo = new HelperInfo<SettingHelperBase>("Setting");

        private void OnEnable()
        {
            m_SettingHelperInfo.Init(serializedObject);
            RefreshTypeNames();
        }

        private void RefreshTypeNames()
        {
            m_SettingHelperInfo.Refresh();
            serializedObject.ApplyModifiedProperties();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            SettingComponent t = target as SettingComponent;

            //运行模式不可操作
            EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
            {
                m_SettingHelperInfo.Draw();
            }
            EditorGUI.EndDisabledGroup();

            if (EditorApplication.isPlaying)
            {
                //点击保存持久层数据
                if(GUILayout.Button("Save Settings"))
                    t.Save();

                //点击移除所有持久层数据
                if (GUILayout.Button("Remove All Settings"))
                    t.RemoveAllSettings();
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
