using System.Reflection;
using UnityEditor;
using UnityGameFrame.Runtime;

namespace UnityGameFrame.Editor
{
    [CustomEditor(typeof(ResourceComponent))]
    internal sealed class ResourceComponentInspector : GameFrameworkInspector
    {
        private static readonly string[] ResourceModeNames = new string[] { "Package", "Updatable" };   //资源模式名称
        //属性
        private SerializedProperty m_ResourceMode = null;
        private SerializedProperty m_ReadWritePathType = null;
        private SerializedProperty m_UnloadUnusedAssetsInterval = null;
        private SerializedProperty m_AssetAutoReleaseInterval = null;
        private SerializedProperty m_AssetCapacity = null;
        private SerializedProperty m_AssetExpireTime = null;
        private SerializedProperty m_AssetPriority = null;
        private SerializedProperty m_ResourceAutoReleaseInterval = null;
        private SerializedProperty m_ResourceCapacity = null;
        private SerializedProperty m_ResourceExpireTime = null;
        private SerializedProperty m_ResourcePriority = null;
        private SerializedProperty m_UpdatePrefixUri = null;
        private SerializedProperty m_UpdateFileCacheLength = null;
        private SerializedProperty m_GenerateReadWriteListLength = null;
        private SerializedProperty m_UpdateRetryCount = null;
        private SerializedProperty m_InstanceRoot = null;
        private SerializedProperty m_LoadResourceAgentHelperCount = null;

        //字段
        private FieldInfo m_EditorResourceModeFieldInfo = null;

        //参数
        private int m_ResourceModeIndex = 0;    //选择资源模式的下标
        private HelperInfo<ResourceHelperBase> m_ResourceHelperInfo = new HelperInfo<ResourceHelperBase>("Resource");
        private HelperInfo<LoadResourceAgentHelperBase> m_LoadResourceAgentHelperInfo = new HelperInfo<LoadResourceAgentHelperBase>("LoadResourceAgent");

        private void OnEnable()
        {
            m_ResourceMode = serializedObject.FindProperty("m_ResourceMode");
            m_ReadWritePathType = serializedObject.FindProperty("m_ReadWritePathType");
            m_UnloadUnusedAssetsInterval = serializedObject.FindProperty("m_UnloadUnusedAssetsInterval");
            m_AssetAutoReleaseInterval = serializedObject.FindProperty("m_AssetAutoReleaseInterval");
            m_AssetCapacity = serializedObject.FindProperty("m_AssetCapacity");
            m_AssetExpireTime = serializedObject.FindProperty("m_AssetExpireTime");
            m_AssetPriority = serializedObject.FindProperty("m_AssetPriority");
            m_ResourceAutoReleaseInterval = serializedObject.FindProperty("m_ResourceAutoReleaseInterval");
            m_ResourceCapacity = serializedObject.FindProperty("m_ResourceCapacity");
            m_ResourceExpireTime = serializedObject.FindProperty("m_ResourceExpireTime");
            m_ResourcePriority = serializedObject.FindProperty("m_ResourcePriority");
            m_UpdatePrefixUri = serializedObject.FindProperty("m_UpdatePrefixUri");
            m_UpdateFileCacheLength = serializedObject.FindProperty("m_UpdateFileCacheLength");
            m_GenerateReadWriteListLength = serializedObject.FindProperty("m_GenerateReadWriteListLength");
            m_UpdateRetryCount = serializedObject.FindProperty("m_UpdateRetryCount");
            m_InstanceRoot = serializedObject.FindProperty("m_InstanceRoot");
            m_LoadResourceAgentHelperCount = serializedObject.FindProperty("m_LoadResourceAgentHelperCount");

            m_EditorResourceModeFieldInfo = target.GetType().GetField("m_IsEditorResourceMode", BindingFlags.NonPublic | BindingFlags.Instance);

            m_ResourceHelperInfo.Init(serializedObject);
            m_LoadResourceAgentHelperInfo.Init(serializedObject);

            m_ResourceModeIndex = (m_ResourceMode.enumValueIndex > 0 ? m_ResourceMode.enumValueIndex - 1 : 0);

            RefreshTypeNames();
        }

        //刷新类型名称
        private void RefreshTypeNames()
        {
            m_ResourceHelperInfo.Refresh();
            m_LoadResourceAgentHelperInfo.Refresh();
            serializedObject.ApplyModifiedProperties();
        }


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            ResourceComponent t = target as ResourceComponent;

            bool isEditorResourceMode = (bool)m_EditorResourceModeFieldInfo.GetValue(target);   //编辑器模式？
            if (isEditorResourceMode)
                EditorGUILayout.HelpBox("Editor resource mode is enabled. Some options are disabled", MessageType.Warning);

            EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
            {
                //选择资源模式
                if (EditorApplication.isPlaying && IsPrefabInHierarchy(t.gameObject))
                {
                    //运行中不能选择
                    EditorGUILayout.EnumPopup("Resource Mode", t.ResourceMode);
                }
                else
                {
                    int selectedIndex = EditorGUILayout.Popup("Resource Mode", m_ResourceModeIndex, ResourceModeNames);
                    if (selectedIndex != m_ResourceModeIndex)
                    {
                        m_ResourceModeIndex = selectedIndex;
                        m_ResourceMode.enumValueIndex = selectedIndex + 1;  //+1是因为定义的资源模式枚举中0表示Unspecified
                    }
                }
                //资源读写路径
                m_ReadWritePathType.enumValueIndex = (byte)(ReadWritePathType)EditorGUILayout.EnumPopup("Read Write Path Type", t.ReadWritePathType);
            }
            EditorGUI.EndDisabledGroup();

            float tempFloat = 0f;
            int tempInt = 0;
            //卸载没用资源的频率
            tempFloat = EditorGUILayout.Slider("Unload Unused Assets Interval", m_UnloadUnusedAssetsInterval.floatValue, 0f, 3600f);
            if (tempFloat != m_UnloadUnusedAssetsInterval.floatValue)
            {
                if (EditorApplication.isPlaying)
                    t.UnloadUnusedAssetsInterval = tempFloat;
                else
                    m_UnloadUnusedAssetsInterval.floatValue = tempFloat;
            }

            EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying && isEditorResourceMode);
            {
                //判断可更新模式
                if (m_ResourceModeIndex == 1)
                {
                    //资源更新网址
                    string updatePrefixUri = EditorGUILayout.DelayedTextField("Update Prefix Uri", m_UpdatePrefixUri.stringValue);
                    if (updatePrefixUri != m_UpdatePrefixUri.stringValue)
                    {
                        if (EditorApplication.isPlaying)
                            t.UpdatePrefixUri = updatePrefixUri;
                        else
                            m_UpdatePrefixUri.stringValue = updatePrefixUri;
                    }

                    //更新文件缓存长度
                    int updateFileCacheLength = EditorGUILayout.DelayedIntField("Update File Cache Length", m_UpdateFileCacheLength.intValue);
                    if (updateFileCacheLength != m_UpdateFileCacheLength.intValue)
                    {
                        if (EditorApplication.isPlaying)
                            t.UpdateFileCacheLength = updateFileCacheLength;
                        else
                            m_UpdateFileCacheLength.intValue = updateFileCacheLength;
                        
                    }

                    //更新资源列表频率大小
                    int generateReadWriteListLength = EditorGUILayout.DelayedIntField("Generate Read Write List Length", m_GenerateReadWriteListLength.intValue);
                    if (generateReadWriteListLength != m_GenerateReadWriteListLength.intValue)
                    {
                        if (EditorApplication.isPlaying)
                            t.GenerateReadWriteListLength = generateReadWriteListLength;
                        else
                            m_GenerateReadWriteListLength.intValue = generateReadWriteListLength;
                    }

                    //资源更新重试次数
                    int updateRetryCount = EditorGUILayout.DelayedIntField("Update Retry Count", m_UpdateRetryCount.intValue);
                    if (updateRetryCount != m_UpdateRetryCount.intValue)
                    {
                        if (EditorApplication.isPlaying)
                            t.UpdateRetryCount = updateRetryCount;
                        else
                            m_UpdateRetryCount.intValue = updateRetryCount;
                    }
                }

                //自动释放可释放资源的频率
                tempFloat = EditorGUILayout.DelayedFloatField("Asset Auto Release Interval", m_AssetAutoReleaseInterval.floatValue);
                if (tempFloat != m_AssetAutoReleaseInterval.floatValue)
                {
                    if (EditorApplication.isPlaying)
                        t.AssetAutoReleaseInterval = tempFloat;
                    else
                        m_AssetAutoReleaseInterval.floatValue = tempFloat;
                }

                //资源对象池容量
                tempInt = EditorGUILayout.DelayedIntField("Asset Capacity", m_AssetCapacity.intValue);
                if (tempInt != m_AssetCapacity.intValue)
                {
                    if (EditorApplication.isPlaying)
                        t.AssetCapacity = tempInt;
                    else
                        m_AssetCapacity.intValue = tempInt;
                }

                //对象池对象过期秒数
                tempFloat = EditorGUILayout.DelayedFloatField("Asset Expire Time", m_AssetExpireTime.floatValue);
                if (tempFloat != m_AssetExpireTime.floatValue)
                {
                    if (EditorApplication.isPlaying)
                        t.AssetExpireTime = tempFloat;
                    else
                        m_AssetExpireTime.floatValue = tempFloat;
                }

                //对象池对象优先级
                tempInt = EditorGUILayout.DelayedIntField("Asset Priority", m_AssetPriority.intValue);
                if (tempInt != m_AssetPriority.intValue)
                {
                    if (EditorApplication.isPlaying)
                        t.AssetPriority = tempInt;
                    else
                        m_AssetPriority.intValue = tempInt;
                }

                //资源文件自动释放的频率
                tempFloat = EditorGUILayout.DelayedFloatField("Resource Auto Release Interval", m_ResourceAutoReleaseInterval.floatValue);
                if (tempFloat != m_ResourceAutoReleaseInterval.floatValue)
                {
                    if (EditorApplication.isPlaying)
                        t.ResourceAutoReleaseInterval = tempFloat;
                    else
                        m_ResourceAutoReleaseInterval.floatValue = tempFloat;
                }

                //资源文件池容量
                tempInt = EditorGUILayout.DelayedIntField("Resource Capacity", m_ResourceCapacity.intValue);
                if (tempInt != m_ResourceCapacity.intValue)
                {
                    if (EditorApplication.isPlaying)
                        t.ResourceCapacity = tempInt;
                    else
                        m_ResourceCapacity.intValue = tempInt;
                }

                //资源文件池对象过期秒数
                tempFloat = EditorGUILayout.DelayedFloatField("Resource Expire Time", m_ResourceExpireTime.floatValue);
                if (tempFloat != m_ResourceExpireTime.floatValue)
                {
                    if (EditorApplication.isPlaying)
                        t.ResourceExpireTime = tempFloat;
                    else
                        m_ResourceExpireTime.floatValue = tempFloat;
                }

                //资源文件池对象优先级
                tempInt = EditorGUILayout.DelayedIntField("Resource Priority", m_ResourcePriority.intValue);
                if (tempInt != m_ResourcePriority.intValue)
                {
                    if (EditorApplication.isPlaying)
                        t.ResourcePriority = tempInt;
                    else
                        m_ResourcePriority.intValue = tempInt;
                }
            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
            {
                EditorGUILayout.PropertyField(m_InstanceRoot);  //根对象

                //绘制辅助器
                m_ResourceHelperInfo.Draw();
                m_LoadResourceAgentHelperInfo.Draw();
                //资源辅助器数量
                m_LoadResourceAgentHelperCount.intValue = EditorGUILayout.IntSlider("Load Resource Agent Helper Count", m_LoadResourceAgentHelperCount.intValue, 1, 64);
            }
            EditorGUI.EndDisabledGroup();

            //运行时显示相关信息
            if (EditorApplication.isPlaying && IsPrefabInHierarchy(t.gameObject))
            {
                EditorGUILayout.LabelField("Read Only Path", t.ReadOnlyPath.ToString());
                EditorGUILayout.LabelField("Read Write Path", t.ReadWritePath.ToString());
                EditorGUILayout.LabelField("Current Variant", t.CurrentVariant ?? "<Unknwon>");
                EditorGUILayout.LabelField("Applicable Game Version", isEditorResourceMode ? "N/A" : t.ApplicableGameVersion ?? "<Unknwon>");
                EditorGUILayout.LabelField("Internal Resource Version", isEditorResourceMode ? "N/A" : t.InternalResourceVersion.ToString());
                EditorGUILayout.LabelField("Asset Count", isEditorResourceMode ? "N/A" : t.AssetCount.ToString());
                EditorGUILayout.LabelField("Resource Count", isEditorResourceMode ? "N/A" : t.ResourceCount.ToString());
                EditorGUILayout.LabelField("Resource Group Count", isEditorResourceMode ? "N/A" : t.ResourceGroupCount.ToString());

                if (m_ResourceModeIndex == 1)
                {
                    EditorGUILayout.LabelField("Updating Resource Group", isEditorResourceMode ? "N/A" : t.UpdatingResourceGroup != null ? t.UpdatingResourceGroup.Name : "<Unknwon>");
                    EditorGUILayout.LabelField("Update Waiting Count", isEditorResourceMode ? "N/A" : t.UpdateWaitingCount.ToString());
                    EditorGUILayout.LabelField("Update Candidate Count", isEditorResourceMode ? "N/A" : t.UpdateCandidateCount.ToString());
                    EditorGUILayout.LabelField("Updating Count", isEditorResourceMode ? "N/A" : t.UpdatingCount.ToString());
                }
                EditorGUILayout.LabelField("Load Total Agent Count", isEditorResourceMode ? "N/A" : t.LoadTotalAgentCount.ToString());
                EditorGUILayout.LabelField("Load Free Agent Count", isEditorResourceMode ? "N/A" : t.LoadFreeAgentCount.ToString());
                EditorGUILayout.LabelField("Load Working Agent Count", isEditorResourceMode ? "N/A" : t.LoadWorkingAgentCount.ToString());
                EditorGUILayout.LabelField("Load Waiting Task Count", isEditorResourceMode ? "N/A" : t.LoadWaitingTaskCount.ToString());
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
