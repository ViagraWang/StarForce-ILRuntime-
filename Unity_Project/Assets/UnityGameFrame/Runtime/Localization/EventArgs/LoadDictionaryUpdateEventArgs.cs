﻿using GameFramework;
using GameFramework.Event;

namespace UnityGameFrame.Runtime
{
    /// <summary>
    /// 加载字典更新事件
    /// </summary>
    public sealed class LoadDictionaryUpdateEventArgs : GameEventArgs
    {
        /// <summary>
        /// 加载字典更新事件编号
        /// </summary>
        public static readonly int EventId = typeof(LoadDictionaryUpdateEventArgs).GetHashCode();

        /// <summary>
        /// 获取加载字典更新事件编号
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
        /// 获取加载字典进度
        /// </summary>
        public float Progress { get; private set; }

        /// <summary>
        /// 获取用户自定义数据
        /// </summary>
        public object UserData { get; private set; }

        public override void Clear()
        {
            DictionaryName = default(string);
            DictionaryAssetName = default(string);
            LoadType = default(LoadType);
            Progress = default(float);
            UserData = default(object);
        }

        /// <summary>
        /// 填充加载字典更新事件
        /// </summary>
        /// <param name="e">内部事件</param>
        /// <returns>加载字典更新事件</returns>
        public LoadDictionaryUpdateEventArgs Fill(GameFramework.Localization.LoadDictionaryUpdateEventArgs e)
        {
            LoadDictionaryInfo loadDictionaryInfo = (LoadDictionaryInfo)e.UserData;
            DictionaryName = loadDictionaryInfo.DictionaryName;
            DictionaryAssetName = e.DictionaryAssetName;
            LoadType = e.LoadType;
            Progress = e.Progress;
            UserData = loadDictionaryInfo.UserData;

            return this;
        }
    }
}
