using GameFramework;
using System;

using UnityEngine;

namespace UnityGameFrame.Runtime
{
    /// <summary>
    /// 引用池组件，实际用不到
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Game Framework/ReferencePool")]
    public sealed class ReferencePoolComponent : GameFrameworkComponent
    {
        /// <summary>
        /// 引用池的数量
        /// </summary>
        public int Count { get { return ReferencePool.Count; } }

        /// <summary>
        /// 获取所有引用池的信息
        /// </summary>
        /// <returns>所有引用池的信息</returns>
        public ReferencePoolInfo[] GetAllReferencePoolInfos()
        {
            return ReferencePool.GetAllReferencePoolInfos();
        }

        /// <summary>
        /// 清除所有引用池
        /// </summary>
        public void ClearAll()
        {
            ReferencePool.ClearAll();
        }

        /// <summary>
        /// 从引用池获取引用
        /// </summary>
        /// <typeparam name="T">引用类型</typeparam>
        /// <returns>引用泛型</returns>
        public T Acquire<T>() where T : class, IReference, new()
        {
            return ReferencePool.Acquire<T>();
        }

        /// <summary>
        /// 从引用池获取引用
        /// </summary>
        /// <param name="referenceType">引用类型</param>
        /// <returns>引用</returns>
        public IReference Acquire(Type referenceType)
        {
            return ReferencePool.Acquire(referenceType);
        }

        /// <summary>
        /// 将引用归还引用池
        /// </summary>
        /// <typeparam name="T">引用类型</typeparam>
        /// <param name="reference">引用</param>
        public void Release<T>(T reference) where T : class, IReference
        {
            ReferencePool.Release<T>(reference);
        }

        /// <summary>
        /// 将引用归还引用池
        /// </summary>
        /// <param name="reference">引用</param>
        public void Release(IReference reference)
        {
            ReferencePool.Release(reference);
        }

        /// <summary>
        /// 向引用池中追加指定数量的引用
        /// </summary>
        /// <typeparam name="T">引用类型</typeparam>
        /// <param name="count">追加数量</param>
        public void Add<T>(int count) where T : class, IReference, new()
        {
            ReferencePool.Add<T>(count);
        }

        /// <summary>
        /// 向引用池中追加指定数量的引用
        /// </summary>
        /// <param name="referenceType">引用类型</param>
        /// <param name="count">追加数量</param>
        public void Add(Type referenceType, int count)
        {
            ReferencePool.Add(referenceType, count);
        }

        /// <summary>
        /// 从引用池中移除指定数量的引用
        /// </summary>
        /// <typeparam name="T">引用类型</typeparam>
        /// <param name="count">移除数量</param>
        public void Remove<T>(int count) where T : class, IReference
        {
            ReferencePool.Remove<T>(count);
        }

        /// <summary>
        /// 从引用池中移除指定数量的引用
        /// </summary>
        /// <param name="referenceType">引用类型</param>
        /// <param name="count">移除数量</param>
        public void Remove(Type referenceType, int count)
        {
            ReferencePool.Remove(referenceType, count);
        }

        /// <summary>
        /// 从引用池中移除所有的引用
        /// </summary>
        /// <typeparam name="T">引用类型</typeparam>
        public static void RemoveAll<T>() where T : class, IReference
        {
            ReferencePool.RemoveAll<T>();
        }

        /// <summary>
        /// 从引用池中移除所有的引用
        /// </summary>
        /// <param name="referenceType">引用类型</param>
        public static void RemoveAll(Type referenceType)
        {
            ReferencePool.RemoveAll(referenceType);

        }

    }
}