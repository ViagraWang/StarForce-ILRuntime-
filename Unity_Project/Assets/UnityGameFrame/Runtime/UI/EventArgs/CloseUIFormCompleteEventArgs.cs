using GameFramework.Event;
using GameFramework.UI;

namespace UnityGameFrame.Runtime
{
    /// <summary>
    /// 关闭界面完成事件
    /// </summary>
    public class CloseUIFormCompleteEventArgs : GameEventArgs
    {
        /// <summary>
        /// 关闭界面完成事件编号
        /// </summary>
        public static readonly int EventId = typeof(CloseUIFormCompleteEventArgs).GetHashCode();

        public override int Id { get { return EventId; } }

        /// <summary>
        /// 获取界面序列编号
        /// </summary>
        public int SerialId { get; private set; }

        /// <summary>
        /// 获取界面资源名称
        /// </summary>
        public string UIFormAssetName { get; private set; }

        /// <summary>
        /// 获取界面组
        /// </summary>
        public IUIGroup UIGroup { get; private set; }

        /// <summary>
        /// 获取用户自定义数据
        /// </summary>
        public object UserData { get; private set; }

        public override void Clear()
        {
            SerialId = default(int);
            UIFormAssetName = default(string);
            UIGroup = default(IUIGroup);
            UserData = default(object);
        }

        /// <summary>
        /// 填充关闭界面完成事件
        /// </summary>
        /// <param name="e">内部事件</param>
        /// <returns>关闭界面完成事件</returns>
        public CloseUIFormCompleteEventArgs Fill(GameFramework.UI.CloseUIFormCompleteEventArgs e)
        {
            SerialId = e.SerialId;
            UIFormAssetName = e.UIFormAssetName;
            UIGroup = e.UIGroup;
            UserData = e.UserData;

            return this;
        }
    }
}
