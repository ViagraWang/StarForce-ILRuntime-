
namespace UnityGameFrame.Editor.AssetBundleTools
{
    /// <summary>
    /// 资源排序
    /// </summary>
    public enum AssetsOrder
    {
        AssetNameAsc,   //资源名升序
        AssetNameDesc,  //资源名降序
        DependencyAssetBundleCountAsc,  //依赖资源包数量升序
        DependencyAssetBundleCountDesc,  //依赖资源包数量降序
        DependencyAssetCountAsc,    //依赖资源数量升序
        DependencyAssetCountDesc,   //依赖资源数量降序
        ScatteredDependencyAssetCountAsc,   //零散依赖资源数量升序
        ScatteredDependencyAssetCountDesc,  //零散依赖资源数量降序
    }
}
