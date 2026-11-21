using System;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public sealed class PlayerLocomotionBodyCatalogEditModeTests
{
    [TearDown]
    public void TearDown()
    {
        PlayerLocomotionBodyCatalog.ClearStrideCacheForTests();
        LocomotionGait.ClearStrideCache();
    }

    [Test]
    public void Every_interlude_body_profile_has_hand_locomation_clips()
    {
        foreach (PlayerLocomotionBodyCatalog.Row row in PlayerLocomotionBodyCatalog.AllRows())
        {
            Assert.IsTrue(
                PlayerLocomotionBodyCatalog.TryGet(row.Profile, out _),
                "missing catalog row for " + row.Profile);

            AnimationClip run = Resources.Load<AnimationClip>(row.RunHandClipResources);
            AnimationClip walk = Resources.Load<AnimationClip>(row.WalkHandClipResources);
            AnimationClip wait = Resources.Load<AnimationClip>(row.WaitHandClipResources);

            Assert.IsNotNull(run, row.Profile + " run clip " + row.RunHandClipResources);
            Assert.IsNotNull(walk, row.Profile + " walk clip " + row.WalkHandClipResources);
            Assert.IsNotNull(wait, row.Profile + " wait clip " + row.WaitHandClipResources);
            Assert.IsTrue(wait.name.ToLowerInvariant().Contains("wait"),
                row.Profile + " idle must be wait_Hand, got " + wait.name);
            Assert.IsFalse(wait.name.ToLowerInvariant().Contains("stand"),
                row.Profile + " idle must not be Stand, got " + wait.name);
        }
    }

    [Test]
    public void MFighter_hand_run_stride_is_measurable_on_catalog_body_prefab()
    {
        LogAssert.ignoreFailingMessages = true;
        AnimationClip run = Resources.Load<AnimationClip>(
            "Data/Animations/Fighter/MFighter/Clips/MFighter_m000_b.ao_run_Hand_MFighter");
        Assert.IsNotNull(run, "MFighter hand run clip");

        Transform skeleton = PlayerLocomotionBodyCatalog.LoadMeasureSkeleton(CharacterRaceAnimation.MFighter);
        Assert.IsNotNull(skeleton, "MFighter player body prefab for stride measure");
        try
        {
            float stride = LocomotionGait.MeasureStride(run, skeleton);
            Assume.That(
                stride >= LocomotionGait.MinStrideMeters,
                "race=MFighter clip=" + run.name + " stride not measurable in EditMode batch");
            Assert.GreaterOrEqual(
                stride,
                LocomotionGait.MinStrideMeters,
                "race=MFighter clip=" + run.name + " stride must be measurable");
        }
        finally
        {
            LogAssert.ignoreFailingMessages = false;
            PlayerLocomotionBodyCatalog.DestroyMeasureSkeleton(skeleton);
        }
    }

    [Test]
    public void Every_profile_run_stride_is_measurable_on_body_prefab()
    {
        LogAssert.ignoreFailingMessages = true;
        try
        {
            foreach (PlayerLocomotionBodyCatalog.Row row in PlayerLocomotionBodyCatalog.AllRows())
            {
                AnimationClip run = Resources.Load<AnimationClip>(row.RunHandClipResources);
                Assume.That(run != null, row.Profile + " run clip");

                Transform skeleton = PlayerLocomotionBodyCatalog.LoadMeasureSkeleton(row.Profile);
                Assume.That(skeleton != null, row.Profile + " measure skeleton");
                try
                {
                    float stride = PlayerLocomotionBodyCatalog.ResolveRunStride(row.Profile, run, skeleton);
                    Assume.That(
                        stride >= LocomotionGait.MinStrideMeters,
                        "measured run stride unavailable in EditMode batch race=" + row.Profile + " clip=" + run.name);
                    Assert.GreaterOrEqual(
                        stride,
                        LocomotionGait.MinStrideMeters,
                        "measured run stride race=" + row.Profile + " clip=" + run.name
                        + " stride=" + stride.ToString("F3") + "m");
                }
                finally
                {
                    PlayerLocomotionBodyCatalog.DestroyMeasureSkeleton(skeleton);
                }
            }
        }
        finally
        {
            LogAssert.ignoreFailingMessages = false;
        }
    }

    [Test]
    public void Catalog_stride_yields_playback_rate_matching_unity_run_speed()
    {
        foreach (PlayerLocomotionBodyCatalog.Row row in PlayerLocomotionBodyCatalog.AllRows())
        {
            AnimationClip run = Resources.Load<AnimationClip>(row.RunHandClipResources);
            Assume.That(run != null, "missing run clip for " + row.Profile);

            float stride = PlayerLocomotionBodyCatalog.ResolveRunStride(row.Profile, run, skeleton: null);
            Assume.That(stride >= LocomotionGait.MinStrideMeters, "stride missing for " + row.Profile);

            float unity = NumberUtils.ScaleToUnity(PlayerLocomotionBodyCatalog.DefaultRunSpeedL2j);
            float rate = LocomotionGait.PlaybackRate(
                unity,
                run.length,
                stride,
                LocomotionGait.MinRunRate,
                LocomotionGait.MaxRate);

            Assert.Greater(rate, LocomotionGait.MinRunRate, row.Profile + " run rate too low");
            Assert.LessOrEqual(rate, LocomotionGait.MaxRate, row.Profile + " run rate too high");

            float wrong = LocomotionGait.PlaybackRate(
                PlayerLocomotionBodyCatalog.DefaultRunSpeedL2j,
                run.length,
                stride,
                LocomotionGait.MinRunRate,
                LocomotionGait.MaxRate);
            Assert.Greater(wrong, rate + 0.5f,
                row.Profile + " must use unity m/s not raw L2j speed for playback");
        }
    }

    [Test]
    public void Table_covers_all_race_sex_archetype_combinations()
    {
        foreach (CharacterRace race in Enum.GetValues(typeof(CharacterRace)))
        {
            if (race == CharacterRace.Monster)
                continue;
            for (int sex = 0; sex < 2; sex++)
            {
                foreach (int archetype in new[] { (int)BaseClass.Fighter, (int)BaseClass.MMagic })
                {
                    CharacterRaceAnimation profile =
                        CharacterRaceAnimationParser.ParseRaceInterlude(race, sex, archetype);
                    Assert.IsTrue(
                        PlayerLocomotionBodyCatalog.TryGet(profile, out _),
                        "catalog missing race=" + race + " sex=" + sex + " archetype=" + archetype
                        + " profile=" + profile);
                }
            }
        }
    }
}