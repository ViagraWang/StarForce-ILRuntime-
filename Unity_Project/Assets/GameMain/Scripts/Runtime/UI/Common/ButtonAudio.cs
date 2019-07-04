//=======================================================
// 作者：
// 描述：带音效的按钮事件
//=======================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Runtime
{	
    /// <summary>
    /// 带音效的按钮
    /// </summary>
	public class ButtonAudio : CommonButton
    {

        public override void OnPointerClick(PointerEventData eventData)
        {
            GameEntry.Sound.PlayUISound(10001);  //播放UI音效
            base.OnPointerClick(eventData);
        }


    }
}