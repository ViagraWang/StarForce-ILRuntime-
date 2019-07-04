
namespace UnityGameFrame.Editor.AssetBundleTools
{
    public sealed partial class AssetBundleAnalyzerController
    {
        //标记
        private struct Stamp
        {
            private readonly string m_HostAssetName;    //主资源名
            private readonly string m_DependencyAssetName;  //依赖资源名

            public string HostAssetName { get { return m_HostAssetName; } }

            public string DependencyAssetName { get { return m_DependencyAssetName; } }

            public Stamp(string hostAssetName, string dependencyAssetName)
            {
                m_HostAssetName = hostAssetName;
                m_DependencyAssetName = dependencyAssetName;
            }
        }
    }
}
