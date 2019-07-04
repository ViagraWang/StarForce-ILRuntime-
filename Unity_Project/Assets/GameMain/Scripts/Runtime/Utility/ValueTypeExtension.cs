using GameFramework;
using System;
using System.Collections.Generic;

namespace Game.Runtime
{
    public static class ValueTypeExtension
    {
        /// <summary>
        /// 根据类型名，获取热更新的类型全名
        /// </summary>
        /// <param name="typeName">类型名</param>
        /// <returns>热更新的类型全名</returns>
        public static string HotFixTypeFullName(this string typeName)
        {
            return Utility.Text.Format("{0}.{1}", HotfixComponent.c_HotfixNamespace, typeName);
        }

    }
}
