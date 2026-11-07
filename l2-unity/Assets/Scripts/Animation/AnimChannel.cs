/// <summary>
/// Exclusive presentation channels for the local player.
/// Mobius keeps cast, combat stance, and social independent — we only need mutual exclusion
/// where Unity has one Animator: a lower channel must not CrossFade over a higher one.
/// </summary>
public static class AnimChannel
{
    public enum Kind
    {
        None = 0,
        Social = 1,
        CombatSwing = 2,
        Cast = 3,
    }

    static Kind _held = Kind.None;

    public static Kind Held => _held;

    public static bool IsHeld(Kind kind) => _held == kind;

    /// <summary>True if <paramref name="kind"/> may CrossFade now (equal or higher than held).</summary>
    public static bool CanPlay(Kind kind)
    {
        return (int)kind >= (int)_held;
    }

    public static void Begin(Kind kind)
    {
        if ((int)kind >= (int)_held)
            _held = kind;
    }

    public static void End(Kind kind)
    {
        if (_held == kind)
            _held = Kind.None;
    }

    public static void Clear()
    {
        _held = Kind.None;
    }
}
