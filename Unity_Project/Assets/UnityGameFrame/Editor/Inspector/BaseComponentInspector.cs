using GameFramework;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityGameFrame.Runtime;

namespace UnityGameFrame.Editor
{
    [CustomEditor(typeof(BaseComponent))]
    internal sealed class BaseComponentInspector : GameFrameworkInspector
    {
        private const string NoneOptionName = "<None>";

        private static readonly float[] GameSpeed = new float[] { 0f, 0.01f, 0.1f, 0.25f, 0.5f, 1f, 1.5f, 2f, 4f, 8f }; //游戏速度
        private static readonly string[] GameSpeedTexts = new string[] { "0x", "0.01x", "0.1x", "0.25x", "0.5x", "1x", "1.5x", "2x", "4x", "8x" };

        //属性
        private SerializedProperty m_EditorResourceMode = null; //是否编辑器资源模式
        private SerializedProperty m_EditorLanguage = null; //编辑器语言
        private SerializedProperty m_EditorMinLoadAssetRandomDelaySeconds = null; //编辑器模拟资源加载最小延迟秒数
        private SerializedProperty m_EditorMaxLoadAssetRandomDelaySeconds = null; //编辑器模拟资源加载最大延迟秒数
        private SerializedProperty m_VersionHelperTypeName = null;      //版本辅助器类名
        private SerializedProperty m_LogHelperTypeName = null;  //打印辅助器类名
        private SerializedProperty m_ZipHelperTypeName = null;  //压缩辅助器类名
        private SerializedProperty m_JsonHelperTypeName = null; //Json辅助器类名
        private SerializedProperty m_ProfilerHelperTypeName = null; //调试辅助器类名
        private SerializedProperty m_FrameRate = null;  //帧率
        private SerializedProperty m_GameSpeed = null;  //游戏速度
        private SerializedProperty m_RunInBackground = null;    //是否后台运行
        private SerializedProperty m_NeverSleep = null; //从不休眠

        private string[] m_VersionHelperTypeNames = null;
        private int m_VersionHelperTypeNameIndex = 0;
        private string[] m_LogHelperTypeNames = null;
        private int m_LogHelperTypeNameIndex = 0;
        private string[] m_ZipHelperTypeNames = null;
        private int m_ZipHelperTypeNameIndex = 0;
        private string[] m_JsonHelperTypeNames = null;
        private int m_JsonHelperTypeNameIndex = 0;
        private string[] m_ProfilerHelperTypeNames = null;
        private int m_ProfilerHelperTypeNameIndex = 0;

        private readonly List<string> TempList = new List<string>();

        private void OnEnable()
        {
            m_EditorResourceMode = serializedObject.FindProperty("m_IsEditorResourceMode");
            m_EditorLanguage = serializedObject.FindProperty("m_EditorLanguage");
            m_EditorMinLoadAssetRandomDelaySeconds = serializedObject.FindProperty("m_MinLoadAssetRandomDelaySeconds");
            m_EditorMaxLoadAssetRandomDelaySeconds = serializedObject.FindProperty("m_MaxLoadAssetRandomDelaySeconds");
            m_EditorLanguage = serializedObject.FindProperty("m_EditorLanguage");
            m_VersionHelperTypeName = serializedObject.FindProperty("m_VersionHelperTypeName");
            m_LogHelperTypeName = serializedObject.FindProperty("m_LogHelperTypeName");
            m_ZipHelperTypeName = serializedObject.FindProperty("m_ZipHelperTypeName");
            m_JsonHelperTypeName = serializedObject.FindProperty("m_JsonHelperTypeName");
            m_ProfilerHelperTypeName = serializedObject.FindProperty("m_ProfilerHelperTypeName");
            m_FrameRate = serializedObject.FindProperty("m_FrameRate");
            m_GameSpeed = serializedObject.FindProperty("m_GameSpeed");
            m_RunInBackground = serializedObject.FindProperty("m_IsRunInBackground");
            m_NeverSleep = serializedObject.FindProperty("m_NeverSleep");

            RefreshTypeNames(); //刷新类型
        }

        //刷新类型名称
        private void RefreshTypeNames()
        {
            //本辅助器类型
            TempList.Clear();
            TempList.Add(NoneOptionName);
            TempList.AddRange(Type.GetRunSubClassNames(typeof(Version.IVersionHelper)));  //反射获取版
            m_VersionHelperTypeNames = TempList.ToArray();
            m_VersionHelperTypeNameIndex = 0;
            if (!string.IsNullOrEmpty(m_VersionHelperTypeName.stringValue)) //如果保存的属性存在值
            {
                m_VersionHelperTypeNameIndex = TempList.IndexOf(m_VersionHelperTypeName.stringValue);
                if (m_VersionHelperTypeNameIndex <= 0)
                {
                    m_VersionHelperTypeNameIndex = 0;
                    m_VersionHelperTypeName.stringValue = null;
                }
            }

            //打印辅助器
            TempList.Clear();
            TempList.Add(NoneOptionName);
            TempList.AddRange(Type.GetRunSubClassNames(typeof(GameFrameworkLog.ILogHelper)));
            m_LogHelperTypeNames = TempList.ToArray();
            m_LogHelperTypeNameIndex = 0;
            if (!string.IsNullOrEmpty(m_LogHelperTypeName.stringValue))
            {
                m_LogHelperTypeNameIndex = TempList.IndexOf(m_LogHelperTypeName.stringValue);
                if (m_LogHelperTypeNameIndex <= 0)
                {
                    m_LogHelperTypeNameIndex = 0;
                    m_LogHelperTypeName.stringValue = null;
                }
            }

            //压缩辅助器
            TempList.Clear();
            TempList.Add(NoneOptionName);
            TempList.AddRange(Type.GetRunSubClassNames(typeof(Utility.Zip.IZipHelper)));
            m_ZipHelperTypeNames = TempList.ToArray();
            m_ZipHelperTypeNameIndex = 0;
            if (!string.IsNullOrEmpty(m_ZipHelperTypeName.stringValue))
            {
                m_ZipHelperTypeNameIndex = TempList.IndexOf(m_ZipHelperTypeName.stringValue);
                if (m_ZipHelperTypeNameIndex <= 0)
                {
                    m_ZipHelperTypeNameIndex = 0;
                    m_ZipHelperTypeName.stringValue = null;
                }
            }

            //Json辅助器
            TempList.Clear();
            TempList.Add(NoneOptionName);
            TempList.AddRange(Type.GetRunSubClassNames(typeof(Utility.Json.IJsonHelper)));
            m_JsonHelperTypeNames = TempList.ToArray();
            m_JsonHelperTypeNameIndex = 0;
            if (!string.IsNullOrEmpty(m_JsonHelperTypeName.stringValue))
            {
                m_JsonHelperTypeNameIndex = TempList.IndexOf(m_JsonHelperTypeName.stringValue);
                if (m_JsonHelperTypeNameIndex <= 0)
                {
                    m_JsonHelperTypeNameIndex = 0;
                    m_JsonHelperTypeName.stringValue = null;
                }
            }

            //调试辅助器
            TempList.Clear();
            TempList.Add(NoneOptionName);
            TempList.AddRange(Type.GetRunSubClassNames(typeof(Utility.Profiler.IProfilerHelper)));
            m_ProfilerHelperTypeNames = TempList.ToArray();
            m_ProfilerHelperTypeNameIndex = 0;
            if (!string.IsNullOrEmpty(m_ProfilerHelperTypeName.stringValue))
            {
                m_ProfilerHelperTypeNameIndex = TempList.IndexOf(m_ProfilerHelperTypeName.stringValue);
                if (m_ProfilerHelperTypeNameIndex <= 0)
                {
                    m_ProfilerHelperTypeNameIndex = 0;
                    m_ProfilerHelperTypeName.stringValue = null;
                }
            }

            serializedObject.ApplyModifiedProperties(); //保存修改的属性
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();

            BaseComponent t = target as BaseComponent;

            EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
            {
                //编辑器模式单选框
                m_EditorResourceMode.boolValue = EditorGUILayout.BeginToggleGroup("Editor Resource Mode", m_EditorResourceMode.boolValue);
                {
                    EditorGUILayout.HelpBox("Editor resource mode option is only for editor mode. Game Framework will use editor resource files, which you should validate first.", MessageType.Warning);
                    EditorGUILayout.PropertyField(m_EditorLanguage);    //编辑器语言
                    EditorGUILayout.HelpBox("Editor language option is only use for localization test in editor mode.", MessageType.Info);
                    m_EditorMinLoadAssetRandomDelaySeconds.floatValue = EditorGUILayout.Slider("Min Load Asset Delay Seconds", m_EditorMinLoadAssetRandomDelaySeconds.floatValue, 0f, 2f);
                    m_EditorMaxLoadAssetRandomDelaySeconds.floatValue = EditorGUILayout.Slider("Max Load Asset Delay Seconds", m_EditorMaxLoadAssetRandomDelaySeconds.floatValue, m_EditorMinLoadAssetRandomDelaySeconds.floatValue, 5f);
                }
                EditorGUILayout.EndToggleGroup();

                EditorGUILayout.BeginVertical("box");
                {
                    EditorGUILayout.LabelField("Global Helpers", EditorStyles.boldLabel);   //标题
                    //选择版本辅助器
                    int versionHelperSelectedIndex = EditorGUILayout.Popup("Version Helper", m_VersionHelperTypeNameIndex, m_VersionHelperTypeNames);
                    if (versionHelperSelectedIndex != m_VersionHelperTypeNameIndex)
                    {
                        m_VersionHelperTypeNameIndex = versionHelperSelectedIndex;
                        m_VersionHelperTypeName.stringValue = (versionHelperSelectedIndex <= 0 ? null : m_VersionHelperTypeNames[versionHelperSelectedIndex]);
                    }

                    //选择打印辅助器
                    int logHelperSelectedIndex = EditorGUILayout.Popup("Log Helper", m_LogHelperTypeNameIndex, m_LogHelperTypeNames);
                    if (logHelperSelectedIndex != m_LogHelperTypeNameIndex)
                    {
                        m_LogHelperTypeNameIndex = logHelperSelectedIndex;
                        m_LogHelperTypeName.stringValue = (logHelperSelectedIndex <= 0 ? null : m_LogHelperTypeNames[logHelperSelectedIndex]);
                    }

                    //选择压缩辅助器
                    int zipHelperSelectedIndex = EditorGUILayout.Popup("Zip Helper", m_ZipHelperTypeNameIndex, m_ZipHelperTypeNames);
                    if (zipHelperSelectedIndex != m_ZipHelperTypeNameIndex)
                    {
                        m_ZipHelperTypeNameIndex = zipHelperSelectedIndex;
                        m_ZipHelperTypeName.stringValue = (zipHelperSelectedIndex <= 0 ? null : m_ZipHelperTypeNames[zipHelperSelectedIndex]);
                    }

                    //选择Json辅助器
                    int jsonHelperSelectedIndex = EditorGUILayout.Popup("JSON Helper", m_JsonHelperTypeNameIndex, m_JsonHelperTypeNames);
                    if (jsonHelperSelectedIndex != m_JsonHelperTypeNameIndex)
                    {
                        m_JsonHelperTypeNameIndex = jsonHelperSelectedIndex;
                        m_JsonHelperTypeName.stringValue = (jsonHelperSelectedIndex <= 0 ? null : m_JsonHelperTypeNames[jsonHelperSelectedIndex]);
                    }

                    //选择调试辅助器
                    int profilerHelperSelectedIndex = EditorGUILayout.Popup("Profiler Helper", m_ProfilerHelperTypeNameIndex, m_ProfilerHelperTypeNames);
                    if (profilerHelperSelectedIndex != m_ProfilerHelperTypeNameIndex)
                    {
                        m_ProfilerHelperTypeNameIndex = profilerHelperSelectedIndex;
                        m_ProfilerHelperTypeName.stringValue = (profilerHelperSelectedIndex <= 0 ? null : m_ProfilerHelperTypeNames[profilerHelperSelectedIndex]);
                    }
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUI.EndDisabledGroup();

            //游戏帧率
            int frameRate = EditorGUILayout.IntSlider("Frame Rate", m_FrameRate.intValue, 1, 120);
            if (frameRate != m_FrameRate.intValue)
            {
                if (EditorApplication.isPlaying)
                    t.FrameRate = frameRate;    //运行中直接修改脚本的
                else
                    m_FrameRate.intValue = frameRate;
            }

            //游戏速度
            EditorGUILayout.BeginVertical("box");
            {
                float gameSpeed = EditorGUILayout.Slider("Game Speed", m_GameSpeed.floatValue, 0f, 8f);
                int selectedGameSpeed = GUILayout.SelectionGrid(GetSelectedGameSpeed(gameSpeed), GameSpeedTexts, 5);
                if (selectedGameSpeed >= 0)
                {
                    gameSpeed = GetGameSpeed(selectedGameSpeed);
                }
                if (gameSpeed != m_GameSpeed.floatValue)
                {
                    if (EditorApplication.isPlaying)
                        t.GameSpeed = gameSpeed;
                    else
                        m_GameSpeed.floatValue = gameSpeed;
                }
            }
            EditorGUILayout.EndVertical();

            //后台运行
            bool runInBackground = EditorGUILayout.Toggle("Run in Background", m_RunInBackground.boolValue);
            if (runInBackground != m_RunInBackground.boolValue)
            {
                if (EditorApplication.isPlaying)
                    t.IsRunInBackground = runInBackground;
                else
                    m_RunInBackground.boolValue = runInBackground;
            }

            //不休眠
            bool neverSleep = EditorGUILayout.Toggle("Never Sleep", m_NeverSleep.boolValue);
            if (neverSleep != m_NeverSleep.boolValue)
            {
                if (EditorApplication.isPlaying)
                    t.IsNeverSleep = neverSleep;
                else
                    m_NeverSleep.boolValue = neverSleep;
            }

            serializedObject.ApplyModifiedProperties(); //最后一定要应用
        }

        protected override void OnCompileComplete()
        {
            RefreshTypeNames();
        }

        protected override void OnCompileStart()
        {

        }

        private float GetGameSpeed(int selectedGameSpeed)
        {
            if (selectedGameSpeed < 0)
            {
                return GameSpeed[0];
            }

            if (selectedGameSpeed >= GameSpeed.Length)
            {
                return GameSpeed[GameSpeed.Length - 1];
            }

            return GameSpeed[selectedGameSpeed];
        }

        //获取游戏速度值的下标
        private int GetSelectedGameSpeed(float gameSpeed)
        {
            for (int i = 0; i < GameSpeed.Length; i++)
            {
                if (gameSpeed == GameSpeed[i])
                    return i;
            }

            return -1;
        }
    }
}
