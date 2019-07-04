//=======================================================
// 作者：
// 描述：组件扩展工具
//=======================================================
using GameFramework.Resource;
using System;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityGameFrame.Runtime;
using Object = UnityEngine.Object;

namespace Game.Runtime
{	
    /// <summary>
    /// 组件扩展工具，因为ILRuntime中最好不要使用Unity工程中的泛型方法，所以组件的操作使用这个类的扩展方法
    /// </summary>
	public static class ComponentExtension
    {

        #region Button

        //给按钮添加点击回调
        public static void ButtonAddClick(this Button button, UnityAction callBack)
        {
            button.onClick.AddListener(callBack);
        }

        //清空按钮回调
        public static void ButtonClearClick(this Button button)
        {
            button.onClick.RemoveAllListeners();
        }


        #endregion

        #region Toggle

        //给Toggle添加点击回调
        public static void ToggleAddChanged(this Toggle toggle, UnityAction<bool> callback)
        {
            toggle.onValueChanged.AddListener(callback);
        }

        //清空Toggle回调
        public static void ToggleClearChanged(this Toggle toggle)
        {
            toggle.onValueChanged.RemoveAllListeners();
        }


        #endregion


        #region Slider

        //给Slider添加拖动回调
        public static void SliderAddChanged(this Slider slider, UnityAction<float> callback)
        {
            slider.onValueChanged.AddListener(callback);
        }

        //给Slider添加拖动回调
        public static void SliderClearChanged(this Slider slider)
        {
            slider.onValueChanged.RemoveAllListeners();
        }


        #endregion


        #region CommonButton

        //给CommonButton添加进入回调
        public static void ComButtonAddEnter(this CommonButton button, UnityAction action)
        {
            button.EnterAddListener(action);
        }

        //给CommonButton移除所有进入回调
        public static void ComButtonClearEnter(this CommonButton button)
        {
            button.EnterRemoveAllListeners();
        }

        //给CommonButton添加离开回调
        public static void ComButtonAddExit(this CommonButton button, UnityAction action)
        {
            button.ExitAddListener(action);
        }

        //给CommonButton移除所有离开回调
        public static void ComButtonClearExit(this CommonButton button)
        {
            button.ExitRemoveAllListeners();
        }

        //给CommonButton添加按下回调
        public static void ComButtonAddDown(this CommonButton button, UnityAction action)
        {
            button.DownAddListener(action);
        }

        //给CommonButton移除所有按下回调
        public static void ComButtonClearDown(this CommonButton button)
        {
            button.DownRemoveAllListeners();
        }

        //给CommonButton添加抬起回调
        public static void ComButtonAddUp(this CommonButton button, UnityAction action)
        {
            button.UpAddListener(action);
        }

        //给CommonButton移除所有抬起回调
        public static void ComButtonClearUp(this CommonButton button)
        {
            button.UpRemoveAllListeners();
        }

        //给CommonButton添加点击回调
        public static void ComButtonAddClick(this CommonButton button, UnityAction action)
        {
            button.ClickAddListener(action);
        }

        //给CommonButton移除所有点击回调
        public static void ComButtonClearClick(this CommonButton button)
        {
            button.ClickRemoveAllListeners();
        }

        #endregion


        #region RuntimeComponent

        /// <summary>
        /// 封装的加载资源方法
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="priority"></param>
        /// <param name="loadAssetSuccessCallbacks"></param>
        /// <param name="loadAssetFailureCallbacks"></param>
        /// <param name="userData"></param>
        public static void LoadAsset(this ResourceComponent resource, string assetName, int priority, Action<string, object, float, object> loadAssetSuccessCallbacks, Action<string, string, string, object> loadAssetFailureCallbacks, object userData = null)
        {
            resource.LoadAsset(assetName, priority,
                new LoadAssetCallbacks(
                    (assetName0, asset0, duration0, userData0) => { loadAssetSuccessCallbacks.Invoke(assetName0, asset0, duration0, userData0); },
                    (assetName0, status0, errorMessage0, userData0) => { loadAssetFailureCallbacks.Invoke(assetName0, status0.ToString(), errorMessage0, userData0); }
                    ),
                userData);
        }

        #endregion

    }
}