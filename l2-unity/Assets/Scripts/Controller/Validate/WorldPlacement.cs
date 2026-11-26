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

    /// <summary>Optional ids for placement failure logs (ground items).</summary>
    public readonly struct PlacementFailContext
    {
        public readonly int ObjectId;
        public readonly int ItemId;

        public PlacementFailContext(int objectId, int itemId)
        {
            ObjectId = objectId;
            ItemId = itemId;
        }
    }

    /// <summary>True when placement must find walkable floor instead of keeping server Y.</summary>
    public static bool RequiresFloorSearch(PlacementKind kind) => kind == PlacementKind.ItemDrop;

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
    /// Resolve a Unity world position. Pawns/teleports: server Y is authority, walkable
    /// mesh within <see cref="WorldGrounding.SnapToleranceMeters"/> may replace it.
    /// <see cref="PlacementKind.ItemDrop"/> searches floor (mesh + geodata) instead.
    /// </summary>
}
