using NUnit.Framework;

public sealed class InterludeMinimapGameTimeEditModeTests
{
    [Test]
    public void FormatHudLabel_matches_interlude_minimap_midnight()
    {
        Assert.AreEqual("AM 12 : 00", InterludeMinimapGameTime.FormatHudLabel(0, 0));
        Assert.IsFalse(InterludeMinimapGameTime.IsDaylightHour(0));
    }

    [Test]
    public void FormatHudLabel_matches_interlude_minimap_afternoon()
    {
        Assert.AreEqual("PM 06 : 05", InterludeMinimapGameTime.FormatHudLabel(18, 5));
        Assert.IsTrue(InterludeMinimapGameTime.IsDaylightHour(18));
    }

    [Test]
    public void IsDaylightHour_matches_interlude_sun_window()
    {
        Assert.IsFalse(InterludeMinimapGameTime.IsDaylightHour(5));
        Assert.IsTrue(InterludeMinimapGameTime.IsDaylightHour(6));
        Assert.IsTrue(InterludeMinimapGameTime.IsDaylightHour(23));
    }
}