using GameFramework.Event;

namespace UnityGameFrame.Runtime
{
    /// <summary>
    /// 打开界面失败事件
    /// </summary>
   public class OpenUIFormFailureEventArgs : GameEventArgs
    {
        /// <summary>
        /// 打开界面失败事件编号
        /// </summary>
        public static readonly int EventId = typeof(OpenUIFormFailureEventArgs).GetHashCode();

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
        /// 获取界面组名称
        /// </summary>
        public string UIGroupName { get; private set; }

        /// <summary>
        /// 获取是否暂停被覆盖的界面
        /// </summary>
        public bool IsPauseCoveredUIForm { get; private set; }

        /// <summary>
        /// 获取错误信息
        /// </summary>
        public string ErrorMessage { get; private set; }

        /// <summary>
        /// 获取用户自定义数据
        /// </summary>
        public object UserData { get; private set; }

        public override void Clear()
        {
            SerialId = default(int);
            UIFormAssetName = default(string);
            UIGroupName = default(string);
            IsPauseCoveredUIForm = default(bool);
            ErrorMessage = default(string);
            UserData = default(object);
        }

        /// <summary>
        /// 填充打开界面失败事件
        /// </summary>
        /// <param name="e">内部事件</param>
        /// <returns>打开界面失败事件</returns>
        public OpenUIFormFailureEventArgs Fill(GameFramework.UI.OpenUIFormFailureEventArgs e)
        {
            SerialId = e.SerialId;
            UIFormAssetName = e.UIFormAssetName;
            UIGroupName = e.UIGroupName;
            IsPauseCoveredUIForm = e.PauseCoveredUIForm;
            ErrorMessage = e.ErrorMessage;
            UserData = e.UserData;

            return this;
        }
    }
}
