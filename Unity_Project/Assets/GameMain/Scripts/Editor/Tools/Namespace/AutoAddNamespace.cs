//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;
//using System.IO;
//using System.Text.RegularExpressions;

//public class AutoAddNamespace : UnityEditor.AssetModificationProcessor
//{
//    private const string spaceName = "Game.Hotfix";  //命名空间

//    private static readonly string AuthorCode =
//    "//=======================================================\r\n"
//    + "// 作者：\r\n"
//    + "// 描述：\r\n"
//    + "//=======================================================\r\n";

//    public static void OnWillCreateAsset(string path)
//    {
//        //只修改C#脚本
//        path = path.Replace(".meta", "");
//        if (path.EndsWith(".cs"))
//        {
//            string contents = File.ReadAllText(path);
//            string res = NamespaceBuilder.AddNameSpace(contents, spaceName);
//            res = AuthorCode + res;
//            File.WriteAllText(path, res);
//            AssetDatabase.Refresh();
//        }
//    }

//    //首字母改成大写
//    public static string FirstLetterUppercase(string str)
//    {
//        if (string.IsNullOrEmpty(str))
//            return str;
//        if (str.Length == 1)
//            return str.ToUpper();

//        var first = str[0];
//        var rest = str.Substring(1);
//        return first.ToString().ToUpper() + rest;
//    }

//    //获取unity自动创建的脚本类名
//    public static string GetClassName(string allText)
//    {
//        var patterm = "public class ([A-Za-z0-9_]+)\\s*:\\s*MonoBehaviour";
//        var match = Regex.Match(allText, patterm);
//        if (match.Success)
//        {
//            return match.Groups[1].Value;
//        }
//        return string.Empty;
//    }

//}
