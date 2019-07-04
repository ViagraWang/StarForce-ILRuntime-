using GameFramework.Event;

namespace UnityGameFrame.Runtime
{
    /// <summary>
    /// 卸载场景失败事件
    /// </summary>
    public sealed class UnloadSceneFailureEventArgs : GameEventArgs
    {
        /// <summary>
        /// 卸载场景失败事件编号
        /// </summary>
        public static readonly int EventId = typeof(UnloadSceneFailureEventArgs).GetHashCode();

        /// <summary>
        /// 获取卸载场景失败事件编号
        /// </summary>
        public override int Id { get { return EventId; } }

        /// <summary>
        /// 获取场景资源名称
        /// </summary>
        public string SceneAssetName { get; private set; }

        /// <summary>
        /// 获取用户自定义数据
        /// </summary>
        public object UserData { get; private set; }

        public override void Clear()
        {
            SceneAssetName = default(string);
            UserData = default(object);
        }

        /// <summary>
        /// 填充卸载场景失败事件
        /// </summary>
        /// <param name="e">内部事件</param>
        /// <returns>卸载场景失败事件</returns>
        public UnloadSceneFailureEventArgs Fill(GameFramework.Scene.UnloadSceneFailureEventArgs e)
        {
            SceneAssetName = e.SceneAssetName;
            UserData = e.UserData;

            return this;
        }
    }
}
