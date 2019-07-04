using GameFramework;
using Game.Runtime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Game.Editor
{	
	public class BuildHotfix
	{
	    private const string ScriptAssembliesDir = "Library/ScriptAssemblies";
	    private const string HotfixDll = "Game.Hotfix.dll";
	    private const string HotfixPdb = "Game.Hotfix.pdb";
	
        //生成bytes文件
	    public static void BuildBytes()
	    {
            File.Copy(Utility.Path.GetCombinePath(ScriptAssembliesDir, HotfixDll), Utility.Path.GetCombinePath(RuntimeAssetUtility.HotfixPath, RuntimeAssetUtility.HotfixDllName), true);
            File.Copy(Utility.Path.GetCombinePath(ScriptAssembliesDir, HotfixPdb), Utility.Path.GetCombinePath(RuntimeAssetUtility.HotfixPath, RuntimeAssetUtility.HotfixPdbName), true);
            Debug.Log($"复制Hotfix.dll, Hotfix.pdb到{RuntimeAssetUtility.HotfixPath}完成");
            AssetDatabase.Refresh();
        }
	}
}
