/// <summary>
/// Sole boundary that maps weapongrp fields to <see cref="WeaponAttachLayout"/> (no downstream WeaponType switches).
/// </summary>
public static class WeaponAttachLayoutResolver
{
    public static WeaponAttachLayout Resolve(Weapongrp grp)
    {
        if (grp == null)
            return WeaponAttachLayout.Empty;

        bool bow = grp.WeaponType == WeaponType.bow;
        bool shield = grp.WeaponType == WeaponType.shield
            || grp.BodyPart == ItemSlot.lhand_shield;
        // lrhand covers pole/staff/bow too — dual sync is weapon-type only (incl. fist on lrhand).
        bool dualVisual = grp.WeaponType == WeaponType.dual
            || grp.WeaponType == WeaponType.dualfist
            || grp.WeaponType == WeaponType.fist;

        bool occupiesBoth = grp.BodyPart == ItemSlot.lrhand
            || bow
            || dualVisual
            || grp.WeaponType == WeaponType.pole
            || grp.WeaponType == WeaponType.staff
            || grp.WeaponType == WeaponType.bigword
            || grp.WeaponType == WeaponType.bigblunt;

        bool stanceRefresh = grp.WeaponType == WeaponType.pole
            || grp.WeaponType == WeaponType.staff
            || grp.WeaponType == WeaponType.bigword
            || bow;

        bool extendedTwoHand = grp.WeaponType == WeaponType.pole
            || grp.WeaponType == WeaponType.staff
            || grp.WeaponType == WeaponType.bigword
            || grp.WeaponType == WeaponType.bigblunt;

        return new WeaponAttachLayout(
            equipPrimaryOnLeft: bow,
            syncAsDualSameId: dualVisual && !bow,
            isShieldSlot: shield,
            occupiesBothHands: occupiesBoth,
            causesStanceAnimRefresh: stanceRefresh,
            isExtendedTwoHandStance: extendedTwoHand);
    }

    public static bool EquipTriggersAnimRefresh(
        WeaponType newPrimaryType,
        WeaponType lastRightHandType,
        WeaponAttachLayout rightHand,
        WeaponAttachLayout leftHand,
        bool leftHandShieldPresent)
    {
        if (rightHand.CausesStanceAnimRefresh || leftHand.CausesStanceAnimRefresh)
            return true;
        if (!leftHandShieldPresent && newPrimaryType != lastRightHandType)
            return true;
        return false;
    }
}