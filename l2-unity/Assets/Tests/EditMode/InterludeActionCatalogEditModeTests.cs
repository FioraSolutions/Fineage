using NUnit.Framework;

/// <summary>
/// Interlude ActionWnd catalog: sections, slot order, icons, social clips per race/gender.
/// </summary>
public sealed class InterludeActionCatalogEditModeTests
{
    [Test]
    public void Basic_section_order_matches_ActionWnd_row()
    {
        int[] ids = InterludeActionCatalog.SectionSlotActionIds(InterludeActionSection.Basic);
        CollectionAssert.AreEqual(
            new[] { 0, 1, 2, 3, 4, 5, 6, 10, 28, 37, 38, 40 },
            ids);
    }

    [Test]
    public void Party_section_order_matches_ActionWnd_row()
    {
        int[] ids = InterludeActionCatalog.SectionSlotActionIds(InterludeActionSection.Party);
        CollectionAssert.AreEqual(new[] { 7, 8, 9, 11, 50 }, ids);
    }

    [Test]
    public void Social_section_order_matches_ActionWnd_rows()
    {
        int[] ids = InterludeActionCatalog.SectionSlotActionIds(InterludeActionSection.Social);
        CollectionAssert.AreEqual(
            new[] { 12, 13, 14, 24, 25, 26, 29, 30, 31, 33, 34, 35, 62, 66 },
            ids);
    }

    [Test]
    public void Catalog_entries_use_expected_sections()
    {
        Assert.IsTrue(InterludeActionCatalog.TryGet(10, out var sell));
        Assert.AreEqual(InterludeActionSection.Basic, sell.Section);
        Assert.IsTrue(InterludeActionCatalog.TryGet(7, out var invite));
        Assert.AreEqual(InterludeActionSection.Party, invite.Section);
        Assert.IsTrue(InterludeActionCatalog.TryGet(12, out var hello));
        Assert.AreEqual(InterludeActionSection.Social, hello.Section);
        Assert.IsTrue(InterludeActionCatalog.TryGet(15, out var petMove));
        Assert.AreEqual(InterludeActionSection.Pet, petMove.Section);
        Assert.IsTrue(InterludeActionCatalog.TryGet(21, out var sumMove));
        Assert.AreEqual(InterludeActionSection.Summon, sumMove.Section);
    }

    [Test]
    public void Action_icons_resolve_for_window_slots()
    {
        foreach (InterludeActionCatalog.Entry e in InterludeActionCatalog.Entries)
        {
            if (e.SlotOrder < 0)
                continue;
            Assert.IsTrue(
                InterludeActionIcons.StemResolvesInResources(e.IconStem),
                "icon " + e.IconStem + " for action id=" + e.Id);
        }
    }

    [Test]
    public void Social_clip_stems_resolve_for_all_races_except_known_gaps()
    {
        int[] socialIds = InterludeActionCatalog.SectionSlotActionIds(InterludeActionSection.Social);
        foreach (int actionId in socialIds)
        {
            Assert.IsTrue(
                InterludeSocialAnimationCatalog.TryGetClipStem(actionId, out string stem),
                "clip stem for social id=" + actionId);

            foreach (var raceRow in InterludeSocialAnimationCatalog.AllRaces())
            {
                bool loaded = InterludeSocialAnimationCatalog.TryLoadClip(raceRow.Race, stem, out _);
                if (raceRow.Race == CharacterRaceAnimation.MFighter)
                {
                    Assert.IsFalse(loaded, "MFighter social clips not imported yet (tracked gap)");
                    continue;
                }

                Assert.IsTrue(
                    loaded,
                    "social id=" + actionId + " race=" + raceRow.Race + " gender=" +
                    InterludeSocialAnimationCatalog.GenderLabel(raceRow.Race) +
                    " clip '" + stem + "'");
            }
        }
    }

    [Test]
    public void Social_action_ids_map_to_ActionType_enum()
    {
        Assert.AreEqual((int)ActionType.SocialGreeting, 12);
        Assert.AreEqual((int)ActionType.SocialShyness, 66);
    }
}