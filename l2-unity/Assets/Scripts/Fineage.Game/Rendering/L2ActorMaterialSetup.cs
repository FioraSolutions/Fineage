using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

/// <summary>
/// Applies Interlude UE2 material flags (alpha test, two-sided, alpha ref) to
/// L2/World/ActorModulate2X materials for NPCs, monsters, and paperdoll pieces.
/// Shared by import batches and runtime validation.
/// </summary>
public static partial class L2ActorMaterialSetup
{
    public const string ActorShaderName = "L2/World/ActorModulate2X";
    public const float DefaultCutoff = 0.498039216f;

    public enum SurfaceMode
    {
        OpaqueBody,
        AlphaCutout,
        HiddenCollisionOnly,
    }

    public struct SetupResult
    {
        public SurfaceMode mode;
        public bool applied;
        public string detail;
        public bool propsFound;
    }

    /// <summary>
    /// Resolve cutout vs opaque from UE2 props, then material/mesh naming heuristics.
    /// </summary>
    public static SetupResult Resolve(string materialOrTextureName, IReadOnlyDictionary<string, string> propsByName)
    {
        var result = new SetupResult { mode = SurfaceMode.OpaqueBody, propsFound = false };
        if (string.IsNullOrEmpty(materialOrTextureName))
        {
            result.detail = "empty material name";
            return result;
        }

        string key = NormalizePropsKey(materialOrTextureName);
        if (propsByName != null && propsByName.Count > 0)
        {
            L2Ue2ShaderProps.PropsInfo ue2 = L2Ue2ShaderProps.Analyze(key, propsByName);
            if (!string.IsNullOrEmpty(ue2.propsPath))
                result.propsFound = true;
            if (ue2.mode == L2Ue2ShaderProps.RenderMode.HiddenCollisionOnly)
            {
                result.mode = SurfaceMode.HiddenCollisionOnly;
                result.detail = ue2.detail;
                return result;
            }

            if (ue2.alphaTest || ue2.mode == L2Ue2ShaderProps.RenderMode.MaskedCutout)
            {
                result.mode = SurfaceMode.AlphaCutout;
                result.detail = "UE2 AlphaTest props=" + ue2.propsPath;
                return result;
            }

            if (ue2.mode == L2Ue2ShaderProps.RenderMode.Opaque)
            {
                result.mode = SurfaceMode.OpaqueBody;
                result.detail = "UE2 opaque";
                return result;
            }

            if (!result.propsFound && !string.IsNullOrEmpty(ue2.detail))
                result.detail = ue2.detail;
        }

        if (NamingImpliesCutout(materialOrTextureName))
        {
            result.mode = SurfaceMode.AlphaCutout;
            result.detail = result.propsFound
                ? "naming cutout (props had no AlphaTest)"
                : "naming cutout (no props for " + key + ")";
            return result;
        }

        return result;
    }

    public static bool NamingImpliesCutout(string name)
    {
        if (string.IsNullOrEmpty(name))
            return false;
        string n = name.ToLowerInvariant();
        if (n.EndsWith("_t01") || n.EndsWith("_t02") || n.EndsWith("_t03"))
            return true;
        if (n.Contains("_t00_h") || n.Contains("_m00_ah") || n.Contains("_m00_bh"))
            return true;
        if (n.EndsWith("_ah") || n.EndsWith("_bh"))
            return true;
        if (n.Contains("_wand") || n.Contains("_wing"))
            return true;
        return false;
    }

    public static string NormalizePropsKey(string materialOrTextureName)
    {
        string n = materialOrTextureName;
        int slash = Math.Max(n.LastIndexOf('/'), n.LastIndexOf('\\'));
        if (slash >= 0 && slash < n.Length - 1)
            n = n.Substring(slash + 1);
        if (n.EndsWith(".mat", StringComparison.OrdinalIgnoreCase))
            n = n.Substring(0, n.Length - 4);
        return n;
    }

    public static bool MaterialLooksLikeActorOpaqueMismatch(Material mat)
    {
        if (mat == null || !NamingImpliesCutout(mat.name))
            return false;
        if (mat.shader == null || mat.shader.name.IndexOf("ActorModulate2X", StringComparison.Ordinal) < 0)
            return true;
        return !mat.IsKeywordEnabled("_ALPHATEST_ON") || mat.GetFloat("_AlphaClip") < 0.5f;
    }

    public static void LogOpaqueAlphaMismatch(Material mat, string meshContext)
    {
        if (mat == null)
            return;
        Texture tex = mat.HasProperty("_BaseMap") ? mat.GetTexture("_BaseMap") : null;
        string texName = tex != null ? tex.name : "(none)";
        Debug.LogError(
            "[L2ActorMaterial] alpha-tested slot on opaque setup mesh="
            + (meshContext ?? "?")
            + " material="
            + mat.name
            + " texture="
            + texName
            + " shader="
            + (mat.shader != null ? mat.shader.name : "(null)"));
    }

    /// <summary>
    /// Enumerate *.props.txt under export roots and shipped texture shader folders.
    /// </summary>
    public static Dictionary<string, string> CollectPropsIndex(IEnumerable<string> extraRoots)
    {
        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        void AddDir(string dir)
        {
            if (string.IsNullOrEmpty(dir) || !Directory.Exists(dir))
                return;
            try
            {
                foreach (string f in Directory.GetFiles(dir, "*.props.txt", SearchOption.AllDirectories))
                {
                    string leaf = Path.GetFileName(f).Replace(".props.txt", "");
                    if (!map.ContainsKey(leaf))
                        map[leaf] = f;
                }
            }
            catch
            {
                // unreadable export dir — caller may log separately
            }
        }

        if (extraRoots != null)
        {
            foreach (string root in extraRoots)
                AddDir(root);
        }

        string dataRoot = Path.Combine(Application.dataPath, "Resources/Data/Textures");
        AddDir(dataRoot);
        return map;
    }

    static int ParseAlphaRef(string propsPath, int fallback)
    {
        if (string.IsNullOrEmpty(propsPath) || !File.Exists(propsPath))
            return fallback;
        string txt = File.ReadAllText(propsPath);
        var m = Regex.Match(txt, @"^\s*AlphaRef\s*=\s*(\d+)", RegexOptions.Multiline);
        return m.Success ? int.Parse(m.Groups[1].Value) : fallback;
    }

    public static float CutoffFromProps(string materialName, IReadOnlyDictionary<string, string> propsByName)
    {
        string key = NormalizePropsKey(materialName);
        if (propsByName != null && propsByName.TryGetValue(key, out string path))
        {
            int alphaRef = ParseAlphaRef(path, 128);
            return Mathf.Clamp01(alphaRef / 255f);
        }

        return DefaultCutoff;
    }
}