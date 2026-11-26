using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;

/// <summary>
/// #1620 coverage: skillgrp ani_char tags, imported player SpAtk/social clips,
/// and official Interlude takes that are genuinely absent (not substituted).
/// </summary>
public sealed class SkillSocialAnimCoverageEditModeTests
{
    static readonly string[] Races =
    {
        "MFighter", "FFighter", "MMagic", "FMagic", "MElf", "FElf",
        "MDarkElf", "FDarkElf", "MDwarf", "FDwarf", "MOrc", "FOrc",
        "MShaman", "FShaman"
    };

    [Test]
    public void Skillgrp_ani_char_tags_match_AnimationCombo_except_official_MS01()
    {
        HashSet<string> comboTags = ReadComboTags();
        Assert.GreaterOrEqual(comboTags.Count, 31, "AnimationCombo_Interlude tags");

        var unexpected = new List<string>();
        var sawMs01 = false;
        string[] skillgrp = File.ReadAllLines(SkillgrpPath());
        string[] header = skillgrp[0].Split('\t');
        int idCol = Array.IndexOf(header, "skill_id");
        int levelCol = Array.IndexOf(header, "skill_level");
        int aniCol = Array.IndexOf(header, "ani_char");
        Assert.GreaterOrEqual(idCol, 0);
        Assert.GreaterOrEqual(aniCol, 0);

        for (int i = 1; i < skillgrp.Length; i++)
        {
            if (skillgrp[i].Length == 0)
                continue;
            string[] cols = skillgrp[i].Split('\t');
            if (aniCol >= cols.Length)
                continue;
            string tag = cols[aniCol].Trim();
            if (tag.Length == 0)
                continue;
            if (string.Equals(tag, "MS01", StringComparison.Ordinal))
            {
                sawMs01 = true;
                Assert.AreEqual("5006", cols[idCol], "MS01 is official leftover skill 5006");
                Assert.IsFalse(comboTags.Contains(tag), "MS01 must not have an invented combo row");
                continue;
            }

            if (!comboTags.Contains(tag))
                unexpected.Add(cols[idCol] + "\t" + cols[levelCol] + "\t" + tag);
        }

        Assert.IsTrue(sawMs01, "skillgrp still has the official MS01 row");
        CollectionAssert.IsEmpty(unexpected, "unexpected ani_char tags without a combo row");
    }

    [Test]
    public void Imported_SpAtk_clips_parse_and_documented_missing_takes_stay_absent()
    {
        Dictionary<string, HashSet<string>> stems = ScanSpAtkStems();
        Assert.AreEqual(Races.Length, stems.Count);
        foreach (string race in Races)
        {
            Assert.Greater(stems[race].Count, 0, race + " has SpAtk clips");
            foreach (string stem in stems[race])
            {
                Assert.IsTrue(
                    stem.StartsWith("SpAtk", StringComparison.OrdinalIgnoreCase),
                    race + " " + stem);
                int under = stem.IndexOf('_');
                Assert.Greater(under, 0, race + " " + stem);
                int n;
                Assert.IsTrue(
                    int.TryParse(stem.Substring(5, under - 5), out n) && n > 0,
                    "unparsed SpAtk stem " + race + " " + stem);
            }
        }

        List<string[]> missing = ReadMissingTakes();
        Assert.GreaterOrEqual(missing.Count, 6);
        for (int i = 0; i < missing.Count; i++)
        {
            if (missing[i][0] != "spatk")
                continue;
            string race = missing[i][1];
            string stem = missing[i][2];
            Assert.IsTrue(stems.ContainsKey(race), race);
            Assert.IsFalse(
                HasStem(stems[race], stem),
                "documented official-missing take is now on disk; update the TSV: " + race + " " + stem);
        }

        string[] requiredOnFighters = { "SpAtk01_1HS", "SpAtk01_2HS", "SpAtk01_Bow" };
        foreach (string race in new[] { "MFighter", "FFighter", "MElf", "FElf", "MDarkElf", "FDarkElf" })
        {
            for (int i = 0; i < requiredOnFighters.Length; i++)
                Assert.IsTrue(HasStem(stems[race], requiredOnFighters[i]), race + " " + requiredOnFighters[i]);
        }
    }

    [Test]
    public void Every_race_has_the_shared_official_social_takes()
    {
        Dictionary<string, HashSet<string>> social = ScanSocialStems();
        HashSet<string> shared = null;
        foreach (string race in Races)
        {
            Assert.Greater(social[race].Count, 0, race + " social clips");
            if (shared == null)
                shared = new HashSet<string>(social[race], StringComparer.OrdinalIgnoreCase);
            else
                shared.IntersectWith(social[race]);
        }

        Assert.Greater(shared.Count, 0, "shared social takes across all 14 race/gender folders");
        foreach (string race in Races)
        {
            foreach (string stem in shared)
                Assert.IsTrue(HasStem(social[race], stem), race + " missing shared social " + stem);
        }
    }

    static bool HasStem(HashSet<string> stems, string stem)
    {
        if (stems.Contains(stem))
            return true;
        foreach (string have in stems)
        {
            if (string.Equals(have, stem, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }

    static Dictionary<string, HashSet<string>> ScanSpAtkStems()
    {
        return ScanStems(name => name.IndexOf("SpAtk", StringComparison.OrdinalIgnoreCase) >= 0);
    }

    static Dictionary<string, HashSet<string>> ScanSocialStems()
    {
        return ScanStems(name =>
        {
            string lower = name.ToLowerInvariant();
            return lower.IndexOf("social", StringComparison.Ordinal) >= 0
                   || lower.IndexOf("emotion", StringComparison.Ordinal) >= 0;
        });
    }

    static Dictionary<string, HashSet<string>> ScanStems(Func<string, bool> keep)
    {
        var map = new Dictionary<string, HashSet<string>>(StringComparer.Ordinal);
        string animRoot = Path.Combine(Application.dataPath, "Resources", "Data", "Animations");
        foreach (string race in Races)
        {
            var stems = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            string[] dirs = Directory.GetDirectories(animRoot, race, SearchOption.AllDirectories);
            for (int d = 0; d < dirs.Length; d++)
            {
                string clips = Path.Combine(dirs[d], "Clips");
                if (!Directory.Exists(clips))
                    continue;
                string[] files = Directory.GetFiles(clips, "*.anim");
                for (int i = 0; i < files.Length; i++)
                {
                    string file = Path.GetFileNameWithoutExtension(files[i]);
                    if (!keep(file))
                        continue;
                    stems.Add(ClipStem(file, race));
                }
            }

            map[race] = stems;
        }

        return map;
    }

    static string ClipStem(string file, string race)
    {
        string name = file;
        int ao = name.IndexOf(".ao_", StringComparison.OrdinalIgnoreCase);
        if (ao >= 0)
            name = name.Substring(ao + 4);
        string suffix = "_" + race;
        if (name.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
            name = name.Substring(0, name.Length - suffix.Length);
        return name;
    }

    static HashSet<string> ReadComboTags()
    {
        var tags = new HashSet<string>(StringComparer.Ordinal);
        string path = Path.Combine(
            Application.dataPath,
            "StreamingAssets",
            "Data",
            "Meta",
            "AnimationCombo_Interlude.txt");
        string[] lines = File.ReadAllLines(path);
        for (int i = 1; i < lines.Length; i++)
        {
            if (lines[i].Length == 0)
                continue;
            string tag = lines[i].Split('\t')[0].Trim();
            if (tag.Length > 0 && tag[0] == '\ufeff')
                tag = tag.Trim().TrimStart('\ufeff');
            if (tag.Length > 0)
                tags.Add(tag);
        }

        return tags;
    }

    static List<string[]> ReadMissingTakes()
    {
        var rows = new List<string[]>();
        string path = Path.Combine(
            Application.dataPath,
            "..",
            "docs",
            "interlude-ref",
            "animations",
            "skill-social-anim-missing-takes.tsv");
        string[] lines = File.ReadAllLines(path);
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            if (line.Length == 0 || line[0] == '#')
                continue;
            if (line.StartsWith("kind", StringComparison.Ordinal))
                continue;
            rows.Add(line.Split('\t'));
        }

        return rows;
    }

    static string SkillgrpPath()
    {
        return Path.GetFullPath(Path.Combine(
            Application.dataPath,
            "..",
            "docs",
            "interlude-ref",
            "dats",
            "skillgrp.tsv"));
    }
}
