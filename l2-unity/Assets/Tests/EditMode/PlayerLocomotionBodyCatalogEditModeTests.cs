using System;
using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public sealed class PlayerLocomotionBodyCatalogEditModeTests
{
    [SetUp]
    public void SetUp()
    {
        LocomotionStrideCatalog.ClearForTests();
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
    public void Every_player_race_sex_has_baked_walk_and_run_stride()
    {
        foreach (PlayerLocomotionBodyCatalog.Row row in PlayerLocomotionBodyCatalog.AllRows())
        {
            AssertBaked(row.Profile, row.RunHandClipResources, "run");
            AssertBaked(row.Profile, row.WalkHandClipResources, "walk");
        }
    }

    [Test]
    public void Every_weapon_walk_and_run_clip_in_the_body_folder_has_stride_data()
    {
        foreach (PlayerLocomotionBodyCatalog.Row row in PlayerLocomotionBodyCatalog.AllRows())
        {
            string absDir = AbsoluteResourcesDir(row.RunHandClipResources);
            Assert.IsTrue(Directory.Exists(absDir), row.Profile + " clips dir " + absDir);
            string[] anims = Directory.GetFiles(absDir, "*.anim");
            int gaits = 0;
            for (int i = 0; i < anims.Length; i++)
            {
                string name = Path.GetFileName(anims[i]);
                string gait = GaitOf(name);
                if (gait == null)
                    continue;
                gaits++;
                string sidecar = anims[i].Substring(0, anims[i].Length - ".anim".Length) + ".stride.json";
                Assert.IsTrue(File.Exists(sidecar),
                    row.Profile + " " + gait + " missing stride sidecar " + sidecar);
                float meters = ReadStride(sidecar);
                Assert.GreaterOrEqual(meters, LocomotionGait.MinStrideMeters,
                    row.Profile + " " + gait + " " + name);
                Assert.IsTrue(LocomotionStrideCatalog.TryGet(Path.GetFileNameWithoutExtension(name), out float indexed),
                    "index missing " + name);
                Assert.AreEqual(meters, indexed, 0.0001f, name + " sidecar vs index");
            }

            Assert.GreaterOrEqual(gaits, 2, row.Profile + " needs walk and run clips");
        }
    }

    [Test]
    public void Baked_stride_playback_rate_is_speed_over_authored_speed()
    {
        foreach (PlayerLocomotionBodyCatalog.Row row in PlayerLocomotionBodyCatalog.AllRows())
        {
            AnimationClip run = Resources.Load<AnimationClip>(row.RunHandClipResources);
            Assert.IsNotNull(run, row.Profile + " run");
            Assert.IsTrue(LocomotionStrideCatalog.TryGet(run.name, out float stride), row.Profile + " stride");

            float unity = NumberUtils.ScaleToUnity(PlayerLocomotionBodyCatalog.DefaultRunSpeedL2j);
            float rate = LocomotionGait.PlaybackRate(
                unity,
                run.length,
                stride,
                LocomotionGait.MinRunRate,
                LocomotionGait.MaxRate);

            Assert.AreEqual(unity * run.length / stride, rate, 0.001f, row.Profile + " rate = speed / (stride/length)");
            Assert.Greater(rate, LocomotionGait.MinRunRate, row.Profile + " run rate too low");
            Assert.LessOrEqual(rate, LocomotionGait.MaxRate, row.Profile + " run rate too high");
        }
    }

    [Test]
    public void Missing_stride_log_names_race_sex_and_gait()
    {
        LogAssert.Expect(
            LogType.Error,
            new Regex("missing stride race=Fighter sex=M profile=MFighter gait=run clip=\"run_not_a_weapon\""));
        LocomotionStrideCatalog.LogMissing(
            "race=Fighter sex=M profile=MFighter",
            "run",
            "run_not_a_weapon");
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

    static void AssertBaked(CharacterRaceAnimation profile, string resourcesPath, string gait)
    {
        string abs = Path.Combine(Application.dataPath, "Resources", resourcesPath + ".stride.json");
        Assert.IsTrue(File.Exists(abs), profile + " " + gait + " sidecar " + abs);
        float meters = ReadStride(abs);
        Assert.GreaterOrEqual(meters, LocomotionGait.MinStrideMeters,
            "race/sex profile=" + profile + " gait=" + gait + " stride=" + meters);

        string clip = Path.GetFileName(resourcesPath);
        Assert.IsTrue(LocomotionStrideCatalog.TryGet(clip, out float indexed),
            profile + " " + gait + " missing from locomotion_strides.json");
        Assert.AreEqual(meters, indexed, 0.0001f, profile + " " + gait);
    }

    static float ReadStride(string sidecarPath)
    {
        var dto = JsonUtility.FromJson<StrideDto>(File.ReadAllText(sidecarPath));
        Assert.IsNotNull(dto, sidecarPath);
        return dto.strideMeters;
    }

    static string AbsoluteResourcesDir(string resourcesFile)
    {
        string rel = resourcesFile.Replace('\\', '/');
        int slash = rel.LastIndexOf('/');
        string dir = slash >= 0 ? rel.Substring(0, slash) : "";
        return Path.Combine(Application.dataPath, "Resources", dir);
    }

    static string GaitOf(string fileName)
    {
        string n = fileName.ToLowerInvariant();
        string[] skip =
        {
            "atk", "wait", "social", "cast", "death", "sit", "stun", "damage",
            "jump", "fall", "swim", "turn", "stand"
        };
        for (int i = 0; i < skip.Length; i++)
        {
            if (n.Contains(skip[i]))
                return null;
        }

        if (Regex.IsMatch(n, @"(^|[_.])walk([_.]|$)") || n.Contains(".ao_walk"))
            return "walk";
        if (Regex.IsMatch(n, @"(^|[_.])run([_.]|$)") || n.Contains(".ao_run"))
            return "run";
        return null;
    }

    [Serializable]
    sealed class StrideDto
    {
        public float strideMeters;
    }
}
