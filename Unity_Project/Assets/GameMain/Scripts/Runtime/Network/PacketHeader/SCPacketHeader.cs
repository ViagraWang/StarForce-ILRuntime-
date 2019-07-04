
namespace Game.Runtime {	
	/// <summary>
	/// 服务器发送到客户端的消息头
	/// </summary>
	public sealed class SCPacketHeader : PacketHeaderBase
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
