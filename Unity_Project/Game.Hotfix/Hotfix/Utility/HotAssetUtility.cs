using Game.Runtime;
using GameFramework;

namespace Game.Hotfix
{
    //资源扩展工具
    public static class HotAssetUtility
    {
        public const string UIItemPath = "Assets/GameMain/UI/UIItems";  //UIItem路径


        //获取UIItems资源内置路径
        public static string GetUIItemsAsset(string assetName)
        {
            return Utility.Text.Format("{0}/{1}.prefab", UIItemPath, assetName);
        }

    }
}
