using System;
using System.Collections.Generic;
using UnityEditor;

namespace UnityGameFrame.Editor
{
    /// <summary>
    /// 游戏框架 Inspector 抽象类。
    /// </summary>
    public abstract class GameFrameworkInspector : UnityEditor.Editor
    {
        private bool m_IsCompiling = false; //是否正在编译的标志位

        //绘制检视面板
        public override void OnInspectorGUI()
        {
            if(m_IsCompiling && !EditorApplication.isCompiling)
            {
                m_IsCompiling = false;
                OnCompileComplete();
            }
            else if(!m_IsCompiling && EditorApplication.isCompiling)
            {
                m_IsCompiling = true;
                OnCompileStart();
            }
        }

        /// <summary>
        /// 编译开始事件。
        /// </summary>
        protected abstract void OnCompileStart();

        /// <summary>
        /// 编译完成事件。
        /// </summary>
        protected abstract void OnCompileComplete();

        protected bool IsPrefabInHierarchy(UnityEngine.Object obj)
        {
            if (obj == null)
                return false;

#if UNITY_2018_3_OR_NEWER
            return true;
#else
            return PrefabUtility.GetPrefabType(obj) != PrefabType.Prefab;
#endif
        }

    }
}
