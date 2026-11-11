#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static partial class L2InstancedPropBaker
{
    [MenuItem("L2/Map/Bake 17_25 DecoLayer To Instances")]
    public static void BakeSeventeenTwentyFiveDeco()
    {
        BakeDecoPrefab(
            "Assets/Resources/Data/Maps/17_25/17_25_DecoLayer.prefab",
            "Assets/Resources/Data/Maps/17_25/17_25_DecoInstances.asset");
    }
}
#endif
