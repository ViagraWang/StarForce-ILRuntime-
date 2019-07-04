using GameFramework;
using GameFramework.Entity;
using UnityEngine;

namespace UnityGameFrame.Runtime
{
    /// <summary>
    /// 实体
    /// </summary>
    public sealed class Entity : MonoBehaviour, IEntity
    {
        /// <summary>
        /// 获取实体编号
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// 获取实体资源名称
        /// </summary>
        public string EntityAssetName { get; private set; }

        /// <summary>
        /// 获取实体实例
        /// </summary>
        public object Handle { get { return gameObject; } }

        /// <summary>
        /// 获取实体所属的实体组
        /// </summary>
        public IEntityGroup EntityGroup { get; private set; }

        /// <summary>
        /// 获取实体逻辑
        /// </summary>
        public EntityLogic Logic { get; private set; }

        /// <summary>
        /// 实体初始化
        /// </summary>
        /// <param name="entityId">实体编号id</param>
        /// <param name="entityAssetName">实体资源名称</param>
        /// <param name="entityGroup">实体所属的实体组</param>
        /// <param name="isNewInstance">是否是新实例</param>
        /// <param name="userData">用户自定义数据</param>
        public void OnInit(int entityId, string entityAssetName, IEntityGroup entityGroup, bool isNewInstance, object userData)
        {
            Id = entityId;
            EntityAssetName = entityAssetName;
            if (isNewInstance)
                EntityGroup = entityGroup;
            else if(EntityGroup != entityGroup)
            {
                Log.Error("[Entity.OnInit] Entity group is inconsistent for non-new-instance entity.");
                return;
            }

            ShowEntityInfo showEntityInfo = userData as ShowEntityInfo;//EntityManager的ShowEntity函数传递进去的，最终又通过生命周期传递回来
            if(showEntityInfo == null || showEntityInfo.EntityLogicType == null)
            {
                Log.Error("[Entity.OnInit] Entity logic type is invalid.");
                return;
            }

            //修改控制逻辑
            if(Logic != null)
            {

                if (Logic.GetType() == showEntityInfo.EntityLogicType)
                {
                    Logic.enabled = true;   //直接启用
                    return;
                }

                Destroy(Logic);
                Logic = null;
            }

            if(!typeof(EntityLogic).IsAssignableFrom(showEntityInfo.EntityLogicType))
            {
                Log.Error("[Entity.OnInit] Type '{0}' is not assignable from EntityLogic.", showEntityInfo.EntityLogicType);
                return;
            }

            Logic = gameObject.AddComponent(showEntityInfo.EntityLogicType) as EntityLogic;
            if(Logic == null)
            {
                Log.Error("Entity '{0}' can not add entity logic.", entityAssetName);
                return;
            }
            Logic.OnInit(this, showEntityInfo.UserData);  //初始化
        }

        /// <summary>
        /// 实体附加子实体
        /// </summary>
        /// <param name="childEntity">子实体</param>
        /// <param name="userData">用户自定义数据</param>
        public void OnAttached(IEntity childEntity, object userData)
        {
            AttachEntityInfo attachEntityInfo = userData as AttachEntityInfo;
            Logic.OnAttached(((Entity)childEntity).Logic, attachEntityInfo.ParentTransform, attachEntityInfo.UserData);
        }

        /// <summary>
        /// 实体附加子实体
        /// </summary>
        /// <param name="parentEntity">被附加的父实体</param>
        /// <param name="userData">用户自定义数据</param>
        public void OnAttachTo(IEntity parentEntity, object userData)
        {
            AttachEntityInfo attachEntityInfo = userData as AttachEntityInfo;
            Logic.OnAttachTo(((Entity)parentEntity).Logic, attachEntityInfo.ParentTransform, attachEntityInfo.UserData);
        }

        /// <summary>
        /// 实体接触子实体
        /// </summary>
        /// <param name="childEntity">解除的子实体</param>
        /// <param name="userData">用户自定义数据</param>
        public void OnDetached(IEntity childEntity, object userData)
        {
            Logic.OnDetached(((Entity)childEntity).Logic, userData);
        }

        /// <summary>
        /// 实体解除子实体
        /// </summary>
        /// <param name="parentEntity">被解除的父实体</param>
        /// <param name="userData">用户自定义数据</param>
        public void OnDetachFrom(IEntity parentEntity, object userData)
        {
            Logic.OnDetachFrom(((Entity)parentEntity).Logic, userData);
        }

        /// <summary>
        /// 实体隐藏
        /// </summary>
        public void OnHide(object userData)
        {
            Logic.OnHide(userData);
        }

        /// <summary>
        /// 实体回收
        /// </summary>
        public void OnRecycle()
        {
            Id = 0;
            Logic.enabled = false;
        }

        /// <summary>
        /// 实体显示
        /// </summary>
        /// <param name="userData">用户自定义数据</param>
        public void OnShow(object userData)
        {
            ShowEntityInfo showEntityInfo = userData as ShowEntityInfo;
            Logic.OnShow(showEntityInfo.UserData);
        }

        /// <summary>
        /// 实体轮询
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位</param>
        public void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            Logic.OnUpdate(elapseSeconds, realElapseSeconds);
        }
    }
}
