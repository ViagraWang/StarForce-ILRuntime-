using GameFramework;
using System;
using UnityEngine;

namespace UnityGameFrame.Runtime
{
    /// <summary>
    /// 辅助器创建器相关的实用函数
    /// </summary>
    public static class Helper
    {
        /// <summary>
        /// 创建辅助器
        /// </summary>
        /// <typeparam name="T">要创建的辅助器类型</typeparam>
        /// <param name="helperTypeName">要创建的辅助器类型名称</param>
        /// <param name="customHelper">若要创建的辅助器类型为空时，使用的自定义辅助器类型</param>
        /// <param name="index">要创建的辅助器索引</param>
        /// <returns>创建的辅助器</returns>
        public static T CreateHelper<T>(string helperTypeName, T customHelper, int index = 0) where T : MonoBehaviour
        {
            T helper = null;
            if (!string.IsNullOrEmpty(helperTypeName))
            {
                Type helperType = Utility.Assembly.GetType(helperTypeName); //获取类型
                if(helperType == null)
                {
                    Log.Warning("[Helper.CreateHelper] Can not find helper type '{0}'.", helperTypeName);
                    return null;
                }

                if (!typeof(T).IsAssignableFrom(helperType))
                {
                    Log.Warning("[Helper.CreateHelper] Type '{0}' is not assignable from '{1}'.", typeof(T).FullName, helperType.FullName);
                    return null;
                }

                helper = new GameObject(helperTypeName).AddComponent(helperType) as T;
            }
            else if(customHelper == null)
            {
                Log.Warning("[Helper.CreateHelper] You must set custom helper with '{0}' type first.", typeof(T).FullName);
                return null;
            }
            else if (customHelper.gameObject.InScene()) //当前自定义的辅助器是否在场景中
            {
                helper = index > 0 ? UnityEngine.Object.Instantiate(customHelper) : customHelper;
            }
            else
            {
                helper = UnityEngine.Object.Instantiate(customHelper);  //不在场景中则直接创建
            }

            return helper;
        }

    }
}
