using UnityEngine;

public class StopMoveDto : IWireDto
{
    private int _objectId;
    private int _x;
    private int _y;
    private int _z;
    private int _heading;
    private Vector3 _stopPos;

    public int ObjId { get => _objectId; }
    public Vector3 StopPos { get => _stopPos; }
    public int X { get => _x; }
    public int Y { get => _y; }
    public int Z { get => _z; }
    public int Heading { get => _heading; }

    /// <summary>Unity-space stop. Used when the domain command is applied back onto the intention layer.</summary>
    public static StopMoveDto FromUnity(int objId, Vector3 stopPos, int heading)
    {
        var dto = new StopMoveDto();
        dto._objectId = objId;
        dto._stopPos = stopPos;
        dto._heading = heading;
        return dto;
    }

    public void ReadFrom(PacketReader reader)
    {
        _objectId = reader.ReadI();

        _x = reader.ReadI();
        _y = reader.ReadI();
        _z = reader.ReadI();
        _heading = reader.ReadI();

        _stopPos = VectorUtils.ConvertPosToUnity(new Vector3(_x, _y, _z));
    }
}
