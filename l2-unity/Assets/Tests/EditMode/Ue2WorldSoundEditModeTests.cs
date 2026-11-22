using NUnit.Framework;

public sealed class Ue2WorldSoundEditModeTests
{
    [TearDown]
    public void TearDown() => AudioErrorThrottle.ClearForTests();

    [Test]
    public void Footstep_radius_30_is_inaudible_past_one_hundred_times_that_radius()
    {
        const float radiusUu = 30f;
        float outer = Ue2SoundAudibility.OuterRadiusMeters(radiusUu);
        Assert.AreEqual(
            Ue2SoundAudibility.InnerRadiusMeters(radiusUu) * Ue2SoundAudibility.OuterRadiusScale,
            outer,
            0.0001f);
        Assert.IsTrue(Ue2SoundAudibility.IsAudible(0f, radiusUu));
        Assert.IsTrue(Ue2SoundAudibility.IsAudible(Ue2SoundAudibility.InnerRadiusMeters(radiusUu), radiusUu));
        Assert.IsTrue(Ue2SoundAudibility.IsAudible(outer - 0.01f, radiusUu));
        Assert.IsFalse(Ue2SoundAudibility.IsAudible(outer, radiusUu));
    }

    [Test]
    public void Attenuation_is_full_inside_the_inner_radius_and_follows_the_rolloff_equation()
    {
        const float radiusUu = 50f;
        float inner = Ue2SoundAudibility.InnerRadiusMeters(radiusUu);
        Assert.AreEqual(1f, Ue2SoundAudibility.Attenuation(inner * 0.5f, radiusUu), 0.0001f);

        float distance = inner * 4f;
        float expected = 1f / (1f + Ue2SoundAudibility.AlAudioDefaultRolloff * (distance - inner) / inner);
        Assert.AreEqual(expected, Ue2SoundAudibility.Attenuation(distance, radiusUu), 0.0001f);
        Assert.AreEqual(0f, Ue2SoundAudibility.Attenuation(inner * 100f, radiusUu), 0.0001f);
    }

    [Test]
    public void Omitted_actor_sound_radius_uses_the_ue2_actor_default()
    {
        Assert.AreEqual(32f, Ue2SoundAudibility.ActorDefaultSoundRadiusUu, 0.001f);
        Assert.AreEqual(32f, Ue2SoundAudibility.ResolveActorSoundRadiusUu(0f), 0.001f);
        Assert.AreEqual(80f, Ue2SoundAudibility.ResolveActorSoundRadiusUu(80f), 0.001f);
        Assert.IsFalse(Ue2SoundAudibility.HasRadius(0f));
    }

    [Test]
    public void Audio_trace_line_names_the_play_and_stays_off_unless_enabled()
    {
        AudioPlayTrace.EnabledOverride = false;
        Assert.IsFalse(AudioPlayTrace.IsEnabled);
        string line = AudioPlayTrace.Format(
            "refuse", 1.25f, "AmbSound.Harbor.h_seagull_01", "Ambient", "AmbSound01", 21,
            3.5f, 32f, 190f, 4, "actor-default");
        StringAssert.Contains("t=1.250", line);
        StringAssert.Contains("clip=\"AmbSound.Harbor.h_seagull_01\"", line);
        StringAssert.Contains("source=Ambient", line);
        StringAssert.Contains("owner=\"AmbSound01\"", line);
        StringAssert.Contains("id=21", line);
        StringAssert.Contains("radiusUu=32", line);
        StringAssert.Contains("reason=actor-default", line);
        AudioPlayTrace.ClearForTests();
    }

    [Test]
    public void Missing_radius_names_the_clip_and_is_not_audible()
    {
        string error = Ue2SoundAudibility.FormatMissingRadius("DarkElf Walk wave=\"default_walk_01\"");
        StringAssert.Contains("default_walk_01", error);
        StringAssert.Contains("missing audible radius", error);
        Assert.IsFalse(Ue2SoundAudibility.HasRadius(float.NaN));
        Assert.IsFalse(Ue2SoundAudibility.HasRadius(0f));
        Assert.IsFalse(Ue2SoundAudibility.IsAudible(0f, float.NaN));
    }

    [Test]
    public void Closer_louder_sound_has_higher_priority()
    {
        const float radiusUu = 100f;
        float near = Ue2SoundAudibility.Priority(1f, 0f, radiusUu);
        float far = Ue2SoundAudibility.Priority(1f, Ue2SoundAudibility.InnerRadiusMeters(radiusUu) * 10f, radiusUu);
        Assert.Greater(near, far);
    }

    [Test]
    public void Voice_steal_replaces_the_quietest_slot_and_rejects_a_quieter_one()
    {
        var pool = new AudioEmitterPool(maxWorld: 2, maxVoice: 1);
        Assert.IsTrue(pool.TryClaim(AudioCategory.Sfx, 0.2f, 1, Ue2SoundAudibility.SlotMisc, out int quiet, out bool replaced, out _));
        Assert.IsFalse(replaced);
        Assert.IsTrue(pool.TryClaim(AudioCategory.Sfx, 0.8f, 2, Ue2SoundAudibility.SlotMisc, out _, out _, out _));
        Assert.AreEqual(2, pool.ActiveWorld);

        Assert.IsFalse(pool.TryClaim(AudioCategory.Sfx, 0.1f, 3, Ue2SoundAudibility.SlotMisc, out _, out _, out bool rejected));
        Assert.IsTrue(rejected);
        Assert.AreEqual(2, pool.ActiveWorld);

        Assert.IsTrue(pool.TryClaim(AudioCategory.Sfx, 0.5f, 4, Ue2SoundAudibility.SlotMisc, out int stolen, out bool stole, out rejected));
        Assert.IsTrue(stole);
        Assert.IsFalse(rejected);
        Assert.AreEqual(quiet, stolen);
        Assert.AreEqual(0.5f, pool.PriorityOf(AudioCategory.Sfx, stolen), 0.0001f);
        Assert.AreEqual(2, pool.ActiveWorld);
    }

    [Test]
    public void Same_actor_slot_replaces_instead_of_taking_another_voice()
    {
        var pool = new AudioEmitterPool(maxWorld: 4, maxVoice: 1);
        Assert.IsTrue(pool.TryClaim(AudioCategory.Sfx, 0.4f, 7, Ue2SoundAudibility.SlotMisc, out int first, out _, out _));
        Assert.IsTrue(pool.TryClaim(AudioCategory.Sfx, 0.9f, 7, Ue2SoundAudibility.SlotMisc, out int second, out bool replaced, out _));
        Assert.IsTrue(replaced);
        Assert.AreEqual(first, second);
        Assert.AreEqual(1, pool.ActiveWorld);
        Assert.AreEqual(0.9f, pool.PriorityOf(AudioCategory.Sfx, first), 0.0001f);

        Assert.IsTrue(pool.TryClaim(AudioCategory.Sfx, 0.3f, 7, Ue2SoundAudibility.SlotTalk, out int talk, out replaced, out _));
        Assert.IsFalse(replaced);
        Assert.AreNotEqual(first, talk);
        Assert.AreEqual(2, pool.ActiveWorld);
    }

    [Test]
    public void Acquire_without_priority_still_stops_at_the_cap()
    {
        var pool = new AudioEmitterPool(maxWorld: 4, maxVoice: 2);
        Assert.IsTrue(pool.TryAcquire(AudioCategory.Voice, out _));
        Assert.IsTrue(pool.TryAcquire(AudioCategory.Voice, out _));
        Assert.IsFalse(pool.TryAcquire(AudioCategory.Voice, out _));
        StringAssert.Contains("Voice", pool.FormatPoolExhaustedError(AudioCategory.Voice));
    }

    [Test]
    public void Error_throttle_emits_once_per_window_and_once_keys_stay_silent()
    {
        Assert.IsTrue(AudioErrorThrottle.TryEmit("pool", 0f, once: false, out _));
        Assert.IsFalse(AudioErrorThrottle.TryEmit("pool", 0.2f, once: false, out _));
        Assert.IsFalse(AudioErrorThrottle.TryEmit("pool", 0.4f, once: false, out _));
        Assert.IsTrue(AudioErrorThrottle.TryEmit("pool", 1f, once: false, out string suffix));
        StringAssert.Contains("repeated 2", suffix);

        Assert.IsTrue(AudioErrorThrottle.TryEmit("radius:walk", 0f, once: true, out _));
        Assert.IsFalse(AudioErrorThrottle.TryEmit("radius:walk", 10f, once: true, out _));
    }

    [Test]
    public void Hitch_larger_than_the_authored_step_does_not_replay_the_range()
    {
        AnimNotifyPlaybackWindow.Result hitch = AnimNotifyPlaybackWindow.Step(
            hasLast: true, lastNormalized: 0.05f, cycle: 0.95f, deltaSeconds: 0.016f, rate: 30f, frameCount: 30);
        Assert.IsFalse(hitch.Emit);
        Assert.IsNull(hitch.Error);
        Assert.AreEqual(0.95f, hitch.Cursor, 0.0001f);

        AnimNotifyPlaybackWindow.Result slow = AnimNotifyPlaybackWindow.Step(
            hasLast: true, lastNormalized: 0.05f, cycle: 0.95f, deltaSeconds: 1f, rate: 30f, frameCount: 30);
        Assert.IsTrue(slow.Emit);
        Assert.IsTrue(AnimNotifyPlaybackWindow.ShouldPlayOnlyLatest(slow.Span, slow.OneFrame));
    }

    [Test]
    public void Entering_mid_cycle_does_not_fire_notifies_already_passed()
    {
        AnimNotifyPlaybackWindow.Result entered = AnimNotifyPlaybackWindow.Step(
            hasLast: false, lastNormalized: 0f, cycle: 0.4f, deltaSeconds: 0.016f, rate: 24f, frameCount: 19);
        Assert.IsFalse(entered.Emit);

        AnimNotifyPlaybackWindow.Result start = AnimNotifyPlaybackWindow.Step(
            hasLast: false, lastNormalized: 0f, cycle: 0f, deltaSeconds: 0.016f, rate: 24f, frameCount: 19);
        Assert.IsTrue(start.Emit);
    }

    [Test]
    public void Missing_sequence_rate_is_a_named_error_and_does_not_emit()
    {
        AnimNotifyPlaybackWindow.Result missing = AnimNotifyPlaybackWindow.Step(
            hasLast: true, lastNormalized: 0.1f, cycle: 0.2f, deltaSeconds: 0.016f, rate: 0f, frameCount: 0);
        Assert.IsFalse(missing.Emit);
        StringAssert.Contains("missing rate or frameCount", missing.Error);
    }
}
