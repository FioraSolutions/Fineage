using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public partial class SkillAnimationDatabase
{
    static bool ContainsRaceToken(string raceName, string token) =>
        raceName.IndexOf(token, StringComparison.OrdinalIgnoreCase) >= 0;

    public static string GetAnimationClipName(string trigerName, string raceName)
    {
        string slotTriggerName = ResolveOverrideSlotTriggerName(trigerName);
        if (FDarkElf == raceName)
        {
            string key = slotTriggerName + "_" + raceName;

            if (_raceCacheFDarkElf.ContainsKey(key))
            {

                return _raceCacheFDarkElf[key].GetName();
            }
        }
        return "";
    }
}
