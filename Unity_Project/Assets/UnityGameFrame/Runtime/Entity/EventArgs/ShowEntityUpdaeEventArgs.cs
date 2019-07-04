using GameFramework.Event;
using System;

namespace UnityGameFrame.Runtime
{
    /// <summary>
    /// 显示实体更新事件
    /// </summary>
    public sealed class ShowEntityUpdateEventArgs : GameEventArgs
    {
        /// <summary>
        /// 显示实体更新事件编号
        /// </summary>
        public static readonly int EventId = typeof(ShowEntityUpdateEventArgs).GetHashCode();

        /// <summary>
        /// 获取显示实体更新事件编号
        /// </summary>
        public override int Id { get { return EventId; } }

        /// <summary>
        /// 获取实体编号
        /// </summary>
        public int EntityId { get; private set; }

        /// <summary>
        /// 获取实体逻辑类型
        /// </summary>
        public Type EntityLogicType { get; private set; }

        /// <summary>
        /// 获取实体资源名称
        /// </summary>
        public string EntityAssetName { get; private set; }

        /// <summary>
        /// 获取实体组名称
        /// </summary>
        public string EntityGroupName { get; private set; }

        /// <summary>
        /// 获取显示实体进度
        /// </summary>
        public float Progress { get; private set; }

        /// <summary>
        /// 获取用户自定义数据
        /// </summary>
        public object UserData { get; private set; }


        public override void Clear()
        {
            EntityId = default(int);
            EntityLogicType = null;
            EntityAssetName = default(string);
            EntityGroupName = default(string);
            Progress = default(float);
            UserData = default(object);
        }

        /// <summary>
        /// 填充显示实体更新事件
        /// </summary>
        /// <param name="e">内部事件</param>
        /// <returns>显示实体更新事件</returns>
        public ShowEntityUpdateEventArgs Fill(GameFramework.Entity.ShowEntityUpdateEventArgs e)
        {
            ShowEntityInfo showEntityInfo = e.UserData as ShowEntityInfo;
            EntityId = e.EntityId;
            EntityLogicType = showEntityInfo.EntityLogicType;
            EntityAssetName = e.EntityAssetName;
            EntityGroupName = e.EntityGroupName;
            Progress = e.Progress;
            UserData = showEntityInfo.UserData;

            return this;
        }
    }
}
