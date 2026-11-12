using UnityEditor;

/// <summary>
/// Matches the Shnok static-mesh import used by world tiles: Unreal units at 1/52.5,
/// no extra FBX file scale. Applies only to the Interlude login-lobby packages.
/// </summary>
public class LoginForestModelImport : AssetPostprocessor
{
    void OnPreprocessModel()
    {
        string path = assetPath.Replace('\\', '/');
        bool lobbyCard = path.Contains("/StaticMeshes/L2_Lobby/lobby_back.")
            || path.Contains("/StaticMeshes/L2_Lobby/L2_Lobby_sky_elf.");
        bool loginPkg = path.Contains("/StaticMeshes/Primitive_FieldOBJ_S/")
            || path.Contains("/StaticMeshes/field_deco_S/");
        if (!lobbyCard && !loginPkg)
            return;

        ModelImporter importer = (ModelImporter)assetImporter;
        importer.globalScale = 0.019f;
        importer.useFileScale = false;
        importer.bakeAxisConversion = false;
    }
}
