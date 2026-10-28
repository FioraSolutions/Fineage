using System;

/// <summary>
/// Pledge/clan wire packet handler access via GameIncomingPacketServices (#142 sub-slice 60).
/// Pledge info on <see cref="PledgeInfoIncomingPacketServices"/> (sub-slice 122).
/// Pledge status-changed on <see cref="PledgeStatusChangedIncomingPacketServices"/> (sub-slice 123).
/// Pledge show-member-list-add on <see cref="PledgeShowMemberListAddIncomingPacketServices"/> (sub-slice 124).
/// Pledge show-member-list-all on <see cref="PledgeShowMemberListAllIncomingPacketServices"/> (sub-slice 125).
/// Pledge show-member-list-delete-all on <see cref="PledgeShowMemberListDeleteAllIncomingPacketServices"/> (sub-slice 126).
/// Pledge show-member-list-delete on <see cref="PledgeShowMemberListDeleteIncomingPacketServices"/> (sub-slice 127).
/// Pledge show-member-list-update on <see cref="PledgeShowMemberListUpdateIncomingPacketServices"/> (sub-slice 128).
/// Manage pledge power on <see cref="ManagePledgePowerIncomingPacketServices"/> (sub-slice 129).
/// </summary>
public static class PledgeIncomingPacketServices
{
    public static void Queue(Action action) => GameIncomingPacketServices.Queue(action);



    public static void ApplyExPledgePowerGradeList(PledgePowerGradeListDto packet) =>
        Queue(() => GameIncomingPacketServices.Clan.ShowGradeInfo(packet));

    public static void ApplyExPledgeReceiveMemberInfo(PledgeReceiveMemberInfoDto packet) =>
        Queue(() => GameIncomingPacketServices.Clan.UpdateDetailedInfo(packet));

    public static void ApplyExPledgeReceivePowerInfo(PledgeReceivePowerInfoDto packet) =>
        Queue(() => GameIncomingPacketServices.Clan.UpdateDetailedInfo(packet));
}
