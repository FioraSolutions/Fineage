#if (UNITY_EDITOR) 
using System.IO;
using UnityEditor;
using UnityEngine;

public partial class L2MaterialBuilder
{
    [MenuItem("Shnok/2. [Material] Generate staticmesh materials")]
    static void SetupMaterialsMenu() {
        RunSetupMaterials();
    }

    public static void RunSetupMaterials() {
        bool overwrite = false;
        if(overwrite) {
            ClearMaterials();
        }

        ProcessProps(overwrite);

        CreateBaseMaterials(overwrite);
    }
}
#endif