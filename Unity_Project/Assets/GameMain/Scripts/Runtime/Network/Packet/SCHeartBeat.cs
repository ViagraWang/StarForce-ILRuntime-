using ProtoBuf;
using System;

namespace Game.Runtime {	
	
	[Serializable, ProtoContract(Name = @"SCHeartBeat")]
	public class SCHeartBeat : SCPacketBase
	{
	    public override int Id { get { return 2; } }
	
	    public override void Clear()
	    {
	
	    }
	
	}
}
