using System.Collections.Generic;
using Fineage.Net.Contract;

/// <summary>Tracks open private-store type per player object id (from <see cref="PrivateStoreInfoNetMsg"/>).</summary>
public static class NetPlayerStoreRegistry
{
    sealed class Entry
    {
        public int StoreType;
        public string Message;
    }

    static readonly Dictionary<int, Entry> ByObjectId = new Dictionary<int, Entry>();

    public static void Reset() => ByObjectId.Clear();

    public static void Note(PrivateStoreInfoNetMsg msg)
    {
        if (msg == null)
            return;
        if (!PrivateStoreProtocolClient.IsOpen((byte)msg.StoreType))
        {
            ByObjectId.Remove(msg.ObjectId);
            return;
        }

        ByObjectId[msg.ObjectId] = new Entry
        {
            StoreType = msg.StoreType,
            Message = msg.Message ?? ""
        };
    }

    public static bool TryGet(int objectId, out int storeType, out string message)
    {
        if (ByObjectId.TryGetValue(objectId, out Entry entry))
        {
            storeType = entry.StoreType;
            message = entry.Message ?? "";
            return true;
        }

        storeType = PrivateStoreProtocolClient.TypeNone;
        message = "";
        return false;
    }

    public static bool IsManufactureStore(int objectId) =>
        TryGet(objectId, out int type, out _) && PrivateStoreProtocolClient.IsManufacture((byte)type);
}