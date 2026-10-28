/// <summary>
/// Unwired game/combat incoming packet handler entry points (#142 sub-slice 57).
/// Stubs reserve facade wiring before UI or world apply is implemented.
/// Shop preview, macro list (sub-slice 80); char create ok (sub-slice 100), henna (sub-slice 99), and
/// friend list, target unselected, skill cool time (sub-slice 81) re-homed to domain facades.
/// Action failed and setup gauge (sub-slice 82) re-homed to <see cref="CombatFeedbackIncomingPacketServices"/>.
/// </summary>
public static class StubIncomingPacketServices
{
}
