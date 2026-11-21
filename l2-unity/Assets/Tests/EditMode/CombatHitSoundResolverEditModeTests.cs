using NUnit.Framework;

public sealed class CombatHitSoundResolverEditModeTests
{
    [Test]
    public void Sword_hit_on_gremlin_resolves_item_or_monsound_not_ui()
    {
        var sword = new Weapongrp
        {
            WeaponType = WeaponType.sword,
            ObjectId = 1,
            ItemSounds = new[]
            {
                "ItemSound.sword_small_1",
                "ItemSound.public_sword_shing_4",
                "ItemSound.sword_small_7",
                "ItemSound.sword_small_2",
            },
        };

        var gremlin = new Npcgrp
        {
            NpcId = 20001,
            DefenseSounds = new[]
            {
                "MonSound.Hit_Wet_4",
                "MonSound.Hit_normal_3",
                "MonSound.Hit_Bone_8",
            },
        };

        string path = CombatHitSoundResolver.ResolveFmodEventPathFromData(
            criticalHit: false,
            sword,
            gremlin,
            weaponItemIdForLog: 1);

        Assert.IsFalse(InterludeSoundTokenPath.IsUiOrInterfaceEvent(path));
        Assert.IsTrue(InterludeSoundTokenPath.IsCombatSfxEvent(path));
        StringAssert.StartsWith("event:/ItemSound/", path);
        StringAssert.Contains("sword_small", path);
    }

    [Test]
    public void Critical_hit_uses_skill_sound_event()
    {
        string path = CombatHitSoundResolver.ResolveFmodEventPathFromData(
            criticalHit: true,
            weaponGrp: null,
            targetNpc: null,
            weaponItemIdForLog: 0);

        Assert.AreEqual("event:/SkillSound/critical_hit", path);
        Assert.IsTrue(InterludeSoundTokenPath.IsCombatSfxEvent(path));
        Assert.IsFalse(InterludeSoundTokenPath.IsUiOrInterfaceEvent(path));
    }

    [Test]
    public void Item_sound_token_maps_to_fmod_event_path()
    {
        Assert.IsTrue(InterludeSoundTokenPath.TryFromDatToken("ItemSound.sword_big_9", out string path));
        Assert.AreEqual("event:/ItemSound/sword_big_9", path);
        Assert.IsFalse(InterludeSoundTokenPath.IsUiOrInterfaceEvent(path));
    }

    [Test]
    public void Interface_token_is_rejected_for_combat()
    {
        Assert.IsFalse(InterludeSoundTokenPath.TryFromDatToken("InterfaceSound.window_open", out _));
        Assert.IsTrue(InterludeSoundTokenPath.IsUiOrInterfaceEvent("event:/InterfaceSound/window_open"));
        Assert.IsFalse(InterludeSoundTokenPath.IsCombatSfxEvent("event:/InterfaceSound/window_open"));
    }

    [Test]
    public void Mob_defense_sound_used_when_attacker_has_no_weapon_grp()
    {
        var gremlin = new Npcgrp
        {
            DefenseSounds = new[] { "MonSound.Hit_Wet_4" },
        };

        string path = CombatHitSoundResolver.ResolveFmodEventPathFromData(
            criticalHit: false,
            weaponGrp: null,
            gremlin,
            weaponItemIdForLog: 0);

        Assert.AreEqual("event:/MonSound/Hit_Wet_4", path);
        Assert.IsFalse(InterludeSoundTokenPath.IsUiOrInterfaceEvent(path));
    }
}