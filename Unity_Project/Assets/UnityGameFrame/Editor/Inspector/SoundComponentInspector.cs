using UnityEditor;
using UnityGameFrame.Runtime;

namespace UnityGameFrame.Editor
{
    [CustomEditor(typeof(SoundComponent))]
    internal sealed class SoundComponentInspector : GameFrameworkInspector
    {
        //是否开启事件的标志位
        private SerializedProperty m_EnablePlaySoundSuccessEvent = null;
        private SerializedProperty m_EnablePlaySoundFailureEvent = null;
        private SerializedProperty m_EnablePlaySoundUpdateEvent = null;
        private SerializedProperty m_EnablePlaySoundDependencyAssetEvent = null;

        private SerializedProperty m_InstanceRoot = null;   //根对象
        private SerializedProperty m_AudioMixer = null; //声音混合器 
        private SerializedProperty m_SoundGroups = null;    //声音组

        private HelperInfo<SoundHelperBase> m_SoundHelperInfo = new HelperInfo<SoundHelperBase>("Sound");
        private HelperInfo<SoundGroupHelperBase> m_SoundGroupHelperInfo = new HelperInfo<SoundGroupHelperBase>("SoundGroup");
        private HelperInfo<SoundAgentHelperBase> m_SoundAgentHelperInfo = new HelperInfo<SoundAgentHelperBase>("SoundAgent");

        private void OnEnable()
        {
            m_EnablePlaySoundSuccessEvent = serializedObject.FindProperty("m_EnablePlaySoundSuccessEvent");
            m_EnablePlaySoundFailureEvent = serializedObject.FindProperty("m_EnablePlaySoundFailureEvent");
            m_EnablePlaySoundUpdateEvent = serializedObject.FindProperty("m_EnablePlaySoundUpdateEvent");
            m_EnablePlaySoundDependencyAssetEvent = serializedObject.FindProperty("m_EnablePlaySoundDependencyAssetEvent");

            m_InstanceRoot = serializedObject.FindProperty("m_InstanceRoot");
            m_AudioMixer = serializedObject.FindProperty("m_AudioMixer");
            m_SoundGroups = serializedObject.FindProperty("m_SoundGroups");

            m_SoundHelperInfo.Init(serializedObject);
            m_SoundGroupHelperInfo.Init(serializedObject);
            m_SoundAgentHelperInfo.Init(serializedObject);

            RefreshTypeNames();
        }

        private void RefreshTypeNames()
        {
            m_SoundHelperInfo.Refresh();
            m_SoundGroupHelperInfo.Refresh();
            m_SoundAgentHelperInfo.Refresh();
            serializedObject.ApplyModifiedProperties();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            SoundComponent t = target as SoundComponent;

            m_EnablePlaySoundSuccessEvent.boolValue = EditorGUILayout.ToggleLeft("Enable Play Sound Success Event", m_EnablePlaySoundSuccessEvent.boolValue);
            m_EnablePlaySoundFailureEvent.boolValue = EditorGUILayout.ToggleLeft("Enable Play Sound Failure Event", m_EnablePlaySoundFailureEvent.boolValue);
            m_EnablePlaySoundUpdateEvent.boolValue = EditorGUILayout.ToggleLeft("Enable Play Sound Update Event", m_EnablePlaySoundUpdateEvent.boolValue);
            m_EnablePlaySoundDependencyAssetEvent.boolValue = EditorGUILayout.ToggleLeft("Enable Play Sound Dependency Asset Event", m_EnablePlaySoundDependencyAssetEvent.boolValue);

            EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
            {
                EditorGUILayout.PropertyField(m_InstanceRoot);  //根对象
                EditorGUILayout.PropertyField(m_AudioMixer);    //声音混合器
                m_SoundHelperInfo.Draw();
                m_SoundGroupHelperInfo.Draw();
                m_SoundAgentHelperInfo.Draw();
                EditorGUILayout.PropertyField(m_SoundGroups, true); //声音组
            }
            EditorGUI.EndDisabledGroup();

            if (EditorApplication.isPlaying && IsPrefabInHierarchy(t.gameObject))
            {
                EditorGUILayout.LabelField("Sound Group Count", t.SoundGroupCount.ToString());
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
