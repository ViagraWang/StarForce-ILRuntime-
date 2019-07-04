using System;
using System.Collections.Generic;
using System.Reflection;

namespace ILRuntime.Runtime.Generated
{
    class CLRBindings
    {


        /// <summary>
        /// Initialize the CLR binding, please invoke this AFTER CLR Redirection registration
        /// </summary>
        public static void Initialize(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            GameFramework_Utility_Binding_Text_Binding.Register(app);
            System_Exception_Binding.Register(app);
            System_String_Binding.Register(app);
            System_DateTime_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_ILTypeInstance_Binding_Enumerator_Binding.Register(app);
            System_Collections_Generic_KeyValuePair_2_String_ILTypeInstance_Binding.Register(app);
            System_IDisposable_Binding.Register(app);
            System_Type_Binding.Register(app);
            System_Collections_Generic_List_1_ILTypeInstance_Binding.Register(app);
            System_Object_Binding.Register(app);
            System_Activator_Binding.Register(app);
            System_Int32_Binding.Register(app);
            UnityEngine_Object_Binding.Register(app);
            Game_Runtime_HotLog_Binding.Register(app);
            UnityEngine_GameObject_Binding.Register(app);
            Game_Runtime_ReferenceCollector_Binding.Register(app);
            UnityEngine_Transform_Binding.Register(app);
            Game_Runtime_ComponentView_Binding.Register(app);
            UnityEngine_Component_Binding.Register(app);
            UnityEngine_Canvas_Binding.Register(app);
            UnityEngine_UI_CanvasScaler_Binding.Register(app);
            UnityEngine_Vector2_Binding.Register(app);
            Game_Runtime_GameEntry_Binding.Register(app);
            Game_Runtime_ComponentExtension_Binding.Register(app);
            UnityEngine_MonoBehaviour_Binding.Register(app);
            UnityEngine_CanvasGroup_Binding.Register(app);
            Game_Runtime_Entity_Binding.Register(app);
            UnityEngine_UI_Slider_Binding.Register(app);
            UnityGameFrame_Runtime_EntityLogic_Binding.Register(app);
            UnityGameFrame_Runtime_SceneComponent_Binding.Register(app);
            UnityEngine_Camera_Binding.Register(app);
            UnityEngine_RectTransformUtility_Binding.Register(app);
            UnityEngine_RectTransform_Binding.Register(app);
            UnityEngine_WaitForSeconds_Binding.Register(app);
            Game_Runtime_RuntimeUIExtension_Binding.Register(app);
            System_NotSupportedException_Binding.Register(app);
            UnityGameFrame_Runtime_DataTableComponent_Binding.Register(app);
            GameFramework_DataTable_IDataTable_1_DRAircraft_Binding.Register(app);
            Game_Runtime_DRAircraft_Binding.Register(app);
            GameFramework_DataTable_IDataTable_1_DRArmor_Binding.Register(app);
            Game_Runtime_DRArmor_Binding.Register(app);
            GameFramework_DataTable_IDataTable_1_DRAsteroid_Binding.Register(app);
            Game_Runtime_DRAsteroid_Binding.Register(app);
            UnityEngine_Vector3_Binding.Register(app);
            UnityEngine_Quaternion_Binding.Register(app);
            GameFramework_DataTable_IDataTable_1_DRThruster_Binding.Register(app);
            Game_Runtime_DRThruster_Binding.Register(app);
            GameFramework_DataTable_IDataTable_1_DRWeapon_Binding.Register(app);
            Game_Runtime_DRWeapon_Binding.Register(app);
            UnityGameFrame_Runtime_EntityComponent_Binding.Register(app);
            UnityGameFrame_Runtime_Entity_Binding.Register(app);
            Game_Runtime_HotEntity_Binding.Register(app);
            System_Reflection_MemberInfo_Binding.Register(app);
            Game_Runtime_EntityExtension_Binding.Register(app);
            Game_Runtime_SoundExtension_Binding.Register(app);
            UnityEngine_Random_Binding.Register(app);
            UnityGameFrame_Runtime_UnityExtension_Binding.Register(app);
            Game_Hotfix_ScrollableBackground_Binding.Register(app);
            UnityEngine_Collider_Binding.Register(app);
            UnityEngine_Bounds_Binding.Register(app);
            UnityEngine_Rect_Binding.Register(app);
            UnityEngine_Input_Binding.Register(app);
            UnityEngine_Mathf_Binding.Register(app);
            UnityEngine_Time_Binding.Register(app);
            UnityGameFrame_Runtime_ShowEntitySuccessEventArgs_Binding.Register(app);
            UnityGameFrame_Runtime_EventComponent_Binding.Register(app);
            UnityGameFrame_Runtime_ShowEntityFailureEventArgs_Binding.Register(app);
            Game_Runtime_UserEntityData_Binding.Register(app);
            GameFramework_Utility_Binding_Random_Binding.Register(app);
            Game_Runtime_HotfixComponent_Binding.Register(app);
            UnityGameFrame_Runtime_LoadSceneSuccessEventArgs_Binding.Register(app);
            UnityGameFrame_Runtime_LoadSceneFailureEventArgs_Binding.Register(app);
            UnityGameFrame_Runtime_LoadSceneUpdateEventArgs_Binding.Register(app);
            UnityGameFrame_Runtime_LoadSceneDependencyAssetEventArgs_Binding.Register(app);
            UnityGameFrame_Runtime_SoundComponent_Binding.Register(app);
            UnityGameFrame_Runtime_BaseComponent_Binding.Register(app);
            GameFramework_Fsm_IFsm_1_IProcedureManager_Binding.Register(app);
            GameFramework_Variable_1_Int32_Binding.Register(app);
            GameFramework_DataTable_IDataTable_1_DRScene_Binding.Register(app);
            Game_Runtime_DRScene_Binding.Register(app);
            Game_Runtime_RuntimeAssetUtility_Binding.Register(app);
            UnityGameFrame_Runtime_ObjectPoolComponent_Binding.Register(app);
            Game_Runtime_HotProcedure_Binding.Register(app);
            System_Single_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Int32_ILTypeInstance_Binding.Register(app);
            UnityGameFrame_Runtime_ConfigComponent_Binding.Register(app);
            UnityGameFramework_Runtime_VarInt_Binding.Register(app);
            UnityGameFrame_Runtime_OpenUIFormSuccessEventArgs_Binding.Register(app);
            Game_Runtime_UGUIForm_Binding.Register(app);
            Game_Runtime_UserUIData_Binding.Register(app);
            Game_Runtime_HotUIForm_Binding.Register(app);
            System_Collections_Generic_ICollection_1_KeyValuePair_2_String_ILTypeInstance_Binding.Register(app);
            System_Threading_Monitor_Binding.Register(app);
            System_Collections_Generic_IEnumerable_1_KeyValuePair_2_String_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_IEnumerator_1_KeyValuePair_2_String_ILTypeInstance_Binding.Register(app);
            System_Collections_IEnumerator_Binding.Register(app);
            System_Collections_Generic_IDictionary_2_String_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_IEnumerable_1_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_IEnumerator_1_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_Queue_1_ILTypeInstance_Binding.Register(app);
            UnityEngine_Screen_Binding.Register(app);
            UnityGameFrame_Runtime_UIFormLogic_Binding.Register(app);
            UnityEngine_UI_Text_Binding.Register(app);
            Game_Runtime_DialogParams_Binding.Register(app);
            UnityGameFrame_Runtime_LocalizationComponent_Binding.Register(app);
            UnityEngine_Application_Binding.Register(app);
            UnityGameFrame_Runtime_GameEntry_Binding.Register(app);
            UnityEngine_UI_Toggle_Binding.Register(app);
            UnityGameFrame_Runtime_SettingComponent_Binding.Register(app);
            GameFramework_DataTable_IDataTable_1_DRUIForm_Binding.Register(app);
            Game_Runtime_DRUIForm_Binding.Register(app);
            UnityGameFrame_Runtime_UIComponent_Binding.Register(app);
            GameFramework_UI_IUIGroup_Binding.Register(app);
            UnityGameFrame_Runtime_UIForm_Binding.Register(app);
            UnityGameFrame_Runtime_ProcedureComponent_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_Int32_Int32_Binding.Register(app);
            System_Collections_Generic_KeyValuePair_2_Byte_Int32_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_KeyValuePair_2_Byte_Int32_Byte_Array_Binding.Register(app);
            System_Collections_Generic_List_1_Byte_Binding.Register(app);
            System_Enum_Binding.Register(app);
            System_Array_Binding.Register(app);

            ILRuntime.CLR.TypeSystem.CLRType __clrType = null;
        }

        /// <summary>
        /// Release the CLR binding, please invoke this BEFORE ILRuntime Appdomain destroy
        /// </summary>
        public static void Shutdown(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
        }
    }
}
