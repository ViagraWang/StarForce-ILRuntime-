using GameFramework.Procedure;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityGameFrame.Runtime;

namespace UnityGameFrame.Editor
{
    [CustomEditor(typeof(ProcedureComponent))]
    internal sealed class ProcedureComponentInspector : GameFrameworkInspector
    {
        private SerializedProperty m_AvailableProcedureTypeNames = null;    //存储在组件中的流程类型名称
        private SerializedProperty m_EntranceProcedureTypeName = null;  //入口流程类型名


        private string[] m_ProcedureTypeNames = null;    //获取所有的流程类型名称
        private List<string> m_CurrentAvailableProcedureTypeNames = new List<string>();   //当前勾选的流程类型名称
        private int m_EntranceProcedureIndex = -1;  //选中的入口流程名称下标

        private void OnEnable()
        {
            m_AvailableProcedureTypeNames = serializedObject.FindProperty("m_AvailableProcedureTypeNames");
            m_EntranceProcedureTypeName = serializedObject.FindProperty("m_EntranceProcedureTypeName");

            RefreshTypeNames();
        }

        //刷新
        private void RefreshTypeNames()
        {
            m_ProcedureTypeNames = Type.GetRunSubClassNames(typeof(ProcedureBase));
            ReadAvailableProcedureTypeNames();
            int oldCount = m_CurrentAvailableProcedureTypeNames.Count;  //之前保存的数量
            m_CurrentAvailableProcedureTypeNames = m_CurrentAvailableProcedureTypeNames.Where(x => m_ProcedureTypeNames.Contains(x)).ToList();
            if (m_CurrentAvailableProcedureTypeNames.Count != oldCount) //数量不相等，说明删除了一部分流程
            {
                WriteAvailableProcedureTypeNames();
            }
            else if (!string.IsNullOrEmpty(m_EntranceProcedureTypeName.stringValue))    //入口流程类名不为空
            {
                m_EntranceProcedureIndex = m_CurrentAvailableProcedureTypeNames.IndexOf(m_EntranceProcedureTypeName.stringValue);
                if (m_EntranceProcedureIndex < 0)
                {
                    m_EntranceProcedureTypeName.stringValue = null;
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        //读取保存的可用的流程类型名称
        private void ReadAvailableProcedureTypeNames()
        {
            m_CurrentAvailableProcedureTypeNames.Clear();
            int count = m_AvailableProcedureTypeNames.arraySize;
            for (int i = 0; i < count; i++)
            {
                m_CurrentAvailableProcedureTypeNames.Add(m_AvailableProcedureTypeNames.GetArrayElementAtIndex(i).stringValue);
            }
        }

        //写入可用流程类型名称
        private void WriteAvailableProcedureTypeNames()
        {
            m_AvailableProcedureTypeNames.ClearArray(); //清空保存的
            if (m_CurrentAvailableProcedureTypeNames.Count == 0)
                return;

            m_CurrentAvailableProcedureTypeNames.Sort();
            for (int i = 0; i < m_CurrentAvailableProcedureTypeNames.Count; i++)
            {
                //保存到组件中
                m_AvailableProcedureTypeNames.InsertArrayElementAtIndex(i);
                m_AvailableProcedureTypeNames.GetArrayElementAtIndex(i).stringValue = m_CurrentAvailableProcedureTypeNames[i];
            }

            if (!string.IsNullOrEmpty(m_EntranceProcedureTypeName.stringValue))
            {
                m_EntranceProcedureIndex = m_CurrentAvailableProcedureTypeNames.IndexOf(m_EntranceProcedureTypeName.stringValue);
                if (m_EntranceProcedureIndex < 0)
                {
                    m_EntranceProcedureTypeName.stringValue = null;
                }
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            ProcedureComponent t = target as ProcedureComponent;

            if (string.IsNullOrEmpty(m_EntranceProcedureTypeName.stringValue))  //保存的入口流程名为空
            {
                EditorGUILayout.HelpBox("Entrance procedure is invalid.", MessageType.Error);
            }
            else if (EditorApplication.isPlaying)
            {
                //运行中显示显示当前流程名称
                EditorGUILayout.LabelField("Current Procedure", t.CurrentProcedure == null ? "None" : t.CurrentProcedure.GetType().ToString());
            }

            EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
            {
                GUILayout.Label("Available Procedures", EditorStyles.boldLabel);    //可用流程列表
                if(m_ProcedureTypeNames.Length > 0)
                {
                    EditorGUILayout.BeginVertical("box");
                    {
                        for (int i = 0; i < m_ProcedureTypeNames.Length; i++)
                        {
                            string procedureTypeName = m_ProcedureTypeNames[i];
                            bool selected = m_CurrentAvailableProcedureTypeNames.Contains(procedureTypeName);
                            if(selected != EditorGUILayout.ToggleLeft(procedureTypeName, selected)) //复选框
                            {
                                if (!selected)
                                {
                                    //之前未选中，当前选中
                                    m_CurrentAvailableProcedureTypeNames.Add(procedureTypeName);
                                    WriteAvailableProcedureTypeNames();
                                }
                                else if(procedureTypeName != m_EntranceProcedureTypeName.stringValue)
                                {
                                    //如果取消勾选的不是入口流程名，才可以移除；否则不能取消勾选
                                    m_CurrentAvailableProcedureTypeNames.Remove(procedureTypeName);
                                    WriteAvailableProcedureTypeNames();
                                }
                            }
                        }
                    }
                    EditorGUILayout.EndVertical();
                }
                else
                {
                    EditorGUILayout.HelpBox("There is no available procedure.", MessageType.Info);
                }

                //选择流程入口下拉框
                if(m_CurrentAvailableProcedureTypeNames.Count > 0)
                {
                    EditorGUILayout.Separator();
                    int selectedIndex = EditorGUILayout.Popup("Entrance Procedure", m_EntranceProcedureIndex, m_CurrentAvailableProcedureTypeNames.ToArray());
                    if(selectedIndex != m_EntranceProcedureIndex)   //不等于之前选中的下标
                    {
                        m_EntranceProcedureIndex = selectedIndex;
                        m_EntranceProcedureTypeName.stringValue = m_CurrentAvailableProcedureTypeNames[m_EntranceProcedureIndex];
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("Select available procedures first.", MessageType.Info);
                }
            }
            EditorGUI.EndDisabledGroup();

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
