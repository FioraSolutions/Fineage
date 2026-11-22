using System.Collections.Generic;
using NUnit.Framework;

public sealed class AnimNotifyTableEditModeTests
{
    [TearDown]
    public void TearDown()
    {
        AnimNotifyTable.ClearForTests();
        AnimNotifySoundResolver.ClearForTests();
        AudioClipCache.ClearForTests();
    }

    const string SampleTable =
        "package\tanimation\tsequence\ttime\tclass\tnotify\tfunction\tvolume\tradius\tsound\tsounds\tsurface_step\tframe_count\trate\n"
        + "DarkElf\tFDarkElf_anim\tWalk_1HS_FDarkElf\t0.0684210509\tAnimNotify_Sound\tAnimNotify_Sound23\t\t250.0\t30\timport:Engine:Sound:default_walk_01\tSound=default_walk_01;DefaultWalkSound=default_walk_01;DefaultWalkSound[1]=default_walk_02;DefaultWalkSound[2]=Default_Walk_03;DefaultRunSound=default_run_01;GrassWalkSound=grass_walk_01;WaterWalkSound=water_shalow_01;DefaultActorWalkSound=Stone_Hard_Walk_01\t1\t19\t24.0\n"
        + "DarkElf\tFDarkElf_anim\tWalk_1HS_FDarkElf\t0.542105258\tAnimNotify_Sound\tAnimNotify_Sound24\t\t250.0\t30\timport:Engine:Sound:default_walk_02\tSound=default_walk_02;DefaultWalkSound=default_walk_01;DefaultRunSound=default_run_01;GrassWalkSound=grass_walk_01;WaterWalkSound=water_shalow_01;DefaultActorWalkSound=Stone_Hard_Walk_01\t1\t19\t24.0\n"
        + "DarkElf\tFDarkElf_anim\tRun_1HS_FDarkElf\t0.193749994\tAnimNotify_Sound\tAnimNotify_Sound5\t\t250.0\t30\timport:Engine:Sound:FDElf_Breath_2\tSound=FDElf_Breath_2;DefaultWalkSound=default_walk_01\t0\t16\t24.0\n"
        + "DarkElf\tFDarkElf_anim\tAtk01_1HS_FDarkElf\t0.528260827\tAnimNotify_Sound\tAnimNotify_Sound160\t\t250.0\t30\timport:Engine:Sound:sword_great_7\tSound=sword_great_7;DefaultWalkSound=default_walk_01\t0\t46\t30.0\n";

    [SetUp]
    public void SetUp()
    {
        AnimNotifyTable.LoadFrom(SampleTable, "test://anim-sequence-notifies.tsv");
    }

    [Test]
    public void Table_loads_official_times_for_walk_sequence()
    {
        Assert.IsTrue(AnimNotifyTable.TryGetBySequence("Walk_1HS_FDarkElf", out var bindings));
        Assert.AreEqual(2, bindings.Count);
        Assert.AreEqual(0.0684210509f, bindings[0].Time, 0.0000001f);
        Assert.AreEqual(0.542105258f, bindings[1].Time, 0.0000001f);
        Assert.IsTrue(bindings[0].SurfaceStep);
        Assert.AreEqual("default_walk_01", bindings[0].Sound);
    }

    [Test]
    public void Resolver_maps_clip_name_to_official_sequence()
    {
        Assert.IsTrue(AnimNotifySequenceResolver.TryResolve("Walk_1HS_FDarkElf", "FDarkElf_anim", out string seq, out string error));
        Assert.IsNull(error);
        Assert.AreEqual("Walk_1HS_FDarkElf", seq);

        Assert.IsTrue(AnimNotifySequenceResolver.TryResolve("Walk_1HS", "FDarkElf_anim", out seq, out error));
        Assert.IsNull(error);
        Assert.AreEqual("Walk_1HS_FDarkElf", seq);
    }

    [Test]
    public void Resolver_logs_when_clip_has_no_official_sequence()
    {
        Assert.IsFalse(AnimNotifySequenceResolver.TryResolve("TotallyUnknownClip", "FDarkElf_anim", out _, out string error));
        StringAssert.Contains("TotallyUnknownClip", error);
        StringAssert.Contains("no official sequence", error);
    }

    [Test]
    public void Surface_step_uses_official_array_not_invented_interval()
    {
        AnimNotifyTable.TryGetBySequence("Walk_1HS_FDarkElf", out var bindings);
        string grass = AnimNotifySoundResolver.ResolveWave(bindings[0], "grass", running: false, out string log);
        Assert.IsNull(log);
        Assert.AreEqual("grass_walk_01", grass);

        string water = AnimNotifySoundResolver.ResolveWave(bindings[0], "water", running: false, out log);
        Assert.AreEqual("water_shalow_01", water);

        string stone = AnimNotifySoundResolver.ResolveWave(bindings[0], "stone", running: false, out log);
        Assert.AreEqual("Stone_Hard_Walk_01", stone);
    }

    [Test]
    public void Non_step_notify_plays_authored_sound_not_surface_array()
    {
        AnimNotifyTable.TryGetBySequence("Run_1HS_FDarkElf", out var bindings);
        Assert.AreEqual(1, bindings.Count);
        Assert.IsFalse(bindings[0].SurfaceStep);
        string wave = AnimNotifySoundResolver.ResolveWave(bindings[0], "grass", running: true, out string log);
        Assert.IsNull(log);
        Assert.AreEqual("FDElf_Breath_2", wave);

        AnimNotifyTable.TryGetBySequence("Atk01_1HS_FDarkElf", out bindings);
        wave = AnimNotifySoundResolver.ResolveWave(bindings[0], "grass", running: false, out log);
        Assert.AreEqual("sword_great_7", wave);
    }

    [Test]
    public void Unknown_surface_tag_uses_default_array_and_names_the_notify()
    {
        AnimNotifyTable.TryGetBySequence("Walk_1HS_FDarkElf", out var bindings);
        string wave = AnimNotifySoundResolver.ResolveWave(bindings[0], "carpet", running: false, out string log);
        Assert.AreEqual("default_walk_01", wave);
        StringAssert.Contains("carpet", log);
        StringAssert.Contains("AnimNotify_Sound23", log);
    }

    [Test]
    public void Run_sequence_detection_is_token_based()
    {
        Assert.IsTrue(AnimNotifySoundResolver.IsRunSequence("Run_1HS_FDarkElf"));
        Assert.IsFalse(AnimNotifySoundResolver.IsRunSequence("Walk_1HS_FDarkElf"));
        Assert.IsFalse(AnimNotifySoundResolver.IsRunSequence("RunicAttack"));
    }

    [Test]
    public void Volume_scale_uses_authored_250_as_unity()
    {
        Assert.AreEqual(1f, AnimNotifySoundResolver.VolumeScale(250f), 0.0001f);
        Assert.AreEqual(0.5f, AnimNotifySoundResolver.VolumeScale(125f), 0.0001f);
        Assert.AreEqual(0f, AnimNotifySoundResolver.VolumeScale(0f), 0.0001f);
        Assert.IsFalse(AnimNotifySoundResolver.TryVolumeScale(float.NaN, out _));
    }

    [Test]
    public void Empty_radius_and_volume_are_not_replaced_with_defaults()
    {
        AnimNotifyTable.LoadFrom(
            "package\tanimation\tsequence\ttime\tclass\tnotify\tfunction\tvolume\tradius\tsound\tsounds\tsurface_step\tframe_count\trate\n"
            + "DarkElf\tFDarkElf_anim\tWalk_Missing\t0.1\tAnimNotify_Sound\tAnimNotify_Sound9\t\t\t\timport:Engine:Sound:default_walk_01\t\t0\t19\t24.0\n",
            "test://missing-radius.tsv");
        Assert.IsTrue(AnimNotifyTable.TryGetBySequence("Walk_Missing", out var bindings));
        Assert.IsTrue(float.IsNaN(bindings[0].Volume));
        Assert.IsTrue(float.IsNaN(bindings[0].Radius));
    }
}
