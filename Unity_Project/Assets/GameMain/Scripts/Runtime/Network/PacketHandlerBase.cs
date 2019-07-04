
using GameFramework.Network;

namespace Game.Runtime {	
	/// <summary>
	/// 消息包处理基类
	/// </summary>
	public abstract class PacketHandlerBase : IPacketHandler
	{
	    /// <summary>
	    /// 获取网络消息包协议编号
	    /// </summary>
	    public abstract int Id { get; }
	
	    /// <summary>
	    /// 网络消息包处理函数
	    /// </summary>
	    /// <param name="sender">网络消息包源</param>
	    /// <param name="packet">网络消息包内容</param>
	    public abstract void Handle(object sender, Packet packet);
	}
}
