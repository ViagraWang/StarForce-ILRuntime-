using GameFramework;
using System;
using System.Collections.Generic;


namespace UnityGameFrame.Runtime
{
    //显示实体信息时传递的数据
    internal sealed class ShowEntityInfo : IReference
    {
        public Type EntityLogicType { get; private set; }

        public object UserData { get; private set; }

        public ShowEntityInfo() { }

        public ShowEntityInfo Fill(Type entityLogicType, object userData)
        {
            EntityLogicType = entityLogicType;
            UserData = userData;
            return this;
        }

        public void Clear()
        {
            EntityLogicType = default;
            UserData = default;
        }

    }
}
