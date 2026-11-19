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

    /// <summary>Deprecated alias of <see cref="WorldGrounding.SnapToleranceMeters"/>.</summary>
    public const float CaveDeckMeters = WorldGrounding.SnapToleranceMeters;

    /// <summary>Deprecated alias of <see cref="WorldGrounding.SnapToleranceMeters"/>.</summary>
    public const float TownFloorDropMeters = WorldGrounding.SnapToleranceMeters;

    /// <summary>Deprecated alias of <see cref="WorldGrounding.SnapToleranceMeters"/>.</summary>
    public const float TownFloorLiftMeters = WorldGrounding.SnapToleranceMeters;

    /// <summary>
    /// Resolve a Unity world position: server Y is the authority, walkable
    /// mesh/terrain (not water) within <see cref="WorldGrounding.SnapToleranceMeters"/>
    /// may replace it. Every placement kind shares this pipeline.
    /// </summary>
}
