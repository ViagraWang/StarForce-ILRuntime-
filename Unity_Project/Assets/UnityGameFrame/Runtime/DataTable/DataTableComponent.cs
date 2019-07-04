using GameFramework;
using GameFramework.DataTable;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace UnityGameFrame.Runtime
{
    /// <summary>
    /// 数据表组件
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Game Framework/Data Table")]
    public sealed class DataTableComponent : GameFrameworkComponent
    {
        private IDataTableManager m_DataTableManager = null;    //数据表管理器
        private EventComponent m_EventComponent = null; //事件组件

        //事件触发标志位
        [SerializeField]
        private bool m_EnableLoadDataTableSuccessEvent = true;
        [SerializeField]
        private bool m_EnableLoadDataTableFailureEvent = true;
        [SerializeField]
        private bool m_EnableLoadDataTableUpdateEvent = false;
        [SerializeField]
        private bool m_EnableLoadDataTableDependencyAssetEvent = false;

        [SerializeField]
        private string m_DataTableHelperTypeName = "UnityGameFrame.Runtime.DefaultDataTableHelper"; //默认数据表辅助器

        [SerializeField]
        private DataTableHelperBase m_CustomDataTableHelper = null; //用户自定义辅助器

        protected override void Awake()
        {
            base.Awake();

            m_DataTableManager = GameFrameworkEntry.GetModule<IDataTableManager>();
            if (m_DataTableManager == null)
            {
                Log.Fatal("[DataTableComponent.Awake] Data table manager is invalid.");
                return;
            }

            m_DataTableManager.LoadDataTableSuccess += OnLoadDataTableSuccess;
            m_DataTableManager.LoadDataTableFailure += OnLoadDataTableFailure;
            m_DataTableManager.LoadDataTableUpdate += OnLoadDataTableUpdate;
            m_DataTableManager.LoadDataTableDependencyAsset += OnLoadDataTableDependencyAsset;
        }

        private void Start()
        {
            BaseComponent baseComponent = GameEntry.GetComponent<BaseComponent>();
            if (baseComponent == null)
            {
                Log.Fatal("[DataTableComponent.Start] Base component is invalid.");
                return;
            }
            m_DataTableManager.SetResourceManager(baseComponent.ResourceManager);   //设置资源管理器
            //时间组件
            m_EventComponent = GameEntry.GetComponent<EventComponent>();
            if (m_EventComponent == null)
            {
                Log.Fatal("[DataTableComponent.Start] Event component is invalid.");
                return;
            }
            //数据表辅助器
            DataTableHelperBase dataTableHelper = Helper.CreateHelper(m_DataTableHelperTypeName, m_CustomDataTableHelper);
            if (dataTableHelper == null)
            {
                Log.Error("[DataTableComponent.Start] Can not create data table helper.");
                return;
            }

            dataTableHelper.name = "Data Table Helper";
            Transform trans = dataTableHelper.transform;
            trans.SetParent(transform);
            trans.localScale = Vector3.one;

            m_DataTableManager.SetDataTableHelper(dataTableHelper);
        }

        /// <summary>
        /// 获取数据表的数量
        /// </summary>
        public int Count { get { return m_DataTableManager.Count; } }

        /// <summary>
        /// 加载数据表
        /// </summary>
        /// <typeparam name="T">数据表行的类型</typeparam>
        /// <param name="dataTableName">数据表名称</param>
        /// <param name="dataTableAssetName">数据表资源名称</param>
        /// <param name="loadType">数据表加载方式</param>
        /// <param name="userData">用户自定义数据</param>
        public void LoadDataTable<T>(string dataTableName, string dataTableAssetName, LoadType loadType, object userData = null) where T : IDataRow
        {
            LoadDataTable(typeof(T), dataTableName, null, dataTableAssetName, 0, userData);
        }

        /// <summary>
        /// 加载数据表
        /// </summary>
        /// <param name="dataRowType">数据表行的类型</param>
        /// <param name="dataTableName">数据表名称</param>
        /// <param name="dataTableAssetName">数据表资源名称</param>
        /// <param name="loadType">数据表加载方式</param>
        /// <param name="userData">用户自定义数据</param>
        public void LoadDataTable(Type dataRowType, string dataTableName, string dataTableAssetName, LoadType loadType, object userData = null)
        {
            LoadDataTable(dataRowType, dataTableName, null, dataTableAssetName, 0, userData);
        }

        /// <summary>
        /// 加载数据表
        /// </summary>
        /// <typeparam name="T">数据表行的类型</typeparam>
        /// <param name="dataTableName">数据表名称</param>
        /// <param name="dataTableAssetName">数据表资源名称</param>
        /// <param name="loadType">数据表加载方式</param>
        /// <param name="priority">加载数据表资源的优先级</param>
        /// <param name="userData">用户自定义数据</param>
        public void LoadDataTable<T>(string dataTableName, string dataTableAssetName, LoadType loadType, int priority, object userData = null) where T : IDataRow
        {
            LoadDataTable(typeof(T), dataTableName, null, dataTableAssetName, loadType, priority, userData);
        }

        /// <summary>
        /// 加载数据表
        /// </summary>
        /// <param name="dataRowType">数据表行的类型</param>
        /// <param name="dataTableName">数据表名称</param>
        /// <param name="dataTableAssetName">数据表资源名称</param>
        /// <param name="loadType">数据表加载方式</param>
        /// <param name="priority">加载数据表资源的优先级</param>
        /// <param name="userData">用户自定义数据</param>
        public void LoadDataTable(Type dataRowType, string dataTableName, string dataTableAssetName, LoadType loadType, int priority, object userData = null)
        {
            LoadDataTable(dataRowType, dataTableName, null, dataTableAssetName, loadType, priority, userData);
        }

        /// <summary>
        /// 加载数据表
        /// </summary>
        /// <typeparam name="T">数据表行的类型</typeparam>
        /// <param name="dataTableName">数据表名称</param>
        /// <param name="dataTableNameInType">数据表类型下的名称</param>
        /// <param name="dataTableAssetName">数据表资源名称</param>
        /// <param name="loadType">数据表加载方式</param>
        /// <param name="userData">用户自定义数据</param>
        public void LoadDataTable<T>(string dataTableName, string dataTableNameInType, string dataTableAssetName, LoadType loadType, object userData = null) where T : IDataRow
        {
            LoadDataTable(typeof(T), dataTableName, dataTableNameInType, dataTableAssetName, loadType, 0, userData);
        }

        /// <summary>
        /// 加载数据表
        /// </summary>
        /// <param name="dataRowType">数据表行的类型</param>
        /// <param name="dataTableName">数据表名称</param>
        /// <param name="dataTableNameInType">数据表类型下的名称</param>
        /// <param name="dataTableAssetName">数据表资源名称</param>
        /// <param name="loadType">数据表加载方式</param>
        /// <param name="userData">用户自定义数据</param>
        public void LoadDataTable(Type dataRowType, string dataTableName, string dataTableNameInType, string dataTableAssetName, LoadType loadType, object userData = null)
        {
            LoadDataTable(dataRowType, dataTableName, dataTableNameInType, dataTableAssetName, loadType, 0, userData);
        }

        /// <summary>
        /// 加载数据表
        /// </summary>
        /// <typeparam name="T">数据表行的类型</typeparam>
        /// <param name="dataTableName">数据表名称</param>
        /// <param name="dataTableNameInType">数据表类型下的名称</param>
        /// <param name="dataTableAssetName">数据表资源名称</param>
        /// <param name="loadType">数据表加载方式</param>
        /// <param name="priority">加载数据表资源的优先级</param>
        /// <param name="userData">用户自定义数据</param>
        public void LoadDataTable<T>(string dataTableName, string dataTableNameInType, string dataTableAssetName, LoadType loadType, int priority, object userData = null) where T : IDataRow
        {
            LoadDataTable(typeof(T), dataTableName, dataTableNameInType, dataTableAssetName, loadType, priority, userData);
        }

        /// <summary>
        /// 加载数据表
        /// </summary>
        /// <param name="dataRowType">数据表行的类型</param>
        /// <param name="dataTableName">数据表名称</param>
        /// <param name="dataTableNameInType">数据表类型下的名称</param>
        /// <param name="dataTableAssetName">数据表资源名称</param>
        /// <param name="loadType">数据表加载方式</param>
        /// <param name="priority">加载数据表资源的优先级</param>
        /// <param name="userData">用户自定义数据</param>
        public void LoadDataTable(Type dataRowType, string dataTableName, string dataTableNameInType, string dataTableAssetName, LoadType loadType, int priority, object userData = null)
        {
            if (dataRowType == null)
            {
                Log.Error("[DataTableComponent.LoadDataTable] Data row type is invalid.");
                return;
            }

            if (string.IsNullOrEmpty(dataTableName))
            {
                Log.Error("[DataTableComponent.LoadDataTable] Data table name is invalid.");
                return;
            }

            m_DataTableManager.LoadDataTable(dataTableAssetName, loadType, priority, new LoadDataTableInfo(dataRowType, dataTableName, dataTableNameInType, userData));
        }

        /// <summary>
        /// 是否存在数据表
        /// </summary>
        /// <typeparam name="T">数据表行的类型</typeparam>
        /// <returns>是否存在数据表</returns>
        public bool HasDataTable<T>() where T : IDataRow
        {
            return m_DataTableManager.HasDataTable<T>();
        }

        /// <summary>
        /// 是否存在数据表
        /// </summary>
        /// <param name="dataRowType">数据表行的类型</param>
        /// <returns>是否存在数据表</returns>
        public bool HasDataTable(Type dataRowType)
        {
            return m_DataTableManager.HasDataTable(dataRowType);
        }

        /// <summary>
        /// 是否存在数据表
        /// </summary>
        /// <typeparam name="T">数据表行的类型</typeparam>
        /// <param name="name">数据表名称</param>
        /// <returns>是否存在数据表</returns>
        public bool HasDataTable<T>(string name) where T : IDataRow
        {
            return m_DataTableManager.HasDataTable<T>(name);
        }

        /// <summary>
        /// 是否存在数据表
        /// </summary>
        /// <param name="dataRowType">数据表行的类型</param>
        /// <param name="name">数据表名称</param>
        /// <returns>是否存在数据表</returns>
        public bool HasDataTable(Type dataRowType, string name)
        {
            return m_DataTableManager.HasDataTable(dataRowType, name);
        }

        /// <summary>
        /// 获取数据表
        /// </summary>
        /// <typeparam name="T">数据表行的类型</typeparam>
        /// <returns>要获取的数据表</returns>
        public IDataTable<T> GetDataTable<T>() where T : IDataRow
        {
            return m_DataTableManager.GetDataTable<T>();
        }

        /// <summary>
        /// 获取数据表
        /// </summary>
        /// <param name="dataRowType">数据表行的类型</param>
        /// <returns>要获取的数据表</returns>
        public DataTableBase GetDataTable(Type dataRowType)
        {
            return m_DataTableManager.GetDataTable(dataRowType);
        }

        /// <summary>
        /// 获取数据表
        /// </summary>
        /// <typeparam name="T">数据表行的类型</typeparam>
        /// <param name="name">数据表名称</param>
        /// <returns>要获取的数据表</returns>
        public IDataTable<T> GetDataTable<T>(string name) where T : IDataRow
        {
            return m_DataTableManager.GetDataTable<T>(name);
        }

        /// <summary>
        /// 获取数据表
        /// </summary>
        /// <param name="dataRowType">数据表行的类型</param>
        /// <param name="name">数据表名称</param>
        /// <returns>要获取的数据表</returns>
        public DataTableBase GetDataTable(Type dataRowType, string name)
        {
            return m_DataTableManager.GetDataTable(dataRowType, name);
        }

        /// <summary>
        /// 获取所有数据表
        /// </summary>
        public DataTableBase[] GetAllDataTables()
        {
            return m_DataTableManager.GetAllDataTables();
        }

        /// <summary>
        /// 获取所有数据表
        /// </summary>
        /// <param name="results">所有数据表</param>
        public void GetAllDataTables(List<DataTableBase> results)
        {
            m_DataTableManager.GetAllDataTables(results);
        }

        /// <summary>
        /// 创建数据表
        /// </summary>
        /// <typeparam name="T">数据表行的类型</typeparam>
        /// <param name="text">要解析的数据表文本</param>
        /// <returns>要创建的数据表</returns>
        public IDataTable<T> CreateDataTable<T>(string text) where T : class, IDataRow, new()
        {
            return m_DataTableManager.CreateDataTable<T>(text);
        }

        /// <summary>
        /// 创建数据表
        /// </summary>
        /// <param name="dataRowType">数据表行的类型</param>
        /// <param name="text">要解析的数据表文本</param>
        /// <returns>要创建的数据表</returns>
        public DataTableBase CreateDataTable(Type dataRowType, string text)
        {
            return m_DataTableManager.CreateDataTable(dataRowType, text);
        }

        /// <summary>
        /// 创建数据表
        /// </summary>
        /// <typeparam name="T">数据表行的类型</typeparam>
        /// <param name="name">数据表名称</param>
        /// <param name="text">要解析的数据表文本</param>
        /// <returns>要创建的数据表</returns>
        public IDataTable<T> CreateDataTable<T>(string name, string text) where T : class, IDataRow, new()
        {
            return m_DataTableManager.CreateDataTable<T>(name, text);
        }

        /// <summary>
        /// 创建数据表
        /// </summary>
        /// <param name="dataRowType">数据表行的类型</param>
        /// <param name="name">数据表名称</param>
        /// <param name="text">要解析的数据表文本</param>
        /// <returns>要创建的数据表</returns>
        public DataTableBase CreateDataTable(Type dataRowType, string name, string text)
        {
            return m_DataTableManager.CreateDataTable(dataRowType, name, text);
        }

        /// <summary>
        /// 销毁数据表
        /// </summary>
        /// <typeparam name="T">数据表行的类型</typeparam>
        /// <returns>是否销毁数据表成功</returns>
        public bool DestroyDataTable<T>() where T : IDataRow, new()
        {
            return m_DataTableManager.DestroyDataTable<T>();
        }

        /// <summary>
        /// 创建数据表
        /// </summary>
        /// <typeparam name="T">数据表行的类型</typeparam>
        /// <param name="bytes">要解析的数据表二进制流</param>
        /// <returns>要创建的数据表</returns>
        public IDataTable<T> CreateDataTable<T>(byte[] bytes) where T : class, IDataRow, new()
        {
            return m_DataTableManager.CreateDataTable<T>(bytes);
        }

        /// <summary>
        /// 创建数据表
        /// </summary>
        /// <param name="dataRowType">数据表行的类型</param>
        /// <param name="bytes">要解析的数据表二进制流</param>
        /// <returns>要创建的数据表</returns>
        public DataTableBase CreateDataTable(Type dataRowType, byte[] bytes)
        {
            return m_DataTableManager.CreateDataTable(dataRowType, bytes);
        }

        /// <summary>
        /// 创建数据表
        /// </summary>
        /// <typeparam name="T">数据表行的类型</typeparam>
        /// <param name="name">数据表名称</param>
        /// <param name="bytes">要解析的数据表二进制流</param>
        /// <returns>要创建的数据表</returns>
        public IDataTable<T> CreateDataTable<T>(string name, byte[] bytes) where T : class, IDataRow, new()
        {
            return m_DataTableManager.CreateDataTable<T>(name, bytes);
        }

        /// <summary>
        /// 创建数据表
        /// </summary>
        /// <param name="dataRowType">数据表行的类型</param>
        /// <param name="name">数据表名称</param>
        /// <param name="bytes">要解析的数据表二进制流</param>
        /// <returns>要创建的数据表</returns>
        public DataTableBase CreateDataTable(Type dataRowType, string name, byte[] bytes)
        {
            return m_DataTableManager.CreateDataTable(dataRowType, name, bytes);
        }

        /// <summary>
        /// 创建数据表
        /// </summary>
        /// <typeparam name="T">数据表行的类型</typeparam>
        /// <param name="stream">要解析的数据表二进制流</param>
        /// <returns>要创建的数据表</returns>
        public IDataTable<T> CreateDataTable<T>(Stream stream) where T : class, IDataRow, new()
        {
            return m_DataTableManager.CreateDataTable<T>(stream);
        }

        /// <summary>
        /// 创建数据表
        /// </summary>
        /// <param name="dataRowType">数据表行的类型</param>
        /// <param name="stream">要解析的数据表二进制流</param>
        /// <returns>要创建的数据表</returns>
        public DataTableBase CreateDataTable(Type dataRowType, Stream stream)
        {
            return m_DataTableManager.CreateDataTable(dataRowType, stream);
        }

        /// <summary>
        /// 创建数据表
        /// </summary>
        /// <typeparam name="T">数据表行的类型</typeparam>
        /// <param name="name">数据表名称</param>
        /// <param name="stream">要解析的数据表二进制流</param>
        /// <returns>要创建的数据表</returns>
        public IDataTable<T> CreateDataTable<T>(string name, Stream stream) where T : class, IDataRow, new()
        {
            return m_DataTableManager.CreateDataTable<T>(name, stream);
        }

        /// <summary>
        /// 创建数据表
        /// </summary>
        /// <param name="dataRowType">数据表行的类型</param>
        /// <param name="name">数据表名称</param>
        /// <param name="stream">要解析的数据表二进制流</param>
        /// <returns>要创建的数据表</returns>
        public DataTableBase CreateDataTable(Type dataRowType, string name, Stream stream)
        {
            return m_DataTableManager.CreateDataTable(dataRowType, name, stream);
        }
        /// <summary>
        /// 销毁数据表
        /// </summary>
        /// <param name="dataRowType">数据表行的类型</param>
        /// <returns>是否销毁数据表成功</returns>
        public bool DestroyDataTable(Type dataRowType)
        {
            return m_DataTableManager.DestroyDataTable(dataRowType);
        }

        /// <summary>
        /// 销毁数据表
        /// </summary>
        /// <typeparam name="T">数据表行的类型</typeparam>
        /// <param name="name">数据表名称</param>
        /// <returns>是否销毁数据表成功</returns>
        public bool DestroyDataTable<T>(string name) where T : IDataRow
        {
            return m_DataTableManager.DestroyDataTable<T>(name);
        }

        /// <summary>
        /// 销毁数据表
        /// </summary>
        /// <param name="dataRowType">数据表行的类型</param>
        /// <param name="name">数据表名称</param>
        /// <returns>是否销毁数据表成功</returns>
        public bool DestroyDataTable(Type dataRowType, string name)
        {
            return m_DataTableManager.DestroyDataTable(dataRowType, name);
        }

        //加载数据表成功回调
        private void OnLoadDataTableSuccess(object sender, GameFramework.DataTable.LoadDataTableSuccessEventArgs e)
        {
            if (m_EnableLoadDataTableSuccessEvent)
                m_EventComponent.Fire(this, ReferencePool.Acquire<LoadDataTableSuccessEventArgs>().Fill(e));
        }

        //加载数据表失败回调
        private void OnLoadDataTableFailure(object sender, GameFramework.DataTable.LoadDataTableFailureEventArgs e)
        {
            Log.Warning("[DataTableComponent.OnLoadDataTableFaliure] Load data table failure, asset name '{0}', error message '{1}'.", e.DataTableAssetName, e.ErrorMessage);
            if (m_EnableLoadDataTableFailureEvent)
                m_EventComponent.Fire(this, ReferencePool.Acquire<LoadDataTableFailureEventArgs>().Fill(e));
        }

        //加载数据表更新败回调
        private void OnLoadDataTableUpdate(object sender, GameFramework.DataTable.LoadDataTableUpdateEventArgs e)
        {
            if (m_EnableLoadDataTableUpdateEvent)
            {
                m_EventComponent.Fire(this, ReferencePool.Acquire<LoadDataTableUpdateEventArgs>().Fill(e));
            }
        }

        //加载数据表依赖资源回调
        private void OnLoadDataTableDependencyAsset(object sender, GameFramework.DataTable.LoadDataTableDependencyAssetEventArgs e)
        {
            if (m_EnableLoadDataTableDependencyAssetEvent)
            {
                m_EventComponent.Fire(this, ReferencePool.Acquire<LoadDataTableDependencyAssetEventArgs>().Fill(e));
            }
        }

    }
}
