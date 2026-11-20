using NUnit.Framework;
using UnityEngine;

public sealed class AudioServiceEditModeTests
{
    [TearDown]
    public void TearDown() => AudioClipCache.ClearForTests();

    [Test]
    public void Path_resolver_builds_fmod_chr_sound_id()
    {
        string path = AudioPathResolver.FmodChrSound(CharacterRaceSound.MHFighter, CharacterSoundEvent.Atk_1H);
        Assert.AreEqual("event:/ChrSound/MHFighter/MHFighter_Atk_1H", path);
    }

    [Test]
    public void Path_resolver_classifies_voice_resources()
    {
        AudioPlaySpec spec = AudioPathResolver.SpecForResourcesPath("Data/Sounds/SkillVoice/M_Elf_Shot");
        Assert.AreEqual(AudioCategory.Voice, spec.Category);
        Assert.AreEqual(AudioSourceKind.ResourcesClip, spec.Source);
    }

    [Test]
    public void Clip_cache_logs_concrete_error_on_missing_clip()
    {
        AudioClipCache.LoaderOverride = _ => null;
        bool ok = AudioClipCache.TryLoad("Data/Sounds/SkillSound/missing_clip", 0f, out _, out string error);
        Assert.IsFalse(ok);
        StringAssert.Contains("missing_clip", error);
        StringAssert.Contains("missing Resources clip", error);
    }

    [Test]
    public void Emitter_pool_enforces_voice_limit()
    {
        var pool = new AudioEmitterPool(maxWorld: 4, maxVoice: 2);
        Assert.IsTrue(pool.TryAcquire(AudioCategory.Voice, out _));
        Assert.IsTrue(pool.TryAcquire(AudioCategory.Voice, out _));
        Assert.IsFalse(pool.TryAcquire(AudioCategory.Voice, out _));
        StringAssert.Contains("Voice", pool.FormatPoolExhaustedError(AudioCategory.Voice));
    }

    [Test]
    public void Step_event_key_is_data_driven()
    {
        Assert.AreEqual("grass_run", AudioPathResolver.StepEventKey("grass", running: true));
        Assert.AreEqual("default_walk", AudioPathResolver.StepEventKey("grass", running: false));
    }

    [Test]
    public void Catalog_login_intro_is_fmod_only()
    {
        Assert.IsTrue(AudioSoundCatalog.TryResolve(AudioSoundCatalog.IdLoginIntro, out AudioPlaySpec spec));
        Assert.AreEqual(AudioSourceKind.FmodStudioEvent, spec.Source);
        Assert.AreEqual("event:/Music/l2_lobby/intro", spec.Id);
    }

    [Test]
    public void Catalog_hit_sounds_are_single_fmod_ids()
    {
        Assert.IsTrue(AudioSoundCatalog.TryResolve(AudioSoundCatalog.IdHitCritical, out AudioPlaySpec crit));
        Assert.AreEqual(AudioSourceKind.FmodStudioEvent, crit.Source);
        Assert.AreEqual("event:/SkillSound/critical_hit", crit.Id);

        Assert.IsTrue(AudioSoundCatalog.TryResolve(AudioSoundCatalog.IdHitArmor, out AudioPlaySpec armor));
        Assert.AreEqual("event:/ItemSound/armor_hit_underwear", armor.Id);
    }
}