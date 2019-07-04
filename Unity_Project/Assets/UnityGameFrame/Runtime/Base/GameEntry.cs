using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityGameFrame.Runtime
{
    /// <summary>
    /// 游戏入口
    /// </summary>
    public static class GameEntry
    {
        private static readonly Dictionary<Type, GameFrameworkComponent> s_GFComponents = new Dictionary<Type, GameFrameworkComponent>();

        /// <summary>
        /// 游戏框架所在的场景编号，即初始化场景
        /// </summary>
        internal const int GFSceneId = 0;

        /// <summary>
        /// 获取游戏框架组件
        /// </summary>
        /// <typeparam name="T">要获取的游戏框架组件类型</typeparam>
        /// <returns>要获取的游戏框架组件</returns>
        public static T GetComponent<T>() where T : GameFrameworkComponent
        {
            return GetComponent(typeof(T)) as T;
        }

        /// <summary>
        /// 获取游戏框架组件
        /// </summary>
        /// <param name="type">要获取的游戏框架组件类型</param>
        /// <returns>要获取的游戏框架组件</returns>
        public static GameFrameworkComponent GetComponent(Type type)
        {
            if (s_GFComponents.TryGetValue(type, out GameFrameworkComponent component))
                return component;
            return null;
        }


        /// <summary>
        /// 获取游戏框架组件
        /// </summary>
        /// <param name="typeName">要获取的游戏框架组件类型名称</param>
        /// <returns>要获取的游戏框架组件</returns>
        public static GameFrameworkComponent GetComponent(string typeName)
        {
            foreach (var item in s_GFComponents)
            {
                if (item.Key.FullName.Equals(typeName) || item.Value.name.Equals(typeName))
                    return item.Value;
            }

            return null;
        }

        /// <summary>
        /// 关闭游戏框架
        /// </summary>
        /// <param name="shutdownType">关闭游戏框架类型</param>
        public static void Shutdown(ShutdownType shutdownType)
        {
            Log.Info("[GameEntry.Shutdown] Shutdown Game Frame ({0})...", shutdownType.ToString());
            BaseComponent baseComponent = GetComponent<BaseComponent>();
            if(baseComponent != null)
            {
                baseComponent.Shutdown();
                baseComponent = null;
            }

            s_GFComponents.Clear(); //清空组件

            switch (shutdownType)
            {
                case ShutdownType.None:
                    break;
                case ShutdownType.Restart:
                    SceneManager.LoadScene(GFSceneId);
                    break;
                case ShutdownType.Quit:
                    Application.Quit();
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#endif
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 注册游戏框架组件
        /// </summary>
        /// <param name="component">要注册的游戏框架组件</param>
        internal static void RegisterComponent(GameFrameworkComponent component)
        {
            if (component == null)
            {
                Log.Error("[GameEntry.RegisterComponent] Game Framework component is invalid.");
                return;
            }

            Type type = component.GetType();
            if (s_GFComponents.ContainsKey(type))
            {
                Log.Error("[GameEntry.RegisterComponent] Game Framework component type '{0}' is already exist.", type.FullName);
                return;
            }

            s_GFComponents.Add(type, component);  //添加
        }

    }
}

