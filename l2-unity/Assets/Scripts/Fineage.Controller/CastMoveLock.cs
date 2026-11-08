using UnityEngine;

/// <summary>Local player is rooted while casting (L2: no move during MagicSkillUse hit time).</summary>
public static class CastMoveLock
{
    static float _until;

    public static bool IsLocked => Time.unscaledTime < _until;

    public static void Begin(int hitTimeMs)
    {
        if (hitTimeMs <= 0)
            return;
        _until = Mathf.Max(_until, Time.unscaledTime + hitTimeMs / 1000f);
    }

    public static void Clear() => _until = 0f;
}
