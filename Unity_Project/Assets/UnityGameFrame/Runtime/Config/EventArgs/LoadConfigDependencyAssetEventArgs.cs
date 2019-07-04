using GameFramework.Event;
using System;

namespace UnityGameFrame.Runtime
{
    /// <summary>
    /// 加载配置时加载依赖资源事件
    /// </summary>
    public sealed class LoadConfigDependencyAssetEventArgs : GameEventArgs
    {

        /// <summary>
        /// 加载配置依赖资源事件编号
        /// </summary>
        public static readonly int EventId = typeof(LoadConfigDependencyAssetEventArgs).GetHashCode();

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
        /// 获取被加载的依赖资源名称
        /// </summary>
        public string DependencyAssetName { get; private set; }

        /// <summary>
        /// 获取当前已加载依赖资源数量
        /// </summary>
        public int LoadedCount { get; private set; }

        /// <summary>
        /// 获取总共加载依赖资源数量
        /// </summary>
        public int TotalCount { get; private set; }

        /// <summary>
        /// 获取用户自定义数据
        /// </summary>
        public object UserData { get; private set; }

        /// <summary>
        /// 清理加载配置依赖资源事件
        /// </summary>
        public override void Clear()
        {
            ConfigName = default(string);
            ConfigAssetName = default(string);
            DependencyAssetName = default(string);
            LoadedCount = default(int);
            TotalCount = default(int);
            UserData = default(object);
        }

        /// <summary>
        /// 填充加载配置依赖资源事件
        /// </summary>
        /// <param name="e">框架内部事件</param>
        /// <returns>加载配置依赖资源事件</returns>
        public LoadConfigDependencyAssetEventArgs Fill(GameFramework.Config.LoadConfigDependencyAssetEventArgs e)
        {
            LoadConfigInfo info = e.UserData as LoadConfigInfo; //内部的自定义数据为加载配置信息
            ConfigName = info.ConfigName;
            ConfigAssetName = e.ConfigAssetName;
            DependencyAssetName = e.DependencyAssetName;
            LoadedCount = e.LoadedCount;
            TotalCount = e.TotalCount;
            UserData = info.UserData;

            return this;
        }
    }
}
