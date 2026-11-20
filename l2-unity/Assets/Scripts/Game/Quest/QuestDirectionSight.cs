using UnityEngine;

/// <summary>
/// Shared Interlude quest-direction pipeline: RadarControl-style target from
/// <see cref="QuestJournalState.Direction"/> (zone QuestDirection packet).
/// Drives both the overhead world arrow and the top-right CH3 radar.
/// </summary>
public static class QuestDirectionSight
{
    /// <summary>Hide indicators once the player is within talk range of the target (L2 UU).</summary>
    public const float ArriveUu = 200f;

    /// <summary>Radius of the CH3 radar disc in L2 UU for placing the target blip.</summary>
    public const float RadarRadiusUu = 4000f;

    public static bool TryActive(out QuestDirectionTarget target, out Vector3 world, out Vector3 playerL2, out float planarUu)
    {
        target = QuestJournalState.Direction;
        world = default;
        playerL2 = default;
        planarUu = 0f;

        if (!target.Show)
            return false;
        if (!QuestPacketServices.IsTrackingEnabled())
            return false;

        PlayerEntity player = DomainPacketServices.LocalPlayer;
        if (player == null)
            return false;

        playerL2 = VectorUtils.ConvertPosUnityToL2j(player.transform.position);
        planarUu = PlanarUu(playerL2.x, playerL2.y, target.X, target.Y);
        if (planarUu <= ArriveUu)
            return false;

        world = VectorUtils.ConvertPosToUnity(new Vector3(target.X, target.Y, target.Z));
        return true;
    }

    public static float PlanarUu(float fromX, float fromY, float toX, float toY)
    {
        float dx = toX - fromX;
        float dy = toY - fromY;
        return Mathf.Sqrt(dx * dx + dy * dy);
    }
}
