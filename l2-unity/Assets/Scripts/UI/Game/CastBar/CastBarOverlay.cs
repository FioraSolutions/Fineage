using UnityEngine;
using System;

/// <summary>
/// Local cast timing clock (Interlude SetupGauge / MagicSkillUse HitTime).
/// Does not draw — <see cref="NameplateCastBarDrawer"/> paints the gauge in the HeadDisplay stack.
/// Instant skills (&lt;200 ms) skip.
/// </summary>
public sealed class CastBarOverlay : MonoBehaviour
{
    static CastBarOverlay _instance;

    float _endRealtime;
    float _duration;
    bool _active;

    public static void Begin(int hitTimeMs, int skillId = 0)
    {
        _ = skillId;
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
        // Same clock as SkillCastSound / InterludeSfx.PlayAfter (realtime).
        _instance._endRealtime = now + duration;
        _instance._active = true;
    }

    public static void Cancel()
    {
        if (_instance != null)
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

    void Update()
    {
        if (!_active || _duration <= 0f)
            return;

        if (_endRealtime - Time.realtimeSinceStartup > 0f)
            return;

        _active = false;
        try { SkillCastSound.NotifyLocalCastBarFinished(); }
        catch (System.Exception) { }
    }
}
