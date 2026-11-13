#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Town NPC FBX import contract (see l2-unity/docs/convert-race-npc/README.md).
/// Builders must emit <b>meter</b> FBX via <c>pskimport(bScaleDown=True)</c>, then
/// <see cref="ConfigureMeterFbx"/> with <c>globalScale=1</c> and a forced reimport.
/// Never YAML-edit globalScale alone — Library can keep UU verts and go giant.
/// </summary>
public static class L2TownNpcFbxImport
{
    public const float MeterGlobalScale = 1f;
    public const float MinHeight = 0.15f;
    public const float MaxHeight = 2.5f;

    public const float UuGlobalScale = 0.019f;

    public static void ConfigureMeterFbx(string fbxPath) =>
        ConfigureFbx(fbxPath, MeterGlobalScale);

    /// <summary>Pipeline B: raw UU FBX (umodel) — Unity scales; do not Blender-bake.</summary>
    public static void ConfigureUuFbx(string fbxPath) =>
        ConfigureFbx(fbxPath, UuGlobalScale);

    /// <summary>
    /// Blender 5 writes these town FBX files in UU and also leaves the 100× centimeter
    /// root that <c>useFileScale</c> would normally cancel. 0.019 alone makes a ~50 UU
    /// body tens of meters tall. 0.019×0.01 lands next to the meter elf FBX at 0.019.
    /// </summary>
    public static void ConfigureBlenderUuFbx(string fbxPath) =>
        ConfigureFbx(fbxPath, UuGlobalScale * 0.01f);

    static void ConfigureFbx(string fbxPath, float globalScale)
    {
        var imp = AssetImporter.GetAtPath(fbxPath) as ModelImporter;
        if (imp == null)
            throw new Exception("missing ModelImporter " + fbxPath);

        imp.animationType = ModelImporterAnimationType.Generic;
        imp.avatarSetup = ModelImporterAvatarSetup.NoAvatar;
        imp.importAnimation = true;
        imp.materialImportMode = ModelImporterMaterialImportMode.None;
        imp.globalScale = globalScale;
        imp.useFileScale = false;
        imp.animationCompression = ModelImporterAnimationCompression.Off;
        imp.SaveAndReimport();
    }

    /// <summary>Renderer bounds height after import.</summary>
    public static float MeasureHumanoidHeight(GameObject root)
    {
        if (root == null)
            throw new Exception("null root");

        Bounds? union = null;
        foreach (var r in root.GetComponentsInChildren<Renderer>(true))
        {
            if (r == null) continue;
            Bounds b = r.bounds;
            if (union == null) union = b;
            else
            {
                Bounds u = union.Value;
                u.Encapsulate(b.min);
                u.Encapsulate(b.max);
                union = u;
            }
        }

        if (union == null)
            throw new Exception("no renderers");
        return union.Value.size.y;
    }

    /// <summary>Renderer bounds height after import. Throws if still UU-giant or dust-sized.</summary>
    public static float AssertHumanoidHeight(GameObject root, string label)
    {
        if (root == null)
            throw new Exception(label + ": null root");

        float h;
        try { h = MeasureHumanoidHeight(root); }
        catch (Exception e) { throw new Exception(label + ": " + e.Message); }

        if (h < MinHeight || h > MaxHeight)
            throw new Exception(
                label + " height=" + h.ToString("0.####")
                + " outside [" + MinHeight + "," + MaxHeight
                + "] — FBX scale contract broken (UU verts with globalScale 1, or meter verts with 0.019).");
        return h;
    }
}
#endif
