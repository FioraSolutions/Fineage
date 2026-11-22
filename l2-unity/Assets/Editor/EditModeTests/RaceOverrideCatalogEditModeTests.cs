using System.IO;
using NUnit.Framework;
using UnityEngine;

public sealed class RaceOverrideCatalogEditModeTests
{
    [SetUp]
    public void SetUp() => SkillAnimationDatabase.ClearRaceOverrideClipCacheForTests();

    [Test]
    public void Hooked_slots_are_subset_of_catalog()
    {
        foreach (string slot in SkillAnimationResolver.GameplayHookedOverrideSlots)
        {
            bool found = false;
            foreach (string catalogSlot in SkillAnimationDatabase.CatalogOverrideSlots)
            {
                if (catalogSlot == slot)
                {
                    found = true;
                    break;
                }
            }

            Assert.IsTrue(found, "hooked slot missing from catalog: " + slot);
        }
    }

    [Test]
    public void Committed_override_clips_load_via_catalog()
    {
        foreach (string race in SkillAnimationDatabase.AllRaceTokensForCatalog())
        {
            foreach (string slot in SkillAnimationDatabase.CatalogOverrideSlots)
            {
                if (!SkillAnimationDatabase.TryGetOverrideResourcePath(slot, race, out string path))
                    continue;

                if (!File.Exists(ResourcesPathOnDisk(path)))
                    continue;

                AnimationClip clip = SkillAnimationDatabase.GetOverrideClip(slot, race);
                Assert.IsNotNull(
                    clip,
                    "slot=" + slot + " race=" + race + " path=" + path);
            }
        }
    }

    static string ResourcesPathOnDisk(string resourcesPath)
    {
        return Path.Combine(Application.dataPath, "Resources", resourcesPath + ".anim");
    }
}