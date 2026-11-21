/// <summary>
/// Hand sync / stance rules for a weapon row, resolved once from <see cref="Weapongrp"/> at equip boundary.
/// </summary>
public readonly struct WeaponAttachLayout
{
    public static readonly WeaponAttachLayout Empty = new WeaponAttachLayout(
        equipPrimaryOnLeft: false,
        syncAsDualSameId: false,
        isShieldSlot: false,
        occupiesBothHands: false,
        causesStanceAnimRefresh: false,
        isExtendedTwoHandStance: false);

    public WeaponAttachLayout(
        bool equipPrimaryOnLeft,
        bool syncAsDualSameId,
        bool isShieldSlot,
        bool occupiesBothHands,
        bool causesStanceAnimRefresh,
        bool isExtendedTwoHandStance)
    {
        EquipPrimaryOnLeft = equipPrimaryOnLeft;
        SyncAsDualSameId = syncAsDualSameId;
        IsShieldSlot = isShieldSlot;
        OccupiesBothHands = occupiesBothHands;
        CausesStanceAnimRefresh = causesStanceAnimRefresh;
        IsExtendedTwoHandStance = isExtendedTwoHandStance;
    }

    /// <summary>Bow mounts on left paperdoll / hand bone.</summary>
    public bool EquipPrimaryOnLeft { get; }

    /// <summary>Dual swords, dual fist, fist (lrhand) — same id in both hands.</summary>
    public bool SyncAsDualSameId { get; }

    public bool IsShieldSlot { get; }

    public bool OccupiesBothHands { get; }

    /// <summary>Pole/staff/big sword/bow — locomotion stance must refresh on equip.</summary>
    public bool CausesStanceAnimRefresh { get; }

    /// <summary>Pole/staff/big sword/big blunt — 2H paperdoll query (excludes bow/dual).</summary>
    public bool IsExtendedTwoHandStance { get; }
}