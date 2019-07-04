using GameFramework;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace UnityGameFrame.Editor
{
    //辅助器信息
    internal sealed class HelperInfo<T> where T : MonoBehaviour
    {
        private const string CustomOptionName = "<Custom>";

        private readonly string m_Name; //辅助器名字

        private SerializedProperty m_HelperTypeName;    //辅助器类型名
        private SerializedProperty m_CustomHelper;  //自定义辅助器
        private string[] m_HelperTypeNames;
        private int m_HelperTypeNameIndex;

        public HelperInfo(string name)
        {
            m_Name = name;

            m_HelperTypeName = null;
            m_CustomHelper = null;
            m_HelperTypeNames = null;
            m_HelperTypeNameIndex = 0;
        }

        public void Init(SerializedObject serializedObject)
        {
            m_HelperTypeName = serializedObject.FindProperty(Utility.Text.Format("m_{0}HelperTypeName", m_Name));
            m_CustomHelper = serializedObject.FindProperty(Utility.Text.Format("m_Custom{0}Helper", m_Name));
        }

        //绘制辅助器列表
        public void Draw()
        {
            string displayName = FieldNameForDisplay(m_Name);
            int selectedIndex = EditorGUILayout.Popup(Utility.Text.Format("{0} Helper", displayName), m_HelperTypeNameIndex, m_HelperTypeNames);
            if (selectedIndex != m_HelperTypeNameIndex)
            {
                m_HelperTypeNameIndex = selectedIndex;
                m_HelperTypeName.stringValue = (selectedIndex <= 0 ? null : m_HelperTypeNames[selectedIndex]);
            }

            if (m_HelperTypeNameIndex <= 0)
            {
                EditorGUILayout.PropertyField(m_CustomHelper);
                if (m_CustomHelper.objectReferenceValue == null)
                {
                    EditorGUILayout.HelpBox(Utility.Text.Format("You must set Custom {0} Helper.", displayName), MessageType.Error);
                }
            }
        }

        //刷新操作，获取T类型的类
        public void Refresh()
        {
            List<string> helperTypeNameList = new List<string>();
            helperTypeNameList.Add(CustomOptionName);
            helperTypeNameList.AddRange(Type.GetRunSubClassNames(typeof(T)));
            m_HelperTypeNames = helperTypeNameList.ToArray();

            m_HelperTypeNameIndex = 0;
            if (!string.IsNullOrEmpty(m_HelperTypeName.stringValue))
            {
                m_HelperTypeNameIndex = helperTypeNameList.IndexOf(m_HelperTypeName.stringValue);
                if (m_HelperTypeNameIndex <= 0)
                {
                    m_HelperTypeNameIndex = 0;
                    m_HelperTypeName.stringValue = null;
                }
            }
        }

        private string FieldNameForDisplay(string fieldName)
        {
            if (string.IsNullOrEmpty(fieldName))
            {
                return string.Empty;
            }

            string str = Regex.Replace(fieldName, @"^m_", string.Empty);
            str = Regex.Replace(str, @"((?<=[a-z])[A-Z]|[A-Z](?=[a-z]))", @" $1").TrimStart();
            return str;
        }
    }
}
