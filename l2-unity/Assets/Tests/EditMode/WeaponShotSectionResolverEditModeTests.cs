using NUnit.Framework;

public sealed class WeaponShotSectionResolverEditModeTests
{
    [Test]
    public void Null_gear_returns_HAND()
    {
        Assert.AreEqual("HAND", WeaponShotSectionResolver.Resolve(null));
    }
}