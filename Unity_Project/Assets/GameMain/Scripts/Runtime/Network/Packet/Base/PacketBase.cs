using GameFramework.Network;
using ProtoBuf;

namespace Game.Runtime {	
	//网络消息包基类
	public abstract class PacketBase : Packet, IExtensible
	{
	    private IExtension m_ExtensionObject;
	
	    public PacketBase()
	    {
	        m_ExtensionObject = null;
	    }
	
	    /// <summary>
	    /// 消息包的类型
	    /// </summary>
	    public abstract PacketType PacketType { get; }
	
	    public IExtension GetExtensionObject(bool createIfMissing)
	    {
	        return Extensible.GetExtensionObject(ref m_ExtensionObject, createIfMissing);
	    }
	
	}
}
