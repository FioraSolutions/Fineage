[IncomingGamePacket(GameServerPacketType.UserInfo)]
public sealed class UserInfoIncoming : IncomingWirePacket<UserInfoDto>
{
    protected override UserInfoDto CreateDto() => PlayerWorldIncomingPacketServices.CreateUserInfoDto();

    public override void Apply(UserInfoDto userInfo) =>
        PlayerWorldIncomingPacketServices.ApplyUserInfo(userInfo);
}
