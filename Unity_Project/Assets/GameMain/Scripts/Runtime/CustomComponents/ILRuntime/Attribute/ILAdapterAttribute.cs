using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Runtime {	
	//适配器的特性
	[AttributeUsage(AttributeTargets.Class)]
	public class ILAdapterAttribute : Attribute
	{
	}
}
