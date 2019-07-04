using GameFramework;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityGameFrame.Editor.AssetBundleTools
{
    /// <summary>
    /// 资源包编辑器。
    /// </summary>
    internal sealed partial class AssetBundleEditor : EditorWindow
    {
        private AssetBundleEditorController m_Controller = null;
        private MenuState m_MenuState = MenuState.Normal;
        private AssetBundleInfo m_SelectedAssetBundleInfo = null;
        private AssetBundleFolder m_AssetBundleRoot = null;
        private HashSet<string> m_ExpandedAssetBundleFolderNames = null;    //展开的Bundle文件夹名称
        private HashSet<AssetInfo> m_SelectedAssetsInSelectedAssetBundle = null;    //选中的Bundle对应的资源信息列表
        private HashSet<SourceFolder> m_ExpandedSourceFolders = null;   //缓存展开的文件夹列表
        private HashSet<SourceAsset> m_SelectedSourceAssets = null; //缓存选中的资源文件列表
        private Texture m_MissingSourceAssetIcon = null;

        private HashSet<SourceFolder> m_CachedSelectedSourceFolders = null; //缓存选中的资源文件夹
        private HashSet<SourceFolder> m_CachedUnselectedSourceFolders = null;   //缓存未选中的资源文件夹
        private HashSet<SourceFolder> m_CachedAssignedSourceFolders = null; //缓存已分配到Bundle中的资源文件夹
        private HashSet<SourceFolder> m_CachedUnassignedSourceFolders = null;   //缓存未分配到Bundle中的资源文件夹
        private HashSet<SourceAsset> m_CachedAssignedSourceAssets = null;   //缓存已分配到Bundle中的资源文件列表
        private HashSet<SourceAsset> m_CachedUnassignedSourceAssets = null; //缓存未分配到Bundle中的资源文件列表

        private Vector2 m_AssetBundlesViewScroll = Vector2.zero;
        private Vector2 m_AssetBundleViewScroll = Vector2.zero;
        private Vector2 m_SourceAssetsViewScroll = Vector2.zero;
        private string m_InputAssetBundleName = null;   //输入的Bundle名称
        private string m_InputAssetBundleVariant = null;    //输入的变体名称
        private bool m_HideAssignedSourceAssets = false;    //隐藏已分配资源的标志位
        private int m_CurrentAssetBundleContentCount = 0;   //当前BundleInfo中包含的资源信息内容数量
        private int m_CurrentAssetBundleRowOnDraw = 0;
        private int m_CurrentSourceRowOnDraw = 0;

        [MenuItem("Game Framework/AssetBundle Tools/AssetBundle Editor", false, 42)]
        private static void Open()
        {
            AssetBundleEditor window = GetWindow<AssetBundleEditor>(true, "AssetBundle Editor", true);
            window.minSize = new Vector2(1400f, 600f);
        }

        private void OnEnable()
        {
            //创建控制器并注册事件
            m_Controller = new AssetBundleEditorController();
            m_Controller.EventOnLoadingAssetBundle += OnLoadingAssetBundle;
            m_Controller.EventOnLoadingAsset += OnLoadingAsset;
            m_Controller.EventOnLoadCompleted += OnLoadCompleted;
            m_Controller.EventOnAssetAssigned += OnAssetAssigned;
            m_Controller.EventOnAssetUnassigned += OnAssetUnassigned;

            m_MenuState = MenuState.Normal;
            m_SelectedAssetBundleInfo = null;
            m_AssetBundleRoot = new AssetBundleFolder("AssetBundles", null);
            m_ExpandedAssetBundleFolderNames = new HashSet<string>();
            m_SelectedAssetsInSelectedAssetBundle = new HashSet<AssetInfo>();
            m_ExpandedSourceFolders = new HashSet<SourceFolder>();
            m_SelectedSourceAssets = new HashSet<SourceAsset>();
            m_MissingSourceAssetIcon = EditorGUIUtility.IconContent("console.warnicon.sml").image;

            m_CachedSelectedSourceFolders = new HashSet<SourceFolder>();
            m_CachedUnselectedSourceFolders = new HashSet<SourceFolder>();
            m_CachedAssignedSourceFolders = new HashSet<SourceFolder>();
            m_CachedUnassignedSourceFolders = new HashSet<SourceFolder>();
            m_CachedAssignedSourceAssets = new HashSet<SourceAsset>();
            m_CachedUnassignedSourceAssets = new HashSet<SourceAsset>();

            m_AssetBundlesViewScroll = Vector2.zero;
            m_AssetBundleViewScroll = Vector2.zero;
            m_SourceAssetsViewScroll = Vector2.zero;
            m_InputAssetBundleName = null;
            m_InputAssetBundleVariant = null;
            m_HideAssignedSourceAssets = false;
            m_CurrentAssetBundleContentCount = 0;
            m_CurrentAssetBundleRowOnDraw = 0;
            m_CurrentSourceRowOnDraw = 0;

            if (m_Controller.Load())
            {
                Debug.Log("Load configuration success.");
            }
            else
            {
                Debug.LogWarning("Load configuration failure.");
            }

            EditorUtility.DisplayProgressBar("Prepare AssetBundle Editor", "Processing...", 0f);
            RefreshAssetBundleTree();
            EditorUtility.ClearProgressBar();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal(GUILayout.Width(position.width), GUILayout.Height(position.height));
            {
                GUILayout.Space(2f);

                //AssetBundle信息列表
                EditorGUILayout.BeginVertical(GUILayout.Width(position.width * 0.25f));
                {
                    GUILayout.Space(5f);
                    EditorGUILayout.LabelField(Utility.Text.Format("AssetBundle List ({0})", m_Controller.AssetBundleCount.ToString()), EditorStyles.boldLabel);
                    EditorGUILayout.BeginHorizontal("box", GUILayout.Height(position.height - 52f));
                    {
                        DrawAssetBundleInfosView(); //绘制Bundle视图
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(5f);
                        DrawAssetBundleInfosMenu(); //绘制底部按钮
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();

                //AssetBundleInfo的内容
                EditorGUILayout.BeginVertical(GUILayout.Width(position.width * 0.25f));
                {
                    GUILayout.Space(5f);
                    EditorGUILayout.LabelField(Utility.Text.Format("AssetBundle Content ({0})", m_CurrentAssetBundleContentCount.ToString()), EditorStyles.boldLabel);
                    EditorGUILayout.BeginHorizontal("box", GUILayout.Height(position.height - 52f));
                    {
                        DrawAssetBundleAssetsView();    //绘制Bundle中包含的资源视图
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(5f);
                        DrawAssetBundleAssetsMenu();    //绘制底部菜单按钮
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();

                //绘制资源列表
                EditorGUILayout.BeginVertical(GUILayout.Width(position.width * 0.5f - 16f));
                {
                    GUILayout.Space(5f);
                    EditorGUILayout.LabelField("Asset List", EditorStyles.boldLabel);
                    EditorGUILayout.BeginHorizontal("box", GUILayout.Height(position.height - 52f));
                    {
                        DrawSourceAssetsView(); //绘制资源文件视图
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(5f);
                        DrawSourceAssetsMenu(); //绘制资源文件夹菜单按钮
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
                GUILayout.Space(5f);
            }
            EditorGUILayout.EndHorizontal();
        }

        //绘制Bundle视图
        private void DrawAssetBundleInfosView()
        {
            m_CurrentAssetBundleRowOnDraw = 0;
            m_AssetBundlesViewScroll = EditorGUILayout.BeginScrollView(m_AssetBundlesViewScroll);
            {
                DrawAssetBundleFolder(m_AssetBundleRoot);
            }
            EditorGUILayout.EndScrollView();
        }

        //绘制Bundle文件夹
        private void DrawAssetBundleFolder(AssetBundleFolder assetBundleFolder)
        {
            bool expand = IsExpandedAssetBundleFolder(assetBundleFolder);   //是否展开
            EditorGUILayout.BeginHorizontal();
            {
                if (expand != EditorGUI.Foldout(new Rect(18f + 14f * assetBundleFolder.Depth, 20f * m_CurrentAssetBundleRowOnDraw + 2f, int.MaxValue, 14f), expand, string.Empty, true))
                {
                    expand = !expand;
                    SetExpandedAssetBundleFolder(assetBundleFolder, expand);
                }

                GUI.DrawTexture(new Rect(32f + 14f * assetBundleFolder.Depth, 20f * m_CurrentAssetBundleRowOnDraw + 1f, 16f, 16f), AssetBundleFolder.Icon);
                EditorGUILayout.LabelField(string.Empty, GUILayout.Width(40f + 14f * assetBundleFolder.Depth), GUILayout.Height(18f));
                EditorGUILayout.LabelField(assetBundleFolder.Name);
            }
            EditorGUILayout.EndHorizontal();

            m_CurrentAssetBundleRowOnDraw++;

            //如果展开，递归绘制子文件夹
            if (expand)
            {
                foreach (AssetBundleFolder subAssetBundleFolder in assetBundleFolder.GetFolders())
                {
                    DrawAssetBundleFolder(subAssetBundleFolder);
                }

                foreach (AssetBundleItem assetBundleItem in assetBundleFolder.GetItems())
                {
                    DrawAssetBundleItem(assetBundleItem);
                }
            }
        }

        //绘制Bundle资源项
        private void DrawAssetBundleItem(AssetBundleItem assetBundleItem)
        {
            EditorGUILayout.BeginHorizontal();
            {
                string title = assetBundleItem.Name;
                if (assetBundleItem.AssetBundleInfo.Packed)
                    title = "[Packed] " + title;

                float emptySpace = position.width;
                if (EditorGUILayout.Toggle(m_SelectedAssetBundleInfo == assetBundleItem.AssetBundleInfo, GUILayout.Width(emptySpace - 12f)))
                {
                    ChangeSelectedAssetBundle(assetBundleItem.AssetBundleInfo);
                }
                else if (m_SelectedAssetBundleInfo == assetBundleItem.AssetBundleInfo)
                {
                    ChangeSelectedAssetBundle(null);
                }

                GUILayout.Space(-emptySpace + 24f);
                GUI.DrawTexture(new Rect(32f + 14f * assetBundleItem.Depth, 20f * m_CurrentAssetBundleRowOnDraw + 1f, 16f, 16f), assetBundleItem.Icon);
                EditorGUILayout.LabelField(string.Empty, GUILayout.Width(26f + 14f * assetBundleItem.Depth), GUILayout.Height(18f));
                EditorGUILayout.LabelField(title);
            }
            EditorGUILayout.EndHorizontal();
            m_CurrentAssetBundleRowOnDraw++;
        }

        //绘制Bundle菜单按钮
        private void DrawAssetBundleInfosMenu()
        {
            switch (m_MenuState)
            {
                case MenuState.Normal:  //普通菜单
                    DrawAssetBundlesMenu_Normal();
                    break;
                case MenuState.Add: //增加Bundle菜单
                    DrawAssetBundlesMenu_Add();
                    break;
                case MenuState.Rename:  //重命名Bunde菜单
                    DrawAssetBundlesMenu_Rename();
                    break;
                case MenuState.Remove:  //移除bundle菜单
                    DrawAssetBundlesMenu_Remove();
                    break;
            }
        }

        //绘制普通菜单按钮
        private void DrawAssetBundlesMenu_Normal()
        {
            if (GUILayout.Button("Add", GUILayout.Width(65f)))  //增加按钮
            {
                m_MenuState = MenuState.Add;
                m_InputAssetBundleName = null;
                m_InputAssetBundleVariant = null;
                GUI.FocusControl(null);
            }
            //选中的Bundle信息为null，则不显示以下内容
            EditorGUI.BeginDisabledGroup(m_SelectedAssetBundleInfo == null);
            {
                if (GUILayout.Button("Rename", GUILayout.Width(65f)))   //重命名按钮
                {
                    m_MenuState = MenuState.Rename;
                    m_InputAssetBundleName = m_SelectedAssetBundleInfo != null ? m_SelectedAssetBundleInfo.Name : null;
                    m_InputAssetBundleVariant = m_SelectedAssetBundleInfo != null ? m_SelectedAssetBundleInfo.Variant : null;
                    GUI.FocusControl(null);
                }
                if (GUILayout.Button("Remove", GUILayout.Width(65f)))   //移除按钮
                {
                    m_MenuState = MenuState.Remove;
                }
                if (m_SelectedAssetBundleInfo == null)
                {
                    EditorGUILayout.EnumPopup(AssetBundleLoadType.LoadFromFile);
                }
                else
                {
                    //加载方式
                    AssetBundleLoadType loadType = (AssetBundleLoadType)EditorGUILayout.EnumPopup(m_SelectedAssetBundleInfo.LoadType);
                    if (loadType != m_SelectedAssetBundleInfo.LoadType)
                    {
                        SetAssetBundleLoadType(loadType);
                    }
                }
                //是否打包
                bool packed = EditorGUILayout.ToggleLeft("Packed", m_SelectedAssetBundleInfo != null && m_SelectedAssetBundleInfo.Packed, GUILayout.Width(65f));
                if (m_SelectedAssetBundleInfo != null && packed != m_SelectedAssetBundleInfo.Packed)
                {
                    SetAssetBundlePacked(packed);
                }
            }
            EditorGUI.EndDisabledGroup();
        }

        //绘制增加AssetBundle的菜单按钮
        private void DrawAssetBundlesMenu_Add()
        {
            GUI.SetNextControlName("NewAssetBundleNameTextField");  //下一个控件名称
            m_InputAssetBundleName = EditorGUILayout.TextField(m_InputAssetBundleName);
            GUI.SetNextControlName("NewAssetBundleVariantTextField");   //下一个控件名称
            m_InputAssetBundleVariant = EditorGUILayout.TextField(m_InputAssetBundleVariant, GUILayout.Width(60f));

            if (GUI.GetNameOfFocusedControl() == "NewAssetBundleNameTextField" || GUI.GetNameOfFocusedControl() == "NewAssetBundleVariantTextField")
            {
                if (Event.current.isKey && Event.current.keyCode == KeyCode.Return)
                {
                    EditorUtility.DisplayProgressBar("Add AssetBundle", "Processing...", 0f);
                    AddAssetBundleInfo(m_InputAssetBundleName, m_InputAssetBundleVariant, true);    //添加AssetBundleInfo
                    EditorUtility.ClearProgressBar();
                    Repaint();  //重绘窗口
                }
            }

            if (GUILayout.Button("Add", GUILayout.Width(50f)))
            {
                EditorUtility.DisplayProgressBar("Add AssetBundle", "Processing...", 0f);
                AddAssetBundleInfo(m_InputAssetBundleName, m_InputAssetBundleVariant, true);    //添加AssetBundleInfo
                EditorUtility.ClearProgressBar();
            }

            if (GUILayout.Button("Back", GUILayout.Width(50f)))
            {
                m_MenuState = MenuState.Normal;
            }
        }

        //绘制重命名AssetBundle的菜单按钮
        private void DrawAssetBundlesMenu_Rename()
        {
            if (m_SelectedAssetBundleInfo == null)
            {
                m_MenuState = MenuState.Normal;
                return;
            }

            GUI.SetNextControlName("RenameAssetBundleNameTextField");   //下一个控件名称
            m_InputAssetBundleName = EditorGUILayout.TextField(m_InputAssetBundleName);
            GUI.SetNextControlName("RenameAssetBundleVariantTextField");    //下一个控件名称
            m_InputAssetBundleVariant = EditorGUILayout.TextField(m_InputAssetBundleVariant, GUILayout.Width(60f));

            if (GUI.GetNameOfFocusedControl() == "RenameAssetBundleNameTextField" || GUI.GetNameOfFocusedControl() == "RenameAssetBundleVariantTextField")
            {
                if (Event.current.isKey && Event.current.keyCode == KeyCode.Return)
                {
                    EditorUtility.DisplayProgressBar("Rename AssetBundle", "Processing...", 0f);
                    RenameAssetBundle(m_SelectedAssetBundleInfo, m_InputAssetBundleName, m_InputAssetBundleVariant);
                    EditorUtility.ClearProgressBar();
                    Repaint();
                }
            }

            if (GUILayout.Button("OK", GUILayout.Width(50f)))
            {
                EditorUtility.DisplayProgressBar("Rename AssetBundle", "Processing...", 0f);
                RenameAssetBundle(m_SelectedAssetBundleInfo, m_InputAssetBundleName, m_InputAssetBundleVariant);
                EditorUtility.ClearProgressBar();
            }

            if (GUILayout.Button("Back", GUILayout.Width(50f)))
            {
                m_MenuState = MenuState.Normal;
            }
        }

        //绘制移除BundleInfo的菜单按钮
        private void DrawAssetBundlesMenu_Remove()
        {
            if (m_SelectedAssetBundleInfo == null)
            {
                m_MenuState = MenuState.Normal;
                return;
            }

            GUILayout.Label(Utility.Text.Format("Remove '{0}' ?", m_SelectedAssetBundleInfo.FullName));

            if (GUILayout.Button("Yes", GUILayout.Width(50f)))
            {
                EditorUtility.DisplayProgressBar("Remove AssetBundle", "Processing...", 0f);
                RemoveAssetBundle();
                EditorUtility.ClearProgressBar();
                m_MenuState = MenuState.Normal;
            }

            if (GUILayout.Button("No", GUILayout.Width(50f)))
            {
                m_MenuState = MenuState.Normal;
            }
        }

        //绘制Bundle视图
        private void DrawAssetBundleAssetsView()
        {
            m_AssetBundleViewScroll = EditorGUILayout.BeginScrollView(m_AssetBundleViewScroll);
            {
                if (m_SelectedAssetBundleInfo != null)
                {
                    int index = 0;
                    AssetInfo[] assetInfos = m_Controller.GetAssetInfos(m_SelectedAssetBundleInfo.Name, m_SelectedAssetBundleInfo.Variant); //BundleInfo包含的资源信息
                    m_CurrentAssetBundleContentCount = assetInfos.Length;   //缓存资源数量
                    foreach (AssetInfo assetInfo in assetInfos)
                    {
                        SourceAsset sourceAsset = m_Controller.GetSourceAsset(assetInfo.Guid);  //获取资源文件
                        string assetName = sourceAsset != null ? (m_Controller.AssetSorter == AssetSorterType.Path ? sourceAsset.Path : (m_Controller.AssetSorter == AssetSorterType.Name ? sourceAsset.Name : sourceAsset.Guid)) : assetInfo.Guid;
                        EditorGUILayout.BeginHorizontal();
                        {
                            float emptySpace = position.width;
                            bool select = IsSelectedAssetInSelectedAssetBundle(assetInfo);
                            if (select != EditorGUILayout.Toggle(select, GUILayout.Width(emptySpace - 12f)))
                            {
                                select = !select;
                                SetSelectedAssetInSelectedAssetBundle(assetInfo, select);   //修改选中状态
                            }

                            GUILayout.Space(-emptySpace + 24f);
                            GUI.DrawTexture(new Rect(20f, 20f * (index++) + 1f, 16f, 16f), (sourceAsset != null ? sourceAsset.Icon : m_MissingSourceAssetIcon));
                            EditorGUILayout.LabelField(string.Empty, GUILayout.Width(14f), GUILayout.Height(18f));
                            EditorGUILayout.LabelField(assetName);
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
                else
                {
                    m_CurrentAssetBundleContentCount = 0;
                }
            }
            EditorGUILayout.EndScrollView();
        }

        //绘制bundle内容底部菜单按钮
        private void DrawAssetBundleAssetsMenu()
        {
            if (GUILayout.Button("All", GUILayout.Width(50f)) && m_SelectedAssetBundleInfo != null)
            {
                //全部选中
                AssetInfo[] assets = m_Controller.GetAssetInfos(m_SelectedAssetBundleInfo.Name, m_SelectedAssetBundleInfo.Variant);
                foreach (AssetInfo asset in assets)
                {
                    SetSelectedAssetInSelectedAssetBundle(asset, true);
                }
            }

            if (GUILayout.Button("None", GUILayout.Width(50f)))
            {
                //取消全选
                m_SelectedAssetsInSelectedAssetBundle.Clear();
            }
            //资源显示方式
            m_Controller.AssetSorter = (AssetSorterType)EditorGUILayout.EnumPopup(m_Controller.AssetSorter, GUILayout.Width(60f));
            GUILayout.Label(string.Empty);

            EditorGUI.BeginDisabledGroup(m_SelectedAssetBundleInfo == null || m_SelectedAssetsInSelectedAssetBundle.Count <= 0);
            {
                //从资源包中移除资源
                if (GUILayout.Button(Utility.Text.Format("{0} >>", m_SelectedAssetsInSelectedAssetBundle.Count.ToString()), GUILayout.Width(80f)))
                {
                    foreach (AssetInfo asset in m_SelectedAssetsInSelectedAssetBundle)
                    {
                        UnassignAssetInfo(asset);   //取消分配资源
                    }

                    m_SelectedAssetsInSelectedAssetBundle.Clear();  //清空选中的资源
                }
            }
            EditorGUI.EndDisabledGroup();
        }

        //绘制资源文件视图
        private void DrawSourceAssetsView()
        {
            m_CurrentSourceRowOnDraw = 0;
            m_SourceAssetsViewScroll = EditorGUILayout.BeginScrollView(m_SourceAssetsViewScroll);
            {
                DrawSourceFolder(m_Controller.SourceAssetRoot); //绘制资源文件夹
            }
            EditorGUILayout.EndScrollView();
        }

        //绘制资源文件夹菜单按钮
        private void DrawSourceAssetsMenu()
        {
            HashSet<SourceAsset> selectedSourceAssets = GetSelectedSourceAssets();
            EditorGUI.BeginDisabledGroup(m_SelectedAssetBundleInfo == null || selectedSourceAssets.Count <= 0);
            {
                if (GUILayout.Button(Utility.Text.Format("<< {0}", selectedSourceAssets.Count.ToString()), GUILayout.Width(80f)))
                {
                    foreach (SourceAsset sourceAsset in selectedSourceAssets)
                    {
                        AssignAsset(sourceAsset, m_SelectedAssetBundleInfo);    //分配资源
                    }

                    m_SelectedSourceAssets.Clear();
                    m_CachedSelectedSourceFolders.Clear();
                }
            }
            EditorGUI.EndDisabledGroup();
            EditorGUI.BeginDisabledGroup(selectedSourceAssets.Count <= 0);
            {
                if (GUILayout.Button(Utility.Text.Format("<<< {0}", selectedSourceAssets.Count.ToString()), GUILayout.Width(80f)))
                {
                    int index = 0;
                    int count = selectedSourceAssets.Count;
                    foreach (SourceAsset sourceAsset in selectedSourceAssets)
                    {
                        EditorUtility.DisplayProgressBar("Add AssetBundles", Utility.Text.Format("{0}/{1} processing...", (++index).ToString(), count.ToString()), (float)index / count);
                        int dotIndex = sourceAsset.FromRootPath.IndexOf('.');
                        string assetBundleName = dotIndex > 0 ? sourceAsset.FromRootPath.Substring(0, dotIndex) : sourceAsset.FromRootPath;
                        AddAssetBundleInfo(assetBundleName, null, false);   //每一个资源都添加一个Bundle
                        AssetBundleInfo assetBundle = m_Controller.GetAssetBundleInfo(assetBundleName, null);
                        if (assetBundle == null)
                            continue;

                        AssignAsset(sourceAsset, assetBundle);  //分配资源到指定Bundle中
                    }

                    EditorUtility.DisplayProgressBar("Add AssetBundles", "Complete processing...", 1f);
                    RefreshAssetBundleTree();   //刷新树状列表
                    EditorUtility.ClearProgressBar();
                    m_SelectedSourceAssets.Clear(); //清空选中资源
                    m_CachedSelectedSourceFolders.Clear();  //清空选中资源文件夹
                }
            }
            EditorGUI.EndDisabledGroup();
            bool hideAssignedSourceAssets = EditorGUILayout.ToggleLeft("Hide Assigned", m_HideAssignedSourceAssets, GUILayout.Width(100f));
            if (hideAssignedSourceAssets != m_HideAssignedSourceAssets)
            {
                m_HideAssignedSourceAssets = hideAssignedSourceAssets;
                m_CachedSelectedSourceFolders.Clear();
                m_CachedUnselectedSourceFolders.Clear();
                m_CachedAssignedSourceFolders.Clear();
                m_CachedUnassignedSourceFolders.Clear();
            }

            GUILayout.Label(string.Empty);
            //清理无效资源和资源包
            if (GUILayout.Button("Clean", GUILayout.Width(80f)))
            {
                EditorUtility.DisplayProgressBar("Clean", "Processing...", 0f);
                CleanAssetBundle();
                EditorUtility.ClearProgressBar();
            }

            //保存
            if (GUILayout.Button("Save", GUILayout.Width(80f)))
            {
                EditorUtility.DisplayProgressBar("Save", "Processing...", 0f);
                SaveConfiguration();
                EditorUtility.ClearProgressBar();
            }
        }

        //绘制资源文件夹
        private void DrawSourceFolder(SourceFolder sourceFolder)
        {
            if (m_HideAssignedSourceAssets && IsAssignedSourceFolder(sourceFolder))
                return;

            bool expand = IsExpandedSourceFolder(sourceFolder); //是否展开了文件夹
            EditorGUILayout.BeginHorizontal();
            {
                bool select = IsSelectedSourceFolder(sourceFolder);
                if (select != EditorGUILayout.Toggle(select, GUILayout.Width(12f + 14f * sourceFolder.Depth)))
                {
                    select = !select;
                    SetSelectedSourceFolder(sourceFolder, select);
                }

                GUILayout.Space(-14f * sourceFolder.Depth);
                if (expand != EditorGUI.Foldout(new Rect(18f + 14f * sourceFolder.Depth, 20f * m_CurrentSourceRowOnDraw + 2f, int.MaxValue, 14f), expand, string.Empty, true))
                {
                    expand = !expand;
                    SetExpandedSourceFolder(sourceFolder, expand);
                }

                GUI.DrawTexture(new Rect(32f + 14f * sourceFolder.Depth, 20f * m_CurrentSourceRowOnDraw + 1f, 16f, 16f), SourceFolder.Icon);
                EditorGUILayout.LabelField(string.Empty, GUILayout.Width(26f + 14f * sourceFolder.Depth), GUILayout.Height(18f));
                EditorGUILayout.LabelField(sourceFolder.Name);
            }
            EditorGUILayout.EndHorizontal();

            m_CurrentSourceRowOnDraw++;

            if (expand)
            {
                foreach (SourceFolder subSourceFolder in sourceFolder.GetFolders())
                {
                    DrawSourceFolder(subSourceFolder);
                }

                foreach (SourceAsset sourceAsset in sourceFolder.GetAssets())
                {
                    DrawSourceAsset(sourceAsset);
                }
            }
        }

        private void DrawSourceAsset(SourceAsset sourceAsset)
        {
            if (m_HideAssignedSourceAssets && IsAssignedSourceAsset(sourceAsset))
            {
                return;
            }

            EditorGUILayout.BeginHorizontal();
            {
                float emptySpace = position.width;
                bool select = IsSelectedSourceAsset(sourceAsset);
                if (select != EditorGUILayout.Toggle(select, GUILayout.Width(emptySpace - 12f)))
                {
                    select = !select;
                    SetSelectedSourceAsset(sourceAsset, select);
                }

                GUILayout.Space(-emptySpace + 24f);
                GUI.DrawTexture(new Rect(32f + 14f * sourceAsset.Depth, 20f * m_CurrentSourceRowOnDraw + 1f, 16f, 16f), sourceAsset.Icon);
                EditorGUILayout.LabelField(string.Empty, GUILayout.Width(26f + 14f * sourceAsset.Depth), GUILayout.Height(18f));
                EditorGUILayout.LabelField(sourceAsset.Name);
                AssetInfo asset = m_Controller.GetAsset(sourceAsset.Guid);
                EditorGUILayout.LabelField(asset != null ? GetAssetBundleFullName(asset.AssetBundleInfo.Name, asset.AssetBundleInfo.Variant) : string.Empty, GUILayout.Width(position.width * 0.15f));
            }
            EditorGUILayout.EndHorizontal();
            m_CurrentSourceRowOnDraw++;
        }

        //修改选择的Bundle名称
        private void ChangeSelectedAssetBundle(AssetBundleInfo assetBundleInfo)
        {
            if (m_SelectedAssetBundleInfo == assetBundleInfo)
                return;

            m_SelectedAssetBundleInfo = assetBundleInfo;
            m_SelectedAssetsInSelectedAssetBundle.Clear();
        }

        //保存配置
        private void SaveConfiguration()
        {
            if (m_Controller.Save())
                Debug.Log("Save configuration success.");
            else
                Debug.LogWarning("Save configuration failure.");
        }

        //添加BundleInfo
        private void AddAssetBundleInfo(string assetBundleName, string assetBundleVariant, bool refresh)
        {
            if (assetBundleVariant == string.Empty) //变体为空
                assetBundleVariant = null;

            string assetBundleFullName = GetAssetBundleFullName(assetBundleName, assetBundleVariant);
            if (m_Controller.AddAssetBundleInfo(assetBundleName, assetBundleVariant, AssetBundleLoadType.LoadFromFile, false))
            {
                if (refresh)
                    RefreshAssetBundleTree();   //刷新Bundle树状列表

                Debug.Log(Utility.Text.Format("Add AssetBundle '{0}' success.", assetBundleFullName));
                m_MenuState = MenuState.Normal;
            }
            else
            {
                Debug.LogWarning(Utility.Text.Format("Add AssetBundle '{0}' failure.", assetBundleFullName));
            }
        }

        //重命名AssetBundleInfo信息
        private void RenameAssetBundle(AssetBundleInfo assetBundleInfo, string newAssetBundleName, string newAssetBundleVariant)
        {
            if (assetBundleInfo == null)
            {
                Debug.LogWarning("AssetBundle is invalid.");
                return;
            }

            if (newAssetBundleVariant == string.Empty)
                newAssetBundleVariant = null;

            string oldAssetBundleFullName = assetBundleInfo.FullName;
            string newAssetBundleFullName = GetAssetBundleFullName(newAssetBundleName, newAssetBundleVariant);
            if (m_Controller.RenameAssetBundleInfo(assetBundleInfo.Name, assetBundleInfo.Variant, newAssetBundleName, newAssetBundleVariant))
            {
                RefreshAssetBundleTree();
                Debug.Log(Utility.Text.Format("Rename AssetBundle '{0}' to '{1}' success.", oldAssetBundleFullName, newAssetBundleFullName));
                m_MenuState = MenuState.Normal;
            }
            else
            {
                Debug.LogWarning(Utility.Text.Format("Rename AssetBundle '{0}' to '{1}' failure.", oldAssetBundleFullName, newAssetBundleFullName));
            }
        }

        //移除选中的BundleInfo
        private void RemoveAssetBundle()
        {
            string assetBundleFullName = m_SelectedAssetBundleInfo.FullName;    //bundleInfo全名
            if (m_Controller.RemoveAssetBundleInfo(m_SelectedAssetBundleInfo.Name, m_SelectedAssetBundleInfo.Variant))
            {
                ChangeSelectedAssetBundle(null);
                RefreshAssetBundleTree();
                Debug.Log(Utility.Text.Format("Remove AssetBundle '{0}' success.", assetBundleFullName));
            }
            else
            {
                Debug.LogWarning(Utility.Text.Format("Remove AssetBundle '{0}' failure.", assetBundleFullName));
            }
        }

        //设置选中Bundle的加载方式
        private void SetAssetBundleLoadType(AssetBundleLoadType loadType)
        {
            string assetBundleFullName = m_SelectedAssetBundleInfo.FullName;
            if (m_Controller.SetAssetBundleLoadType(m_SelectedAssetBundleInfo.Name, m_SelectedAssetBundleInfo.Variant, loadType))
            {
                Debug.Log(Utility.Text.Format("Set AssetBundle '{0}' load type to '{1}' success.", assetBundleFullName, loadType.ToString()));
            }
            else
            {
                Debug.LogWarning(Utility.Text.Format("Set AssetBundle '{0}' load type to '{1}' failure.", assetBundleFullName, loadType.ToString()));
            }
        }

        //设置Bundle打包标志位
        private void SetAssetBundlePacked(bool pack)
        {
            string assetBundleFullName = m_SelectedAssetBundleInfo.FullName;
            if (m_Controller.SetAssetBundlePacked(m_SelectedAssetBundleInfo.Name, m_SelectedAssetBundleInfo.Variant, pack))
            {
                Debug.Log(Utility.Text.Format("{1} AssetBundle '{0}' success.", assetBundleFullName, pack ? "Pack" : "Unpack"));
            }
            else
            {
                Debug.LogWarning(Utility.Text.Format("{1} AssetBundle '{0}' failure.", assetBundleFullName, pack ? "Pack" : "Unpack"));
            }
        }

        //分配资源
        private void AssignAsset(SourceAsset sourceAsset, AssetBundleInfo assetBundleInfo)
        {
            if (!m_Controller.AssignAsset(sourceAsset.Guid, assetBundleInfo.Name, assetBundleInfo.Variant))
            {
                Debug.LogWarning(Utility.Text.Format("Assign asset '{0}' to AssetBundle '{1}' failure.", sourceAsset.Name, assetBundleInfo.FullName));
            }
        }

        //取消分配资源
        private void UnassignAssetInfo(AssetInfo asset)
        {
            if (!m_Controller.UnassignAssetInfo(asset.Guid))
            {
                Debug.LogWarning(Utility.Text.Format("Unassign asset '{0}' from AssetBundle '{1}' failure.", asset.Guid, m_SelectedAssetBundleInfo.FullName));
            }
        }

        //清空资源包
        private void CleanAssetBundle()
        {
            int unknownAssetCount = m_Controller.RemoveUnknownAssetInfos(); //移除无效资源信息，返回无效资源数量
            int unusedAssetBundleCount = m_Controller.RemoveUnusedAssetBundles();
            RefreshAssetBundleTree();

            Debug.Log(Utility.Text.Format("Clean complete, {0} unknown assets and {1} unused AssetBundles has been removed.", unknownAssetCount.ToString(), unusedAssetBundleCount.ToString()));
        }

        //刷新树状列表
        private void RefreshAssetBundleTree()
        {
            m_AssetBundleRoot.Clear();
            AssetBundleInfo[] assetBundleInfos = m_Controller.GetAssetBundleInfos();
            foreach (AssetBundleInfo assetBundleInfo in assetBundleInfos)
            {
                string[] splitPath = assetBundleInfo.Name.Split('/');
                AssetBundleFolder folder = m_AssetBundleRoot;
                for (int i = 0; i < splitPath.Length - 1; i++)
                {
                    AssetBundleFolder subFolder = folder.GetFolder(splitPath[i]); //获取存在的子文件夹
                    folder = subFolder == null ? folder.AddFolder(splitPath[i]) : subFolder;  //不存在则添加
                }
                //最后添加Bundle全名
                string assetBundleFullName = assetBundleInfo.Variant != null ? Utility.Text.Format("{0}.{1}", splitPath[splitPath.Length - 1], assetBundleInfo.Variant) : splitPath[splitPath.Length - 1];
                folder.AddItem(assetBundleFullName, assetBundleInfo);   //添加Bundle资源项
            }
        }

        //判断文件夹是否展开
        private bool IsExpandedAssetBundleFolder(AssetBundleFolder assetBundleFolder)
        {
            return m_ExpandedAssetBundleFolderNames.Contains(assetBundleFolder.FromRootPath);
        }

        //设置Bundle文件夹是否展开，修改展开列表
        private void SetExpandedAssetBundleFolder(AssetBundleFolder assetBundleFolder, bool expand)
        {
            if (expand)
            {
                m_ExpandedAssetBundleFolderNames.Add(assetBundleFolder.FromRootPath);
            }
            else
            {
                m_ExpandedAssetBundleFolderNames.Remove(assetBundleFolder.FromRootPath);
            }
        }
        
        //是否选中资源包中的资源文件信息
        private bool IsSelectedAssetInSelectedAssetBundle(AssetInfo asset)
        {
            return m_SelectedAssetsInSelectedAssetBundle.Contains(asset);
        }

        //修改资源包中资源信息的选中状态
        private void SetSelectedAssetInSelectedAssetBundle(AssetInfo asset, bool select)
        {
            if (select)
                m_SelectedAssetsInSelectedAssetBundle.Add(asset);
            else
                m_SelectedAssetsInSelectedAssetBundle.Remove(asset);
        }

        private bool IsExpandedSourceFolder(SourceFolder sourceFolder)
        {
            return m_ExpandedSourceFolders.Contains(sourceFolder);
        }

        private void SetExpandedSourceFolder(SourceFolder sourceFolder, bool expand)
        {
            if (expand)
            {
                m_ExpandedSourceFolders.Add(sourceFolder);
            }
            else
            {
                m_ExpandedSourceFolders.Remove(sourceFolder);
            }
        }

        private bool IsSelectedSourceFolder(SourceFolder sourceFolder)
        {
            if (m_CachedSelectedSourceFolders.Contains(sourceFolder))
            {
                return true;
            }

            if (m_CachedUnselectedSourceFolders.Contains(sourceFolder))
            {
                return false;
            }

            foreach (SourceAsset sourceAsset in sourceFolder.GetAssets())
            {
                if (m_HideAssignedSourceAssets && IsAssignedSourceAsset(sourceAsset))
                {
                    continue;
                }

                if (!IsSelectedSourceAsset(sourceAsset))
                {
                    m_CachedUnselectedSourceFolders.Add(sourceFolder);
                    return false;
                }
            }

            foreach (SourceFolder subSourceFolder in sourceFolder.GetFolders())
            {
                if (m_HideAssignedSourceAssets && IsAssignedSourceFolder(sourceFolder))
                {
                    continue;
                }

                if (!IsSelectedSourceFolder(subSourceFolder))
                {
                    m_CachedUnselectedSourceFolders.Add(sourceFolder);
                    return false;
                }
            }

            m_CachedSelectedSourceFolders.Add(sourceFolder);
            return true;
        }

        //修改资源文件夹选中状态
        private void SetSelectedSourceFolder(SourceFolder sourceFolder, bool select)
        {
            if (select)
            {
                m_CachedSelectedSourceFolders.Add(sourceFolder);
                m_CachedUnselectedSourceFolders.Remove(sourceFolder);

                SourceFolder folder = sourceFolder;
                while (folder != null)
                {
                    m_CachedUnselectedSourceFolders.Remove(folder);
                    folder = folder.Folder;
                }
            }
            else
            {
                m_CachedSelectedSourceFolders.Remove(sourceFolder);
                m_CachedUnselectedSourceFolders.Add(sourceFolder);

                SourceFolder folder = sourceFolder;
                while (folder != null)
                {
                    m_CachedSelectedSourceFolders.Remove(folder);
                    folder = folder.Folder;
                }
            }

            foreach (SourceAsset sourceAsset in sourceFolder.GetAssets())
            {
                if (m_HideAssignedSourceAssets && IsAssignedSourceAsset(sourceAsset))
                    continue;

                SetSelectedSourceAsset(sourceAsset, select);
            }

            foreach (SourceFolder subSourceFolder in sourceFolder.GetFolders())
            {
                if (m_HideAssignedSourceAssets && IsAssignedSourceFolder(subSourceFolder))
                    continue;

                SetSelectedSourceFolder(subSourceFolder, select);
            }
        }

        private bool IsSelectedSourceAsset(SourceAsset sourceAsset)
        {
            return m_SelectedSourceAssets.Contains(sourceAsset);
        }

        //修改资源的选中状态
        private void SetSelectedSourceAsset(SourceAsset sourceAsset, bool select)
        {
            if (select)
            {
                m_SelectedSourceAssets.Add(sourceAsset);

                SourceFolder folder = sourceAsset.Folder;
                while (folder != null)
                {
                    m_CachedUnselectedSourceFolders.Remove(folder);
                    folder = folder.Folder;
                }
            }
            else
            {
                m_SelectedSourceAssets.Remove(sourceAsset);

                SourceFolder folder = sourceAsset.Folder;
                while (folder != null)
                {
                    m_CachedSelectedSourceFolders.Remove(folder);
                    folder = folder.Folder;
                }
            }
        }

        //判断当前资源文件是否被分配到Bundle中
        private bool IsAssignedSourceAsset(SourceAsset sourceAsset)
        {
            if (m_CachedAssignedSourceAssets.Contains(sourceAsset))
                return true;

            if (m_CachedUnassignedSourceAssets.Contains(sourceAsset))
                return false;

            return m_Controller.GetAsset(sourceAsset.Guid) != null;
        }

        //判断当前资源文件夹是否被分配到Bundle中
        private bool IsAssignedSourceFolder(SourceFolder sourceFolder)
        {
            if (m_CachedAssignedSourceFolders.Contains(sourceFolder))
                return true;

            if (m_CachedUnassignedSourceFolders.Contains(sourceFolder))
                return false;

            foreach (SourceAsset sourceAsset in sourceFolder.GetAssets())
            {
                if (!IsAssignedSourceAsset(sourceAsset))
                {
                    m_CachedUnassignedSourceFolders.Add(sourceFolder);
                    return false;
                }
            }

            foreach (SourceFolder subSourceFolder in sourceFolder.GetFolders())
            {
                if (!IsAssignedSourceFolder(subSourceFolder))
                {
                    m_CachedUnassignedSourceFolders.Add(sourceFolder);
                    return false;
                }
            }

            m_CachedAssignedSourceFolders.Add(sourceFolder);
            return true;
        }

        private HashSet<SourceAsset> GetSelectedSourceAssets()
        {
            if (!m_HideAssignedSourceAssets)
            {
                return m_SelectedSourceAssets;
            }

            HashSet<SourceAsset> selectedUnassignedSourceAssets = new HashSet<SourceAsset>();
            foreach (SourceAsset sourceAsset in m_SelectedSourceAssets)
            {
                if (!IsAssignedSourceAsset(sourceAsset))
                {
                    selectedUnassignedSourceAssets.Add(sourceAsset);
                }
            }

            return selectedUnassignedSourceAssets;
        }

        //获取Bundle的全名
        private string GetAssetBundleFullName(string assetBundleName, string assetBundleVariant)
        {
            return assetBundleVariant != null ? Utility.Text.Format("{0}.{1}", assetBundleName, assetBundleVariant) : assetBundleName;
        }

        private void OnLoadingAssetBundle(int index, int count)
        {
            EditorUtility.DisplayProgressBar("Loading AssetBundles", Utility.Text.Format("Loading AssetBundles, {0}/{1} loaded.", index.ToString(), count.ToString()), (float)index / count);
        }

        private void OnLoadingAsset(int index, int count)
        {
            EditorUtility.DisplayProgressBar("Loading Assets", Utility.Text.Format("Loading assets, {0}/{1} loaded.", index.ToString(), count.ToString()), (float)index / count);
        }

        private void OnLoadCompleted()
        {
            EditorUtility.ClearProgressBar();
        }

        private void OnAssetAssigned(SourceAsset[] sourceAssets)
        {
            HashSet<SourceFolder> affectedFolders = new HashSet<SourceFolder>();
            foreach (SourceAsset sourceAsset in sourceAssets)
            {
                m_CachedAssignedSourceAssets.Add(sourceAsset);
                m_CachedUnassignedSourceAssets.Remove(sourceAsset);

                affectedFolders.Add(sourceAsset.Folder);
            }

            foreach (SourceFolder sourceFolder in affectedFolders)
            {
                SourceFolder folder = sourceFolder;
                while (folder != null)
                {
                    m_CachedUnassignedSourceFolders.Remove(folder);
                    folder = folder.Folder;
                }
            }
        }

        //取消分配资源的回调
        private void OnAssetUnassigned(SourceAsset[] sourceAssets)
        {
            HashSet<SourceFolder> affectedFolders = new HashSet<SourceFolder>();
            foreach (SourceAsset sourceAsset in sourceAssets)
            {
                m_CachedAssignedSourceAssets.Remove(sourceAsset);
                m_CachedUnassignedSourceAssets.Add(sourceAsset);

                affectedFolders.Add(sourceAsset.Folder);
            }

            foreach (SourceFolder sourceFolder in affectedFolders)
            {
                SourceFolder folder = sourceFolder;
                while (folder != null)  //递归处理文件夹
                {
                    m_CachedSelectedSourceFolders.Remove(folder);
                    m_CachedAssignedSourceFolders.Remove(folder);
                    m_CachedUnassignedSourceFolders.Add(folder);
                    folder = folder.Folder;
                }
            }
        }
    }
}
