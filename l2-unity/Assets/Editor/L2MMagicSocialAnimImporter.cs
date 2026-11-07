#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Human mystic (MMagic) social / extra clips from the original client, same pipeline as L2MElfAnimImporter:
/// Magic.ukx MMagic_anim.psa + MMagic_M000_b.psk -> (umodel, psk_psa_to_fbx.py under Blender 3.6) -> Assets/L2ImportTmp/MMagic_anim.fbx
/// Only sequences that MMagic/Clips does not have yet are written (Social_*, etc.); existing clips are untouched.
/// Runs once automatically when the tmp FBX is present (editor reload), or via the menu.
/// </summary>
[InitializeOnLoad]
public static class L2MMagicSocialAnimImporter
{
    const string Src = "Assets/L2ImportTmp/MMagic_anim.fbx";
    const string Root = "Assets/Resources/Data/Animations/";
    const string ClipDir = Root + "Magic/MMagic/Clips/";
    const string RefFbx = Root + "Magic/MMagic/Models/anim/MMagic_anim.fbx";

    static L2MMagicSocialAnimImporter()
    {
        EditorApplication.delayCall += () =>
        {
            if (File.Exists(Src)) Debug.Log("[MMagicSocial] " + Run());
        };
    }

    [MenuItem("Shnok/Characters/Import MMagic social animations (Magic.ukx)")]
    public static void Menu() { Debug.Log("[MMagicSocial] " + Run()); }

    static string Seq(string clipName, string prefix, string suffix)
    {
        string n = clipName;
        int i = n.LastIndexOf('|'); if (i >= 0) n = n.Substring(i + 1);
        if (n.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)) n = n.Substring(prefix.Length);
        if (n.StartsWith("ao_", StringComparison.OrdinalIgnoreCase)) n = n.Substring(3);
        if (n.EndsWith(suffix, StringComparison.OrdinalIgnoreCase)) n = n.Substring(0, n.Length - suffix.Length);
        return n.ToLowerInvariant();
    }

    public static string Run()
    {
        if (!File.Exists(Src)) return "missing " + Src;
        var refImp = (ModelImporter)UnityEditor.AssetImporter.GetAtPath(RefFbx);
        var imp = (ModelImporter)UnityEditor.AssetImporter.GetAtPath(Src);
        if (imp == null) { AssetDatabase.ImportAsset(Src); imp = (ModelImporter)UnityEditor.AssetImporter.GetAtPath(Src); }
        if (imp == null) return "importer null for " + Src;
        imp.globalScale = refImp.globalScale; imp.useFileScale = refImp.useFileScale; imp.useFileUnits = refImp.useFileUnits;
        imp.bakeAxisConversion = refImp.bakeAxisConversion;
        imp.animationType = ModelImporterAnimationType.Generic; imp.avatarSetup = ModelImporterAvatarSetup.NoAvatar;
        imp.importAnimation = true; imp.resampleCurves = true; imp.animationCompression = refImp.animationCompression;
        imp.SaveAndReimport();

        var refGo = AssetDatabase.LoadAssetAtPath<GameObject>(RefFbx);
        var map = new Dictionary<string, string>();
        foreach (var t in refGo.GetComponentsInChildren<Transform>(true))
            map[t.name.ToLowerInvariant()] = AnimationUtility.CalculateTransformPath(t, refGo.transform);

        var have = new HashSet<string>();
        foreach (var g in AssetDatabase.FindAssets("t:AnimationClip", new[] { ClipDir.TrimEnd('/') }))
            have.Add(Seq(Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(g)), "MMagic_M000_b.ao_", "_MMagic"));

        var added = new List<string>(); int unmapped = 0;
        foreach (var src in AssetDatabase.LoadAllAssetsAtPath(Src).OfType<AnimationClip>())
        {
            if (src.name.StartsWith("__preview__")) continue;
            string seq = Seq(src.name, "MMagic_M000_b.ao|", "_MMagic");
            if (have.Contains(seq)) continue;
            string raw = src.name.Substring(src.name.LastIndexOf('|') + 1);
            if (raw.StartsWith("ao_")) raw = raw.Substring(3);
            var dst = new AnimationClip { frameRate = src.frameRate };
            foreach (var b in AnimationUtility.GetCurveBindings(src))
            {
                string leaf = b.path.Length == 0 ? "MMagic_M000_b.ao" : b.path.Substring(b.path.LastIndexOf('/') + 1);
                if (!map.TryGetValue(leaf.ToLowerInvariant(), out var np)) { unmapped++; continue; }
                var nb = b; nb.path = np;
                AnimationUtility.SetEditorCurve(dst, nb, AnimationUtility.GetEditorCurve(src, b));
            }
            bool loop = seq.Contains("wait") || seq.StartsWith("swim");
            var st = AnimationUtility.GetAnimationClipSettings(dst);
            st.loopTime = loop; st.loopBlend = false;
            AnimationUtility.SetAnimationClipSettings(dst, st);
            string path = ClipDir + "MMagic_M000_b.ao_" + raw + ".anim";
            dst.name = Path.GetFileNameWithoutExtension(path);
            AssetDatabase.CreateAsset(dst, path);
            added.Add(raw);
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.DeleteAsset("Assets/L2ImportTmp");
        string result = "added " + added.Count + " (unmapped bindings " + unmapped + "): " + string.Join(",", added);
        File.WriteAllText("Logs/MMagicSocialImport.txt", result);
        return result;
    }
}
#endif
