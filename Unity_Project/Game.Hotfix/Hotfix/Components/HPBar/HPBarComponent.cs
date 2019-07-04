using Game.Runtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Game.Hotfix
{
    //后期这种自定义组件可以整合到prefab，然后预加载并关联脚本
    public class HPBarComponent
    {
        private GameObject m_HPBarItemTemplate = null;   //血条模板

        private Transform m_HPBarInstanceRoot = null;   //血条实体根对象

        private int m_InstancePoolCapacity = 16;    //对象池容量

        private IObjectPool<HPBarItemObject> m_HPBarItemObjectPool = null;  //血条对象池
        private List<HPBarItem> m_ListActiveHPBar = null;   //激活的血条
        private Canvas m_CachedCanvas = null;   //画布

        //这种方法后期可以抽象出去，例如使用接口、使用特性获取调用等
        public HPBarComponent()
        {
            GameObject obj = new GameObject("HP Bar");
            obj.transform.SetParent(HotfixEntry.GlobalObj.transform.Find("Customs"));
            (obj.AddComponent(typeof(ComponentView)) as ComponentView).Component = this;

            GameObject InstaceObj = new GameObject("HP Bar Instance");
            InstaceObj.transform.SetParent(obj.transform);

            m_CachedCanvas = InstaceObj.AddComponent(typeof(Canvas)) as Canvas;
            m_CachedCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = InstaceObj.AddComponent(typeof(CanvasScaler)) as CanvasScaler;
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0;

            m_HPBarInstanceRoot = InstaceObj.transform;
            if (m_HPBarInstanceRoot == null)
            {
                HotLog.Error("You must set HP bar instance root first.");
                return;
            }

            m_HPBarItemObjectPool = HotfixEntry.ObjectPool.CreateSingleSpawnObjectPool<HPBarItemObject>("HPBarItem", m_InstancePoolCapacity);
            m_ListActiveHPBar = new List<HPBarItem>();

            GameEntry.Resource.LoadAsset(HotAssetUtility.GetUIItemsAsset("HPBarItem"), RuntimeConstant.AssetPriority.UIFormAsset,
                (assetName, asset, duration, userData) =>
                {
                    HotLog.Info("Load HPBarItem OK.");
                    m_HPBarItemTemplate = asset as GameObject;
                },
                    //加载失败的回调
                    (assetName, status, errorMessage, userData) =>
                    {
                        HotLog.Error("Can not load HPBarItem from '{0}' with error message '{1}'.", assetName, errorMessage);
                    });
        }

        public void Update()
        {
            for (int i = m_ListActiveHPBar.Count - 1; i >= 0; i--)
            {
                HPBarItem hpBarItem = m_ListActiveHPBar[i];
                if (hpBarItem.Refresh())
                    continue;

                HideHPBar(hpBarItem);   //隐藏血条
            }
        }

        //显示实体血条
        public void ShowHPBar(Entity entity, float fromHPRatio, float toHPRatio)
        {
            if (entity == null)
            {
                HotLog.Warning("Entity is invalid.");
                return;
            }

            HPBarItem hpBarItem = GetActiveHPBarItem(entity);
            if (hpBarItem == null)
            {
                hpBarItem = CreateHPbarItem();
                m_ListActiveHPBar.Add(hpBarItem);
            }

            hpBarItem.Init(entity, m_CachedCanvas, fromHPRatio, toHPRatio);
        }

        //隐藏血条
        private void HideHPBar(HPBarItem hpBarItem)
        {
            hpBarItem.Reset();
            m_ListActiveHPBar.Remove(hpBarItem);
            m_HPBarItemObjectPool.Unspawn(hpBarItem);

        }

        //获取实体激活的血条
        private HPBarItem GetActiveHPBarItem(Entity entity)
        {
            if (entity == null)
                return null;

            for (int i = 0; i < m_ListActiveHPBar.Count; i++)
            {
                if (m_ListActiveHPBar[i].Owner == entity)
                    return m_ListActiveHPBar[i];
            }

            return null;
        }

        //创建实体血条
        private HPBarItem CreateHPbarItem()
        {
            HPBarItem hpBarItem;
            //先从对象池获取
            HPBarItemObject hpBarItemObject = m_HPBarItemObjectPool.Spawn();
            if (hpBarItemObject != null)
            {
                hpBarItem = hpBarItemObject.Target as HPBarItem;
            }
            else
            {
                //对象池中不存在，则创建新的
                ReferenceCollector referenceCollector = Object.Instantiate(m_HPBarItemTemplate).GetComponent(typeof(ReferenceCollector)) as ReferenceCollector;
                hpBarItem = new HPBarItem(referenceCollector);

                referenceCollector.CachedTransform.SetParent(m_HPBarInstanceRoot, false);
                m_HPBarItemObjectPool.Register(new HPBarItemObject(hpBarItem), true);
            }

            return hpBarItem;
        }

    }
}
