using GameFramework;
using GameFramework.Network;
using UnityEditor;
using UnityEngine;
using UnityGameFrame.Runtime;

namespace UnityGameFrame.Editor
{
    [CustomEditor(typeof(NetworkComponent))]
    internal sealed class NetworkComponentInspector : GameFrameworkInspector
    {

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!EditorApplication.isPlaying)
            {
                EditorGUILayout.HelpBox("Available during runtime only.", MessageType.Info);
                return;
            }

            NetworkComponent t = target as NetworkComponent;
            if(IsPrefabInHierarchy(t.gameObject))
            {
                EditorGUILayout.LabelField("Network Channel Count", t.NetworkChannelCount.ToString());

                INetworkChannel[] networkChannels = t.GetAllNetworkChannels();
                for (int i = 0; i < networkChannels.Length; i++)
                {
                    DrawNetworkChannel(networkChannels[i]);
                }
            }

            Repaint();
        }

        private void DrawNetworkChannel(INetworkChannel networkChannel)
        {
            EditorGUILayout.BeginVertical("box");
            {
                //网络频道相关信息
                EditorGUILayout.LabelField(networkChannel.Name, networkChannel.Connected ? "Connected" : "Disconnected");
                EditorGUILayout.LabelField("Network Type", networkChannel.NetworkType.ToString());  //网络类型：IPV4、IPV6
                EditorGUILayout.LabelField("Local Address", networkChannel.Connected ?  networkChannel.Socket.LocalEndPoint.ToString() : "Unavailable");
                EditorGUILayout.LabelField("Remote Address", networkChannel.Connected ?  networkChannel.Socket.RemoteEndPoint.ToString() : "Unavailable");
                EditorGUILayout.LabelField("Send Packet", Utility.Text.Format("{0} / {1}", networkChannel.SendPacketCount, networkChannel.SentPacketCount));
                EditorGUILayout.LabelField("Receive Packet", Utility.Text.Format("{0} / {1}", networkChannel.ReceivePacketCount, networkChannel.ReceivedPacketCount));
                EditorGUILayout.LabelField("Miss Heart Beat Count", networkChannel.MissHeartBeatCount.ToString());
                EditorGUILayout.LabelField("Heart Beat", Utility.Text.Format("{0} / {1}", networkChannel.HeartBeatElapseSeconds.ToString("F2"), networkChannel.HeartBeatInterval.ToString("F2")));
                EditorGUI.BeginDisabledGroup(!networkChannel.Connected);
                {
                    //关闭按钮
                    if (GUILayout.Button("Disconnect"))
                    {
                        networkChannel.Close();
                    }
                }
                EditorGUI.EndDisabledGroup();
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.Separator();
        }

        protected override void OnCompileComplete()
        {

        }

        protected override void OnCompileStart()
        {

        }
    }
}
