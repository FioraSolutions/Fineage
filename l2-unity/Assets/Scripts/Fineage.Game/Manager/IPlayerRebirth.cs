/// <summary>
/// Local-player rebirth <c>World.Revive</c> applies (#131 S38q).
/// <c>GameLifetimeScope</c> registers <c>PlayerRebirthRelay</c> as this interface.
/// <c>PlayerStateMachine.AdoptFromPlayer</c> attaches the live machine after spawn.
/// </summary>
public interface IPlayerRebirth
{
    void NotifyRebirth();
}
