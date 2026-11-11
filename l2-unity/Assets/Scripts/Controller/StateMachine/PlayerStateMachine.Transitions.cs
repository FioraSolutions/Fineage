using System;

public partial class PlayerStateMachine : MonoBehaviour
{
    private void InitializeState()
    {
        _stateInstance = GetOrAddState(_dictState, _currentState);
    }

    private void InitializeIntention()
    {
        _intentionInstance = GetOrAddIntention(_dictIntention, _currentIntention);
    }

    private StateBase GetOrAddState(Dictionary<PlayerState, StateBase> dict, PlayerState key)
    {

        if (!dict.ContainsKey(key))
        {
            var state = CreatorStatePlayer.GetState(key, this);
            dict.Add(key, state);
        }


        return dict[key];
    }

    private IntentionBase GetOrAddIntention(Dictionary<Intention, IntentionBase> dict, Intention key)
    {

        if (!dict.ContainsKey(key))
        {
            var state = CreateIntentionPlayer.GetIntention(key, this);
            dict.Add(key, state);
        }


        return dict[key];
    }

    public void NotifyEvent(Event evt , object payload = null)
    {
        //if (_enableLogs) Debug.Log("[StateMachine][EVENT] " + evt);
        _stateInstance?.HandleEvent(evt , payload);
    }

    public bool ShouldKeepLocomotion()
    {
        if (_currentState == PlayerState.WALKING || _currentState == PlayerState.RUNNING)
            return true;
        if (_currentIntention == Intention.INTENTION_MOVE_TO ||
            _currentIntention == Intention.INTENTION_MOVE_TO_PAWN)
            return true;
        return ControllerPacketServices.Player != null &&
               ControllerPacketServices.Player.RunningToDestination;
    }

    public void OnWaitReturn()
    {
        // WaitReturnLog.Dump("PlayerStateMachine.OnWaitReturn");
        NotifyEvent(Event.WAIT_RETURN);
    }

    public void ToggleRide()
    {
        if (Player == null || _currentState == PlayerState.DEAD)
            return;
        StriderRideService ride = AnimationPacketServices.StriderRide;
        if (ride == null)
            return;

        if (ride.IsRiding(Player))
            ride.Dismount(Player);
        else if (!ride.TryMount(Player))
            return;

        NotifyEvent(Event.RIDE);
    }
}
