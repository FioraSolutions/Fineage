using System;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Weapon rangesample (or a procedural stand-in) and armor emission, driven by
/// <see cref="EnchantGlow"/> placement + level tables.
/// </summary>
public sealed class ItemEnchantGlowView : MonoBehaviour
{
    public const string ChildName = "enchant_glow";
    const string ShaderName = "L2/Effects/EnchantGlow";
    const string OfficialMeshPath = "Data/Animations/LineageWeapons/rangesample";

    static Mesh _proceduralMesh;
    static Material _sharedMaterial;
    static bool _officialMeshTried;
    static Mesh _officialMesh;

    Renderer[] _armorRenderers;
    MaterialPropertyBlock _block;
    GameObject _weaponChild;
    MeshRenderer _weaponRenderer;
    bool _armorMode;

    public void ApplyWeapon(EnchantGlowPlacement placement, int enchantLevel)
    {
        _armorMode = false;
        if (!placement.Present || !EnchantGlow.TryStyle(EnchantGlow.KindWeapon, enchantLevel, out EnchantGlowStyle style))
        {
            SetWeaponVisible(false);
            return;
        }

        EnsureWeaponChild(placement);
        if (_weaponRenderer == null)
            return;

        Material mat = _weaponRenderer.material;
        ApplyStyle(mat, style);
        SetWeaponVisible(true);
    }

    public void ApplyArmor(int enchantLevel)
    {
        _armorMode = true;
        HideWeaponChild();
        if (!EnchantGlow.TryStyle(EnchantGlow.KindArmor, enchantLevel, out EnchantGlowStyle style))
        {
            ClearArmorEmission();
            return;
        }

        if (_armorRenderers == null)
            _armorRenderers = GetComponentsInChildren<Renderer>(true);

        if (_block == null)
            _block = new MaterialPropertyBlock();

        Color emission = new Color(style.R, style.G, style.B, 1f) * style.Intensity;
        for (int i = 0; i < _armorRenderers.Length; i++)
        {
            Renderer renderer = _armorRenderers[i];
            if (renderer == null || renderer.GetComponentInParent<ItemEnchantGlowView>() != this)
                continue;
            if (renderer.transform.name == ChildName)
                continue;
            renderer.GetPropertyBlock(_block);
            _block.SetColor("_EmissionColor", emission);
            renderer.SetPropertyBlock(_block);
        }
    }

    public void Clear()
    {
        SetWeaponVisible(false);
        ClearArmorEmission();
    }

    void OnDisable()
    {
        Clear();
    }

    void EnsureWeaponChild(EnchantGlowPlacement placement)
    {
        if (_weaponChild == null)
        {
            Transform existing = transform.Find(ChildName);
            _weaponChild = existing != null ? existing.gameObject : new GameObject(ChildName);
            _weaponChild.transform.SetParent(transform, false);
        }

        Mesh mesh = ResolveMesh(placement.Mesh);
        MeshFilter filter = _weaponChild.GetComponent<MeshFilter>();
        if (filter == null)
            filter = _weaponChild.AddComponent<MeshFilter>();
        filter.sharedMesh = mesh;

        _weaponRenderer = _weaponChild.GetComponent<MeshRenderer>();
        if (_weaponRenderer == null)
            _weaponRenderer = _weaponChild.AddComponent<MeshRenderer>();
        if (_weaponRenderer.sharedMaterial == null)
            _weaponRenderer.sharedMaterial = SharedMaterial();

        // Official offsets are Unreal units (cm). Scale is a unitless multiplier of the
        // rangesample (canonical length 1 m for the procedural stand-in).
        _weaponChild.transform.localPosition = new Vector3(
            placement.OffsetX * EnchantGlow.UuToMeters,
            placement.OffsetY * EnchantGlow.UuToMeters,
            placement.OffsetZ * EnchantGlow.UuToMeters);
        _weaponChild.transform.localRotation = Quaternion.identity;
        float sx = placement.ScaleX != 0f ? placement.ScaleX : 1f;
        float sy = placement.ScaleY != 0f ? placement.ScaleY : 1f;
        float sz = placement.ScaleZ != 0f ? placement.ScaleZ : 1f;
        AlignToBlade(ref sx, ref sy, ref sz);
        _weaponChild.transform.localScale = new Vector3(sx, sy, sz);
    }

    void AlignToBlade(ref float sx, ref float sy, ref float sz)
    {
        if (!TryBladeAxis(out Vector3 axis, out float length) || length < 0.02f)
            return;

        _weaponChild.transform.localRotation = Quaternion.FromToRotation(Vector3.right, axis);
        // Keep official length factor, but size the stand-in to the equipped blade.
        sx = Mathf.Max(length * (sx != 0f ? sx : 1f), 0.02f);
        float thickness = Mathf.Clamp(length * 0.08f, 0.008f, 0.06f);
        sy = thickness * (sy != 0f ? sy : 1f);
        sz = thickness * (sz != 0f ? sz : 1f);
        _weaponChild.transform.localPosition += axis * (length * 0.15f);
    }

    bool TryBladeAxis(out Vector3 axis, out float length)
    {
        axis = Vector3.right;
        length = 0f;
        Bounds bounds = new Bounds();
        bool have = false;
        Renderer[] renderers = GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < renderers.Length; i++)
        {
            Renderer renderer = renderers[i];
            if (renderer == null || renderer.transform == _weaponChild.transform ||
                renderer.transform.IsChildOf(_weaponChild.transform))
                continue;
            if (!have)
            {
                bounds = renderer.localBounds;
                have = true;
            }
            else
                bounds.Encapsulate(renderer.localBounds);
        }
        if (!have)
            return false;

        Vector3 size = bounds.size;
        length = size.x;
        axis = Vector3.right;
        if (size.y > length)
        {
            length = size.y;
            axis = Vector3.up;
        }
        if (size.z > length)
        {
            length = size.z;
            axis = Vector3.forward;
        }
        // Point away from the origin (grip) toward the far end of the blade.
        Vector3 far = bounds.center + axis * (length * 0.5f);
        Vector3 near = bounds.center - axis * (length * 0.5f);
        if (far.sqrMagnitude < near.sqrMagnitude)
            axis = -axis;
        return length > 0.001f;
    }

    void SetWeaponVisible(bool visible)
    {
        if (_weaponChild != null)
            _weaponChild.SetActive(visible);
    }

    void HideWeaponChild()
    {
        SetWeaponVisible(false);
    }

    void ClearArmorEmission()
    {
        if (_armorRenderers == null)
            return;
        if (_block == null)
            _block = new MaterialPropertyBlock();
        _block.Clear();
        for (int i = 0; i < _armorRenderers.Length; i++)
        {
            Renderer renderer = _armorRenderers[i];
            if (renderer != null)
                renderer.SetPropertyBlock(null);
        }
    }

    static void ApplyStyle(Material mat, EnchantGlowStyle style)
    {
        if (mat == null)
            return;
        Color color = new Color(style.R, style.G, style.B, Mathf.Clamp01(style.Intensity));
        if (mat.HasProperty("_BaseColor"))
            mat.SetColor("_BaseColor", color);
        if (mat.HasProperty("_Intensity"))
            mat.SetFloat("_Intensity", style.Intensity);
        if (mat.HasProperty("_Color"))
            mat.SetColor("_Color", color);
    }

    static Material SharedMaterial()
    {
        if (_sharedMaterial != null)
            return _sharedMaterial;
        Shader shader = Shader.Find(ShaderName);
        if (shader == null)
            shader = Shader.Find("Universal Render Pipeline/Unlit");
        _sharedMaterial = new Material(shader != null ? shader : Shader.Find("Hidden/InternalErrorShader"));
        _sharedMaterial.name = "EnchantGlow";
        _sharedMaterial.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
        _sharedMaterial.SetInt("_DstBlend", (int)BlendMode.One);
        return _sharedMaterial;
    }

    static Mesh ResolveMesh(string meshName)
    {
        if (!_officialMeshTried)
        {
            _officialMeshTried = true;
            GameObject prefab = Resources.Load<GameObject>(OfficialMeshPath);
            if (prefab != null)
            {
                MeshFilter filter = prefab.GetComponentInChildren<MeshFilter>();
                if (filter != null)
                    _officialMesh = filter.sharedMesh;
            }
        }
        if (_officialMesh != null)
            return _officialMesh;
        return ProceduralMesh();
    }

    static Mesh ProceduralMesh()
    {
        if (_proceduralMesh != null)
            return _proceduralMesh;

        // Unit-length diamond (octahedron) along +X, thin in YZ — stand-in for
        // LineageWeapons.rangesample until the official mesh is imported.
        const float half = 0.5f;
        const float radius = 0.06f;
        var vertices = new Vector3[]
        {
            new Vector3(-half, 0f, 0f),
            new Vector3(half, 0f, 0f),
            new Vector3(0f, radius, 0f),
            new Vector3(0f, -radius, 0f),
            new Vector3(0f, 0f, radius),
            new Vector3(0f, 0f, -radius)
        };
        var triangles = new int[]
        {
            0, 2, 4, 0, 4, 3, 0, 3, 5, 0, 5, 2,
            1, 4, 2, 1, 3, 4, 1, 5, 3, 1, 2, 5
        };
        _proceduralMesh = new Mesh { name = "rangesample_standin" };
        _proceduralMesh.vertices = vertices;
        _proceduralMesh.triangles = triangles;
        _proceduralMesh.RecalculateNormals();
        _proceduralMesh.RecalculateBounds();
        return _proceduralMesh;
    }
}
