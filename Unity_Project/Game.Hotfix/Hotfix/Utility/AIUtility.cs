//=======================================================
// 作者：
// 描述：AI工具类
//=======================================================
using Game.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameEntry = Game.Runtime.GameEntry;

namespace Game.Hotfix
{
    /// <summary>
    /// AI 工具类
    /// </summary>
    public static class AIUtility
    {
        //阵营关系类型
        private static Dictionary<int, RelationType> s_CampPairToRelation = new Dictionary<int, RelationType>();
        //与阵营存在某一关系的所有阵营类型缓存
        private static Dictionary<KeyValuePair<CampType, RelationType>, CampType[]> s_CampAndRelationToCamps = new Dictionary<KeyValuePair<CampType, RelationType>, CampType[]>();

        static AIUtility()
        {
            //与玩家相关的阵营关系
            s_CampPairToRelation.Add(((int)CampType.Player << 4) + (int)CampType.Player, RelationType.Friendly);
            s_CampPairToRelation.Add(((int)CampType.Player << 4) + (int)CampType.Enemy, RelationType.Hostile);
            s_CampPairToRelation.Add(((int)CampType.Player << 4) + (int)CampType.Neutral, RelationType.Neutral);
            s_CampPairToRelation.Add(((int)CampType.Player << 4) + (int)CampType.Player2, RelationType.Hostile);
            s_CampPairToRelation.Add(((int)CampType.Player << 4) + (int)CampType.Enemy2, RelationType.Hostile);
            s_CampPairToRelation.Add(((int)CampType.Player << 4) + (int)CampType.Neutral2, RelationType.Neutral);

            //与敌人相关的阵营关系
            s_CampPairToRelation.Add(((int)CampType.Enemy << 4) + (int)CampType.Enemy, RelationType.Friendly);
            s_CampPairToRelation.Add(((int)CampType.Enemy << 4) + (int)CampType.Neutral, RelationType.Neutral);
            s_CampPairToRelation.Add(((int)CampType.Enemy << 4) + (int)CampType.Player2, RelationType.Hostile);
            s_CampPairToRelation.Add(((int)CampType.Enemy << 4) + (int)CampType.Enemy2, RelationType.Hostile);
            s_CampPairToRelation.Add(((int)CampType.Enemy << 4) + (int)CampType.Neutral2, RelationType.Neutral);

            //与第一中立营相关的阵营关
            s_CampPairToRelation.Add(((int)CampType.Neutral << 4) + (int)CampType.Neutral, RelationType.Neutral);
            s_CampPairToRelation.Add(((int)CampType.Neutral << 4) + (int)CampType.Player2, RelationType.Neutral);
            s_CampPairToRelation.Add(((int)CampType.Neutral << 4) + (int)CampType.Enemy2, RelationType.Neutral);
            s_CampPairToRelation.Add(((int)CampType.Neutral << 4) + (int)CampType.Neutral2, RelationType.Hostile);

            //与玩家2相关的阵营关系
            s_CampPairToRelation.Add(((int)CampType.Player2 << 4) + (int)CampType.Player2, RelationType.Friendly);
            s_CampPairToRelation.Add(((int)CampType.Player2 << 4) + (int)CampType.Enemy2, RelationType.Hostile);
            s_CampPairToRelation.Add(((int)CampType.Player2 << 4) + (int)CampType.Neutral2, RelationType.Neutral);

            //与敌人2相关的阵营关系
            s_CampPairToRelation.Add(((int)CampType.Enemy2 << 4) + (int)CampType.Enemy2, RelationType.Friendly);
            s_CampPairToRelation.Add(((int)CampType.Enemy2 << 4) + (int)CampType.Neutral2, RelationType.Neutral);

            //与第二中立营相关的阵营关
            s_CampPairToRelation.Add(((int)CampType.Neutral2 << 4) + (int)CampType.Neutral2, RelationType.Neutral);
        }

        /// <summary>
        /// 获取两个阵营之间的关系。
        /// </summary>
        /// <param name="first">阵营一。</param>
        /// <param name="second">阵营二。</param>
        /// <returns>阵营间关系。</returns>
        public static RelationType GetRelation(CampType first, CampType second)
        {
            //第一阵营一定小于或等于第二阵营
            if (first > second)
            {
                CampType temp = first;
                first = second;
                second = temp;
            }

            RelationType relationType;
            if (s_CampPairToRelation.TryGetValue(((int)first << 4) + (int)second, out relationType))
            {
                return relationType;
            }

            HotLog.Warning("Unknown relation between '{0}' and '{1}'.", first.ToString(), second.ToString());
            return RelationType.Unknown;
        }

        //获取与阵营存在某一关系的所有阵营
        public static CampType[] GetCamps(CampType camp, RelationType relation)
        {
            KeyValuePair<CampType, RelationType> key = new KeyValuePair<CampType, RelationType>(camp, relation);
            CampType[] results = null;
            if (s_CampAndRelationToCamps.TryGetValue(key, out results))
                return results;

            List<CampType> listCamp = new List<CampType>();
            Array arrCamps = Enum.GetValues(typeof(CampType));
            for (int i = 0; i < arrCamps.Length; i++)
            {
                CampType secondCamp = (CampType)arrCamps.GetValue(i);
                if (GetRelation(camp, secondCamp) == relation)
                    listCamp.Add(camp);
            }

            results = listCamp.ToArray();
            if (s_CampAndRelationToCamps.ContainsKey(key))
                s_CampAndRelationToCamps[key] = results;
            else
                s_CampAndRelationToCamps.Add(key, results);

            return results;
        }

        //获取实体之间的距离
        public static float GetDistance(HotEntity fromEntity, HotEntity toEntity)
        {
            return (toEntity.Position - fromEntity.CachedTransform.position).magnitude;
        }

        //执行碰撞
        public static void PerformCollision(TargetableObject entity, EntityLogicBase other)
        {
            if (entity == null || other == null)
                return;

            //如果是可命中的实体对象
            TargetableObject target = other as TargetableObject;
            if (target != null)
            {
                //获取撞击数据
                ImpactData entityImpactData = entity.GetImpactData();
                ImpactData targetImpactData = target.GetImpactData();
                //友好关系则无碰撞
                if (GetRelation(entityImpactData.Camp, targetImpactData.Camp) == RelationType.Friendly)
                    return;

                //计算双方的伤害血量
                int entityDamageHP = CalculateDamageHP(targetImpactData.Attack, entityImpactData.Defense);
                int targetDamageHP = CalculateDamageHP(entityImpactData.Attack, targetImpactData.Defense);

                //这里增加伤害值，为了让其中一方收到伤害直接死亡
                int delta = Mathf.Min(entityImpactData.HP - entityDamageHP, targetImpactData.HP - targetDamageHP);
                if (delta > 0)
                {
                    entityDamageHP += delta;
                    targetDamageHP += delta;
                }
                //应用伤害
                entity.ApplyDamage(target, entityDamageHP);
                target.ApplyDamage(entity, targetDamageHP);

                //记得回收
                ReferencePool.Release(entityImpactData);
                ReferencePool.Release(targetImpactData);
                return;
            }

            //子弹碰撞
            Bullet bullet = other as Bullet;
            if (bullet != null)
            {
                ImpactData bulletImpactData = bullet.GetImpactData();
                ImpactData entityImpactData = entity.GetImpactData();

                if (GetRelation(entityImpactData.Camp, bulletImpactData.Camp) == RelationType.Friendly)
                    return;

                int entityDamageHP = CalculateDamageHP(bulletImpactData.Attack, entityImpactData.Defense);  //伤害值
                entity.ApplyDamage(bullet, entityDamageHP); //执行伤害
                GameEntry.Entity.HideEntity(bullet);    //隐藏子弹

                //记得回收
                ReferencePool.Release(bulletImpactData);
                ReferencePool.Release(entityImpactData);
                return;
            }

        }

        //计算伤害血量
        private static int CalculateDamageHP(int attack, int defense)
        {
            if (attack <= 0)
                return 0;

            if (defense < 0)
                defense = 0;

            return attack * attack / (attack + defense);    //装甲抵挡伤害的公式
        }

    }
}