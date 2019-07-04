using Game.Runtime;
using GameFramework;
using System;
using System.Collections.Generic;

namespace Game.Hotfix
{
    /// <summary>
    /// 引用池
    /// </summary>
    public static partial class ReferencePool
    {

        private static readonly IDictionary<string, ReferenceCollection> s_ReferenceCollections = new Dictionary<string, ReferenceCollection>();    //引用池字典

        /// <summary>
        /// 获取引用池数量
        /// </summary>
        public static int Count { get { return s_ReferenceCollections.Count; } }

        /// <summary>
        /// 获取所有引用池的信息
        /// </summary>
        /// <returns>所有引用池的信息</returns>
        public static ReferencePoolInfo[] GetAllReferencePoolInfos()
        {
            int index = 0;
            ReferencePoolInfo[] results = null;
            lock (s_ReferenceCollections)
            {
                results = new ReferencePoolInfo[s_ReferenceCollections.Count];
                foreach (var item in s_ReferenceCollections)
                {
                    results[index++] = new ReferencePoolInfo(item.Key, item.Value.UnusedReferenceCount, item.Value.UsingReferenceCount, item.Value.AcquireReferenceCount, item.Value.ReleaseReferenceCount, item.Value.AddReferenceCount, item.Value.RemoveReferenceCount);
                }
            }
            return results;
        }

        /// <summary>
        /// 清除所有引用池
        /// </summary>
        public static void ClearAll()
        {
            lock (s_ReferenceCollections)
            {
                foreach (var item in s_ReferenceCollections.Values)
                {
                    item.RemoveAll();
                }
                s_ReferenceCollections.Clear();
            }
        }

        /// <summary>
        /// 从引用池获取引用
        /// </summary>
        /// <typeparam name="T">引用类型</typeparam>
        /// <returns>引用泛型</returns>
        public static T Acquire<T>() where T : class, IReference, new()
        {
            return GetReferenceCollection(typeof(T)).Acquire<T>();
        }

        /// <summary>
        /// 从引用池获取引用
        /// </summary>
        /// <param name="referenceType">引用类型</param>
        /// <returns>引用</returns>
        public static IReference Acquire(Type referenceType)
        {
            InternalCheckReferenceType(referenceType);
            return GetReferenceCollection(referenceType).Acquire();
        }

        /// <summary>
        /// 将引用归还引用池
        /// </summary>
        /// <typeparam name="T">引用类型</typeparam>
        /// <param name="reference">引用</param>
        public static void Release<T>(T reference) where T : class, IReference
        {
            if (reference == null)
                throw new Exception("[ReferencePool.Release<T>] Reference is invalid -> reference == null");

            GetReferenceCollection(typeof(T)).Release(reference);
        }

        /// <summary>
        /// 将引用归还引用池
        /// </summary>
        /// <param name="reference">引用</param>
        public static void Release(IReference reference)
        {
            if (reference == null)
                throw new Exception("[ReferencePool.Release] Reference is invalid -> reference == null");
            Type referenceType = reference.GetType();
            InternalCheckReferenceType(referenceType);
            GetReferenceCollection(referenceType).Release(reference);
        }

        /// <summary>
        /// 向引用池中追加指定数量的引用
        /// </summary>
        /// <typeparam name="T">引用类型</typeparam>
        /// <param name="count">追加数量</param>
        public static void Add<T>(int count) where T : class, IReference, new()
        {
            GetReferenceCollection(typeof(T)).Add<T>(count);
        }

        /// <summary>
        /// 向引用池中追加指定数量的引用
        /// </summary>
        /// <param name="referenceType">引用类型</param>
        /// <param name="count">追加数量</param>
        public static void Add(Type referenceType, int count)
        {
            InternalCheckReferenceType(referenceType);
            GetReferenceCollection(referenceType).Add(count);
        }

        /// <summary>
        /// 从引用池中移除指定数量的引用
        /// </summary>
        /// <typeparam name="T">引用类型</typeparam>
        /// <param name="count">移除数量</param>
        public static void Remove<T>(int count) where T : class, IReference
        {
            GetReferenceCollection(typeof(T)).Remove(count);
        }

        /// <summary>
        /// 从引用池中移除指定数量的引用
        /// </summary>
        /// <param name="referenceType">引用类型</param>
        /// <param name="count">移除数量</param>
        public static void Remove(Type referenceType, int count)
        {
            InternalCheckReferenceType(referenceType);
            GetReferenceCollection(referenceType).Remove(count);
        }

        /// <summary>
        /// 从引用池中移除所有的引用
        /// </summary>
        /// <typeparam name="T">引用类型</typeparam>
        public static void RemoveAll<T>() where T : class, IReference
        {
            GetReferenceCollection(typeof(T)).RemoveAll();
        }

        /// <summary>
        /// 从引用池中移除所有的引用
        /// </summary>
        /// <param name="referenceType">引用类型</param>
        public static void RemoveAll(Type referenceType)
        {
            InternalCheckReferenceType(referenceType);
            GetReferenceCollection(referenceType).RemoveAll();
        }

        //内部检查引用
        private static void InternalCheckReferenceType(Type referenceType)
        {
            if(referenceType == null)
                throw new Exception("[ReferencePool.InternalCheckReferenceType] Reference Type is invalid -> referenceType == null");

            if(!referenceType.IsClass || referenceType.IsAbstract)
                throw new Exception("[ReferencePool.InternalCheckReferenceType] Reference type is not a non-abstract class type.");

            if(!typeof(IReference).IsAssignableFrom(referenceType))
                throw new Exception(Utility.Text.Format("[ReferencePool.InternalCheckReferenceType] Reference type '{0}' is invalid.", referenceType.FullName));

        }

        //获取引用池
        private static ReferenceCollection GetReferenceCollection(Type referenceType)
        {
            if (referenceType == null)
            {
                throw new Exception("[ReferencePool.GetReferenceCollection] ReferenceType is invalid -> referenceType == null");
            }

            string fullName = referenceType.FullName;
            ReferenceCollection referenceCollection = null;
            lock (s_ReferenceCollections)
            {
                if(!s_ReferenceCollections.TryGetValue(fullName, out referenceCollection))
                {
                    referenceCollection = new ReferenceCollection(referenceType);    //不存在则创建
                    s_ReferenceCollections.Add(fullName, referenceCollection);
                }
            }
            return referenceCollection;
        }

    }
}
