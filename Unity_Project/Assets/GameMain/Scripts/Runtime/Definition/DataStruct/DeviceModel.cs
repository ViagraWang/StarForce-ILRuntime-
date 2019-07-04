using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Runtime {	
	//设备模型
	[Serializable]
	public class DeviceModel
	{
	    [SerializeField]
	    private string m_DeviceName = null; //设备名称
	
	    [SerializeField]
	    private string m_ModelName = null;  //模型名称
	
	    [SerializeField]
	    private QualityLevelType m_QualityLevel = QualityLevelType.Fastest; //品质等级
	
	    public string DeviceName { get { return m_DeviceName; } }
	
	    public string ModelName { get { return m_ModelName; } }
	
	    public QualityLevelType QualityLevel { get { return m_QualityLevel; } }
	}
}
