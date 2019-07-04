﻿using GameFramework.Download;
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
    /// 使用 UnityWebRequest 实现的下载代理辅助器
    /// </summary>
    public partial class UnityWebRequestDownloadAgentHelper : DownloadAgentHelperBase, IDisposable
    {
        private readonly byte[] m_DownloadCache = new byte[1024 * 1024];   //下载缓存区

        private UnityWebRequest m_UnityWebRequest = null;    //下载请求
        private bool m_Disposed = false;    //是否释放的标志位

        private EventHandler<DownloadAgentHelperUpdateBytesEventArgs> m_DownloadAgentHelperUpdateBytesEventHandler = null;
        private EventHandler<DownloadAgentHelperUpdateLengthEventArgs> m_DownloadAgentHelperUpdateLengthEventHandler = null;
        private EventHandler<DownloadAgentHelperCompleteEventArgs> m_DownloadAgentHelperCompleteEventHandler = null;
        private EventHandler<DownloadAgentHelperErrorEventArgs> m_DownloadAgentHelperErrorEventHandler = null;

        /// <summary>
        /// 下载代理辅助器更新数据流事件
        /// </summary>
        public override event EventHandler<DownloadAgentHelperUpdateBytesEventArgs> DownloadAgentHelperUpdateBytes
        {
            add { m_DownloadAgentHelperUpdateBytesEventHandler += value; }
            remove { m_DownloadAgentHelperUpdateBytesEventHandler -= value; }
        }

        /// <summary>
        /// 下载代理辅助器更新数据大小事件
        /// </summary>
        public override event EventHandler<DownloadAgentHelperUpdateLengthEventArgs> DownloadAgentHelperUpdateLength
        {
            add { m_DownloadAgentHelperUpdateLengthEventHandler += value; }
            remove { m_DownloadAgentHelperUpdateLengthEventHandler -= value; }
        }

        /// <summary>
        /// 下载代理辅助器完成事件
        /// </summary>
        public override event EventHandler<DownloadAgentHelperCompleteEventArgs> DownloadAgentHelperComplete
        {
            add { m_DownloadAgentHelperCompleteEventHandler += value; }
            remove { m_DownloadAgentHelperCompleteEventHandler -= value; }
        }

        /// <summary>
        /// 下载代理辅助器错误事件
        /// </summary>
        public override event EventHandler<DownloadAgentHelperErrorEventArgs> DownloadAgentHelperError
        {
            add { m_DownloadAgentHelperErrorEventHandler += value; }
            remove { m_DownloadAgentHelperErrorEventHandler -= value; }
        }

        /// <summary>
        /// 通过下载代理辅助器下载指定地址的数据
        /// </summary>
        /// <param name="downloadUri">下载地址</param>
        /// <param name="userData">用户自定义数据</param>
        public override void Download(string downloadUri, object userData)
        {
            if (m_DownloadAgentHelperUpdateBytesEventHandler == null || m_DownloadAgentHelperUpdateLengthEventHandler == null || m_DownloadAgentHelperCompleteEventHandler == null || m_DownloadAgentHelperErrorEventHandler == null)
            {
                Log.Fatal("[UnityWebRequestDownloadAgentHelper.Download] Download agent helper handler is invalid.");
                return;
            }

            m_UnityWebRequest = new UnityWebRequest(downloadUri);
            m_UnityWebRequest.downloadHandler = new DownloadHandler(this);
#if UNITY_2017_2_OR_NEWER
            m_UnityWebRequest.SendWebRequest();
#else
            m_UnityWebRequest.Send();
#endif
        }

        /// <summary>
        /// 通过下载代理辅助器下载指定地址的数据
        /// </summary>
        /// <param name="downloadUri">下载地址</param>
        /// <param name="fromPosition">下载数据起始位置</param>
        /// <param name="userData">用户自定义数据</param>
        public override void Download(string downloadUri, int fromPosition, object userData)
        {
            if (m_DownloadAgentHelperUpdateBytesEventHandler == null || m_DownloadAgentHelperUpdateLengthEventHandler == null || m_DownloadAgentHelperCompleteEventHandler == null || m_DownloadAgentHelperErrorEventHandler == null)
            {
                Log.Fatal("[UnityWebRequestDownloadAgentHelper.Download] Download agent helper handler is invalid.");
                return;
            }

            m_UnityWebRequest = new UnityWebRequest(downloadUri);
            m_UnityWebRequest.SetRequestHeader("Range", Utility.Text.Format("bytes={0}-", fromPosition));
            m_UnityWebRequest.downloadHandler = new DownloadHandler(this);
#if UNITY_2017_2_OR_NEWER
            m_UnityWebRequest.SendWebRequest();
#else
            m_UnityWebRequest.Send();
#endif
        }

        /// <summary>
        /// 通过下载代理辅助器下载指定地址的数据
        /// </summary>
        /// <param name="downloadUri">下载地址</param>
        /// <param name="fromPosition">下载数据起始位置</param>
        /// <param name="toPosition">下载数据结束位置</param>
        /// <param name="userData">用户自定义数据</param>
        public override void Download(string downloadUri, int fromPosition, int toPosition, object userData)
        {
            if (m_DownloadAgentHelperUpdateBytesEventHandler == null || m_DownloadAgentHelperUpdateLengthEventHandler == null || m_DownloadAgentHelperCompleteEventHandler == null || m_DownloadAgentHelperErrorEventHandler == null)
            {
                Log.Fatal("[UnityWebRequestDownloadAgentHelper.Download] Download agent helper handler is invalid.");
                return;
            }

            m_UnityWebRequest = new UnityWebRequest(downloadUri);
            m_UnityWebRequest.SetRequestHeader("Range", Utility.Text.Format("bytes={0}-{1}", fromPosition.ToString(), toPosition.ToString()));
            m_UnityWebRequest.downloadHandler = new DownloadHandler(this);
#if UNITY_2017_2_OR_NEWER
            m_UnityWebRequest.SendWebRequest();
#else
            m_UnityWebRequest.Send();
#endif
        }

        /// <summary>
        /// 重置下载代理辅助器
        /// </summary>
        public override void Reset()
        {
            if (m_UnityWebRequest != null)
            {
                m_UnityWebRequest.Abort();
                m_UnityWebRequest.Dispose();
                m_UnityWebRequest = null;
            }

        }

        /// <summary>
        /// 释放资源
        /// </summary>
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
                return;

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

        private void Update()
        {
            if (m_UnityWebRequest == null)
                return;

            if (!m_UnityWebRequest.isDone)
                return;

#if UNITY_2017_1_OR_NEWER
            if(m_UnityWebRequest.isNetworkError)
#else
            if(m_WebRequest.isError)
#endif
            {
                m_DownloadAgentHelperErrorEventHandler.Invoke(this, new DownloadAgentHelperErrorEventArgs(m_UnityWebRequest.error));
            }
            else
            {
                m_DownloadAgentHelperCompleteEventHandler.Invoke(this, new DownloadAgentHelperCompleteEventArgs((int)m_UnityWebRequest.downloadedBytes));

            }

        }

    }
}
