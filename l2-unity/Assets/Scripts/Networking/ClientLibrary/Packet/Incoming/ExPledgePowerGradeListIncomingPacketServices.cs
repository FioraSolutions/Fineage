using System;

/// <summary>
/// Ex pledge power-grade-list wire packet handler entry (#142 sub-slice 130).
/// Splits apply out of <see cref="PledgeIncomingPacketServices"/> (sub-slice 60 encapsulation).
/// </summary>
public static class ExPledgePowerGradeListIncomingPacketServices
{
    public static void Queue(Action action) => GameIncomingPacketServices.Queue(action);

    public static void ApplyExPledgePowerGradeList(PledgePowerGradeListDto packet) =>
        Queue(() => GameIncomingPacketServices.Clan.ShowGradeInfo(packet));
