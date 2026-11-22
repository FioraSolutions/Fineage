using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;

public sealed class SpAtkCombatClipEditModeTests
{
    [OneTimeSetUp]
    public void LoadInterludeAnimTables()
    {
        AnimTweenTable.Instance.Initialize();
        InterludeAnimResourceTakeTable.EnsureLoaded();
    }

    static readonly PlayerAnimWeapon[] WeaponTypes =
    {
        PlayerAnimWeapon.Hand,
        PlayerAnimWeapon.OneHS,
        PlayerAnimWeapon.TwoHS,
        PlayerAnimWeapon.Pole,
        PlayerAnimWeapon.Dual,
        PlayerAnimWeapon.Bow,
    };

    [Test]
    public void PlayerSpAtkAnim_resolves_SpAtk10_through_28_with_weapon_suffix()
    {
        for (int number = 10; number <= 28; number++)
        {
            string trigger = "SpAtk" + number + "AnimName_1HS";
            Assert.IsTrue(
                PlayerSpAtkAnim.TryResolve(trigger, out string state),
                "failed to resolve " + trigger);
            Assert.AreEqual("SpAtk" + number.ToString("00") + "_1HS", state);
        }
    }

    [Test]
    public void Mix_combos_expose_spatk_phases_in_column_order()
    {
        AnimationCombo mix01 = new AnimationCombo(
            "Mix01",
            new[] { "SpAtk09AnimName", "SpAtk17AnimName", "SpAtk24AnimName" },
            "-1");
        string[] phases = SkillAnimationDatabase.GetOrderedSpAtkPhases(mix01);
        Assert.AreEqual(3, phases.Length);
        Assert.AreEqual("SpAtk09AnimName", phases[0]);
        Assert.AreEqual("SpAtk17AnimName", phases[1]);
        Assert.AreEqual("SpAtk24AnimName", phases[2]);
        Assert.IsTrue(SkillAnimationDatabase.IsMixComboTag("Mix01"));
    }

    [Test]
    public void Every_animation_combo_classic_tag_resolves_spatk_states_for_all_weapons()
    {
        foreach (AnimationComboRow row in LoadAnimationComboClassic())
        {
            string[] phases = SkillAnimationDatabase.GetOrderedSpAtkPhases(row.Combo);
            if (phases.Length == 0)
                continue;

            if (SkillAnimationDatabase.IsMixComboTag(row.Tag) && phases.Length > 1)
            {
                Assert.AreEqual(
                    phases.Length,
                    CountNonNoneSpAtkColumns(row.Combo),
                    "Mix tag " + row.Tag + " must list SpAtk phases in column order.");
            }

            foreach (string phase in phases)
            {
                if (!TryReadSpAtkNumber(phase, out int wantNumber))
                    continue;
                foreach (PlayerAnimWeapon weapon in WeaponTypes)
                {
                    Assert.IsTrue(
                        SkillAnimationDatabase.TryBuildSpAtkStateName(phase, weapon, out string state),
                        row.Tag + " phase=" + phase + " weapon=" + weapon);
                    AssertSpAtkIndex(state, wantNumber);
                }
            }
        }
    }

    [Test]
    public void Every_animation_combo_classic_tag_loads_committed_spatk_clips_without_spatk01_fallback()
    {
        bool strict = StrictSpAtkClipMatrixEnabled();
        HashSet<string> interludeTakes = strict ? LoadInterludeTakeNames() : null;
        foreach (AnimationComboRow row in LoadAnimationComboClassic())
        {
            string[] phases = SkillAnimationDatabase.GetOrderedSpAtkPhases(row.Combo);
            if (phases.Length == 0)
                continue;

            foreach (string phase in phases)
            {
                if (!TryReadSpAtkNumber(phase, out int wantNumber))
                    continue;
                foreach (string race in SkillAnimationDatabase.AllSpAtkRaceTokens())
                {
                    foreach (PlayerAnimWeapon weapon in WeaponTypes)
                    {
                        Assert.IsTrue(
                            SkillAnimationDatabase.TryBuildSpAtkStateName(phase, weapon, out string state),
                            row.Tag + " phase=" + phase + " weapon=" + weapon);

                        if (!SkillAnimationDatabase.TryGetSpAtkCombatResourcePath(state, race, out string resourcesPath))
                            continue;

                        string diskPath = ResourcesPathOnDisk(resourcesPath);
                        if (!File.Exists(diskPath) && !TryFindClipFileIgnoreCase(diskPath, out diskPath))
                        {
                            // AnimationCombo still names swings the ukx never authored
                            // (SpAtk27/28, and weapon variants a race does not have).
                            // Those are not missing imports. A take that is in
                            // player-anim-takes.tsv must have a committed clip.
                            if (strict && interludeTakes.Contains(TakeName(resourcesPath)))
                            {
                                Assert.Fail(
                                    row.Tag + " race=" + race + " state=" + state
                                    + " missing Interlude take " + TakeName(resourcesPath)
                                    + " path=" + resourcesPath);
                            }

                            continue;
                        }

                        AnimationClip clip = SkillAnimationDatabase.GetSpAtkCombatClip(state, race);
                        Assert.IsNotNull(
                            clip,
                            row.Tag + " race=" + race + " state=" + state + " path=" + resourcesPath);

                        if (wantNumber != 1)
                        {
                            Assert.IsFalse(
                                clip.name.IndexOf("SpAtk01", StringComparison.OrdinalIgnoreCase) >= 0,
                                row.Tag + " race=" + race + " state=" + state
                                + " must not load generic SpAtk01 clip; got " + clip.name);
                        }

                        AssertSpAtkNumberInClipName(clip.name, wantNumber);
                    }
                }
            }
        }
    }

    /// <summary>
    /// After #1622 the MFighter gap import has run. Require every combo cell whose
    /// sequence is an Interlude take, not every weapon variant the combo table can spell.
    /// </summary>
    static bool StrictSpAtkClipMatrixEnabled()
    {
        return File.Exists(Path.Combine(
            Application.dataPath,
            "Resources/Data/Animations/Fighter/MFighter/Clips/MFighter_m000_b.ao_SpAtk18_1HS_MFighter.anim"));
    }

    static HashSet<string> LoadInterludeTakeNames()
    {
        string path = Path.GetFullPath(Path.Combine(
            Application.dataPath, "..", "..", "Docs", "interlude-whitelist", "player-anim-takes.tsv"));
        Assert.IsTrue(File.Exists(path), path);
        var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        string[] lines = File.ReadAllLines(path);
        for (int i = 1; i < lines.Length; i++)
        {
            if (lines[i].Length == 0)
                continue;
            string[] cols = lines[i].Split('\t');
            if (cols.Length > 2 && cols[2].Length > 0)
                names.Add(cols[2]);
        }

        Assert.Greater(names.Count, 0, path);
        return names;
    }

    static string TakeName(string resourcesPath)
    {
        int ao = resourcesPath.LastIndexOf(".ao_", StringComparison.Ordinal);
        return ao >= 0 ? resourcesPath.Substring(ao + 4) : resourcesPath;
    }

    static bool TryFindClipFileIgnoreCase(string canonicalDiskPath, out string found)
    {
        found = null;
        string dir = Path.GetDirectoryName(canonicalDiskPath);
        if (string.IsNullOrEmpty(dir) || !Directory.Exists(dir))
            return false;

        string want = Path.GetFileName(canonicalDiskPath);
        string[] files = Directory.GetFiles(dir, "*.anim");
        for (int i = 0; i < files.Length; i++)
        {
            if (!string.Equals(Path.GetFileName(files[i]), want, StringComparison.OrdinalIgnoreCase))
                continue;
            found = files[i];
            return true;
        }

        return false;
    }

    static string ResourcesPathOnDisk(string resourcesPath)
    {
        return Path.Combine(
            Application.dataPath,
            "Resources",
            resourcesPath + ".anim");
    }

    static void AssertSpAtkIndex(string state, int wantNumber)
    {
        Assert.IsTrue(PlayerSpAtkAnim.TryParse(state, out int got, out _, out _));
        Assert.AreEqual(wantNumber, got, "state=" + state);
        if (wantNumber != 1)
            Assert.IsFalse(state.StartsWith("SpAtk01_", StringComparison.Ordinal));
    }

    static void AssertSpAtkNumberInClipName(string clipName, int wantNumber)
    {
        Assert.IsTrue(
            TryReadSpAtkNumber(clipName, out int got),
            "clipName=" + clipName);
        Assert.AreEqual(wantNumber, got, "clipName=" + clipName);
        if (wantNumber != 1)
        {
            Assert.IsFalse(
                clipName.IndexOf("SpAtk01", StringComparison.OrdinalIgnoreCase) >= 0,
                "clipName=" + clipName);
        }
    }

    static bool TryReadSpAtkNumber(string text, out int number)
    {
        number = 0;
        int at = text.IndexOf("SpAtk", StringComparison.OrdinalIgnoreCase);
        if (at < 0)
            return false;

        int digitStart = at + 5;
        int digitEnd = digitStart;
        while (digitEnd < text.Length && char.IsDigit(text[digitEnd]))
            digitEnd++;

        return digitEnd > digitStart &&
               int.TryParse(text.Substring(digitStart, digitEnd - digitStart), out number);
    }

    static int ReadSpAtkNumber(string phaseToken)
    {
        string probe = phaseToken;
        if (probe.IndexOf("AnimName", StringComparison.OrdinalIgnoreCase) >= 0)
            probe = probe.Replace("AnimName", "", StringComparison.OrdinalIgnoreCase);
        probe += "_1HS";
        Assert.IsTrue(PlayerSpAtkAnim.TryParse(probe, out int number, out _, out _));
        return number;
    }

    static int CountNonNoneSpAtkColumns(AnimationCombo combo)
    {
        string[] cycle = combo.GetAnimCycle();
        int count = 0;
        if (cycle == null)
            return 0;
        for (int i = 0; i < cycle.Length; i++)
        {
            string token = cycle[i];
            if (string.IsNullOrWhiteSpace(token) ||
                string.Equals(token.Trim(), "none", StringComparison.OrdinalIgnoreCase))
                continue;
            if (token.IndexOf("SpAtk", StringComparison.OrdinalIgnoreCase) >= 0)
                count++;
        }

        return count;
    }

    sealed class AnimationComboRow
    {
        public string Tag;
        public AnimationCombo Combo;
    }

    static IEnumerable<AnimationComboRow> LoadAnimationComboClassic()
    {
        string dataPath = Path.Combine(Application.streamingAssetsPath, "Data/Meta/AnimationCombo_Interlude.txt");
        Assert.IsTrue(File.Exists(dataPath), dataPath);

        var rows = new List<AnimationComboRow>();
        using (var reader = new StreamReader(dataPath))
        {
            string line;
            int index = 0;
            while ((line = reader.ReadLine()) != null)
            {
                if (index++ == 0)
                    continue;
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                string[] cols = line.Split('\t');
                if (cols.Length < 4)
                    continue;

                string tag = cols[0];
                rows.Add(new AnimationComboRow
                {
                    Tag = tag,
                    Combo = new AnimationCombo(
                        tag,
                        new[] { ClearComboToken(cols[1]), ClearComboToken(cols[2]), ClearComboToken(cols[3]) },
                        cols.Length > 4 ? cols[4] : string.Empty),
                });
            }
        }

        return rows;
    }

    static string ClearComboToken(string raw)
    {
        if (string.IsNullOrEmpty(raw))
            return string.Empty;
        string stripped = raw.Replace("a,", string.Empty).Replace("\\0", string.Empty);
        int animNameAt = stripped.IndexOf("AnimName", StringComparison.OrdinalIgnoreCase);
        if (animNameAt >= 0)
            stripped = stripped.Remove(animNameAt, "AnimName".Length);
        return stripped;
    }
}