using NUnit.Framework;

public sealed class InterludeSoundResourcesAliasEditModeTests
{
    [Test]
    public void Item_alias_resolves_playtest_missing_stems()
    {
        string path = AudioPathResolver.ResourcesItemSoundRoot + "sword_small_3";
        Assert.IsTrue(InterludeSoundResourcesAlias.TryResolveItemResourcesPath(path, out string alias));
        Assert.AreEqual(AudioPathResolver.ResourcesItemSoundRoot + "armor_underwear_2", alias);
    }

    [Test]
    public void Monster_dryad_death_aliases_to_shipped_clip()
    {
        string path = AudioPathResolver.ResourcesMonSoundRoot + "dryad/death";
        Assert.IsTrue(InterludeSoundResourcesAlias.TryResolveMonsterResourcesPath(path, out string alias));
        StringAssert.StartsWith(AudioPathResolver.ResourcesMonSoundRoot, alias);
    }
}