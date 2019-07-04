using GameFramework.UI;
using UnityEngine;

namespace UnityGameFrame.Runtime
{
    /// <summary>
    /// 默认界面辅助器
    /// </summary>
    public class DefaultUIFormHelper : UIFormHelperBase
    {
        private ResourceComponent m_ResourceComponent = null;   //资源组件

        private void Start()
        {
            m_ResourceComponent = GameEntry.GetComponent<ResourceComponent>();
            if (m_ResourceComponent == null)
            {
                Log.Fatal("[DefaultUIFormHelper.Start] Resource component is invalid.");
                return;
            }
        }

        /// <summary>
        /// 创建界面，将界面放置到界面组下，然后添加或获取UIForm
        /// </summary>
        /// <param name="uiFormInstance">界面实例</param>
        /// <param name="uiGroup">界面所属的界面组</param>
        /// <param name="userData">用户自定义数据</param>
        /// <returns></returns>
        public override IUIForm CreateUIForm(object uiFormInstance, IUIGroup uiGroup, object userData)
        {
            GameObject obj = uiFormInstance as GameObject;
            if(obj == null)
            {
                Log.Error("[DefaultUIFormHelper.CreateUIForm] UI form instance is invalid.");
                return null;
            }

            Transform trans = obj.transform;
            trans.SetParent((uiGroup.Helper as MonoBehaviour).transform);
            trans.localScale = Vector3.one;

            return obj.GetOrAddComponent<UIForm>();
        }

        /// <summary>
        /// 实例化界面
        /// </summary>
        /// <param name="uiFormAsset">要实例化的界面资源</param>
        /// <returns>实例化后的界面</returns>
        public override object InstantiateUIForm(object uiFormAsset)
        {
            return Instantiate((Object)uiFormAsset);
        }

        /// <summary>
        /// 释放界面
        /// </summary>
        /// <param name="uiFormAsset">要释放的界面资源</param>
        /// <param name="uiFormInstance">要释放的界面对象实例</param>
        public override void ReleaseUIForm(object uiFormAsset, object uiFormInstance)
        {
            m_ResourceComponent.UnloadAsset(uiFormAsset);   //卸载资源
            Destroy((Object)uiFormInstance);
        }
    }
}
