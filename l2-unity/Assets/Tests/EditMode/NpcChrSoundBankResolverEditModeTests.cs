using NUnit.Framework;

public sealed class NpcChrSoundBankResolverEditModeTests
{
    [Test]
    public void Parses_interlude_damage_sound_chr_bank()
    {
        Assert.IsTrue(NpcChrSoundBankResolver.TryParseChrSoundBankToken(
            "ChrSound.FNpc_Young_Dmg_1",
            out string bank));
        Assert.AreEqual("FNpc_Young", bank);

        Assert.IsTrue(NpcChrSoundBankResolver.TryResolveFromDamageSounds(
            new[] { "[ChrSound.MNpc_Old_Dmg_2]" },
            out bank));
        Assert.AreEqual("MNpc_Old", bank);
    }
}