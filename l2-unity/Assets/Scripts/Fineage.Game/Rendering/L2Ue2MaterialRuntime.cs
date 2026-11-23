using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Drives every world material that has a committed UE2 props chain.
/// Replaces the per-statue <c>L2VillageStatuePanner</c>: pan / oscillate come from the op.
/// </summary>
public sealed class L2Ue2MaterialRuntime : MonoBehaviour
{
    const string IndexRelative = "Data/Meta/Materials/ue2-texture-index.tsv";

    struct Driven
    {
        public Material Material;
        public L2Ue2MaterialChain.Eval Eval;
    }

    readonly List<string> _names = new List<string>();
    readonly List<string> _packages = new List<string>();
    readonly List<string> _paths = new List<string>();
    readonly Dictionary<string, Texture> _loaded = new Dictionary<string, Texture>(StringComparer.OrdinalIgnoreCase);
    readonly HashSet<string> _missing = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    readonly Dictionary<int, Material> _instances = new Dictionary<int, Material>();
    readonly Dictionary<string, L2Ue2MaterialChain.Eval> _evalByName =
        new Dictionary<string, L2Ue2MaterialChain.Eval>(StringComparer.OrdinalIgnoreCase);
    readonly HashSet<string> _logged = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    Dictionary<string, string> _props;
    Dictionary<string, string> _sources;
    readonly Dictionary<string, List<L2Ue2PropsIndex.Entry>> _byShort =
        new Dictionary<string, List<L2Ue2PropsIndex.Entry>>(StringComparer.OrdinalIgnoreCase);
    string _resolvingMaterial;
    Driven[] _driven = Array.Empty<Driven>();

    /// <summary>
    /// Lobby and tile loads must upgrade cutout materials even when the World
    /// copy of this component is inactive. Play mode applies the chain shader;
    /// a material rebake is not required for the shot.
    /// </summary>
    public static void Ensure()
    {
        L2Ue2MaterialRuntime[] all = FindObjectsByType<L2Ue2MaterialRuntime>(
            FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int i = 0; i < all.Length; i++)
        {
            if (all[i] != null && all[i].isActiveAndEnabled)
            {
                all[i].Rebuild();
                return;
            }
        }
        var go = new GameObject(nameof(L2Ue2MaterialRuntime));
        go.AddComponent<L2Ue2MaterialRuntime>();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        Rebuild();
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) => Rebuild();

    void Update()
    {
        if (_driven.Length == 0)
            return;
        float time = Time.time;
        for (int i = 0; i < _driven.Length; i++)
        {
            if (_driven[i].Material == null)
                continue;
            L2Ue2MaterialApply.ApplyUv(_driven[i].Material, _driven[i].Eval, time);
        }
    }

    public void Rebuild()
    {
        EnsureProps();
        EnsureIndex();
        var driven = new List<Driven>();
        var seen = new HashSet<int>();
        Renderer[] renderers = FindObjectsByType<Renderer>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        for (int i = 0; i < renderers.Length; i++)
        {
            Material[] shared = renderers[i].sharedMaterials;
            bool changed = false;
            for (int m = 0; m < shared.Length; m++)
            {
                Material source = shared[m];
                if (source == null)
                    continue;
                string name = BaseName(source.name);
                string shaderName = source.shader != null ? source.shader.name : null;
                if (!L2Ue2MaterialChain.CanUpgradeShader(shaderName))
                    continue;
                if (!TryPropsFor(source, name, out IReadOnlyDictionary<string, string> props, out string package))
                    continue;
                string cacheKey = _props.ContainsKey(name)
                    ? name
                    : name + "\n" + (CurrentTextureName(source) ?? "");
                if (!_evalByName.TryGetValue(cacheKey, out L2Ue2MaterialChain.Eval eval))
                {
                    eval = L2Ue2MaterialChain.Evaluate(name, props, package, _sources);
                    _evalByName[cacheKey] = eval;
                }
                if (!L2Ue2MaterialChain.NeedsChainShader(eval))
                    continue;
                if (!L2Ue2MaterialApply.TexturePackageAgrees(
                        CurrentTextureName(source),
                        eval.DiffuseTexture,
                        _names,
                        _packages,
                        out string packageDetail))
                {
                    if (_logged.Add(name))
                        Debug.LogError("[Ue2Material] material " + name + " " + packageDetail);
                    continue;
                }
                int sourceId = source.GetInstanceID();
                bool fresh = !_instances.TryGetValue(sourceId, out Material existing) || existing == null;
                Material inst = Instance(source);
                if (fresh)
                {
                    _resolvingMaterial = name;
                    L2Ue2MaterialApply.Apply(inst, eval, Time.time, ResolveTexture);
                }
                shared[m] = inst;
                changed = true;
                if (seen.Add(inst.GetInstanceID()))
                    driven.Add(new Driven { Material = inst, Eval = eval });
            }
            if (changed)
                renderers[i].sharedMaterials = shared;
        }
        _driven = driven.ToArray();
    }

    void EnsureProps()
    {
        if (_props != null)
            return;
        _props = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        _sources = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        List<L2Ue2PropsIndex.Entry> files = L2Ue2PropsIndex.Load(L2Ue2PropsIndex.TexturesRoot());
        if (files.Count == 0)
        {
            LoadPropsWithoutPaths();
            return;
        }
        var conflicts = new List<string>();
        L2Ue2PropsIndex.AddKeys(files, _props, _sources, conflicts);
        for (int i = 0; i < conflicts.Count; i++)
            Debug.LogError("[Ue2Material] props key rejected " + conflicts[i]);
        for (int i = 0; i < files.Count; i++)
        {
            L2Ue2PropsIndex.Entry file = files[i];
            if (!_byShort.TryGetValue(file.Name, out List<L2Ue2PropsIndex.Entry> list))
            {
                list = new List<L2Ue2PropsIndex.Entry>();
                _byShort[file.Name] = list;
            }
            list.Add(file);
        }
    }

    bool TryPropsFor(Material source, string name, out IReadOnlyDictionary<string, string> props, out string package)
    {
        props = _props;
        package = null;
        if (_props != null && _props.ContainsKey(name))
        {
            if (_sources != null && _sources.TryGetValue(name, out string path))
                package = PackageOf(path);
            return true;
        }
        if (_byShort == null || !_byShort.TryGetValue(name, out List<L2Ue2PropsIndex.Entry> candidates))
            return false;
        L2Ue2PropsIndex.Entry chosen = L2Ue2PropsIndex.Choose(
            candidates, CurrentTextureName(source), _names, _packages, out string reason);
        if (chosen == null)
        {
            if (_logged.Add("props:" + name))
                Debug.LogError(
                    "[Ue2Material] material " + name + " props ambiguous "
                    + L2Ue2PropsIndex.DescribePaths(candidates));
            return false;
        }
        if (_logged.Add("props:" + name + "\n" + chosen.RelativePath))
            Debug.Log("[Ue2Material] props " + name + " -> " + chosen.RelativePath + " (" + reason + ")");
        var overlay = new Dictionary<string, string>(_props, StringComparer.OrdinalIgnoreCase);
        overlay[name] = chosen.Text;
        props = overlay;
        package = chosen.Package;
        return true;
    }

    static string PackageOf(string relativePath)
    {
        if (string.IsNullOrEmpty(relativePath))
            return null;
        int slash = relativePath.IndexOf('/');
        return slash > 0 ? relativePath.Substring(0, slash) : null;
    }

    void LoadPropsWithoutPaths()
    {
        var rejected = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        TextAsset[] assets = Resources.LoadAll<TextAsset>("Data/Textures");
        for (int i = 0; i < assets.Length; i++)
        {
            TextAsset asset = assets[i];
            if (asset == null || asset.name == null || !asset.name.EndsWith(".props", StringComparison.OrdinalIgnoreCase))
                continue;
            string key = asset.name.Substring(0, asset.name.Length - ".props".Length);
            if (rejected.Contains(key))
                continue;
            if (_props.TryGetValue(key, out string existing))
            {
                if (!string.Equals(existing, asset.text, StringComparison.OrdinalIgnoreCase))
                {
                    Debug.LogError(
                        "[Ue2Material] material " + key + " props ambiguous "
                        + DiffuseLine(existing) + " vs " + DiffuseLine(asset.text));
                    _props.Remove(key);
                    rejected.Add(key);
                }
                continue;
            }
            _props.Add(key, asset.text);
        }
    }

    void EnsureIndex()
    {
        if (_names.Count > 0)
            return;
        string path = Path.Combine(Application.streamingAssetsPath, IndexRelative);
        L2Ue2MaterialApply.LoadIndex(path, _names, _packages, _paths);
    }

    Texture ResolveTexture(string reference)
    {
        if (string.IsNullOrEmpty(reference))
            return null;
        if (_loaded.TryGetValue(reference, out Texture cached))
            return cached;
        if (!_missing.Add(reference))
            return null;
        var errors = new List<string>();
        string resourcePath = L2Ue2MaterialApply.PickTexturePath(
            _names, _packages, _paths, reference, errors, _resolvingMaterial);
        if (resourcePath == null)
        {
            if (errors.Count == 0)
                Debug.LogError("[Ue2Material] material " + _resolvingMaterial + " texture " + reference + " missing");
            for (int i = 0; i < errors.Count; i++)
                Debug.LogError(errors[i]);
            return null;
        }
        Texture tex = Resources.Load<Texture>(resourcePath);
        if (tex == null)
        {
            Debug.LogError("[Ue2Material] material " + _resolvingMaterial + " texture " + reference + " missing");
            return null;
        }
        _missing.Remove(reference);
        _loaded[reference] = tex;
        return tex;
    }

    Material Instance(Material source)
    {
        int id = source.GetInstanceID();
        if (_instances.TryGetValue(id, out Material existing) && existing != null)
            return existing;
        var inst = new Material(source) { name = BaseName(source.name) };
        _instances[id] = inst;
        return inst;
    }

    static string CurrentTextureName(Material source)
    {
        if (source == null)
            return null;
        Texture tex = source.mainTexture;
        if (tex == null && source.HasProperty("_MainTex"))
            tex = source.GetTexture("_MainTex");
        if (tex == null && source.HasProperty("_BaseMap"))
            tex = source.GetTexture("_BaseMap");
        return tex != null ? tex.name : null;
    }

    static string DiffuseLine(string body)
    {
        if (string.IsNullOrEmpty(body))
            return "Diffuse missing";
        int i = body.IndexOf("Diffuse", StringComparison.OrdinalIgnoreCase);
        if (i < 0)
            return "Diffuse missing";
        int end = body.IndexOf('\n', i);
        string line = end > i ? body.Substring(i, end - i) : body.Substring(i);
        return line.Trim();
    }

    static string BaseName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return name;
        const string suffix = " (Instance)";
        if (name.EndsWith(suffix, StringComparison.Ordinal))
            return name.Substring(0, name.Length - suffix.Length);
        return name;
    }
}
