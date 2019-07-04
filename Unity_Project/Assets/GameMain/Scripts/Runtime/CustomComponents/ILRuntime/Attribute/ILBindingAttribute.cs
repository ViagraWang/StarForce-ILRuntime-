using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Runtime {	
	//绑定热更类的特性
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public class ILBindingAttribute : Attribute
	{
	}
}
