using GameFramework.Fsm;
using GameFramework.Localization;
using GameFramework.Procedure;
using System;
using UnityEngine;
using UnityGameFrame.Runtime;

namespace Game.Runtime {	
	public class ProcedureLaunch : ProcedureBase
	{
	    public override bool UseNativeDialog { get { return true; } }
	
	    protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
	    {
	        base.OnEnter(procedureOwner);
	
//#if UNITY_EDITOR
	//        if (Application.unityVersion != "2018.2.20f1" || Application.platform != RuntimePlatform.WindowsEditor)
	//        {
	//            UnityEditor.EditorUtility.DisplayDialog("警告", "此热更新 Demo 使用的资源包仅适用于 Unity 2018.2.20f1、Windows 系统平台版本，您当前使用的 Unity 版本或系统平台不匹配，这可能导致材质丢失等显示错误。", "知道了");
	//        }
//#endif
	
	        // 构建信息：发布版本时，把一些数据以 Json 的格式写入 Assets/GameMain/Configs/BuildInfo.txt，供游戏逻辑读取。
	        GameEntry.BuiltinData.InitBuildInfo();
	
	        // 语言配置：设置当前使用的语言，如果不设置，则默认使用操作系统语言。
	        InitLanguageSettings();
	
	        // 变体配置：根据使用的语言，通知底层加载对应的资源变体。
	        InitCurrentVariant();
	
	        // 画质配置：根据检测到的硬件信息 Assets/Main/Configs/DeviceModelConfig 和用户配置数据，设置即将使用的画质选项。
	        InitQualitySettings();
	
	        // 声音配置：根据用户配置数据，设置即将使用的声音选项。
	        InitSoundSettings();
	
	        // 默认字典：加载默认字典文件 Assets/GameMain/Configs/DefaultDictionary.xml。
	        // 此字典文件记录了资源更新前使用的各种语言的字符串，会随 App 一起发布，故不可更新。
	        GameEntry.BuiltinData.InitDefaultDictionary();
	
	        ChangeState<ProcedureSplash>(procedureOwner);   //立即切换流程
	    }
	
	    //protected override void OnUpdate(IFsm<IProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
	    //{
	    //    base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
	
	    //    ChangeState<ProcedureSplash>(procedureOwner);   //立即切换流程
	    //}
	
	    //初始化语言设置
	    private void InitLanguageSettings()
	    {
	        // 编辑器资源模式直接使用 Inspector 上设置的语言
	        if (GameEntry.Base.IsEditorResourceMode && GameEntry.Base.EditorLanguage != Language.Unspecified)
	            return;
	
	        Language language = GameEntry.Localization.Language;
	        string languageString = GameEntry.Setting.GetString(RuntimeConstant.Setting.Language);
	        if (!string.IsNullOrEmpty(languageString))
	        {
	            try
	            {
	                language = (Language)Enum.Parse(typeof(Language), languageString);
	            }
	            catch (Exception)
	            {
	                Log.Warning("无法解析保存的语言类型 -> " + languageString);
	            }
	        }
	
	        //不支持的语言类型
	        //TODO:后面改为支持的语言类型判断
	        if(language != Language.English && language != Language.ChineseSimplified && language != Language.ChineseTraditional && language != Language.Korean)
	        {
	            language = Language.English;    //默认英语
	
	            GameEntry.Setting.SetString(RuntimeConstant.Setting.Language, language.ToString());
	            GameEntry.Setting.Save();
	        }
	
	        GameEntry.Localization.Language = language; //设置语言类型
	        Log.Info("Init language settings complete, current language is '{0}'.", language.ToString());
	    }
	
	    //初始化当前变体
	    private void InitCurrentVariant()
	    {
	        // 编辑器资源模式不使用 AssetBundle，也就没有变体了
	        if (GameEntry.Base.IsEditorResourceMode)
	            return;
	
	        string currentVariant = null;
	        switch (GameEntry.Localization.Language)
	        {
	            case Language.English:
	                currentVariant = "en-us";
	                break;
	            case Language.ChineseSimplified:
	                currentVariant = "zh-cn";
	                break;
	            case Language.ChineseTraditional:
	                currentVariant = "zh-tw";
	                break;
	            case Language.Korean:
	                currentVariant = "ko-kr";
	                break;
	            default:
	                currentVariant = "zh-cn";
	                break;
	        }
	
	        GameEntry.Resource.SetCurrentVariant(currentVariant);   //设置当前变体名
	
	        Log.Info("Init current variant complete.");
	    }
	
	    //初始化品质设置
	    private void InitQualitySettings()
	    {
	        //QualityLevelType defaultQuality = GameEntry.BuiltinData.DeviceModelConfig.GetDefaultQualityLevel();
	        QualityLevelType defaultQuality = QualityLevelType.Fantastic;
	        int qualityLevel = GameEntry.Setting.GetInt(RuntimeConstant.Setting.QualityLevel, (int)defaultQuality);
	        QualitySettings.SetQualityLevel(qualityLevel, true);
	        Log.Info("Init quality settings complete.");
	    }
	
	    //初始化声音设置
	    private void InitSoundSettings()
	    {
	        GameEntry.Sound.Mute("Music", GameEntry.Setting.GetBool(RuntimeConstant.Setting.MusicMuted, false));
	        GameEntry.Sound.SetGroupVolume("Music", GameEntry.Setting.GetFloat(RuntimeConstant.Setting.MusicVolume, 0.3f));
	        GameEntry.Sound.Mute("Sound", GameEntry.Setting.GetBool(RuntimeConstant.Setting.SoundMuted, false));
	        GameEntry.Sound.SetGroupVolume("Sound", GameEntry.Setting.GetFloat(RuntimeConstant.Setting.SoundVolume, 1f));
	        GameEntry.Sound.Mute("UISound", GameEntry.Setting.GetBool(RuntimeConstant.Setting.UISoundMuted, false));
	        GameEntry.Sound.SetGroupVolume("UISound", GameEntry.Setting.GetFloat(RuntimeConstant.Setting.UISoundVolume, 1f));
	
	        Log.Info("Init sound settings complete.");
	    }
	
	}
}
