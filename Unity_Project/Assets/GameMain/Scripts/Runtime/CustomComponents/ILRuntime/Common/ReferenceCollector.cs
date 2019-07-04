//=======================================================
// 作者：
// 描述：绑定组件对象，方便Hotfix中直接引用
//=======================================================
using GameFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFrame.Runtime;

namespace Game.Runtime
{
    [AddComponentMenu("Game Main/Reference Collector")]   //组件列表菜单
    [DisallowMultipleComponent]
    public sealed partial class ReferenceCollector : MonoBehaviour, ISerializationCallbackReceiver
    {
        [SerializeField]
        [HideInInspector]
        private List<ReferenceCollectorData> m_ReferenceObjects = new List<ReferenceCollectorData>(); //引用的对象

        //排序
        public void SortObjects()
        {
            m_ReferenceObjects.Sort(new ReferenceCollectorDataComparer());
        }

        //组件视图
        private ComponentView m_ComponentView = null;

        public ComponentView ComponentView
        {
            get
            {
                if (m_ComponentView == null)
                    m_ComponentView = gameObject.AddComponent<ComponentView>();

                return m_ComponentView;
            }
        }

        //实际存储的对象
        private Dictionary<string, GameObject> m_Dict = new Dictionary<string, GameObject>();

        private Transform m_Transform;
        public Transform CachedTransform
        {
            get
            {
                if (m_Transform == null)
                    m_Transform = transform;
                return m_Transform;
            }
        }

        private GameObject m_GameObject;
        public GameObject CachedGameObject
        {
            get
            {
                if (m_GameObject == null)
                    m_GameObject = gameObject;
                return m_GameObject;
            }
        }

        /// <summary>
        /// 当前保存的组件数量
        /// </summary>
        public int Count { get { return m_Dict.Count; } }


        #region 获取组件

        //使用泛型返回对应key的Object
        public Component Get(string key, Type type)
        {
            GameObject obj = GetGO(key);
            if (obj == null)
                return null;
            return obj.GetComponent(type);
        }

        //获取对象
        public GameObject GetGO(string key)
        {
            GameObject dictGo;
            if (!m_Dict.TryGetValue(key, out dictGo))
            {
                Log.Error(Utility.Text.Format("[HotUIForm.GetGo] HotUIForm中不存在对象名{0}", key));
                return null;
            }
            return dictGo;
        }

        public void OnBeforeSerialize()
        {

        }

        public void OnAfterDeserialize()
        {
            m_Dict.Clear();
            for (int i = 0; i < m_ReferenceObjects.Count; i++)
            {
                ReferenceCollectorData data = m_ReferenceObjects[i];
                if (m_Dict.ContainsKey(data.key))
                    m_Dict[data.key] = data.obj;    //存在相同的则覆盖
                else
                    m_Dict.Add(data.key, data.obj);
            }
#if !UNITY_EDITOR
                        m_ReferenceObjects.Clear();
                        m_ReferenceObjects = null;
#endif
        }

        #endregion

    }
}