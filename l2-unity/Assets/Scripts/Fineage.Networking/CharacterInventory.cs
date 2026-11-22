using System;
using System.Collections.Generic;

/// <summary>
/// The character's inventory. One map, keyed by server objectId.
/// <see cref="Replace"/> is an ItemList (the map becomes that list).
/// <see cref="Apply"/> is an InventoryUpdate (add / modify / remove by objectId, in order).
/// Equipped is a field on the row (EquippedSlot &gt;= 0), not a second map.
/// Bag order is the packet order, with new rows appended. Nothing else inserts rows.
/// </summary>
public sealed class CharacterInventory
{
    public const int Add = 0;
    public const int Modify = 1;
    public const int Remove = 2;
    public const int AdenaItemId = 57;
    /// <summary>A tail that is not 0 or 4 bytes, or a negative int32. The book must not change.</summary>
    public const int UnreadableVersion = -1;

    public enum ApplyStatus
    {
        Applied,
        IgnoredReplay,
        Gap,
        Rejected
    }

    public struct Row
    {
        public int ObjectId;
        public int ItemId;
        public int Count;
        public int EnchantLevel;
        public int EquippedSlot;
        public string BodySlot;
        public string Kind;
        public string Icon;
        public string WeaponType;
        public string ArmorType;
    }

    public struct Change
    {
        public int Code;
        public Row Row;
    }

    readonly Dictionary<int, Row> _rows = new Dictionary<int, Row>();
    readonly List<int> _order = new List<int>();

    public int Version { get; private set; }
    public bool HasVersion { get; private set; }
    public bool ResyncRequired { get; private set; }
    public int Generation { get; private set; }
    public int Count { get { return _rows.Count; } }

    public int Adena
    {
        get
        {
            int total = 0;
            for (int i = 0; i < _order.Count; i++)
            {
                Row row = _rows[_order[i]];
                if (row.ItemId == AdenaItemId)
                    total += row.Count;
            }
            return total;
        }
    }

    public bool Contains(int objectId)
    {
        return _rows.ContainsKey(objectId);
    }

    public bool TryGet(int objectId, out Row row)
    {
        return _rows.TryGetValue(objectId, out row);
    }

    public int[] ObjectIds()
    {
        var ids = new int[_order.Count];
        for (int i = 0; i < _order.Count; i++)
            ids[i] = _order[i];
        return ids;
    }

    /// <summary>
    /// The zone appends one little-endian int32 after the codec body
    /// (Fineage-Backend docs/inventory.md). Re-encoding the decoded message
    /// is the body. Zero extra bytes is unversioned (return 0). Four extra
    /// bytes are the version. Any other length, or a negative int32, is
    /// <see cref="UnreadableVersion"/>.
    /// </summary>
    public static int ReadVersionTail(byte[] payload, byte[] body)
    {
        int payloadLength = payload == null ? 0 : payload.Length;
        int bodyLength = body == null ? 0 : body.Length;
        int extra = payloadLength - bodyLength;
        if (extra == 0)
            return 0;
        if (extra != 4 || payload == null)
            return UnreadableVersion;
        int version = System.Buffers.Binary.BinaryPrimitives.ReadInt32LittleEndian(
            new System.ReadOnlySpan<byte>(payload, bodyLength, 4));
        if (version < 0)
            return UnreadableVersion;
        return version;
    }

    public ApplyStatus Replace(IReadOnlyList<Row> rows, int version, List<string> errors)
    {
        if (errors == null)
            throw new ArgumentNullException(nameof(errors));
        if (version < 0)
        {
            int expected = HasVersion ? Version : 1;
            errors.Add("ItemList version " + version + " received; expected " + expected + "; snapshot not applied");
            ResyncRequired = true;
            return ApplyStatus.Rejected;
        }
        if (rows == null)
        {
            errors.Add("ItemList rows are null");
            ResyncRequired = true;
            return ApplyStatus.Rejected;
        }

        var next = new Dictionary<int, Row>();
        var order = new List<int>(rows.Count);
        bool dirty = false;
        for (int i = 0; i < rows.Count; i++)
        {
            Row row = Normalize(rows[i]);
            string problem = ValidateRow(row, false);
            if (problem != null)
            {
                errors.Add("ItemList row " + i + ": " + problem);
                dirty = true;
                continue;
            }
            if (next.ContainsKey(row.ObjectId))
            {
                errors.Add("ItemList duplicate objectId " + row.ObjectId + "; keeping the later row");
                dirty = true;
                next[row.ObjectId] = row;
                continue;
            }
            if (row.Count == 0)
                errors.Add("ItemList objectId " + row.ObjectId + " has count 0; row kept until the server removes it");
            next.Add(row.ObjectId, row);
            order.Add(row.ObjectId);
        }

        _rows.Clear();
        _order.Clear();
        foreach (KeyValuePair<int, Row> pair in next)
            _rows.Add(pair.Key, pair.Value);
        for (int i = 0; i < order.Count; i++)
            _order.Add(order[i]);

        if (version > 0)
        {
            Version = version;
            HasVersion = true;
        }
        else if (HasVersion)
        {
            errors.Add("ItemList version 0 received; expected " + Version + "; cursor left at " + Version);
        }

        if (NotePaperdollCollisions(errors))
            dirty = true;

        ResyncRequired = dirty;
        Generation++;
        return ApplyStatus.Applied;
    }

    public ApplyStatus Apply(IReadOnlyList<Change> changes, int version, List<string> errors)
    {
        if (errors == null)
            throw new ArgumentNullException(nameof(errors));
        if (changes == null)
        {
            errors.Add("InventoryUpdate changes are null");
            ResyncRequired = true;
            return ApplyStatus.Rejected;
        }

        ApplyStatus versionStatus = AcceptUpdateVersion(version, errors);
        if (versionStatus != ApplyStatus.Applied)
            return versionStatus;

        for (int i = 0; i < changes.Count; i++)
        {
            int code = changes[i].Code;
            if (code != Add && code != Modify && code != Remove)
            {
                errors.Add("InventoryUpdate change " + i + " has unknown code " + code + " for objectId " + changes[i].Row.ObjectId);
                ResyncRequired = true;
                return ApplyStatus.Rejected;
            }
            bool removing = code == Remove;
            string problem = ValidateRow(changes[i].Row, removing);
            if (problem != null)
            {
                errors.Add("InventoryUpdate change " + i + ": " + problem);
                ResyncRequired = true;
                return ApplyStatus.Rejected;
            }
        }

        for (int i = 0; i < changes.Count; i++)
            ApplyOne(changes[i], errors);

        if (version > 0)
        {
            Version = version;
            HasVersion = true;
        }

        if (NotePaperdollCollisions(errors))
            ResyncRequired = true;

        Generation++;
        return ApplyStatus.Applied;
    }

    public Row[] BagRows()
    {
        var list = new List<Row>();
        for (int i = 0; i < _order.Count; i++)
        {
            Row row = _rows[_order[i]];
            if (row.EquippedSlot < 0)
                list.Add(row);
        }
        return list.ToArray();
    }

    /// <summary>One row per equipped slot. When two rows claim a slot, the later one is returned.</summary>
    public Row[] PaperdollRows()
    {
        var bySlot = new Dictionary<int, Row>();
        var slots = new List<int>();
        for (int i = 0; i < _order.Count; i++)
        {
            Row row = _rows[_order[i]];
            if (row.EquippedSlot < 0)
                continue;
            if (!bySlot.ContainsKey(row.EquippedSlot))
                slots.Add(row.EquippedSlot);
            bySlot[row.EquippedSlot] = row;
        }
        slots.Sort();
        var result = new Row[slots.Count];
        for (int i = 0; i < slots.Count; i++)
            result[i] = bySlot[slots[i]];
        return result;
    }

    public int SumItem(int itemId)
    {
        int total = 0;
        for (int i = 0; i < _order.Count; i++)
        {
            Row row = _rows[_order[i]];
            if (row.ItemId == itemId)
                total += row.Count;
        }
        return total;
    }

    public bool TryResolveShortcut(int objectId, int itemIdHint, out Row row, out string error)
    {
        error = null;
        row = default;
        if (objectId > 0 && _rows.TryGetValue(objectId, out row))
            return true;

        row = default;
        if (itemIdHint <= 0)
            return false;

        int found = 0;
        Row match = default;
        for (int i = 0; i < _order.Count; i++)
        {
            Row candidate = _rows[_order[i]];
            if (candidate.ItemId != itemIdHint)
                continue;
            found++;
            if (found == 1)
                match = candidate;
        }

        if (found == 1)
        {
            row = match;
            return true;
        }
        if (found > 1)
            error = "shortcut itemId " + itemIdHint + " matches " + found + " objectIds";
        return false;
    }

    ApplyStatus AcceptUpdateVersion(int version, List<string> errors)
    {
        int expected = Version + 1;
        if (version < 0)
        {
            errors.Add("InventoryUpdate version " + version + " received; expected " + expected + "; rows not applied");
            ResyncRequired = true;
            return ApplyStatus.Rejected;
        }
        if (version == 0)
        {
            if (!HasVersion)
                return ApplyStatus.Applied;
            errors.Add("InventoryUpdate version 0 received; expected " + expected + "; rows not applied");
            ResyncRequired = true;
            return ApplyStatus.Rejected;
        }
        if (version == expected)
            return ApplyStatus.Applied;
        if (version <= Version)
        {
            errors.Add("InventoryUpdate version " + version + " received; expected " + expected + "; replay ignored");
            return ApplyStatus.IgnoredReplay;
        }
        errors.Add("InventoryUpdate version " + version + " received; expected " + expected + "; rows not applied");
        ResyncRequired = true;
        return ApplyStatus.Gap;
    }

    void ApplyOne(Change change, List<string> errors)
    {
        Row row = Normalize(change.Row);
        if (change.Code == Remove)
        {
            if (!_rows.Remove(row.ObjectId))
                errors.Add("InventoryUpdate remove objectId " + row.ObjectId + " is not in the inventory");
            _order.Remove(row.ObjectId);
            return;
        }

        if (row.Count == 0)
            errors.Add("InventoryUpdate objectId " + row.ObjectId + " has count 0; row kept until the server removes it");

        if (_rows.ContainsKey(row.ObjectId))
        {
            if (change.Code == Add)
                errors.Add("InventoryUpdate add objectId " + row.ObjectId + " already exists; replacing that row");
            _rows[row.ObjectId] = row;
            return;
        }

        if (change.Code == Modify)
        {
            errors.Add("InventoryUpdate modify objectId " + row.ObjectId + " was not in the inventory; inserting the server row");
            ResyncRequired = true;
        }
        _rows.Add(row.ObjectId, row);
        _order.Add(row.ObjectId);
    }

    bool NotePaperdollCollisions(List<string> errors)
    {
        bool collision = false;
        var owner = new Dictionary<int, int>();
        for (int i = 0; i < _order.Count; i++)
        {
            Row row = _rows[_order[i]];
            if (row.EquippedSlot < 0)
                continue;
            int previous;
            if (owner.TryGetValue(row.EquippedSlot, out previous))
            {
                errors.Add("paperdoll slot " + row.EquippedSlot + " is claimed by objectId " + previous + " and " + row.ObjectId);
                collision = true;
            }
            owner[row.EquippedSlot] = row.ObjectId;
        }
        return collision;
    }

    static Row Normalize(Row row)
    {
        row.BodySlot = row.BodySlot ?? "";
        row.Kind = row.Kind ?? "";
        row.Icon = row.Icon ?? "";
        row.WeaponType = row.WeaponType ?? "";
        row.ArmorType = row.ArmorType ?? "";
        return row;
    }

    static string ValidateRow(Row row, bool removing)
    {
        if (row.ObjectId <= 0)
            return "objectId " + row.ObjectId + " is not a real item";
        if (removing)
            return null;
        if (row.ItemId <= 0)
            return "objectId " + row.ObjectId + " has no itemId";
        if (row.Count < 0)
            return "objectId " + row.ObjectId + " count " + row.Count + " is negative";
        return null;
    }
}
