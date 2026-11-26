using System;

/// <summary>Shared Interlude char-create pawn indices (logongrp slots 8–25, #933).</summary>
public static class LobbyCharCreateStageCatalog
{
    public enum Stage
    {
        None = 0,
        Race = 1,
        Class = 2,
        Sex = 3,
    }

    public static bool TryRaceBaseIndex(string race, out int baseIndex)
    {
        switch (race)
        {
            case "Human":
                baseIndex = 8;
                return true;
            case "Elf":
                baseIndex = 12;
                return true;
            case "Dark Elf":
                baseIndex = 16;
                return true;
            case "Orc":
                baseIndex = 20;
                return true;
            case "Dwarf":
                baseIndex = 24;
                return true;
            default:
                baseIndex = 0;
                return false;
        }
    }

    public static int RaceGalleryCount(string race) => race == "Dwarf" ? 2 : 4;

    public static bool TryPawnIndex(string race, string pawnClass, string gender, out int index)
    {
        index = 0;
        if (!TryRaceBaseIndex(race, out index))
            return false;

        if (race == "Dwarf" && pawnClass == "Mystic")
            return false;

        if (pawnClass == "Mystic")
            index += 2;

        if (gender == "Female")
            index += 1;

        return true;
    }

    public static int ClassGalleryBase(string race, string pawnClass)
    {
        if (!TryRaceBaseIndex(race, out int baseIndex))
            return -1;
        return baseIndex + (pawnClass == "Mystic" ? 2 : 0);
    }
}