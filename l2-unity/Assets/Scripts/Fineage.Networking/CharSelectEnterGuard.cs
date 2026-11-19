/// <summary>
/// Tracks a char-select Start attempt so the button can be re-enabled on any failure.
/// Pure state — no Unity dependency, so EditMode tests can cover the transitions.
/// </summary>
public sealed class CharSelectEnterGuard
{
    public const float TimeoutSeconds = 12f;

    public bool IsEntering { get; private set; }

    public bool TryBegin()
    {
        if (IsEntering)
            return false;
        IsEntering = true;
        return true;
    }

    public void Succeed()
    {
        IsEntering = false;
    }

    public void Fail()
    {
        IsEntering = false;
    }

    public bool HasTimedOut(float elapsedSeconds)
    {
        return IsEntering && elapsedSeconds >= TimeoutSeconds;
    }

    /// <summary>
    /// After Handoff the login server typically closes the lobby socket.
    /// That close must not abort a zone enter already in flight.
    /// </summary>
    public static bool ShouldAbortEnterOnLobbyClose(bool hasHandoff)
    {
        return !hasHandoff;
    }
}
