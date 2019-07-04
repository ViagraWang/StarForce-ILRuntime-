using GameFramework;
using GameFramework.Event;
using System;
using UnityEngine;

namespace UnityGameFrame.Runtime
{
    /// <summary>
    /// 事件组件
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Game Framework/Event")]
    public class EventComponent : GameFrameworkComponent
    {
        private IEventManager m_EventManager = null;    //事件管理器

        /// <summary>
        /// 获取事件处理函数的数量
        /// </summary>
        public int EventHandlerCount { get { return m_EventManager.EventHandlerCount; } }

        /// <summary>
        /// 事件的数量
        /// </summary>
        public int EventCount { get { return m_EventManager.EventCount; } }

        protected override void Awake()
        {
            base.Awake();
            m_EventManager = GameFrameworkEntry.GetModule<IEventManager>();
            if(m_EventManager == null)
                Log.Fatal("[EventComponent.Awake] Event manager is invalid -> m_EventManager == null.");
        }

        /// <summary>
        /// 获取事件处理函数的数量
        /// </summary>
        /// <param name="eventId">事件id编号</param>
        /// <returns>处理函数的数量</returns>
        public int Count(int id)
        {
            return m_EventManager.Count(id);
        }

        /// <summary>
        /// 检查是否存在事件处理函数
        /// </summary>
        /// <param name="eventId">事件id编号</param>
        /// <param name="handler">事件处理函数</param>
        /// <returns>是否存在</returns>
        public bool CheckExist(int eventId, EventHandler<GameEventArgs> handler)
        {
            return m_EventManager.Check(eventId, handler);
        }

        /// <summary>
        /// 订阅事件回调
        /// </summary>
        /// <param name="eventId">事件id编号</param>
        /// <param name="handler">事件处理函数</param>
        public void Subscribe(int eventId, EventHandler<GameEventArgs> handler)
        {
            m_EventManager.Subscribe(eventId, handler);
        }

        /// <summary>
        /// 取消订阅事件回调
        /// </summary>
        /// <param name="eventId">事件id编号</param>
        /// <param name="handler">要取消的事件处理函数</param>
        public void Unsubscribe(int eventId, EventHandler<GameEventArgs> handler)
        {
            m_EventManager.Unsubscribe(eventId, handler);
        }

        /// <summary>
        /// 设置默认的事件回调
        /// </summary>
        /// <param name="handler">事件处理函数</param>
        public void SetDefaultHandler(EventHandler<GameEventArgs> handler)
        {
            m_EventManager.SetDefaultHandler(handler);
        }

        /// <summary>
        /// 抛出事件，这个操作是线程安全的，即使不在主线程中抛出，也可保证在主线程中回调事件处理函数，但事件会在抛出后的下一帧分发
        /// </summary>
        /// <param name="sender">事件源</param>
        /// <param name="e">事件参数</param>
        public void Fire(object sender, GameEventArgs e)
        {
            m_EventManager.Fire(sender, e);
        }

        /// <summary>
        /// 抛出事件立即模式，这个操作不是线程安全的，事件会立刻分发
        /// </summary>
        /// <param name="sender">事件源</param>
        /// <param name="e">事件参数</param>
        public void FireNow(object sender, GameEventArgs e)
        {
            m_EventManager.FireNow(sender, e);
        }
    }

}
