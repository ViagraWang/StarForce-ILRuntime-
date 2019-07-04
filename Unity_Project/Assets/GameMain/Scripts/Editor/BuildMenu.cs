using GameFramework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class BuildMenu
{
    private static string BuildPath = Application.dataPath + "/../../Build";
    private static string WebGLDir = "Web";
    private static string AndroidDir = "Android";

    //全部打包
    [MenuItem("Build/All")]
    public static void BuildAll()
    {
        ClearBuildInfo();
        BuildWebGL();
        BuildAndroid();
    }

    [MenuItem("Build/WebGL")]
    public static void BuildWebGL()
    {
        string productName = Utility.Text.Format("{0}\\{1}_{2}", WebGLDir, Application.productName, DateTime.Now.ToString("yyyyMMddHHmm"));
        CreateBuildText(productName);
        string path = Utility.Text.Format("{0}/{1}", BuildPath, productName).Replace("\\","/");
        BuildPipeline.BuildPlayer(FindEnableEditorrScenes(), path, BuildTarget.WebGL, BuildOptions.None);
    }

    private static void ClearBuildInfo()
    {
        FileInfo fileInfo = new FileInfo(Utility.Text.Format("{0}/BuildInfo.txt", BuildPath));
        if (!Directory.Exists(fileInfo.Directory.FullName))
            Directory.CreateDirectory(fileInfo.Directory.FullName);
        if (fileInfo.Exists)
            fileInfo.Delete();
        var stream = fileInfo.CreateText();
        stream.Close();
        stream.Dispose();
    }

    //创建打包内容
    private static void CreateBuildText(string productName)
    {
        using(FileStream fs = new FileStream(Utility.Text.Format("{0}/BuildInfo.txt", BuildPath), FileMode.Append, FileAccess.Write))
        {
            using(StreamWriter sw = new StreamWriter(fs, Encoding.Default))
            {
                sw.WriteLine(productName);
            }
        }
    }

    private static string[] FindEnableEditorrScenes()
    {
        List<string> editorScenes = new List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (!scene.enabled) continue;
            editorScenes.Add(scene.path);
        }
        return editorScenes.ToArray();
    }


    #region 打包Android设置

    [MenuItem("Build/Android")]
    public static void BuildAndroid()
    {
        //Android配置
        PlayerSettings.Android.keystoreName = Application.dataPath.Replace("/Assets", "") + "/user.keystore";
        PlayerSettings.Android.keyaliasName = "cc";
        PlayerSettings.Android.keystorePass = "12301230";
        PlayerSettings.Android.keyaliasPass = "12301230";

        BuildSetting buildSetting = GetAndoridBuildSetting();
        string suffix = SetAndroidSetting(buildSetting);

        string productName = Utility.Text.Format("{0}\\{1}_{2}_{3}.apk", AndroidDir, PlayerSettings.productName, suffix, DateTime.Now.ToString("yyyyMMddHHmm"));
        CreateBuildText(productName);
        string path = Utility.Text.Format("{0}/{1}", BuildPath, productName).Replace("\\", "/");

        BuildPipeline.BuildPlayer(FindEnableEditorrScenes(), path, BuildTarget.Android, BuildOptions.None);
    }

    static BuildSetting GetAndoridBuildSetting()
    {
        string[] parameters = Environment.GetCommandLineArgs(); //获取控制台指令
        BuildSetting buildSetting = new BuildSetting();
        foreach (string str in parameters)
        {
            if (str.StartsWith("Place"))
            {
                var tempParam = str.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (tempParam.Length == 2)
                {
                    buildSetting.Place = (Place)Enum.Parse(typeof(Place), tempParam[1], true);
                }
            }
            else if (str.StartsWith("Version"))
            {
                var tempParam = str.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (tempParam.Length == 2)
                {
                    buildSetting.Version = tempParam[1].Trim();
                }
            }
            else if (str.StartsWith("Build"))
            {
                var tempParam = str.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (tempParam.Length == 2)
                {
                    buildSetting.Build = tempParam[1].Trim();
                }
            }
            else if (str.StartsWith("Name"))
            {
                var tempParam = str.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (tempParam.Length == 2)
                {
                    buildSetting.Name = tempParam[1].Trim();
                }
            }
            else if (str.StartsWith("Debug"))
            {
                var tempParam = str.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (tempParam.Length == 2)
                {
                    bool.TryParse(tempParam[1], out buildSetting.Debug);
                }
            }
            else if (str.StartsWith("MulRendering"))
            {
                var tempParam = str.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (tempParam.Length == 2)
                {
                    bool.TryParse(tempParam[1], out buildSetting.MulRendering);
                }
            }
            else if (str.StartsWith("IL2CPP"))
            {
                var tempParam = str.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (tempParam.Length == 2)
                {
                    bool.TryParse(tempParam[1], out buildSetting.IL2CPP);
                }
            }
        }
        return buildSetting;
    }

    static string SetAndroidSetting(BuildSetting setting)
    {
        string suffix = "";
        if (setting.Place != Place.None)
        {
            //代表了渠道包
            string symbol = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android); //获取已存在的宏
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, symbol + ";" + setting.Place.ToString());    //添加新的宏
            suffix += setting.Place.ToString();
        }

        if (!string.IsNullOrEmpty(setting.Version))
        {
            PlayerSettings.bundleVersion = setting.Version;
            suffix += setting.Version;
        }
        if (!string.IsNullOrEmpty(setting.Build))
        {
            PlayerSettings.Android.bundleVersionCode = int.Parse(setting.Build);
            suffix += "_" + setting.Build;
        }
        if (!string.IsNullOrEmpty(setting.Name))
        {
            PlayerSettings.productName = setting.Name;
            //PlayerSettings.applicationIdentifier = "com.TTT." + setting.Name;
        }

        if (setting.MulRendering)
        {
            PlayerSettings.MTRendering = true;
            suffix += "_MTR";   //多线程
        }
        else
        {
            PlayerSettings.MTRendering = false;
        }

        if (setting.IL2CPP)
        {
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
            suffix += "_IL2CPP";
        }
        else
        {
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.Mono2x);
        }

        if (setting.Debug)
        {
            EditorUserBuildSettings.development = true;
            EditorUserBuildSettings.connectProfiler = true;
            suffix += "_Debug";
        }
        else
        {
            EditorUserBuildSettings.development = false;
        }
        return suffix;
    }
    #endregion



}

//构建设置
public class BuildSetting
{
    //版本号
    public string Version = "";
    //build次数
    public string Build = "";
    //程序名称
    public string Name = "";
    //是否debug
    public bool Debug = false;
    //渠道
    public Place Place = Place.None;
    //多线程渲染
    public bool MulRendering = false;
    //是否IL2CPP
    public bool IL2CPP = false;
    //是否开启动态合批
    public bool DynamicBatching = false;
}

//渠道包
public enum Place
{
    None = 0,
    Xiaomi,
    Bilibili,
    Huawei,
    Meizu,
    Weixin,
}