//=======================================================
// 作者：
// 描述：引用的对象键值对
//=======================================================
using System;

namespace Game.Runtime
{	
	public sealed partial class ReferenceCollector
    {

        [Serializable]
        public class ReferenceCollectorData
        {
            public string key;
            //Object并非C#基础中的Object，而是 UnityEngine.Object
            public UnityEngine.GameObject obj;
        }

    }
}