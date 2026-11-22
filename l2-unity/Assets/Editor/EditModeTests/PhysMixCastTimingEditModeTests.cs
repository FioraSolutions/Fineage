using System;
using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

/// <summary>
/// Targeted PHYS Mix and self skills share MagicCastData. A missing SpAtk take is an error,
/// not a Player_Basic SpAtk01 swing.
/// </summary>
public sealed class PhysMixCastTimingEditModeTests
{
    [Test]
    public void Sonic_blaster_mix_pins_shot_to_server_hit()
    {
        var cast = new MagicCastData();
        cast.SkillId = 6;
        // style 10 is Mix06 (skillgrp). Shot time is inside the loaded clip; the pin
        // moves that frame onto HitTime when nothing is flying.
        cast.Setup(1596f, 0f, new[] { 0.8f }, 0.44f, 99, castStyle: 10, mixCombo: true);

        Assert.AreEqual(6, cast.SkillId);
        Assert.AreEqual(1.596f, cast.HitTime, 0.001f);
        Assert.AreEqual(1.596f, cast.serverTimeToShoot, 0.001f);
        Assert.AreEqual(1.596f, cast.AnimationShotAtSeconds, 0.05f);
        Assert.Greater(cast.SpeedMid, 0f);
        Assert.AreEqual(cast.SpeedMid, cast.SpeedForSpAtkPhase(0), 0.0001f);
        Assert.AreEqual(0, cast.PeekSpAtkPhase());
        cast.CommitSpAtkPhase();
        Assert.AreEqual(1, cast.PeekSpAtkPhase());
    }

    [Test]
    public void Three_phase_mix_maps_each_swing_onto_its_cast_speed()
    {
        var cast = new MagicCastData();
        cast.SkillId = 1;
        cast.Setup(1733f, 0f, new[] { 0.7f, 0.65f, 0.8f }, 0.4f, 50, castStyle: 9, mixCombo: true);

        Assert.AreEqual(1.733f, cast.HitTime, 0.001f);
        Assert.AreEqual(1.733f, cast.AnimationShotAtSeconds, 0.05f);
        Assert.AreEqual(cast.SpeedMid, cast.SpeedForSpAtkPhase(0), 0.0001f);
        Assert.AreEqual(cast.SpeedEnd, cast.SpeedForSpAtkPhase(1), 0.0001f);
        Assert.AreEqual(cast.SpeedShot, cast.SpeedForSpAtkPhase(2), 0.0001f);
        Assert.AreNotEqual(cast.SpeedMid, cast.SpeedShot);
    }

    [Test]
    public void Missing_dual_spatk_does_not_substitute_1hs()
    {
        const string state = "SpAtk18_dual";
        const string race = "MFighter";
        bool clipOnDisk = File.Exists(Path.Combine(
            Application.dataPath,
            "Resources/Data/Animations/Fighter/MFighter/Clips/MFighter_m000_b.ao_SpAtk18_Dual_MFighter.anim"));
        if (!clipOnDisk)
        {
            LogAssert.Expect(
                LogType.Error,
                new Regex("skillId=1801.*race=MFighter.*weapon=dual.*expectedClipStem=SpAtk18_Dual.*path=Resources/"));
        }

        string resolved = SkillAnimationResolver.ResolvePlayableCombatState(
            null,
            state,
            race,
            1801,
            hasState => hasState.EndsWith("_1HS", StringComparison.Ordinal));

        Assert.AreEqual(state, resolved);
        Assert.IsFalse(resolved.EndsWith("_1HS", StringComparison.Ordinal));
        Assert.IsTrue(SkillAnimationResolver.IsCombatBodyState(state));
        Assert.IsFalse(SkillAnimationResolver.IsCombatBodyState("CastMid"));
    }

    [Test]
    public void SpAtk06_hand_mapping_is_logged_when_the_gesture_clip_exists()
    {
        string hand = Path.Combine(
            Application.dataPath,
            "Resources/Data/Animations/Fighter/MFighter/Clips/MFighter_m000_b.ao_SpAtk06_Hand_MFighter.anim");
        if (!File.Exists(hand))
        {
            Assert.Ignore("SpAtk06_Hand clip is not in this tree");
            return;
        }

        LogAssert.Expect(
            LogType.Log,
            new Regex("SpAtk06 maps to the shared hand gesture skillId=8"));
        string resolved = SkillAnimationResolver.ResolvePlayableCombatState(
            null,
            "SpAtk06_dual",
            "MFighter",
            8,
            _ => false);
        Assert.AreEqual("SpAtk06_hand", resolved);
    }

    [Test]
    public void Skillsoundgrp_columns_match_the_interlude_dat()
    {
        string tsvPath = Path.GetFullPath(Path.Combine(
            Application.dataPath, "..", "docs", "interlude-ref", "dats", "skillsoundgrp.tsv"));
        string jsonPath = Path.Combine(Application.dataPath, "Resources/Data/SkillSoundGrp.json");
        Assert.IsTrue(File.Exists(tsvPath), tsvPath);
        Assert.IsTrue(File.Exists(jsonPath), jsonPath);
        string tsv = File.ReadAllText(tsvPath);
        string json = File.ReadAllText(jsonPath);
        int[] ids = { 1, 5, 6, 8, 261, 1177 };
        for (int i = 0; i < ids.Length; i++)
        {
            string row = FindSoundRow(tsv, ids[i]);
            string[] cols = row.Split('\t');
            Assert.GreaterOrEqual(cols.Length, 5, row);
            string slice = JsonSlice(json, ids[i]);
            Assert.That(slice, Does.Contain("\"cast\":\"" + Stem(cols[2]) + "\""));
            Assert.That(slice, Does.Contain("\"shot\":\"" + Stem(cols[3]) + "\""));
            string explode = Stem(cols[4]);
            if (explode.Length > 0)
                Assert.That(slice, Does.Contain("\"explode\":\"" + explode + "\""));
        }
    }

    [Test]
    public void Zero_flying_time_on_a_flying_shot_is_a_missing_field()
    {
        const string desc =
            "skill_id\tlevel\tdesc\n" +
            "6\t1\t\n" +
            "99\t1\t\n" +
            "1177\t1\tskill.el.1177\n";
        const string usk =
            "skill_id\tclass\tflying\tphase\teffect\n" +
            "99\tskill.xx.99\t0\tshot\tat_example_fl\n" +
            "1177\tskill.el.1177\t0.4\tshot\tel_wind_strike_fl\n";

        string error;
        Assert.IsTrue(SkillVisualCatalog.TryParse(desc, usk, out SkillVisualCatalog catalog, out error), error);

        SkillVisualResolution blaster = catalog.Resolve(6, 1, _ => true);
        Assert.AreEqual(SkillVisualStatus.None, blaster.Status);
        Assert.IsFalse(catalog.HasOfficialFlyingPart(6));

        SkillVisualResolution broken = catalog.Resolve(99, 1, _ => true);
        Assert.AreEqual(SkillVisualStatus.Missing, broken.Status);
        Assert.IsTrue(catalog.HasOfficialFlyingPart(99));
        Assert.AreEqual(0, broken.Actions.Count);
        CollectionAssert.Contains(broken.MissingAssets, "phase:shot missing field=FlyingTime effect=at_example_fl");

        SkillVisualResolution wind = catalog.Resolve(1177, 1, _ => true);
        Assert.AreEqual(SkillVisualStatus.Ready, wind.Status);
        Assert.AreEqual(0.4f, wind.FlyingTime, 0.0001f);
        Assert.AreEqual(1, wind.Actions.Count);
        Assert.AreEqual(SkillVisualPartKind.Projectile, wind.Actions[0].Kind);
    }

    static string FindSoundRow(string tsv, int skillId)
    {
        string prefix = skillId + "\t1\t";
        int at = tsv.IndexOf("\n" + prefix, StringComparison.Ordinal);
        if (at < 0 && tsv.StartsWith(prefix, StringComparison.Ordinal))
            at = -1;
        else if (at >= 0)
            at += 1;
        Assert.GreaterOrEqual(at, 0, "skillsoundgrp row " + skillId);
        int end = tsv.IndexOf('\n', at);
        return end < 0 ? tsv.Substring(at) : tsv.Substring(at, end - at);
    }

    static string JsonSlice(string json, int skillId)
    {
        string key = "\"skillId\":" + skillId + ",\"level\":1,";
        int at = json.IndexOf(key, StringComparison.Ordinal);
        Assert.GreaterOrEqual(at, 0, key);
        int len = Math.Min(480, json.Length - at);
        return json.Substring(at, len);
    }

    static string Stem(string token)
    {
        if (string.IsNullOrEmpty(token))
            return string.Empty;
        int dot = token.LastIndexOf('.');
        return dot >= 0 ? token.Substring(dot + 1) : token;
    }
}
