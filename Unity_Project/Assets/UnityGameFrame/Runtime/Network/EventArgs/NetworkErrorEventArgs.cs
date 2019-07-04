﻿using GameFramework.Event;
using GameFramework.Network;
using System.Net.Sockets;

namespace UnityGameFrame.Runtime
{
    /// <summary>
    /// 网络错误事件
    /// </summary>
    public sealed class NetworkErrorEventArgs : GameEventArgs
    {
        /// <summary>
        /// 网络连接错误事件编号
        /// </summary>
        public static readonly int EventId = typeof(NetworkErrorEventArgs).GetHashCode();

        /// <summary>
        /// 获取网络错误事件编号
        /// </summary>
        public override int Id { get { return EventId; } }

        /// <summary>
        /// 获取网络频道
        /// </summary>
        public INetworkChannel NetworkChannel { get; private set; }

        /// <summary>
        /// 获取错误码
        /// </summary>
        public NetworkErrorCode ErrorCode { get; private set; }

        /// <summary>
        /// 获取 Socket 错误码。
        /// </summary>
        public SocketError SocketErrorCode { get; private set; }

        /// <summary>
        /// 获取错误信息
        /// </summary>
        public string ErrorMessage { get; private set; }

        public override void Clear()
        {
            NetworkChannel = default(INetworkChannel);
            ErrorCode = default(NetworkErrorCode);
            SocketErrorCode = default(SocketError);
            ErrorMessage = default(string);
        }

        /// <summary>
        /// 填充网络错误事件
        /// </summary>
        /// <param name="e">内部事件</param>
        /// <returns>网络错误事件</returns>
        public NetworkErrorEventArgs Fill(GameFramework.Network.NetworkErrorEventArgs e)
        {
            NetworkChannel = e.NetworkChannel;
            ErrorCode = e.ErrorCode;
            SocketErrorCode = e.SocketErrorCode;
            ErrorMessage = e.ErrorMessage;

            return this;
        }
    }
}
