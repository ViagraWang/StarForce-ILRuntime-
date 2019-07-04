using System;
using Game.Runtime;
using UnityEngine;

namespace Game.Hotfix
{
    //热更新入口
    public static class HotfixEntry
    {
        public static GameObject GlobalObj = null;  //框架对象

        /// <summary>
        /// 逻辑帧时间
        /// </summary>
        public static float deltaTime { get; private set; } = 0f;

        /// <summary>
        /// 真实帧时间
        /// </summary>
        public static float unscaleDeltaTime { get; private set; } = 0f;

        //对象池
        internal static ObjectPoolManager ObjectPool { get; private set; }

        //血条管理
        internal static HPBarComponent HPBar { get; private set; }



        public static void Start(GameObject frameObj)
        {
            HotLog.Debug("热更新启动");
            GlobalObj = frameObj;

            ObjectPool = new ObjectPoolManager();
            HPBar = new HPBarComponent();

            //注册回调函数
            GameEntry.Hotfix.OnUpdate = Update;
            GameEntry.Hotfix.OnLateUpdate = LateUpdate;
            GameEntry.Hotfix.OnApplication = ApplicationQuit;

        }

        public static void Update(float elapseSeconds, float realElapseSeconds)
        {
            try
            {
                deltaTime = elapseSeconds;
                unscaleDeltaTime = realElapseSeconds;

                ObjectPool.Update(deltaTime, unscaleDeltaTime);

                HPBar.Update();

            }
            catch (Exception)
            {

                throw;
            }
        }

        public static void LateUpdate()
        {
            try
            {

                //HotLog.Debug("Hotfix LateUpdate...");
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static void ApplicationQuit()
        {
            try
            {
                ObjectPool.Shutdown();
                ReferencePool.ClearAll();
                HotLog.Debug("Hotfix ApplicationQuit...");

            }
            catch (Exception)
            {

                throw;
            }
        }
    }

}
