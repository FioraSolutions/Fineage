using System.Collections.Generic;
using NUnit.Framework;

public class OfficialWaveTableEditModeTests
{
    [SetUp]
    public void SetUp()
    {
        OfficialWaveTable.ClearForTests();
    }

    [TearDown]
    public void TearDown()
    {
        OfficialWaveTable.ClearForTests();
    }

    static void LoadSample()
    {
        OfficialWaveTable.LoadFrom(
            "package\tgroup\tname\tresources_path\n"
            + "itemsound\tWeapon_Bow\tbow_big_1\tData/Sounds/ItemSound/bow_big_1\n"
            + "itemsound\tWeapon_Sword\tsword_small_1\tData/Sounds/ItemSound/sword_small_1\n"
            + "EtcSound\tQuest\tquest_accept\tData/Sounds/ItemSound/quest_accept\n"
            + "monsound\tgremlin\tgremlin_atk\tData/Sounds/MonSound/gremlin/gremlin_atk\n"
            + "monsound\tzenta_lizardman_scout\tbow_big_1\tData/Sounds/MonSound/zenta_lizardman_scout/bow_big_1\n"
            + "monsound2\toel_mahum_warrior\toel_w_dmg_1\tData/Sounds/MonSound/oel_mahum_warrior/oel_w_dmg_1\n"
            + "monsound2\toel_mahum_witch_doctor\toel_w_dmg_1\tData/Sounds/MonSound/oel_mahum_witch_doctor/oel_w_dmg_1\n"
            + "chrsound\tFDElf_Basic_Action\tFDElf_Dmg_1\tData/Sounds/Voice/FDElf_Dmg_1\n"
            + "chrsound\tFDElf_Basic_Action\tFDElf_Dmg_2\tData/Sounds/Voice/FDElf_Dmg_2\n"
            + "chrsound\tFDElf_Normal_Attack\tFDElf_1H_Atk_2\tData/Sounds/Voice/FDElf_1H_Atk_2\n"
            + "chrsound\tFNpc_Young\tFNpc_Young_Dmg_1\tData/Sounds/Voice/FNpc_Young_Dmg_1\n"
            + "chrsound\tFHMagician_Social_Action\tFHMagician_Stand\tData/Sounds/Voice/FHMagician_Stand\n",
            "test://official-waves.tsv");
    }

    [Test]
    public void Item_dat_token_resolves_to_item_resources_path()
    {
        LoadSample();
        Assert.IsTrue(OfficialWaveTable.TryResolveDatToken("ItemSound.sword_small_1", out string path, out string error));
        Assert.AreEqual("Data/Sounds/ItemSound/sword_small_1", path);
        Assert.IsNull(error);
    }

    [Test]
    public void EtcSound_token_resolves_to_item_folder()
    {
        LoadSample();
        Assert.IsTrue(OfficialWaveTable.TryResolveDatToken("EtcSound.quest_accept", out string path, out _));
        Assert.AreEqual("Data/Sounds/ItemSound/quest_accept", path);
    }

    [Test]
    public void ChrSound_token_resolves_exact_official_wave()
    {
        LoadSample();
        Assert.IsTrue(OfficialWaveTable.TryResolveDatToken("ChrSound.FNpc_Young_Dmg_1", out string path, out _));
        Assert.AreEqual("Data/Sounds/Voice/FNpc_Young_Dmg_1", path);
    }

    [Test]
    public void Ambiguous_bow_big_1_without_hint_logs_package_and_name()
    {
        LoadSample();
        var logs = new List<string>();
        Assert.IsFalse(OfficialWaveTable.TryResolve("bow_big_1", out _, out string error));
        Assert.IsTrue(error.Contains("ambiguous wave name=\"bow_big_1\""), error);
        Assert.IsTrue(error.Contains("itemsound"), error);
        Assert.IsTrue(error.Contains("monsound"), error);
    }

    [Test]
    public void Ambiguous_name_uses_group_or_context_hint()
    {
        LoadSample();
        Assert.IsTrue(OfficialWaveTable.TryResolve("bow_big_1", out OfficialWaveTable.Entry item, out _, packageHint: "itemsound"));
        Assert.AreEqual("Data/Sounds/ItemSound/bow_big_1", item.ResourcesPath);

        Assert.IsTrue(OfficialWaveTable.TryResolve("oel_w_dmg_1", out OfficialWaveTable.Entry mon, out _, groupHint: "oel_mahum_witch_doctor"));
        Assert.AreEqual("Data/Sounds/MonSound/oel_mahum_witch_doctor/oel_w_dmg_1", mon.ResourcesPath);

        Assert.IsTrue(OfficialWaveTable.TryResolve("oel_w_dmg_1", out OfficialWaveTable.Entry hinted, out _, contextHint: "LineageMonster.oel_mahum_warrior"));
        Assert.AreEqual("Data/Sounds/MonSound/oel_mahum_warrior/oel_w_dmg_1", hinted.ResourcesPath);
    }

    [Test]
    public void Monster_event_uses_official_group_folder()
    {
        LoadSample();
        Assert.IsTrue(OfficialWaveTable.TryResolveMonster("gremlin", "atk", out string path, out string error));
        Assert.AreEqual("Data/Sounds/MonSound/gremlin/gremlin_atk", path);
        Assert.IsNull(error);
    }

    [Test]
    public void Missing_monster_wave_names_package_and_stem()
    {
        LoadSample();
        Assert.IsFalse(OfficialWaveTable.TryResolveMonster("gremlin", "dance", out _, out string error));
        Assert.IsTrue(error.Contains("package=\"monsound\""), error);
        Assert.IsTrue(error.Contains("gremlin_dance"), error);
    }

    [Test]
    public void Character_bank_event_resolves_official_dmg_and_1h_atk()
    {
        LoadSample();
        Assert.IsTrue(OfficialWaveTable.TryResolveCharacter("FDElf", CharacterSoundEvent.Dmg, out var dmg, out _));
        Assert.AreEqual(2, dmg.Count);
        Assert.AreEqual("Data/Sounds/Voice/FDElf_Dmg_1", dmg[0].ResourcesPath);

        Assert.IsTrue(OfficialWaveTable.TryResolveCharacter("FDElf", CharacterSoundEvent.Atk_1H, out var atk, out _));
        Assert.AreEqual("Data/Sounds/Voice/FDElf_1H_Atk_2", atk[0].ResourcesPath);

        Assert.IsTrue(OfficialWaveTable.TryResolveCharacter("FNpc_Young", CharacterSoundEvent.Dmg, out var npc, out _));
        Assert.AreEqual("Data/Sounds/Voice/FNpc_Young_Dmg_1", npc[0].ResourcesPath);
    }

    [Test]
    public void Official_stand_suffix_maps_to_standup()
    {
        LoadSample();
        Assert.IsTrue(OfficialWaveTable.TryResolveCharacter("FHMagician", CharacterSoundEvent.Standup, out var entries, out _));
        Assert.AreEqual("Data/Sounds/Voice/FHMagician_Stand", entries[0].ResourcesPath);
    }

    [Test]
    public void Missing_character_event_logs_bank_and_event()
    {
        LoadSample();
        Assert.IsFalse(OfficialWaveTable.TryResolveCharacter("FDElf", CharacterSoundEvent.Yawn, out _, out string error));
        Assert.IsTrue(error.Contains("package=\"chrsound\""), error);
        Assert.IsTrue(error.Contains("FDElf_Yawn"), error);
    }

    [Test]
    public void SkillCastSound_resolves_item_and_etc_cues_from_official_wave_table()
    {
        OfficialWaveTable.LoadFrom(
            "package\tgroup\tname\tresources_path\n"
            + "itemsound\titemdrop_etc_02\titemdrop_wood\tData/Sounds/ItemSound/itemdrop_wood\n"
            + "ItemSound2\tbroken_key\tbroken_key\tData/Sounds/ItemSound/broken_key\n"
            + "EtcSound\tProject_M\tbattle\tData/Sounds/ItemSound/battle\n"
            + "ItemSound2\tChronicle3_Firework\tC3_Firework_explosion\tData/Sounds/ItemSound/C3_Firework_explosion\n",
            "test://official-waves-skill-cues.tsv");

        string path;
        string error;
        Assert.IsTrue(SkillCastSound.TryResolveEffectClip("ItemSound", "itemdrop_wood", "itemdrop_wood", out path, out error));
        Assert.AreEqual("Data/Sounds/ItemSound/itemdrop_wood", path);
        Assert.IsTrue(SkillCastSound.TryResolveEffectClip("EtcSound", "battle", "battle", out path, out error));
        Assert.AreEqual("Data/Sounds/ItemSound/battle", path);
        Assert.IsTrue(SkillCastSound.TryResolveEffectClip("ItemSound2", "C3_Firework_explosion", "C3_Firework_explosion", out path, out error));
        Assert.AreEqual("Data/Sounds/ItemSound/C3_Firework_explosion", path);
        Assert.IsFalse(SkillCastSound.TryResolveEffectClip("SkillSound2", "antaras_creak", "antaras_creak", out path, out error));
        Assert.IsTrue(error.Contains("antaras_creak"), error);
    }

}
