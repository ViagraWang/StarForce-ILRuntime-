using System;
using System.Collections;
using System.Collections.Generic;

namespace Game.Hotfix
{	
	/// <summary>
	/// 我的飞机数据
	/// </summary>
	public class MyAircraftData : AircraftData
	{
        public string Name { get; set; } = null;

        public MyAircraftData(int entityId, int typeId) : base(entityId, typeId, CampType.Player) { }
	}
}
