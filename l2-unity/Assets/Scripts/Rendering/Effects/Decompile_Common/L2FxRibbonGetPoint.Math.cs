using UnityEngine;

public static partial class L2FxRibbonGetPoint
{

    /// <summary>
    /// Linear sheet between two cross-sections (a2/a3/a4).
    /// Prefer <see cref="LerpEdgesArc"/> for swing trails — linear chords look
    /// "скошенными" on large per-frame tip motion.
    /// </summary>

    /// <summary>
    /// Arc sheet: Slerp tip and base around <paramref name="pivot"/> (weapon/hand).
    /// Unity only has one animated pose per frame; linear Lerp of endpoints
    /// draws a chord. Pivot-Slerp approximates the real swing arc so a top-down
    /// cut reads as a curve instead of a flat cut.
    /// </summary>


    /// <summary>
    /// Sheet count from the longest of tip/base/mid travel (tip often moves
    /// farther than mid on a swing).
    /// </summary>

    /// <summary>
    /// How many intermediate sheets to insert so consecutive midpoints are
    /// at most <paramref name="maxSegmentMeters"/> apart.
    /// </summary>
}
