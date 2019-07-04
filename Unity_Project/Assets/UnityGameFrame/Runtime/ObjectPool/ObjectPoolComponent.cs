﻿using GameFramework;
using GameFramework.ObjectPool;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityGameFrame.Runtime
{
    /// <summary>
    /// 对象池组件
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Game Framework/Object Pool")]
    public sealed class ObjectPoolComponent : GameFrameworkComponent
    {
        private IObjectPoolManager m_ObjectPoolManager = null;  //对象池管理器

        /// <summary>
        /// 获取对象池数量
        /// </summary>
        public int Count { get { return m_ObjectPoolManager.Count; } }

        protected override void Awake()
        {
            base.Awake();
            m_ObjectPoolManager = GameFrameworkEntry.GetModule<IObjectPoolManager>();
            if (m_ObjectPoolManager == null)
            {
                Log.Fatal("[ObjectPoolComponent.Awake] Object pool manager is invalid.");
                return;
            }
        }

        /// <summary>
        /// 检查是否存在对象池
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">对象池名称</param>
        /// <returns>是否存在对象池</returns>
        public bool HasObjectPool<T>(string name = null) where T : ObjectBase
        {
            return m_ObjectPoolManager.HasObjectPool<T>(name);
        }

        /// <summary>
        /// 检查是否存在对象池
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="name">对象池名称</param>
        /// <returns>是否存在对象池</returns>
        public bool HasObjectPool(Type objectType, string name = null)
        {
            return m_ObjectPoolManager.HasObjectPool(objectType, name);
        }

        /// <summary>
        /// 检查是否存在对象池
        /// </summary>
        /// <param name="fullName">对象池完整名称</param>
        /// <returns>是否存在对象池</returns>
        public bool HasObjectPool(string fullName)
        {
            return m_ObjectPoolManager.HasObjectPool(fullName);
        }

        /// <summary>
        /// 检查是否存在对象池
        /// </summary>
        /// <param name="condition">要检查的条件</param>
        /// <returns>是否存在对象池</returns>
        public bool HasObjectPool(Predicate<ObjectPoolBase> condition)
        {
            return m_ObjectPoolManager.HasObjectPool(condition);
        }

        /// <summary>
        /// 获取对象池
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">对象池名称</param>
        /// <returns>要获取的对象池</returns>
        public IObjectPool<T> GetObjectPool<T>(string name = null) where T : ObjectBase
        {
            return m_ObjectPoolManager.GetObjectPool<T>(name);
        }

        /// <summary>
        /// 获取对象池
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="name">对象池名称</param>
        /// <returns>要获取的对象池</returns>
        public ObjectPoolBase GetObjectPool(Type objectType, string name = null)
        {
            return m_ObjectPoolManager.GetObjectPool(objectType, name);
        }

        /// <summary>
        /// 获取对象池
        /// </summary>
        /// <param name="fullName">对象池完整名称</param>
        /// <returns>要获取的对象池</returns>
        public ObjectPoolBase GetObjectPool(string fullName)
        {
            return m_ObjectPoolManager.GetObjectPool(fullName);
        }

        /// <summary>
        /// 获取对象池
        /// </summary>
        /// <param name="condition">要检查的条件</param>
        /// <returns>要获取的对象池</returns>
        public ObjectPoolBase GetObjectPool(Predicate<ObjectPoolBase> condition)
        {
            return m_ObjectPoolManager.GetObjectPool(condition);
        }

        /// <summary>
        /// 获取对象池
        /// </summary>
        /// <param name="condition">要检查的条件</param>
        /// <returns>要获取的对象池</returns>
        public ObjectPoolBase[] GetObjectPools(Predicate<ObjectPoolBase> condition)
        {
            return m_ObjectPoolManager.GetObjectPools(condition);
        }

        /// <summary>
        /// 获取对象池
        /// </summary>
        /// <param name="condition">要检查的条件</param>
        /// <param name="results">要获取的对象池</param>
        public void GetObjectPools(Predicate<ObjectPoolBase> condition, List<ObjectPoolBase> results)
        {
            m_ObjectPoolManager.GetObjectPools(condition, results);
        }


        /// <summary>
        /// 获取所有对象池
        /// </summary>
        public ObjectPoolBase[] GetAllObjectPools()
        {
            return m_ObjectPoolManager.GetAllObjectPools();
        }

        /// <summary>
        /// 获取所有对象池
        /// </summary>
        /// <param name="results">所有对象池</param>
        public void GetAllObjectPools(List<ObjectPoolBase> results)
        {
            m_ObjectPoolManager.GetAllObjectPools(results);
        }

        /// <summary>
        /// 获取所有对象池
        /// </summary>
        /// <param name="sort">是否根据对象池的优先级排序</param>
        /// <returns>所有对象池</returns>
        public ObjectPoolBase[] GetAllObjectPools(bool sort)
        {
            return m_ObjectPoolManager.GetAllObjectPools(sort);
        }

        /// <summary>
        /// 获取所有对象池
        /// </summary>
        /// <param name="sort">是否根据对象池的优先级排序</param>
        /// <param name="results">所有对象池</param>
        public void GetAllObjectPools(bool sort, List<ObjectPoolBase> results)
        {
            m_ObjectPoolManager.GetAllObjectPools(sort, results);
        }

        /// <summary>
        /// 创建允许单次获取的对象池
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="allowMulti">是否允许多次获取对象</param>
        /// <returns>创建的对象池</returns>
        public IObjectPool<T> CreateSpawnObjectPool<T>(bool allowMulti) where T : ObjectBase
        {
            return allowMulti ? m_ObjectPoolManager.CreateMultiSpawnObjectPool<T>() : m_ObjectPoolManager.CreateSingleSpawnObjectPool<T>();
        }

        /// <summary>
        /// 创建允许单次获取的对象池
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="allowMulti">是否允许多次获取对象</param>
        /// <returns>要创建的允许单次获取的对象池</returns>
        public ObjectPoolBase CreateSpawnObjectPool(bool allowMulti, Type objectType)
        {
            return allowMulti ? m_ObjectPoolManager.CreateMultiSpawnObjectPool(objectType) : m_ObjectPoolManager.CreateSingleSpawnObjectPool(objectType);

        }

        /// <summary>
        /// 创建允许单次获取的对象池
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="allowMulti">是否允许多次获取对象</param>
        /// <param name="name">对象池名称</param>
        /// <returns>要创建的允许单次获取的对象池</returns>
        public IObjectPool<T> CreateSpawnObjectPool<T>(bool allowMulti, string name) where T : ObjectBase
        {
            return allowMulti ? m_ObjectPoolManager.CreateMultiSpawnObjectPool<T>(name) : m_ObjectPoolManager.CreateSingleSpawnObjectPool<T>(name);
        }

        /// <summary>
        /// 创建允许单次获取的对象池
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="allowMulti">是否允许多次获取对象</param>
        /// <param name="name">对象池名称</param>
        /// <returns>要创建的允许单次获取的对象池</returns>
        public ObjectPoolBase CreateSpawnObjectPool(Type objectType, bool allowMulti, string name)
        {
            return allowMulti ? m_ObjectPoolManager.CreateMultiSpawnObjectPool(objectType, name) : m_ObjectPoolManager.CreateSingleSpawnObjectPool(objectType, name);
        }

        /// <summary>
        /// 创建允许单次获取的对象池
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="allowMulti">是否允许多次获取对象</param>
        /// <param name="capacity">对象池的容量</param>
        /// <returns>要创建的允许单次获取的对象池</returns>
        public IObjectPool<T> CreateSpawnObjectPool<T>(bool allowMulti, int capacity) where T : ObjectBase
        {
            return allowMulti ? m_ObjectPoolManager.CreateMultiSpawnObjectPool<T>(capacity) : m_ObjectPoolManager.CreateSingleSpawnObjectPool<T>(capacity);
        }

        /// <summary>
        /// 创建允许单次获取的对象池
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="allowMulti">是否允许多次获取对象</param>
        /// <param name="capacity">对象池的容量</param>
        /// <returns>要创建的允许单次获取的对象池</returns>
        public ObjectPoolBase CreateSpawnObjectPool(Type objectType, bool allowMulti, int capacity)
        {
            return allowMulti ? m_ObjectPoolManager.CreateMultiSpawnObjectPool(objectType, capacity) : m_ObjectPoolManager.CreateSingleSpawnObjectPool(objectType, capacity);
        }

        /// <summary>
        /// 创建允许单次获取的对象池
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="allowMulti">是否允许多次获取对象</param>
        /// <param name="expireTime">对象池对象过期秒数</param>
        /// <returns>要创建的允许单次获取的对象池</returns>
        public IObjectPool<T> CreateSpawnObjectPool<T>(bool allowMulti, float expireTime) where T : ObjectBase
        {
            return allowMulti ? m_ObjectPoolManager.CreateMultiSpawnObjectPool<T>(expireTime) : m_ObjectPoolManager.CreateSingleSpawnObjectPool<T>(expireTime);
        }

        /// <summary>
        /// 创建允许单次获取的对象池
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="allowMulti">是否允许多次获取对象</param>
        /// <param name="expireTime">对象池对象过期秒数</param>
        /// <returns>要创建的允许单次获取的对象池</returns>
        public ObjectPoolBase CreateSpawnObjectPool(Type objectType, bool allowMulti, float expireTime)
        {
            return allowMulti ? m_ObjectPoolManager.CreateMultiSpawnObjectPool(objectType, expireTime) : m_ObjectPoolManager.CreateSingleSpawnObjectPool(objectType, expireTime);
        }

        /// <summary>
        /// 创建允许单次获取的对象池
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="allowMulti">是否允许多次获取对象</param>
        /// <param name="name">对象池名称</param>
        /// <param name="capacity">对象池的容量</param>
        /// <returns>要创建的允许单次获取的对象池</returns>
        public IObjectPool<T> CreateSpawnObjectPool<T>(bool allowMulti, string name, int capacity) where T : ObjectBase
        {
            return allowMulti ? m_ObjectPoolManager.CreateMultiSpawnObjectPool<T>(name, capacity) : m_ObjectPoolManager.CreateSingleSpawnObjectPool<T>(name, capacity);
        }

        /// <summary>
        /// 创建允许单次获取的对象池
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="allowMulti">是否允许多次获取对象</param>
        /// <param name="name">对象池名称</param>
        /// <param name="capacity">对象池的容量</param>
        /// <returns>要创建的允许单次获取的对象池</returns>
        public ObjectPoolBase CreateSpawnObjectPool(Type objectType, bool allowMulti, string name, int capacity)
        {
            return allowMulti ? m_ObjectPoolManager.CreateMultiSpawnObjectPool(objectType, name, capacity) : m_ObjectPoolManager.CreateSingleSpawnObjectPool(objectType, name, capacity);
        }

        /// <summary>
        /// 创建允许单次获取的对象池
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="allowMulti">是否允许多次获取对象</param>
        /// <param name="name">对象池名称</param>
        /// <param name="expireTime">对象池对象过期秒数</param>
        /// <returns>要创建的允许单次获取的对象池</returns>
        public IObjectPool<T> CreateSpawnObjectPool<T>(bool allowMulti, string name, float expireTime) where T : ObjectBase
        {
            return allowMulti ? m_ObjectPoolManager.CreateMultiSpawnObjectPool<T>(name, expireTime) : m_ObjectPoolManager.CreateSingleSpawnObjectPool<T>(name, expireTime);
        }

        /// <summary>
        /// 创建允许单次获取的对象池
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="allowMulti">是否允许多次获取对象</param>
        /// <param name="name">对象池名称</param>
        /// <param name="expireTime">对象池对象过期秒数</param>
        /// <returns>要创建的允许单次获取的对象池</returns>
        public ObjectPoolBase CreateSpawnObjectPool(Type objectType, bool allowMulti, string name, float expireTime)
        {
            return allowMulti ? m_ObjectPoolManager.CreateMultiSpawnObjectPool(objectType, name, expireTime) : m_ObjectPoolManager.CreateSingleSpawnObjectPool(objectType, name, expireTime);
        }

        /// <summary>
        /// 创建允许单次获取的对象池
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="allowMulti">是否允许多次获取对象</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="expireTime">对象池对象过期秒数</param>
        /// <returns>要创建的允许单次获取的对象池</returns>
        public IObjectPool<T> CreateSpawnObjectPool<T>(bool allowMulti, int capacity, float expireTime) where T : ObjectBase
        {
            return allowMulti ? m_ObjectPoolManager.CreateMultiSpawnObjectPool<T>(capacity, expireTime) : m_ObjectPoolManager.CreateSingleSpawnObjectPool<T>(capacity, expireTime);
        }

        /// <summary>
        /// 创建允许单次获取的对象池
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="allowMulti">是否允许多次获取对象</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="expireTime">对象池对象过期秒数</param>
        /// <returns>要创建的允许单次获取的对象池</returns>
        public ObjectPoolBase CreateSpawnObjectPool(Type objectType, bool allowMulti, int capacity, float expireTime)
        {
            return allowMulti ? m_ObjectPoolManager.CreateMultiSpawnObjectPool(objectType, capacity, expireTime) : m_ObjectPoolManager.CreateSingleSpawnObjectPool(objectType, capacity, expireTime);
        }

        /// <summary>
        /// 创建允许单次获取的对象池
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="allowMulti">是否允许多次获取对象</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>要创建的允许单次获取的对象池</returns>
        public IObjectPool<T> CreateSpawnObjectPool<T>(bool allowMulti, int capacity, int priority) where T : ObjectBase
        {
            return allowMulti ? m_ObjectPoolManager.CreateMultiSpawnObjectPool<T>(capacity, priority) : m_ObjectPoolManager.CreateSingleSpawnObjectPool<T>(capacity, priority);
        }

        /// <summary>
        /// 创建允许单次获取的对象池
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="allowMulti">是否允许多次获取对象</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>要创建的允许单次获取的对象池</returns>
        public ObjectPoolBase CreateSpawnObjectPool(Type objectType, bool allowMulti, int capacity, int priority)
        {
            return allowMulti ? m_ObjectPoolManager.CreateMultiSpawnObjectPool(objectType, capacity, priority) : m_ObjectPoolManager.CreateSingleSpawnObjectPool(objectType, capacity, priority);
        }

        /// <summary>
        /// 创建允许单次获取的对象池
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="allowMulti">是否允许多次获取对象</param>
        /// <param name="expireTime">对象池对象过期秒数</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>要创建的允许单次获取的对象池</returns>
        public IObjectPool<T> CreateSpawnObjectPool<T>(bool allowMulti, float expireTime, int priority) where T : ObjectBase
        {
            return allowMulti ? m_ObjectPoolManager.CreateMultiSpawnObjectPool<T>(expireTime, priority) : m_ObjectPoolManager.CreateSingleSpawnObjectPool<T>(expireTime, priority);
        }

        /// <summary>
        /// 创建允许单次获取的对象池
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="allowMulti">是否允许多次获取对象</param>
        /// <param name="expireTime">对象池对象过期秒数</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>要创建的允许单次获取的对象池</returns>
        public ObjectPoolBase CreateSpawnObjectPool(Type objectType, bool allowMulti, float expireTime, int priority)
        {
            return allowMulti ? m_ObjectPoolManager.CreateMultiSpawnObjectPool(objectType, expireTime, priority) : m_ObjectPoolManager.CreateSingleSpawnObjectPool(objectType, expireTime, priority);
        }

        /// <summary>
        /// 创建允许单次获取的对象池
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="allowMulti">是否允许多次获取对象</param>
        /// <param name="name">对象池名称</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="expireTime">对象池对象过期秒数</param>
        /// <returns>要创建的允许单次获取的对象池</returns>
        public IObjectPool<T> CreateSpawnObjectPool<T>(bool allowMulti, string name, int capacity, float expireTime) where T : ObjectBase
        {
            return allowMulti ? m_ObjectPoolManager.CreateMultiSpawnObjectPool<T>(name, capacity, expireTime) : m_ObjectPoolManager.CreateSingleSpawnObjectPool<T>(name, capacity, expireTime);
        }

        /// <summary>
        /// 创建允许单次获取的对象池
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="allowMulti">是否允许多次获取对象</param>
        /// <param name="name">对象池名称</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="expireTime">对象池对象过期秒数</param>
        /// <returns>要创建的允许单次获取的对象池</returns>
        public ObjectPoolBase CreateSpawnObjectPool(Type objectType, bool allowMulti, string name, int capacity, float expireTime)
        {
            return allowMulti ? m_ObjectPoolManager.CreateMultiSpawnObjectPool(objectType, name, capacity, expireTime) : m_ObjectPoolManager.CreateSingleSpawnObjectPool(objectType, name, capacity, expireTime);
        }

        /// <summary>
        /// 创建允许单次获取的对象池
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="allowMulti">是否允许多次获取对象</param>
        /// <param name="name">对象池名称</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>要创建的允许单次获取的对象池</returns>
        public IObjectPool<T> CreateSpawnObjectPool<T>(bool allowMulti, string name, int capacity, int priority) where T : ObjectBase
        {
            return allowMulti ? m_ObjectPoolManager.CreateMultiSpawnObjectPool<T>(name, capacity, priority) : m_ObjectPoolManager.CreateSingleSpawnObjectPool<T>(name, capacity, priority);
        }

        /// <summary>
        /// 创建允许单次获取的对象池
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="allowMulti">是否允许多次获取对象</param>
        /// <param name="name">对象池名称</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>要创建的允许单次获取的对象池</returns>
        public ObjectPoolBase CreateSpawnObjectPool(Type objectType, bool allowMulti, string name, int capacity, int priority)
        {
            return allowMulti ? m_ObjectPoolManager.CreateMultiSpawnObjectPool(objectType, name, capacity, priority) : m_ObjectPoolManager.CreateSingleSpawnObjectPool(objectType, name, capacity, priority);
        }

        /// <summary>
        /// 创建允许单次获取的对象池
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="allowMulti">是否允许多次获取对象</param>
        /// <param name="name">对象池名称</param>
        /// <param name="expireTime">对象池对象过期秒数</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>要创建的允许单次获取的对象池</returns>
        public IObjectPool<T> CreateSpawnObjectPool<T>(bool allowMulti, string name, float expireTime, int priority) where T : ObjectBase
        {
            return allowMulti ? m_ObjectPoolManager.CreateMultiSpawnObjectPool<T>(name, expireTime, priority) : m_ObjectPoolManager.CreateSingleSpawnObjectPool<T>(name, expireTime, priority);
        }

        /// <summary>
        /// 创建允许单次获取的对象池
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="allowMulti">是否允许多次获取对象</param>
        /// <param name="name">对象池名称</param>
        /// <param name="expireTime">对象池对象过期秒数</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>要创建的允许单次获取的对象池</returns>
        public ObjectPoolBase CreateSpawnObjectPool(Type objectType, bool allowMulti, string name, float expireTime, int priority)
        {
            return allowMulti ? m_ObjectPoolManager.CreateMultiSpawnObjectPool(objectType, name, expireTime, priority) : m_ObjectPoolManager.CreateSingleSpawnObjectPool(objectType, name, expireTime, priority);
        }

        /// <summary>
        /// 创建允许单次获取的对象池
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="allowMulti">是否允许多次获取对象</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="expireTime">对象池对象过期秒数</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>要创建的允许单次获取的对象池</returns>
        public IObjectPool<T> CreateSpawnObjectPool<T>(bool allowMulti, int capacity, float expireTime, int priority) where T : ObjectBase
        {
            return allowMulti ? m_ObjectPoolManager.CreateMultiSpawnObjectPool<T>(capacity, expireTime, priority) : m_ObjectPoolManager.CreateSingleSpawnObjectPool<T>(capacity, expireTime, priority);
        }

        /// <summary>
        /// 创建允许单次获取的对象池
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="allowMulti">是否允许多次获取对象</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="expireTime">对象池对象过期秒数</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>要创建的允许单次获取的对象池</returns>
        public ObjectPoolBase CreateSpawnObjectPool(Type objectType, bool allowMulti, int capacity, float expireTime, int priority)
        {
            return allowMulti ? m_ObjectPoolManager.CreateMultiSpawnObjectPool(objectType, capacity, expireTime, priority) : m_ObjectPoolManager.CreateSingleSpawnObjectPool(objectType, capacity, expireTime, priority);
        }

        /// <summary>
        /// 创建允许单次获取的对象池
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="allowMulti">是否允许多次获取对象</param>
        /// <param name="name">对象池名称</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="expireTime">对象池对象过期秒数</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>要创建的允许单次获取的对象池</returns>
        public IObjectPool<T> CreateSpawnObjectPool<T>(bool allowMulti, string name, int capacity, float expireTime, int priority) where T : ObjectBase
        {
            return allowMulti ? m_ObjectPoolManager.CreateMultiSpawnObjectPool<T>(name, capacity, expireTime, priority) : m_ObjectPoolManager.CreateSingleSpawnObjectPool<T>(name,  capacity, expireTime, priority);
        }

        /// <summary>
        /// 创建允许单次获取的对象池
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="allowMulti">是否允许多次获取对象</param>
        /// <param name="name">对象池名称</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="expireTime">对象池对象过期秒数</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>要创建的允许单次获取的对象池</returns>
        public ObjectPoolBase CreateSpawnObjectPool(Type objectType, bool allowMulti, string name, int capacity, float expireTime, int priority)
        {
            return allowMulti ? m_ObjectPoolManager.CreateMultiSpawnObjectPool(objectType, name, capacity, expireTime, priority) : m_ObjectPoolManager.CreateSingleSpawnObjectPool(objectType, name, capacity, expireTime, priority);
        }

        /// <summary>
        /// 创建允许单次获取的对象池
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="allowMulti">是否允许多次获取对象</param>
        /// <param name="name">对象池名称</param>
        /// <param name="autoReleaseInterval">对象池自动释放可释放对象的间隔秒数</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="expireTime">对象池对象过期秒数</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>要创建的允许单次获取的对象池</returns>
        public IObjectPool<T> CreateSpawnObjectPool<T>(bool allowMulti, string name, float autoReleaseInterval, int capacity, float expireTime, int priority) where T : ObjectBase
        {
            return allowMulti ? m_ObjectPoolManager.CreateMultiSpawnObjectPool<T>(name, autoReleaseInterval, capacity, expireTime, priority) : m_ObjectPoolManager.CreateSingleSpawnObjectPool<T>(name, autoReleaseInterval, capacity, expireTime, priority);
        }

        /// <summary>
        /// 创建允许单次获取的对象池
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="allowMulti">是否允许多次获取对象</param>
        /// <param name="name">对象池名称</param>
        /// <param name="autoReleaseInterval">对象池自动释放可释放对象的间隔秒数</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="expireTime">对象池对象过期秒数</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>要创建的允许单次获取的对象池</returns>
        public ObjectPoolBase CreateSpawnObjectPool(Type objectType, bool allowMulti, string name, float autoReleaseInterval, int capacity, float expireTime, int priority)
        {
            return allowMulti ? m_ObjectPoolManager.CreateMultiSpawnObjectPool(objectType, name, autoReleaseInterval, capacity, expireTime, priority) : m_ObjectPoolManager.CreateSingleSpawnObjectPool(objectType, name, autoReleaseInterval, capacity, expireTime, priority);
        }

        /// <summary>
        /// 销毁对象池
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">要销毁的对象池名称</param>
        /// <returns>是否销毁对象池成功</returns>
        public bool DestroyObjectPool<T>(string name = null) where T : ObjectBase
        {
            return m_ObjectPoolManager.DestroyObjectPool<T>(name);
        }

        /// <summary>
        /// 销毁对象池
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="name">要销毁的对象池名称</param>
        /// <returns>是否销毁对象池成功</returns>
        public bool DestroyObjectPool(Type objectType, string name = null)
        {
            return m_ObjectPoolManager.DestroyObjectPool(objectType, name);
        }

        /// <summary>
        /// 销毁对象池
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="objectPool">要销毁的对象池</param>
        /// <returns>是否销毁对象池成功</returns>
        public bool DestroyObjectPool<T>(IObjectPool<T> objectPool) where T : ObjectBase
        {
            return m_ObjectPoolManager.DestroyObjectPool(objectPool);
        }

        /// <summary>
        /// 销毁对象池
        /// </summary>
        /// <param name="objectPool">要销毁的对象池</param>
        /// <returns>是否销毁对象池成功</returns>
        public bool DestroyObjectPool(ObjectPoolBase objectPool)
        {
            return m_ObjectPoolManager.DestroyObjectPool(objectPool);
        }

        /// <summary>
        /// 释放对象池的可释放对象
        /// </summary>
        public void Release()
        {
            Log.Info("[ObjectPoolComponent.Release] Object pool release unused...");
            m_ObjectPoolManager.Release();
        }

        /// <summary>
        /// 释放对象池中的所有未使用对象
        /// </summary>
        public void ReleaseAllUnused()
        {
            Log.Info("[ObjectPoolComponent.Release] Object pool release all unused...");
            m_ObjectPoolManager.ReleaseAllUnused();
        }
    }
}
