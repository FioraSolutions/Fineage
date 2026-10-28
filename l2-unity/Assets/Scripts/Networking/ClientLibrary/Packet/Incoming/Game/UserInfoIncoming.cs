[IncomingGamePacket(GameServerPacketType.UserInfo)]
public sealed class UserInfoIncoming : IncomingWirePacket<UserInfoDto>
{
    protected override UserInfoDto CreateDto() => UserInfoIncomingPacketServices.CreateUserInfoDto();

    public override void Apply(UserInfoDto userInfo) =>
        UserInfoIncomingPacketServices.ApplyUserInfo(userInfo);
}
