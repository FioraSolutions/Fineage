using UnityEngine;

public partial class UserInfoDto : IWireDto
{
    private PlayerInfoInterlude _info;
    public PlayerInfoInterlude PlayerInfoInterlude { get { return _info; } }

    public UserInfoDto()
    {
    }

    public UserInfoDto(PlayerInfoInterlude info)
    {
        _info = info;
    }

    public void ReadFrom(PacketReader reader)
    {
        ReadIdentityAndVitals(reader);
        ReadPaperDollAndCombat(reader);
        ReadSocialAndFlags(reader);
    }
}
