using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Runtime
{
    public static partial class RuntimeConstant
    {
        /// <summary>
        /// 基础资源加载的优先级
        /// </summary>
        public static class AssetPriority
        {
            public const int ConfigAsset = 100;
            public const int DataTableAsset = 100;
            public const int DictionaryAsset = 100;
            public const int FontAsset = 50;
            public const int MusicAsset = 20;
            public const int SceneAsset = 0;
            public const int SoundAsset = 30;
            public const int UIFormAsset = 50;
            public const int UISoundAsset = 30;
            public const int HotfixAsset = 10;


        }
    }

}
