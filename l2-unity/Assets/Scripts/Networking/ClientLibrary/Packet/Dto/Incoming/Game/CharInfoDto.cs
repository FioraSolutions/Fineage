using UnityEngine;

/// <summary>
/// aCis Interlude CharInfo (0x03):
/// aCis_gameserver/.../serverpackets/CharInfo.java writeImpl()
/// </summary>
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

    public void ReadFrom(PacketReader reader)
    {
        ReadIdentity(reader);
        ReadEquipment(reader);
        ReadSocial(reader);
    }
}
