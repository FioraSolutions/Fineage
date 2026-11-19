using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// One rule for standing a pawn on the world: the server height is the authority.
/// Pick the nearest walkable height within <see cref="SnapToleranceMeters"/>;
/// otherwise keep the server height. No per-area or per-mob exceptions.
/// </summary>
public static class WorldGrounding
{
    /// <summary>
    /// Max |ΔY| from the server height for a mesh hit to replace it.
    /// Covers town geodata-vs-brush (up to ~2 m) and stacked cave decks (~4 m),
    /// but not a basement shell about 9 m down.
    /// </summary>
    public const float SnapToleranceMeters = 4.2f;

    /// <summary>When two hits are this close in |ΔY|, prefer the higher deck.</summary>
    public const float NearTieMeters = 0.75f;

    /// <summary>
    /// How far aside to search when the exact XY has no walkable floor
    /// (pillar, water hole, missing rim).
    /// </summary>
    public const float NearbySearchMeters = 5f;

    /// <summary>
    /// Pull nameplate depth toward the camera so the label does not z-fight
    /// the owner's own mesh, while still hiding behind walls and other bodies.
    /// </summary>
    public const float NameplateDepthBiasMeters = 0.15f;

    /// <summary>
    /// Choose a walkable Y nearest to <paramref name="serverY"/>.
    /// Returns <paramref name="serverY"/> when nothing is within
    /// <paramref name="tolerance"/>. Among near-ties, the higher surface wins.
    /// </summary>
    public static float PickHeight(
        float serverY,
        IList<float> walkableYs,
        float tolerance = SnapToleranceMeters)
    {
        if (walkableYs == null || walkableYs.Count == 0)
            return serverY;

        float bestAbs = float.PositiveInfinity;
        for (int i = 0; i < walkableYs.Count; i++)
        {
            float abs = Mathf.Abs(walkableYs[i] - serverY);
            if (abs < bestAbs)
                bestAbs = abs;
        }

        if (bestAbs > tolerance)
            return serverY;

        float bestY = float.NegativeInfinity;
        bool found = false;
        for (int i = 0; i < walkableYs.Count; i++)
        {
            float y = walkableYs[i];
            float abs = Mathf.Abs(y - serverY);
            if (abs > bestAbs + NearTieMeters)
                continue;
            if (!found || y > bestY)
            {
                bestY = y;
                found = true;
            }
        }

        return found ? bestY : serverY;
    }

    /// <summary>
    /// True when a water surface sits above the feet, so the pawn would be submerged.
    /// </summary>
    public static bool IsSubmerged(float feetY, float waterSurfaceY, float epsilon = 0.05f)
    {
        return waterSurfaceY > feetY + epsilon;
    }
}
