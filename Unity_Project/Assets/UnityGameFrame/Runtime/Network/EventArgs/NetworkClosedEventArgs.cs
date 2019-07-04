﻿using GameFramework.Event;
using GameFramework.Network;

namespace UnityGameFrame.Runtime
{
    /// <summary>
    /// 网络连接关闭事件
    /// </summary>
    public sealed class NetworkClosedEventArgs : GameEventArgs
    {
        /// <summary>
        /// 网络连接关闭事件编号
        /// </summary>
        public static readonly int EventId = typeof(NetworkClosedEventArgs).GetHashCode();

        /// <summary>
        /// 获取网络连接关闭事件编号
        /// </summary>
        public override int Id { get { return EventId; } }

        /// <summary>
        /// 获取网络频道
        /// </summary>
        public INetworkChannel NetworkChannel { get; private set; }


        public override void Clear()
        {
            NetworkChannel = default(INetworkChannel);
        }

        /// <summary>
        /// 填充网络连接关闭事件
        /// </summary>
        /// <param name="e">内部事件</param>
        /// <returns>网络连接关闭事件</returns>
        public NetworkClosedEventArgs Fill(GameFramework.Network.NetworkClosedEventArgs e)
        {
            NetworkChannel = e.NetworkChannel;

            return this;
        }
    }
}
