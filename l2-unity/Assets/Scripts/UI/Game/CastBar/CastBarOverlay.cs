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
    /// <summary>GUI points above the head screen point (higher = further above head).</summary>
    const float GuiYAboveHead = 18f;
    /// <summary>World meters above the head/nameplate anchor so the gauge clears the name.</summary>
    const float WorldYAboveHeadMeters = 0.28f;

    static CastBarOverlay _instance;

    Texture2D _bg;
    Texture2D _fill;
    float _endRealtime;
    float _startRealtime;
    float _duration;
    bool _active;
    bool _firstPaintLogged;
    bool _waitHeadLogged;
    int _skillId;
    /// <summary>Last logged depleted bucket 0..10 (0%=start, 10%=empty).</summary>
    int _lastPctBucket = -1;

    public static void Begin(int hitTimeMs, int skillId = 0)
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
        float now = Time.realtimeSinceStartup;
        _instance._duration = duration;
        // Same clock as SkillCastSound / InterludeSfx.PlayAfter (realtime), not unscaledTime.
        _instance._startRealtime = now;
        _instance._endRealtime = now + duration;
        _instance._active = true;
        _instance._firstPaintLogged = false;
        _instance._waitHeadLogged = false;
        _instance._lastPctBucket = -1;
        _instance._skillId = skillId;

        Debug.Log(
            "[CastSync] BAR.begin skill=" + skillId +
            " hitMs=" + hitTimeMs +
            " durSec=" + duration.ToString("F3") +
            " t0=" + now.ToString("F3") +
            " emptyAt=" + _instance._endRealtime.ToString("F3"));
    }

    public static void Cancel()
    {
        if (_instance == null || !_instance._active)
            return;
        float t = Time.realtimeSinceStartup - _instance._startRealtime;
        float rem = Mathf.Clamp01((_instance._endRealtime - Time.realtimeSinceStartup) / Mathf.Max(0.0001f, _instance._duration));
        Debug.Log(
            "[CastSync] BAR.cancel skill=" + _instance._skillId +
            " t=" + t.ToString("F3") +
            " remainPct=" + (rem * 100f).ToString("F0"));
        _instance._active = false;
    }

    /// <summary>Remaining fill 0..1 while a cast gauge is active; -1 if idle.</summary>
    public static float Remaining01
    {
        get
        {
            if (_instance == null || !_instance._active || _instance._duration <= 0f)
                return -1f;
            return Mathf.Clamp01((_instance._endRealtime - Time.realtimeSinceStartup) / _instance._duration);
        }
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

        float now = Time.realtimeSinceStartup;
        float remainingSec = _endRealtime - now;
        if (remainingSec <= 0f || _duration <= 0f)
        {
            if (_active)
            {
                _active = false;
                float t = now - _startRealtime;
                Debug.Log(
                    "[CastSync] BAR.empty skill=" + _skillId +
                    " t=" + t.ToString("F3") +
                    " planned=" + _duration.ToString("F3") +
                    " remainPct=0");
                // Same wall-clock as HitTime — fires *_shot if the delayed SFX coroutine was lost.
                try { SkillCastSound.NotifyLocalCastBarFinished(); }
                catch (System.Exception) { }
            }
            return;
        }

        float remaining = Mathf.Clamp01(remainingSec / _duration);
        float depleted01 = 1f - remaining;
        int bucket = Mathf.Clamp(Mathf.FloorToInt(depleted01 * 10f), 0, 10);
        if (bucket != _lastPctBucket)
        {
            _lastPctBucket = bucket;
            Debug.Log(
                "[CastSync] BAR.tick skill=" + _skillId +
                " t=" + (now - _startRealtime).ToString("F3") +
                " remainPct=" + (remaining * 100f).ToString("F0") +
                " depletedPct=" + (depleted01 * 100f).ToString("F0") +
                " remainSec=" + remainingSec.ToString("F3"));
        }

        if (!TryGetHeadGui(out float guiX, out float guiY))
        {
            if (!_waitHeadLogged)
            {
                _waitHeadLogged = true;
                Debug.LogWarning(
                    "[CastSync] BAR.waitHead skill=" + _skillId +
                    " t=" + (now - _startRealtime).ToString("F3") +
                    " remainPct=" + (remaining * 100f).ToString("F0") +
                    " note=anchor miss — bar not drawn yet");
            }
            return;
        }

        if (!_firstPaintLogged)
        {
            _firstPaintLogged = true;
            Debug.Log(
                "[CastSync] BAR.firstPaint skill=" + _skillId +
                " delay=" + (now - _startRealtime).ToString("F3") +
                " remainPct=" + (remaining * 100f).ToString("F0"));
        }

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
        PlayerEntity playerEntity = DomainPacketServices.LocalPlayer;
        Transform player = playerEntity != null ? playerEntity.transform : null;
        if (player == null)
            return false;

        Camera cam = null;
        CameraController cameraController = CameraPacketServices.Controller;
        if (cameraController != null)
            cam = cameraController.GetComponent<Camera>();
        if (cam == null)
            cam = Camera.main;
        if (cam == null)
            return false;

        // Same head-bone preference as NameplateEntryStore — CC top alone parks mid-torso.
        Vector3 head;
        if (TryGetHeadBone(player, out Transform headBone))
        {
            head = headBone.position + Vector3.up * WorldYAboveHeadMeters;
        }
        else
        {
            float ch = L2NameplateAnchor.DefaultCollisionHeightMeters;
            if (playerEntity.Appearance != null)
                ch = playerEntity.Appearance.CollisionHeight;
            head = L2NameplateAnchor.GetHeadWorldPos(
                player,
                ch,
                L2NameplateAnchor.DefaultHeadHeightOffsetMeters);
            head += Vector3.up * WorldYAboveHeadMeters;
        }

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

    static bool TryGetHeadBone(Transform root, out Transform head)
    {
        head = null;
        if (root == null)
            return false;
        Transform[] all = root.GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < all.Length; i++)
        {
            Transform t = all[i];
            if (t == null || string.IsNullOrEmpty(t.name))
                continue;
            string n = t.name;
            if (n.Equals("Bip01_head", System.StringComparison.OrdinalIgnoreCase)
                || n.Equals("Bip01 Head", System.StringComparison.OrdinalIgnoreCase)
                || n.Equals("Bip01_Head", System.StringComparison.OrdinalIgnoreCase))
            {
                head = t;
                return true;
            }
        }
        return false;
    }
}
