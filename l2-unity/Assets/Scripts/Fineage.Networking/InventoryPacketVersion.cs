using System;
using System.Buffers.Binary;
using Fineage.Net.Contract;

/// <summary>
/// Inventory version sits after the existing ItemList / InventoryUpdate body as one int32.
/// The synced contract does not carry the field yet (Backend docs/inventory.md). A payload
/// that round-trips through the current codec has no version. Four extra bytes are the version.
/// Anything else is an error: the client does not guess a version out of a mismatched body.
/// </summary>
public static class InventoryPacketVersion
{
    public const int Absent = 0;
    public const int Unreadable = -1;

    public static int ReadItemList(ItemListNetMsg decoded, byte[] payload, out string error)
    {
        if (decoded == null)
        {
            error = "ItemList message is null";
            return Unreadable;
        }
        if (payload == null)
        {
            error = "ItemList payload is missing; inventory version cannot be read";
            return Unreadable;
        }
        return ReadTail(payload, NetCodec.EncodeItemList(decoded), "ItemList", out error);
    }

    public static int ReadUpdate(InventoryUpdateNetMsg decoded, byte[] payload, out string error)
    {
        if (decoded == null)
        {
            error = "InventoryUpdate message is null";
            return Unreadable;
        }
        if (payload == null)
        {
            error = "InventoryUpdate payload is missing; inventory version cannot be read";
            return Unreadable;
        }
        return ReadTail(payload, NetCodec.EncodeInventoryUpdate(decoded), "InventoryUpdate", out error);
    }

    static int ReadTail(byte[] payload, byte[] canonical, string name, out string error)
    {
        error = null;
        if (canonical == null)
        {
            error = name + " could not be re-encoded; inventory version cannot be read";
            return Unreadable;
        }

        int extra = payload.Length - canonical.Length;
        if (extra != 0 && extra != 4)
        {
            error = name + " has " + extra + " unexpected trailing bytes; expected 0 or a 4-byte inventory version";
            return Unreadable;
        }
        if (!SamePrefix(payload, canonical))
        {
            error = name + " body does not match the decoded message; inventory version cannot be read";
            return Unreadable;
        }
        if (extra == 0)
            return Absent;

        int version = BinaryPrimitives.ReadInt32LittleEndian(payload.AsSpan(payload.Length - 4));
        if (version < 0)
        {
            error = name + " inventory version " + version + " is negative";
            return Unreadable;
        }
        return version;
    }

    static bool SamePrefix(byte[] payload, byte[] canonical)
    {
        if (payload.Length < canonical.Length)
            return false;
        for (int i = 0; i < canonical.Length; i++)
        {
            if (payload[i] != canonical[i])
                return false;
        }
        return true;
    }
}
