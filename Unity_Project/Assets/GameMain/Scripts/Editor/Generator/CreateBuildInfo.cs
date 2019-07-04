using Game.Runtime;
using GameFramework;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityGameFrame.Runtime;

namespace Game.Editor
{	
	//创建版本信息
	public class CreateBuildInfo : EditorWindow
	{
	    private const string CheckVersionUrl = "http://111.231.54.50:8080/Downlist/GameAssets/";    //版本列表更新url
	    private static string s_BuildInfoFileName = "BuildInfo.txt";
	    private static string s_BuildInfoPath = Utility.Path.GetCombinePath(Application.dataPath, GameFrameworkConfigs.s_ConfigFolderPath, s_BuildInfoFileName);
	
	    public static void Create()
	    {
	        Rect rect = new Rect(400, 400, 500, 500);
	        CreateBuildInfo window = GetWindowWithRect<CreateBuildInfo>(rect, true, "创建版本信息"); //创建一个窗口
	        //Tool_ReName window = GetWindow<Tool_ReName>("批量重命名");    //创建窗口
	        window.Show();
	    }
	
	    private string gameVersionId = "0";  //版本号
	    private int internalGameVersion = 1;    //内置版本号
	    private string checkVersionUrl = CheckVersionUrl;   //版本列表更新网址
	    private string standaloneAppUrl = "";   //独立应用url
	    private string iosAppUrl = "";   //ios应用url
	    private string androidAppUrl = "";   //Android应用url
	    private string endOfJson = "";   //结束语
	
	    void OnGUI()
	    {
	        gameVersionId = EditorGUILayout.TextField("GameVersion(游戏版本号)：", gameVersionId);
	        internalGameVersion = EditorGUILayout.IntField("InternalGameVersion(内置游戏版本号)：", internalGameVersion);
	        checkVersionUrl = EditorGUILayout.TextField("CheckVersionUrl(检查版本号的URL)：", checkVersionUrl);
	        standaloneAppUrl = EditorGUILayout.TextField("StandaloneAppUrl(独立应用的URL)：", standaloneAppUrl);
	        iosAppUrl = EditorGUILayout.TextField("IosAppUrl(苹果应用的URL)：", iosAppUrl);
	        androidAppUrl = EditorGUILayout.TextField("AndroidAppUrl(安卓应用的URL)：", androidAppUrl);
	        endOfJson = EditorGUILayout.TextField("END_OF_JSON(结束语)：", endOfJson);
	
	        if (GUILayout.Button("创建BuildInfo文件"))
	        {
	            BuildInfo info = new BuildInfo(gameVersionId, internalGameVersion, checkVersionUrl, standaloneAppUrl, iosAppUrl, androidAppUrl, endOfJson);
	            Utility.Json.SetJsonHelper(new DefaultJsonHelper());    //设置默认的Json辅助器
	
	            string json = Utility.Json.ToJson(info);
	
	            string directory = Path.GetDirectoryName(s_BuildInfoPath);
	            if (!Directory.Exists(directory))
	                Directory.CreateDirectory(directory);
	
            using (FileStream fs = new FileStream(s_BuildInfoPath, FileMode.Create, FileAccess.Write))
	            {
                using (StreamWriter sw = new StreamWriter(fs))
	                {
	                    sw.Write(json);
	                }
	            }
	            AssetDatabase.Refresh();    //刷新编辑器
	            AssetDatabase.SaveAssets();
	            Debug.Log("成功创建版本信息文件=>" + s_BuildInfoPath);
	        }
	    }
	}
}
