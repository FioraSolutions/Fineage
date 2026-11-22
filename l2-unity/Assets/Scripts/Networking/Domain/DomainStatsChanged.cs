/// <summary>
/// Local (or owned) player stats/equip refresh. UserInfo is the Interlude bridge.
/// </summary>
public sealed class DomainStatsChanged
{
    public int ObjectId;
    public PlayerInfoInterlude Player;
    public byte MountType;
    public int MountNpcId;
}
