using GameFramework;
using GameFramework.Event;
using GameFramework.Network;
using ProtoBuf;
using ProtoBuf.Meta;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityGameFrame.Runtime;

namespace Game.Runtime {	
	/// <summary>
	/// 网络频道辅助器
	/// </summary>
	public class NetworkChannelHelper : INetworkChannelHelper
	{
	    private const int c_MissHeartBeatCloseCount = 2;    //引起关闭连接的丢包次数
	    private const int c_PacketHeaderLength = sizeof(int);   //消息包头长度
	
	    private readonly Dictionary<int, Type> m_ServerToClientPacketTypes = new Dictionary<int, Type>();   //服务器发来的消息包类型
	    private readonly MemoryStream m_CachedStream = new MemoryStream(1024 * 8);  //内存流
	    private INetworkChannel m_NetworkChannel = null;    //网络频带
	
	    /// <summary>
	    /// 获取消息包头长度
	    /// </summary>
	    public int PacketHeaderLength { get { return c_PacketHeaderLength; } }
	
	    /// <summary>
	    /// 初始化网络频道辅助器
	    /// </summary>
	    /// <param name="networkChannel">网络频道。</param>
	    public void Initialize(INetworkChannel networkChannel)
	    {
	        m_NetworkChannel = networkChannel;
	
	        //反射注册包和包处理函数
	        Type packetBaseType = typeof(SCPacketBase);
	        Type packetHandlerBaseType = typeof(PacketHandlerBase);
	        Assembly assembly = Assembly.GetExecutingAssembly();
	        Type[] types = assembly.GetTypes();
	        for (int i = 0; i < types.Length; i++)
	        {
	            if (!types[i].IsClass || types[i].IsAbstract)
	                continue;
	
	            if(types[i].BaseType == packetBaseType) //服务器发来消息包的所有子类
	            {
	                SCPacketBase packetBase = Activator.CreateInstance(types[i]) as SCPacketBase;
	                Type packetType = GetServerToClientPacketType(packetBase.Id);
	                if (packetType != null)
	                {
	                    Log.Warning("Already exist packet type '{0}', check '{1}' or '{2}'?.", packetBase.Id.ToString(), packetType.Name, packetBase.GetType().Name);
	                    continue;
	                }
	
	                m_ServerToClientPacketTypes.Add(packetBase.Id, types[i]);   //保存
	            }
	            else if (types[i].BaseType == packetHandlerBaseType)    //消息包处理方法的所有子类
	            {
	                IPacketHandler packetHandler = (IPacketHandler)Activator.CreateInstance(types[i]);
	                m_NetworkChannel.RegisterHandler(packetHandler);
	            }
	        }
	
	        //注册网络事件回调
	        GameEntry.Event.Subscribe(UnityGameFrame.Runtime.NetworkConnectedEventArgs.EventId, OnNetworkConnected);
	        GameEntry.Event.Subscribe(UnityGameFrame.Runtime.NetworkClosedEventArgs.EventId, OnNetworkClosed);
	        GameEntry.Event.Subscribe(UnityGameFrame.Runtime.NetworkMissHeartBeatEventArgs.EventId, OnNetworkMissHeartBeat);
	        GameEntry.Event.Subscribe(UnityGameFrame.Runtime.NetworkErrorEventArgs.EventId, OnNetworkError);
	        GameEntry.Event.Subscribe(UnityGameFrame.Runtime.NetworkCustomErrorEventArgs.EventId, OnNetworkCustomError);
	    }
	
	    /// <summary>
	    /// 关闭并清理网络频道辅助器
	    /// </summary>
	    public void Shutdown()
	    {
	        GameEntry.Event.Unsubscribe(UnityGameFrame.Runtime.NetworkConnectedEventArgs.EventId, OnNetworkConnected);
	        GameEntry.Event.Unsubscribe(UnityGameFrame.Runtime.NetworkClosedEventArgs.EventId, OnNetworkClosed);
	        GameEntry.Event.Unsubscribe(UnityGameFrame.Runtime.NetworkMissHeartBeatEventArgs.EventId, OnNetworkMissHeartBeat);
	        GameEntry.Event.Unsubscribe(UnityGameFrame.Runtime.NetworkErrorEventArgs.EventId, OnNetworkError);
	        GameEntry.Event.Unsubscribe(UnityGameFrame.Runtime.NetworkCustomErrorEventArgs.EventId, OnNetworkCustomError);
	
	        m_NetworkChannel = null;
	    }
	
	    /// <summary>
	    /// 发送心跳消息包
	    /// </summary>
	    /// <returns>是否发送心跳消息包成功</returns>
	    public bool SendHeartBeat()
	    {
	        m_NetworkChannel.Send(ReferencePool.Acquire<CSHeartBeat>());
	        return true;
	    }
	
	    /// <summary>
	    /// 序列化消息包
	    /// </summary>
	    /// <typeparam name="T">消息包类型</typeparam>
	    /// <param name="packet">要序列化的消息包</param>
	    /// <param name="destination">要序列化的目标流</param>
	    /// <returns>是否序列化成功</returns>
	    public bool Serialize<T>(T packet, Stream destination) where T : Packet
	    {
	        PacketBase packetImpl = packet as PacketBase;
	        if(packetImpl == null)
	        {
	            Log.Warning("Packet is invalid.");
	            return false;
	        }
	
	        //只能序列化客户端发送给服务器的消息包
	        if(packetImpl.PacketType != PacketType.ClientToServer)
	        {
	            Log.Warning("Send packet invalid.");
	            return false;
	        }
	
	        m_CachedStream.SetLength(m_CachedStream.Capacity);  // 此行防止 Array.Copy 的数据无法写入
	        m_CachedStream.Position = 0L;
	
	        //序列化包头
	        CSPacketHeader packetHeader = ReferencePool.Acquire<CSPacketHeader>();
	        Serializer.Serialize(m_CachedStream, packetHeader);
	        ReferencePool.Release(packetHeader);    //记得回收
	        //序列化消息包
	        Serializer.SerializeWithLengthPrefix(m_CachedStream, packet, PrefixStyle.Fixed32);
	        ReferencePool.Release((IReference)packet);    //记得回收
	
	        m_CachedStream.WriteTo(destination);
	        return true;
	    }
	
	    /// <summary>
	    /// 反序列消息包头
	    /// </summary>
	    /// <param name="source">要反序列化的来源流</param>
	    /// <param name="customErrorData">用户自定义错误数据</param>
	    /// <returns></returns>
	    public IPacketHeader DeserializePacketHeader(Stream source, out object customErrorData)
	    {
	        //注意：此函数并不在主线程调用！
	        customErrorData = null;
	        return RuntimeTypeModel.Default.Deserialize(source, ReferencePool.Acquire<SCPacketHeader>(), typeof(SCPacketHeader)) as IPacketHeader;
	    }
	
	    /// <summary>
	    /// 反序列化消息包
	    /// </summary>
	    /// <param name="packetHeader">消息包头</param>
	    /// <param name="source">要反序列化的来源流</param>
	    /// <param name="customErrorData">用户自定义错误数据</param>
	    /// <returns>反序列化后的消息包</returns>
	    public Packet DeserializePacket(IPacketHeader packetHeader, Stream source, out object customErrorData)
	    {
	        // 注意：此函数并不在主线程调用！
	        customErrorData = null;
	        SCPacketHeader scPacketHeader = packetHeader as SCPacketHeader; //接收的消息包头
	        if (scPacketHeader == null)
	        {
	            Log.Warning("Packet header is invalid.");
	            return null;
	        }
	
	        Packet packet = null;
	        if (scPacketHeader.IsValid)
	        {
	            Type packetType = GetServerToClientPacketType(scPacketHeader.Id);
	            if (packetType != null)
	                packet = RuntimeTypeModel.Default.DeserializeWithLengthPrefix(source, ReferencePool.Acquire(packetType), packetType, PrefixStyle.Fixed32, 0) as Packet;
	            else
	                Log.Warning("Can not deserialize packet for packet id '{0}'.", scPacketHeader.Id.ToString());
	        }
	        else
	        {
	            Log.Warning("Packet header is invalid.");
	        }
	
	        ReferencePool.Release(scPacketHeader);  //回收消息头
	        return packet;
	    }
	
	    //获取服务器到客户端的消息包类型
	    private Type GetServerToClientPacketType(int id)
	    {
	        Type type = null;
	        if (m_ServerToClientPacketTypes.TryGetValue(id, out type))
	            return type;
	
	        return null;
	    }
	
	    //网络连接成功的回调
	    private void OnNetworkConnected(object sender, GameEventArgs e)
	    {
	        UnityGameFrame.Runtime.NetworkConnectedEventArgs args = e as UnityGameFrame.Runtime.NetworkConnectedEventArgs;
	        if (args.NetworkChannel != m_NetworkChannel)
	            return;
	
	        Log.Info("Network channel '{0}' connected, local address '{1}', remote address '{2}'.", 
	            args.NetworkChannel.Name, 
	            args.NetworkChannel.Socket.LocalEndPoint.ToString(), 
	            args.NetworkChannel.Socket.RemoteEndPoint.ToString());
	    }
	
	    //网络关闭的回调
	    private void OnNetworkClosed(object sender, GameEventArgs e)
	    {
	        UnityGameFrame.Runtime.NetworkClosedEventArgs args = e as UnityGameFrame.Runtime.NetworkClosedEventArgs;
	        if (args.NetworkChannel != m_NetworkChannel)
	            return;
	
	        Log.Info("Network channel '{0}' closed.", args.NetworkChannel.Name);
	    }
	
	    //网络丢失心跳包的回调
	    private void OnNetworkMissHeartBeat(object sender, GameEventArgs e)
	    {
	        UnityGameFrame.Runtime.NetworkMissHeartBeatEventArgs args = e as UnityGameFrame.Runtime.NetworkMissHeartBeatEventArgs;
	        if (args.NetworkChannel != m_NetworkChannel)
	            return;
	
	        Log.Info("Network channel '{0}' miss heart beat '{1}' times.", args.NetworkChannel.Name, args.MissCount.ToString());
	
	        if (args.MissCount < 2) //丢包数量小于2，则不影响
	            return;
	
	        args.NetworkChannel.Close();
	    }
	
	    //网络错误的回调
	    private void OnNetworkError(object sender, GameEventArgs e)
	    {
	        UnityGameFrame.Runtime.NetworkErrorEventArgs args = e as UnityGameFrame.Runtime.NetworkErrorEventArgs;
	        if (args.NetworkChannel != m_NetworkChannel)
	            return;
	
	        Log.Info("Network channel '{0}' error, error code is '{1}', error message is '{2}'.", args.NetworkChannel.Name, args.ErrorCode.ToString(), args.ErrorMessage);
	
	        args.NetworkChannel.Close();
	    }
	
	    //自定义的网络错误回调
	    private void OnNetworkCustomError(object sender, GameEventArgs e)
	    {
	        UnityGameFrame.Runtime.NetworkCustomErrorEventArgs args = e as UnityGameFrame.Runtime.NetworkCustomErrorEventArgs;
	        if (args.NetworkChannel != m_NetworkChannel)
	            return;
	
	        //TODO:这里扩展自定义的网络错误
	
	    }
	
	}
}
