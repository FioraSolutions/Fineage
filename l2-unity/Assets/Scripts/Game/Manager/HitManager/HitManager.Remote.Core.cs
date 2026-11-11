using System;
using System.Collections.Generic;
using UnityEngine;

public partial class HitManager
{
    public void RememberLocalAttack(AttackDto dto)
    {
        _localAttack = dto;
        _localAttackToken++;
    }

    public void FireRememberedExtraHits(Entity attacker)
    {
        if (attacker == null || _localAttack == null || _localAttackFiredToken == _localAttackToken)
            return;

        _localAttackFiredToken = _localAttackToken;
        FireExtraHits(attacker, _localAttack, _localAttack.TargetId);
    }
}
