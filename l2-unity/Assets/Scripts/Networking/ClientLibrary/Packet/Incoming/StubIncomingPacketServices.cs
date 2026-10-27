/// <summary>
/// Unwired game/combat incoming packet handler entry points (#142 sub-slice 57).
/// Stubs reserve facade wiring before UI or world apply is implemented.
/// Character create ok, henna, shop preview, and macro list re-homed in sub-slice 80.
/// </summary>
public static class StubIncomingPacketServices
{
    public static void ApplyFriendList(FriendListIncomingDto dto)
    {
    }

    public static void ApplyTargetUnselected(TargetUnselectedDto dto)
    {
    }

    public static void ApplySkillCoolTime(SkillCoolTimeDto packet)
    {
    }

    public static void ApplyActionFailed(ActionFailedDto packet)
    {
    }

    public static void ApplySetupGauge(SetupGaugeDto packet)
    {
    }
}
