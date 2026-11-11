using System;


/// <summary>
/// Login roster row (<c>CharacterSummaryMsg</c>). Keys 0–14 identity/appearance/position;
/// keys 15–24 CharSelect vitals (cur/max HP MP CP, exp, sp, karma, weaponItemId);
/// key 25 paperdoll int[17]. Array length 26.
/// </summary>
public struct NetCharacterSummary
{
    public int CharacterId;
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
    public int CurHp, MaxHp, CurMp, MaxMp, CurCp, MaxCp;
    public long Exp;
    public int Sp, Karma, WeaponItemId;
    /// <summary>Equipped item ids by paperdoll slot (length 17). Null when the login did not send key 25.</summary>
    public int[] Paperdoll;
}

/// <summary>Character select ticket. <see cref="Token"/> is the zone Auth password.</summary>
public struct NetHandoff
{
    public string Account;
    public string CharacterName;
    public int CharacterId;
    public int ClassId;
    public string Token;
    public string ZoneHost;
    public int ZonePort;
    public long ExpiresUnixSeconds;
}

public struct NetCharacterCreated
{
    public int CharacterId;
    public string Name;
    public int ClassId;
    public int Race;
    public int Sex;
    public int HairStyle;
    public int HairColor;
    public int Face;
    public int X;
    public int Y;
    public int Z;
}

public struct NetCharacterDeleted
{
    public int Slot;
    public int CharacterId;
    public bool HardDeleted;
}

public struct NetCharacterDeleteFailed
{
    public int Slot;
    public int CharacterId;
    public string Code;
    public string Message;
}

/// <summary>Opcode 1012. <see cref="ChatType"/> is the type the speaker sent (0 all, 1 shout).</summary>
public struct NetCreatureSay
{
    public int ObjectId;
    public string Name;
    public string Text;
    public int ChatType;
}

/// <summary>Opcode 1040. <see cref="Text"/> is the line to show. Id 0 means show it as written.</summary>
public struct NetSystemMessage
{
    public int Id;
    public string[] Params;
    public string Text;
}
