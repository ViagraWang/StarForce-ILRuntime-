using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GameFramework;

namespace UnityGameFrame.Editor.AssetBundleTools
{
    /// <summary>
    /// 资源包分析器。
    /// </summary>
    public class AssetBundleAnalyzer : EditorWindow
    {
        private AssetBundleAnalyzerController m_Controller = null;  //分析控制器
        private bool m_Analyzed = false;    //是否分析完成的标志位
        private int m_ToolbarIndex = 0; //工具按钮下标

        private int m_AssetCount = 0;   //工具数量
        private string[] m_CachedAssetNames = null; //缓存资源名称数组
        private int m_SelectedAssetIndex = -1;  //选中资源的下标
        private string m_SelectedAssetName = null;  //选中的资源名称
        private DependencyData m_SelectedDependencyData = null; //选中的依赖资源数据
        private AssetsOrder m_AssetsOrder = AssetsOrder.AssetNameAsc;   //排序方式，名称升序
        private string m_AssetsFilter = null;   //保存输入的资源过滤名称
        private Vector2 m_AssetsScroll = Vector2.zero;  //滚动坐标
        private Vector2 m_DependencyAssetBundlesScroll = Vector2.zero;  //依赖Bundle滚动坐标
        private Vector2 m_DependencyAssetsScroll = Vector2.zero;    //依赖资源滚动坐标
        private Vector2 m_ScatteredDependencyAssetsScroll = Vector2.zero;   //零散依赖资源滚动坐标

        private int m_ScatteredAssetCount = 0;  //零散资源数量
        private string[] m_CachedScatteredAssetNames = null;    //缓存零散资源名称
        private int m_SelectedScatteredAssetIndex = -1; //选中的零散资源下标
        private string m_SelectedScatteredAssetName = null; //选中的零散资源名称
        private AssetInfo[] m_SelectedHostAssets = null;    //选中的主资源数组
        private ScatteredAssetsOrder m_ScatteredAssetsOrder = ScatteredAssetsOrder.AssetNameAsc;    //零散资源排序
        private string m_ScatteredAssetsFilter = null;  //输入的零散资源过滤名称
        private Vector2 m_ScatteredAssetsScroll = Vector2.zero;  //零散资源滚动坐标
        private Vector2 m_HostAssetsScroll = Vector2.zero; //主资源滚动坐标

        private int m_CircularDependencyCount = 0;  //间接依赖数量
        private string[][] m_CachedCircularDependencyDatas = null;  //缓存间接依赖资源数据
        private Vector2 m_CircularDependencyScroll = Vector2.zero;  //间接依赖资源滚动坐标

        //标题
        private string[] m_Titles = new string[] { "Summary", "Asset Dependency Viewer", "Scattered Asset Viewer", "Circular Dependency Viewer" };

        [MenuItem("Game Framework/AssetBundle Tools/AssetBundle Analyzer", false, 43)]
        private static void Open()
        {
            AssetBundleAnalyzer window = GetWindow<AssetBundleAnalyzer>(true, "AssetBundle Analyzer", true);
            window.minSize = new Vector2(1024f, 768f);
        }


        private void OnEnable()
        {
            //创建分析控制器
            m_Controller = new AssetBundleAnalyzerController();
            m_Controller.EventOnLoadingAssetBundle += OnLoadingAssetBundle;
            m_Controller.EventOnLoadingAsset += OnLoadingAsset;
            m_Controller.EventOnLoadCompleted += OnLoadCompleted;
            m_Controller.EventOnAnalyzingAsset += OnAnalyzingAsset;
            m_Controller.EventOnAnalyzeCompleted += OnAnalyzeCompleted;

            m_Analyzed = false;
            m_ToolbarIndex = 0;

            m_AssetCount = 0;
            m_CachedAssetNames = null;
            m_SelectedAssetIndex = -1;
            m_SelectedAssetName = null;
            m_SelectedDependencyData = new DependencyData();
            m_AssetsOrder = AssetsOrder.ScatteredDependencyAssetCountDesc;
            m_AssetsFilter = null;
            m_AssetsScroll = Vector2.zero;
            m_DependencyAssetBundlesScroll = Vector2.zero;
            m_DependencyAssetsScroll = Vector2.zero;
            m_ScatteredDependencyAssetsScroll = Vector2.zero;

            m_ScatteredAssetCount = 0;
            m_CachedScatteredAssetNames = null;
            m_SelectedScatteredAssetIndex = -1;
            m_SelectedScatteredAssetName = null;
            m_SelectedHostAssets = new AssetInfo[] { };
            m_ScatteredAssetsOrder = ScatteredAssetsOrder.HostAssetCountDesc;
            m_ScatteredAssetsFilter = null;
            m_ScatteredAssetsScroll = Vector2.zero;
            m_HostAssetsScroll = Vector2.zero;

            m_CircularDependencyCount = 0;
            m_CachedCircularDependencyDatas = null;
            m_CircularDependencyScroll = Vector2.zero;
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(position.width), GUILayout.Height(position.height));
            {
                GUILayout.Space(5f);
                int toolbarIndex = GUILayout.Toolbar(m_ToolbarIndex, m_Titles, GUILayout.Height(30f));
                if(toolbarIndex != m_ToolbarIndex)
                {
                    m_ToolbarIndex = toolbarIndex;
                    GUI.FocusControl(null);
                }

                switch (m_ToolbarIndex)
                {
                    case 0: //绘制概要
                        DrawSummary();
                        break;
                    case 1: //绘制资源依赖视图
                        DrawAssetDependencyViewer();
                        break;
                    case 2: //绘制零散资源视图
                        DrawScatteredAssetViewer();
                        break;
                    case 3: //绘制间接依赖资源视图
                        DrawCircularDependencyViewer();
                        break;
                    default:
                        break;
                }

            }
            EditorGUILayout.EndVertical();
        }

        //绘制概要
        private void DrawSummary()
        {
            DrawAnalyzeButton();
        }

        //绘制资源依赖视图
        private void DrawAssetDependencyViewer()
        {
            if (!m_Analyzed)
            {
                DrawAnalyzeButton();    //没分析则绘制分析按钮
                return;
            }

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Space(5f);
                //Bundle中的资源
                EditorGUILayout.BeginVertical(GUILayout.Width(position.width * 0.4f));
                {
                    GUILayout.Space(5f);
                    string title = null;
                    if (string.IsNullOrEmpty(m_AssetsFilter))   //不存在过滤文字
                        title = Utility.Text.Format("Assets In AssetBundles ({0})", m_AssetCount);
                    else
                        title = Utility.Text.Format("Assets In AssetBundles ({0}/{1})", m_CachedAssetNames.Length, m_AssetCount);

                    EditorGUILayout.LabelField(title, EditorStyles.boldLabel);  //标题
                    EditorGUILayout.BeginVertical("box", GUILayout.Height(position.height - 150f));
                    {
                        m_AssetsScroll = EditorGUILayout.BeginScrollView(m_AssetsScroll);   //滚动窗口
                        {
                            //选中缓存的资源
                            int selectedIndex = GUILayout.SelectionGrid(m_SelectedAssetIndex, m_CachedAssetNames, 1, "toggle");
                            if (selectedIndex != m_SelectedAssetIndex)
                            {
                                m_SelectedAssetIndex = selectedIndex;
                                m_SelectedAssetName = m_CachedAssetNames[selectedIndex];
                                m_SelectedDependencyData = m_Controller.GetDependencyData(m_SelectedAssetName);
                            }
                        }
                        EditorGUILayout.EndScrollView();
                    }
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.BeginVertical("box");
                    {
                        EditorGUILayout.LabelField("Asset Name", m_SelectedAssetName ?? "<None>");  //选中的资源
                        EditorGUILayout.LabelField("AssetBundle Name", m_SelectedAssetName == null ? "<None>" : m_Controller.GetAssetInfo(m_SelectedAssetName).AssetBundleInfo.FullName);
                        EditorGUILayout.BeginHorizontal();
                        {
                            //选择排序方式
                            AssetsOrder assetsOrder = (AssetsOrder)EditorGUILayout.EnumPopup("Order By", m_AssetsOrder);
                            if(assetsOrder != m_AssetsOrder)
                            {
                                m_AssetsOrder = assetsOrder;
                                OnAssetsOrderOrFilterChanged(); //排序改变
                            }
                        }
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        {
                            string assetsFilter = EditorGUILayout.TextField("Assets Filter", m_AssetsFilter);   //输入过滤名称
                            if(assetsFilter != m_AssetsFilter)
                            {
                                m_AssetsFilter = assetsFilter;
                                OnAssetsOrderOrFilterChanged();
                            }
                            //禁用区域
                            EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(m_AssetsFilter));
                            {
                                if(GUILayout.Button("x", GUILayout.Width(20f)))
                                {
                                    m_AssetsFilter = null;
                                    GUI.FocusControl(null);
                                    OnAssetsOrderOrFilterChanged();
                                }
                            }
                            EditorGUI.EndDisabledGroup();
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndVertical();

                //依赖资源包 Bundles
                EditorGUILayout.BeginVertical(GUILayout.Width(position.width * 0.6f - 14f));
                {
                    GUILayout.Space(5f);
                    EditorGUILayout.LabelField(Utility.Text.Format("Dependency AssetBundles ({0})", m_SelectedDependencyData.DependencyAssetBundleInfoCount), EditorStyles.boldLabel);
                    EditorGUILayout.BeginVertical("box", GUILayout.Height(position.height * 0.2f));
                    {
                        m_DependencyAssetsScroll = EditorGUILayout.BeginScrollView(m_DependencyAssetsScroll);
                        {
                            AssetBundleInfo[] dependencyAssetBundleInfos = m_SelectedDependencyData.AllDependencyAssetBundleInfos;
                            foreach (AssetBundleInfo dependencyAssetBundleInfo in dependencyAssetBundleInfos)
                            {
                                GUILayout.Label(dependencyAssetBundleInfo.FullName);
                            }
                        }
                        EditorGUILayout.EndScrollView();
                    }
                    EditorGUILayout.EndVertical();

                    //依赖资源
                    EditorGUILayout.LabelField(Utility.Text.Format("Dependency Assets ({0})", m_SelectedDependencyData.DependencyAssetInfoCount), EditorStyles.boldLabel);
                    EditorGUILayout.BeginVertical("box", GUILayout.Height(position.height * 0.3f));
                    {
                        m_DependencyAssetBundlesScroll = EditorGUILayout.BeginScrollView(m_DependencyAssetBundlesScroll);
                        {
                            AssetInfo[] dependencyAssetInfos = m_SelectedDependencyData.AllDependencyAssetInfos;
                            foreach (AssetInfo dependencyAssetInfo in dependencyAssetInfos)
                            {
                                EditorGUILayout.BeginHorizontal();
                                {
                                    if (GUILayout.Button("GO", GUILayout.Width(30f)))
                                    {
                                        m_SelectedAssetName = dependencyAssetInfo.Name;
                                        m_SelectedAssetIndex = new List<string>(m_CachedAssetNames).IndexOf(m_SelectedAssetName);
                                        m_SelectedDependencyData = m_Controller.GetDependencyData(m_SelectedAssetName);
                                    }

                                    GUILayout.Label(dependencyAssetInfo.Name);
                                }
                                EditorGUILayout.EndHorizontal();
                            }
                        }
                        EditorGUILayout.EndScrollView();
                    }
                    EditorGUILayout.EndVertical();

                    //零散依赖资源
                    EditorGUILayout.LabelField(Utility.Text.Format("Scattered Dependency Assets ({0})", m_SelectedDependencyData.ScatteredDependencyAssetCount), EditorStyles.boldLabel);
                    EditorGUILayout.BeginVertical("box", GUILayout.Height(position.height * 0.5f - 116f));
                    {
                        m_ScatteredDependencyAssetsScroll = EditorGUILayout.BeginScrollView(m_ScatteredDependencyAssetsScroll);
                        {
                            string[] scatteredDependencyAssetNames = m_SelectedDependencyData.AllScatteredDependencyAssetNames;
                            foreach (string scatteredDependencyAssetName in scatteredDependencyAssetNames)
                            {
                                EditorGUILayout.BeginHorizontal();
                                {
                                    int count = m_Controller.GetHostAssets(scatteredDependencyAssetName).Length;
                                    EditorGUI.BeginDisabledGroup(count < 2);
                                    {
                                        if (GUILayout.Button("GO", GUILayout.Width(30f)))
                                        {
                                            m_SelectedScatteredAssetName = scatteredDependencyAssetName;
                                            m_SelectedScatteredAssetIndex = new List<string>(m_CachedScatteredAssetNames).IndexOf(m_SelectedScatteredAssetName);
                                            m_SelectedHostAssets = m_Controller.GetHostAssets(m_SelectedScatteredAssetName);
                                            m_ToolbarIndex = 2;
                                            GUI.FocusControl(null);
                                        }
                                    }
                                    EditorGUI.EndDisabledGroup();
                                    GUILayout.Label(count > 1 ? Utility.Text.Format("{0} ({1})", scatteredDependencyAssetName, count.ToString()) : scatteredDependencyAssetName);
                                }
                                EditorGUILayout.EndHorizontal();
                            }
                        }
                        EditorGUILayout.EndScrollView();
                    }
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();
        }

        //绘制零散资源视图
        private void DrawScatteredAssetViewer()
        {
            if (!m_Analyzed)
            {
                DrawAnalyzeButton();
                return;
            }

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Space(5f);
                EditorGUILayout.BeginVertical(GUILayout.Width(position.width * 0.4f));
                {
                    GUILayout.Space(5f);
                    string title = null;
                    if(string.IsNullOrEmpty(m_ScatteredAssetsFilter))
                        title = Utility.Text.Format("Scattered Assets ({0})", m_ScatteredAssetCount);
                    else
                        title = Utility.Text.Format("Scattered Assets ({0}/{1})", m_CachedScatteredAssetNames.Length, m_ScatteredAssetCount);

                    EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
                    //选择的零散资源
                    EditorGUILayout.BeginVertical("box", GUILayout.Height(position.height - 132f));
                    {
                        m_ScatteredAssetsScroll = EditorGUILayout.BeginScrollView(m_ScatteredAssetsScroll);
                        {
                            int selectedIndex = GUILayout.SelectionGrid(m_SelectedScatteredAssetIndex, m_CachedScatteredAssetNames, 1, "toggle");
                            if (selectedIndex != m_SelectedScatteredAssetIndex)
                            {
                                m_SelectedScatteredAssetIndex = selectedIndex;
                                m_SelectedScatteredAssetName = m_CachedScatteredAssetNames[selectedIndex];
                                m_SelectedHostAssets = m_Controller.GetHostAssets(m_SelectedScatteredAssetName);
                            }
                        }
                        EditorGUILayout.EndScrollView();
                    }
                    EditorGUILayout.EndVertical();

                    //零散资源名
                    EditorGUILayout.BeginVertical("box");
                    {
                        EditorGUILayout.LabelField("Scattered Asset Name", m_SelectedScatteredAssetName ?? "<None>");
                        EditorGUILayout.BeginHorizontal();
                        {
                            ScatteredAssetsOrder scatteredAssetsOrder = (ScatteredAssetsOrder)EditorGUILayout.EnumPopup("Order by", m_ScatteredAssetsOrder);
                            if (scatteredAssetsOrder != m_ScatteredAssetsOrder)
                            {
                                m_ScatteredAssetsOrder = scatteredAssetsOrder;
                                OnScatteredAssetsOrderOrFilterChanged();
                            }
                        }
                        EditorGUILayout.EndHorizontal();

                        //零散资源过滤名称
                        EditorGUILayout.BeginHorizontal();
                        {
                            string scatteredAssetsFilter = EditorGUILayout.TextField("Assets Filter", m_ScatteredAssetsFilter);
                            if (scatteredAssetsFilter != m_ScatteredAssetsFilter)
                            {
                                m_ScatteredAssetsFilter = scatteredAssetsFilter;
                                OnScatteredAssetsOrderOrFilterChanged();
                            }
                            EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(m_ScatteredAssetsFilter));
                            {
                                if (GUILayout.Button("x", GUILayout.Width(20f)))
                                {
                                    m_ScatteredAssetsFilter = null;
                                    GUI.FocusControl(null);
                                    OnScatteredAssetsOrderOrFilterChanged();
                                }
                            }
                            EditorGUI.EndDisabledGroup();
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical(GUILayout.Width(position.width * 0.6f - 14f));
                {
                    GUILayout.Space(5f);
                    EditorGUILayout.LabelField(Utility.Text.Format("Host Assets ({0})", m_SelectedHostAssets.Length.ToString()), EditorStyles.boldLabel);
                    EditorGUILayout.BeginVertical("box", GUILayout.Height(position.height - 68f));
                    {
                        m_HostAssetsScroll = EditorGUILayout.BeginScrollView(m_HostAssetsScroll);
                        {
                            foreach (AssetInfo hostAssetInfo in m_SelectedHostAssets)
                            {
                                EditorGUILayout.BeginHorizontal();
                                {
                                    if (GUILayout.Button("GO", GUILayout.Width(30f)))
                                    {
                                        m_SelectedAssetName = hostAssetInfo.Name;
                                        m_SelectedAssetIndex = (new List<string>(m_CachedAssetNames)).IndexOf(m_SelectedAssetName);
                                        m_SelectedDependencyData = m_Controller.GetDependencyData(m_SelectedAssetName);
                                        m_ToolbarIndex = 1;
                                        GUI.FocusControl(null);
                                    }

                                    GUILayout.Label(Utility.Text.Format("{0} [{1}]", hostAssetInfo.Name, hostAssetInfo.AssetBundleInfo.FullName));
                                }
                                EditorGUILayout.EndHorizontal();
                            }
                        }
                        EditorGUILayout.EndScrollView();
                    }
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();

        }

        //绘制间接依赖资源视图
        private void DrawCircularDependencyViewer()
        {
            if (!m_Analyzed)
            {
                DrawAnalyzeButton();
                return;
            }

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Space(5f);
                EditorGUILayout.BeginVertical();
                {
                    GUILayout.Space(5f);
                    EditorGUILayout.LabelField(Utility.Text.Format("Circular Dependency ({0})", m_CircularDependencyCount.ToString()), EditorStyles.boldLabel);
                    m_CircularDependencyScroll = EditorGUILayout.BeginScrollView(m_CircularDependencyScroll);
                    {
                        int count = 0;
                        foreach (string[] circularDependencyData in m_CachedCircularDependencyDatas)
                        {
                            GUILayout.Label(Utility.Text.Format("{0}) {1}", (++count).ToString(), circularDependencyData[circularDependencyData.Length - 1]), EditorStyles.boldLabel);
                            EditorGUILayout.BeginVertical("box");
                            {
                                foreach (string circularDependency in circularDependencyData)
                                {
                                    EditorGUILayout.BeginHorizontal();
                                    {
                                        GUILayout.Label(circularDependency);
                                        if (GUILayout.Button("GO", GUILayout.Width(30f)))
                                        {
                                            m_SelectedAssetName = circularDependency;
                                            m_SelectedAssetIndex = (new List<string>(m_CachedAssetNames)).IndexOf(m_SelectedAssetName);
                                            m_SelectedDependencyData = m_Controller.GetDependencyData(m_SelectedAssetName);
                                            m_ToolbarIndex = 1;
                                            GUI.FocusControl(null);
                                        }
                                    }
                                    EditorGUILayout.EndHorizontal();
                                }
                            }
                            EditorGUILayout.EndVertical();
                            GUILayout.Space(5f);
                        }
                    }
                    EditorGUILayout.EndScrollView();
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();
        }

        //绘制分析按钮
        private void DrawAnalyzeButton()
        {
            if (!m_Analyzed)
                EditorGUILayout.HelpBox("Please analyze first.", MessageType.Info); //先分析资源

            if(GUILayout.Button("Analyze", GUILayout.Height(30f)))  //分析按钮
            {
                m_Controller.Clear();

                m_SelectedAssetIndex = -1;
                m_SelectedAssetName = null;
                m_SelectedDependencyData = new DependencyData();    //创建依赖数据

                m_SelectedScatteredAssetIndex = -1;
                m_SelectedScatteredAssetName = null;
                m_SelectedHostAssets = new AssetInfo[] { };

                if (m_Controller.Prepare()) //加载xml配置
                {
                    m_Controller.Analyze(); //分析
                    m_Analyzed = true;
                    m_AssetCount = m_Controller.GetAssetNames().Length; //资源数量
                    m_ScatteredAssetCount = m_Controller.GetScatteredAssetNames().Length;   //零散资源数量
                    m_CachedCircularDependencyDatas = m_Controller.GetCircularDependencyDatas();    //缓存间接依赖资源数据
                    m_CircularDependencyCount = m_CachedCircularDependencyDatas.Length; //间接依赖数量
                    OnAssetsOrderOrFilterChanged();
                    OnScatteredAssetsOrderOrFilterChanged();
                }
                else
                {
                    //无法解析xml
                    EditorUtility.DisplayDialog("AssetBundle Analyze", "Can not parse 'AssetBundleCollection.xml', please use 'AssetBundle Editor' tool first.", "OK");
                }
            }
        }


        //资源排序或过滤修改
        private void OnAssetsOrderOrFilterChanged()
        {
            m_CachedAssetNames = m_Controller.GetAssetNames(m_AssetsOrder, m_AssetsFilter); //获取缓存资源名数组
            if (!string.IsNullOrEmpty(m_SelectedAssetName))
                m_SelectedAssetIndex = new List<string>(m_CachedAssetNames).IndexOf(m_SelectedAssetName);
        }

        //零散资源排序或过滤修改
        private void OnScatteredAssetsOrderOrFilterChanged()
        {
            //过滤缓存零散资源
            m_CachedScatteredAssetNames = m_Controller.GetScatteredAssetNames(m_ScatteredAssetsOrder, m_ScatteredAssetsFilter);
            //选中的零散资源下标
            if (!string.IsNullOrEmpty(m_SelectedScatteredAssetName))
                m_SelectedScatteredAssetIndex = new List<string>(m_CachedScatteredAssetNames).IndexOf(m_SelectedScatteredAssetName);
        }

        //加载Bundle的回调
        private void OnLoadingAssetBundle(int index, int count)
        {
            EditorUtility.DisplayProgressBar("Loading AssetBundles", Utility.Text.Format("Loading AssetBundles, {0}/{1} loaded.", index, count), (float)index / count);
        }

        //加载资源的回调
        private void OnLoadingAsset(int index, int count)
        {
            EditorUtility.DisplayProgressBar("Loading Assets", Utility.Text.Format("Loading assets, {0}/{1} loaded.", index.ToString(), count.ToString()), (float)index / count);
        }

        //加载完成的回调
        private void OnLoadCompleted()
        {
            EditorUtility.ClearProgressBar();
        }

        //分析资源的回调
        private void OnAnalyzingAsset(int index, int count)
        {
            EditorUtility.DisplayProgressBar("Analyzing assets", Utility.Text.Format("Analyzing assets, {0}/{1} analyzed.", index.ToString(), count.ToString()), (float)index / count);
        }

        //分析完成
        private void OnAnalyzeCompleted()
        {
            EditorUtility.ClearProgressBar();
        }

    }
}

