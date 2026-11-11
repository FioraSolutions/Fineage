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

    public static string ResolveOverrideSlotTriggerName(string triggerName)
    {
        if (string.IsNullOrEmpty(triggerName))
        {
            return triggerName;
        }

        switch (triggerName)
        {
            case "CastEnd2P":
                return "CastEnd";
            case "MagicShot2P":
                return "MagicShot";
            default:
                return triggerName;
        }
    }

    public static string ResolveOverrideCompletionEventName(string triggerOrOverrideKey)
    {
        if (string.IsNullOrEmpty(triggerOrOverrideKey))
        {
            return triggerOrOverrideKey;
        }

        string slotName = ResolveOverrideSlotTriggerName(triggerOrOverrideKey);
        if (!string.Equals(slotName, triggerOrOverrideKey, StringComparison.Ordinal))
        {
            return slotName;
        }

        switch (triggerOrOverrideKey)
        {
            case "CastLong":
            case "CastShort":
                return "CastMid";
            case "MagicNoTarget":
            case "MagicNotarget":
            case "MagicThrow":
                return "MagicShot";
            default:
                return triggerOrOverrideKey;
        }
    }
}
