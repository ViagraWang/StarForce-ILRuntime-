using GameFramework.WebRequest;
using System;
#if UNITY_5_4_OR_NEWER
using UnityEngine.Networking;
#else
using UnityEngine.Experimental.Networking;
#endif
using Utility = GameFramework.Utility;

namespace UnityGameFrame.Runtime
{
    /// <summary>
    /// 使用 UnityWebRequest 实现的 Web 请求代理辅助器
    /// </summary>
    class UnityWebRequestAgentHelper : WebRequestAgentHelperBase, IDisposable
    {
        private UnityWebRequest m_UnityWebRequest = null;
        private bool m_Disposed = false;

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

            WWWFormInfo wwwFormInfo = userData as WWWFormInfo;
            if (wwwFormInfo.WWWForm == null)
                m_UnityWebRequest = UnityWebRequest.Get(webRequestUri);
            else
                m_UnityWebRequest = UnityWebRequest.Post(webRequestUri, wwwFormInfo.WWWForm);

#if UNITY_2017_2_OR_NEWER
            m_UnityWebRequest.SendWebRequest();
#else
            m_UnityWebRequest.Send();
#endif
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

            m_UnityWebRequest = UnityWebRequest.Post(webRequestUri, Utility.Converter.GetString(postData));
#if UNITY_2017_2_OR_NEWER
            m_UnityWebRequest.SendWebRequest();
#else
            m_UnityWebRequest.Send();
#endif
        }

        public override void Reset()
        {
            if (m_UnityWebRequest != null)
            {
                m_UnityWebRequest.Dispose();
                m_UnityWebRequest = null;
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
                if (m_UnityWebRequest != null)
                {
                    m_UnityWebRequest.Dispose();
                    m_UnityWebRequest = null;
                }
            }

            m_Disposed = true;
        }

        void Update()
        {
            if (m_UnityWebRequest == null || !m_UnityWebRequest.isDone)
            {
                return;
            }

            bool isError = false;
#if UNITY_2017_1_OR_NEWER
            isError = m_UnityWebRequest.isNetworkError;
#else
            isError = m_UnityWebRequest.isError;
#endif

            if (isError)
            {
                m_WebRequestAgentHelperErrorEventHandler.Invoke(this, new WebRequestAgentHelperErrorEventArgs(m_UnityWebRequest.error));
            }
            else if (m_UnityWebRequest.downloadHandler.isDone)
            {
                m_WebRequestAgentHelperCompleteEventHandler.Invoke(this, new WebRequestAgentHelperCompleteEventArgs(m_UnityWebRequest.downloadHandler.data));
            }
        }

    }
}
