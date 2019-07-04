using GameFramework.DataTable;
using System;
using UnityGameFrame.Runtime;

namespace Game.Runtime
{
    public static class EntityExtension
    {
        //显示实体
        private static void ShowRuntimeEntity(this EntityComponent entityComponent, Type logicType, string entityGroup, int priority, int entityId, int dataId, object userData)
        {
            if (entityId == 0 || dataId == 0)
            {
                Log.Warning("[EntityExtension.ShowRuntimeEntity] entityId or dataId is invalid.");
                return;
            }

            IDataTable<DREntity> dtEntity = GameEntry.DataTable.GetDataTable<DREntity>();
            DREntity drEntity = dtEntity.GetDataRow(dataId);
            if (drEntity == null)
            {
                Log.Warning("[EntityExtension.ShowRuntimeEntity] Can not load entity id '{0}' from data table.", dataId.ToString());
                return;
            }

            entityComponent.ShowEntity(entityId, logicType, RuntimeAssetUtility.GetEntityAsset(drEntity.AssetName), entityGroup, priority, userData);
        }

        //显示实体
        public static void ShowHotEntity(this EntityComponent entityComponent, string hotLogicTypeName, string entityGroup, int priority, int entityId, int dataId, object userData)
        {
            if (entityId == 0 || dataId == 0)
            {
                Log.Warning("[EntityExtension.ShowRuntimeEntity] entityId or dataId is invalid.");
                return;
            }

            IDataTable<DREntity> dtEntity = GameEntry.DataTable.GetDataTable<DREntity>();
            DREntity drEntity = dtEntity.GetDataRow(dataId);
            if (drEntity == null)
            {
                Log.Warning("Can not load entity id '{0}' from data table.", dataId.ToString());
                return;
            }

            //实体传递的数据
            //UserEntityData uiData = ReferencePool.Acquire<UserEntityData>();
            //uiData.Fill(hotLogicTypeName, userData);
            UserEntityData entityData = new UserEntityData(hotLogicTypeName, userData);
            entityComponent.ShowEntity(entityId, typeof(HotEntity), RuntimeAssetUtility.GetEntityAsset(drEntity.AssetName), entityGroup, priority, entityData);
        }
    }

}
