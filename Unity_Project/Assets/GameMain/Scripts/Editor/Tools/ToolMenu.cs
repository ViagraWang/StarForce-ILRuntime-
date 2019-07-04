using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityGameFrame.Editor;

namespace Game.Editor
{
    //工具菜单
    public sealed class ToolMenu
    {
        private const string menuName = "Tools/";
        private static string[] HotfixTypes = { "ILRuntime", "Reflect", "Internal" };  //脚本热更新的类型

        //创建热更新程序文件
        [MenuItem(menuName + "Build Hotfix Bytes", false, 0)]
        private static void BuildHotfixBytes()
        {
            BuildHotfix.BuildBytes();
        }

        //生成自动绑定脚本
        [MenuItem(menuName + "ILRuntime/Generate CLR Binding Code by Analysis", false, 51)]
        private static void GenerateCLRBindingByAnalysis()
        {
            ILRuntimeCLRBinding.GenerateCLRBindingByAnalysis();
        }

        //设置热更新的类型ILRuntime
        [MenuItem(menuName + "Set Hotfix Type/ILRuntime", false, 52)]
        private static void SetHotfixTypeILRuntime()
        {
            for (int i = 0; i < HotfixTypes.Length; i++)
            {
                if (i == 0)
                    ScriptingDefineSymbols.AddScriptingDefineSymbol(HotfixTypes[i]);
                else
                    ScriptingDefineSymbols.RemoveScriptingDefineSymbol(HotfixTypes[i]);
            }
        }

        //设置热更新的类型MonoReflect
        [MenuItem(menuName + "Set Hotfix Type/Reflect", false, 53)]
        private static void SetHotfixTypeReflect()
        {
            for (int i = 0; i < HotfixTypes.Length; i++)
            {
                if (i == 1)
                    ScriptingDefineSymbols.AddScriptingDefineSymbol(HotfixTypes[i]);
                else
                    ScriptingDefineSymbols.RemoveScriptingDefineSymbol(HotfixTypes[i]);
            }
        }

        //设置热更新的类型Internal
        [MenuItem(menuName + "Set Hotfix Type/Internal", false, 54)]
        private static void SetHotfixTypeInternal()
        {
            for (int i = 0; i < HotfixTypes.Length; i++)
            {
                if (i == 2)
                    ScriptingDefineSymbols.AddScriptingDefineSymbol(HotfixTypes[i]);
                else
                    ScriptingDefineSymbols.RemoveScriptingDefineSymbol(HotfixTypes[i]);
            }
        }

        //添加命名空间
        [MenuItem(menuName + "Add Namespace", false, 101)]
        private static void CreateNameSpace()
        {
            AddNamespaceTool.CreateWizard();
        }

        //重命名文件夹中的资源名称
        [MenuItem(menuName + "ReName objects in Folder", false, 102)]
        private static void RenameAssets()
        {
            RenameAssetsTool.CreateWindow();
        }

        //重命名子对象的名称
        [MenuItem(menuName + "ReName Children In Hierarchy", false, 103)]
        private static void RenamePrefabChild()
        {
            RenamePrefabChildrenTool.CreateWindow();
        }

        //打开热更新的工程
        [MenuItem(menuName + "Open Hotfix C# Project", false, 400)]
        private static void OpenHitfixProject()
        {
            string path = Application.dataPath + "/../Game.Hotfix/Game.Hotfix.sln";
            if (!File.Exists(path))
            {
                UnityEngine.Debug.Log("路径不存在 -> " + path);
                return;
            }
            Process.Start(path);
        }

    }
}
