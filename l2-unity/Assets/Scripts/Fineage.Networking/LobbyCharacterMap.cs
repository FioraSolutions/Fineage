using System;
using System.Collections.Generic;


public static class LobbyCharacterMap
{
    public static LobbyCharacter FromSummary(NetCharacterSummary row)
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
            IsLastPlayed = row.IsLastPlayed,
            CurHp = row.CurHp,
            MaxHp = row.MaxHp,
            CurMp = row.CurMp,
            MaxMp = row.MaxMp,
            CurCp = row.CurCp,
            MaxCp = row.MaxCp,
            Exp = row.Exp,
            Sp = row.Sp,
            Karma = row.Karma,
            WeaponItemId = row.WeaponItemId,
            Paperdoll = row.Paperdoll
        };
    }

    public static LobbyCharacter[] FromSummaries(NetCharacterSummary[] rows)
    {
        if (rows == null || rows.Length == 0)
            return Array.Empty<LobbyCharacter>();
        LobbyCharacter[] list = new LobbyCharacter[rows.Length];
        for (int i = 0; i < rows.Length; i++)
            list[i] = FromSummary(rows[i]);
        return list;
    }

    public static LobbyCharacter FromNetSummary(Fineage.Net.Contract.CharacterSummaryNet row)
    {
        int deleteSeconds = row.DeleteSecondsLeft;
        if (deleteSeconds <= 0 && row.PendingDelete)
            deleteSeconds = 1;
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
            DeleteSecondsLeft = deleteSeconds,
            IsLastPlayed = row.IsLastPlayed,
            CurHp = row.Hp,
            MaxHp = row.MaxHp,
            CurMp = row.Mp,
            MaxMp = row.MaxMp,
            CurCp = row.CurCp,
            MaxCp = row.MaxCp,
            Exp = row.Exp,
            Sp = row.Sp,
            Karma = row.Karma,
            WeaponItemId = row.WeaponItemId,
            Paperdoll = row.Paperdoll
        };
    }

    public static LobbyCharacter[] FromNetRoster(Fineage.Net.Contract.CharacterRosterMsg roster)
    {
        if (roster?.Characters == null || roster.Characters.Length == 0)
            return Array.Empty<LobbyCharacter>();
        LobbyCharacter[] list = new LobbyCharacter[roster.Characters.Length];
        for (int i = 0; i < roster.Characters.Length; i++)
            list[i] = FromNetSummary(roster.Characters[i]);
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
