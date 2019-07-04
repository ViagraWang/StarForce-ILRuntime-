using Game.Runtime;
using GameFramework;
using Spire.Xls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Game.Editor
{	
	//xlsx转csv工具
	public sealed class ExcelAndCsv
	{
	    public const string Separator = "\t";   //分隔符
    private const string CommentLineSeparator = "#";    //注释符
	    private static readonly char[] DataTrimSeparators = new char[] { '\"' };
	    public const string excelExtension = ".xlsx";   //Excel表文件的扩展名
	    //public const string csvExtension = ".txt";  //csv文件的扩展名
	
	    //外部的数值表
	    public static readonly string OutValuesPath = Application.dataPath + "/../../数值/";
	    public static readonly string OutConfigs = OutValuesPath + "Configs";
	    public static readonly string OutDataTables = OutValuesPath + "DataTables";
	    public static readonly string OutLocalizations = OutValuesPath + "Localizations";
	
	    //数据表
	    public static void ExcelDataTablesToCsv()
	    {
	        ExcelToCsv(OutDataTables, Utility.Path.GetCombinePath(RuntimeAssetUtility.DataTablePath, RuntimeAssetUtility.CsvFolder));
	
	        AssetDatabase.SaveAssets();
	        AssetDatabase.Refresh();
	        Debug.Log(Utility.Text.Format("DataTables Excel -> Csv 完成：{0}", RuntimeAssetUtility.DataTablePath));
	    }
	
	    //数据表
	    public static void CsvDataTablesToExcel()
	    {
	        CsvToExcel(RuntimeAssetUtility.DataTablePath, OutDataTables);
	        Debug.Log(Utility.Text.Format("DataTables Csv -> Excel 完成：{0}", OutDataTables));
	    }
	
	    //配置表
	    public static void ExcelConfigsToCsv()
	    {
	        ExcelToCsv(OutConfigs, RuntimeAssetUtility.ConfigPath);
	        AssetDatabase.SaveAssets();
	        AssetDatabase.Refresh();
	        Debug.Log(Utility.Text.Format("DataTables Csv -> Excel 完成：{0}", OutDataTables));
	    }
	
	    //配置表
	    public static void CsvConfigsToExcel()
	    {
	
	    }
	
	    //本地化
	    public static void ExcelLocalizationToCsv()
	    {
	        ExcelToCsv(OutLocalizations, Utility.Path.GetCombinePath(RuntimeAssetUtility.LocalizationPath, RuntimeAssetUtility.CsvFolder));
	        AssetDatabase.SaveAssets();
	        AssetDatabase.Refresh();
	        Debug.Log(Utility.Text.Format("Localizations Excel -> Csv 完成：{0}", RuntimeAssetUtility.LocalizationPath));
	    }
	
	    //本地化
	    public static void CsvLocalizationToExcel()
	    {
	        CsvToExcel(RuntimeAssetUtility.LocalizationPath, OutLocalizations);
	        Debug.Log(Utility.Text.Format("Localizations Csv -> Excel 完成：{0}", OutLocalizations));
	    }
	
	    //Excel -> Csv
	    private static void ExcelToCsv(string excelDirectory, string csvDirectory)
	    {
	        List<FileInfo> listFile = GetFiles(excelDirectory, excelExtension);
	        for (int i = 0; i < listFile.Count; i++)
	        {
	            EditorUtility.DisplayProgressBar("转换 Excel 至 Csv", Utility.Text.Format("正在转换{0}/{1}", i + 1, listFile.Count), (float)i / listFile.Count);
	            FileInfo fileInfo = listFile[i];
	            DoExcelToCsv(fileInfo.FullName.Replace("\\", "/"), Utility.Path.GetCombinePath(csvDirectory, fileInfo.Name.Replace(excelExtension, RuntimeAssetUtility.csvExtension)));
	        }
	        EditorUtility.ClearProgressBar();
	    }
	
	    //Csv -> Excel
	    private static void CsvToExcel(string csvDirectory, string excelDirectory)
	    {
	        List<FileInfo> listFile = GetFiles(csvDirectory, RuntimeAssetUtility.csvExtension);
	        for (int i = 0; i < listFile.Count; i++)
	        {
	            EditorUtility.DisplayProgressBar("转换 Excel 至 Csv", Utility.Text.Format("正在转换{0}/{1}", i + 1, listFile.Count), (float)i / listFile.Count);
	            FileInfo fileInfo = listFile[i];
	            DoCsvToExcel(fileInfo.FullName.Replace("\\", "/"), Utility.Path.GetCombinePath(excelDirectory, fileInfo.Name.Replace(RuntimeAssetUtility.csvExtension, excelExtension)));
	        }
	        EditorUtility.ClearProgressBar();
	    }
	
	    //单个xlsx转csv
	    private static bool DoExcelToCsv(string excelPath, string csvPath)
	    {
	        if(!File.Exists(excelPath))
	        {
	            return false;
	        }
	        try
	        {
	            string dir = Path.GetDirectoryName(csvPath);
	            if (!Directory.Exists(dir))
	                Directory.CreateDirectory(dir);
	            else if (File.Exists(csvPath))
	                File.Delete(csvPath);
            using (Workbook work = new Workbook())
	            {
	                work.LoadFromFile(excelPath, ExcelVersion.Version2016);
	                Worksheet sheet = work.Worksheets[0];   //获取第一张工作表
	                sheet.IsStringsPreserved = true;
	                sheet.SaveToFile(csvPath, Separator, Encoding.UTF8);
	                return true;
	            }
	        }
	        catch (Exception e)
	        {
	            Debug.Log(Utility.Text.Format("xlsx转csv出错 -> {0}", e.ToString()));
	            return false;
	        }
	    }
	
	    //单个csv转xlsx
	    private static bool DoCsvToExcel(string csvPath, string excelPath)
	    {
	        if (!File.Exists(csvPath))
	        {
	            return false;
	        }
	        try
	        {
	            string dir = Path.GetDirectoryName(excelPath);
	            if (!Directory.Exists(dir))
	                Directory.CreateDirectory(dir);
	            else if (File.Exists(excelPath))
	                File.Delete(excelPath);
            using (Workbook work = new Workbook())
	            {
	                Debug.Log("Csv文件 -> " + csvPath);
	                Debug.Log("Excel文件 -> " + excelPath);
	                work.LoadFromFile(csvPath, Separator);
	                work.SaveToFile(excelPath, FileFormat.Version2016);
	                return true;
	            }
	        }
	        catch (Exception e)
	        {
	            Debug.Log(Utility.Text.Format("csv转xlsx出错 -> {0}", e.ToString()));
	            return false;
	        }
	    }
	
	    //获取文件列表
	    private static List<FileInfo> GetFiles(string Directory, string fileExtension)
	    {
	        DirectoryInfo folder = new DirectoryInfo(Directory);
	        List<FileInfo> listTemp = new List<FileInfo>();
	        foreach (FileInfo file in folder.GetFiles("*" + fileExtension, SearchOption.AllDirectories))
	        {
	            if (file.Name.Contains("~$"))
	                continue;
	            string filePath = file.FullName.Replace('\\', '/');
	            listTemp.Add(file);
	        }
	        return listTemp;
	    }
	
	}
}
