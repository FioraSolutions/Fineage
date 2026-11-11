public partial class NewAttackState : AbstractAttackEvents
{
    public NewAttackState(PlayerStateMachine stateMachine) :
        base(stateMachine.GetObjectId() , 
        SpecialAnimationNames.GetSpecialsAttackAnimations() , 
        stateMachine)
    {

    }
}
