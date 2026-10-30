using UnityEngine;

/// <summary>
/// Local player cast gauge above the head (Interlude HeadDisplay 128×8).
/// Countdown: full at cast start → empty at hit. Instant skills (&lt;200 ms) skip.
/// </summary>
public sealed class CastBarOverlay : MonoBehaviour
{
    const string GaugeRoot = "Data/UI/Assets/Target/HeadDisplay_DF_Gauge_HP/";
    const float Width = 128f;
    const float Height = 8f;
    /// <summary>GUI points above the head screen point.</summary>
    const float GuiYAboveHead = 10f;
    const float WorldYAboveNameplateMeters = 0.22f;

    static CastBarOverlay _instance;

    Texture2D _bg;
    Texture2D _fill;
    float _endTime;
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

        float duration = hitTimeMs / 1000f;
        _instance._duration = duration;
        // End-time based so remaining falls every frame from the first repaint.
        _instance._endTime = Time.unscaledTime + duration;
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
        // Project has a global Event enum — qualify UnityEngine.Event.
        UnityEngine.Event ev = UnityEngine.Event.current;
        if (ev == null || ev.type != EventType.Repaint)
            return;
        if (!_active || _bg == null || _fill == null)
            return;

        float remainingSec = _endTime - Time.unscaledTime;
        if (remainingSec <= 0f || _duration <= 0f)
        {
            _active = false;
            return;
        }

        float remaining = Mathf.Clamp01(remainingSec / _duration);

        if (!TryGetHeadGui(out float guiX, out float guiY))
            return;

        float x = guiX - Width * 0.5f;
        float y = guiY - Height - GuiYAboveHead;

        GUI.DrawTexture(new Rect(x, y, Width, Height), _bg);

        float fillW = Width * remaining;
        if (fillW < 1f)
            return;

        // Left-aligned remaining slice (empties from the right as cast progresses).
        Rect dst = new Rect(x, y, fillW, Height);
        Rect uv = new Rect(0f, 0f, remaining, 1f);
        GUI.DrawTextureWithTexCoords(dst, _fill, uv);
    }

    static bool TryGetHeadGui(out float guiX, out float guiY)
    {
        guiX = guiY = 0f;
        Transform player = PlayerEntity.Instance != null ? PlayerEntity.Instance.transform : null;
        if (player == null)
            return false;

        Camera cam = null;
        if (CameraController.Instance != null)
            cam = CameraController.Instance.GetComponent<Camera>();
        if (cam == null)
            cam = Camera.main;
        if (cam == null)
            return false;

        Vector3 head = L2NameplateAnchor.GetHeadWorldPos(
            player,
            L2NameplateAnchor.DefaultCollisionHeightMeters,
            L2NameplateAnchor.DefaultHeadHeightOffsetMeters);
        head += Vector3.up * WorldYAboveNameplateMeters;

        Vector3 sp = cam.WorldToScreenPoint(head);
        if (sp.z <= 0f)
            return false;

        // Unity 6: GUIUtility.pixelsPerPoint is editor-only; ScreenToGUIPoint
        // flips Y and applies the GUI scale (Retina) for OnGUI coords.
        Vector2 gui = GUIUtility.ScreenToGUIPoint(new Vector2(sp.x, sp.y));
        guiX = gui.x;
        guiY = gui.y;
        return true;
    }
}
