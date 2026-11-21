using UnityEngine;

/// <summary>
/// Attacker context for the next target <see cref="Entity.PlayHitReaction"/> (melee packet timing).
/// </summary>
public static class CombatAudioHitContext
{
    struct Entry
    {
        public Entity Attacker;
        public bool Critical;
    }

    static Entry _pending;

    public static void Set(Entity attacker, bool critical)
    {
        _pending = new Entry { Attacker = attacker, Critical = critical };
    }

    public static bool TryTake(out Entity attacker, out bool critical)
    {
        attacker = _pending.Attacker;
        critical = _pending.Critical;
        _pending = default;
        return attacker != null;
    }

    public static void Clear() => _pending = default;
}