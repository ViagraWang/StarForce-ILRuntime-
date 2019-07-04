using UnityEditor;
using UnityGameFrame.Runtime;

namespace UnityGameFrame.Editor
{
    [CustomEditor(typeof(UIComponent))]
    internal sealed class UIComponentInspector : GameFrameworkInspector
    {
        //是否开启事件的标志位
        private SerializedProperty m_EnableOpenUIFormSuccessEvent = null;
        private SerializedProperty m_EnableOpenUIFormFailureEvent = null;
        private SerializedProperty m_EnableOpenUIFormUpdateEvent = null;
        private SerializedProperty m_EnableOpenUIFormDependencyAssetEvent = null;
        private SerializedProperty m_EnableCloseUIFormCompleteEvent = null;
        //对象池属性
        private SerializedProperty m_InstanceAutoReleaseInterval = null;
        private SerializedProperty m_InstanceCapacity = null;
        private SerializedProperty m_InstanceExpireTime = null;
        private SerializedProperty m_InstancePriority = null;

        private SerializedProperty m_InstanceRoot = null;   //根对象
        private SerializedProperty m_UIGroups = null;   //界面组

        private HelperInfo<UIFormHelperBase> m_UIFormHelperInfo = new HelperInfo<UIFormHelperBase>("UIForm");
        private HelperInfo<UIGroupHelperBase> m_UIGroupHelperInfo = new HelperInfo<UIGroupHelperBase>("UIGroup");

        private void OnEnable()
        {
            m_EnableOpenUIFormSuccessEvent = serializedObject.FindProperty("m_EnableOpenUIFormSuccessEvent");
            m_EnableOpenUIFormFailureEvent = serializedObject.FindProperty("m_EnableOpenUIFormFailureEvent");
            m_EnableOpenUIFormUpdateEvent = serializedObject.FindProperty("m_EnableOpenUIFormUpdateEvent");
            m_EnableOpenUIFormDependencyAssetEvent = serializedObject.FindProperty("m_EnableOpenUIFormDependencyAssetEvent");
            m_EnableCloseUIFormCompleteEvent = serializedObject.FindProperty("m_EnableCloseUIFormCompleteEvent");
            m_InstanceAutoReleaseInterval = serializedObject.FindProperty("m_InstanceAutoReleaseInterval");
            m_InstanceCapacity = serializedObject.FindProperty("m_InstanceCapacity");
            m_InstanceExpireTime = serializedObject.FindProperty("m_InstanceExpireTime");
            m_InstancePriority = serializedObject.FindProperty("m_InstancePriority");
            m_InstanceRoot = serializedObject.FindProperty("m_InstanceRoot");
            m_UIGroups = serializedObject.FindProperty("m_UIGroups");

            m_UIFormHelperInfo.Init(serializedObject);
            m_UIGroupHelperInfo.Init(serializedObject);

            RefreshTypeNames();
        }

        private void RefreshTypeNames()
        {
            m_UIFormHelperInfo.Refresh();
            m_UIGroupHelperInfo.Refresh();
            serializedObject.ApplyModifiedProperties();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            UIComponent t = target as UIComponent;

            m_EnableOpenUIFormSuccessEvent.boolValue = EditorGUILayout.ToggleLeft("Enable Open UIForm Success Event", m_EnableOpenUIFormSuccessEvent.boolValue);
            m_EnableOpenUIFormFailureEvent.boolValue = EditorGUILayout.ToggleLeft("Enable Open UIForm Failure Event", m_EnableOpenUIFormFailureEvent.boolValue);
            m_EnableOpenUIFormUpdateEvent.boolValue = EditorGUILayout.ToggleLeft("Enable Open UIForm Update Event", m_EnableOpenUIFormUpdateEvent.boolValue);
            m_EnableOpenUIFormDependencyAssetEvent.boolValue = EditorGUILayout.ToggleLeft("Enable Open UIForm Dependency Asset Event", m_EnableOpenUIFormDependencyAssetEvent.boolValue);
            m_EnableCloseUIFormCompleteEvent.boolValue = EditorGUILayout.ToggleLeft("Enable Close UIForm Complete Event", m_EnableCloseUIFormCompleteEvent.boolValue);

            //对象池实例自动释放的频率
            float instanceAutoReleaseInterval = EditorGUILayout.DelayedFloatField("Instance Auto Release Interval", m_InstanceAutoReleaseInterval.floatValue);
            if (instanceAutoReleaseInterval != m_InstanceAutoReleaseInterval.floatValue)
            {
                if (EditorApplication.isPlaying)
                    t.InstanceAutoReleaseInterval = instanceAutoReleaseInterval;
                else
                    m_InstanceAutoReleaseInterval.floatValue = instanceAutoReleaseInterval;
            }

            //对象池的容量
            int instanceCapacity = EditorGUILayout.DelayedIntField("Instance Capacity", m_InstanceCapacity.intValue);
            if (instanceCapacity != m_InstanceCapacity.intValue)
            {
                if (EditorApplication.isPlaying)
                    t.InstanceCapacity = instanceCapacity;
                else
                    m_InstanceCapacity.intValue = instanceCapacity;
            }

            //对象池实例过期秒数
            float instanceExpireTime = EditorGUILayout.DelayedFloatField("Instance Expire Time", m_InstanceExpireTime.floatValue);
            if (instanceExpireTime != m_InstanceExpireTime.floatValue)
            {
                if (EditorApplication.isPlaying)
                    t.InstanceExpireTime = instanceExpireTime;
                else
                    m_InstanceExpireTime.floatValue = instanceExpireTime;
            }

            //对象池实例的优先级
            int instancePriority = EditorGUILayout.DelayedIntField("Instance Priority", m_InstancePriority.intValue);
            if (instancePriority != m_InstancePriority.intValue)
            {
                if (EditorApplication.isPlaying)
                    t.InstancePriority = instancePriority;
                else
                    m_InstancePriority.intValue = instancePriority;
            }

            EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
            {
                EditorGUILayout.PropertyField(m_InstanceRoot);
                m_UIFormHelperInfo.Draw();
                m_UIGroupHelperInfo.Draw();
                EditorGUILayout.PropertyField(m_UIGroups, true);
            }
            EditorGUI.EndDisabledGroup();

            //显示界面组数量
            if (EditorApplication.isPlaying && IsPrefabInHierarchy(t.gameObject))
            {
                EditorGUILayout.LabelField("UI Group Count", t.UIGroupCount.ToString());
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
