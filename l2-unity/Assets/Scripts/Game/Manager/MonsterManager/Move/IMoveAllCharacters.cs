
public interface IMoveAllCharacters : IMoveRoster
{
    public void AddMoveData(int id, MovementData data);
    public void AddRotate(int id, RotateData data);
    public void CancelMove(int objId);
    public void CancelRotate(int objId);
    MovementData GetMove(int objId);
}
