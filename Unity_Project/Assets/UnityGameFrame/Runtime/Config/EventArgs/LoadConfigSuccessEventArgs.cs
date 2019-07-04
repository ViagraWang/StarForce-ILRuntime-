using GameFramework;
using GameFramework.Event;
using System;

namespace UnityGameFrame.Runtime
{
    /// <summary>
    /// 加载配置成功事件
    /// </summary>
    public sealed class LoadConfigSuccessEventArgs : GameEventArgs
    {

        /// <summary>
        /// 加载配置成功事件编号
        /// </summary>
        public static readonly int EventId = typeof(LoadConfigSuccessEventArgs).GetHashCode();

        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        /// <summary>
        /// 配置名称
        /// </summary>
        public string ConfigName { get; private set; }

        /// <summary>
        /// 获取配置资源名称
        /// </summary>
        public string ConfigAssetName { get; private set; }

        /// <summary>
        /// 获取配置加载方式。
        /// </summary>
        public LoadType LoadType { get; private set; }

        /// <summary>
        /// 获取加载持续时间
        /// </summary>
        public float Duration { get; private set; }

        /// <summary>
        /// 获取用户自定义数据
        /// </summary>
        public object UserData { get; private set; }

        /// <summary>
        /// 清理加载配置成功事件
        /// </summary>
        public override void Clear()
        {
            ConfigName = default(string);
            ConfigAssetName = default(string);
            LoadType = default(LoadType);
            Duration = default(float);
            UserData = default(object);
        }

        /// <summary>
        /// 填充加载配置成功事件
        /// </summary>
        /// <param name="e">框架内部事件</param>
        /// <returns>加载配置成功事件</returns>
        public LoadConfigSuccessEventArgs Fill(GameFramework.Config.LoadConfigSuccessEventArgs e)
        {
            LoadConfigInfo info = e.UserData as LoadConfigInfo; //内部的自定义数据为加载配置信息
            ConfigName = info.ConfigName;
            ConfigAssetName = e.ConfigAssetName;
            LoadType = e.LoadType;
            Duration = e.Duration;
            UserData = info.UserData;

            return this;
        }
    }
}
