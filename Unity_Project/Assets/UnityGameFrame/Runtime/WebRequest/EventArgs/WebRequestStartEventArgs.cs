﻿using GameFramework.Event;

namespace UnityGameFrame.Runtime
{
    /// <summary>
    /// Web 请求开始事件
    /// </summary>
    public sealed class WebRequestStartEventArgs : GameEventArgs
    {

        /// <summary>
        /// Web 请求开始事件编号
        /// </summary>
        public static readonly int EventId = typeof(WebRequestStartEventArgs).GetHashCode();

        /// <summary>
        /// 获取 Web 请求开始事件编号
        /// </summary>
        public override int Id { get { return EventId; } }

        /// <summary>
        /// 获取 Web 请求任务的序列编号
        /// </summary>
        public int SerialId { get; private set; }

        /// <summary>
        /// 获取 Web 请求地址
        /// </summary>
        public string WebRequestUri { get; private set; }

        /// <summary>
        /// 获取用户自定义数据
        /// </summary>
        public object UserData { get; private set; }

        public override void Clear()
        {
            SerialId = default(int);
            WebRequestUri = default(string);
            UserData = default(object);
        }

        /// <summary>
        /// 填充 Web 请求开始事件
        /// </summary>
        /// <param name="e">内部事件</param>
        /// <returns>Web 请求开始事件</returns>
        public WebRequestStartEventArgs Fill(GameFramework.WebRequest.WebRequestStartEventArgs e)
        {
            WWWFormInfo wwwFormInfo = (WWWFormInfo)e.UserData;
            SerialId = e.SerialId;
            WebRequestUri = e.WebRequestUri;
            UserData = wwwFormInfo.UserData;

            return this;
        }
    }
}
