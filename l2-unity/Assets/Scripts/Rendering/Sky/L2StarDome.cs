using System;
using UnityEngine;

/// <summary>
/// L2 StarField two-pass dome (RenderDoc high_elf_moon EID 1408 + 1414).
/// Sky-cap mesh L2sky_sp04 (121 verts). Same lid family as simple clouds 1–2.
/// Layer L2Sky so UNORM blit + fxGain 1.4 apply like sun/moon.
/// Night black sky is the scene copy and is not boosted.
/// Visible with the moon (sunScale ~ 0).
/// </summary>
[ExecuteInEditMode]
[DisallowMultipleComponent]
public partial class L2StarDome : MonoBehaviour
{
    const string SmallTexPath = "Data/Clock/StarField_smallStar01";
    const string LargeTexPath = "Data/Clock/StarField_largeStar02";
    const string ShaderName = "L2/Sky/StarField";
    const string MeshObjectName = "L2StarDomeMesh";

    static readonly int StarColorId = Shader.PropertyToID("_StarColor");
    static readonly int SkyDistanceId = Shader.PropertyToID("_SkyDistance");

    // high_elf_moon.rdc D3D9 textureFactor (consts.f[0]) at night.
    static readonly Color SmallNightTint = new Color(90f / 255f, 90f / 255f, 90f / 255f, 1f);
    static readonly Color LargeNightTint = new Color(200f / 255f, 200f / 255f, 200f / 255f, 1f);

    [Obsolete("Use [Inject] / PacketServices instead")]
    public static L2StarDome Instance { get; private set; }

    [SerializeField] Texture2D _smallStars;
    [SerializeField] Texture2D _largeStars;
    [SerializeField] bool _followCamera = true;

    Transform _tf;
    MeshFilter _mf;
    MeshRenderer _mr;
    Mesh _mesh;
    Material _smallMat;
    Material _largeMat;
    Camera _cam;

    public static L2StarDome EnsureOn(GameObject host)
    {
        if (Instance != null)
        {
            return Instance;
        }

        L2StarDome dome = host != null ? host.GetComponent<L2StarDome>() : null;
        if (dome == null && host != null)
        {
            dome = host.AddComponent<L2StarDome>();
        }

        return dome;
    }

    void OnEnable()
    {
        if (Instance != null && Instance != this)
        {
            enabled = false;
            return;
        }

        Instance = this;
        EnsureDome();
    }

    void OnDisable()
    {
        if (Instance == this)
        {
            Instance = null;
        }

        DestroyDome();
    }

    void LateUpdate()
    {
        if (Instance != this)
        {
            return;
        }

        EnsureDome();
        if (_tf == null || _smallMat == null || _largeMat == null)
        {
            return;
        }

        ApplySkyLayer(_tf.gameObject);

        float fade = NightFade();
        bool draw = fade > 0.01f;
        if (_mr != null)
        {
            _mr.enabled = draw;
        }

        _tf.gameObject.SetActive(draw);
        if (!draw)
        {
            return;
        }

        ApplyTint(_smallMat, SmallNightTint, fade);
        ApplyTint(_largeMat, LargeNightTint, fade);

        Camera cam = ResolveCamera();
        _tf.position = _followCamera && cam != null ? cam.transform.position : Vector3.zero;
        _tf.rotation = Quaternion.identity;
        _tf.localScale = Vector3.one;
        float skyZ = ResolveSkyDistance(cam);
        _smallMat.SetFloat(SkyDistanceId, skyZ);
        _largeMat.SetFloat(SkyDistanceId, skyZ);
    }

    static float NightFade()
    {
        L2CelestialLut.EnsureLoaded();
        if (!L2CelestialLut.IsReady)
        {
            WorldClock clock = WorldClockPacketServices.Clock;
            if (clock == null)
            {
                return 1f;
            }

            return clock.Clock.darkRatio > 0.01f ? 1f : 0f;
        }

        WorldClock hoursClock = WorldClockPacketServices.Clock;
        if (hoursClock == null)
        {
            hoursClock = WorldClock.EnsurePersistent();
        }

        float hours = hoursClock != null ? hoursClock.WorldHours : 0f;
        var sample = L2CelestialLut.SampleAt(hours);
        // Same night window as the moon disc. sunScale is 0 at night in the LUT.
        if (sample.sunScale > 0.02f)
        {
            return 0f;
        }

        return Mathf.Clamp01(sample.moonScale / 4.5f);
    }

    static void ApplyTint(Material mat, Color nightTint, float fade)
    {
        if (mat == null)
        {
            return;
        }

        mat.SetVector(StarColorId, new Vector4(
            nightTint.r * fade,
            nightTint.g * fade,
            nightTint.b * fade,
            nightTint.a));
    }

    void EnsureDome()
    {
        if (_smallStars == null)
        {
            _smallStars = Resources.Load<Texture2D>(SmallTexPath);
        }

        if (_largeStars == null)
        {
            _largeStars = Resources.Load<Texture2D>(LargeTexPath);
        }

        Shader shader = Shader.Find(ShaderName);
        if (shader == null || _smallStars == null || _largeStars == null)
        {
            return;
        }

        ConfigureTex(_smallStars);
        ConfigureTex(_largeStars);

        if (_mesh == null || _mesh.name != L2StarDomeMesh.MeshName)
        {
            if (_mesh != null)
            {
                if (Application.isPlaying)
                    Destroy(_mesh);
                else
                    DestroyImmediate(_mesh);
            }

            _mesh = L2StarDomeMesh.Build();
            _mesh.hideFlags = HideFlags.DontSave;
            if (_mf != null)
            {
                _mf.sharedMesh = _mesh;
            }
        }

        if (_tf != null)
        {
            return;
        }

        var go = new GameObject(MeshObjectName);
        go.hideFlags = HideFlags.HideAndDontSave;
        _tf = go.transform;
        _tf.SetParent(transform, false);
        ApplySkyLayer(go);

        _mf = go.AddComponent<MeshFilter>();
        _mf.sharedMesh = _mesh;

        _mr = go.AddComponent<MeshRenderer>();
        _mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        _mr.receiveShadows = false;
        _mr.allowOcclusionWhenDynamic = false;
        _mr.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
        _mr.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;

        _smallMat = MakeMat(shader, _smallStars, SmallNightTint, 2990);
        _largeMat = MakeMat(shader, _largeStars, LargeNightTint, 2991);
        _mr.sharedMaterials = new[] { _smallMat, _largeMat };
    }

}
