using Game.Runtime;
using GameFramework;
using GameFramework.Localization;
using System;
using UnityEditor;
using UnityEngine;
using UnityGameFrame.Editor.Processor;

namespace Game.Editor
{	
	public sealed class GeneratorMenu
	{
        private const string MenuName = "GameMain Generator/";

        //创建打包初始化文件
        [MenuItem(MenuName + "Create Build Info", false, 0)]
        private static void CreateBuildInfo()
        {
            Editor.CreateBuildInfo.Create();
        }

        //生成数据表数据文件
        [MenuItem(MenuName + "Generate DataTable Files", false, 21)]
	    private static void GenerateDataTableFile()
	    {
	        for (int i = 0; i < ProcedurePreload.DataTableNames.Length; i++)
	        {
	            string dataTableName = ProcedurePreload.DataTableNames[i];
	            EditorUtility.DisplayProgressBar("序列化数据表文件", Utility.Text.Format("正在序列化{0}/{1}", i + 1, ProcedurePreload.DataTableNames.Length), (float)i / ProcedurePreload.DataTableNames.Length);
	            try
	            {
	                DataTableProcessor dataTableProcessor = DataTableGenerator.CreateDataTableProcessor(dataTableName);
	                if (!DataTableGenerator.CheckRawData(dataTableProcessor, dataTableName))
	                {
	                    Debug.LogError(Utility.Text.Format("Check raw data failure. DataTableName='{0}'", dataTableName));
	                    break;
	                }
	
	                DataTableGenerator.GenerateDataFile(dataTableProcessor, dataTableName); //创建数据文件
	            }
	            catch (Exception e)
	            {
	                Debug.LogError(e.ToString());
	            }
	        }
	
	        EditorUtility.ClearProgressBar();
	        AssetDatabase.SaveAssets();
	        AssetDatabase.Refresh();
	    }
	
	    //生成数据表数据文件和数据脚本
	    [MenuItem(MenuName + "Generate DataTable Files And Scripts", false, 22)]
	    private static void GenerateDataTableFilesAndScripts()
	    {
	        for (int i = 0; i < ProcedurePreload.DataTableNames.Length; i++)
	        {
	            string dataTableName = ProcedurePreload.DataTableNames[i];
	            EditorUtility.DisplayProgressBar("序列化数据表文件和数据脚本", Utility.Text.Format("正在序列化{0}/{1}", i + 1, ProcedurePreload.DataTableNames.Length), (float)i / ProcedurePreload.DataTableNames.Length);
	            try
	            {
	                DataTableProcessor dataTableProcessor = DataTableGenerator.CreateDataTableProcessor(dataTableName);
	                if (!DataTableGenerator.CheckRawData(dataTableProcessor, dataTableName))
	                {
	                    Debug.LogError(Utility.Text.Format("Check raw data failure. DataTableName='{0}'", dataTableName));
	                    break;
	                }
	
	                DataTableGenerator.GenerateDataFile(dataTableProcessor, dataTableName); //创建数据文件
	                DataTableGenerator.GenerateCodeFile(dataTableProcessor, dataTableName); //创建脚本文件
	            }
	            catch (Exception e)
	            {
	                Debug.LogError(e.ToString());
	            }
	        }
	
	        EditorUtility.ClearProgressBar();
	        AssetDatabase.SaveAssets();
	        AssetDatabase.Refresh();
	    }
	
	    //生成配置表
	    [MenuItem(MenuName + "Generate Config Files", false, 23)]
	    private static void GenerateConfig()
	    {
	        for (int i = 0; i < ProcedurePreload.ConfigNames.Length; i++)
	        {
	            string configName = ProcedurePreload.ConfigNames[i];
	            EditorUtility.DisplayProgressBar("序列化配置表文件", Utility.Text.Format("正在序列化{0}/{1}", i + 1, ProcedurePreload.ConfigNames.Length), (float)i / ProcedurePreload.ConfigNames.Length);
	            try
	            {
	                ConfigProcessor configProcessor = ConfigGenerator.CreateConfigProcessor(configName);
	                ConfigGenerator.GenerateDataFile(configProcessor, configName);
	            }
	            catch (Exception e)
	            {
	                Debug.LogError(e.ToString());
	            }
	        }
	
	        EditorUtility.ClearProgressBar();
	        AssetDatabase.SaveAssets();
	        AssetDatabase.Refresh();
	        Debug.Log("成功:生成全部配置表");
	    }
	
	    //生成本地化
	    [MenuItem(MenuName + "Generate Localization Files", false, 24)]
	    private static void GenerateLocalization()
	    {
	        string[] localizationNames = Enum.GetNames(typeof(Language));
	        for (int i = 0; i < localizationNames.Length; i++)
	        {
	            string localizationName = localizationNames[i];
	            EditorUtility.DisplayProgressBar("序列化本地化数据", Utility.Text.Format("正在序列化{0}/{1}", i + 1, localizationNames.Length), (float)i / localizationNames.Length);
	            try
	            {
	                ConfigProcessor configProcessor = LocalizationGenerator.CreateConfigProcessor(localizationName);
	                if (configProcessor == null)
	                {
	                    Debug.LogWarning(Utility.Text.Format("警告:不存在本地化 -> {0}.", localizationName));
	                    continue;
	                }
	                LocalizationGenerator.GenerateDataFile(configProcessor, localizationName);
	            }
	            catch (Exception e)
	            {
	                Debug.LogError(e.ToString());
	            }
	        }
	
	        EditorUtility.ClearProgressBar();
	        AssetDatabase.SaveAssets();
	        AssetDatabase.Refresh();
	        Debug.Log("成功:生成全部本地化数据");
	    }
	
	    //数据表Excel -> Csv
	    [MenuItem(MenuName + "DataTables Excel2Csv", false, 41)]
	    public static void DataTablesExcelToCsv()
	    {
	        ExcelAndCsv.ExcelDataTablesToCsv();
	    }
	
	    //数据表 Csv -> Excel
	    //[MenuItem("GameMain/DataTables Csv2Excel", false, 42)]
	    //public static void DataTablesCsvToExcel()
	    //{
	    //    ExcelAndCsv.CsvDataTablesToExcel();
	    //}
	
	    //配置表 Excel -> Csv
	    [MenuItem(MenuName + "Configs Excel2Csv", false, 43)]
	    public static void ConfigsExcelToCsv()
	    {
	        ExcelAndCsv.ExcelConfigsToCsv();
	    }
	
	    //本地化 Excel -> Csv
	    [MenuItem(MenuName + "Localizations Excel2Csv", false, 44)]
	    public static void LocalizationsExcelToCsv()
	    {
	        ExcelAndCsv.ExcelLocalizationToCsv();
	    }

        //本地化 Csv -> Excel
        //[MenuItem("GameMain/Localizations Csv2Excel", false, 45)]
        //public static void LocalizationsCsvToExcel()
        //{
        //    ExcelAndCsv.CsvLocalizationToExcel();
        //}

        /// <summary>
        /// 存储可序列化的资源。
        /// </summary>
        /// <remarks>等同于执行 Unity 菜单 File/Save Project。</remarks>
        [MenuItem(MenuName  +"Save Assets &s", false, 200)]
        public static void SaveAssets()
        {
#if UNITY_5_5_OR_NEWER
            AssetDatabase.SaveAssets();
#else
	        EditorApplication.SaveAssets();
#endif
            Debug.Log("You have saved the serializable assets in the project.");
        }

    }
}
