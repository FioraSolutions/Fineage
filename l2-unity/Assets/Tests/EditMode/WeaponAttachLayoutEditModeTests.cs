using NUnit.Framework;

public sealed class WeaponAttachLayoutEditModeTests
{
    [Test]
    public void Bow_EquipPrimaryOnLeft_AndStanceRefresh()
    {
        var grp = new Weapongrp { WeaponType = WeaponType.bow, BodyPart = ItemSlot.lrhand };
        var layout = WeaponAttachLayoutResolver.Resolve(grp);
        Assert.IsTrue(layout.EquipPrimaryOnLeft);
        Assert.IsTrue(layout.CausesStanceAnimRefresh);
        Assert.IsTrue(layout.OccupiesBothHands);
        Assert.IsFalse(layout.SyncAsDualSameId);
    }

    [Test]
    public void DualFist_SyncAsDualSameId()
    {
        var grp = new Weapongrp { WeaponType = WeaponType.dualfist, BodyPart = ItemSlot.lrhand };
        var layout = WeaponAttachLayoutResolver.Resolve(grp);
        Assert.IsTrue(layout.SyncAsDualSameId);
        Assert.IsFalse(layout.EquipPrimaryOnLeft);
    }

    [Test]
    public void Pole_ExtendedTwoHandStance_NotDualSync()
    {
        var grp = new Weapongrp { WeaponType = WeaponType.pole, BodyPart = ItemSlot.lrhand };
        var layout = WeaponAttachLayoutResolver.Resolve(grp);
        Assert.IsTrue(layout.IsExtendedTwoHandStance);
        Assert.IsTrue(layout.CausesStanceAnimRefresh);
        Assert.IsFalse(layout.SyncAsDualSameId);
    }

    [Test]
    public void Shield_IsShieldSlot()
    {
        var grp = new Weapongrp { WeaponType = WeaponType.shield, BodyPart = ItemSlot.lhand };
        Assert.IsTrue(WeaponAttachLayoutResolver.Resolve(grp).IsShieldSlot);
    }

    [Test]
    public void EquipTriggersAnimRefresh_OnStanceWeapon()
    {
        var pole = WeaponAttachLayoutResolver.Resolve(new Weapongrp { WeaponType = WeaponType.pole });
        Assert.IsTrue(WeaponAttachLayoutResolver.EquipTriggersAnimRefresh(
            WeaponType.pole, WeaponType.sword, pole, WeaponAttachLayout.Empty, leftHandShieldPresent: false));
    }
}