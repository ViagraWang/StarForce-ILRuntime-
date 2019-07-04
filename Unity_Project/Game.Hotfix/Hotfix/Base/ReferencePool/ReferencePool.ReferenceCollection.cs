using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Hotfix
{
    public static partial class ReferencePool
    {
        //引用池容器
        private sealed class ReferenceCollection
        {
            private readonly Queue<IReference> m_References;    //引用队列
            private readonly Type m_ReferenceType;  //引用的类型
            //未使用的引用的数量
            public int UnusedReferenceCount { get { return m_References.Count; } }
            //引用的类型
            public Type ReferenceType { get { return m_ReferenceType; } }
            //使用中的引用数量
            public int UsingReferenceCount { get; private set; }
            //获得的引用数量
            public int AcquireReferenceCount { get; private set; }
            //释放的引用数量
            public int ReleaseReferenceCount { get; private set; }
            //添加的引用数量
            public int AddReferenceCount { get; private set; }
            //移除的引用数量
            public int RemoveReferenceCount { get; private set; }
            
            //构造器
            public ReferenceCollection(Type referenceType)
            {
                m_References = new Queue<IReference>();
                m_ReferenceType = referenceType;
                UsingReferenceCount = 0;
                AcquireReferenceCount = 0;
                ReleaseReferenceCount = 0;
                AddReferenceCount = 0;
                RemoveReferenceCount = 0;
            }

            //获取引用
            public T Acquire<T>() where T : class, IReference, new()
            {
                if(typeof(T) != m_ReferenceType)
                    throw new Exception("[ReferenceCollection.Acquire<T>] Type is invalid.");

                UsingReferenceCount++;
                AcquireReferenceCount++;
                lock (m_References)
                {
                    if (m_References.Count > 0)
                        return (T)m_References.Dequeue();
                }
                //如果不存在则创建
                AddReferenceCount++;
                return new T();
            }

            //获取引用
            public IReference Acquire()
            {
                AcquireReferenceCount++;
                lock (m_References)
                {
                    if (m_References.Count > 0)
                        return m_References.Dequeue();
                }

                AddReferenceCount++;
                return Activator.CreateInstance(m_ReferenceType) as IReference;
            }

            //添加引用
            public void Add<T>(int count) where T : class, IReference, new()
            {
                if (typeof(T) != m_ReferenceType)
                    throw new Exception("[ReferenceCollection.Add<T>] Type is invalid.");

                lock (m_References)
                {
                    AddReferenceCount += count;
                    while(count-- > 0)
                    {
                        m_References.Enqueue(new T());
                    }
                }
            }

            //添加引用
            public void Add(int count)
            {
                lock (m_References)
                {
                    AddReferenceCount += count;
                    while (count-- > 0)
                    {
                        m_References.Enqueue((IReference)Activator.CreateInstance(m_ReferenceType));
                    }
                }
            } 

            //释放引用
            public void Release(IReference reference)
            {
                reference.Clear();
                lock (m_References)
                {
                    if(m_References.Contains(reference))
                        throw new Exception("[ReferenceCollection.Release] The reference has been released.");
                    m_References.Enqueue(reference);
                }
                ReleaseReferenceCount++;
                UsingReferenceCount--;
            }

            //移除引用
            public void Remove(int count)
            {
                lock (m_References)
                {
                    if (count > m_References.Count)
                        count = m_References.Count;

                    RemoveReferenceCount += count;
                    while (count-- > 0)
                    {
                        m_References.Dequeue();
                    }
                }
            }

            //移除所有
            public void RemoveAll()
            {
                lock (m_References)
                {
                    RemoveReferenceCount += m_References.Count;
                    m_References.Clear();
                }
            }

        }

    }
}
