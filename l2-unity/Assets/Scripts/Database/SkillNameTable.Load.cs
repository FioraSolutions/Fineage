using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public partial class SkillNameTable
{
    public static void Bind(SkillNameTable table)
    {
        if (table != null)
            _instance = table;
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
}
