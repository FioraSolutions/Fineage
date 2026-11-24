using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

/// <summary>
/// One rule for world brushes, static meshes, and weapons: UE2 TwoSided / TreatAsTwoSided
/// is Cull Off, anything else is Cull Back. A material with no props is not guessed.
/// </summary>
public static class L2TwoSidedMaterials
{
    public const float CullOff = 0f;
    public const float CullBack = 2f;

    static readonly string[] ResourceRoots =
    {
        "Data/Textures",
        "Data/SysTextures",
        "Data/StaticMeshes",
    };

    static Dictionary<string, string> _props;
    static Dictionary<string, bool> _agreedFlag;
    static HashSet<string> _flagClash;
    static HashSet<string> _bodyDropped;
    static readonly HashSet<string> _logged = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    public static float CullFor(bool twoSided) => twoSided ? CullOff : CullBack;

    public static IReadOnlyDictionary<string, string> Props
    {
        get
        {
            EnsureLoaded();
            return _props;
        }
    }

    public static void EnsureLoaded()
    {
        if (_props != null)
            return;
        _props = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        _agreedFlag = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
        _flagClash = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        _bodyDropped = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        for (int r = 0; r < ResourceRoots.Length; r++)
        {
            TextAsset[] assets = Resources.LoadAll<TextAsset>(ResourceRoots[r]);
            for (int i = 0; i < assets.Length; i++)
                Consider(assets[i]);
        }
    }

    static void Consider(TextAsset asset)
    {
        if (asset == null || asset.name == null || !asset.name.EndsWith(".props", StringComparison.OrdinalIgnoreCase))
            return;
        string key = asset.name.Substring(0, asset.name.Length - ".props".Length);
        if (_flagClash.Contains(key))
            return;
        bool sided = Flag(asset.text);
        if (_bodyDropped.Contains(key))
        {
            if (_agreedFlag.TryGetValue(key, out bool known) && known != sided)
            {
                Debug.LogError("[TwoSided] material " + key + " TwoSided flag disagrees across props");
                _agreedFlag.Remove(key);
                _flagClash.Add(key);
            }
            return;
        }
        if (!_props.TryGetValue(key, out string existing))
        {
            _props.Add(key, asset.text);
            _agreedFlag[key] = sided;
            return;
        }
        if (string.Equals(existing, asset.text, StringComparison.Ordinal))
            return;
        if (_agreedFlag[key] != sided)
        {
            Debug.LogError("[TwoSided] material " + key + " TwoSided flag disagrees across props");
            _props.Remove(key);
            _agreedFlag.Remove(key);
            _flagClash.Add(key);
            return;
        }
        // Same flag, different body (Shader vs FinalBlend copy). Keep the flag.
        // Drop the chain body so a second package cannot retarget the material.
        _props.Remove(key);
        _bodyDropped.Add(key);
    }

    public static bool TryGetProps(string materialName, out string body)
    {
        EnsureLoaded();
        body = null;
        string key = BaseName(materialName);
        if (string.IsNullOrEmpty(key))
            return false;
        return _props.TryGetValue(key, out body);
    }

    /// <summary>
    /// Writes <c>_Cull</c> from the original flag. Logs when TwoSided is set and the shader has no cull property.
    /// </summary>
    public static bool Apply(Material material, bool twoSided, string materialName)
    {
        if (material == null)
            return false;
        string name = string.IsNullOrEmpty(materialName) ? BaseName(material.name) : materialName;
        if (!material.HasProperty("_Cull"))
        {
            if (twoSided)
            {
                Debug.LogError("[TwoSided] material " + name + " shader \"" + ShaderName(material)
                    + "\" has no _Cull; original TwoSided cannot be honored");
            }
            return false;
        }
        material.SetFloat("_Cull", CullFor(twoSided));
        material.doubleSidedGI = twoSided;
        return true;
    }

    /// <summary>
    /// Applies the original flag when props exist. Missing props log once and leave the material unchanged.
    /// </summary>
    public static void BindSharedMaterial(Material material, bool logIfPropsMissing, string context)
    {
        if (material == null)
            return;
        string name = BaseName(material.name);
        if (string.IsNullOrEmpty(name))
            return;
        if (!TryResolve(name, out bool twoSided))
        {
            if (logIfPropsMissing && (_flagClash == null || !_flagClash.Contains(name)))
            {
                LogOnce("missing:" + name, "[TwoSided] material " + name
                    + " props missing; original TwoSided flag unknown (" + context + ")");
            }
            return;
        }
        Apply(material, twoSided, name);
    }

    /// <summary>Original TwoSided / TreatAsTwoSided, including a shader referenced by a FinalBlend.</summary>
    public static bool TryResolve(string materialName, out bool twoSided)
    {
        EnsureLoaded();
        twoSided = false;
        string key = BaseName(materialName);
        if (string.IsNullOrEmpty(key) || _flagClash.Contains(key))
            return false;
        if (_props.TryGetValue(key, out _))
        {
            L2Ue2MaterialChain.Eval eval = L2Ue2MaterialChain.Evaluate(key, _props);
            twoSided = eval.TwoSided;
            return true;
        }
        return _agreedFlag.TryGetValue(key, out twoSided);
    }

    public static void BindHierarchy(GameObject root, bool logIfPropsMissing, string context)
    {
        if (root == null)
            return;
        Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < renderers.Length; i++)
        {
            Renderer renderer = renderers[i];
            if (renderer == null)
                continue;
            Material[] mats = renderer.sharedMaterials;
            if (mats == null)
                continue;
            for (int m = 0; m < mats.Length; m++)
                BindSharedMaterial(mats[m], logIfPropsMissing, context);
        }
    }

    public static string BaseName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return name;
        const string suffix = " (Instance)";
        if (name.EndsWith(suffix, StringComparison.Ordinal))
            return name.Substring(0, name.Length - suffix.Length);
        return name;
    }

    static bool Flag(string body)
    {
        if (string.IsNullOrEmpty(body))
            return false;
        return Regex.IsMatch(body, @"^\s*(TwoSided|TreatAsTwoSided)\s*=\s*true", RegexOptions.Multiline);
    }

    static void LogOnce(string key, string message)
    {
        if (_logged.Add(key))
            Debug.LogError(message);
    }

    static string ShaderName(Material material)
    {
        if (material.shader == null)
            return "null";
        return material.shader.name;
    }
}
