using System;


public struct NetUserInfo
{
    public int ObjectId;
    public string Name;
    public int ClassId;
    public int Race;
    public int Sex;
    public int X;
    public int Y;
    public int Z;
    public float RunSpeed;
    public float WalkSpeed;
    public bool Running;
    public bool HasPaperdoll;
    public NetPaperdollSlot[] Paperdoll;
    public bool HasClan;
    public int ClanId;
    public int ClanCrestId;
    public bool HasVitals;
    public int Level;
    public int CurHp, MaxHp, CurMp, MaxMp, CurCp, MaxCp;
    public long Exp;
    public int Sp;
    public bool HasStats;
    public int Str, Dex, Con, Int, Wit, Men;
    public int PAtk, MAtk, PDef, MDef, PAtkSpd, MAtkSpd, Accuracy, Evasion, CritRate;
    public int CurrWeight;
    public int MaxWeight;
    public int Karma;
    public byte PvpFlag;
}

public struct NetMyTargetSelected
{
    public int ObjectId;
    public int ColorHint;
}

public struct NetAttack
{
    public int AttackerObjectId;
    public int TargetObjectId;
    public int Damage;
    public byte Flags;
    public int AttackerX;
    public int AttackerY;
    public int AttackerZ;
    public int TargetX;
    public int TargetY;
    public int TargetZ;
}

public struct NetStatusUpdate
{
    public int ObjectId;
    public int[] AttributeIds;
    public int[] Values;
}

public struct NetSkillEntry
{
    public int SkillId;
    public int Level;
    public int ReuseDelayMs;
}

public struct NetMagicSkillUse
{
    public int CasterObjectId;
    public int TargetObjectId;
    public int SkillId;
    public int SkillLevel;
    public int HitTimeMs;
    public int ReuseDelayMs;
}

public struct NetDie
{
    public int ObjectId;
    public int KillerObjectId;
}

public struct NetRevive
{
    public int ObjectId;
}

public struct NetTeleportToLocation
{
    public int ObjectId;
    public int X;
    public int Y;
    public int Z;
    public int Heading;
}

public struct NetAbnormalStatusUpdate
{
    public int ObjectId;
    public NetAbnormalEffect[] Effects;
}

public struct NetAbnormalEffect
{
    public int Id;
    public int Value;
    public int DurationSeconds;
    /// <summary>Mobius <c>abnormalType</c> slot (SLEEP, STUN, PARALYZE, …).</summary>
    public string Slot;
}
