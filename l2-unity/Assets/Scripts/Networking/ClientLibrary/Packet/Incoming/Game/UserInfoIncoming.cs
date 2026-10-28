[IncomingGamePacket(GameServerPacketType.UserInfo)]
public sealed class UserInfoIncoming : IncomingWirePacket<UserInfoDto>
{
    protected override UserInfoDto CreateDto() => UserInfoApplyIncomingPacketServices.CreateUserInfoDto();

    public override void Apply(UserInfoDto userInfo) =>
        UserInfoApplyIncomingPacketServices.ApplyUserInfo(userInfo);
}
