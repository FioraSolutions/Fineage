using System;

public partial class PlayerStateMachine : MonoBehaviour
{
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
