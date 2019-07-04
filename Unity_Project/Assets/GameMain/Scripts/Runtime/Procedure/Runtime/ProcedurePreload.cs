using GameFramework;
using GameFramework.Fsm;
using GameFramework.Procedure;
using GameFramework.Resource;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityGameFrame.Runtime;

namespace Game.Runtime
{	
	//预加载资源的流程
	public class ProcedurePreload : ProcedureBase
	{
	    //配置表名称
	    public static readonly string[] ConfigNames = new string[]
	    {
	        "DefaultConfig"
	    };
	
	    //数据表名称
	    public static readonly string[] DataTableNames = new string[]
	    {
	            //"Test", // 这是个测试资源，并没有使用
	            "Aircraft",
	            "Armor",
	            "Asteroid",
	            "Entity",
	            "Music",
	            "Scene",
	            "Sound",
	            "Thruster",
	            "UIForm",
	            "UISound",
	            "Weapon",
	    };
	
	    //保存加载完成的标志位
	    private Dictionary<string, bool> m_LoadedFlag = new Dictionary<string, bool>();
	
	    public override bool UseNativeDialog { get { return true; } }

        //热更新程序
        private Dictionary<string, byte[]> m_loadedHotifx = new Dictionary<string, byte[]>();

        private bool m_IsFinish = false;  //是否加载完的标志位

        //进入流程
        protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
	    {
	        base.OnEnter(procedureOwner);
	        //订阅事件
	        GameEntry.Event.Subscribe(LoadConfigSuccessEventArgs.EventId, OnLoadConfigSuccess);
	        GameEntry.Event.Subscribe(LoadConfigFailureEventArgs.EventId, OnLoadConfigFailure);
	        GameEntry.Event.Subscribe(LoadDataTableSuccessEventArgs.EventId, OnLoadDataTableSuccess);
	        GameEntry.Event.Subscribe(LoadDataTableFailureEventArgs.EventId, OnLoadDataTableFailure);
	        GameEntry.Event.Subscribe(LoadDictionarySuccessEventArgs.EventId, OnLoadDictionarySuccess);
	        GameEntry.Event.Subscribe(LoadDictionaryFailureEventArgs.EventId, OnLoadDictionaryFailure);
	
	        m_LoadedFlag.Clear();
	
	        PreloadResources(); //预加载资源
	    }
	
	    //离开流程
	    protected override void OnLeave(IFsm<IProcedureManager> procedureOwner, bool isShutdown)
	    {
	        base.OnLeave(procedureOwner, isShutdown);
	        //取消订阅事件
	        GameEntry.Event.Unsubscribe(LoadConfigSuccessEventArgs.EventId, OnLoadConfigSuccess);
	        GameEntry.Event.Unsubscribe(LoadConfigFailureEventArgs.EventId, OnLoadConfigFailure);
	        GameEntry.Event.Unsubscribe(LoadDataTableSuccessEventArgs.EventId, OnLoadDataTableSuccess);
	        GameEntry.Event.Unsubscribe(LoadDataTableFailureEventArgs.EventId, OnLoadDataTableFailure);
	        GameEntry.Event.Unsubscribe(LoadDictionarySuccessEventArgs.EventId, OnLoadDictionarySuccess);
	        GameEntry.Event.Unsubscribe(LoadDictionaryFailureEventArgs.EventId, OnLoadDictionaryFailure);
	    }
	
	    protected override void OnUpdate(IFsm<IProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
	    {
	        base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            if (m_IsFinish) //完成全部预加载，并且切换到热更新模式后，等待热更新模式的流程开启
                return;

	        foreach (var item in m_LoadedFlag.Values)
	        {
	            if (!item)
	                return;
	        }

            //进入热更新
            m_IsFinish = true;
            string hotfixDllName = Utility.Text.Format("Hotfix.{0}", RuntimeAssetUtility.HotfixDllName);
            string hotfixPdbName = Utility.Text.Format("Hotfix.{0}", RuntimeAssetUtility.HotfixPdbName);
            byte[] dllBytes, pdbBytes;
            m_loadedHotifx.TryGetValue(hotfixDllName, out dllBytes);
            m_loadedHotifx.TryGetValue(hotfixPdbName, out pdbBytes);
            GameEntry.Hotfix.LoadHotfixAssembly(dllBytes, pdbBytes); //启动热更新

            //设置新的流程数据，开启新的流程
            //HotProcedure[] newProcedures = GetAllProcedures().ToArray();
            //GameEntry.Procedure.StartInitProcedure(newProcedures, newProcedures[0]);
            ChangeState<HotProcedureEntry>(procedureOwner);   //这里不能这样操作
            GC.Collect();
        }

        //获取所有的流程
        private static List<HotProcedure> GetAllProcedures()
        {
            List<HotProcedure> results = new List<HotProcedure>();
            string enterProcedureTypeName = typeof(HotProcedureEntry).FullName;  //流程入口

            Type[] types = Assembly.GetExecutingAssembly().GetTypes();  //所有的类型
            for (int i = 0; i < types.Length; i++)
            {
                Type type = types[i];

                if (type.IsAbstract || !type.IsClass) //抽象类或者非类
                    continue;

                if (typeof(HotProcedure).IsAssignableFrom(type))
                {
                    object obj = Activator.CreateInstance(type);
                    if (enterProcedureTypeName == type.FullName)
                        results.Insert(0, obj as HotProcedure);    //流程入口放入第一个
                    else
                        results.Add(obj as HotProcedure);
                }
            }

            return results;
        }

        //预加载资源
        private void PreloadResources()
	    {
	        //加载配置表
	        for (int i = 0; i < ConfigNames.Length; i++)
	        {
	            LoadConfig(ConfigNames[i]);
	        }
	
	        //Preload data tables
	        for (int i = 0; i < DataTableNames.Length; i++)
	        {
	            LoadDataTable(DataTableNames[i]);
	        }
	
	        //加载本地化
	        LoadDictionary(GameEntry.Localization.Language.ToString());
	
	        //加载字体
	        LoadFont("MainFont");

            //加载热更资源
            LoadHotfix(RuntimeAssetUtility.HotfixDllName);
            LoadHotfix(RuntimeAssetUtility.HotfixPdbName);

        }
	
	    //加载配置
	    private void LoadConfig(string configName)
	    {
	        m_LoadedFlag.Add(Utility.Text.Format("Config.{0}", configName), false);
	        GameEntry.Config.LoadConfig(configName, LoadType.Bytes, this);
	    }
	
	    //加载配置表数据
	    private void LoadDataTable(string dataTableName)
	    {
	        m_LoadedFlag.Add(Utility.Text.Format("DataTable.{0}", dataTableName), false);
	        GameEntry.DataTable.LoadDataTable(dataTableName, LoadType.Bytes, this);
	    }
	
	    //加载字典
	    private void LoadDictionary(string dictionaryName)
	    {
	        m_LoadedFlag.Add(Utility.Text.Format("Dictionary.{0}", dictionaryName), false);
	        GameEntry.Localization.LoadDictionary(dictionaryName, LoadType.Bytes, this);
	    }
	
	    //加载字体
	    private void LoadFont(string fontName)
	    {
            string name = Utility.Text.Format("Font.{0}", fontName);
	        m_LoadedFlag.Add(name, false);
	        GameEntry.Resource.LoadAsset(RuntimeAssetUtility.GetFontAsset(fontName), RuntimeConstant.AssetPriority.FontAsset,
	            new LoadAssetCallbacks(
	                //加载成功的回调
	                (assetName, asset, duration, userData) =>
	                {
	                    m_LoadedFlag[name] = true;
	                    UGUIForm.SetMainFont((Font)asset);
	                    Log.Info("Load font '{0}' OK.", fontName);
	                },
	                //加载失败的回调
	                (assetName, status, errorMessage, userData) =>
	                {
	                    Log.Error("Can not load font '{0}' from '{1}' with error message '{2}'.", fontName, assetName, errorMessage);
	                }
	            ));
	    }

        //加载热更新脚本
        private void LoadHotfix(string hotfixName)
        {
            string name = Utility.Text.Format("Hotfix.{0}", hotfixName);
            m_LoadedFlag.Add(name, false);
            GameEntry.Resource.LoadAsset(RuntimeAssetUtility.GetHotfixAsset(hotfixName), RuntimeConstant.AssetPriority.FontAsset,
                new LoadAssetCallbacks(
                    //加载成功的回调
                    (assetName, asset, duration, userData) =>
                    {
                        m_LoadedFlag[name] = true;
                        m_loadedHotifx.Add(name, ((TextAsset)asset).bytes);
                        Log.Info("Load hotfix '{0}' OK.", hotfixName);
                    },
                    //加载失败的回调
                    (assetName, status, errorMessage, userData) =>
                    {
                        Log.Error("Can not load hotfix '{0}' from '{1}' with error message '{2}'.", hotfixName, assetName, errorMessage);
                    }
            ));

        }
	
	    //加载配置成功的回调
	    private void OnLoadConfigSuccess(object sender, BaseEventArgs e)
	    {
	        LoadConfigSuccessEventArgs args = e as LoadConfigSuccessEventArgs;
	        if (args.UserData != this)
	            return;
	
	        m_LoadedFlag[Utility.Text.Format("Config.{0}", args.ConfigName)] = true;
	        Log.Info("Load config '{0}' OK.", args.ConfigName);
	    }
	
	    //加载配置失败的回调
	    private void OnLoadConfigFailure(object sender, BaseEventArgs e)
	    {
	        LoadConfigFailureEventArgs args = e as LoadConfigFailureEventArgs;
	        if (args.UserData != this)
	            return;
	
	        Log.Error("Can not load config '{0}' from '{1}' with error message '{2}'.", args.ConfigName, args.ConfigAssetName, args.ErrorMessage);
	    }
	
	    //加载数据表成功的回调
	    private void OnLoadDataTableSuccess(object sender, BaseEventArgs e)
	    {
	        LoadDataTableSuccessEventArgs args = e as LoadDataTableSuccessEventArgs;
	        if (args.UserData == this)
            {
                m_LoadedFlag[Utility.Text.Format("DataTable.{0}", args.DataTableName)] = true;
                Log.Info("Load data table '{0}' OK.", args.DataTableName);
            }
	    }
	
	    //加载数据表失败的回调
	    private void OnLoadDataTableFailure(object sender, BaseEventArgs e)
	    {
	        LoadDataTableFailureEventArgs args = e as LoadDataTableFailureEventArgs;
	        if (args.UserData != this)
	            return;
	
	        Log.Error("Can not load data table '{0}' from '{1}' with error message '{2}'.", args.DataTableName, args.DataTableAssetName, args.ErrorMessage);
	    }
	
	    //加载字典成功的回调
	    private void OnLoadDictionarySuccess(object sender, BaseEventArgs e)
	    {
	        LoadDictionarySuccessEventArgs args = e as LoadDictionarySuccessEventArgs;
	        if (args.UserData != this)
	            return;
	
	        m_LoadedFlag[Utility.Text.Format("Dictionary.{0}", args.DictionaryName)] = true;
	        Log.Info("Load dictionary '{0}' OK.", args.DictionaryName);
	    }
	
	    //加载字典失败的回调
	    private void OnLoadDictionaryFailure(object sender, BaseEventArgs e)
	    {
	        LoadDictionaryFailureEventArgs args = e as LoadDictionaryFailureEventArgs;
	        if (args.UserData != this)
	            return;
	
	        Log.Error("Can not load dictionary '{0}' from '{1}' with error message '{2}'.", args.DictionaryName, args.DictionaryAssetName, args.ErrorMessage);
	    }
	
	}
}
