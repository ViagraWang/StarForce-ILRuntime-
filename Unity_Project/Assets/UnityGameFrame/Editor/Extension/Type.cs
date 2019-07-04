using GameFramework;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace UnityGameFrame.Editor
{
    /// <summary>
    /// 类型相关的实用函数。
    /// </summary>
    internal static class Type
    {
        /// <summary>
        /// 程序集名称
        /// </summary>
        private static readonly string[] AssemblyNames = 
            {
#if UNITY_2017_3_OR_NEWER
            "UnityGameFrame.Runtime",
            "Game.Runtime",
#else
            "Assembly-CSharp"
#endif
            };

        /// <summary>
        /// 编辑器程序集
        /// </summary>
        private static readonly string[] EditorAssemblyNames =
        {
#if UNITY_2017_3_OR_NEWER
            "UnityGameFrame.Editor",
            "Game.Editor",
#else
            "Assembly-CSharp-Editor"
#endif
            
        };

        /// <summary>
        /// 获取配置路径
        /// </summary>
        /// <typeparam name="T">配置类型</typeparam>
        /// <returns>配置路径</returns>
        internal static string GetConfigurationPath<T>() where T : ConfigPathAttribute
        {
            foreach (var type in Utility.Assembly.GetTypes())
            {
                if (!type.IsAbstract || !type.IsSealed) //抽象类型和不可继承类型
                    continue;

                //遍历查找字段
                foreach (FieldInfo fieldInfo in type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
                {
                    if (fieldInfo.FieldType == typeof(string) && fieldInfo.IsDefined(typeof(T), false))
                        return fieldInfo.GetValue(null) as string;
                }

                //遍历查找属性
                foreach (PropertyInfo propertyInfo in type.GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
                {
                    if (propertyInfo.PropertyType == typeof(string) && propertyInfo.IsDefined(typeof(T), false))
                        return propertyInfo.GetValue(null, null) as string;
                }
            }

            return null;
        }

        /// <summary>
        /// 获取非编辑器中指定基类的所有子类的名称
        /// </summary>
        /// <param name="typeBase">基类类型</param>
        /// <returns>指定基类的所有子类的名称</returns>
        internal static string[] GetRunSubClassNames(System.Type typeBase)
        {
            return GetSubClassNames(typeBase, AssemblyNames);
        }

        /// <summary>
        /// 获取编辑器中指定基类的所有子类的名称。
        /// </summary>
        /// <param name="typeBase">基类类型。</param>
        /// <returns>指定基类的所有子类的名称。</returns>
        internal static string[] GetEditorSubClassNames(System.Type typeBase)
        {
            return GetSubClassNames(typeBase, EditorAssemblyNames);
        }

        //内部获取子类名称
        private static string[] GetSubClassNames(System.Type typeBase, string[] assemblyNames)
        {
            List<string> typeNames = new List<string>();
            foreach (string assemblyName in assemblyNames)
            {
                Assembly assembly = null;
                try
                {
                    assembly = Assembly.Load(assemblyName);
                }
                catch (Exception e)
                {
                    Debug.LogWarning("获取类型异常 -> " + e.ToString());
                    continue;
                }

                if (assembly == null)
                    continue;

                System.Type[] types = assembly.GetTypes();  //获取所有类型
                foreach (System.Type type in types)
                {
                    if (type.IsClass && !type.IsAbstract && typeBase.IsAssignableFrom(type))
                    {
                        typeNames.Add(type.FullName);
                    }
                }
            }

            typeNames.Sort();
            return typeNames.ToArray();
        }

    }
}
