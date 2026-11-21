using NUnit.Framework;

public sealed class PlaytestConsoleSpamEditModeTests
{
    [Test]
    public void NpcSkinnedBoundsPolicy_uses_collision_height_in_meters_without_extra_scale()
    {
        Assert.AreEqual(1.8f, NpcSkinnedBoundsPolicy.ExpectedBodyHeightMeters(collisionHeightMeters: null));
        Assert.AreEqual(1.0f, NpcSkinnedBoundsPolicy.ExpectedBodyHeightMeters(1.0f));
        Assert.AreEqual(3.35f, NpcSkinnedBoundsPolicy.MaxSkinnedBoundsHeightMeters(1.0f), 0.001f);
    }

    [Test]
    public void NpcChrSoundBankResolver_maps_npc_voice_bank_to_fmod_race_bank()
    {
        Assert.AreEqual("MHFighter", NpcChrSoundBankResolver.MapDamageBankToFmodBank("MNpc_Young"));
        Assert.AreEqual("FHFighter", NpcChrSoundBankResolver.MapDamageBankToFmodBank("FNpc_Young"));
        Assert.AreEqual("MElf", NpcChrSoundBankResolver.MapDamageBankToFmodBank("MNpc_Elf"));
        Assert.AreEqual("MHFighter", NpcChrSoundBankResolver.MapDamageBankToFmodBank("MHFighter"));
    }

    [Test]
    public void AudioPathResolver_NormalizeMonsterSoundFolder_strips_lineage_prefix_and_sm_suffix()
    {
        Assert.AreEqual("dryad", AudioPathResolver.NormalizeMonsterSoundFolder("LineageMonster.dryad"));
        Assert.AreEqual("dryad", AudioPathResolver.NormalizeMonsterSoundFolder("dryad_sm"));
        Assert.AreEqual(
            "event:/MonSound/dryad/death",
            AudioPathResolver.FmodMonsterSound("LineageMonster.dryad", "death"));
    }
}