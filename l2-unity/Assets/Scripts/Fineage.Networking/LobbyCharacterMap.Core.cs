using System;
using System.Collections.Generic;

public static partial class LobbyCharacterMap
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
}
