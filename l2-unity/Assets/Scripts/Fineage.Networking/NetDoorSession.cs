using System.Collections.Generic;
using Fineage.Net.Contract;

/// <summary>Live door entity context keyed by zone object id (NpcType Door, npcId 0).</summary>
public static class NetDoorSession
{
    static readonly HashSet<int> PendingDoorObjectIds = new HashSet<int>();
    static readonly Dictionary<int, DoorStatusNetMsg> LatestByObjectId = new Dictionary<int, DoorStatusNetMsg>();

    public static void Reset()
    {
        PendingDoorObjectIds.Clear();
        LatestByObjectId.Clear();
    }

    public static bool IsLiveDoorObject(int objectId) =>
        objectId > 0 && (PendingDoorObjectIds.Contains(objectId) || LatestByObjectId.ContainsKey(objectId));

    public static void RememberDoorSpawn(int objectId)
    {
        if (objectId > 0)
            PendingDoorObjectIds.Add(objectId);
    }

    public static void Forget(int objectId)
    {
        PendingDoorObjectIds.Remove(objectId);
        LatestByObjectId.Remove(objectId);
    }

    public static void NoteStatus(DoorStatusNetMsg msg)
    {
        if (msg == null || msg.ObjectId <= 0)
            return;
        PendingDoorObjectIds.Remove(msg.ObjectId);
        LatestByObjectId[msg.ObjectId] = msg;
    }

    public static bool TryGetStatus(int objectId, out DoorStatusNetMsg status) =>
        LatestByObjectId.TryGetValue(objectId, out status);
}