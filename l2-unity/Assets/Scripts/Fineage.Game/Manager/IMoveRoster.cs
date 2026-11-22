/// <summary>
/// Move roster <c>World</c> forgets and queries (#131 S38d).
/// <c>GetMove</c> stays on <c>IMoveAllCharacters</c> because it returns <c>MovementData</c>.
/// </summary>
public interface IMoveRoster
{
    bool IsMoving(int objId);

    void Forget(int objId);
}
