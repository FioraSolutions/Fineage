using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;

/// <summary>
/// Learnable player skills must keep a resolvable original motion for every
/// race and sex that can learn them. Known gaps (the Interlude ukx has no take,
/// or a clip is not imported yet) live in skill-parity-known-gaps.txt, which
/// tools/skill-parity/generate_skill_parity.py rewrites. A new miss fails.
/// </summary>
public sealed class SkillParityEditModeTests
{
    [Test]
    public void Learnable_skills_resolve_original_motion_or_known_gap()
    {
        string root = RepoRoot();
        string motionsPath = Path.Combine(root, "l2-unity/docs/interlude-ref/skill-parity-motions.tsv");
        string allowPath = Path.Combine(root, "l2-unity/docs/interlude-ref/skill-parity-known-gaps.txt");
        string treesPath = Path.Combine(root, "l2-unity/docs/interlude-ref/skills/player-skill-trees.json");
        string skillgrpPath = Path.Combine(root, "l2-unity/docs/interlude-ref/dats/skillgrp.tsv");
        string animRoot = Path.Combine(root, "l2-unity/Assets/Resources/Data/Animations");

        Assert.IsTrue(File.Exists(motionsPath), motionsPath);
        Assert.IsTrue(File.Exists(allowPath), allowPath);
        Assert.IsTrue(File.Exists(treesPath), treesPath);
        Assert.IsTrue(File.Exists(skillgrpPath), skillgrpPath);

        HashSet<string> shipped = ScanTakes(animRoot);
        Assert.Greater(shipped.Count, 0, "no animation takes under Resources");

        var allow = new HashSet<string>(StringComparer.Ordinal);
        string[] allowLines = File.ReadAllLines(allowPath);
        for (int i = 0; i < allowLines.Length; i++)
        {
            string line = allowLines[i].Trim();
            if (line.Length == 0 || line[0] == '#')
                continue;
            string[] parts = line.Split('|');
            Assert.GreaterOrEqual(parts.Length, 3, line);
            allow.Add(parts[0] + "|" + parts[1] + "|" + parts[2]);
        }

        var learnedWithMotion = new HashSet<string>(StringComparer.Ordinal);
        var failures = new List<string>();
        string[] motionLines = File.ReadAllLines(motionsPath);
        Assert.Greater(motionLines.Length, 1, "skill-parity-motions.tsv");
        for (int i = 1; i < motionLines.Length; i++)
        {
            if (motionLines[i].Length == 0)
                continue;
            string[] cols = motionLines[i].Split('\t');
            if (cols.Length < 4)
            {
                failures.Add("short row " + motionLines[i]);
                continue;
            }

            string skillId = cols[0];
            string body = cols[1];
            string expected = cols[2];
            string kind = cols[3];
            learnedWithMotion.Add(skillId);
            if (kind == "ok")
            {
                if (!shipped.Contains(expected.ToLowerInvariant()))
                    failures.Add("missing shipped clip skill=" + skillId + " body=" + body + " take=" + expected);
                continue;
            }

            string key = skillId + "|" + body + "|" + expected;
            if (!allow.Contains(key))
                failures.Add("ungranted gap " + key + " kind=" + kind);
        }

        HashSet<string> treeSkills = ReadTreeSkillIds(treesPath);
        Dictionary<string, string> ani = ReadAniChar(skillgrpPath);
        foreach (string skillId in treeSkills)
        {
            string tag;
            if (!ani.TryGetValue(skillId, out tag))
                continue;
            if (tag.Length == 0 || string.Equals(tag, "none", StringComparison.OrdinalIgnoreCase))
                continue;
            if (!learnedWithMotion.Contains(skillId))
                failures.Add("learnable skill " + skillId + " ani_char=" + tag + " is absent from skill-parity-motions.tsv");
        }

        CollectionAssert.IsEmpty(failures, string.Join("\n", failures.ToArray()));
    }

    static HashSet<string> ScanTakes(string animRoot)
    {
        var found = new HashSet<string>(StringComparer.Ordinal);
        if (!Directory.Exists(animRoot))
            return found;
        string[] files = Directory.GetFiles(animRoot, "*.anim", SearchOption.AllDirectories);
        for (int i = 0; i < files.Length; i++)
        {
            string name = Path.GetFileName(files[i]);
            int ao = name.IndexOf(".ao_", StringComparison.OrdinalIgnoreCase);
            if (ao < 0)
                continue;
            string take = name.Substring(ao + 4);
            if (take.EndsWith(".anim", StringComparison.OrdinalIgnoreCase))
                take = take.Substring(0, take.Length - 5);
            if (take.Length > 0)
                found.Add(take.ToLowerInvariant());
        }

        return found;
    }

    static HashSet<string> ReadTreeSkillIds(string path)
    {
        var ids = new HashSet<string>(StringComparer.Ordinal);
        string text = File.ReadAllText(path);
        const string needle = "\"skillId\":";
        int at = 0;
        while (true)
        {
            int hit = text.IndexOf(needle, at, StringComparison.Ordinal);
            if (hit < 0)
                break;
            int start = hit + needle.Length;
            while (start < text.Length && (text[start] == ' ' || text[start] == '\t'))
                start++;
            int end = start;
            while (end < text.Length && text[end] >= '0' && text[end] <= '9')
                end++;
            if (end > start)
                ids.Add(text.Substring(start, end - start));
            at = end;
        }

        return ids;
    }

    static Dictionary<string, string> ReadAniChar(string path)
    {
        var ani = new Dictionary<string, string>(StringComparer.Ordinal);
        string[] lines = File.ReadAllLines(path);
        if (lines.Length == 0)
            return ani;
        string[] header = lines[0].Split('\t');
        int idCol = IndexOf(header, "skill_id");
        int aniCol = IndexOf(header, "ani_char");
        Assert.GreaterOrEqual(idCol, 0);
        Assert.GreaterOrEqual(aniCol, 0);
        for (int i = 1; i < lines.Length; i++)
        {
            if (lines[i].Length == 0)
                continue;
            string[] cols = lines[i].Split('\t');
            if (aniCol >= cols.Length || idCol >= cols.Length)
                continue;
            string tag = cols[aniCol].Trim();
            if (tag.Length == 0)
                continue;
            string id = cols[idCol].Trim();
            if (!ani.ContainsKey(id))
                ani.Add(id, tag);
        }

        return ani;
    }

    static int IndexOf(string[] header, string name)
    {
        for (int i = 0; i < header.Length; i++)
        {
            if (header[i] == name)
                return i;
        }

        return -1;
    }

    static string RepoRoot()
    {
        return Path.GetFullPath(Path.Combine(Application.dataPath, "..", ".."));
    }
}
