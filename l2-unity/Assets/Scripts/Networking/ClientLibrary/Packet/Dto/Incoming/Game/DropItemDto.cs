using UnityEngine;

/// <summary>aCis DropItem 0x0C: charObjId, objectId, itemId, x,y,z, stackable, count, unk.</summary>
public sealed class DropItemDto : IWireDto
{
    public int CharObjId { get; private set; }
    public int ObjectId { get; private set; }
    public int ItemId { get; private set; }
    public int X { get; private set; }
    public int Y { get; private set; }
    public int Z { get; private set; }
    public bool Stackable { get; private set; }
    public int Count { get; private set; }
    public int Unknown { get; private set; }
    public Vector3 UnityPos { get; private set; }

    public void ReadFrom(PacketReader reader)
    {
        CharObjId = reader.ReadI();
        ObjectId = reader.ReadI();
        ItemId = reader.ReadI();
        X = reader.ReadI();
        Y = reader.ReadI();
        Z = reader.ReadI();
        Stackable = reader.ReadI() != 0;
        Count = reader.ReadI();
        Unknown = reader.ReadI();
        UnityPos = VectorUtils.ConvertPosToUnity(new Vector3(X, Y, Z));
    }

    /// <summary>Zone V2 <see cref="ZoneV2MsgType.DropItem"/> (1019).</summary>
    public static DropItemDto FromZoneV2(
        int charObjectId,
        int objectId,
        int itemId,
        int x,
        int y,
        int z,
        bool stackable,
        int count,
        int unknown = 0)
    {
        var dto = new DropItemDto();
        dto.CharObjId = charObjectId;
        dto.ObjectId = objectId;
        dto.ItemId = itemId;
        dto.X = x;
        dto.Y = y;
        dto.Z = z;
        dto.Stackable = stackable;
        dto.Count = count;
        dto.Unknown = unknown;
        dto.UnityPos = VectorUtils.ConvertPosToUnity(new Vector3(x, y, z));
        return dto;
    }
}
