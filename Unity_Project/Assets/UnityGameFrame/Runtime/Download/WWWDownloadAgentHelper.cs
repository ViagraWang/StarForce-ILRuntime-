﻿#if !UNITY_2018_3_OR_NEWER

using GameFramework;
using GameFramework.Download;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityGameFrame.Runtime
{
    /// <summary>
    /// 默认WWW下载代理辅助器
    /// </summary>
    public class WWWDownloadAgentHelper : DownloadAgentHelperBase, IDisposable
    {
        private WWW m_WWW = null;   //下载类
        private int m_LastDownloadedSize = 0;   //上次下载的大小，主要用来根据下载大小变化调用更新事件
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
                Log.Fatal("[DefaultDownloadAgentHelper.Download] Download agent helper handler is invalid.");
                return;
            }
            m_WWW = new WWW(downloadUri);
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
                Log.Fatal("[DefaultDownloadAgentHelper.Download] Download agent helper handler is invalid.");
                return;
            }

            Dictionary<string, string> header = new Dictionary<string, string>();
            header.Add("Range", Utility.Text.Format("bytes = {0} -", fromPosition));    //设置断点续传
            m_WWW = new WWW(downloadUri, null, header);
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
                Log.Fatal("[DefaultDownloadAgentHelper.Download] Download agent helper handler is invalid.");
                return;
            }

            Dictionary<string, string> header = new Dictionary<string, string>();
            header.Add("Range", Utility.Text.Format("bytes={0}-{1}", fromPosition.ToString(), toPosition.ToString()));
            m_WWW = new WWW(downloadUri, null, header);
        }

        /// <summary>
        /// 重置下载代理辅助器
        /// </summary>
        public override void Reset()
        {
            if(m_WWW != null)
            {
                m_WWW.Dispose();
                m_WWW = null;
            }
            m_LastDownloadedSize = 0;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);  //标记此对象，无需再次自动进行回收
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
                if (m_WWW != null)
                {
                    m_WWW.Dispose();
                    m_WWW = null;
                }
            }
            m_Disposed = true;
        }


        private void Update()
        {
            if (m_WWW == null)
                return;

            //更新下载增量
            int deltaLength = m_WWW.bytesDownloaded - m_LastDownloadedSize;
            if(deltaLength > 0)
            {
                m_LastDownloadedSize = m_WWW.bytesDownloaded;
                m_DownloadAgentHelperUpdateLengthEventHandler.Invoke(this, new DownloadAgentHelperUpdateLengthEventArgs(deltaLength));
            }

            if (!m_WWW.isDone)
                return;

            //下载完成
            if (!string.IsNullOrEmpty(m_WWW.error))
            {
                m_DownloadAgentHelperErrorEventHandler.Invoke(this, new DownloadAgentHelperErrorEventArgs(m_WWW.error));
            }
            else
            {
                byte[] bytes = m_WWW.bytes;
                m_DownloadAgentHelperUpdateBytesEventHandler.Invoke(this, new DownloadAgentHelperUpdateBytesEventArgs(bytes, 0, bytes.Length));
                m_DownloadAgentHelperCompleteEventHandler.Invoke(this, new DownloadAgentHelperCompleteEventArgs(bytes.Length));
            }

        }

    }
}

#endif