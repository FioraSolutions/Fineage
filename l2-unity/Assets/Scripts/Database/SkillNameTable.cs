using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class SkillNameTable
{
    private static SkillNameTable _instance;
    private static Dictionary<int, Dictionary<int, SkillNameData>> _names;
    public Dictionary<int, Dictionary<int, SkillNameData>> Names { get { return _names; } }
    public static SkillNameTable Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SkillNameTable();
            }

            return _instance;
        }
    }

    

    public SkillNameData GetName(int id , int level)
    {
        if (Names.ContainsKey(id) != true) return null;

        Dictionary<int, SkillNameData> _skillNameLevel = Names[id];
        if (!_skillNameLevel.ContainsKey(level)) return null;

        return _skillNameLevel[level];
    }
    

    public void Initialize()
    {
        _names = new Dictionary<int, Dictionary<int, SkillNameData>>();
        ReadActions();
        ReadNameInterlude();
        // Debug.Log("");
    }

    private void ReadActions()
    {
       
        string dataPath = Path.Combine(Application.streamingAssetsPath, "Data/Meta/SkillName_Classic-eu.txt");

        if (!File.Exists(dataPath))
        {
            // Debug.LogWarning("File not found: " + dataPath);
            return;
        }

        using (StreamReader reader = new StreamReader(dataPath))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                SkillNameData nameData = new SkillNameData();

                string[] keyvals = line.Split('\t');

                for (int i = 0; i < keyvals.Length; i++)
                {
                    if (!keyvals[i].Contains("="))
                    {
                        continue;
                    }

                    string[] keyval = keyvals[i].Split("=");
                    string key = keyval[0];
                    string value = keyval[1];
                    
                    switch (key)
                    {
                        case "skill_id":
                            nameData.Id = int.Parse(value);
                            if (nameData.Id.Equals(410))
                            {
                                //Debug.Log("");
                            }
                           break;
                        case "skill_level":
                            nameData.Level = int.Parse(value);
                            break;
                        case "skill_sublevel":
                            nameData.SubLevel = int.Parse(value);
                            break;
                        case "name":
                            nameData.Name = DatUtils.CleanupString(value);
                            break;
                        case "desc":
                            nameData.Desc = DatUtils.CleanupString(value);
                            break;
                        case "prev_skill_id":
                            nameData.PrevSkillId = int.Parse(value);
                            break;
                    }
                }

                TryAdd(nameData);
            }

            // Debug.Log($"Successfully imported {_names.Count} actionName(s)");
        }
    }

    private void TryAdd(SkillNameData skillName)
    {

        if (_names.ContainsKey(skillName.Id) == true)
        {
            Dictionary<int, SkillNameData> dataGrp = _names[skillName.Id];
            dataGrp.TryAdd(skillName.Level, skillName);
        }
        else
        {
            Dictionary<int, SkillNameData> dataGrp = CreateDict();
            AddDict(dataGrp, skillName);
            _names.TryAdd(skillName.Id, dataGrp);
        }
    }

    private Dictionary<int, SkillNameData> CreateDict()
    {
        return new Dictionary<int, SkillNameData>();
    }
    private void AddDict(Dictionary<int, SkillNameData> dataGrp, SkillNameData skillName)
    {
        dataGrp.TryAdd(skillName.Level, skillName);
    }

    private const int IndexName = 2;
    private const int IndexDescription = 3;

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
