using FMOD;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
using Debug = UnityEngine.Debug;

public partial class SkillgrpTable
{
    public SkillNameData GetSkillName(int id, int level)
    {
        return TablePacketServices.SkillNames.GetName(id, level);
    }
}
