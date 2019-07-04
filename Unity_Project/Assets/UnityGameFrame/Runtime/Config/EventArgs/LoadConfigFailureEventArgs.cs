using GameFramework;
using GameFramework.Event;
using System;

namespace UnityGameFrame.Runtime
{
    /// <summary>
    /// 加载配置失败事件
    /// </summary>
    public sealed class LoadConfigFailureEventArgs : GameEventArgs
    {

        /// <summary>
        /// 加载配置失败事件编号
        /// </summary>
        public static readonly int EventId = typeof(LoadConfigFailureEventArgs).GetHashCode();

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
        /// 获取错误信息
        /// </summary>
        public string ErrorMessage { get; private set; }

        /// <summary>
        /// 获取用户自定义数据
        /// </summary>
        public object UserData { get; private set; }

        /// <summary>
        /// 清理加载配置失败事件
        /// </summary>
        public override void Clear()
        {
            ConfigName = default(string);
            ConfigAssetName = default(string);
            LoadType = default(LoadType);
            ErrorMessage = default(string);
            UserData = default(object);
        }

        /// <summary>
        /// 填充加载配置失败事件
        /// </summary>
        /// <param name="e">框架内部事件</param>
        /// <returns>加载配置失败事件</returns>
        public LoadConfigFailureEventArgs Fill(GameFramework.Config.LoadConfigFailureEventArgs e)
        {
            LoadConfigInfo info = e.UserData as LoadConfigInfo; //内部的自定义数据为加载配置信息
            ConfigName = info.ConfigName;
            ConfigAssetName = e.ConfigAssetName;
            LoadType = e.LoadType;
            ErrorMessage = e.ErrorMessage;
            UserData = info.UserData;

            return this;
        }
    }
}
