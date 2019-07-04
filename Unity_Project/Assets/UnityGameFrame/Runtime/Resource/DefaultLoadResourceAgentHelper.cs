﻿using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using GameFramework.Resource;
#if UNITY_5_4_OR_NEWER
using UnityEngine.Networking;
#endif
using Utility = GameFramework.Utility;

namespace UnityGameFrame.Runtime
{
    /// <summary>
    /// 默认加载资源代理辅助器
    /// </summary>
    public class DefaultLoadResourceAgentHelper : LoadResourceAgentHelperBase, IDisposable
    {
        private string m_FileFullPath = null;   //文件全路径
        private string m_BytesFullPath = null;  //字节流全路径
        private int m_LoadType = 0; //加载类型
        private string m_AssetName = null;  //要加载的子资源名
        private float m_LastProgress = 0f;   //上一阶段的进度
        private bool m_Disposed = false;    //释放的标志位
#if UNITY_5_4_OR_NEWER
        private UnityWebRequest m_UnityWebRequest = null;
#else
        private WWW m_WWW = null;
#endif
        private AssetBundleCreateRequest m_FileAssetBundleCreateRequest = null; //文件中异步创建Bundle
        private AssetBundleCreateRequest m_BytesAssetBundleCreateRequest = null; //字节流中异步创建Bundle
        private AssetBundleRequest m_AssetBundleRequest = null; //Bundle中异步加载资源
        private AsyncOperation m_AsyncOperation = null; //加载场景的异步操作

        private EventHandler<LoadResourceAgentHelperUpdateEventArgs> m_LoadResourceAgentHelperUpdateEventHandler = null;
        private EventHandler<LoadResourceAgentHelperReadFileCompleteEventArgs> m_LoadResourceAgentHelperReadFileCompleteEventHandler = null;
        private EventHandler<LoadResourceAgentHelperReadBytesCompleteEventArgs> m_LoadResourceAgentHelperReadBytesCompleteEventHandler = null;
        private EventHandler<LoadResourceAgentHelperParseBytesCompleteEventArgs> m_LoadResourceAgentHelperParseBytesCompleteEventHandler = null;
        private EventHandler<LoadResourceAgentHelperLoadCompleteEventArgs> m_LoadResourceAgentHelperLoadCompleteEventHandler = null;
        private EventHandler<LoadResourceAgentHelperErrorEventArgs> m_LoadResourceAgentHelperErrorEventHandler = null;

        /// <summary>
        /// 加载资源代理辅助器异步加载资源更新事件
        /// </summary>
        public override event EventHandler<LoadResourceAgentHelperUpdateEventArgs> LoadResourceAgentHelperUpdate
        {
            add { m_LoadResourceAgentHelperUpdateEventHandler += value; }
            remove { m_LoadResourceAgentHelperUpdateEventHandler -= value; }
        }

        /// <summary>
        /// 加载资源代理辅助器异步读取资源文件完成事件
        /// </summary>
        public override event EventHandler<LoadResourceAgentHelperReadFileCompleteEventArgs> LoadResourceAgentHelperReadFileComplete
        {
            add { m_LoadResourceAgentHelperReadFileCompleteEventHandler += value; }
            remove { m_LoadResourceAgentHelperReadFileCompleteEventHandler -= value; }
        }

        /// <summary>
        /// 加载资源代理辅助器异步读取资源二进制流完成事件
        /// </summary>
        public override event EventHandler<LoadResourceAgentHelperReadBytesCompleteEventArgs> LoadResourceAgentHelperReadBytesComplete
        {
            add { m_LoadResourceAgentHelperReadBytesCompleteEventHandler += value; }
            remove { m_LoadResourceAgentHelperReadBytesCompleteEventHandler -= value; }
        }

        /// <summary>
        /// 加载资源代理辅助器异步将资源二进制流转换为加载对象完成事件
        /// </summary>
        public override event EventHandler<LoadResourceAgentHelperParseBytesCompleteEventArgs> LoadResourceAgentHelperParseBytesComplete
        {
            add { m_LoadResourceAgentHelperParseBytesCompleteEventHandler += value; }
            remove { m_LoadResourceAgentHelperParseBytesCompleteEventHandler -= value; }
        }

        /// <summary>
        /// 加载资源代理辅助器异步加载资源完成事件
        /// </summary>
        public override event EventHandler<LoadResourceAgentHelperLoadCompleteEventArgs> LoadResourceAgentHelperLoadComplete
        {
            add { m_LoadResourceAgentHelperLoadCompleteEventHandler += value; }
            remove { m_LoadResourceAgentHelperLoadCompleteEventHandler -= value; }
        }

        /// <summary>
        /// 加载资源代理辅助器错误事件
        /// </summary>
        public override event EventHandler<LoadResourceAgentHelperErrorEventArgs> LoadResourceAgentHelperError
        {
            add { m_LoadResourceAgentHelperErrorEventHandler += value; }
            remove { m_LoadResourceAgentHelperErrorEventHandler -= value; }
        }

        /// <summary>
        /// 通过加载资源代理辅助器开始异步加载资源
        /// </summary>
        /// <param name="resource">资源包</param>
        /// <param name="assetName">要加载的资源名</param>
        /// <param name="assetType">要加载资源的类型</param>
        /// <param name="isScene">要加载的资源是否是场景</param>
        public override void LoadAsset(object resource, string assetName, Type assetType, bool isScene)
        {
            if (m_LoadResourceAgentHelperLoadCompleteEventHandler == null || 
                m_LoadResourceAgentHelperUpdateEventHandler == null || 
                m_LoadResourceAgentHelperErrorEventHandler == null)
            {
                Log.Fatal("[DefaultLoadResourceAgentHelper.LoadAsset] Load resource agent helper handler is invalid.");
                return;
            }
            //检查Bundle
            AssetBundle assetBundle = resource as AssetBundle;
            if(assetBundle == null)
            {
                m_LoadResourceAgentHelperErrorEventHandler.Invoke(this, new LoadResourceAgentHelperErrorEventArgs(LoadResourceStatus.TypeError, "Can not load asset bundle from loaded resource which is not an asset bundle."));
                return;
            }
            //检查资源名称
            if (string.IsNullOrEmpty(assetName))
            {
                m_LoadResourceAgentHelperErrorEventHandler.Invoke(this, new LoadResourceAgentHelperErrorEventArgs(LoadResourceStatus.AssetError, "Can not load asset from asset bundle which child name is invalid."));
                return;
            }

            m_AssetName = assetName;
            if (isScene)    //检查是否是场景资源
            {
                int sceneNamePositionStart = assetName.LastIndexOf('/');
                int sceneNamePositionEnd = assetName.LastIndexOf('.');
                if(sceneNamePositionStart <= 0 || sceneNamePositionEnd <= 0 || sceneNamePositionStart > sceneNamePositionEnd)
                {
                    m_LoadResourceAgentHelperErrorEventHandler.Invoke(this, new LoadResourceAgentHelperErrorEventArgs(LoadResourceStatus.AssetError, Utility.Text.Format("Scene name '{0}' is invalid.", assetName)));
                    return;
                }
                string sceneName = assetName.Substring(sceneNamePositionStart +1, sceneNamePositionEnd - sceneNamePositionStart - 1);   //场景名
                m_AsyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            }
            else
            {
                if (assetType != null)
                    m_AssetBundleRequest = assetBundle.LoadAssetAsync(assetName, assetType);
                else
                    m_AssetBundleRequest = assetBundle.LoadAssetAsync(assetName);
            }
        }

        /// <summary>
        /// 通过加载资源代理辅助器开始异步将资源二进制流转换为加载对象
        /// </summary>
        /// <param name="bytes">要加载资源的二进制流</param>
        public override void ParseBytes(byte[] bytes)
        {
            if (m_LoadResourceAgentHelperParseBytesCompleteEventHandler == null || 
                m_LoadResourceAgentHelperUpdateEventHandler == null || 
                m_LoadResourceAgentHelperErrorEventHandler == null)
            {
                Log.Fatal("[DefaultLoadResourceAgentHelper.ParseBytes] Load resource agent helper handler is invalid.");
                return;
            }

            m_BytesAssetBundleCreateRequest = AssetBundle.LoadFromMemoryAsync(bytes);
        }

        /// <summary>
        /// 通过加载资源代理辅助器开始异步读取资源二进制流
        /// </summary>
        /// <param name="fullPath">要加载资源的完整路径名</param>
        /// <param name="loadType">资源加载方式</param>
        public override void ReadBytes(string fullPath, int loadType)
        {
            if (m_LoadResourceAgentHelperReadBytesCompleteEventHandler == null ||
                m_LoadResourceAgentHelperUpdateEventHandler == null || 
                m_LoadResourceAgentHelperErrorEventHandler == null)
            {
                Log.Fatal("[DefaultLoadResourceAgentHelper.ReadBytes] Load resource agent helper handler is invalid.");
                return;
            }

            m_BytesFullPath = fullPath;
            m_LoadType = loadType;
#if UNITY_5_4_OR_NEWER
            m_UnityWebRequest = UnityWebRequest.Get(Utility.Path.GetRemotePath(fullPath));
#if UNITY_2017_2_OR_NEWER
            m_UnityWebRequest.SendWebRequest();
#else
            m_UnityWebRequest.Send();
#endif
#else
            m_WWW = new WWW(Utility.Path.GetRemotePath(fullPath));
#endif
        }

        /// <summary>
        /// 通过加载资源代理辅助器开始异步读取资源文件
        /// </summary>
        /// <param name="fullPath">要加载资源的完整路径名</param>
        public override void ReadFile(string fullPath)
        {
            if(m_LoadResourceAgentHelperReadFileCompleteEventHandler == null || 
                m_LoadResourceAgentHelperUpdateEventHandler == null || 
                m_LoadResourceAgentHelperErrorEventHandler == null)
            {
                Log.Fatal("[DefaultLoadResourceAgentHelper.ReadFile] Load resource agent helper handler is invalid.");
                return;
            }
            m_FileFullPath = fullPath;
            m_FileAssetBundleCreateRequest = AssetBundle.LoadFromFileAsync(m_FileFullPath);
        }

        /// <summary>
        /// 重置加载资源代理辅助器
        /// </summary>
        public override void Reset()
        {
            m_FileFullPath = null;
            m_BytesFullPath = null;
            m_LoadType = 0;
            m_AssetName = null;
            m_LastProgress = 0f;
#if UNITY_5_4_OR_NEWER
            if (m_UnityWebRequest != null)
            {
                m_UnityWebRequest.Dispose();
                m_UnityWebRequest = null;
            }
#else
            if (m_WWW != null)
            {
                m_WWW.Dispose();
                m_WWW = null;
            }
#endif

            m_FileAssetBundleCreateRequest = null;
            m_BytesAssetBundleCreateRequest = null;
            m_AssetBundleRequest = null;
            m_AsyncOperation = null;
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
            {
                return;
            }

            if (disposing)
            {
#if UNITY_5_4_OR_NEWER
                if (m_UnityWebRequest != null)
                {
                    m_UnityWebRequest.Dispose();
                    m_UnityWebRequest = null;
                }
#else
                if (m_WWW != null)
                {
                    m_WWW.Dispose();
                    m_WWW = null;
                }
#endif
            }

            m_Disposed = true;
        }

        private void Update()
        {
#if UNITY_5_4_OR_NEWER
            UpdateUnityWebRequest();
#else
            UpdateWWW();
#endif
            UpdateFileAssetBundleCreateRequest();
            UpdateBytesAssetBundleCreateRequest();
            UpdateAssetBundleRequest(); //更新加载资源异步操作
            UpdateAsyncOperation(); //更新加载场景异步操作
        }

#if UNITY_5_4_OR_NEWER

        private void UpdateUnityWebRequest()
        {
            if(m_UnityWebRequest != null)
            {
                if (m_UnityWebRequest.isDone)
                {
                    if (string.IsNullOrEmpty(m_UnityWebRequest.error))
                    {
                        m_LoadResourceAgentHelperReadBytesCompleteEventHandler.Invoke(this, new LoadResourceAgentHelperReadBytesCompleteEventArgs(m_UnityWebRequest.downloadHandler.data, m_LoadType));
                        m_UnityWebRequest.Dispose();
                        m_UnityWebRequest = null;
                        m_BytesFullPath = null;
                        m_LoadType = 0;
                        m_LastProgress = 0f;
                    }
                    else
                    {
                        bool isError = false;
#if UNITY_2017_1_OR_NEWER
                        isError = m_UnityWebRequest.isNetworkError;
#else
                        isError = m_UnityWebRequest.isError;
#endif
                        m_LoadResourceAgentHelperErrorEventHandler(this, new LoadResourceAgentHelperErrorEventArgs(LoadResourceStatus.NotExist, Utility.Text.Format("Can not load asset bundle '{0}' with error message '{1}'.", m_BytesFullPath, isError ? m_UnityWebRequest.error : null)));
                    }
                }
                else if(m_UnityWebRequest.downloadProgress != m_LastProgress)
                {
                    m_LastProgress = m_UnityWebRequest.downloadProgress;
                    m_LoadResourceAgentHelperUpdateEventHandler(this, new LoadResourceAgentHelperUpdateEventArgs(LoadResourceProgress.ReadResource, m_UnityWebRequest.downloadProgress));
                }
            }
        }

#else
        
        private void UpdateWWW()
        {
            if(m_WWW != null)
            {
                if (m_WWW.isDone)
                {
                    if (string.IsNullOrEmpty(m_WWW.error))
                    {
                        m_LoadResourceAgentHelperReadBytesCompleteEventHandler.Invoke(this, 
                            new LoadResourceAgentHelperReadBytesCompleteEventArgs(m_WWW.bytes, m_LoadType));
                        m_BytesFullPath = null;
                        m_LoadType = 0;
                        m_LastProgress = 0f;
                    }
                    else
                    {
                        m_LoadResourceAgentHelperErrorEventHandler.Invoke(this,
                        new LoadResourceAgentHelperErrorEventArgs(LoadResourceStatus.NotExist, Utility.Text.Format("Can not load asset bundle '{0}' with error message '{1}'", m_BytesFullPath, m_WWW.error)));
                    }
                    m_WWW.Dispose();
                    m_WWW = null;
                }
                else if (m_WWW.progress != m_LastProgress)
                {
                    m_LastProgress = m_WWW.progress;
                    m_LoadResourceAgentHelperUpdateEventHandler.Invoke(this, 
                        new LoadResourceAgentHelperUpdateEventArgs(LoadResourceProgress.ReadResource, m_WWW.progress));
                }
            }
        }

#endif

        private void UpdateFileAssetBundleCreateRequest()
        {
            if(m_FileAssetBundleCreateRequest != null)
            {
                if (m_FileAssetBundleCreateRequest.isDone)  //完成
                {
                    AssetBundle assetBundle = m_FileAssetBundleCreateRequest.assetBundle;
                    if(assetBundle != null) //检查是否为null
                    {
                        AssetBundleCreateRequest oldFileAssetBundleCreateRequest = m_FileAssetBundleCreateRequest;
                        m_LoadResourceAgentHelperReadFileCompleteEventHandler.Invoke(this,
                            new LoadResourceAgentHelperReadFileCompleteEventArgs(assetBundle));
                        if (m_FileAssetBundleCreateRequest == oldFileAssetBundleCreateRequest)
                        {
                            m_FileAssetBundleCreateRequest = null;
                            m_LastProgress = 0f;
                        }
                    }
                    else if (m_FileAssetBundleCreateRequest.progress != m_LastProgress)
                    {
                        m_LastProgress = m_FileAssetBundleCreateRequest.progress;
                        m_LoadResourceAgentHelperErrorEventHandler.Invoke(this,
                            new LoadResourceAgentHelperErrorEventArgs(LoadResourceStatus.NotExist, Utility.Text.Format("Can not load asset bundle from file '{0}' which is not a valid asset bundle.", m_FileFullPath)));
                    }
                }
                else
                {
                    m_LoadResourceAgentHelperUpdateEventHandler.Invoke(this,
                        new LoadResourceAgentHelperUpdateEventArgs(LoadResourceProgress.LoadResource, m_FileAssetBundleCreateRequest.progress));
                }
            }
        }

        private void UpdateBytesAssetBundleCreateRequest()
        {
            if(m_BytesAssetBundleCreateRequest != null)
            {
                if (m_BytesAssetBundleCreateRequest.isDone)
                {
                    AssetBundle assetBundle = m_BytesAssetBundleCreateRequest.assetBundle;
                    if(assetBundle != null)
                    {
                        AssetBundleCreateRequest oldBytesAssetBundleCreateRequest = m_BytesAssetBundleCreateRequest;
                        m_LoadResourceAgentHelperParseBytesCompleteEventHandler.Invoke(this,
                            new LoadResourceAgentHelperParseBytesCompleteEventArgs(assetBundle));
                        if(m_BytesAssetBundleCreateRequest == oldBytesAssetBundleCreateRequest)
                        {
                            m_BytesAssetBundleCreateRequest = null;
                            m_LastProgress = 0f;
                        }
                    }
                    else if (m_BytesAssetBundleCreateRequest.progress != m_LastProgress)
                    {
                        m_LastProgress = m_BytesAssetBundleCreateRequest.progress;
                        m_LoadResourceAgentHelperErrorEventHandler.Invoke(this, 
                            new LoadResourceAgentHelperErrorEventArgs(LoadResourceStatus.NotExist, "Can not load asset bundle from memory which is not a valid asset bundle."));
                    }
                }
                else
                {
                    m_LoadResourceAgentHelperUpdateEventHandler.Invoke(this, 
                        new LoadResourceAgentHelperUpdateEventArgs(LoadResourceProgress.LoadResource, m_BytesAssetBundleCreateRequest.progress));
                }
            }
        }

        private void UpdateAssetBundleRequest()
        {
            if(m_AssetBundleRequest != null)
            {
                if (m_AssetBundleRequest.isDone)
                {
                    if(m_AssetBundleRequest.asset != null)
                    {
                        m_LoadResourceAgentHelperLoadCompleteEventHandler.Invoke(this, 
                            new LoadResourceAgentHelperLoadCompleteEventArgs(m_AssetBundleRequest.asset));
                        m_AssetName = null;
                        m_LastProgress = 0f;
                        m_AssetBundleRequest = null;
                    }
                    else if (m_AssetBundleRequest.progress != m_LastProgress)
                    {
                        m_LastProgress = m_AssetBundleRequest.progress;
                        m_LoadResourceAgentHelperErrorEventHandler.Invoke(this,
                            new LoadResourceAgentHelperErrorEventArgs(LoadResourceStatus.AssetError, Utility.Text.Format("Can not load asset '{0}' from asset bundle which is not exist.", m_AssetName)));
                    }
                }
                else
                {
                    m_LoadResourceAgentHelperUpdateEventHandler.Invoke(this, 
                        new LoadResourceAgentHelperUpdateEventArgs(LoadResourceProgress.LoadAsset, m_AssetBundleRequest.progress));
                }
            }
        }

        private void UpdateAsyncOperation()
        {
            if(m_AsyncOperation != null)
            {
                if (m_AsyncOperation.isDone)
                {
                    if (m_AsyncOperation.allowSceneActivation)
                    {
                        m_LoadResourceAgentHelperLoadCompleteEventHandler.Invoke(this,
                            new LoadResourceAgentHelperLoadCompleteEventArgs(new object()));
                        m_AssetName = null;
                        m_LastProgress = 0f;
                        m_AsyncOperation = null;
                    }
                    else
                    {
                        m_LoadResourceAgentHelperErrorEventHandler.Invoke(this,
                            new LoadResourceAgentHelperErrorEventArgs(LoadResourceStatus.AssetError, Utility.Text.Format("Can not load scene asset '{0}' from asset bundle.", m_AssetName)));
                    }
                }
                else if (m_AsyncOperation.progress != m_LastProgress)
                {
                    m_LastProgress = m_AsyncOperation.progress;
                    m_LoadResourceAgentHelperUpdateEventHandler.Invoke(this,
                        new LoadResourceAgentHelperUpdateEventArgs(LoadResourceProgress.LoadScene, m_AsyncOperation.progress));
                }
            }
        }

    }
}
