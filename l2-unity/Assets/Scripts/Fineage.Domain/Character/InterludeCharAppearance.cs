using UnityEngine;

/// <summary>
/// Packet race id and fighter/mystic base class shared by CharInfo and UserInfo.
/// Race ids match <c>CharacterRace</c> 0–4. Base class ids match <c>BaseClass</c>.
/// </summary>
public static class InterludeCharAppearance
{
    public const int Fighter = 0;
    public const int Mystic = 10;

    public static int RaceFromPacket(int packetRace)
    {
        if (packetRace >= 0 && packetRace <= 4)
            return packetRace;

        Debug.LogError("[MapClassId] unknown lobby race id=" + packetRace + " (expected 0–4 Interlude race).");
        return 0;
    }

    /// <summary>
    /// Mage tree through 3rd profession (ClassId). CharacterClass is only 0–57; do not cast 88+.
    /// </summary>
    public static bool IsMage(int classId)
    {
        switch (classId)
        {
            case (int)ClassId.MAGE:
            case (int)ClassId.WIZARD:
            case (int)ClassId.SORCERER:
            case (int)ClassId.NECROMANCER:
            case (int)ClassId.WARLOCK:
            case (int)ClassId.CLERIC:
            case (int)ClassId.BISHOP:
            case (int)ClassId.PROPHET:
            case (int)ClassId.ARCHMAGE:
            case (int)ClassId.SOULTAKER:
            case (int)ClassId.ARCANA_LORD:
            case (int)ClassId.CARDINAL:
            case (int)ClassId.HIEROPHANT:
            case (int)ClassId.ELVEN_MAGE:
            case (int)ClassId.ELVEN_WIZARD:
            case (int)ClassId.SPELLSINGER:
            case (int)ClassId.ELEMENTAL_SUMMONER:
            case (int)ClassId.ORACLE:
            case (int)ClassId.ELDER:
            case (int)ClassId.MYSTIC_MUSE:
            case (int)ClassId.ELEMENTAL_MASTER:
            case (int)ClassId.EVA_SAINT:
            case (int)ClassId.DARK_MAGE:
            case (int)ClassId.DARK_WIZARD:
            case (int)ClassId.SPELLHOWLER:
            case (int)ClassId.PHANTOM_SUMMONER:
            case (int)ClassId.SHILLIEN_ORACLE:
            case (int)ClassId.SHILLIEN_ELDER:
            case (int)ClassId.STORM_SCREAMER:
            case (int)ClassId.SPECTRAL_MASTER:
            case (int)ClassId.SHILLIEN_SAINT:
            case (int)ClassId.ORC_MAGE:
            case (int)ClassId.ORC_SHAMAN:
            case (int)ClassId.OVERLORD:
            case (int)ClassId.WARCRYER:
            case (int)ClassId.DOMINATOR:
            case (int)ClassId.DOOMCRYER:
                return true;
            default:
                return false;
        }
    }

    public static int BaseClassFromClassId(int classId)
    {
        return IsMage(classId) ? Mystic : Fighter;
    }
}
