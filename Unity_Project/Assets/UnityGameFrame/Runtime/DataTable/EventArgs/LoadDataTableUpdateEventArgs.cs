using GameFramework;
using GameFramework.Event;
using System;

namespace UnityGameFrame.Runtime
{
    /// <summary>
    /// 加载数据表更新事件
    /// </summary>
    public sealed class LoadDataTableUpdateEventArgs : GameEventArgs
    {
        /// <summary>
        /// 加载数据表更新事件编号
        /// </summary>
        public static readonly int EventId = typeof(LoadDataTableUpdateEventArgs).GetHashCode();

        /// <summary>
        /// 获取加载数据表更新事件编号
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
        /// 获取数据表加载方式。
        /// </summary>
        public LoadType LoadType { get; private set; }

        /// <summary>
        /// 获取加载数据表进度
        /// </summary>
        public float Progress { get; private set; }

        /// <summary>
        /// 获取用户自定义数据
        /// </summary>
        public object UserData { get; private set; }

        public override void Clear()
        {
            DataRowType = default(Type);
            DataTableName = default(string);
            DataTableAssetName = default(string);
            LoadType = default(LoadType);
            Progress = default(float);
            UserData = default(object);
        }

        /// <summary>
        /// 填充加载数据表更新事件
        /// </summary>
        /// <param name="e">内部事件</param>
        /// <returns>加载数据表更新事件</returns>
        public LoadDataTableUpdateEventArgs Fill(GameFramework.DataTable.LoadDataTableUpdateEventArgs e)
        {
            LoadDataTableInfo info = e.UserData as LoadDataTableInfo;
            DataRowType = info.DataRowType;
            DataTableName = info.DataTableName;
            DataTableAssetName = e.DataTableAssetName;
            LoadType = e.LoadType;
            Progress = e.Progress;
            UserData = info.UserData;

            return this;
        }

    }
}
