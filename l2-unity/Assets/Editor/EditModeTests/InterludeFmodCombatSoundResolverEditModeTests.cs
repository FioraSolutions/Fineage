using NUnit.Framework;

public sealed class InterludeFmodCombatSoundResolverEditModeTests
{
    [Test]
    public void Resolve_dat_token_to_fmod_and_resources_stem()
    {
        Assert.IsTrue(
            InterludeFmodCombatSoundResolver.TryResolveFromDatToken(
                "ItemSound.sword_small_3",
                out InterludeFmodCombatSoundResolver.ResolveResult result));
        Assert.AreEqual("event:/ItemSound/sword_small_3", result.FmodEventPath);
        Assert.AreEqual(
            AudioPathResolver.ResourcesItemSoundRoot + "sword_small_3",
            result.ResourcesPath);
    }

    [Test]
    public void Parse_bank_from_event_path()
    {
        Assert.AreEqual(
            "ItemSound",
            InterludeFmodCombatSoundResolver.ParseBankName("event:/ItemSound/sword_small_3"));
        Assert.AreEqual(
            "MonSound",
            InterludeFmodCombatSoundResolver.ParseBankName("event:/MonSound/dryad/death"));
    }
}