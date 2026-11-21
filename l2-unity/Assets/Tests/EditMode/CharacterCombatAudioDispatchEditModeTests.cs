using NUnit.Framework;
using UnityEngine;

public sealed class CharacterCombatAudioDispatchEditModeTests
{
    [Test]
    public void Dispatch_resolves_chr_bank_path_for_damage()
    {
        string path = AudioPathResolver.FmodChrSoundBank("MHFighter", CharacterSoundEvent.Dmg);
        Assert.AreEqual("event:/ChrSound/MHFighter/MHFighter_Dmg", path);
    }

    [Test]
    public void Dispatch_resolves_monster_fmod_path()
    {
        string path = AudioPathResolver.FmodMonsterSound("young_fox", "death");
        Assert.AreEqual("event:/MonSound/young_fox/death", path);
    }

    [Test]
    public void Locomotion_playback_uses_unity_planar_speed_not_server_raw_units()
    {
        float serverRun = 120f;
        float unity = NumberUtils.ScaleToUnity(serverRun);
        float clipLen = 1f;
        const float stride = 1.5f;
        float rate = LocomotionGait.PlaybackRate(
            unity, clipLen, stride, LocomotionGait.MinRunRate, LocomotionGait.MaxRate);
        float wrongRate = LocomotionGait.PlaybackRate(
            serverRun, clipLen, stride, LocomotionGait.MinRunRate, LocomotionGait.MaxRate);
        Assert.Greater(wrongRate, rate * 10f,
            "Writing server L2j speed into run_speed would overshoot stride-based playback.");
        Assert.AreEqual(unity * clipLen / stride, rate, 0.001f);
    }
}