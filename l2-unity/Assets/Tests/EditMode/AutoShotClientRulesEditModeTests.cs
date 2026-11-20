using NUnit.Framework;

/// <summary>
/// Auto-shot shortcut rules and the shared ToggleEffect frame paths used by the bar shine.
/// </summary>
public sealed class AutoShotClientRulesEditModeTests
{
    [Test]
    public void Soul_shot_etc_type_covers_every_shot_kind()
    {
        Assert.IsTrue(AutoShotClientRules.IsShotEtcType("soul_shot"));
        Assert.IsTrue(AutoShotClientRules.IsShotEtcType("Soul_Shot"));
        Assert.IsFalse(AutoShotClientRules.IsShotEtcType("arrow"));
        Assert.IsFalse(AutoShotClientRules.IsShotEtcType("potion"));
        Assert.IsFalse(AutoShotClientRules.IsShotEtcType(null));
        Assert.IsFalse(AutoShotClientRules.IsShotEtcType(""));
    }

    [Test]
    public void Right_click_toggles_auto_left_click_does_not()
    {
        Assert.IsTrue(AutoShotClientRules.RightClickTogglesAuto(true));
        Assert.IsFalse(AutoShotClientRules.RightClickTogglesAuto(false));
    }

    [Test]
    public void ToggleEffect_resource_paths_use_three_digit_indices()
    {
        Assert.AreEqual("Data/UI/ShortCut/skill_time/ToggleEffect001", ToggleEffectFrames.ResourcePath(1));
        Assert.AreEqual("Data/UI/ShortCut/skill_time/ToggleEffect010", ToggleEffectFrames.ResourcePath(10));
        Assert.AreEqual("Data/UI/ShortCut/skill_time/ToggleEffect013", ToggleEffectFrames.ResourcePath(13));
        Assert.AreEqual(13, ToggleEffectFrames.Count);
    }
}
