using System;

/// <summary>
/// Forwards <see cref="IPlayerIntention"/> to the live <see cref="PlayerStateMachine"/> (#131 S41b).
/// The machine is bound after the pawn spawns, so the slot stays registered while the machine is still null.
/// </summary>
public sealed class PlayerIntentionSlot : IPlayerIntention
{
    public bool IsMoveToPawn => Machine.IsMoveToPawn;

    public PlayerState State => Machine.State;

    public void ChangeState(PlayerState newState) => Machine.ChangeState(newState);

    public void ChangeIntention(Intention newIntention, object arg0) =>
        Machine.ChangeIntention(newIntention, arg0);

    public void NotifyEvent(Event evt) => Machine.NotifyEvent(evt);

    static PlayerStateMachine Machine
    {
        get
        {
            PlayerStateMachine machine = PlayerStatePacketServices.PlayerState;
            if (machine == null)
            {
                throw new InvalidOperationException(
                    "[PlayerIntentionSlot] IPlayerIntention missing — PlayerStateMachine was not bound, so the pawn intention was not applied.");
            }

            return machine;
        }
    }
}
