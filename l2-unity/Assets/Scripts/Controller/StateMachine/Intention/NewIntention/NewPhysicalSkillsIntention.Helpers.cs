using System;
using UnityEngine;

public partial class NewPhysicalSkillsIntention : IntentionBase
{
    private void IfUseSelf(int objectId , MagicSkillUseDto useSkill)
    {
        if (objectId != useSkill.TargetId)
        {
            _stateMachine.ChangeState(PlayerState.PHYSICAL_SKILLS);
            _stateMachine.NotifyEvent(Event.READY_TO_ACT, useSkill);
        }
        else
        {
            _stateMachine.ChangeState(PlayerState.PHYSICAL_SKILLS);
            _stateMachine.NotifyEvent(Event.APPLY_SELF_SKILL, useSkill);
        }
    }

    public override void Exit() { }

    public override void Update()
    {

    }
}
