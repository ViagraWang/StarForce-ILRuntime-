using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Runtime {	
	/// <summary>
	/// 客户端发送给服务器的消息包基类
	/// </summary>
	public abstract class CSPacketBase : PacketBase
	{
	    public override PacketType PacketType
	    {
	        get
	        {
	            return PacketType.ClientToServer;
	        }
	    }
	}
}
