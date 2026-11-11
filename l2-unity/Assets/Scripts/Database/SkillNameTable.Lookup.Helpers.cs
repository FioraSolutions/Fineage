using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public partial class SkillNameTable
{
    public void ReadNameInterlude()
    {
        string dataPath = Path.Combine(Application.streamingAssetsPath, "Data/Meta/Skillname_interlude-e.txt");
        if (!File.Exists(dataPath))
            return;

        // Interlude SoT: Classic-eu carries later-chronicle skill text (e.g. 3562
        // "P./M. Atk. +4%"). Upsert name+desc for every Interlude row; do not leave
        // Classic-only higher levels for the same skillId (enhanced/blessed variants).
        var interludeLevelsBySkill = new Dictionary<int, HashSet<int>>();

        using (StreamReader reader = new StreamReader(dataPath))
        {
            string line;
            int index = 0;
            while ((line = reader.ReadLine()) != null)
            {
                if (index != 0)
                {
                    string[] ids = line.Split('\t');
                    if (ids.Length <= IndexDescription)
                    {
                        index++;
                        continue;
                    }

                    if (!int.TryParse(ids[0], out int id) || !int.TryParse(ids[1], out int level))
                    {
                        index++;
                        continue;
                    }

                    string name = DatUtils.CleanupStringOldData(ids[IndexName]);
                    string description = DatUtils.CleanupStringOldData(ids[IndexDescription])
                        .Replace("Effect", "Power");
                    UpsertInterlude(id, level, name, description);

                    if (!interludeLevelsBySkill.TryGetValue(id, out HashSet<int> levels))
                    {
                        levels = new HashSet<int>();
                        interludeLevelsBySkill[id] = levels;
                    }
                    levels.Add(level);
                }

                index++;
            }
        }

        foreach (var pair in interludeLevelsBySkill)
        {
            if (!_names.TryGetValue(pair.Key, out Dictionary<int, SkillNameData> byLevel))
                continue;
            List<int> drop = null;
            foreach (int level in byLevel.Keys)
            {
                if (!pair.Value.Contains(level))
                    (drop ??= new List<int>()).Add(level);
            }
            if (drop == null)
                continue;
            foreach (int level in drop)
                byLevel.Remove(level);
        }
    }

    private void UpsertInterlude(int id, int level, string name, string description)
    {
        if (!_names.TryGetValue(id, out Dictionary<int, SkillNameData> byLevel))
        {
            byLevel = CreateDict();
            _names[id] = byLevel;
        }

        if (byLevel.TryGetValue(level, out SkillNameData existing))
        {
            if (!string.IsNullOrEmpty(name))
                existing.Name = name;
            if (!string.IsNullOrEmpty(description))
                existing.Desc = description;
            return;
        }

        var created = new SkillNameData
        {
            Id = id,
            Level = level,
            Name = name,
            Desc = description
        };
        byLevel[level] = created;
    }
}
