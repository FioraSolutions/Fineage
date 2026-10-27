/// <summary>
/// Unwired game/combat incoming packet handler entry points (#142 sub-slice 57).
/// Stubs reserve facade wiring before UI or world apply is implemented.
/// Character create ok, henna, shop preview, macro list (sub-slice 80) and
/// friend list, target unselected, skill cool time (sub-slice 81) re-homed to domain facades.
/// </summary>
public static class StubIncomingPacketServices
{
    public static void ApplyActionFailed(ActionFailedDto packet)
    {
    }

    public static void ApplySetupGauge(SetupGaugeDto packet)
    {
    }
}
