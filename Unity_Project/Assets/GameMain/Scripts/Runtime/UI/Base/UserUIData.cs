//=======================================================
// 作者：
// 描述：打开UI时传递的数据，用户自定义数据封装
//=======================================================
using GameFramework;

namespace Game.Runtime
{
    public sealed class UserUIData
    {
        /// <summary>
        /// UI界面热更逻辑名称
        /// </summary>
        public string HotLogicTypeName { get; private set; }

        /// <summary>
        /// UI界面热更逻辑全名称
        /// </summary>
        public string HotLogicTypeFullName { get; private set; }

        /// <summary>
        /// 用户自定义数据
        /// </summary>
        public object UserData { get; set; }

        /// <summary>
        /// 绑定的HotUIForm
        /// </summary>
        public HotUIForm RuntimeUIForm { get; set; }

        public UserUIData() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="hotLogicTypeName">热更的界面逻辑名称</param>
        /// <param name="userData">用户自定义数据</param>
        public UserUIData(string hotLogicTypeName, object userData)
        {
            HotLogicTypeName = hotLogicTypeName;
            HotLogicTypeFullName = hotLogicTypeName.HotFixTypeFullName();
            UserData = userData;
        }

        /// <summary>
        /// 填充数据
        /// </summary>
        /// <param name="hotLogicTypeName">热更的界面逻辑名称</param>
        /// <param name="userData">用户自定义数据</param>
        public UserUIData Fill(string hotLogicTypeName, object userData)
        {
            HotLogicTypeName = hotLogicTypeName;
            HotLogicTypeFullName = hotLogicTypeName.HotFixTypeFullName();
            UserData = userData;
            return this;
        }

        //清理
        //public void Clear()
        //{
        //    HotLogicTypeName = default;
        //    HotLogicTypeFullName = default;
        //    UserData = default;
        //    HotUIForm = default;
        //}
    }
}