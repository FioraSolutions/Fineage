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
        ConfigureFbx(fbxPath, MeterGlobalScale, useFileScale: true);

    /// <summary>
    /// Fails fast when an FBX meta drifts off the meter import contract (regression guard).
    /// </summary>
    public static void AssertMeterImporter(string fbxPath)
    {
        if (!(AssetImporter.GetAtPath(fbxPath) is ModelImporter imp))
            throw new Exception("missing ModelImporter " + fbxPath);
        if (!Mathf.Approximately(imp.globalScale, MeterGlobalScale) || !imp.useFileScale)
            throw new Exception(
                fbxPath + " importer must be globalScale=" + MeterGlobalScale
                + " useFileScale=true (got " + imp.globalScale.ToString("0.####")
                + " useFileScale=" + imp.useFileScale + ")");
    }

    /// <summary>Pipeline B: raw UU FBX (umodel) — Unity scales; do not Blender-bake.</summary>
    public static void ConfigureUuFbx(string fbxPath) =>
        ConfigureFbx(fbxPath, UuGlobalScale);

    /// <summary>Legacy Blender FBX that still carries its centimeter root scale.</summary>
    public static void ConfigureBlenderUuFbx(string fbxPath) =>
        ConfigureFbx(fbxPath, UuGlobalScale * 0.01f);

    static void ConfigureFbx(string fbxPath, float globalScale, bool useFileScale = false)
    {
        var imp = AssetImporter.GetAtPath(fbxPath) as ModelImporter;
        if (imp == null)
            throw new Exception("missing ModelImporter " + fbxPath);

        // Meter-patched FBX have already folded the legacy factor into vertices and
        // transforms. Never let a later NPC rebuild re-apply 0.019 and shrink them 52x.
        if (imp.globalScale == 1f && imp.useFileScale)
        {
            globalScale = 1f;
            useFileScale = true;
        }

        imp.animationType = ModelImporterAnimationType.Generic;
        imp.avatarSetup = ModelImporterAvatarSetup.NoAvatar;
        imp.importAnimation = true;
        imp.materialImportMode = ModelImporterMaterialImportMode.None;
        imp.globalScale = globalScale;
        imp.useFileScale = useFileScale;
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
