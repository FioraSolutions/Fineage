using UnityEngine;

public class TeleportToLocationDto : IWireDto
{
    private int _targetObjId;
    private int _x;
    private int _y;
    private int _z;
    private int _heading;
    private Vector3 _telePos;

    public int TarObjId { get => _targetObjId; }
    public Vector3 TeleportPos { get => _telePos; }
    public int X { get => _x; }
    public int Y { get => _y; }
    public int Z { get => _z; }

    /// <summary>Zone V2 <see cref="ZoneV2MsgType.TeleportToLocation"/> (1072).</summary>
    public static TeleportToLocationDto FromZoneWire(int objectId, int x, int y, int z, int heading)
    {
        var dto = new TeleportToLocationDto();
        dto._targetObjId = objectId;
        dto._x = x;
        dto._y = y;
        dto._z = z;
        dto._heading = heading;
        dto._telePos = VectorUtils.ConvertPosToUnity(new Vector3(x, y, z));
        return dto;
    }
    

    public void ReadFrom(PacketReader reader)
    {
        _targetObjId = reader.ReadI();

        _x = reader.ReadI();
        _y = reader.ReadI();
        _z = reader.ReadI();
        int point = reader.ReadI(); // Fade 0, Instant 1.
        _heading = reader.ReadI();

        _telePos = VectorUtils.ConvertPosToUnity(new Vector3(_x, _y, _z));
    }
}
