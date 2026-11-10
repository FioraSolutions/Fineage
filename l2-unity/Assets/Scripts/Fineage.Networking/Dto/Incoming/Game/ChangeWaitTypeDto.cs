using UnityEngine;

public sealed class ChangeWaitTypeDto : IWireDto
{
    public int ObjectId { get; private set; }
    public WaitType WaitType { get; private set; }
    public Vector3 Position { get; private set; }

    public ChangeWaitTypeDto() { }

    ChangeWaitTypeDto(int objectId, int waitType, int x, int y, int z)
    {
        ObjectId = objectId;
        WaitType = (WaitType)waitType;
        Position = VectorUtils.ConvertPosToUnity(new Vector3(x, y, z));
    }

    public void ReadFrom(PacketReader reader)
    {
        ObjectId = reader.ReadI();
        WaitType = (WaitType)reader.ReadI();
        int x = reader.ReadI();
        int y = reader.ReadI();
        int z = reader.ReadI();
        Position = VectorUtils.ConvertPosToUnity(new Vector3(x, y, z));
    }

    /// <summary>Net <c>ChangeWaitType</c> (sit/stand + the L2 position the server settled the pawn to).</summary>
    public static ChangeWaitTypeDto FromNet(int objectId, int waitType, int x, int y, int z) =>
        new ChangeWaitTypeDto(objectId, waitType, x, y, z);
}
