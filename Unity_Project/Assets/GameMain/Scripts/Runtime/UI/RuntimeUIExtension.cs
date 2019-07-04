//=======================================================
// 作者：
// 描述：扩展工具
//=======================================================
using GameFramework;
using GameFramework.DataTable;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFrame.Runtime;

namespace Game.Runtime
{	
	public static class RuntimeUIExtension
    {
        //透明度渐变
        public static IEnumerator FadeToAlpha(this CanvasGroup canvasGroup, float alpha, float duration)
        {
            float time = 0f;
            float originalAlpha = canvasGroup.alpha;
            while (time < duration)
            {
                time += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(originalAlpha, alpha, time / duration);
                yield return null;
            }

            canvasGroup.alpha = alpha;
        }

        //滑动条渐变
        public static IEnumerator SmoothValue(this Slider slider, float value, float duration)
        {
            float time = 0f;
            float originalValue = slider.value;
            while (time < duration)
            {
                time += Time.deltaTime;
                slider.value = Mathf.Lerp(originalValue, value, time / duration);
                yield return null;
            }

            slider.value = value;
        }

        //打开UI界面
        public static int? OpenRuntimeUIForm(this UIComponent uiComponent, int uiFormId, object userData = null)
        {
            //先获取UI配置表数据
            IDataTable<DRUIForm> dtUIForm = GameEntry.DataTable.GetDataTable<DRUIForm>();
            DRUIForm drUIForm = dtUIForm.GetDataRow(uiFormId);
            if (drUIForm == null)
                return null;

            //获取资源路径
            string assetName = RuntimeAssetUtility.GetUIFormAsset(drUIForm.AssetName);
            if (!drUIForm.AllowMultiInstance)   //不允许存在多个界面实例
            {
                if (uiComponent.IsLoadingUIForm(assetName)) //正在加载
                    return null;

                UIForm uiForm = uiComponent.GetUIForm(assetName);   //获取已加载的界面
                if (uiForm != null)
                    return uiForm.SerialId; //TODO:这里返回已打开界面id？还是返回null？
            }

            return uiComponent.OpenUIForm(assetName, drUIForm.UIGroupName, RuntimeConstant.AssetPriority.UIFormAsset, drUIForm.PauseCoveredUIForm, userData);
        }

        public static int? OpenRuntimeUIForm(this UIComponent uiComponent, int uiFormId, string hotFormTypeName, object userData = null)
        {
            //先获取UI配置表数据
            IDataTable<DRUIForm> dtUIForm = GameEntry.DataTable.GetDataTable<DRUIForm>();
            DRUIForm drUIForm = dtUIForm.GetDataRow(uiFormId);
            if (drUIForm == null)
                return null;

            //获取资源路径
            string assetName = RuntimeAssetUtility.GetUIFormAsset(drUIForm.AssetName);
            if (!drUIForm.AllowMultiInstance)   //不允许存在多个界面实例
            {
                if (uiComponent.IsLoadingUIForm(assetName)) //正在加载
                    return null;

                UIForm uiForm = uiComponent.GetUIForm(assetName);   //获取已加载的界面
                if (uiForm != null)
                    return uiForm.SerialId; //TODO:这里返回已打开界面id？还是返回null？
            }

            return uiComponent.OpenUIForm(assetName, drUIForm.UIGroupName, RuntimeConstant.AssetPriority.UIFormAsset, drUIForm.PauseCoveredUIForm, userData);
        }

        public static int? OpenHotUIForm(this UIComponent uiComponent, int uiFormId, string hotFormTypeName, object userData = null)
        {
            //先获取UI配置表数据
            IDataTable<DRUIForm> dtUIForm = GameEntry.DataTable.GetDataTable<DRUIForm>();
            DRUIForm drUIForm = dtUIForm.GetDataRow(uiFormId);
            if (drUIForm == null)
                return null;

            //获取资源路径
            string assetName = RuntimeAssetUtility.GetUIFormAsset(drUIForm.AssetName);
            if (!drUIForm.AllowMultiInstance)   //不允许存在多个界面实例
            {
                if (uiComponent.IsLoadingUIForm(assetName)) //正在加载
                    return null;

                UIForm uiForm = uiComponent.GetUIForm(assetName);   //获取已加载的界面
                if (uiForm != null)
                    return uiForm.SerialId; //TODO:这里返回已打开界面id？还是返回null？
            }

            //UI传递的数据
            //UserUIData uiData = ReferencePool.Acquire<UserUIData>();
            //uiData.Fill(hotFormTypeName, userData);
            UserUIData uiData = new UserUIData(hotFormTypeName, userData);

            return uiComponent.OpenUIForm(assetName, drUIForm.UIGroupName, RuntimeConstant.AssetPriority.UIFormAsset, drUIForm.PauseCoveredUIForm, uiData);
        }

        //打开对话框界面
        public static void OpenDialog(this UIComponent uiComponent, DialogParams dialogParams)
        {
            if ((GameEntry.Procedure.CurrentProcedure as ProcedureBase).UseNativeDialog)
                OpenNativeDialog(dialogParams);
            else
                uiComponent.OpenHotUIForm(1, "DialogForm", dialogParams);
        }

        //打开本地对话框
        private static void OpenNativeDialog(DialogParams dialogParams)
        {
            //throw new System.NotImplementedException("OpenNativeDialog");
            // TODO：这里应该弹出原生对话框，先简化实现为直接按确认按钮
            if (dialogParams.OnClickConfirm != null)
            {
                dialogParams.OnClickConfirm(dialogParams.UserData);
            }
        }
    }
}