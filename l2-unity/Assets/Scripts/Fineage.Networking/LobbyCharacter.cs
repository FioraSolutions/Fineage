using System;
using System.Collections.Generic;


/// <summary>
/// Character-select row for the lobby UI (presentation shape, not a raw Net contract row).
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
    public int CurHp, MaxHp, CurMp, MaxMp, CurCp, MaxCp;
    public long Exp;
    public int Sp, Karma, WeaponItemId;
    /// <summary>Equipped item ids by paperdoll slot (length 17). Null when login omitted key 25.</summary>
    public int[] Paperdoll;

    public bool PendingDelete
    {
        get { return DeleteSecondsLeft > 0; }
    }
}
