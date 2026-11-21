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
        public bool Shielded;
    }

    static Entry _pending;

    public static void Set(Entity attacker, bool critical, bool shielded)
    {
        _pending = new Entry { Attacker = attacker, Critical = critical, Shielded = shielded };
    }

    public static bool PendingShielded => _pending.Shielded;

    public static bool TryTake(out Entity attacker, out bool critical, out bool shielded)
    {
        attacker = _pending.Attacker;
        critical = _pending.Critical;
        shielded = _pending.Shielded;
        _pending = default;
        return attacker != null || shielded;
    }

    public static void Clear() => _pending = default;
}