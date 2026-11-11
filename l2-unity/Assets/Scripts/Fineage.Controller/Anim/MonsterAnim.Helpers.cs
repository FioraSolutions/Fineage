using System;

public static partial class MonsterAnim
{
    public static bool TryResolve(string animName, out string stateName, out MonsterAnimState family)
    {
        stateName = null;
        family = default;
        if (string.IsNullOrEmpty(animName))
        {
            return false;
        }

        // Exact match (AnimationNames.MONSTER_* already use bare names).
        if (string.Equals(animName, Wait, StringComparison.OrdinalIgnoreCase))
        {
            family = MonsterAnimState.Wait;
            stateName = Wait;
            return true;
        }

        if (string.Equals(animName, Walk, StringComparison.OrdinalIgnoreCase))
        {
            family = MonsterAnimState.Walk;
            stateName = Walk;
            return true;
        }

        if (string.Equals(animName, Run, StringComparison.OrdinalIgnoreCase))
        {
            family = MonsterAnimState.Run;
            stateName = Run;
            return true;
        }

        if (string.Equals(animName, SpAtk01, StringComparison.OrdinalIgnoreCase))
        {
            family = MonsterAnimState.SpAtk01;
            stateName = SpAtk01;
            return true;
        }

        if (string.Equals(animName, Atk01, StringComparison.OrdinalIgnoreCase))
        {
            family = MonsterAnimState.Atk01;
            stateName = Atk01;
            return true;
        }

        if (string.Equals(animName, Death, StringComparison.OrdinalIgnoreCase))
        {
            family = MonsterAnimState.Death;
            stateName = Death;
            return true;
        }

        if (string.Equals(animName, AtkWait, StringComparison.OrdinalIgnoreCase))
        {
            family = MonsterAnimState.AtkWait;
            stateName = AtkWait;
            return true;
        }

        return false;
    }

    public static bool IsOneShot(MonsterAnimState family)
    {
        return family == MonsterAnimState.Atk01
            || family == MonsterAnimState.SpAtk01;
    }
}
