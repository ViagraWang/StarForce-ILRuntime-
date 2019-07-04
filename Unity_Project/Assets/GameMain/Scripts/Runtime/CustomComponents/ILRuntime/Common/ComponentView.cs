using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Runtime
{
    public class ComponentView : MonoBehaviour
    {
        [SerializeField]
        private string m_LogicScript;
        //逻辑脚本名称
        public string LogicScript { get { return m_LogicScript; } }

        private object m_Component;
        //绑定的脚本对象
        public object Component
        {
            get { return m_Component; }
            set
            {
                string newLogic = value.ToString();
                if (m_LogicScript != newLogic)
                    m_LogicScript = newLogic;
                m_Component = value;
            }
        }

    }
}
