using System;

/// <summary>
/// Rebirth calls <c>World</c> injects. The live <see cref="PlayerStateMachine"/>
/// is attached when the player is adopted, because the scene machine is a stub.
/// </summary>
public sealed class PlayerRebirthRelay : IPlayerRebirth
{
    PlayerStateMachine _live;

    public void Attach(PlayerStateMachine machine)
    {
        _live = machine;
    }

    public void NotifyRebirth()
    {
        if (_live == null)
        {
            throw new InvalidOperationException(
                "[PlayerRebirthRelay] IPlayerRebirth missing — PlayerStateMachine.AdoptFromPlayer did not attach, so rebirth was not applied.");
        }

        _live.ChangeState(PlayerState.REBIRTH);
        _live.NotifyEvent(Event.REBIRTH);
    }
}
