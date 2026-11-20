using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Interlude overhead quest arrow: a world-space quad above the local player's
/// head, yaw-locked to the QuestDirection target on the ground plane.
/// Texture is LineageDecosTex <c>float_t00</c> (two-frame sheet).
/// </summary>
public sealed class QuestOverheadDirectionArrow : MonoBehaviour
{
    const string TexturePath = "Data/SysTextures/LineageDecos/float_t00";
    const string ShaderName = "Universal Render Pipeline/Unlit";
    const float FrameSec = 0.35f;
    const float SizeMeters = 1.0f;
    // Sit above the nameplate anchor (already includes NameOffsetMeters).
    const float AboveNameMeters = 0.7f;

    GameObject _arrow;
    MeshRenderer _renderer;
    Material _material;
    Texture2D _tex;
    bool _loadAttempted;

    void OnDestroy()
    {
        if (_arrow != null)
            Destroy(_arrow);
        if (_material != null)
            Destroy(_material);
    }

    void LateUpdate()
    {
        if (!QuestDirectionSight.TryActive(out _, out Vector3 world, out _, out _))
        {
            SetVisible(false);
            return;
        }

        PlayerEntity player = DomainPacketServices.LocalPlayer;
        if (player == null)
        {
            SetVisible(false);
            return;
        }

        if (!EnsureArrow())
        {
            SetVisible(false);
            return;
        }

        Vector3 head = L2OverheadAnchor.GetWorldPos(player.transform);
        Vector3 pos = head + Vector3.up * AboveNameMeters;
        Vector3 to = world - pos;
        to.y = 0f;
        if (to.sqrMagnitude < 0.0001f)
        {
            SetVisible(false);
            return;
        }

        to.Normalize();
        _arrow.transform.position = pos;
        _arrow.transform.rotation = Quaternion.LookRotation(Vector3.Cross(Vector3.up, to), to);
        ApplyFrame();
        SetVisible(true);
    }

    bool EnsureArrow()
    {
        if (_arrow != null)
            return true;
        if (_loadAttempted)
            return false;
        _loadAttempted = true;

        _tex = Resources.Load<Texture2D>(TexturePath);
        if (_tex == null)
        {
            Debug.LogError($"[QuestDirection] missing overhead arrow texture path={TexturePath}. Import LineageDecosTex float_t00 from Interlude client.");
            return false;
        }

        Shader shader = Shader.Find(ShaderName);
        if (shader == null)
        {
            Debug.LogError($"[QuestDirection] missing shader name={ShaderName} for overhead arrow.");
            return false;
        }

        _material = new Material(shader);
        _material.SetFloat("_Surface", 1f);
        _material.SetFloat("_Blend", 0f);
        _material.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha);
        _material.SetFloat("_DstBlend", (float)BlendMode.OneMinusSrcAlpha);
        _material.SetFloat("_ZWrite", 0f);
        _material.SetFloat("_Cull", (float)CullMode.Off);
        _material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        _material.DisableKeyword("_ALPHATEST_ON");
        _material.EnableKeyword("_ALPHABLEND_ON");
        _material.renderQueue = 3000;
        _material.color = Color.white;
        _material.SetColor("_BaseColor", Color.white);
        _material.mainTexture = _tex;
        _material.SetTexture("_BaseMap", _tex);
        // Two-frame 128x256 sheet: show the top (rest) frame first.
        _material.mainTextureScale = new Vector2(1f, 0.5f);
        _material.SetTextureScale("_BaseMap", new Vector2(1f, 0.5f));

        _arrow = GameObject.CreatePrimitive(PrimitiveType.Quad);
        _arrow.name = "QuestOverheadDirectionArrow";
        Collider col = _arrow.GetComponent<Collider>();
        if (col != null)
            Destroy(col);
        _arrow.transform.SetParent(null, true);
        _arrow.transform.localScale = new Vector3(SizeMeters, SizeMeters, SizeMeters);
        _renderer = _arrow.GetComponent<MeshRenderer>();
        _renderer.sharedMaterial = _material;
        _renderer.shadowCastingMode = ShadowCastingMode.Off;
        _renderer.receiveShadows = false;
        return true;
    }

    void ApplyFrame()
    {
        if (_material == null)
            return;
        int frame = Mathf.FloorToInt(Time.unscaledTime / FrameSec) & 1;
        // Unity UV y=0 is the bottom of the texture; top (rest) frame is offset 0.5.
        var offset = new Vector2(0f, frame == 0 ? 0.5f : 0f);
        _material.mainTextureOffset = offset;
        _material.SetTextureOffset("_BaseMap", offset);
    }

    void SetVisible(bool on)
    {
        if (_arrow != null && _arrow.activeSelf != on)
            _arrow.SetActive(on);
    }
}
