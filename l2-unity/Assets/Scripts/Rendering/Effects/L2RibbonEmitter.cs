using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// CPU port of URibbonEmitter trail buffer → triangle strip.
/// Prefers Sword_Tip / Sword_Base as a2/a3 (blade cross-section), else GetNew CS1.
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public sealed partial class L2RibbonEmitter : EffectPart
{
    public enum WidthAxisSource
    {
        TransformRight = 0,
        TransformUp = 1,
        TransformForward = 2,
        CustomLocal = 3,
    }

    public enum SampleMode
    {
        AutoBladeEnds = 0,
        BladeEndsOnly = 1,
        BoneNormalScaleRatio = 2,
    }

    [Header("RibbonSet / GetNewRibbonPoint")]
    [SerializeField] private SampleMode _sampleMode = SampleMode.AutoBladeEnds;
    [Tooltip("UC ScaleRatio — used only for BoneNormalScaleRatio fallback.")]
    [SerializeField] private float _scaleRatio = 1.5f;
    [SerializeField] private float _l2ToUnityScale = 0.2f;
    [SerializeField, Range(0f, 1f)] private float _edgeRatio = 0.5f;
    [SerializeField] private float _sampleRate = 0.002f;
    [SerializeField, Min(2)] private int _maxPoints = 160;
    [SerializeField] private int _minPoints = 20;
    [Tooltip("Max midpoint/tip gap between ribbon samples (m). Larger frame motion inserts arc sheets.")]
    [SerializeField] private float _maxSegmentMeters = 0.02f;
    [SerializeField, Min(0)] private int _maxSheetsPerSample = 48;
    [Tooltip("Slerp tip/base around weapon pivot instead of linear chords (needed for curved swings).")]
    [SerializeField] private bool _arcSheetInterpolation = true;
    [SerializeField] private WidthAxisSource _widthAxisSource = WidthAxisSource.TransformRight;
    [SerializeField] private Vector3 _customWidthAxisLocal = Vector3.right;
    [SerializeField] private Transform _sampleBone;
    [SerializeField] private string _autoBoneName = "Weapon_R_Bone";
    [SerializeField] private Transform _swordTip;
    [SerializeField] private Transform _swordBase;
    [SerializeField] private string _swordTipName = "Sword_Tip";
    [SerializeField] private string _swordBaseName = "Sword_Base";
    [Tooltip("If true, always use MeshFilter blade ends when available (never alternate with Sword_Tip/Base).")]
    [SerializeField] private bool _preferMeshBladeEnds = true;
    [Tooltip("If true, keep a2/a3 from flipping when tip/base sources disagree.")]
    [SerializeField] private bool _stabilizeEdgePolarity = true;

    [Header("Trail")]
    [Tooltip("False = store world edges (trail stays in world while effect root moves).")]
    [SerializeField] private bool _storeInOwnerLocal = false;
    [SerializeField] private float _minSampleDistance = 0.0001f;
    [SerializeField] private bool _fadeAlphaAlongTrail = true;
    [SerializeField, Range(0f, 1f)] private float _headAlpha = 1f;
    [SerializeField, Range(0f, 1f)] private float _tailAlpha = 0.15f;
    [SerializeField, Range(0f, 2f)] private float _opacity = 1f;
    [Tooltip("Always keep a starter strip so something is visible before the sword swings.")]
    [SerializeField] private bool _seedStarterStrip = true;
    [SerializeField] private float _starterStripMeters = 0.05f;

    [Header("Debug")]
    [SerializeField] private bool _debugLog = false;
    [Tooltip("Per-frame sword tip/base motion + ribbon head (compare to L2 RibbonSnapshot).")]
    [SerializeField] private bool _debugTraceMotion = false;
    [Tooltip("Dump first/last ribbon points + segment lengths when buffer changes.")]
    [SerializeField] private bool _debugTracePoints = false;
    [SerializeField] private float _debugLogInterval = 0.1f;
    [SerializeField] private int _debugTraceFirstFrames = 90;
    [SerializeField] private bool _drawGizmos = false;

    private struct RibbonPoint
    {
        public Vector3 A2;
        public Vector3 A3;
        public float A4;
        public float Time;
    }

    private readonly List<RibbonPoint> _points = new List<RibbonPoint>(96);
    private MeshFilter _meshFilter;
    private Mesh _mesh;
    private Vector3[] _verts;
    private Vector2[] _uvs;
    private Color[] _colors;
    private int[] _tris;
    private bool _playing;
    private float _sampleTimer;
    private float _nextDebugLogTime;
    private int _sampleAttempts;
    private int _sampleAccepted;
    private string _lastSampleSource = "?";
    private bool _loggedPlayOnce;
    private Transform _weaponRoot;
    private int _traceFrame;
    private bool _havePrevBlade;
    private Vector3 _prevTipWorld;
    private Vector3 _prevBaseWorld;
    private Vector3 _prevMidWorld;
    private float _playStartedAt;
    private int _lastLoggedPointCount = -1;
    private string _lastBladeResolveSource = "?";
    private Vector3 _lastTipWorld;
    private Vector3 _lastBaseWorld;

    public override void Initialize(EffectSettings settings, float baseSize)
    {
        base.Initialize(settings, baseSize);
        EnsureMesh();
    }

    public override void Setup(EffectSettings settings, MagicCastData castData)
    {
        _settings = settings;
        _castData = castData;
        EnsureMesh();
        if (propBlock == null)
        {
            propBlock = new MaterialPropertyBlock();
        }

        Log(
            $"Setup owner='{(OwnerTarget != null ? OwnerTarget.name : "null")}' " +
            $"follow='{(FollowTarget != null ? FollowTarget.name : "null")}'");
    }

    public override void PlayPart()
    {
        EnsureMesh();
        if (propBlock == null)
        {
            propBlock = new MaterialPropertyBlock();
        }

        ResolveBladeAnchors();
        ApplyOpacityToMaterial();
        _points.Clear();
        _sampleTimer = 0f;
        _sampleAttempts = 0;
        _sampleAccepted = 0;
        _playing = true;
        _loggedPlayOnce = false;
        _nextDebugLogTime = 0f;
        _traceFrame = 0;
        _havePrevBlade = false;
        _playStartedAt = Time.time;
        _lastLoggedPointCount = -1;

        if (_meshFilter != null)
        {
            _meshFilter.sharedMesh = _mesh;
        }

        if (targetRenderer != null)
        {
            targetRenderer.enabled = true;
        }

        bool ok = TrySample(force: true);
        if (ok && _seedStarterStrip && _points.Count == 1)
        {
            SeedStarterStrip();
        }

        RebuildMesh();

        Shader shader = targetRenderer != null && targetRenderer.sharedMaterial != null
            ? targetRenderer.sharedMaterial.shader
            : null;
        Log(
            $"PlayPart okSample={ok} tip='{(_swordTip != null ? _swordTip.name : "null")}' " +
            $"base='{(_swordBase != null ? _swordBase.name : "null")}' " +
            $"bone='{(_sampleBone != null ? _sampleBone.name : "null")}' " +
            $"source={_lastSampleSource} points={_points.Count} " +
            $"owner='{(OwnerTarget != null ? OwnerTarget.name : "null")}' " +
            $"renderer={(targetRenderer != null ? targetRenderer.enabled.ToString() : "null")} " +
            $"mat={(targetRenderer != null && targetRenderer.sharedMaterial != null ? targetRenderer.sharedMaterial.name : "null")} " +
            $"shader={(shader != null ? shader.name : "null")} supported={(shader != null && shader.isSupported)} " +
            $"pos={transform.position}");
        _loggedPlayOnce = true;
    }

    private void SeedStarterStrip()
    {
        RibbonPoint head = _points[0];
        Vector3 blade = head.A2 - head.A3;
        Vector3 side = Vector3.Cross(blade.normalized, Vector3.up);
        if (side.sqrMagnitude < 1e-8f)
        {
            side = Vector3.Cross(blade.normalized, Vector3.right);
        }

        side = side.normalized * Mathf.Max(0.01f, _starterStripMeters);
        _points.Add(new RibbonPoint
        {
            A2 = head.A2 - side,
            A3 = head.A3 - side,
            A4 = head.A4,
            Time = head.Time - 0.001f,
        });
        Log($"SeedStarterStrip side={side.magnitude:F3} points={_points.Count} a4={head.A4:F3}");
    }

    public override void StopPart()
    {
        // Keep sampling until destroy — BeginFadeOut would otherwise freeze/clear the trail mid-swing.
    }

    private void Awake()
    {
        EnsureMesh();
        if (targetRenderer == null)
        {
            targetRenderer = GetComponent<Renderer>();
        }
    }

    private void OnDisable()
    {
        _playing = false;
    }

}
