using GameFramework;
using GameFramework.ObjectPool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityGameFrame.Runtime;

namespace UnityGameFrame.Editor
{
    [CustomEditor(typeof(ObjectPoolComponent))]
    internal sealed class ObjectPoolComponentInspector : GameFrameworkInspector
    {
        private HashSet<string> m_OpenedItems = new HashSet<string>();  //打开的选项

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!EditorApplication.isPlaying)
            {
                EditorGUILayout.HelpBox("Available during runtime only.", MessageType.Info);
                return;
            }

            ObjectPoolComponent t = target as ObjectPoolComponent;

            if(IsPrefabInHierarchy(t.gameObject))
            {
                EditorGUILayout.LabelField("Object Pool Count", t.Count.ToString());    //对象池数量

                ObjectPoolBase[] objectPools = t.GetAllObjectPools(true);
                for (int i = 0; i < objectPools.Length; i++)
                {
                    DrawObjectPool(objectPools[i]);
                }
            }

            Repaint();
        }

        //绘制对象池信息
        private void DrawObjectPool(ObjectPoolBase objectPool)
        {
            bool lastState = m_OpenedItems.Contains(objectPool.FullName);   //上次状态
            bool currentState = EditorGUILayout.Foldout(lastState, objectPool.FullName);    //文件夹状态
            if(currentState != lastState)
            {
                if (currentState)
                    m_OpenedItems.Add(objectPool.FullName);
                else
                    m_OpenedItems.Remove(objectPool.FullName);
            }

            //打开状态下，显示对对象池相关信息
            if (currentState)
            {
                EditorGUILayout.BeginVertical("box");
                {
                    EditorGUILayout.LabelField("Name", objectPool.Name);    //对象池名称
                    EditorGUILayout.LabelField("Type", objectPool.ObjectType.FullName); //对象池类型
                    EditorGUILayout.LabelField("Auto Release Interval", objectPool.AutoReleaseInterval.ToString()); //自动释放频率
                    EditorGUILayout.LabelField("Capacity", objectPool.Capacity.ToString()); //对象池容量
                    EditorGUILayout.LabelField("Used Count", objectPool.Count.ToString());  //使用的对象数量
                    EditorGUILayout.LabelField("Can Release Count", objectPool.CanReleaseCount.ToString()); //可释放的数量
                    EditorGUILayout.LabelField("Expire Time", objectPool.ExpireTime.ToString());    //过期秒数
                    EditorGUILayout.LabelField("Priority", objectPool.Priority.ToString()); //优先级

                    ObjectInfo[] objectInfos = objectPool.GetAllObjectInfos();
                    if (objectInfos.Length > 0)
                    {
                        for (int i = 0; i < objectInfos.Length; i++)
                        {
                            var objectInfo = objectInfos[i];
                            //对象信息
                            EditorGUILayout.LabelField(objectInfo.Name, Utility.Text.Format("{0}, {1}, {2}, {3}", objectInfo.Locked.ToString(), objectPool.AllowMultiSpawn ? objectInfo.SpawnCount.ToString() : objectInfo.IsInUse.ToString(), objectInfo.Priority.ToString(), objectInfo.LastUseTime.ToString("yyyy-MM-dd HH:mm:ss")));
                        }

                        if (GUILayout.Button("Release"))    //释放对象池
                            objectPool.Release();

                        if (GUILayout.Button("Release All Unused")) //释放所有未使用的对象
                            objectPool.ReleaseAllUnused();

                        if (GUILayout.Button("Export SCV Data")) //导出CSV数据
                        {
                            string exportFileName = EditorUtility.SaveFilePanel("Export CSV Data", string.Empty, Utility.Text.Format("Object Pool Data - {0}.csv", objectPool.Name), string.Empty);
                            try
                            {
                                int index = 0;
                                string[] data = new string[objectInfos.Length + 1];
                                data[index++] = Utility.Text.Format("Name,Locked,{0},Custom Can Release Flag,Priority,Last Use Time", objectPool.AllowMultiSpawn ? "Count" : "IsUsing");
                                for (int i = 0; i < objectInfos.Length; i++)
                                {
                                    ObjectInfo objectInfo = objectInfos[i];
                                    data[index++] = Utility.Text.Format("{0},{1},{2},{3},{4},{5}",
                                        objectInfo.Name, objectInfo.Locked.ToString(), objectPool.AllowMultiSpawn ? objectInfo.SpawnCount.ToString() : objectInfo.IsInUse.ToString(), objectInfo.CustomCanReleaseFlag.ToString(), objectInfo.Priority.ToString(), objectInfo.LastUseTime.ToString("yyyy-MM-dd HH:mm:ss"));
                                }
                                File.WriteAllLines(exportFileName, data, Encoding.UTF8);
                                Debug.Log(Utility.Text.Format("Export CSV data to '{0}' success.", exportFileName));
                            }
                            catch (Exception exception)
                            {
                                Debug.LogError(Utility.Text.Format("Export CSV data to '{0}' failure, exception is '{1}'.", exportFileName, exception.Message));
                            }
                        }

                    }
                    else
                    {
                        GUILayout.Label("Object Pool is Empty ...");
                    }
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.Separator();
            }

        }

        protected override void OnCompileComplete()
        {

        }

        protected override void OnCompileStart()
        {

        }
    }
}
