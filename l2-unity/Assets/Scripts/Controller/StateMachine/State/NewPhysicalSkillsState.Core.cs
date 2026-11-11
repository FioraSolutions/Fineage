using System;
using System.Threading;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public partial class NewPhysicalSkillsState : AbstractAttackEvents
{
    public NewPhysicalSkillsState(PlayerStateMachine stateMachine) : 
        base(stateMachine.GetObjectId() ,
        SpecialAnimationNames.GetPhisicalSkillsAnimations(),
        stateMachine) { }
}
