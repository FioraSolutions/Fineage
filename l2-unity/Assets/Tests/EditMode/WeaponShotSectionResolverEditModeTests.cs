using NUnit.Framework;

public sealed class WeaponShotSectionResolverEditModeTests
{
    [Test]
    public void Bare_hands_returns_HAND()
    {
        Assert.AreEqual(
            "HAND",
            WeaponShotSectionResolver.Resolve(
                CombatStyleProfile.BareHand,
                WeaponType.none,
                WeaponType.none));
    }

    [Test]
    public void Ranged_combat_style_returns_BOW()
    {
        CombatStyleProfile bow = CombatStyleProfileResolver.Resolve(
            new Weapongrp { WeaponType = WeaponType.bow },
            weaponItemId: 14);
        Assert.AreEqual(
            "BOW",
            WeaponShotSectionResolver.Resolve(bow, WeaponType.bow, WeaponType.arrow));
    }
}