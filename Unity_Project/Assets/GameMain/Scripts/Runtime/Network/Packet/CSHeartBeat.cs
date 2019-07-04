using ProtoBuf;
using System;

namespace Game.Runtime {	
	[Serializable, ProtoContract(Name =@"CSHeartBeat")]
	public class CSHeartBeat : CSPacketBase
	{
	    public CSHeartBeat()
	    {
	
	    }
	
	    public override int Id { get { return 1; } }
	
	    public override void Clear()
	    {
	
	    }
	
	}
}
