//=======================================================
// 作者：
// 描述：
//=======================================================
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Editor
{	
	public class AutoCreatEditor {
	
        [MenuItem("Assets/Create/Hotfix C# Scripts", false, 70)]
	    public static void CreateHotfixCS()
	    {
	        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<CreateHotfixScriptAsset>(), GetSelectedPath() + "/NewHotfixScript.cs", GetDefaultIcon(".cs"), "Assets/GameMain/Scripts/Editor/CreateHotfixCs/NewCSClass.cs");
	    }

        [MenuItem("Assets/Create/Runtime C# Scripts", false, 71)]
        public static void CreateRuntimeCS()
        {
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<CreateRuntimeScriptAsset>(), GetSelectedPath() + "/NewRuntimeScript.cs", GetDefaultIcon(".cs"), "Assets/GameMain/Scripts/Editor/CreateHotfixCs/NewCSClass.cs");
        }

        //获取当前选择的路径
        public static string GetSelectedPath()
	    {
	        string path = "Assets";
	        foreach (Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
	        {
	            path = AssetDatabase.GetAssetPath(obj);
	            if(!string.IsNullOrEmpty(path) && File.Exists(path))
	            {
	                path = Path.GetDirectoryName(path);
	                break;
	            }
	        }
	        return path;
	    }
	
	    //通过扩展名获取默认图标
	    private static Texture2D GetDefaultIcon(string extension)
	    {
	        switch (extension)
	        {
	            case ".cs":
	                return EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D;
	            case ".shader":
	                return EditorGUIUtility.IconContent("Shader Icon").image as Texture2D;
	        }
	        return null;
	    }
	
        public static Object CreateScriptAssetFromTemplate(string pathName, string resourcesFile, bool hotfix)
        {
            string fullPath = Path.GetFullPath(pathName);
            string text = "";
            using (StreamReader sr = new StreamReader(resourcesFile))
            {
                text = sr.ReadToEnd();
            }

            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(pathName);   //无扩展名
            text = Regex.Replace(text, "NameSpaceName", hotfix ? "Game.Hotfix" : "Game.Runtime");   //命名空间
            text = Regex.Replace(text, "NewCSClass", hotfix ? fileNameWithoutExtension : fileNameWithoutExtension + " : MonoBehaviour"); //类名

            UTF8Encoding encoding = new UTF8Encoding(true, false);
            bool append = false;
            using (StreamWriter sw = new StreamWriter(fullPath, append, encoding))
            {
                sw.Write(text);
            }

            AssetDatabase.ImportAsset(pathName);
            return AssetDatabase.LoadAssetAtPath(pathName, typeof(Object));
        }


    }
	
	//动态创建Hotfix脚本
	class CreateHotfixScriptAsset : EndNameEditAction
	{
	    public override void Action(int instanceId, string pathName, string resourceFile)
	    {
	        Object obj = AutoCreatEditor.CreateScriptAssetFromTemplate(pathName, resourceFile, true);
	        ProjectWindowUtil.ShowCreatedAsset(obj);
	    }
	}

    //动态创建Runtime脚本
    class CreateRuntimeScriptAsset : EndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            Object obj = AutoCreatEditor.CreateScriptAssetFromTemplate(pathName, resourceFile, false);
            ProjectWindowUtil.ShowCreatedAsset(obj);
        }
    }


}