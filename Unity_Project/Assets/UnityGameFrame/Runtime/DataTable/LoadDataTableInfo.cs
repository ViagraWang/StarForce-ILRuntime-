using System;

namespace UnityGameFrame.Runtime
{
    //加载数据表的信息
    internal sealed class LoadDataTableInfo
    {
        private readonly Type m_DataRowType;    //数据类型
        private readonly string m_DataTableName;    //数据表名
        private readonly string m_DataTableNameInType;
        private readonly object m_UserData;

        public Type DataRowType { get { return m_DataRowType; } }

        public string DataTableName { get { return m_DataTableName; } }

        public string DataTableNameInType { get { return m_DataTableNameInType; } }

        public object UserData { get { return m_UserData; } }


        public LoadDataTableInfo(Type dataRowType, string dataTableName, string dataTableNameInType, object userData)
        {
            m_DataRowType = dataRowType;
            m_DataTableName = dataTableName;
            m_DataTableNameInType = dataTableNameInType;
            m_UserData = userData;
        }

    }
}
