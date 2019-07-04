using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Runtime {	
	/// <summary>
	/// 服务器发送给客户端的消息包基类
	/// </summary>
	public abstract class SCPacketBase : PacketBase
	{
	    public override PacketType PacketType
	    {
	        get
	        {
	            return PacketType.ServerToClient;
	        }
	    }
	}
}
