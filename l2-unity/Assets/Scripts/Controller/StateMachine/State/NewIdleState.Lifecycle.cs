using UnityEngine;

public partial class NewIdleState : StateBase
{
    public NewIdleState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Update() { }

    public override void HandleEvent(Event evt, object payload = null)
    {
        switch (evt)
        {
            case Event.ENTER_WORLD:
                HandleEnterWorld();
                break;
            case Event.CHANGE_EQUIP:
                HandleEquipChange();
                break;
            case Event.ARRIVED:
                HandleArrival();
                break;
            case Event.WAIT_RETURN:
                HandleWaitReturn(payload);
                break;
            case Event.RIDE:
                PlayAnimation(ForRide(AnimationNames.WAIT));
                break;
        }
    }

    private void HandleEnterWorld()
    {
        PlayAnimation(ForRide(AnimationNames.WAIT));
    }

    private void HandleEquipChange()
    {
        var animation = _stateMachine.Player.isAutoAttack
            ? AnimationNames.ATK_WAIT
            : AnimationNames.WAIT;
        PlayAnimation(ForRide(animation));

    }

    private void HandleArrival()
    {
        bool useAtkWait = _stateMachine.Player != null && _stateMachine.Player.isAutoAttack;
        var animation = useAtkWait ? AnimationNames.ATK_WAIT : AnimationNames.WAIT;
        PlayAnimation(ForRide(animation));
    }
}
