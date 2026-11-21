/// <summary>
/// Derives FMOD ChrSound bank id from Interlude npcgrp damage_sound tokens (e.g. ChrSound.FNpc_Young_Dmg_1).
/// </summary>
public static class NpcChrSoundBankResolver
{
    public static bool TryResolveFromDamageSounds(string[] damageSounds, out string bank)
    {
        bank = null;
        if (damageSounds == null || damageSounds.Length == 0)
            return false;

        for (int i = 0; i < damageSounds.Length; i++)
        {
            if (TryParseChrSoundBankToken(damageSounds[i], out bank))
                return true;
        }

        return false;
    }

    public static bool TryParseChrSoundBankToken(string token, out string bank)
    {
        bank = null;
        if (string.IsNullOrEmpty(token))
            return false;

        string cleaned = DatUtils.CleanupString(token);
        if (string.IsNullOrEmpty(cleaned))
            return false;

        string[] parts = cleaned.Split('.');
        if (parts.Length < 2)
            return false;

        if (!parts[0].Equals("ChrSound", System.StringComparison.OrdinalIgnoreCase))
            return false;

        string remainder = parts[1];
        int dmgIdx = remainder.LastIndexOf("_Dmg_", System.StringComparison.OrdinalIgnoreCase);
        if (dmgIdx > 0)
            bank = remainder.Substring(0, dmgIdx);
        else
            bank = remainder;

        return !string.IsNullOrEmpty(bank);
    }

    /// <summary>
    /// Interlude npcgrp ChrSound tokens name NPC voice banks (MNpc_Young, …) but shipped FMOD ChrSound.bank
    /// only contains player race folders (MHFighter, FElf, …). Map npc tokens before building event paths.
    /// </summary>
    public static string MapDamageBankToFmodBank(string damageSoundBank)
    {
        if (string.IsNullOrEmpty(damageSoundBank))
            return damageSoundBank;

        string bank = damageSoundBank;
        bool female = bank.Length > 0 && (bank[0] == 'F' || bank[0] == 'f');
        if (bank.StartsWith("MNpc_", System.StringComparison.OrdinalIgnoreCase)
            || bank.StartsWith("FNpc_", System.StringComparison.OrdinalIgnoreCase))
        {
            if (bank.IndexOf("Elf", System.StringComparison.OrdinalIgnoreCase) >= 0)
                return female ? "FElf" : "MElf";
            if (bank.IndexOf("Dwarf", System.StringComparison.OrdinalIgnoreCase) >= 0)
                return female ? "FDwarf" : "MDwarf";
            return female ? "FHFighter" : "MHFighter";
        }

        return bank;
    }
}