using System;
using System.Collections.Generic;


/// <summary>
/// Client-side create checks for the five Interlude races. The server stays authoritative.
/// Appearance ranges match Fineage-Backend: hair 0–4 male / 0–6 female, color 0–3, face 0–2.
/// </summary>
public static class LobbyCharacterCreate
{
    public const int SexMale = 0;
    public const int SexFemale = 1;

    public static readonly string[] ForbiddenFragments =
    {
        "annou", "ammou", "amnou", "anmou", "anou", "amou"
    };

    public static readonly string[] MaleHair =
    {
        "Type A", "Type B", "Type C", "Type D", "Type E"
    };

    public static readonly string[] FemaleHair =
    {
        "Type A", "Type B", "Type C", "Type D", "Type E", "Type F", "Type G"
    };

    public static int MaxHairStyle(int sex)
    {
        return sex == SexFemale ? 6 : 4;
    }

    public static string[] HairLabels(int sex)
    {
        return sex == SexFemale ? FemaleHair : MaleHair;
    }

    public static int ClampAppearance(int value, int max)
    {
        if (value < 0)
            return 0;
        if (value > max)
            return max;
        return value;
    }

    /// <summary>Returns a server error code, or null when the request is allowed.</summary>
    public static string Validate(string name, int classId, int race, int sex, int hairStyle, int hairColor, int face)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Trim().Length > 16)
            return "name_too_long";
        name = name.Trim();
        if (!IsAsciiAlnum(name) || ContainsForbidden(name))
            return "invalid_name";
        if (sex != SexMale && sex != SexFemale)
            return "sex_not_supported";
        if (face < 0 || face > 2 || hairColor < 0 || hairColor > 3)
            return "creation_failed";
        if (hairStyle < 0 || hairStyle > MaxHairStyle(sex))
            return "creation_failed";
        if (!LobbyRaceCatalog.IsBaseClass(classId))
            return "class_not_supported";
        if (!LobbyRaceCatalog.RaceMatchesClass(classId, race))
            return "race_not_supported";
        return null;
    }

    static bool ContainsForbidden(string name)
    {
        for (int i = 0; i < ForbiddenFragments.Length; i++)
        {
            if (name.IndexOf(ForbiddenFragments[i], StringComparison.OrdinalIgnoreCase) >= 0)
                return true;
        }

        return false;
    }

    static bool IsAsciiAlnum(string name)
    {
        for (int i = 0; i < name.Length; i++)
        {
            char ch = name[i];
            bool digit = ch >= '0' && ch <= '9';
            bool upper = ch >= 'A' && ch <= 'Z';
            bool lower = ch >= 'a' && ch <= 'z';
            if (!digit && !upper && !lower)
                return false;
        }

        return name.Length > 0;
    }
}
