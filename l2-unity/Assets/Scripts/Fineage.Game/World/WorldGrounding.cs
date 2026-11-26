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
    /// Highest walkable Y at or below <paramref name="serverY"/> (loot must rest on floor,
    /// not at the mob's body height when the packet Y is above the deck).
    /// </summary>
    public static bool TryPickFloorBelow(
        float serverY,
        IList<float> walkableYs,
        out float floorY,
        float aboveEpsilon = 0.5f)
    {
        floorY = serverY;
        if (walkableYs == null || walkableYs.Count == 0)
            return false;

        float best = float.NegativeInfinity;
        bool found = false;
        for (int i = 0; i < walkableYs.Count; i++)
        {
            float y = walkableYs[i];
            if (y > serverY + aboveEpsilon)
                continue;
            if (!found || y > best)
            {
                best = y;
                found = true;
            }
        }

        if (!found)
            return false;
        floorY = best;
        return true;
    }

    /// <summary>Closest walkable height by |ΔY| (for failure diagnostics only).</summary>
    public static bool TryGetNearestWalkableY(float serverY, IList<float> walkableYs, out float nearestY)
    {
        nearestY = serverY;
        if (walkableYs == null || walkableYs.Count == 0)
            return false;

        float bestAbs = float.PositiveInfinity;
        bool found = false;
        for (int i = 0; i < walkableYs.Count; i++)
        {
            float y = walkableYs[i];
            float abs = Mathf.Abs(y - serverY);
            if (abs < bestAbs)
            {
                bestAbs = abs;
                nearestY = y;
                found = true;
            }
        }

        return found;
    }
}
