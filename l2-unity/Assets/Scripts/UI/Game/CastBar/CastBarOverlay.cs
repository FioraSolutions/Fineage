using UnityEngine;

/// <summary>
/// Local player cast gauge above the head (Interlude HeadDisplay 128×8),
/// driven by MagicSkillUse.HitTime. Instant skills (hitTime &lt; 200 ms) skip the bar.
/// </summary>
public sealed class CastBarOverlay : MonoBehaviour
{
    const string GaugeRoot = "Data/UI/Assets/Target/HeadDisplay_DF_Gauge_HP/";
    const float Width = 128f;
    const float Height = 8f;
    const float ScreenYBelowName = 14f;

    static CastBarOverlay _instance;

    Texture2D _bg;
    Texture2D _fill;
    float _startTime;
    float _duration;
    bool _active;

    public static void Begin(int hitTimeMs)
    {
        if (hitTimeMs < 200)
            return;
        if (_instance == null)
        {
            var go = new GameObject(nameof(CastBarOverlay));
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
        _bg = Resources.Load<Texture2D>(GaugeRoot + "HeadDisplay_DF_Gauge_HP_Bg");
        _fill = Resources.Load<Texture2D>(GaugeRoot + "HeadDisplay_DF_Gauge_HP");
    }

    void OnGUI()
    {
        if (!_active || _bg == null || _fill == null)
            return;

        float t = _duration <= 0f ? 1f : (Time.unscaledTime - _startTime) / _duration;
        if (t >= 1f)
        {
            _active = false;
            return;
        }

        if (!TryGetHeadScreen(out Vector2 screen))
            return;

        float x = screen.x - Width * 0.5f;
        float y = Screen.height - screen.y + ScreenYBelowName;
        GUI.DrawTexture(new Rect(x, y, Width, Height), _bg);

        float fillW = Width * Mathf.Clamp01(t);
        if (fillW <= 0f)
            return;

        // Clip the full HeadDisplay fill to the cast progress width.
        GUI.BeginGroup(new Rect(x, y, fillW, Height));
        GUI.DrawTexture(new Rect(0f, 0f, Width, Height), _fill);
        GUI.EndGroup();
    }

    static bool TryGetHeadScreen(out Vector2 screen)
    {
        screen = default;
        Transform player = PlayerEntity.Instance != null ? PlayerEntity.Instance.transform : null;
        if (player == null)
            return false;

        Camera cam = Camera.main;
        if (cam == null)
            return false;

        Vector3 head = L2NameplateAnchor.GetHeadWorldPos(
            player,
            L2NameplateAnchor.DefaultCollisionHeightMeters,
            L2NameplateAnchor.DefaultHeadHeightOffsetMeters);
        Vector3 sp = cam.WorldToScreenPoint(head);
        if (sp.z <= 0f)
            return false;

        screen = sp;
        return true;
    }
}
