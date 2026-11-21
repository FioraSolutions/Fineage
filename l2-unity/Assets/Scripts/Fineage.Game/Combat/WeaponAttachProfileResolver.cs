public static class WeaponAttachProfileResolver
{
    public static WeaponAttachProfile Resolve(Weapongrp grp, WeaponAttachEquipMode mode)
    {
        var profile = new WeaponAttachProfile();
        if (grp == null)
            return profile;

        switch (mode)
        {
            case WeaponAttachEquipMode.Shield:
                profile.Add(new WeaponAttachMount(
                    WeaponAttachSocket.Shield,
                    grp.Model,
                    doubleSidedMaterials: true));
                break;
            case WeaponAttachEquipMode.Bow:
                profile.Add(new WeaponAttachMount(WeaponAttachSocket.Bow, grp.Model));
                break;
            case WeaponAttachEquipMode.Arrow:
                profile.Add(new WeaponAttachMount(
                    WeaponAttachSocket.RightHand,
                    grp.Model,
                    snapIdentityAtSocket: true));
                break;
            case WeaponAttachEquipMode.RightHand:
                profile.Add(new WeaponAttachMount(WeaponAttachSocket.RightHand, grp.Model));
                break;
            case WeaponAttachEquipMode.LeftHand:
                profile.Add(new WeaponAttachMount(
                    WeaponAttachSocket.LeftHand,
                    WeapongrpHandMeshes.ModelForHand(grp, leftHand: true)));
                break;
            case WeaponAttachEquipMode.BothHands:
                profile.Add(new WeaponAttachMount(WeaponAttachSocket.RightHand, grp.Model));
                bool mirrorLeft = WeapongrpHandMeshes.ShouldMirrorLeftHand(grp);
                profile.Add(new WeaponAttachMount(
                    WeaponAttachSocket.LeftHand,
                    WeapongrpHandMeshes.ModelForHand(grp, leftHand: true),
                    mirrorScaleX: mirrorLeft));
                break;
        }

        return profile;
    }

    public static WeaponAttachEquipMode ModeForSingleEquip(Weapongrp grp, bool leftSlot)
    {
        if (grp == null)
            return leftSlot ? WeaponAttachEquipMode.LeftHand : WeaponAttachEquipMode.RightHand;
        if (grp.WeaponType == WeaponType.bow)
            return WeaponAttachEquipMode.Bow;
        if (grp.WeaponType == WeaponType.shield
            || grp.BodyPart == ItemSlot.lhand_shield)
            return WeaponAttachEquipMode.Shield;
        return leftSlot ? WeaponAttachEquipMode.LeftHand : WeaponAttachEquipMode.RightHand;
    }
}