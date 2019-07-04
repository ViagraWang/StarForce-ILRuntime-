using System;
using UnityEngine;

namespace UnityGameFrame.Runtime
{
    public sealed partial class UIComponent
    {
        //界面组
        [Serializable]
        private sealed class UIGroup
        {
            [SerializeField]
            private string m_Name = null;  //界面组名public称
            [SerializeField]
            private int m_Depth = 0;    //界面组深度

            public string Name
            {
                get
                {
                    return m_Name;
                }
            }

            public int Depth
            {
                get
                {
                    return m_Depth;
                }
            }

        }
    }
}
