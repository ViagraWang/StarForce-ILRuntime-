#if !UNITY_2018_1_OR_NEWER
using GameFramework.WebRequest;
using System;
using UnityEngine;

namespace UnityGameFrame.Runtime
{
    /// <summary>
    /// 默认 Web 请求代理辅助器
    /// </summary>
    public class WWWWebRequestAgentHelper : WebRequestAgentHelperBase, IDisposable
    {
        private WWW m_WWW = null;
        private bool m_Disposed = false;    //是否释放的标志位

        private EventHandler<WebRequestAgentHelperCompleteEventArgs> m_WebRequestAgentHelperCompleteEventHandler = null;
        private EventHandler<WebRequestAgentHelperErrorEventArgs> m_WebRequestAgentHelperErrorEventHandler = null;

        /// <summary>
        /// Web 请求代理辅助器完成事件
        /// </summary>
        public override event EventHandler<WebRequestAgentHelperCompleteEventArgs> WebRequestAgentHelperComplete
        {
            add { m_WebRequestAgentHelperCompleteEventHandler += value; }
            remove { m_WebRequestAgentHelperCompleteEventHandler -= value; }
        }

        /// <summary>
        /// Web 请求代理辅助器错误事件
        /// </summary>
        public override event EventHandler<WebRequestAgentHelperErrorEventArgs> WebRequestAgentHelperError
        {
            add { m_WebRequestAgentHelperErrorEventHandler += value; }
            remove { m_WebRequestAgentHelperErrorEventHandler -= value; }
        }

        /// <summary>
        /// 通过 Web 请求代理辅助器发送请求
        /// </summary>
        /// <param name="webRequestUri">要发送的远程地址</param>
        /// <param name="userData">用户自定义数据</param>
        public override void Request(string webRequestUri, object userData)
        {
            if (m_WebRequestAgentHelperCompleteEventHandler == null || m_WebRequestAgentHelperErrorEventHandler == null)
            {
                Log.Fatal("[DefaultWebRequestAgentHelper.Request] Web request agent helper handler is invalid.");
                return;
            }

            WWWFormInfo info = userData as WWWFormInfo;
            if (info.WWWForm == null)
                m_WWW = new WWW(webRequestUri); //没有表单数据，则直接请求
            else
                m_WWW = new WWW(webRequestUri, info.WWWForm);
        }

        /// <summary>
        /// 通过 Web 请求代理辅助器发送请求
        /// </summary>
        /// <param name="webRequestUri">要发送的远程地址</param>
        /// <param name="postData">要发送的数据流</param>
        /// <param name="userData">用户自定义数据</param>
        public override void Request(string webRequestUri, byte[] postData, object userData)
        {
            if (m_WebRequestAgentHelperCompleteEventHandler == null || m_WebRequestAgentHelperErrorEventHandler == null)
            {
                Log.Fatal("[DefaultWebRequestAgentHelper.Request] Web request agent helper handler is invalid.");
                return;
            }

            m_WWW = new WWW(webRequestUri, postData);
        }

        /// <summary>
        /// 重置 Web 请求代理辅助器
        /// </summary>
        public override void Reset()
        {
            if (m_WWW != null)
            {
                m_WWW.Dispose();
                m_WWW = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing">释放资源标记</param>
        private void Dispose(bool disposing)
        {
            if (m_Disposed)
            {
                return;
            }

            if (disposing)
            {
                if (m_WWW != null)
                {
                    m_WWW.Dispose();
                    m_WWW = null;
                }
            }

            m_Disposed = true;
        }

        void Update()
        {
            if (m_WWW == null || !m_WWW.isDone)
                return;

            if(!string.IsNullOrEmpty(m_WWW.error))
                m_WebRequestAgentHelperErrorEventHandler.Invoke(this, new WebRequestAgentHelperErrorEventArgs(m_WWW.error));
            else
                m_WebRequestAgentHelperCompleteEventHandler.Invoke(this, new WebRequestAgentHelperCompleteEventArgs(m_WWW.bytes));
        }
    }
}

#endif