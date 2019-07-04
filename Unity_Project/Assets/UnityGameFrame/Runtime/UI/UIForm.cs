using GameFramework.UI;
using UnityEngine;

namespace UnityGameFrame.Runtime
{
    public sealed class UIForm : MonoBehaviour, IUIForm
    {
        /// <summary>
        /// 界面序列编号
        /// </summary>
        public int SerialId { get; private set; }

        /// <summary>
        /// 界面资源名称
        /// </summary>
        public string UIFormAssetName { get; private set; }

        /// <summary>
        /// 界面对象
        /// </summary>
        public object Handle { get { return gameObject; } }

        /// <summary>
        /// //界面组
        /// </summary>
        public IUIGroup UIGroup { get; private set; }

        /// <summary>
        /// 在界面组中的深度
        /// </summary>
        public int DepthInUIGroup { get; private set; }

        /// <summary>
        /// 是否暂定覆盖的界面
        /// </summary>
        public bool PauseCoveredUIForm { get; private set; }

        /// <summary>
        /// 缓存
        /// </summary>
        public Transform CachedTransform { get; private set; }

        /// <summary>
        /// 界面控制器
        /// </summary>
        public UIFormLogic Logic { get; private set; }

        /// <summary>
        /// 初始化界面
        /// </summary>
        /// <param name="serialId">界面序列编号</param>
        /// <param name="uiFormAssetName">界面资源名称</param>
        /// <param name="uiGroup">界面所处的界面组</param>
        /// <param name="pauseCoveredUIForm">是否暂停被覆盖的界面</param>
        /// <param name="isNewInstance">是否是新实例</param>
        /// <param name="userData">用户自定义数据</param>
        public void OnInit(int serialId, string uiFormAssetName, IUIGroup uiGroup, bool isPauseConveredUIForm, bool isNewInstance, object userData)
        {
            SerialId = serialId;
            UIFormAssetName = uiFormAssetName;
            DepthInUIGroup = 0; //初始在界面组中的深度为0
            PauseCoveredUIForm = isPauseConveredUIForm;
            CachedTransform = transform;

            if (isNewInstance)
                UIGroup = uiGroup;  //新创建的实例要保存新的对象组???
            else
            {
                if (UIGroup != uiGroup)
                    Log.Error("[UIForm.OnInit] UI group is inconsistent for non-new-instance UI form.");
                return;
            }

            Logic = gameObject.GetOrAddComponent<UIFormLogic>();
            if(Logic == null)
            {
                Log.Error("[UIForm.OnInit] UI form '{0}' can not get UI form logic.", uiFormAssetName);
                return;
            }

            Logic.OnInit(this, userData);
        }

        /// <summary>
        /// 界面回收
        /// </summary>
        public void OnRecycle()
        {
            SerialId = 0;
            DepthInUIGroup = 0;
            PauseCoveredUIForm = true;
        }

        /// <summary>
        /// 界面打开
        /// </summary>
        /// <param name="userData">用户自定义数据</param>
        public void OnOpen(object userData)
        {
            Logic.OnOpen(userData);
        }

        /// <summary>
        /// 界面关闭
        /// </summary>
        /// <param name="userData">用户自定义数据</param>
        public void OnClose(object userData)
        {
            Logic.OnClose(userData);
        }

        /// <summary>
        /// 界面暂停
        /// </summary>
        public void OnPause()
        {
            Logic.OnPause();
        }

        /// <summary>
        /// 界面暂停恢复
        /// </summary>
        public void OnResume()
        {
            Logic.OnResume();
        }

        /// <summary>
        /// 界面覆盖
        /// </summary>
        public void OnCover()
        {
            Logic.OnCover();
        }

        /// <summary>
        /// 界面覆盖恢复
        /// </summary>
        public void OnReveal()
        {
            Logic.OnReveal();
        }

        /// <summary>
        /// 界面激活
        /// </summary>
        /// <param name="userData">用户自定义数据</param>
        public void OnRefocus(object userData)
        {
            Logic.OnRefocus(userData);
        }

        /// <summary>
        /// 界面深度改变
        /// </summary>
        /// <param name="uiGroupDepth">界面组深度</param>
        /// <param name="depthInUIGroup">界面在界面组中的深度</param>
        public void OnDepthChanged(int uiGroupDepth, int depthInUIGroup)
        {
            DepthInUIGroup = depthInUIGroup;
            Logic.OnDepthChanged(uiGroupDepth, depthInUIGroup);
        }

        /// <summary>
        /// 界面轮询更新
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位</param>
        public void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            Logic.OnUpdate(elapseSeconds, realElapseSeconds);
        }

    }
}
