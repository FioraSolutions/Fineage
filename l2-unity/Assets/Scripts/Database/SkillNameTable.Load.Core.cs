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

    public static void Unbind() => _instance = null;

    public SkillNameData GetName(int id , int level)
    {
        if (Names.ContainsKey(id) != true) return null;

        Dictionary<int, SkillNameData> _skillNameLevel = Names[id];
        if (!_skillNameLevel.ContainsKey(level)) return null;

        return _skillNameLevel[level];
    }
}
