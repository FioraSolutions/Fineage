using System.Text;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Batch: -executeMethod LobbyRaceMeshDiag.Run
/// Exercises the real UserGear / ItemTable paperdoll path (not manual FBX parent).
/// </summary>
public static class LobbyRaceMeshDiag
{
    public static void Run()
    {
        try
        {
            BootstrapTables();
            DiagLivePath(CharacterRaceAnimation.MOrc, "Data/Animations/Orc/MOrc/Pawn_MOrc");
            DiagLivePath(CharacterRaceAnimation.FDwarf, "Data/Animations/Dwarf/FDwarf/Pawn_FDwarf");
            DiagLivePath(CharacterRaceAnimation.MOrc, "Data/Animations/Orc/MOrc/Player_MOrc");
        }
        catch (System.Exception ex)
        {
            Debug.LogException(ex);
            EditorApplication.Exit(1);
            return;
        }
        EditorApplication.Exit(0);
    }

    static void BootstrapTables()
    {
        ArmorgrpTable.Instance.Initialize();
        ItemTable.Instance.Initialize();
        ItemTable.Instance.CacheItems();
        ModelTable.Instance.Initialize();
    }

    static void DiagLivePath(CharacterRaceAnimation race, string resourcesPath)
    {
        GameObject prefab = Resources.Load<GameObject>(resourcesPath);
        var sb = new StringBuilder();
        sb.Append("[LobbyRaceMeshDiag] LIVE ").Append(race).Append(' ').Append(resourcesPath)
          .Append(" prefab=").Append(prefab != null).Append('\n');
        if (prefab == null)
        {
            Debug.LogError(sb.ToString());
            return;
        }

        GameObject pawn = Object.Instantiate(prefab);
        pawn.SetActive(false);

        UserGear gear = pawn.GetComponent<UserGear>();
        if (gear == null)
            gear = pawn.AddComponent<UserGear>();

        gear.Initialize(-1, race);
        PlayerAppearance appearance = new PlayerAppearance
        {
            Chest = 26,
            Legs = 34,
            Gloves = 48,
            Feet = 35,
        };
        gear.SyncEquippedArmor(appearance);

        pawn.SetActive(true);
        SkinnedMeshSync sync = pawn.GetComponentInChildren<SkinnedMeshSync>(true);
        sb.Append("  sync=").Append(sync != null)
          .Append(" containerChildren=").Append(CountGearChildren(gear))
          .Append('\n');
        sync?.SyncMesh();

        foreach (SkinnedMeshRenderer r in pawn.GetComponentsInChildren<SkinnedMeshRenderer>(true))
            AppendSmr(sb, r, sync);
        Debug.Log(sb.ToString());
        Object.DestroyImmediate(pawn);
    }

    static int CountGearChildren(UserGear gear)
    {
        // Reflect container via children of model / root with armor-like names
        Transform model = gear.transform.Find("model");
        Transform root = model != null ? model : gear.transform;
        return root.childCount;
    }

    static void AppendSmr(StringBuilder sb, SkinnedMeshRenderer r, SkinnedMeshSync sync)
    {
        int bones = r.bones != null ? r.bones.Length : 0;
        int onAnim = 0;
        Transform animRoot = sync != null ? sync.transform : null;
        if (r.bones != null && animRoot != null)
        {
            for (int i = 0; i < r.bones.Length; i++)
            {
                Transform b = r.bones[i];
                if (b != null && b.IsChildOf(animRoot))
                    onAnim++;
            }
        }
        string shader = r.sharedMaterial != null && r.sharedMaterial.shader != null
            ? r.sharedMaterial.shader.name
            : "null";
        sb.Append("  ").Append(r.name)
          .Append(r.enabled ? "+" : "-")
          .Append(" bones=").Append(bones)
          .Append(" onAnim=").Append(onAnim)
          .Append(" size=").Append(r.bounds.size)
          .Append(" shader=").Append(shader)
          .Append('\n');
    }
}
