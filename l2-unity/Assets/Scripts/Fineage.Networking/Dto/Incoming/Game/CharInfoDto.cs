
/// <summary>
/// aCis Interlude CharInfo (0x03):
/// aCis_gameserver/.../serverpackets/CharInfo.java writeImpl()
/// </summary>
using UnityEngine;

public partial class CharInfoDto : IWireDto
{
    public EntityIdentity Identity { get; private set; }
    public PlayerStatus Status { get; private set; }
    public PlayerStats Stats { get; private set; }
    public PlayerAppearance Appearance { get; private set; }
    public bool AlikeDead { get; private set; }
    public int PacketRace { get; private set; }
    public int PacketClassFirst { get; private set; }
    public int PacketClassSecond { get; private set; }

    public CharInfoDto()
    {
        Identity = new EntityIdentity();
        Status = new PlayerStatus();
        Stats = new PlayerStats();
        Appearance = new PlayerAppearance();
    }

    /// <summary>
    /// Same identity, status, stats, and appearance instances the packet parsed.
    /// Knownlist apply rebuilds the wire DTO from those domain parts.
    /// </summary>
    public static CharInfoDto FromKnownlist(
        EntityIdentity identity,
        PlayerStatus status,
        PlayerStats stats,
        PlayerAppearance appearance,
        bool alikeDead)
    {
        var dto = new CharInfoDto();
        dto.Identity = identity;
        dto.Status = status;
        dto.Stats = stats;
        dto.Appearance = appearance;
        dto.AlikeDead = alikeDead;
        return dto;
    }

    public void ReadFrom(PacketReader reader)
    {
        ReadIdentity(reader);
        ReadEquipment(reader);
        ReadSocial(reader);
    }

    static float GetRealSpeed(int baseSpeed, float speedMultiplier)
    {
        if (speedMultiplier <= 0f)
            return baseSpeed;
        return baseSpeed * speedMultiplier;
    }

    void ApplyStubHpFromAlikeDead()
    {
        if (Stats.MaxHp <= 0)
            Stats.MaxHp = 100;

        if (AlikeDead)
            Status.SetHp(0);
        else
            Status.SetHp(Stats.MaxHp);
    }

    static int ClampByte(int value)
    {
        if (value < 0)
            return 0;
        if (value > 255)
            return 255;
        return value;
    }
}
