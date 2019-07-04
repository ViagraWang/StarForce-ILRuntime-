using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Hotfix
{
    /// <summary>
    /// 引用池信息
    /// </summary>
    public sealed class ReferencePoolInfo
    {

        private readonly string m_TypeName;
        private readonly int m_UnusedReferenceCount;
        private readonly int m_UsingReferenceCount;
        private readonly int m_AcquireReferenceCount;
        private readonly int m_ReleaseReferenceCount;
        private readonly int m_AddReferenceCount;
        private readonly int m_RemoveReferenceCount;

        /// <summary>
        /// 引用类型名称
        /// </summary>
        public string TypeName { get { return m_TypeName; } }

        /// <summary>
        /// 未使用引用数量
        /// </summary>
        public int UnusedReferenceCount { get { return m_UnusedReferenceCount; } }

        /// <summary>
        /// 正在使用引用数量
        /// </summary>
        public int UsingReferenceCount { get { return m_UsingReferenceCount; } }

        /// <summary>
        /// 获取引用数量
        /// </summary>
        public int AcquireReferenceCount { get { return m_AcquireReferenceCount; } }

        /// <summary>
        /// 释放引用数量
        /// </summary>
        public int ReleaseReferenceCount { get { return m_ReleaseReferenceCount; } }

        /// <summary>
        /// 增加的引用数量
        /// </summary>
        public int AddReferenceCount { get { return m_AddReferenceCount; } }

        /// <summary>
        /// 移除的引用数量
        /// </summary>
        public int RemoveReferenceCount { get { return m_RemoveReferenceCount; } }


        /// <summary>
        /// 初始化引用池信息的新实例
        /// </summary>
        /// <param name="typeName">引用池类型名称</param>
        /// <param name="unusedReferenceCount">未使用引用数量</param>
        /// <param name="usingReferenceCount">正在使用引用数量</param>
        /// <param name="acquireReferenceCount">获取引用数量</param>
        /// <param name="releaseReferenceCount">释放引用数量</param>
        /// <param name="addReferenceCount">增加引用数量</param>
        /// <param name="removeReferenceCount">移除引用数量</param>
        public ReferencePoolInfo(string typeName, int unusedReferenceCount, int usingReferenceCount, int acquireReferenceCount, int releaseReferenceCount, int addReferenceCount, int removeReferenceCount)
        {
            m_TypeName = typeName;
            m_UnusedReferenceCount = unusedReferenceCount;
            m_UsingReferenceCount = usingReferenceCount;
            m_AcquireReferenceCount = acquireReferenceCount;
            m_ReleaseReferenceCount = releaseReferenceCount;
            m_AddReferenceCount = addReferenceCount;
            m_RemoveReferenceCount = removeReferenceCount;
        }

    }
}
