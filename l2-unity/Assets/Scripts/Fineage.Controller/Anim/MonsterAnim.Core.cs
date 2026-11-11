using System;

public static partial class MonsterAnim
{
    public static string ToStateName(MonsterAnimState state)
    {
        switch (state)
        {
            case MonsterAnimState.Wait: return Wait;
            case MonsterAnimState.Walk: return Walk;
            case MonsterAnimState.Run: return Run;
            case MonsterAnimState.SpAtk01: return SpAtk01;
            case MonsterAnimState.Atk01: return Atk01;
            case MonsterAnimState.Death: return Death;
            case MonsterAnimState.AtkWait: return AtkWait;
            default: return Wait;
        }
    }
}
