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

    /// <summary>
    /// Dark Elf street sits about 3 m under the server Z. The next shell is about 9 m
    /// below that. Anything farther than this is the basement, not the floor.
    /// </summary>
    public const float CaveDeckMeters = 4.2f;

    /// <summary>
    /// Town server Z often sits on the geodata node, half a metre to two metres above
    /// the brush floor (Gludin plaza is ~0.5 m). Farther than this is the next storey.
    /// </summary>
    public const float TownFloorDropMeters = 2.4f;

    /// <summary>Resolve a Unity world position for the given placement kind.</summary>
}
