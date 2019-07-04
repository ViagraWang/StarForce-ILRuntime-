using GameFramework;
using UnityEngine;

namespace UnityGameFrame.Runtime
{
    /// <summary>
    /// 默认游戏框架日志辅助器
    /// </summary>
    public sealed class DefaultLogHelper : GameFrameworkLog.ILogHelper
    {
        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="level">日志等级</param>
        /// <param name="message">日志内容</param>
        public void Log(GameFrameworkLogLevel level, object message)
        {
            switch (level)
            {
                case GameFrameworkLogLevel.Debug:  //灰色信息
                    Debug.Log(Utility.Text.Format("<color=#888888>{0}</color>", message.ToString()));
                    break;
                case GameFrameworkLogLevel.Info:   //信息
                    Debug.Log(message);
                    break;
                case GameFrameworkLogLevel.Warning:    //警告
                    Debug.LogWarning(message);
                    break;
                case GameFrameworkLogLevel.Error:  //错误
                    Debug.LogError(message);
                    break;
                case GameFrameworkLogLevel.Fatal:  //严重错误
                    throw new GameFrameworkException(message.ToString());
                default:
                    break;
            }
        }
    }
}
