using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;

public sealed class SkillVisualCatalogEditModeTests
{
    const int CatalogSkillCount = 2630;
    const int UniqueEffectCount = 928;
    const int NoUniqueEffectCount = 1702;

    [Test]
    public void Catalog_partitions_928_with_assets_or_missing_report_and_1702_with_none()
    {
        string docs = DocsRoot();
        string effectsRoot = Path.Combine(Application.dataPath, "Resources", "Data", "Effects");
        string reportPath = Path.Combine(docs, "skills", "skill-visual-missing-assets.tsv");
        string nonePath = Path.Combine(docs, "skills", "skills-without-unique-effect.tsv");

        SkillVisualCatalog catalog = LoadCatalog(docs);
        Assert.AreEqual(CatalogSkillCount, catalog.SkillCount);

        Dictionary<string, List<string>> prefabs = ScanPrefabs(effectsRoot);
        Func<string, bool> hasAsset = key => prefabs.ContainsKey(key);

        var missing = new List<string>();
        var noneIds = new HashSet<int>();
        int ready = 0;
        int missingIds = 0;
        foreach (int skillId in catalog.SkillIds)
        {
            SkillVisualStatus status = catalog.Rollup(skillId, hasAsset, missing);
            if (status == SkillVisualStatus.None)
                noneIds.Add(skillId);
            else if (status == SkillVisualStatus.Ready)
                ready++;
            else
                missingIds++;
        }

        Assert.AreEqual(NoUniqueEffectCount, noneIds.Count, "skills with no unique effect");
        Assert.AreEqual(UniqueEffectCount, ready + missingIds, "desc path or skill.usk");
        Assert.AreEqual(CatalogSkillCount, noneIds.Count + ready + missingIds);

        HashSet<int> listedNone = ReadSkillIds(nonePath);
        Assert.AreEqual(NoUniqueEffectCount, listedNone.Count);
        Assert.IsTrue(noneIds.SetEquals(listedNone), "derived none set must match skills-without-unique-effect.tsv");

        missing.Sort(StringComparer.Ordinal);
        List<string> report = ReadReportRows(reportPath);
        report.Sort(StringComparer.Ordinal);
        CollectionAssert.AreEqual(missing, report, "missing-asset report drifted from the catalog");

        var reportedIds = new HashSet<int>();
        for (int i = 0; i < report.Count; i++)
        {
            string idText = report[i].Split('\t')[0];
            reportedIds.Add(int.Parse(idText));
        }

        Assert.AreEqual(missingIds, reportedIds.Count, "every missing skill id is in the report");
        foreach (int skillId in catalog.SkillIds)
        {
            SkillVisualStatus status = catalog.Rollup(skillId, hasAsset, null);
            if (status == SkillVisualStatus.Missing)
                Assert.IsTrue(reportedIds.Contains(skillId), "missing skill " + skillId);
            else
                Assert.IsFalse(reportedIds.Contains(skillId), "skill " + skillId + " is " + status);
        }
    }

    [Test]
    public void Wind_strike_1177_uses_usk_flight_and_hit_phases()
    {
        SkillVisualCatalog catalog = LoadCatalog(DocsRoot());
        SkillVisualResolution wind = catalog.Resolve(1177, 1, Present);
        Assert.AreEqual(SkillVisualStatus.Ready, wind.Status);
        Assert.AreEqual(SkillVisualCatalog.SourceUsk, wind.Source);
        Assert.AreEqual("skill.el.1177", wind.VisualKey);
        Assert.AreEqual(0.4f, wind.FlyingTime, 0.0001f);
        Assert.AreEqual(4, wind.Actions.Count);
        Assert.AreEqual("casting", wind.Actions[0].Phase);
        Assert.AreEqual("el_wind_strike_ca", wind.Actions[0].AssetKey);
        Assert.AreEqual(EffectAttachmentPoint.CasterCenter, wind.Actions[0].Attachment);
        Assert.AreEqual(SkillVisualPartKind.Projectile, wind.Actions[2].Kind);
        Assert.AreEqual("el_wind_strike_fl", wind.Actions[2].AssetKey);
        Assert.AreEqual(CompositePartSpawnTiming.OnAnimationShoot, wind.Actions[2].Timing);
        Assert.AreEqual(SkillVisualPartKind.Impact, wind.Actions[3].Kind);
        Assert.AreEqual(CompositePartSpawnTiming.OnHitCollider, wind.Actions[3].Timing);
        Assert.AreEqual(EffectAttachmentPoint.WorldHitPoint, wind.Actions[3].Attachment);

        SkillVisualResolution power = catalog.Resolve(3, 1, Present);
        Assert.AreEqual(SkillVisualStatus.None, power.Status);
        Assert.AreEqual(0, power.Actions.Count);

        SkillVisualResolution healAlias = catalog.Resolve(21, 1, Present);
        Assert.AreEqual(SkillVisualCatalog.SourceDesc, healAlias.Source);
        Assert.AreEqual("skill.wh.1012", healAlias.VisualKey);
        Assert.AreNotEqual(SkillVisualStatus.None, healAlias.Status);

        SkillVisualResolution dance = catalog.Resolve(365, 1, Present);
        Assert.AreEqual(SkillVisualCatalog.SourceDesc, dance.Source);
        Assert.AreEqual("skill.phenomenon.271", dance.VisualKey);
        Assert.AreNotEqual(SkillVisualStatus.None, dance.Status);
    }

    [Test]
    public void Streaming_tables_match_the_interlude_sources_and_prefab_scan()
    {
        string docs = DocsRoot();
        string streaming = Path.Combine(Application.dataPath, "StreamingAssets", "Data", "Meta", "SkillVisual");
        string effectsRoot = Path.Combine(Application.dataPath, "Resources", "Data", "Effects");

        Assert.AreEqual(
            Normalize(File.ReadAllText(Path.Combine(docs, "skill-effects-engine.tsv"))),
            Normalize(File.ReadAllText(Path.Combine(streaming, "skill-effects-engine.txt"))));

        Dictionary<int, Dictionary<int, string>> fromDat = ReadDesc(Path.Combine(docs, "dats", "skillgrp.tsv"), "skill_level");
        Dictionary<int, Dictionary<int, string>> fromRuntime = ReadDesc(Path.Combine(streaming, "skillgrp-desc.txt"), "level");
        Assert.AreEqual(fromDat.Count, fromRuntime.Count);
        foreach (KeyValuePair<int, Dictionary<int, string>> skill in fromDat)
        {
            Dictionary<int, string> runtimeLevels;
            Assert.IsTrue(fromRuntime.TryGetValue(skill.Key, out runtimeLevels), "streaming desc missing skill " + skill.Key);
            Assert.AreEqual(skill.Value.Count, runtimeLevels.Count, "level count skill " + skill.Key);
            foreach (KeyValuePair<int, string> level in skill.Value)
            {
                string desc;
                Assert.IsTrue(runtimeLevels.TryGetValue(level.Key, out desc));
                Assert.AreEqual(level.Value, desc, "desc skill " + skill.Key + " level " + level.Key);
            }
        }

        Dictionary<string, List<string>> prefabs = ScanPrefabs(effectsRoot);
        Dictionary<string, string> index = ReadIndex(Path.Combine(streaming, "effect-prefabs.txt"));
        Assert.AreEqual(prefabs.Count, index.Count);
        foreach (KeyValuePair<string, List<string>> pair in prefabs)
        {
            string chosen;
            Assert.IsTrue(index.TryGetValue(pair.Key, out chosen), pair.Key);
            Assert.AreEqual(SkillVisualCatalog.ChoosePrefabResourcePath(pair.Value), chosen, pair.Key);
        }
    }

    static bool Present(string assetKey)
    {
        return true;
    }

    static SkillVisualCatalog LoadCatalog(string docs)
    {
        string error;
        SkillVisualCatalog catalog;
        bool ok = SkillVisualCatalog.TryParse(
            File.ReadAllText(Path.Combine(docs, "dats", "skillgrp.tsv")),
            File.ReadAllText(Path.Combine(docs, "skill-effects-engine.tsv")),
            out catalog,
            out error);
        Assert.IsTrue(ok, error);
        return catalog;
    }

    static string DocsRoot()
    {
        return Path.GetFullPath(Path.Combine(Application.dataPath, "..", "docs", "interlude-ref"));
    }

    static Dictionary<string, List<string>> ScanPrefabs(string effectsRoot)
    {
        var byName = new Dictionary<string, List<string>>(StringComparer.Ordinal);
        string resources = Path.Combine(Application.dataPath, "Resources");
        string[] files = Directory.GetFiles(effectsRoot, "*.prefab", SearchOption.AllDirectories);
        for (int i = 0; i < files.Length; i++)
        {
            string file = files[i].Replace('\\', '/');
            string key = Path.GetFileNameWithoutExtension(file).ToLowerInvariant();
            string relative = file.Substring(resources.Replace('\\', '/').Length + 1);
            if (relative.EndsWith(".prefab", StringComparison.OrdinalIgnoreCase))
                relative = relative.Substring(0, relative.Length - ".prefab".Length);
            List<string> paths;
            if (!byName.TryGetValue(key, out paths))
            {
                paths = new List<string>();
                byName[key] = paths;
            }

            paths.Add(relative);
        }

        return byName;
    }

    static HashSet<int> ReadSkillIds(string path)
    {
        var ids = new HashSet<int>();
        string[] lines = File.ReadAllLines(path);
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            if (line.Length == 0 || line[0] == '#')
                continue;
            string id = line.Split('\t')[0];
            int skillId;
            if (int.TryParse(id, out skillId))
                ids.Add(skillId);
        }

        return ids;
    }

    static List<string> ReadReportRows(string path)
    {
        var rows = new List<string>();
        string[] lines = File.ReadAllLines(path);
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            if (line.Length == 0 || line[0] == '#')
                continue;
            if (line.StartsWith("skill_id", StringComparison.Ordinal))
                continue;
            rows.Add(line);
        }

        return rows;
    }

    static Dictionary<int, Dictionary<int, string>> ReadDesc(string path, string levelColumn)
    {
        var map = new Dictionary<int, Dictionary<int, string>>();
        string[] lines = File.ReadAllLines(path);
        string[] header = lines[0].Split('\t');
        int idCol = Array.IndexOf(header, "skill_id");
        int levelCol = Array.IndexOf(header, levelColumn);
        int descCol = Array.IndexOf(header, "desc");
        Assert.GreaterOrEqual(idCol, 0);
        Assert.GreaterOrEqual(levelCol, 0);
        Assert.GreaterOrEqual(descCol, 0);
        for (int i = 1; i < lines.Length; i++)
        {
            if (lines[i].Length == 0)
                continue;
            string[] cols = lines[i].Split('\t');
            int skillId = int.Parse(cols[idCol]);
            int level = int.Parse(cols[levelCol]);
            Dictionary<int, string> levels;
            if (!map.TryGetValue(skillId, out levels))
            {
                levels = new Dictionary<int, string>();
                map[skillId] = levels;
            }

            levels[level] = cols[descCol];
        }

        return map;
    }

    static Dictionary<string, string> ReadIndex(string path)
    {
        var map = new Dictionary<string, string>(StringComparer.Ordinal);
        string[] lines = File.ReadAllLines(path);
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].Length == 0 || lines[i][0] == '#')
                continue;
            string[] cols = lines[i].Split('\t');
            if (cols[0] == "asset_key")
                continue;
            map[cols[0]] = cols[1];
        }

        return map;
    }

    static string Normalize(string text)
    {
        return text.Replace("\r\n", "\n").TrimEnd('\n') + "\n";
    }
}
