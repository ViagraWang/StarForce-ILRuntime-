using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Runtime
{	
	public class UpdateResourceForm : MonoBehaviour
	{
	
	    [SerializeField]
	    private Text m_DescriptionText = null;  //提示文本
	
	    [SerializeField]
	    private Slider m_ProgressSlider = null; //进度条
	
	    private void Start()
	    {
	        m_ProgressSlider.value = m_ProgressSlider.minValue;
	    }
	
	    //设置进度
	    public void SetProgress(float progress, string description)
	    {
	        m_ProgressSlider.value = progress;
	        m_DescriptionText.text = description;
	    }
	
	}
}
