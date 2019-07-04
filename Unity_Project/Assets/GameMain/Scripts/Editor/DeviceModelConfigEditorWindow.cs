using Game.Runtime;
using GameFramework;
using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Game.Editor
{	
	public class DeviceModelConfigEditorWindow : EditorWindow {
	
	    private const string CreateModelConfig = "GameMain/Create Device Mode Config";
	    private const string EditorModelConfig = "GameMain/Device Model Config Editor";
	
	    private static string s_ConfigPathName = "DeviceModelConfig.asset";
	    private static string s_ConfigFullPath = Utility.Path.GetCombinePath(Application.dataPath, GameFrameworkConfigs.s_ConfigFolderPath, s_ConfigPathName);
	
	    private DeviceModelConfig m_Config = null;
	
	    //[MenuItem(EditorModelConfig, false)]
	    public static void EditorDeviceModelConfig()
	    {
	        OpenWindow(AssetDatabase.LoadAssetAtPath<DeviceModelConfig>(Utility.Path.GetCombinePath("Assets/", GameFrameworkConfigs.s_ConfigFolderPath, s_ConfigPathName)));
	    }
	
	    //[MenuItem(EditorModelConfig, true)]
	    public static bool EditorDeviceModelConfigValidate()
	    {
	        bool isHas = File.Exists(s_ConfigFullPath);
	        Menu.SetChecked(EditorModelConfig, !isHas);
	        return isHas;
	    }
	
	    //[MenuItem(CreateModelConfig, false)]
	    public static void CreateDeviceModelConfig()
	    {
	        //目录
	        string directory = Path.GetDirectoryName(s_ConfigFullPath);
	        if (!Directory.Exists(directory))
	        {
	            Directory.CreateDirectory(directory);
	            AssetDatabase.Refresh();    //刷新
	        }
	
	        //创建
	        DeviceModelConfig config = CreateInstance<DeviceModelConfig>();
	        AssetDatabase.CreateAsset(config, "Assets/" + s_ConfigPathName);
	        AssetDatabase.SaveAssets();
	        AssetDatabase.Refresh();
	        Debug.Log("成功创建设备模型配置");
	    }
	
	    //[MenuItem(CreateModelConfig, true)]
	    public static bool CreateDeviceModelConfigValidate()
	    {
	        bool isHas = File.Exists(s_ConfigFullPath);
	        Menu.SetChecked(CreateModelConfig, isHas);
	        return !isHas;
	    }
	
	    //打开窗口
	    public static void OpenWindow(DeviceModelConfig deviceModelConfig)
	    {
	        if(deviceModelConfig == null)
	        {
	            Debug.LogWarning("Device Model Config is invalid.");
	            return;
	        }
	
	        DeviceModelConfigEditorWindow window = GetWindow<DeviceModelConfigEditorWindow>(true, "Device Model Config Editor");
	        window.m_Config = deviceModelConfig;
	        window.minSize = new Vector2(460f, 400f);
	    }
	
	    private void OnGUI()
	    {
	        if (m_Config == null)
	            return;
	
	        OnDeviceModelGUI();
	    }
	
	    //设备模型中面板位置
	    private Vector2 m_DeviceModelTablePosition = Vector2.zero;
	    //设备名称字段
	    private FieldInfo m_DeviceNameCellField = typeof(DeviceModel).GetField("m_DeviceName", BindingFlags.NonPublic | BindingFlags.Instance);
	    //模型名称字段
	    private FieldInfo m_ModelNameCellField = typeof(DeviceModel).GetField("m_ModelName", BindingFlags.NonPublic | BindingFlags.Instance);
	    //品质级别字段
	    private FieldInfo m_QualityLevelCellField = typeof(DeviceModel).GetField("m_QualityLevel", BindingFlags.NonPublic | BindingFlags.Instance);
	
	    private void OnDeviceModelGUI()
	    {
	        DeviceModel[] deviceModels = m_Config.GetDeviceModels();
	
	        DrawHeader();   //绘制头部
	
	        m_DeviceModelTablePosition = EditorGUILayout.BeginScrollView(m_DeviceModelTablePosition, GUILayout.Width(position.width));
	        {
	            int deleteIndex = -1;   //纪录删除的下标
	            for (int i = 0; i < deviceModels.Length; i++)
	            {
	                if (DrawItem(deviceModels[i]))
	                    deleteIndex = i;
	            }
	
	
	            //移除
	            if (deleteIndex >= 0)
	                m_Config.RemoveDeviceModelAt(deleteIndex);
	
	            //添加
	            if (GUILayout.Button("+", GUILayout.Width(20f)))
	                m_Config.NewDeviceModel();
	        }
	        EditorGUILayout.EndScrollView();
	    }
	
	    //绘制头部
	    private void DrawHeader()
	    {
	        EditorGUILayout.BeginHorizontal();
	        {
	            EditorGUILayout.LabelField(string.Empty, GUILayout.Width(20f));
	            EditorGUILayout.LabelField("Device Name", GUILayout.Width(200f));
	            EditorGUILayout.LabelField("Model Name", GUILayout.Width(100f));
	            EditorGUILayout.LabelField("Quality Level", GUILayout.Width(100f));
	        }
	        EditorGUILayout.EndHorizontal();
	    }
	
	    //绘制一项驱动模型
	    private bool DrawItem(DeviceModel row)
	    {
	        bool deleteMe = false;  //是否删除当前
	        EditorGUILayout.BeginHorizontal();
	        {
	            deleteMe = GUILayout.Button("-", GUILayout.Width(20f), GUILayout.Height(EditorGUIUtility.singleLineHeight));
	            DrawTextItem(row, m_DeviceNameCellField, 200f); //绘制驱动名
	            DrawTextItem(row, m_ModelNameCellField, 100f);  //绘制模型名
	            DrawEnumItem(row, m_QualityLevelCellField, 100f);   //绘制品质级别
	        }
	        EditorGUILayout.EndHorizontal();
	        return deleteMe;
	    }
	
	    //绘制文本项
	    private void DrawTextItem(object obj, FieldInfo field, float width = 300f)
	    {
	        string oldValue = (string)field.GetValue(obj);  //旧名称
	        string value = EditorGUILayout.TextField(oldValue, GUILayout.Width(width));
	        if (value != oldValue)
	            EditorUtility.SetDirty(m_Config);
	
	        field.SetValue(obj, value);
	    }
	
	    //绘制枚举项
	    private void DrawEnumItem(object obj, FieldInfo field, float width = 300f)
	    {
	        Enum oldValue = (Enum)field.GetValue(obj);
	        Enum value = EditorGUILayout.EnumPopup(oldValue, GUILayout.Width(width));
	        if (value != oldValue)
	            EditorUtility.SetDirty(m_Config);
	
	        field.SetValue(obj, value);
	    }
	
	}
}
