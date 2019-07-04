
namespace UnityGameFrame.Runtime
{
    //加载字典信息数据
    internal sealed class LoadDictionaryInfo
    {
        private readonly string m_DictionaryName;   //字典名称
        private readonly object m_UserData; //用户自定义数据

        public string DictionaryName { get { return m_DictionaryName; } }

        public object UserData { get { return m_UserData; } }

        public LoadDictionaryInfo(string dictionaryName, object userData)
        {
            m_DictionaryName = dictionaryName;
            m_UserData = userData;
        }

    }
}
