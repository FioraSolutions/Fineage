using System;
using System.Collections.Generic;


/// <summary>Labels and lobby rules shared by the select screen. The server stays authoritative.</summary>
public static class LobbyCharacterText
{
    public const int CharMaxNumber = 7;
    public const string EmptyRosterStatus = "No characters yet.";
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
        string label = LobbyRaceCatalog.ClassLabel(classId);
        if (label != null)
            return label;
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
