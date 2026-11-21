using NUnit.Framework;

public sealed class SkillPresentationCatalogEditModeTests
{
    [Test]
    public void Wind_strike_and_twist_doom_ids()
    {
        Assert.IsTrue(SkillPresentationCatalog.IsWindStrike(4247));
        Assert.IsTrue(SkillPresentationCatalog.IsTwistDoomPhysical(2046));
        Assert.IsFalse(SkillPresentationCatalog.IsWindStrike(1));
    }
}