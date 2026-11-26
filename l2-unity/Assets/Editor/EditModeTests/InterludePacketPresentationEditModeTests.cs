using NUnit.Framework;
using UnityEngine;

public sealed class InterludePacketPresentationEditModeTests
{
    [Test]
    public void Catalog_resolves_class_change_skill_5103()
    {
        InterludePacketPresentationCatalog.EnsureLoaded();
        Assert.IsTrue(
            InterludePacketPresentationCatalog.TryResolveMagicSkillUse(
                5103, out InterludePresentationKind kind));
        Assert.AreEqual(InterludePresentationKind.LevelUpCelebration, kind);
    }

    [Test]
    public void Catalog_resolves_level_up_social_15()
    {
        InterludePacketPresentationCatalog.EnsureLoaded();
        Assert.IsTrue(
            InterludePacketPresentationCatalog.TryResolveSocialAction(
                15, out InterludePresentationKind kind));
        Assert.AreEqual(InterludePresentationKind.LevelUpCelebration, kind);
    }

    [Test]
    public void Catalog_unknown_skill_returns_false()
    {
        InterludePacketPresentationCatalog.EnsureLoaded();
        Assert.IsFalse(
            InterludePacketPresentationCatalog.TryResolveMagicSkillUse(
                4247, out _));
    }
}