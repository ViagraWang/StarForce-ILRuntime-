//=======================================================
// 作者：
// 描述：
//=======================================================
using UnityGameFrame.Runtime;

namespace Game.Runtime
{
    /// <summary>
    /// 主要供Hotfix中调用
    /// </summary>
	public static class HotLog
    {
        /// <summary>
        /// 记录调试级别日志，用于记录调试类日志信息
        /// </summary>
        /// <param name="message">日志内容</param>
        /// <remarks>仅在带有 DEBUG 预编译选项且带有 ENABLE_LOG、ENABLE_DEBUG_LOG 或 ENABLE_DEBUG_AND_ABOVE_LOG 预编译选项时生效</remarks>
        public static void Debug(object message)
        {
            Log.Debug(message);
        }

        /// <summary>
        /// 记录调试级别日志，用于记录调试类日志信息
        /// </summary>
        /// <param name="message">日志内容</param>
        /// <remarks>仅在带有 DEBUG 预编译选项且带有 ENABLE_LOG、ENABLE_DEBUG_LOG 或 ENABLE_DEBUG_AND_ABOVE_LOG 预编译选项时生效</remarks>
        public static void Debug(string message)
        {
            Log.Debug(message);
        }

        /// <summary>
        /// 记录调试级别日志，用于记录调试类日志信息
        /// </summary>
        /// <param name="format">日志格式</param>
        /// <param name="args">日志参数</param>
        /// <remarks>仅在带有 DEBUG 预编译选项且带有 ENABLE_LOG、ENABLE_DEBUG_LOG 或 ENABLE_DEBUG_AND_ABOVE_LOG 预编译选项时生效</remarks>
        public static void Debug(string format, params object[] args)
        {
            Log.Debug(format, args);
        }

        /// <summary>
        /// 打印信息级别日志，用于记录程序正常运行日志信息
        /// </summary>
        /// <param name="message">日志内容</param>
        /// <remarks>仅在带有 ENABLE_LOG、ENABLE_INFO_LOG、ENABLE_DEBUG_AND_ABOVE_LOG 或 ENABLE_INFO_AND_ABOVE_LOG 预编译选项时生效</remarks>
        public static void Info(object message)
        {
            Log.Info(message);
        }

        /// <summary>
        /// 打印信息级别日志，用于记录程序正常运行日志信息
        /// </summary>
        /// <param name="message">日志内容</param>
        /// <remarks>仅在带有 ENABLE_LOG、ENABLE_INFO_LOG、ENABLE_DEBUG_AND_ABOVE_LOG 或 ENABLE_INFO_AND_ABOVE_LOG 预编译选项时生效</remarks>
        public static void Info(string message)
        {
            Log.Info(message);
        }

        /// <summary>
        /// 打印信息级别日志，用于记录程序正常运行日志信息
        /// </summary>
        /// <param name="format">日志格式</param>
        /// <param name="args">日志参数</param>
        /// <remarks>仅在带有 ENABLE_LOG、ENABLE_INFO_LOG、ENABLE_DEBUG_AND_ABOVE_LOG 或 ENABLE_INFO_AND_ABOVE_LOG 预编译选项时生效</remarks>
        public static void Info(string format, params object[] args)
        {
            Log.Info(format, args);
        }

        /// <summary>
        /// 打印警告级别日志，建议在发生局部功能逻辑错误，但尚不会导致游戏崩溃或异常时使用
        /// </summary>
        /// <param name="message">日志内容</param>
        /// <remarks>仅在带有 ENABLE_LOG、ENABLE_INFO_LOG、ENABLE_DEBUG_AND_ABOVE_LOG、ENABLE_INFO_AND_ABOVE_LOG 或 ENABLE_WARNING_AND_ABOVE_LOG 预编译选项时生效</remarks>
        public static void Warning(object message)
        {
            Log.Warning(message);
        }

        /// <summary>
        /// 打印警告级别日志，建议在发生局部功能逻辑错误，但尚不会导致游戏崩溃或异常时使用
        /// </summary>
        /// <param name="message">日志内容</param>
        /// <remarks>仅在带有 ENABLE_LOG、ENABLE_INFO_LOG、ENABLE_DEBUG_AND_ABOVE_LOG、ENABLE_INFO_AND_ABOVE_LOG 或 ENABLE_WARNING_AND_ABOVE_LOG 预编译选项时生效</remarks>
        public static void Warning(string message)
        {
            Log.Warning(message);
        }

        /// <summary>
        /// 打印警告级别日志，建议在发生局部功能逻辑错误，但尚不会导致游戏崩溃或异常时使用
        /// </summary>
        /// <param name="format">日志格式</param>
        /// <param name="args">日志参数</param>
        /// <remarks>仅在带有 ENABLE_LOG、ENABLE_INFO_LOG、ENABLE_DEBUG_AND_ABOVE_LOG、ENABLE_INFO_AND_ABOVE_LOG 或 ENABLE_WARNING_AND_ABOVE_LOG 预编译选项时生效</remarks>
        public static void Warning(string format, params object[] args)
        {
            Log.Warning(format, args);
        }

        /// <summary>
        /// 打印错误级别日志，建议在发生功能逻辑错误，但尚不会导致游戏崩溃或异常时使用
        /// </summary>
        /// <param name="message">日志内容</param>
        /// <remarks>仅在带有 ENABLE_LOG、ENABLE_INFO_LOG、ENABLE_DEBUG_AND_ABOVE_LOG、ENABLE_INFO_AND_ABOVE_LOG、ENABLE_WARNING_AND_ABOVE_LOG 或 ENABLE_ERROR_AND_ABOVE_LOG 预编译选项时生效</remarks>
        public static void Error(object message)
        {
            Log.Error(message);
        }

        /// <summary>
        /// 打印错误级别日志，建议在发生功能逻辑错误，但尚不会导致游戏崩溃或异常时使用
        /// </summary>
        /// <param name="message">日志内容</param>
        /// <remarks>仅在带有 ENABLE_LOG、ENABLE_INFO_LOG、ENABLE_DEBUG_AND_ABOVE_LOG、ENABLE_INFO_AND_ABOVE_LOG、ENABLE_WARNING_AND_ABOVE_LOG 或 ENABLE_ERROR_AND_ABOVE_LOG 预编译选项时生效</remarks>
        public static void Error(string message)
        {
            Log.Error(message);
        }

        /// <summary>
        /// 打印错误级别日志，建议在发生功能逻辑错误，但尚不会导致游戏崩溃或异常时使用
        /// </summary>
        /// <param name="format">日志格式</param>
        /// <param name="args">日志参数</param>
        /// <remarks>仅在带有 ENABLE_LOG、ENABLE_INFO_LOG、ENABLE_DEBUG_AND_ABOVE_LOG、ENABLE_INFO_AND_ABOVE_LOG、ENABLE_WARNING_AND_ABOVE_LOG 或 ENABLE_ERROR_AND_ABOVE_LOG 预编译选项时生效</remarks>
        public static void Error(string format, params object[] args)
        {
            Log.Error(format, args);
        }

        /// <summary>
        /// 打印严重错误级别日志，建议在发生严重错误，可能导致游戏崩溃或异常时使用，此时应尝试重启进程或重建游戏框架
        /// </summary>
        /// <param name="message">日志内容</param>
        /// <remarks>仅在带有 ENABLE_LOG、ENABLE_INFO_LOG、ENABLE_DEBUG_AND_ABOVE_LOG、ENABLE_INFO_AND_ABOVE_LOG、ENABLE_WARNING_AND_ABOVE_LOG、ENABLE_ERROR_AND_ABOVE_LOG 或 ENABLE_FATAL_AND_ABOVE_LOG 预编译选项时生效</remarks>
        public static void Fatal(object message)
        {
            Log.Fatal(message);
        }

        /// <summary>
        /// 打印严重错误级别日志，建议在发生严重错误，可能导致游戏崩溃或异常时使用，此时应尝试重启进程或重建游戏框架
        /// </summary>
        /// <param name="message">日志内容</param>
        /// <remarks>仅在带有 ENABLE_LOG、ENABLE_INFO_LOG、ENABLE_DEBUG_AND_ABOVE_LOG、ENABLE_INFO_AND_ABOVE_LOG、ENABLE_WARNING_AND_ABOVE_LOG、ENABLE_ERROR_AND_ABOVE_LOG 或 ENABLE_FATAL_AND_ABOVE_LOG 预编译选项时生效</remarks>
        public static void Fatal(string message)
        {
            Log.Fatal(message);
        }

        /// <summary>
        /// 打印严重错误级别日志，建议在发生严重错误，可能导致游戏崩溃或异常时使用，此时应尝试重启进程或重建游戏框架
        /// </summary>
        /// <param name="format">日志格式</param>
        /// <param name="args">日志参数</param>
        /// <remarks>仅在带有 ENABLE_LOG、ENABLE_INFO_LOG、ENABLE_DEBUG_AND_ABOVE_LOG、ENABLE_INFO_AND_ABOVE_LOG、ENABLE_WARNING_AND_ABOVE_LOG、ENABLE_ERROR_AND_ABOVE_LOG 或 ENABLE_FATAL_AND_ABOVE_LOG 预编译选项时生效</remarks>
        public static void Fatal(string format, params object[] args)
        {
            Log.Fatal(format, args);
        }

    }
}