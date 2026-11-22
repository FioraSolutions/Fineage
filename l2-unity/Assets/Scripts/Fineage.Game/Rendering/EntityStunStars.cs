using UnityEngine;

/// <summary>
/// Interlude AVE_STUN: three yellow stars orbit the victim's head.
/// Retail art is <c>LineageEffectsStaticmeshes.starz</c> (three quads, additive) with
/// <c>Particles3.fx_m_t7018</c> / <c>fx_m_t7019</c>; the client draws it over the head bone while the
/// STUN abnormal is live. The static mesh has no local import, so the same retail texture is drawn on
/// three camera-facing quads on the retail ring instead of inventing a sprite.
/// </summary>
public sealed class EntityStunStars : MonoBehaviour
{
    const string StarTexturePath = "Data/Textures/LineageEffectsTextures/fx_m_t7018";
    const int StarCount = 3;
    const float OrbitRadius = 0.23f;
    const float OrbitHeight = 0.26f;
    const float OrbitDegreesPerSecond = 200f;
    const float StarSize = 0.26f;
    const float BobAmplitude = 0.02f;
    const float BobDegreesPerSecond = 320f;

    static Material _sharedMaterial;
    static Texture2D _sharedTexture;

    readonly Transform[] _stars = new Transform[StarCount];
    float _angle;
    float _bob;

    public static EntityStunStars Attach(Transform parent, Vector3 localOffset)
    {
        if (parent == null)
            return null;

        GameObject holder = new GameObject("AVE_Stun");
        holder.transform.SetParent(parent, false);
        holder.transform.localPosition = localOffset;
        holder.transform.localRotation = Quaternion.identity;
        holder.transform.localScale = Vector3.one;
        L2FxCompositorLayers.ApplySkillEffectLayer(holder);

        EntityStunStars stars = holder.AddComponent<EntityStunStars>();
        stars.Build();
        return stars;
    }

    void Build()
    {
        Material material = ResolveMaterial();
        if (material == null)
            return;

        for (int i = 0; i < StarCount; i++)
        {
            GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            quad.name = "star" + i;
            Collider collider = quad.GetComponent<Collider>();
            if (collider != null)
                Destroy(collider);
            quad.transform.SetParent(transform, false);
            quad.transform.localScale = Vector3.one * StarSize;

            MeshRenderer renderer = quad.GetComponent<MeshRenderer>();
            renderer.sharedMaterial = material;
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.receiveShadows = false;

            _stars[i] = quad.transform;
        }

        Layout();
    }

    static Material ResolveMaterial()
    {
        if (_sharedMaterial != null)
            return _sharedMaterial;

        if (_sharedTexture == null)
            _sharedTexture = Resources.Load<Texture2D>(StarTexturePath);
        if (_sharedTexture == null)
            return null;

        // URP Particles/Unlit: _Blend 1 is additive, which is how the retail star mesh is drawn
        // (24-bit star texture with a black key — no per-pixel alpha needed).
        Shader shader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
        if (shader == null)
            shader = Shader.Find("Universal Render Pipeline/Unlit");
        if (shader == null)
            return null;

        Material material = new Material(shader);
        material.name = "AVE_StunStar";
        material.mainTexture = _sharedTexture;
        if (material.HasProperty("_BaseMap"))
            material.SetTexture("_BaseMap", _sharedTexture);
        if (material.HasProperty("_BaseColor"))
            material.SetColor("_BaseColor", Color.white);
        if (material.HasProperty("_Surface"))
            material.SetFloat("_Surface", 1f);
        if (material.HasProperty("_Blend"))
            material.SetFloat("_Blend", 1f);
        if (material.HasProperty("_ZWrite"))
            material.SetFloat("_ZWrite", 0f);
        if (material.HasProperty("_Cull"))
            material.SetFloat("_Cull", 0f);
        material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

        _sharedMaterial = material;
        return _sharedMaterial;
    }

    void Update()
    {
        if (_stars[0] == null)
            return;
        float dt = Time.deltaTime;
        _angle += OrbitDegreesPerSecond * dt;
        if (_angle >= 360f)
            _angle -= 360f;
        _bob += BobDegreesPerSecond * dt;
        if (_bob >= 360f)
            _bob -= 360f;
        Layout();
    }

    void Layout()
    {
        for (int i = 0; i < _stars.Length; i++)
        {
            Transform star = _stars[i];
            if (star == null)
                continue;
            float degrees = _angle + (360f / StarCount) * i;
            float radians = degrees * Mathf.Deg2Rad;
            float y = OrbitHeight + Mathf.Sin((_bob + degrees) * Mathf.Deg2Rad) * BobAmplitude;
            star.localPosition = new Vector3(
                Mathf.Cos(radians) * OrbitRadius,
                y,
                Mathf.Sin(radians) * OrbitRadius);

            Camera camera = Camera.main;
            if (camera != null)
                star.rotation = camera.transform.rotation;
        }
    }

    void OnDestroy()
    {
        // The shared material/texture stay resident across pawns; only the quads are destroyed.
    }
}
