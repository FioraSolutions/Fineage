using System;

/// <summary>
/// Pledge/clan wire packet handler access via GameIncomingPacketServices (#142 sub-slice 60).
/// Pledge info on <see cref="PledgeInfoIncomingPacketServices"/> (sub-slice 122).
/// Pledge status-changed on <see cref="PledgeStatusChangedIncomingPacketServices"/> (sub-slice 123).
/// Pledge show-member-list-add on <see cref="PledgeShowMemberListAddIncomingPacketServices"/> (sub-slice 124).
/// </summary>
public static class PledgeIncomingPacketServices
{
    public static void Queue(Action action) => GameIncomingPacketServices.Queue(action);

    public static void ApplyPledgeShowMemberListAll(PledgeShowMemberListAllDto packet) =>
        Queue(() => GameIncomingPacketServices.Clan.AddClanData(packet));

    public static void ApplyPledgeShowMemberListDeleteAll() =>
        Queue(() => GameIncomingPacketServices.Clan.DeleteMemberData());

    public static void ApplyPledgeShowMemberListDelete(PledgeShowMemberListDeleteDto packet) =>
        Queue(() => GameIncomingPacketServices.Clan.DeleteMemberData(packet));

    public static void ApplyPledgeShowMemberListUpdate(PledgeShowMemberListUpdateDto packet) =>
        Queue(() => GameIncomingPacketServices.Clan.UpdateMemberData(packet));

    public static void ApplyManagePledgePower(ManagePledgePowerDto packet) =>
        Queue(() => GameIncomingPacketServices.Clan.UpdateDetailedInfo(packet));

    public static void ApplyExPledgePowerGradeList(PledgePowerGradeListDto packet) =>
        Queue(() => GameIncomingPacketServices.Clan.ShowGradeInfo(packet));

    public static void ApplyExPledgeReceiveMemberInfo(PledgeReceiveMemberInfoDto packet) =>
        Queue(() => GameIncomingPacketServices.Clan.UpdateDetailedInfo(packet));

    public static void ApplyExPledgeReceivePowerInfo(PledgeReceivePowerInfoDto packet) =>
        Queue(() => GameIncomingPacketServices.Clan.UpdateDetailedInfo(packet));
}
