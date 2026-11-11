using System;

public static partial class MonsterAnim
{
    public const string Wait = "wait";
    public const string Walk = "walk";
    public const string Run = "run";
    public const string SpAtk01 = "spatk01";
    public const string Atk01 = "atk01";
    public const string Death = "death";
    public const string AtkWait = "atkwait";



    /// <summary>
    /// Attack swings that must restart from 0 on every trigger.
    /// Death is terminal (see <see cref="MonsterCrossFade"/>) — not a retriggerable one-shot.
    /// </summary>
}
