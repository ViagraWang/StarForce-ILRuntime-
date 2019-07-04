﻿using GameFramework;
using GameFramework.Event;
using System;

namespace UnityGameFrame.Runtime
{
    /// <summary>
    /// 加载数据表成功事件
    /// </summary>
    public sealed class LoadDataTableSuccessEventArgs : GameEventArgs
    {
        /// <summary>
        /// 加载数据表成功事件编号
        /// </summary>
        public static readonly int EventId = typeof(LoadDataTableSuccessEventArgs).GetHashCode();

        /// <summary>
        /// 获取加载数据表成功事件编号
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
        /// 获取加载持续时间
        /// </summary>
        public float Duration { get; private set; }

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
            Duration = default(float);
            UserData = default(object);
        }

        /// <summary>
        /// 填充加载数据表成功事件
        /// </summary>
        /// <param name="e">内部事件</param>
        /// <returns>加载数据表成功事件</returns>
        public LoadDataTableSuccessEventArgs Fill(GameFramework.DataTable.LoadDataTableSuccessEventArgs e)
        {
            LoadDataTableInfo info = e.UserData as LoadDataTableInfo;
            DataRowType = info.DataRowType;
            DataTableName = info.DataTableName;
            DataTableAssetName = e.DataTableAssetName;
            LoadType = e.LoadType;
            Duration = e.Duration;
            UserData = info.UserData;

            return this;
        }

    }
}
