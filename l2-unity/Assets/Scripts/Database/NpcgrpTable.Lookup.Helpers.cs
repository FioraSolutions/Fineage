using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public partial class NpcgrpTable
{
    public Npcgrp GetNpcgrp(int id) {
        Npcgrp npcgrp = null;
        _npcgrps.TryGetValue(id, out npcgrp);

        if(npcgrp == null) {
            // Debug.LogWarning($"Npcgrp not found for id [{id}]");
        }

        return npcgrp;
    }

    static void ParsePropertyList(Npcgrp npcgrp, string value)
    {
        if (npcgrp == null || string.IsNullOrEmpty(value))
            return;
        string[] parts = DatUtils.ParseArray(value);
        if (parts == null || parts.Length < 2)
            return;

        int pairCount = parts.Length / 2;
        int[] ids = new int[pairCount];
        int[] levels = new int[pairCount];
        int written = 0;
        for (int i = 0; i + 1 < parts.Length; i += 2)
        {
            if (!int.TryParse(parts[i], out int skillId) || skillId <= 0)
                continue;
            if (!int.TryParse(parts[i + 1], out int level) || level <= 0)
                level = 1;
            ids[written] = skillId;
            levels[written] = level;
            written++;
        }

        if (written == 0)
            return;
        if (written != pairCount)
        {
            System.Array.Resize(ref ids, written);
            System.Array.Resize(ref levels, written);
        }

        npcgrp.PropertySkillIds = ids;
        npcgrp.PropertySkillLevels = levels;
    }
}
