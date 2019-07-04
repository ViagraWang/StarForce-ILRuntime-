using GameFramework;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace UnityGameFrame.Editor.AssetBundleTools
{
    //Bundle编辑器配置
    public class AssetBundleEditorConfig
    {

        //xml配置文件的内容
        private const string m_config = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n"
            + "<UnityGameFramework>\n"
              + "\t<AssetBundleEditor>\n"
                + "\t\t<Settings>\n"
                  + "\t\t\t<!--配置资源搜索的根目录，默认是从 Assets 开始进行全部查找，由于经常在 Assets 下放一些 Unity 插件，故也可以以某一级子目录（如 Asset/GameMain）作为根目录进行资源查找。-->\n"
                  + "\t\t\t<SourceAssetRootPath>Assets/GameMain</SourceAssetRootPath>\n"
                  + "\t\t\t<!--配置资源搜索的子目录（相对于根目录的路径），每个子目录填写一行 SourceAssetSearchPath，若不填则搜索所有子目录。-->\n"
                  + "\t\t\t<SourceAssetSearchPaths>\n"
                    + "\t\t\t\t<SourceAssetSearchPath RelativePath = \"\" />\n"
                  + "\t\t\t</SourceAssetSearchPaths>\n"
                  + "\t\t\t<!--要筛选并包含的资源类型，默认会包含场景（t:Scene）、预制体（t:Prefab）、着色器（t:Shader）、材质（t:Material）、贴图（t:Texture）、声音（t:AudioClip）、动画（t:AnimationClip t:AnimatorController）、字体（t:Font）、文本（t:TextAsset t:ScriptableObject）资源。-->\n"
                  + "\t\t\t<SourceAssetUnionTypeFilter>t:Scene t:Prefab t:Shader t:Model t:Material t:Texture t:AudioClip t:AnimationClip t:AnimatorController t:Font t:TextAsset t:ScriptableObject</SourceAssetUnionTypeFilter>\n"
                  + "\t\t\t<!--要筛选并包含的标签类型，默认会包含带有 AssetBundleInclusive 标签（l:AssetBundleInclusive）的资源。-->\n"
                  + "\t\t\t<SourceAssetUnionLabelFilter>l:AssetBundleInclusive</SourceAssetUnionLabelFilter>\n"
                  + "\t\t\t<!--要筛选并排除的资源类型，默认会排除脚本（t:Script）资源-->\n"
                  + "\t\t\t<SourceAssetExceptTypeFilter>t:Script</SourceAssetExceptTypeFilter>\n"
                  + "\t\t\t<!--要筛选并排除的标签类型，默认会排除带有 AssetBundleExclusive 标签（l:AssetBundleExclusive）的资源。-->\n"
                  + "\t\t\t<SourceAssetExceptLabelFilter>l:AssetBundleExclusive</SourceAssetExceptLabelFilter>\n"
                  + "\t\t\t<!--编辑器内资源列表排序顺序，可以是 Name（资源文件名）、Path（资源全路径）或者 Guid（资源GUID）。-->\n"
                  + "\t\t\t<AssetSorter>Path</AssetSorter>\n"
                + "\t\t</Settings>\n"
              + "\t</AssetBundleEditor>\n"
            + "</UnityGameFramework>\n"
            + "<!--最终筛选出的资源 = SourceAssetUnionTypeFilter 筛选出的资源 + SourceAssetUnionLabelFilter 筛选出的资源 – SourceAssetExceptTypeFilter 筛选出的资源 – SourceAssetExceptLabelFilter 筛选出的资源。-->";

        [MenuItem("Game Framework/AssetBundle Tools/Create AssetBundleEditor.xml", false, 0)]
        private static void CreateBundleEditorConfig()
        {
            string path = Type.GetConfigurationPath<AssetBundleEditorConfigPathAttribute>() ?? Utility.Path.GetCombinePath(Application.dataPath, "GameFramework/Configs/AssetBundleEditor.xml");
            EditorUtility.DisplayProgressBar("创建AssetBundleEditor.xml", "正在创建配置文件AssetBundleEditor.xml，请稍等...", 0.5f);
            bool isCreate = true;
            if (File.Exists(path))
            {
                isCreate = EditorUtility.DisplayDialog("警告：已存在AssetBundleEditor.xml文件", Utility.Text.Format("路径:{0}，是否替换？", path), "替换", "取消");
                if (isCreate)
                    File.Delete(path);
            }
            else
            {
                //创建文件夹
                string directory = Path.GetDirectoryName(path);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
            }
            if (isCreate)
            {
                StreamWriter sw = null;
                try
                {
                    sw = File.CreateText(path);
                    sw.Write(m_config);
                    Debug.Log("创建AssetBundleEditor.xml完成。\n路径 -> " + path);
                }
                catch (System.Exception e)
                {
                    Debug.LogError("创建AssetBundleEditor.xml异常 -> " + e.Message);
                }
                finally
                {
                    if (sw != null)
                    {
                        sw.Close();
                        sw.Dispose();
                        sw = null;
                    }
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            
            EditorUtility.ClearProgressBar();
        }

    }

}
