using GameFramework;
using GameFramework.DataTable;
using System.IO;

namespace UnityGameFrame.Runtime
{
    /// <summary>
    /// 数据表行基类
    /// </summary>
    public abstract class DataRowBase : IDataRow
    {
        /// <summary>
        /// 获取数据表行的编号
        /// </summary>
        public abstract int Id { get; }

        public virtual bool ParseDataRow(GameFrameworkSegment<string> dataRowText)
        {
            Log.Warning("[DataRowBase.ParseDataRow] Not implemented ParseDataRow(GameFrameworkSegment<string>)");
            return false;
        }

        /// <summary>
        /// 数据表行二进制流解析器
        /// </summary>
        /// <param name="dataRowSegment">要解析的数据表行片段</param>
        /// <returns>是否解析数据表行成功</returns>
        public virtual bool ParseDataRow(GameFrameworkSegment<byte[]> dataRowSegment)
        {
            Log.Warning("[DataRowBase.ParseDataRow] Not implemented ParseDataRow(GameFrameworkSegment<byte[]>)");
            return false;
        }

        /// <summary>
        /// 数据表行二进制流解析器
        /// </summary>
        /// <param name="dataRowSegment">要解析的数据表行片段</param>
        /// <returns>是否解析数据表行成功</returns>
        public virtual bool ParseDataRow(GameFrameworkSegment<Stream> dataRowSegment)
        {
            Log.Warning("[DataRowBase.ParseDataRow] Not implemented ParseDataRow(GameFrameworkSegment<Stream>)");
            return false;
        }
    }
}
