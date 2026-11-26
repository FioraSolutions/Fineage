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
        float authored = NumberUtils.ScaleToUnity(120f);
        const float official = 13f / 15f;
        const float clipLen = 13f / 24f;
        float rate = LocomotionGait.PlaybackRateForSequence(
            authored, authored, clipLen, official, LocomotionGait.MinRunRate, LocomotionGait.MaxRate);
        float rawAgainstMeters = LocomotionGait.PlaybackRateForSequence(
            120f, authored, clipLen, official, LocomotionGait.MinRunRate, LocomotionGait.MaxRate);
        Assert.AreEqual(15f / 24f, rate, 0.0001f);
        Assert.AreEqual(LocomotionGait.MaxRate, rawAgainstMeters, 0.001f,
            "server L2j units must be converted before they are compared with GroundSpeed in meters");
    }
}