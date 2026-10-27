using System;
using System.Collections.Generic;

/// <summary>
/// Player character world-state incoming handler access via WorldIncomingPacketServices (#142 sub-slice 67).
/// </summary>
public static class PlayerWorldIncomingPacketServices
{
    public static GameClient Game => WorldIncomingPacketServices.Game;

    public static World GameWorld => WorldIncomingPacketServices.GameWorld;

    public static IWorldDomainSink Domain => WorldIncomingPacketServices.Domain;

    public static void Queue(Action action) => WorldIncomingPacketServices.Queue(action);

    public static void ApplyWorld(Action<WorldPacketApply> apply) =>
        WorldIncomingPacketServices.ApplyWorld(apply);

    public static UserInfoDto CreateUserInfoDto() => new UserInfoDto(Game.PlayerInfo);

    public static void ApplyUserInfo(UserInfoDto userInfo)
    {
        StorageNpc.getInstance().AddUserInfo(userInfo);
        Game.SetDataPreparationCompleted(true);
        Queue(() =>
        {
            DomainStatsChanged changed = InterludeDomainMap.Stats(userInfo);
            if (Domain != null && changed != null)
                Domain.ApplyStatsChanged(changed);
            else if (GameWorld != null)
            {
                GameWorld.UserInfoUpdateCharacter(userInfo);
                if (PlayerEntity.Instance != null)
                    GameWorld.UpdateUserInfo(PlayerEntity.Instance, userInfo);
            }

            if (userInfo.PlayerInfoInterlude.Identity.ClanId != 0)
                Game.Send(new RequestPledgeInfoCommand(userInfo.PlayerInfoInterlude.Identity.ClanId));
        });
    }

    public static void ApplyStatusUpdate(int objectId, List<StatusUpdate.Attribute> attributes)
    {
        if (GameWorld != null)
            GameWorld.StatusUpdate(objectId, attributes);
    }

    public static void ApplyTeleportToLocation(TeleportToLocationDto packet)
    {
        Queue(() => GameWorld.TeleportTo(packet.TarObjId, packet.TeleportPos));
    }

    public static void ApplyRevive(ReviveDto dto) => ApplyWorld(apply => apply.Revive(dto));
}
