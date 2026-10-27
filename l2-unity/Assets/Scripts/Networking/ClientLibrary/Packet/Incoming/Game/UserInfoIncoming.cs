using UnityEngine;

[IncomingGamePacket(GameServerPacketType.UserInfo)]
public sealed class UserInfoIncoming : IncomingWirePacket<UserInfoDto>
{
    protected override UserInfoDto CreateDto()
    {
        return new UserInfoDto(IncomingPacketServices.Game.PlayerInfo);
    }

    public override void Apply(UserInfoDto userInfo)
    {
        StorageNpc.getInstance().AddUserInfo(userInfo);
        IncomingPacketServices.Game.SetDataPreparationCompleted(true);
        PlayerAppearance app = userInfo != null ? userInfo.PlayerInfoInterlude.Appearance : null;
        int uid = userInfo != null && userInfo.PlayerInfoInterlude.Identity != null
            ? userInfo.PlayerInfoInterlude.Identity.Id
            : 0;
        // GearFlowLog.Info("UserInfo RECV id=" + uid + " " + GearFlowLog.Paperdoll(app) +
            // " local=" + (PlayerEntity.Instance != null ? GearFlowLog.Entity(PlayerEntity.Instance) : "PlayerEntity=null"));
        IncomingPacketServices.Queue(() =>
        {
            DomainStatsChanged changed = InterludeDomainMap.Stats(userInfo);
            if (IncomingPacketServices.Domain != null && changed != null)
            {
                IncomingPacketServices.Domain.ApplyStatsChanged(changed);
            }
            else if (IncomingPacketServices.GameWorld != null)
            {
                IncomingPacketServices.GameWorld.UserInfoUpdateCharacter(userInfo);
                if (PlayerEntity.Instance != null)
                    IncomingPacketServices.GameWorld.UpdateUserInfo(PlayerEntity.Instance, userInfo);
            }

            if (userInfo.PlayerInfoInterlude.Identity.ClanId != 0)
            {
                IncomingPacketServices.Game.Send(new RequestPledgeInfoCommand(userInfo.PlayerInfoInterlude.Identity.ClanId));
            }
        });
    }
}
