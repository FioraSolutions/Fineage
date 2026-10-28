using System;

/// <summary>
/// Apply facade (#142 batch slice 151). Re-homes from <see cref="UserInfoIncomingPacketServices"/>.
/// 
/// User info wire packet handler entry (#142 batch slice 143).
/// </summary>
public static class UserInfoApplyIncomingPacketServices
{
    public static GameClient Game => WorldIncomingPacketServices.Game;

    public static World GameWorld => WorldIncomingPacketServices.GameWorld;

    public static IWorldDomainSink Domain => WorldIncomingPacketServices.Domain;

    public static void Queue(Action action) => WorldIncomingPacketServices.Queue(action);

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
