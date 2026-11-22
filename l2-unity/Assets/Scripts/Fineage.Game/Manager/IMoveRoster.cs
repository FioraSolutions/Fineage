/// <summary>
/// Move roster <c>World</c> injects (#131 S38d, S38l).
/// <c>GameLifetimeScope</c> registers the scene <c>MoveAllCharacters</c> as this interface.
/// <c>GetMove</c> stays on <c>IMoveAllCharacters</c> because it returns <c>MovementData</c>.
/// </summary>
public interface IMoveRoster
{
    bool IsMoving(int objId);

    void Forget(int objId);
}
