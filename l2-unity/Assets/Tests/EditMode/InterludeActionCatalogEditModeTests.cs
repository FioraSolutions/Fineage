using NUnit.Framework;

/// <summary>
/// Interlude ActionWnd catalog is driven by ActionName-Interlude-eu.txt
/// (converted from system/actionname-e.dat).
/// </summary>
public sealed class InterludeActionCatalogEditModeTests
{
    [Test]
    public void Table_loads_all_interlude_actionname_rows()
    {
        InterludeActionCatalog.EnsureLoaded();
        Assert.AreEqual(102, InterludeActionCatalog.Entries.Count);
    }

    [Test]
    public void Basic_section_order_matches_interlude_dat()
    {
        int[] ids = InterludeActionCatalog.SectionSlotActionIds(InterludeActionSection.Basic);
        CollectionAssert.AreEqual(
            new[] { 0, 1, 2, 3, 4, 5, 6, 10, 28, 37, 38, 40, 51, 55, 57, 58, 59 },
            ids);
    }

    [Test]
    public void Party_section_order_matches_interlude_dat()
    {
        int[] ids = InterludeActionCatalog.SectionSlotActionIds(InterludeActionSection.Party);
        CollectionAssert.AreEqual(new[] { 7, 8, 9, 11, 50, 56, 60 }, ids);
    }

    [Test]
    public void Social_section_order_matches_interlude_dat()
    {
        int[] ids = InterludeActionCatalog.SectionSlotActionIds(InterludeActionSection.Social);
        CollectionAssert.AreEqual(
            new[] { 12, 13, 14, 24, 25, 26, 29, 30, 31, 33, 34, 35 },
            ids);
    }

    [Test]
    public void Catalog_assigns_availability_from_actionname_rows()
    {
        Assert.IsTrue(InterludeActionCatalog.TryGet(38, out var mount));
        Assert.AreEqual(InterludeActionAvailabilityKind.MountablePetSummonedOrMounted, mount.Availability);
        Assert.IsTrue(InterludeActionCatalog.TryGet(3, out var trade));
        Assert.AreEqual(InterludeActionAvailabilityKind.None, trade.Availability);
    }

    [Test]
    public void Catalog_icons_and_names_match_interlude_actionname()
    {
        Assert.IsTrue(InterludeActionNameFile.TryLoad(out InterludeActionNameFile.Row[] rows));
        Assert.AreEqual(102, rows.Length);

        Assert.IsTrue(InterludeActionCatalog.TryGet(3, out var trade));
        Assert.AreEqual("Exchange", trade.CmdName);
        Assert.AreEqual("action005", trade.IconStem);
        Assert.AreEqual(InterludeActionSection.Basic, trade.Section);

        Assert.IsTrue(InterludeActionCatalog.TryGet(4, out var next));
        Assert.AreEqual("action007", next.IconStem);

        Assert.IsTrue(InterludeActionCatalog.TryGet(12, out var hello));
        Assert.AreEqual("Greeting", hello.CmdName);
        Assert.AreEqual(InterludeActionSection.Social, hello.Section);

        Assert.IsTrue(InterludeActionCatalog.TryGet(15, out var petMove));
        Assert.AreEqual(InterludeActionSection.Pet, petMove.Section);
        Assert.AreEqual("action101", petMove.IconStem);

        Assert.IsTrue(InterludeActionCatalog.TryGet(21, out var sumMove));
        Assert.AreEqual(InterludeActionSection.Summon, sumMove.Section);
        Assert.AreEqual("action101", sumMove.IconStem);

        Assert.IsFalse(InterludeActionCatalog.TryGet(62, out _));
        Assert.IsFalse(InterludeActionCatalog.TryGet(66, out _));
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
        Assert.AreEqual((int)ActionType.SocialSorrow, 35);
        Assert.AreEqual((int)ActionType.SocialCharm, 62);
        Assert.AreEqual((int)ActionType.SocialShyness, 66);
    }
}
