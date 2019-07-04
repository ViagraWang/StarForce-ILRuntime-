using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;

namespace Game.Editor
{
    public class RenameAssetsTool : EditorWindow
    {

        public static void CreateWindow()
        {
            //Rect rect = new Rect(400, 400, 300, 400);
            //AudioWindowEditor window = EditorWindow.GetWindowWithRect<AudioWindowEditor>(rect); //创建一个窗口
            RenameAssetsTool window = GetWindow<RenameAssetsTool>("批量重命名");    //创建窗口
            window.Show();

        }

        private string folderPath;  //文件夹路径
        private string MainName;    //保存输入的文字
        private string ExtendsName;   //保存的路径
        private string start;
        private int startNum;

        void OnGUI()
        {
            Object[] Directorys = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);  //选择的文件夹对象
            if (Directorys.Length == 0)
            {
                EditorGUILayout.LabelField("请先选择一个文件夹！");
                return;
            }
            else if (Directorys.Length > 1)
            {
                EditorGUILayout.LabelField("只能选择一个文件夹！");
                return;
            }

            string stringPath = AssetDatabase.GetAssetPath(Directorys[0]);
            if (!Directory.Exists(stringPath))
            {
                EditorGUILayout.LabelField("只能选择一个文件夹！");
                return;
            }

            EditorGUILayout.LabelField("选择的文件夹路径为：" + stringPath);
            MainName = EditorGUILayout.TextField("输入主名：", MainName);
            ExtendsName = EditorGUILayout.TextField("输入后缀格式(例如:0000)：", ExtendsName);
            start = EditorGUILayout.TextField("输入起始数字：", start);
            if (!int.TryParse(start, out startNum))
            {
                EditorGUILayout.LabelField("输入的起始数字必须为整数！");
                return;
            }

            if (GUILayout.Button("确认修改"))
            {
                DirectoryInfo dir = new DirectoryInfo(stringPath);  //目录
                FileInfo[] files = dir.GetFiles("*", SearchOption.AllDirectories);  //获取所有的文件信息
                int count = files.Length / 2;
                for (int i = 0; i < files.Length; i++)
                {
                    FileInfo fileInfo = files[i];
                    if (!fileInfo.Name.EndsWith(".meta"))
                    {
                        EditorUtility.DisplayProgressBar("修改文件名称", "正在修改" + (i + 1) / 2 + "/" + count + "个文件名称...", (i + 1) / 2 / (float)count);
                        string newName = MainName + startNum.ToString(ExtendsName); //新名称

                        string basePath = "Assets" + fileInfo.FullName.Substring(Application.dataPath.Length);  //相对路径
                        basePath = basePath.Replace('\\', '/');
                        if (!string.IsNullOrEmpty(AssetDatabase.RenameAsset(basePath, newName)))
                        {
                            Debug.LogWarning("名称修改失败：" + basePath);
                        }
                        startNum++;
                    }
                }

                EditorUtility.ClearProgressBar();
                AssetDatabase.Refresh();    //刷新编辑器
                Debug.Log("名称修改成功！");
            }

        }

    }
}