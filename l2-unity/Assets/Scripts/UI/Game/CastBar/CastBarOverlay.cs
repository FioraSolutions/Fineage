using UnityEngine;

/// <summary>
/// Local player's casting gauge, driven by MagicSkillUse.HitTime (Mobius sends hitTime in ms;
/// damage lands when it elapses). Uses the Interlude DF MP gauge textures already shipped in
/// Resources/Data/UI/Assets/Status/Gauge. Hidden for instant skills (hitTime &lt; 200 ms).
/// </summary>
public sealed class CastBarOverlay : MonoBehaviour
{
    const string GaugeRoot = "Data/UI/Assets/Status/Gauge/Gauge_DF_Large_MP/";
    const float Width = 220f;
    const float Height = 12f;

    static CastBarOverlay _instance;

    Texture2D _bgLeft, _bgCenter, _bgRight, _fillLeft, _fillCenter, _fillRight;
    float _startTime;
    float _duration;
    bool _active;

    public static void Begin(int hitTimeMs)
    {
        if (hitTimeMs < 200)
            return;
        if (_instance == null)
        {
            var go = new GameObject("CastBarOverlay");
            DontDestroyOnLoad(go);
            _instance = go.AddComponent<CastBarOverlay>();
        }
        _instance._startTime = Time.unscaledTime;
        _instance._duration = hitTimeMs / 1000f;
        _instance._active = true;
    }

    public static void Cancel()
    {
        if (_instance != null)
            _instance._active = false;
    }

    void Awake()
    {
        _bgLeft = Resources.Load<Texture2D>(GaugeRoot + "Gauge_DF_Large_MP_bg_Left");
        _bgCenter = Resources.Load<Texture2D>(GaugeRoot + "Gauge_DF_Large_MP_bg_Center");
        _bgRight = Resources.Load<Texture2D>(GaugeRoot + "Gauge_DF_Large_MP_bg_Right");
        _fillLeft = Resources.Load<Texture2D>(GaugeRoot + "Gauge_DF_Large_MP_Left");
        _fillCenter = Resources.Load<Texture2D>(GaugeRoot + "Gauge_DF_Large_MP_Center");
        _fillRight = Resources.Load<Texture2D>(GaugeRoot + "Gauge_DF_Large_MP_Right");
    }

    void OnGUI()
    {
        if (!_active)
            return;
        float t = _duration <= 0f ? 1f : (Time.unscaledTime - _startTime) / _duration;
        if (t >= 1f)
        {
            _active = false;
            return;
        }

        float x = (Screen.width - Width) * 0.5f;
        float y = Screen.height * 0.72f;
        DrawGauge(new Rect(x, y, Width, Height), _bgLeft, _bgCenter, _bgRight, Color.white);
        DrawGauge(new Rect(x, y, Width * Mathf.Clamp01(t), Height), _fillLeft, _fillCenter, _fillRight, Color.white);
    }

    static void DrawGauge(Rect r, Texture2D left, Texture2D center, Texture2D right, Color tint)
    {
        if (r.width <= 0f)
            return;
        GUI.color = tint;
        if (left == null || center == null || right == null)
        {
            GUI.DrawTexture(r, Texture2D.whiteTexture);
            GUI.color = Color.white;
            return;
        }
        float cap = Mathf.Min(left.width, r.width * 0.5f);
        GUI.DrawTexture(new Rect(r.x, r.y, cap, r.height), left);
        GUI.DrawTexture(new Rect(r.x + cap, r.y, Mathf.Max(0f, r.width - 2f * cap), r.height), center);
        GUI.DrawTexture(new Rect(r.xMax - cap, r.y, cap, r.height), right);
        GUI.color = Color.white;
    }
}
