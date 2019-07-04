using GameFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Runtime
{
    [CustomEditor(typeof(ReferenceCollector))]
    public class ReferenceCollectorInspector : UnityEditor.Editor
    {
        //关联的ReferenceCollector
        private ReferenceCollector m_ReferenceCollector;

        private SerializedProperty m_ReferenceObjectsProperty = null; //所有引用的列表属性

        private bool m_ConfirmDeleteAll = false;    //删除全部的标志位i

        private void OnEnable()
        {
            m_ReferenceCollector = target as ReferenceCollector;
            m_ReferenceObjectsProperty = serializedObject.FindProperty("m_ReferenceObjects");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            //使ReferenceCollector支持撤销操作，还有Redo，不过没有在这里使用
            Undo.RecordObject(m_ReferenceCollector, "ReferenceCollector Changed");

            //只能在没运行的情况下使用
            EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
            {

            }
            EditorGUI.EndDisabledGroup();
            //水平布局
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("添加引用"))
                {
                    //添加新的元素，具体的函数注释
                    AddReference(m_ReferenceObjectsProperty, Guid.NewGuid().GetHashCode().ToString(), null);
                }

                if (GUILayout.Button("全部删除"))
                {
                    m_ConfirmDeleteAll = EditorUtility.DisplayDialog("警告", "全部删除绑定的对象，这将不可恢复，确认删除吗？", "删除", "取消");
                }
                else if (m_ConfirmDeleteAll)
                {
                    m_ConfirmDeleteAll = false;
                    m_ReferenceObjectsProperty.ClearArray();
                    EditorUtility.SetDirty(m_ReferenceCollector);
                    serializedObject.ApplyModifiedProperties();
                    serializedObject.UpdateIfRequiredOrScript();
                }

                if (GUILayout.Button("排序"))
                {
                    m_ReferenceCollector.SortObjects();
                    EditorUtility.SetDirty(m_ReferenceCollector);
                    serializedObject.ApplyModifiedProperties();
                    serializedObject.UpdateIfRequiredOrScript();
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical();
            {
                EditorGUILayout.LabelField(Utility.Text.Format("当前引用组件数量：{0}", m_ReferenceCollector.Count), EditorStyles.boldLabel);   //数量
                if (m_ReferenceCollector.Count != m_ReferenceObjectsProperty.arraySize)
                    EditorGUILayout.HelpBox("数量与实际对象不符，可能存在重名对象，请检查.", MessageType.Warning);
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical("box");
            {
                EditorGUILayout.LabelField("引用管理:");   //数量
                SerializedProperty property;
                //遍历ReferenceCollector中data list的所有元素，显示在编辑器中
                for (int i = 0; i < m_ReferenceObjectsProperty.arraySize;)
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.Label((i+ 1).ToString("00"));
                        property = m_ReferenceObjectsProperty.GetArrayElementAtIndex(i).FindPropertyRelative("key");
                        string newKey = EditorGUILayout.TextField(property.stringValue, GUILayout.Width(150));  //key值
                        property.stringValue = newKey;
                        property = m_ReferenceObjectsProperty.GetArrayElementAtIndex(i).FindPropertyRelative("obj");
                        Object newObj = EditorGUILayout.ObjectField(property.objectReferenceValue, typeof(GameObject), true);
                        property.objectReferenceValue = newObj;
                        if (GUILayout.Button("X"))
                        {
                            //将元素添加进删除list
                            m_ReferenceObjectsProperty.DeleteArrayElementAtIndex(i);
                            EditorUtility.SetDirty(m_ReferenceCollector);
                            serializedObject.ApplyModifiedProperties();
                            serializedObject.UpdateIfRequiredOrScript();
                        }
                        else
                        {
                            i++;
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
            serializedObject.UpdateIfRequiredOrScript();
        }

        //添加元素，具体知识点在ReferenceCollector中说了
        private void AddReference(SerializedProperty dataProperty, string key, GameObject obj)
        {
            int index = dataProperty.arraySize;
            dataProperty.InsertArrayElementAtIndex(index);
            var element = dataProperty.GetArrayElementAtIndex(index);
            element.FindPropertyRelative("key").stringValue = key;
            element.FindPropertyRelative("obj").objectReferenceValue = obj;
            EditorUtility.SetDirty(m_ReferenceCollector);
            serializedObject.ApplyModifiedProperties();
            serializedObject.UpdateIfRequiredOrScript();
        }

    }
}
