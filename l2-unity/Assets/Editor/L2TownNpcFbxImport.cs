#if UNITY_EDITOR
using System;
using System.Diagnostics;
using System.IO;
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

    public static void ConfigureMeterFbx(string fbxPath) =>
        ConfigureFbx(fbxPath, MeterGlobalScale, useFileScale: true);

    /// <summary>
    /// Patch a leftover legacy x100 carrier if needed, apply the meter importer, then assert it.
    /// The only supported import path for NPC/monster FBX.
    /// </summary>
    public static void EnsureMeterContract(string fbxPath)
    {
        PatchLegacyCarrierIfNeeded(fbxPath);
        ConfigureMeterFbx(fbxPath);
        AssertMeterImporter(fbxPath);
    }

    /// <summary>Obsolete UU importer. Redirects to <see cref="EnsureMeterContract"/>.</summary>
    public static void ConfigureUuFbx(string fbxPath) => EnsureMeterContract(fbxPath);

    /// <summary>Obsolete Blender-UU importer. Redirects to <see cref="EnsureMeterContract"/>.</summary>
    public static void ConfigureBlenderUuFbx(string fbxPath) => EnsureMeterContract(fbxPath);

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


    static void PatchLegacyCarrierIfNeeded(string fbxPath)
    {
        string abs = AbsFbx(fbxPath);
        if (!File.Exists(abs))
            throw new Exception("missing FBX " + fbxPath);

        string repo = RepoRoot();
        string script = Path.Combine(repo, "tools", "interlude", "fbx_meter_patch.py");
        if (!File.Exists(script))
            throw new Exception("missing " + script);

        var start = new ProcessStartInfo
        {
            FileName = "python3",
            Arguments = "\"" + script + "\" --ensure \"" + abs + "\"",
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            WorkingDirectory = repo
        };
        using Process process = Process.Start(start);
        if (process == null)
            throw new Exception("failed to start python3 for " + script);
        string stdout = process.StandardOutput.ReadToEnd();
        string stderr = process.StandardError.ReadToEnd();
        process.WaitForExit();
        if (process.ExitCode != 0)
            throw new Exception("fbx_meter_patch --ensure failed (" + process.ExitCode + "): " + stderr + stdout);
        if (!string.IsNullOrEmpty(stdout))
            UnityEngine.Debug.Log("[MeterFbx] " + stdout.Trim());
    }

    static string AbsFbx(string fbxPath)
    {
        if (Path.IsPathRooted(fbxPath))
            return fbxPath;
        return Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), fbxPath));
    }

    static string RepoRoot()
    {
        string project = Directory.GetParent(Application.dataPath)?.FullName;
        if (!string.IsNullOrEmpty(project))
        {
            string parent = Directory.GetParent(project)?.FullName;
            if (!string.IsNullOrEmpty(parent) && File.Exists(Path.Combine(parent, "tools", "interlude", "fbx_meter_patch.py")))
                return parent;
        }

        string cwd = Directory.GetCurrentDirectory();
        if (File.Exists(Path.Combine(cwd, "tools", "interlude", "fbx_meter_patch.py")))
            return cwd;
        string up = Directory.GetParent(cwd)?.FullName;
        if (!string.IsNullOrEmpty(up) && File.Exists(Path.Combine(up, "tools", "interlude", "fbx_meter_patch.py")))
            return up;
        throw new Exception("cannot locate tools/interlude/fbx_meter_patch.py");
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
