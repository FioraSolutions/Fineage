
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

    /// <summary>
    /// Same player bag the packet parsed, plus the mount fields the sink reads.
    /// </summary>
    public static UserInfoDto FromStats(PlayerInfoInterlude player, byte mountType, int mountNpcId)
    {
        var dto = new UserInfoDto(player);
        dto.MountType = mountType;
        dto.MountNpcId = mountNpcId;
        return dto;
    }

    public void ReadFrom(PacketReader reader)
    {
        ReadIdentityAndVitals(reader);
        ReadPaperDollAndCombat(reader);
        ReadSocialAndFlags(reader);
    }
}
