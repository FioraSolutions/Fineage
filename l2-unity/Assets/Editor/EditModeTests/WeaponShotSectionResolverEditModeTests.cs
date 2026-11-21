using System.Reflection;
using NUnit.Framework;
using UnityEngine;

public sealed class WeaponShotSectionResolverEditModeTests
{
    [Test]
    public void Null_gear_returns_HAND()
    {
        Assert.AreEqual("HAND", WeaponShotSectionResolver.Resolve(null));
    }

    [Test]
    public void Ranged_combat_style_returns_BOW()
    {
        CombatStyleProfile bow = CombatStyleProfileResolver.Resolve(
            new Weapongrp { WeaponType = WeaponType.bow },
            weaponItemId: 14);
        Gear gear = CreateGear(bow);
        Assert.AreEqual("BOW", WeaponShotSectionResolver.Resolve(gear));
        Object.DestroyImmediate(gear.gameObject);
    }

    static Gear CreateGear(CombatStyleProfile style)
    {
        var go = new GameObject("gear_test");
        Gear gear = go.AddComponent<Gear>();
        FieldInfo field = typeof(Gear).GetField("_combatStyle", BindingFlags.Instance | BindingFlags.NonPublic);
        field.SetValue(gear, style);
        return gear;
    }
}