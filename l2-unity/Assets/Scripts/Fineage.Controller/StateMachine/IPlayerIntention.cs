/// <summary>
/// Intention, state, and arrival events <c>PlayerController</c> sends while it moves (#131 S41b).
/// <c>Intention</c>, <c>PlayerState</c>, and <c>Event</c> already live in <c>Fineage.Controller</c>,
/// so this seam stays here. <c>Fineage.Game</c> does not reference <c>Fineage.Controller</c>.
/// <c>GameLifetimeScope</c> registers <c>PlayerIntentionSlot</c> as this interface.
/// The slot reads the live <c>PlayerStateMachine</c> on each call.
/// </summary>
public interface IPlayerIntention
{
    bool IsMoveToPawn { get; }

    PlayerState State { get; }

    void ChangeState(PlayerState newState);

    void ChangeIntention(Intention newIntention, object arg0);

    void NotifyEvent(Event evt);
}
