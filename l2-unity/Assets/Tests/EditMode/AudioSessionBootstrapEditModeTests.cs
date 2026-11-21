using NUnit.Framework;

public sealed class AudioSessionBootstrapEditModeTests
{
    [Test]
    public void ShouldCreatePrefab_when_either_manager_missing()
    {
        Assert.IsTrue(AudioSessionRules.ShouldCreatePrefab(false, true));
        Assert.IsTrue(AudioSessionRules.ShouldCreatePrefab(true, false));
        Assert.IsTrue(AudioSessionRules.ShouldCreatePrefab(false, false));
        Assert.IsFalse(AudioSessionRules.ShouldCreatePrefab(true, true));
    }

    [Test]
    public void Sound_prefab_resource_path_matches_menu_placement()
    {
        Assert.AreEqual("Prefab/Sound", AudioSessionRules.SoundPrefabResourcesPath);
    }

    [Test]
    public void Music_area_exit_uses_allow_fadeout_before_priority_reset()
    {
        Assert.IsTrue(MusicAreaExitFade.UseAllowFadeoutOnAreaExit);
        Assert.IsTrue(MusicAreaExitFade.ShouldResetPriorityOnExit(MusicAreaExitFade.UseAllowFadeoutOnAreaExit));
    }

    [Test]
    public void Required_service_log_key_is_stable_per_caller()
    {
        Assert.AreEqual(
            "AudioManager@L2Footsteps",
            RequiredAudioServiceLog.FormatKey("AudioManager", "L2Footsteps"));
    }
}