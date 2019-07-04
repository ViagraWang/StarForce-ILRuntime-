using GameFramework;
using UnityEngine;
using UnityGameFrame.Runtime;

namespace Game.Runtime {	
	//内置数据组件，包含设备类型配置，版本更新信息、本地化字典
	public class BuiltinDataComponent : GameFrameworkComponent
	{
	
	    [SerializeField]
	    private TextAsset m_BuildInfoTextAsset = null;  //配置的构建版本信息文本
	
	    [SerializeField]
	    private TextAsset m_DefaultDictionaryTextAsset = null;  //本地化字典文本
	
	    [SerializeField]
	    private UpdateResourceForm m_UpdateResourceFormTemplate = null; //更新资源的界面模板
	    public UpdateResourceForm UpdateResourceFormTemplate { get { return m_UpdateResourceFormTemplate; } }
	    //[SerializeField]
	    //private DeviceModelConfig m_DeviceModelConfig = null;   //设备驱动配置
	    //public DeviceModelConfig DeviceModelConfig { get { return m_DeviceModelConfig; } }
	
	    private BuildInfo m_BuildInfo = null;   //构建的发包信息
	    public BuildInfo BuildInfo { get { return m_BuildInfo; } }
	
	    //初始化发包信息
	    public void InitBuildInfo()
	    {
	        //配置的版本信息文本
	        if(m_BuildInfoTextAsset == null || string.IsNullOrEmpty(m_BuildInfoTextAsset.text))
	        {
	            Log.Info("Build info can not be found or empty.");
	            return;
	        }
	
	        //反序列化
	        m_BuildInfo = Utility.Json.ToObject<BuildInfo>(m_BuildInfoTextAsset.text);
	        if (m_BuildInfo == null)
	        {
	            Log.Warning("Parse build info failure.");
	            return;
	        }
	    }
	
	    //初始化默认本地化字典
	    public void InitDefaultDictionary()
	    {
	        if (m_DefaultDictionaryTextAsset == null || string.IsNullOrEmpty(m_DefaultDictionaryTextAsset.text))
	        {
	            Log.Info("Default dictionary can not be found or empty.");
	            return;
	        }
	
	        if (!GameEntry.Localization.ParseDictionary(m_DefaultDictionaryTextAsset.text))
	        {
	            Log.Warning("Parse default dictionary failure.");
	            return;
	        }
	
	        //卸载资源
	        Resources.UnloadAsset(m_DefaultDictionaryTextAsset);
	    }
	
	}
}
