using NUnit.Framework;

public sealed class LobbyCharCreateStageCatalogEditModeTests
{
    [Test]
    public void Elf_fighter_female_is_logongrp_index_13()
    {
        Assert.IsTrue(LobbyCharCreateStageCatalog.TryPawnIndex("Elf", "Fighter", "Female", out int index));
        Assert.AreEqual(13, index);
    }

    [Test]
    public void Dwarf_race_gallery_has_two_slots()
    {
        Assert.AreEqual(2, LobbyCharCreateStageCatalog.RaceGalleryCount("Dwarf"));
        Assert.IsTrue(LobbyCharCreateStageCatalog.TryRaceBaseIndex("Dwarf", out int baseIndex));
        Assert.AreEqual(24, baseIndex);
    }

    [Test]
    public void Human_race_gallery_has_four_slots()
    {
        Assert.AreEqual(4, LobbyCharCreateStageCatalog.RaceGalleryCount("Human"));
        Assert.AreEqual(8, LobbyCharCreateStageCatalog.ClassGalleryBase("Human", "Fighter"));
        Assert.AreEqual(10, LobbyCharCreateStageCatalog.ClassGalleryBase("Human", "Mystic"));
    }
}