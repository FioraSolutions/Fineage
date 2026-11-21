/// <summary>
/// Locomotion animator bool suffix (wait_/run_/atk01_) and optional lobby tuning from resolved equip state.
/// </summary>
public readonly struct LocomotionProfile
{
    public static readonly LocomotionProfile BareHand = new LocomotionProfile("hand", lobbyWalkUnitsPerSec: 1.2f, isResolved: true);

    public LocomotionProfile(string animSuffix, float lobbyWalkUnitsPerSec, bool isResolved)
    {
        AnimSuffix = animSuffix ?? string.Empty;
        LobbyWalkUnitsPerSec = lobbyWalkUnitsPerSec;
        IsResolved = isResolved;
    }

    public string AnimSuffix { get; }

    /// <summary>Char select pawn slide speed (not in-world run speed).</summary>
    public float LobbyWalkUnitsPerSec { get; }

    public bool IsResolved { get; }
}