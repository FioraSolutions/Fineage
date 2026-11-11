using UnityEngine;

public static partial class WorldPlacement
{
    public enum PlacementKind
    {
        PlayerTeleport,
        NpcSpawn,
        MonsterSpawn,
        ItemDrop,
        WalkValidate
    }

    public const float HighOriginY = 750f;
    public const float HighMaxDistance = 1000f;
    public const float DefaultStartAbove = 50f;
    public const float DefaultMaxDistance = 300f;

    /// <summary>Near-tie epsilon: prefer higher deck when Δ(serverY) is almost equal.</summary>
    const float NearTieMeters = 0.75f;

    /// <summary>Resolve a Unity world position for the given placement kind.</summary>
}
