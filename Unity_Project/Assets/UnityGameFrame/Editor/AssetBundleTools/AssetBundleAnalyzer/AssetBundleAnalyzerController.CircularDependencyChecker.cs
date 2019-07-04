using System.Collections.Generic;
using System.Linq;

namespace UnityGameFrame.Editor.AssetBundleTools
{
    public sealed partial class AssetBundleAnalyzerController
    {
        /// <summary>
        /// 间接依赖检查
        /// </summary>
        private sealed class CircularDependencyChecker
        {
            private readonly Stamp[] m_Stamps;

            public CircularDependencyChecker(Stamp[] stamps)
            {
                m_Stamps = stamps;
            }

            //检查
            public string[][] Check()
            {
                HashSet<string> hosts = new HashSet<string>();
                foreach (Stamp stamp in m_Stamps)
                {
                    hosts.Add(stamp.HostAssetName);
                }

                List<string[]> results = new List<string[]>();
                foreach (string host in hosts)
                {
                    Stack<string> route = new Stack<string>();
                    HashSet<string> visited = new HashSet<string>();
                    if (Check(host, route, visited))
                    {
                        results.Add(route.ToArray());
                    }
                }

                return results.ToArray();
            }

            //检查资源
            private bool Check(string host, Stack<string> route, HashSet<string> visited)
            {
                visited.Add(host);
                route.Push(host);

                foreach (Stamp stamp in m_Stamps)
                {
                    if (host != stamp.HostAssetName)
                        continue;

                    if (visited.Contains(stamp.DependencyAssetName))
                    {
                        route.Push(stamp.DependencyAssetName);
                        return true;
                    }

                    if (Check(stamp.DependencyAssetName, route, visited))
                    {
                        return true;
                    }
                }

                route.Pop();
                visited.Remove(host);
                return false;
            }
        }
    }
}
