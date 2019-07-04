﻿using GameFramework;
using GameFramework.Event;

namespace UnityGameFrame.Runtime
{
    /// <summary>
    /// 加载字典失败事件
    /// </summary>
    public sealed class LoadDictionaryFailureEventArgs : GameEventArgs
    {
        /// <summary>
        /// 加载字典失败事件编号
        /// </summary>
        public static readonly int EventId = typeof(LoadDictionaryFailureEventArgs).GetHashCode();

        /// <summary>
        /// 获取加载字典失败事件编号
        /// </summary>
        public override int Id { get { return EventId; } }

        /// <summary>
        /// 获取字典名称
        /// </summary>
        public string DictionaryName { get; private set; }

        /// <summary>
        /// 获取字典资源名称
        /// </summary>
        public string DictionaryAssetName { get; private set; }

        /// <summary>
        /// 获取字典加载方式。
        /// </summary>
        public LoadType LoadType { get; private set; }

        /// <summary>
        /// 获取错误信息
        /// </summary>
        public string ErrorMessage { get; private set; }

        /// <summary>
        /// 获取用户自定义数据
        /// </summary>
        public object UserData { get; private set; }


        public override void Clear()
        {
            DictionaryName = default(string);
            DictionaryAssetName = default(string);
            LoadType = default(LoadType);
            ErrorMessage = default(string);
            UserData = default(object);
        }

        /// <summary>
        /// 填充加载字典失败事件
        /// </summary>
        /// <param name="e">内部事件</param>
        /// <returns>加载字典失败事件</returns>
        public LoadDictionaryFailureEventArgs Fill(GameFramework.Localization.LoadDictionaryFailureEventArgs e)
        {
            LoadDictionaryInfo loadDictionaryInfo = (LoadDictionaryInfo)e.UserData;
            DictionaryName = loadDictionaryInfo.DictionaryName;
            DictionaryAssetName = e.DictionaryAssetName;
            LoadType = e.LoadType;
            ErrorMessage = e.ErrorMessage;
            UserData = loadDictionaryInfo.UserData;

            return this;
        }
    }
}
