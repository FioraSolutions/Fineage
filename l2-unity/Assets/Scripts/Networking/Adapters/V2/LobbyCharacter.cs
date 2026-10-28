using System;
using System.Collections.Generic;

/// <summary>
/// Character-select row for the lobby UI. Independent of the MessagePack wire struct.
/// </summary>
public sealed class LobbyCharacter
{
    public int Id;
    public string Name;
    public int ClassId;
    public int Race;
    public int Sex;
    public int Level;
    public int Slot;
    public int HairStyle;
    public int HairColor;
    public int Face;
    public int X;
    public int Y;
    public int Z;
    public int DeleteSecondsLeft;
    public bool IsLastPlayed;

    public bool PendingDelete
    {
        get { return DeleteSecondsLeft > 0; }
    }
}

public static class LobbyCharacterMap
{
    public static LobbyCharacter FromSummary(ZoneV2CharacterSummary row)
    {
        return new LobbyCharacter
        {
            Id = row.CharacterId,
            Name = row.Name ?? "",
            ClassId = row.ClassId,
            Race = row.Race,
            Sex = row.Sex,
            Level = row.Level,
            Slot = row.Slot,
            HairStyle = row.HairStyle,
            HairColor = row.HairColor,
            Face = row.Face,
            X = row.X,
            Y = row.Y,
            Z = row.Z,
            DeleteSecondsLeft = row.DeleteSecondsLeft,
            IsLastPlayed = row.IsLastPlayed
        };
    }

    public static LobbyCharacter[] FromSummaries(ZoneV2CharacterSummary[] rows)
    {
        if (rows == null || rows.Length == 0)
            return Array.Empty<LobbyCharacter>();
        LobbyCharacter[] list = new LobbyCharacter[rows.Length];
        for (int i = 0; i < rows.Length; i++)
            list[i] = FromSummary(rows[i]);
        return list;
    }

    /// <summary>New character id, otherwise the last-played row, otherwise the first slot.</summary>
    public static int PreferredIndex(IReadOnlyList<LobbyCharacter> rows, int preselectId)
    {
        if (rows == null || rows.Count == 0)
            return -1;
        if (preselectId > 0)
        {
            for (int i = 0; i < rows.Count; i++)
            {
                if (rows[i] != null && rows[i].Id == preselectId)
                    return i;
            }
        }

        for (int i = 0; i < rows.Count; i++)
        {
            if (rows[i] != null && rows[i].IsLastPlayed)
                return i;
        }

        return 0;
    }
}

/// <summary>Labels and lobby rules shared by the select screen. The server stays authoritative.</summary>
public static class LobbyCharacterText
{
    public const int CharMaxNumber = 7;
    public const int RaceHuman = 0;
    public const int ClassFighter = 0;
    public const int ClassMystic = 10;

    public static bool CanCreate(int characterCount)
    {
        return characterCount < CharMaxNumber;
    }

    public static bool CanEnter(LobbyCharacter character)
    {
        return character != null && !character.PendingDelete;
    }

    public static string ClassLabel(int classId)
    {
        if (classId == ClassFighter)
            return "Human Fighter";
        if (classId == ClassMystic)
            return "Human Mystic";
        return "Class " + classId;
    }

    public static string DeleteCountdown(int seconds)
    {
        if (seconds <= 0)
            return "";
        int days = seconds / 86400;
        int hours = (seconds % 86400) / 3600;
        int minutes = (seconds % 3600) / 60;
        if (days > 0)
            return "Deletes in " + days + "d " + hours + "h";
        if (hours > 0)
            return "Deletes in " + hours + "h " + minutes + "m";
        if (minutes > 0)
            return "Deletes in " + minutes + "m";
        return "Deletes in " + seconds + "s";
    }

    public static string ErrorMessage(string code, string serverMessage)
    {
        switch (code)
        {
            case "name_too_long":
                return "Maximum 16 letters or digits.";
            case "invalid_name":
                return "Incorrect name.";
            case "name_taken":
                return "This name already exists.";
            case "character_limit":
                return "You cannot create another character. Delete one first.";
            case "creation_failed":
            case "race_not_supported":
            case "class_not_supported":
            case "sex_not_supported":
                return "Character creation failed.";
            case "create_not_allowed":
                return "Characters cannot be created on this server.";
            case "character_not_found":
                return "That character was not found.";
            case "character_pending_delete":
                return "That character is pending deletion.";
            case "delete_failed":
                return "Could not delete that character.";
            case "auth_required":
                return "Login required.";
            case "bad_request":
                return "The request was rejected.";
            case "bad_slot":
                return "That character slot is not valid.";
            default:
                if (!string.IsNullOrEmpty(serverMessage))
                    return serverMessage;
                if (!string.IsNullOrEmpty(code))
                    return code;
                return "The server rejected the request.";
        }
    }
}

/// <summary>
/// Client-side create checks for the Human Fighter/Mystic lobby. The server stays authoritative.
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
        if (classId != LobbyCharacterText.ClassFighter && classId != LobbyCharacterText.ClassMystic)
            return "class_not_supported";
        if (race != LobbyCharacterText.RaceHuman)
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
