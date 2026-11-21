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
    public void Shield_block_resolves_primary_item_sound_from_weapongrp()
    {
        var buckler = new Weapongrp
        {
            WeaponType = WeaponType.shield,
            ItemSounds = new[]
            {
                "ItemSound.shield_wood_1",
                "ItemSound.shield_wood_2",
                "ItemSound.shield_steel_8",
                "MonSound.Hit_Metal_Clang_7",
            },
        };

        string path = CombatHitSoundResolver.ResolveShieldBlockFmodEventPathFromData(20, buckler);

        Assert.AreEqual("event:/ItemSound/shield_steel_8", path);
        Assert.IsTrue(InterludeSoundTokenPath.IsCombatSfxEvent(path));
        Assert.IsFalse(InterludeSoundTokenPath.IsUiOrInterfaceEvent(path));
    }

    [Test]
    public void Shield_block_without_item_sound_does_not_fall_back_to_armor_hit()
    {
        var empty = new Weapongrp
        {
            WeaponType = WeaponType.shield,
            ItemSounds = new[] { "", "", "", "" },
        };

        Assert.IsNull(CombatHitSoundResolver.ResolveShieldBlockFmodEventPathFromData(20, empty));
    }

    [Test]
    public void Shield_block_animator_state_matches_anim_tween_shieldatk_1HS()
    {
        Assert.AreEqual("shieldatk_1HS", CombatShieldBlockPresentation.ResolveAnimatorStateName(null));
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