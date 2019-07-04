using GameFramework;
using GameFramework.Network;

namespace Game.Runtime
{
	/// <summary>
	/// 消息头基类
	/// </summary>
	public abstract class PacketHeaderBase : IPacketHeader, IReference
	{
	    public abstract PacketType PacketType { get; }
	
	    /// <summary>
	    /// 消息包的类型编号
	    /// </summary>
	    public int Id { get; set; }
	
	    /// <summary>
	    /// 消息长度
	    /// </summary>
	    public int PacketLength { get; set; }
	
	    /// <summary>
	    /// 是否合法
	    /// </summary>
	    public bool IsValid { get { return PacketType != PacketType.Undefined && Id > 0 && PacketLength >= 0; } }
	
	    public void Clear()
	    {
	        Id = 0;
	        PacketLength = 0;
	    }
	
	}
}
