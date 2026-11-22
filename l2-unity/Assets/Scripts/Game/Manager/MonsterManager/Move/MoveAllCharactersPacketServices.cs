/// <summary>
/// Move-all-characters access with [Inject] wiring and Controller/container fallback (#131).
/// </summary>
public static class MoveAllCharactersPacketServices
{
    private static IMoveAllCharacters _moves;

    public static void Bind(IMoveAllCharacters moves = null)
    {
        if (moves != null)
            _moves = moves;
    }

    /// <summary>The roster <see cref="Bind"/> stored. No container or <c>Instance</c> fallback.</summary>
    public static IMoveRoster BoundMoves => _moves;

    public static IMoveAllCharacters Moves =>
        _moves != null
            ? _moves
            : ControllerPacketServices.Moves;
}
