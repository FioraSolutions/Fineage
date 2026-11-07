using UnityEngine;

/// <summary>
/// Mobius <c>CreatureAI.thinkCast</c> / <c>Player.useMagic</c> → <c>clientStopMoving(null)</c>:
/// the caster halts on the spot the instant a cast is requested or confirmed.
///
/// The server already halts its authoritative timed move in <c>StopForCast</c> and (by design)
/// sends nothing back to the caster, expecting the client to stop on <c>MagicSkillUse</c>.
/// Blocking new input through <see cref="CastMoveLock"/> alone is not enough: the local
/// <see cref="PlayerController"/> keeps stepping toward its old destination, so the client
/// drifts a few metres ahead of the server, the next ValidatePosition fails and
/// PositionCorrection rubber-bands the hero back to the cast origin.
/// </summary>
public static class LocalCastStop
{
    /// <summary>
    /// Clears the local move order and the locomotion state so the pawn stands still at once.
    /// Safe to call repeatedly; a no-op when the hero is not moving.
    /// </summary>
    public static void Halt()
    {
        PlayerController player = ControllerPacketServices.Player;
        if (player == null)
            return;

        if (!player.RunningToDestination)
            return;

        player.StopMove();
    }

    /// <summary>
    /// Stops the hero and drops the queued click/keyboard destination, then lets the state
    /// machine fall back to idle so the cast animation starts from a standing pose.
    /// </summary>
    public static void HaltForCast()
    {
        Halt();

        PlayerStateMachine machine = PlayerStateMachine.Instance;
        if (machine == null)
            return;

        if (machine.State == PlayerState.RUNNING || machine.State == PlayerState.WALKING)
        {
            machine.NotifyEvent(Event.ARRIVED);
            machine.ChangeIntention(Intention.INTENTION_IDLE);
        }

        // A queued MoveToPawn chase would otherwise resume the run after the cast.
        machine.IsMoveToPawn = false;
    }
}
