﻿using GameFramework.Event;

namespace UnityGameFrame.Runtime
{
    /// <summary>
    /// 加载场景更新事件
    /// </summary>
    public sealed class LoadSceneUpdateEventArgs : GameEventArgs
    {
        /// <summary>
        /// 加载场景更新事件编号
        /// </summary>
        public static readonly int EventId = typeof(LoadSceneUpdateEventArgs).GetHashCode();

        /// <summary>
        /// 获取加载场景更新事件编号
        /// </summary>
        public override int Id { get { return EventId; } }

        /// <summary>
        /// 获取场景资源名称
        /// </summary>
        public string SceneAssetName { get; private set; }

        /// <summary>
        /// 获取加载场景进度
        /// </summary>
        public float Progress { get; private set; }

        /// <summary>
        /// 获取用户自定义数据
        /// </summary>
        public object UserData { get; private set; }

        public override void Clear()
        {
            SceneAssetName = default(string);
            Progress = default(float);
            UserData = default(object);
        }

        /// <summary>
        /// 填充加载场景更新事件
        /// </summary>
        /// <param name="e">内部事件</param>
        /// <returns>加载场景更新事件</returns>
        public LoadSceneUpdateEventArgs Fill(GameFramework.Scene.LoadSceneUpdateEventArgs e)
        {
            SceneAssetName = e.SceneAssetName;
            Progress = e.Progress;
            UserData = e.UserData;

            return this;
        }
    }
}
