using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public partial class SkillNameTable
{
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

    private void AddDict(Dictionary<int, SkillNameData> dataGrp, SkillNameData skillName)
    {
        dataGrp.TryAdd(skillName.Level, skillName);
    }
}
