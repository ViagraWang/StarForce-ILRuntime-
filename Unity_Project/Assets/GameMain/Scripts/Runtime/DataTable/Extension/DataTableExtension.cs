using GameFramework;
using System;
using UnityEngine;
using UnityGameFrame.Runtime;

namespace Game.Runtime {	
	//数据表扩展
	public static class DataTableExtension
	{
	    public const string DataRowClassPrefixName = "DR";   //数据表类的前缀名
	    internal static readonly char[] DataSplitSeparators = new char[] { '\t' };   //数据分离字符
	    internal static readonly char[] DataTrimSeparators = new char[] { '\"' };    //结尾排除字符
	
	    //加载数据表
	    public static void LoadDataTable(this DataTableComponent dataTableComponent, string dataTableName, LoadType loadType, object userData)
	    {
	        if (string.IsNullOrEmpty(dataTableName))
	        {
	            Log.Warning("Data table name is invalid.");
	            return;
	        }
	
	        //只能有一个下划线
	        string[] splitNames = dataTableName.Split('_');
	        if (splitNames.Length > 2)
	        {
	            Log.Warning("Data table name is invalid.");
	            return;
	        }

            string dataRowClassName = Utility.Text.Format("Game.Runtime.{0}{1}", DataRowClassPrefixName, splitNames[0]);

            Type dataRowType = Type.GetType(dataRowClassName, true);  //获取数据类
	        if(dataRowType == null)
	        {
	            Log.Warning("Can not get data row type with class name '{0}'.", dataRowClassName);
	            return;
	        }
	
	        string dataTableNameInType = splitNames.Length > 1 ? splitNames[1] : null;
	        dataTableComponent.LoadDataTable(dataRowType, dataTableName, dataTableNameInType, RuntimeAssetUtility.GetDataTableAsset(dataTableName, loadType), loadType, userData);
	    }
	
	    //解析32位颜色
	    public static Color32 ParseColor32(string value)
	    {
	        string[] splitValue = value.Split(',');
	        return new Color32(byte.Parse(splitValue[0]), byte.Parse(splitValue[1]), byte.Parse(splitValue[2]), byte.Parse(splitValue[3]));
	    }
	
	    //解析颜色
	    public static Color ParseColor(string value)
	    {
	        string[] splitValue = value.Split(',');
	        return new Color(float.Parse(splitValue[0]), float.Parse(splitValue[1]), float.Parse(splitValue[2]), float.Parse(splitValue[3]));
	    }
	
	    //解析四元数
	    public static Quaternion ParseQuaternion(string value)
	    {
	        string[] splitValue = value.Split(',');
	        return new Quaternion(float.Parse(splitValue[0]), float.Parse(splitValue[1]), float.Parse(splitValue[2]), float.Parse(splitValue[3]));
	    }
	
	    //解析Rect
	    public static Rect ParseRect(string value)
	    {
	        string[] splitValue = value.Split(',');
	        return new Rect(float.Parse(splitValue[0]), float.Parse(splitValue[1]), float.Parse(splitValue[2]), float.Parse(splitValue[3]));
	    }
	
	    //解析Vector2
	    public static Vector2 ParseVector2(string value)
	    {
	        string[] splitValue = value.Split(',');
	        return new Vector2(float.Parse(splitValue[0]), float.Parse(splitValue[1]));
	    }
	
	    //解析Vector3
	    public static Vector3 ParseVector3(string value)
	    {
	        string[] splitValue = value.Split(',');
	        return new Vector3(float.Parse(splitValue[0]), float.Parse(splitValue[1]), float.Parse(splitValue[2]));
	    }
	
	    //解析Vector4
	    public static Vector4 ParseVector4(string value)
	    {
	        string[] splitValue = value.Split(',');
	        return new Vector4(float.Parse(splitValue[0]), float.Parse(splitValue[1]), float.Parse(splitValue[2]), float.Parse(splitValue[3]));
	    }
	}
}
