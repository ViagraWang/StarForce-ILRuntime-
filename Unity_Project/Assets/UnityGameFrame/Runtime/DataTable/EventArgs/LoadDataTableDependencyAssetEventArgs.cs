using GameFramework.Event;
using System;

namespace UnityGameFrame.Runtime
{
    /// <summary>
    /// 加载数据表依赖资源事件
    /// </summary>
    public sealed class LoadDataTableDependencyAssetEventArgs : GameEventArgs
    {
        /// <summary>
        /// 加载数据表依赖资源事件编号
        /// </summary>
        public static readonly int EventId = typeof(LoadDataTableDependencyAssetEventArgs).GetHashCode();

        /// <summary>
        /// 获取加载数据表依赖资源事件编号
        /// </summary>
        public override int Id { get { return EventId; } }

        /// <summary>
        /// 获取数据表行的类型
        /// </summary>
        public Type DataRowType { get; private set; }

        /// <summary>
        /// 获取数据表名称
        /// </summary>
        public string DataTableName { get; private set; }

        /// <summary>
        /// 获取数据表资源名称
        /// </summary>
        public string DataTableAssetName { get; private set; }

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

        public override void Clear()
        {
            DataRowType = default(Type);
            DataTableName = default(string);
            DataTableAssetName = default(string);
            DependencyAssetName = default(string);
            LoadedCount = default(int);
            TotalCount = default(int);
            UserData = default(object);
        }

        /// <summary>
        /// 填充加载数据表依赖资源事件
        /// </summary>
        /// <param name="e">内部事件</param>
        /// <returns>加载数据表依赖资源事件</returns>
        public LoadDataTableDependencyAssetEventArgs Fill(GameFramework.DataTable.LoadDataTableDependencyAssetEventArgs e)
        {
            LoadDataTableInfo info = e.UserData as LoadDataTableInfo;
            DataRowType = info.DataRowType;
            DataTableName = info.DataTableName;
            DataTableAssetName = e.DataTableAssetName;
            DependencyAssetName = e.DependencyAssetName;
            LoadedCount = e.LoadedCount;
            TotalCount = e.TotalCount;
            UserData = info.UserData;

            return this;
        }

    }
}
