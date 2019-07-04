using GameFramework;
using GameFramework.Fsm;
using GameFramework.Procedure;
using System;
using System.Collections;
using UnityEngine;

namespace UnityGameFrame.Runtime
{
    /// <summary>
    /// 流程组件
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Game Framework/Procedure")]
    public sealed class ProcedureComponent : GameFrameworkComponent
    {
        private IProcedureManager m_ProcedureManager = null;
        private ProcedureBase m_EntranceProcedure = null;

        [SerializeField]
        private string[] m_AvailableProcedureTypeNames = null;  //可使用的流程类型名称

        [SerializeField]
        private string m_EntranceProcedureTypeName = null;  //入口流程类型名

        /// <summary>
        /// 获取当前流程
        /// </summary>
        public ProcedureBase CurrentProcedure { get { return m_ProcedureManager.CurrentProcedure; } }

        protected override void Awake()
        {
            base.Awake();
            m_ProcedureManager = GameFrameworkEntry.GetModule<IProcedureManager>();
            if (m_ProcedureManager == null)
            {
                Log.Fatal("[ProcedureComponent.Awake] Procedure manager is invalid.");
                return;
            }
        }


        private void Start()
        {
            ProcedureBase[] procedures = new ProcedureBase[m_AvailableProcedureTypeNames.Length];
            for (int i = 0; i < m_AvailableProcedureTypeNames.Length; i++)
            {
                //遍历创建流程
                Type procedureType = Utility.Assembly.GetType(m_AvailableProcedureTypeNames[i]);
                if (procedureType == null)
                {
                    Log.Error("[ProcedureComponent.Start] Can not find procedure type '{0}'.", m_AvailableProcedureTypeNames[i]);
                    return;
                }
                procedures[i] = (ProcedureBase)Activator.CreateInstance(procedureType);
                if (procedures[i] == null)
                {
                    Log.Error("[ProcedureComponent.Start] Can not create procedure instance '{0}'.", m_AvailableProcedureTypeNames[i]);
                    return;
                }

                //保存入口流程
                if (m_EntranceProcedureTypeName == m_AvailableProcedureTypeNames[i])
                    m_EntranceProcedure = procedures[i];
            }

            if (m_EntranceProcedure == null)
            {
                Log.Error("[ProcedureComponent.Start] Entrance procedure is invalid.");
                return;
            }
            StartInitProcedure(procedures, m_EntranceProcedure);
        }

        //开始初始化流程
        public void StartInitProcedure(ProcedureBase[] procedures, ProcedureBase entranceProcedure)
        {
            StartCoroutine(InitProcedures(procedures, entranceProcedure));
        }

        //协程初始化流程，如果不等待一帧，进入ProcedureLaunch时，初始化辅助器的操作还未完成
        private IEnumerator InitProcedures(ProcedureBase[] procedures, ProcedureBase entranceProcedure)
        {
            m_ProcedureManager.Initialize(GameFrameworkEntry.GetModule<IFsmManager>(), procedures);    //初始化流程管理器
            yield return UnityExtension.waitOneFrame;
            m_ProcedureManager.StartProcedure(entranceProcedure.GetType());   //开始入口流程

        }

        /// <summary>
        /// 是否存在流程
        /// </summary>
        /// <typeparam name="T">流程类型</typeparam>
        /// <returns>是否存在流程</returns>
        public bool HasProcedure<T>() where T : ProcedureBase
        {
            return m_ProcedureManager.HasProcedure<T>();
        }

        /// <summary>
        /// 获取流程
        /// </summary>
        /// <returns>要获取的流程</returns>
        /// <typeparam name="T">流程类型</typeparam>
        public ProcedureBase GetProcedure<T>() where T : ProcedureBase
        {
            return m_ProcedureManager.GetProcedure<T>();
        }
    }
}
