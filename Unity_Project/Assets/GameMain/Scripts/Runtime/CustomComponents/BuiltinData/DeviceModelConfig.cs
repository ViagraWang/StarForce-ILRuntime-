using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Runtime {	
	public class DeviceModelConfig : ScriptableObject
	{
	    [SerializeField]
	    private List<DeviceModel> m_DeviceModels = new List<DeviceModel>();    //多设备类型
	
	    //获取所有配置的设备
	    public DeviceModel[] GetDeviceModels()
	    {
	        return m_DeviceModels.ToArray();
	    }
	
	    //添加新的设备类型
	    public void NewDeviceModel()
	    {
	        m_DeviceModels.Add(new DeviceModel());
	    }
	
	    //移除设备
	    public void RemoveDeviceModelAt(int index)
	    {
	        m_DeviceModels.RemoveAt(index);
	    }
	
	    //获取品质级别
	    public QualityLevelType GetDefaultQualityLevel()
	    {
	        string modelName = SystemInfo.deviceModel;
	        for (int i = 0; i < m_DeviceModels.Count; i++)
	        {
	            if (m_DeviceModels[i].ModelName == modelName)
	            {
	                return m_DeviceModels[i].QualityLevel;
	            }
	        }
	
	        return QualityLevelType.Fastest;
	    }
	
	}
}
