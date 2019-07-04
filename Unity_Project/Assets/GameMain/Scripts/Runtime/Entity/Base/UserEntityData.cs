//=======================================================
// 作者：
// 描述：创建实体的传递的数据，用户自定义数据封装
//=======================================================

namespace Game.Runtime
{
    public sealed class UserEntityData
    {
        /// <summary>
        /// 实体逻辑热更名称
        /// </summary>
        public string HotLogicTypeName { get; private set; }

        /// <summary>
        /// 实体逻辑热更全名称
        /// </summary>
        public string HotLogicTypeFullName { get; private set; }

        /// <summary>
        /// 用户自定义数据
        /// </summary>
        public object UserData { get; private set; }

        /// <summary>
        /// 绑定的HotEntity
        /// </summary>
        public HotEntity RuntimeEntity { get; set; }

        public UserEntityData() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="hotLogicTypeName">热更的实体逻辑类型名称</param>
        /// <param name="userData">用户自定义数据</param>
        public UserEntityData(string hotLogicTypeName, object userData)
        {
            HotLogicTypeName = hotLogicTypeName;
            HotLogicTypeFullName = hotLogicTypeName.HotFixTypeFullName();
            UserData = userData;
        }

        /// <summary>
        /// 填充数据
        /// </summary>
        /// <param name="hotLogicTypeName">热更的实体逻辑类型名称</param>
        /// <param name="userData">用户自定义数据</param>
        public UserEntityData Fill(string hotLogicTypeName, object userData)
        {
            HotLogicTypeName = hotLogicTypeName;
            HotLogicTypeFullName = hotLogicTypeName.HotFixTypeFullName();
            UserData = userData;
            return this;
        }

        ////清理
        //public void Clear()
        //{
        //    HotLogicTypeName = default;
        //    HotLogicTypeFullName = default;
        //    UserData = default;
        //    RuntimeEntity = default;
        //}
    }
}
