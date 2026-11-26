using Fineage.Net.Contract;
using UnityEngine;

/// <summary>Apply Mobius ClientSetTime to the shared world clock and minimap HUD (#1671).</summary>
public static class ClientSetTimeApplyIncomingPacketServices
{
    public static void ApplyClientSetTime(ClientSetTimeNetMsg msg)
    {
        if (msg == null)
        {
            Debug.LogError("[GameTime] ClientSetTime apply called with null message.");
            return;
        }

        WorldClock clock = WorldClockPacketServices.Clock;
        if (clock == null)
            clock = WorldClock.EnsurePersistent();
        if (clock == null)
        {
            Debug.LogError("[GameTime] ClientSetTime apply failed: WorldClock is missing.");
            return;
        }

        clock.ApplyMobiusClientSetTime(msg.GameMinutes, msg.ClockSpeed);
        HudCorePacketServices.Map?.RefreshMinimapGameTime();
    }
}