
namespace Game.Runtime {	
	/// <summary>
	/// 客户端发送到服务器的消息头
	/// </summary>
	public sealed class CSPacketHeader : PacketHeaderBase
	{
	    public override PacketType PacketType { get { return PacketType.ClientToServer; } }
	    
	}
}
