using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public sealed class CombatStyleProfileEditModeTests
{
    [Test]
    public void Bare_hand_profile_is_melee_hand_suffix()
    {
        CombatStyleProfile profile = CombatStyleProfileResolver.ResolveBareHand();
        Assert.IsTrue(profile.IsResolved);
        Assert.AreEqual("hand", profile.AnimSuffix);
        Assert.AreEqual(40, profile.AttackRangeUu);
        Assert.IsFalse(profile.IsRanged);
        Assert.IsFalse(profile.IsDualAnimSuffix);
        Assert.AreEqual(CombatStyleProfile.InterludeMeleeHitFraction, profile.HitFractionOverAttackCycle);
    }

    [Test]
    public void Bow_weapongrp_is_ranged_with_interlude_hit_fraction()
    {
        var grp = new Weapongrp { WeaponType = WeaponType.bow };
        CombatStyleProfile profile = CombatStyleProfileResolver.Resolve(grp, weaponItemId: 14);
        Assert.IsTrue(profile.IsResolved);
        Assert.AreEqual("bow", profile.AnimSuffix);
        Assert.AreEqual(500, profile.AttackRangeUu);
        Assert.IsTrue(profile.IsRanged);
        Assert.AreEqual(CombatStyleProfile.InterludeBowHitFraction, profile.HitFractionOverAttackCycle);
    }

    [Test]
    public void Pole_weapongrp_uses_pole_suffix_and_range()
    {
        var grp = new Weapongrp { WeaponType = WeaponType.pole };
        CombatStyleProfile profile = CombatStyleProfileResolver.Resolve(grp, weaponItemId: 97);
        Assert.AreEqual("pole", profile.AnimSuffix);
        Assert.AreEqual(66, profile.AttackRangeUu);
        Assert.IsFalse(profile.IsRanged);
    }

    [Test]
    public void Dual_weapongrp_sets_dual_anim_suffix_flag()
    {
        var grp = new Weapongrp { WeaponType = WeaponType.dual };
        CombatStyleProfile profile = CombatStyleProfileResolver.Resolve(grp, weaponItemId: 2525);
        Assert.AreEqual("dual", profile.AnimSuffix);
        Assert.IsTrue(profile.IsDualAnimSuffix);
    }

    [Test]
    public void Dagger_and_two_hand_sword_meet_melee_defaults()
    {
        var dagger = new Weapongrp { WeaponType = WeaponType.dagger };
        Assert.AreEqual("1HS", CombatStyleProfileResolver.Resolve(dagger, 10).AnimSuffix);
        Assert.AreEqual(40, CombatStyleProfileResolver.Resolve(dagger, 10).AttackRangeUu);

        var twoHand = new Weapongrp { WeaponType = WeaponType.bigword };
        Assert.AreEqual("2HS", CombatStyleProfileResolver.Resolve(twoHand, 51).AnimSuffix);
    }

    [Test]
    public void Missing_weapongrp_returns_unresolved()
    {
        LogAssert.Expect(
            LogType.Error,
            "[World] CombatStyleProfile: missing Weapongrp for weaponItemId=99999 — check Weapongrp table / item id");
        CombatStyleProfile profile = CombatStyleProfileResolver.Resolve(null, weaponItemId: 99999);
        Assert.IsFalse(profile.IsResolved);
        Assert.AreEqual(string.Empty, profile.AnimSuffix);
        Assert.AreEqual(0, profile.AttackRangeUu);
    }
}