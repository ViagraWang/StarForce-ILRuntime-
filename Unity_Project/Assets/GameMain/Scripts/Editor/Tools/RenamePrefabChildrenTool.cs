using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;

namespace Game.Editor
{
    public class RenamePrefabChildrenTool : EditorWindow
    {

        public static void CreateWindow()
        {
            RenamePrefabChildrenTool window = GetWindow<RenamePrefabChildrenTool>("批量重命名子对象");    //创建窗口
            window.Show();
        }

        private string MainName;    //保存输入的文字
        private bool isAddNum = false;  //是否添加序号
        private string start;
        private int startNum;

        void OnGUI()
        {
            Transform parent = Selection.activeTransform;
            if (parent == null)
            {
                EditorGUILayout.LabelField("请在Hierarchy面板中选择父对象，重命名子对象");
                return;
            }

            MainName = EditorGUILayout.TextField("输入名称：", MainName);
            isAddNum = EditorGUILayout.Toggle("是否在添加序号", isAddNum);
            if (isAddNum)
            {
                start = EditorGUILayout.TextField("输入起始序号：", start);
                if (!int.TryParse(start, out startNum))
                {
                    EditorGUILayout.LabelField("输入的起始序号必须为整数！");
                    return;
                }
            }

            if (GUILayout.Button("确认修改"))
            {
                int count = parent.childCount;
                for (int i = 0; i < count; i++)
                {
                    EditorUtility.DisplayProgressBar("修改子对象名称", "正在修改" + (i + 1) + "/" + count + "个文件名称...", (i + 1) / (float)count);
                    if (isAddNum)
                    {
                        parent.GetChild(i).name = MainName + startNum.ToString();
                        startNum++;
                    }
                    else
                    {
                        parent.GetChild(i).name = MainName;
                    }
                }

                EditorUtility.ClearProgressBar();
                AssetDatabase.Refresh();    //刷新编辑器
                Debug.Log("名称修改成功！");
            }

        }
    }
}
