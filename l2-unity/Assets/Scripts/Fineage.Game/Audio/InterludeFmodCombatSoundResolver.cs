using FMODUnity;
using UnityEngine;

/// <summary>
/// Single combat SFX resolve path: Interlude dat token / FMOD event → load bank → FMOD one-shot,
/// else the matching Resources stem (same name as the original event). No cross-stem fallbacks.
/// </summary>
public static class InterludeFmodCombatSoundResolver
{
    static readonly string[] CombatBankNames =
    {
        "Master",
        "Master.strings",
        "ItemSound",
        "MonSound",
        "SkillSound",
        "ChrSound",
    };

    static readonly System.Collections.Generic.HashSet<string> LoggedBankLoadFailures =
        new System.Collections.Generic.HashSet<string>();

    public struct ResolveResult
    {
        public string FmodEventPath;
        public string ResourcesPath;
        public string DatToken;
    }

    public static bool TryResolveFromDatToken(string datToken, out ResolveResult result)
    {
        result = default;
        if (!InterludeSoundTokenPath.TryFromDatToken(datToken, out string fmodPath))
            return false;

        result.DatToken = datToken;
        result.FmodEventPath = fmodPath;
        result.ResourcesPath = InterludeSoundTokenPath.ResourcesPathFromFmodEvent(fmodPath);
        return true;
    }

    public static bool EnsureCombatBankForEvent(string fmodEventPath)
    {
        string bank = ParseBankName(fmodEventPath);
        if (string.IsNullOrEmpty(bank))
            return false;

        if (RuntimeManager.HasBankLoaded(bank))
            return true;

        if (bank != "Master" && bank != "Master.strings"
            && !RuntimeManager.HasBankLoaded("Master.strings"))
            TryLoadBankQuiet("Master.strings");

        if (TryLoadBankQuiet(bank))
            return RuntimeManager.HasBankLoaded(bank);

        if (LoggedBankLoadFailures.Add(bank))
        {
            Debug.LogError(
                "InterludeFmodCombatSoundResolver: failed to load FMOD bank=\"" + bank
                + "\" for event=\"" + fmodEventPath + "\"");
        }

        return RuntimeManager.HasBankLoaded(bank);
    }

    static bool TryLoadBankQuiet(string bankName)
    {
        if (string.IsNullOrEmpty(bankName))
            return false;
        try
        {
            RuntimeManager.LoadBank(bankName);
            return RuntimeManager.HasBankLoaded(bankName);
        }
        catch
        {
            return false;
        }
    }

    public static string ParseBankName(string fmodEventPath)
    {
        if (string.IsNullOrEmpty(fmodEventPath) || !fmodEventPath.StartsWith("event:/"))
            return null;

        string tail = fmodEventPath.Substring("event:/".Length);
        int slash = tail.IndexOf('/');
        if (slash <= 0)
            return null;

        string bank = tail.Substring(0, slash);
        if (bank.Equals("ChrSound", System.StringComparison.OrdinalIgnoreCase))
            return "ChrSound";
        if (bank.Equals("ItemSound", System.StringComparison.OrdinalIgnoreCase))
            return "ItemSound";
        if (bank.Equals("MonSound", System.StringComparison.OrdinalIgnoreCase))
            return "MonSound";
        if (bank.Equals("SkillSound", System.StringComparison.OrdinalIgnoreCase))
            return "SkillSound";
        return bank;
    }

    public static void WarmCombatBanks()
    {
        for (int i = 0; i < CombatBankNames.Length; i++)
        {
            string bank = CombatBankNames[i];
            if (!RuntimeManager.HasBankLoaded(bank))
                TryLoadBankQuiet(bank);
        }
    }
}