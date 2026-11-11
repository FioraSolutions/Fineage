using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;

public static partial class MagicSkillUseTrace
{
    public static void Begin(int skillId)
    {
        if (_active)
        {
            _suppress++;
            return;
        }

        _active = true;
        _skillId = skillId;
        _level = 0;
        _started = Stopwatch.GetTimestamp();
        Rows.Clear();
    }

    public static void End()
    {
        if (_suppress > 0)
        {
            _suppress--;
            return;
        }

        if (!_active)
            return;

        _active = false;
        double total = Elapsed(_started);
        if (total >= DumpMs)
            Dump(total);
        Rows.Clear();
    }
}
