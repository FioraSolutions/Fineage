
using UnityEngine;

public partial class NpcInfoDto : IWireDto
{
    //private EntityIdentity _identity;
    //private NpcStatusInterlude _status;
    //private PlayerStats _stats;
    public EntityIdentity Identity { get; private set; }

    public Appearance Appearance { get; private set; }


    public NpcStatusInterlude Status { get; private set; }
    public Stats Stats { get; private set; }

    /// <summary>
    /// Same identity, status, stats, and appearance the packet parsed.
    /// Knownlist apply rebuilds the wire DTO from those parts.
    /// </summary>
    public static NpcInfoDto FromKnownlist(
        EntityIdentity identity,
        NpcStatusInterlude status,
        Stats stats,
        Appearance appearance)
    {
        var dto = new NpcInfoDto();
        dto.Identity = identity;
        dto.Status = status;
        dto.Stats = stats;
        dto.Appearance = appearance;
        return dto;
    }





    /// <summary>
    /// aCis PetInfo 0xB1. Owner-only. Other players see the same summon as NpcInfo 0x16.
    /// Position is the server spawn point.
    /// </summary>


}
