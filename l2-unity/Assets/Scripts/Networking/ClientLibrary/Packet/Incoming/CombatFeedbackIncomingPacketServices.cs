/// <summary>
/// Combat feedback wire packet handler entry (#142 sub-slice 82).
/// Re-homes unwired action-failed / setup-gauge apply from StubIncomingPacketServices until combat UI wiring lands.
/// </summary>
public static class CombatFeedbackIncomingPacketServices
{
    public static void ApplyActionFailed(ActionFailedDto packet)
    {
    }

    public static void ApplySetupGauge(SetupGaugeDto packet)
    {
    }
}
