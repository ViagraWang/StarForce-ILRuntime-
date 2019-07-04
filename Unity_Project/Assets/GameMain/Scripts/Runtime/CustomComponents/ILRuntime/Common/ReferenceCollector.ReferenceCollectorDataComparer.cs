//=======================================================
// 作者：
// 描述：数据对象排序
//=======================================================
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Runtime
{	
	public sealed partial class ReferenceCollector
    {
        public class ReferenceCollectorDataComparer : IComparer<ReferenceCollectorData>
        {
            public int Compare(ReferenceCollectorData x, ReferenceCollectorData y)
            {
                return string.Compare(x.key, y.key, StringComparison.Ordinal);
            }
        }

    }
}