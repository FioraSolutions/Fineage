using System;

/// <summary>
/// Apply facade (#142 batch slice 151). Re-homes from <see cref="UserInfoIncomingPacketServices"/>.
/// 
/// User info wire packet handler entry (#142 batch slice 143; world hub slice 179).
/// </summary>
public static class UserInfoApplyIncomingPacketServices
{
    public static GameClient Game => WorldSessionApplyIncomingPacketServices.Game;

    public static World GameWorld => WorldSessionApplyIncomingPacketServices.GameWorld;

    public static IWorldDomainSink Domain => WorldSessionApplyIncomingPacketServices.Domain;

    public static void Queue(Action action) => WorldSessionApplyIncomingPacketServices.Queue(action);

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
}
