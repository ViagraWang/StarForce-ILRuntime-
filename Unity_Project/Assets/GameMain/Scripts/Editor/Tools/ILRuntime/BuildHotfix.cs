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
	
        //����bytes�ļ�
	    public static void BuildBytes()
	    {
            File.Copy(Utility.Path.GetCombinePath(ScriptAssembliesDir, HotfixDll), Utility.Path.GetCombinePath(RuntimeAssetUtility.HotfixPath, RuntimeAssetUtility.HotfixDllName), true);
            File.Copy(Utility.Path.GetCombinePath(ScriptAssembliesDir, HotfixPdb), Utility.Path.GetCombinePath(RuntimeAssetUtility.HotfixPath, RuntimeAssetUtility.HotfixPdbName), true);
            Debug.Log($"����Hotfix.dll, Hotfix.pdb��{RuntimeAssetUtility.HotfixPath}���");
            AssetDatabase.Refresh();
        }
	}
}
