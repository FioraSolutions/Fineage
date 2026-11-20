using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

/// <summary>Maps umodel *.props.txt UE2 shader/material chains to Unity render/collision behavior.</summary>
public static class L2Ue2ShaderProps
{
    public enum RenderMode
    {
        Unknown,
        Opaque,
        MaskedCutout,
        TranslucentWater,
        HiddenCollisionOnly,
    }

    public struct PropsInfo
    {
        public string materialName;
        public string propsPath;
        public string resolvedDiffuseTexture;
        public bool alphaTest;
        public int alphaRef;
        public bool twoSided;
        public bool zWrite;
        public bool hasOpacityMap;
        public RenderMode mode;
        public string detail;
    }

    static readonly Regex RefLine = new Regex(
        @"^\s*(Diffuse|Material|Material1|Material2|Texture|SelfIllumination|Opacity|FallbackMaterial)\s*=\s*\w+'(?:[\w]+\.)*([\w]+)'",
        RegexOptions.Multiline);

    public static PropsInfo Analyze(string materialName, IReadOnlyDictionary<string, string> propsByName, int depth = 0)
    {
        var info = new PropsInfo { materialName = materialName, mode = RenderMode.Unknown };
        if (depth > 8 || materialName == null) return info;
        if (!propsByName.TryGetValue(materialName, out var path) || !File.Exists(path))
        {
            info.detail = "missing props for material " + materialName;
            return info;
        }
        info.propsPath = path;
        string txt = File.ReadAllText(path);
        info.alphaTest = Regex.IsMatch(txt, @"^\s*AlphaTest\s*=\s*true", RegexOptions.Multiline)
            || Regex.IsMatch(txt, @"^\s*bAlphaTest\s*=\s*true", RegexOptions.Multiline);
        info.alphaRef = ParseInt(txt, @"^\s*AlphaRef\s*=\s*(\d+)", 128);
        info.twoSided = Regex.IsMatch(txt, @"^\s*(TwoSided|TreatAsTwoSided)\s*=\s*true", RegexOptions.Multiline);
        info.zWrite = !Regex.IsMatch(txt, @"^\s*ZWrite\s*=\s*false", RegexOptions.Multiline);
        info.hasOpacityMap = Regex.IsMatch(txt, @"^\s*Opacity\s*=\s*\w+'", RegexOptions.Multiline);

        string diffuse = ResolveDiffuseChain(materialName, propsByName, new HashSet<string>(StringComparer.OrdinalIgnoreCase));
        info.resolvedDiffuseTexture = diffuse;

        bool water = materialName.IndexOf("water", StringComparison.OrdinalIgnoreCase) >= 0;
        if (water && info.hasOpacityMap && !info.alphaTest)
        {
            info.mode = RenderMode.TranslucentWater;
            return info;
        }

        if (diffuse == null)
        {
            bool shaderSlot = Regex.IsMatch(txt, @"^\s*Class\s*=\s*Shader", RegexOptions.Multiline)
                || Regex.IsMatch(txt, @"^\s*Class\s*=\s*FinalBlend", RegexOptions.Multiline)
                || materialName.EndsWith("_shader", StringComparison.OrdinalIgnoreCase)
                || materialName.EndsWith("_sh", StringComparison.OrdinalIgnoreCase);
            if (shaderSlot && info.zWrite)
            {
                info.mode = RenderMode.HiddenCollisionOnly;
                info.detail = "no visible diffuse in UE2 shader chain";
                return info;
            }
            info.mode = RenderMode.Unknown;
            info.detail = "no diffuse and not classified as collision-only shader";
            return info;
        }

        if (info.alphaTest || info.hasOpacityMap)
        {
            info.mode = RenderMode.MaskedCutout;
            return info;
        }
        info.mode = RenderMode.Opaque;
        return info;
    }

    static string ResolveDiffuseChain(string name, IReadOnlyDictionary<string, string> propsByName, HashSet<string> seen)
    {
        if (name == null || !seen.Add(name)) return null;
        if (!propsByName.TryGetValue(name, out var path)) return null;
        string txt = File.ReadAllText(path);
        foreach (Match m in RefLine.Matches(txt))
        {
            string prop = m.Groups[1].Value;
            string target = m.Groups[2].Value;
            if (string.Equals(prop, "Opacity", StringComparison.OrdinalIgnoreCase))
                continue;
            if (string.Equals(prop, "FallbackMaterial", StringComparison.OrdinalIgnoreCase))
            {
                string sub = ResolveDiffuseChain(target, propsByName, seen);
                if (sub != null) return sub;
                continue;
            }
            if (propsByName.ContainsKey(target))
            {
                string sub = ResolveDiffuseChain(target, propsByName, seen);
                if (sub != null) return sub;
            }
            else
                return target;
        }
        return null;
    }

    static int ParseInt(string txt, string pattern, int fallback)
    {
        var m = Regex.Match(txt, pattern, RegexOptions.Multiline);
        return m.Success ? int.Parse(m.Groups[1].Value) : fallback;
    }
}