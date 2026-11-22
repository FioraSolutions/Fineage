using System.Collections.Generic;

/// <summary>
/// What the inventory window, paperdoll, and counts paint. Built only from <see cref="CharacterInventory"/>.
/// </summary>
public sealed class InventorySurface
{
    public int[] BagObjectIds = System.Array.Empty<int>();
    public int[] BagCounts = System.Array.Empty<int>();
    public int[] PaperdollSlots = System.Array.Empty<int>();
    public int[] PaperdollObjectIds = System.Array.Empty<int>();
    public int Adena;
    public int UsedSlots;

    public static InventorySurface Project(CharacterInventory book)
    {
        var surface = new InventorySurface();
        if (book == null)
            return surface;

        CharacterInventory.Row[] bag = book.BagRows();
        surface.BagObjectIds = new int[bag.Length];
        surface.BagCounts = new int[bag.Length];
        for (int i = 0; i < bag.Length; i++)
        {
            surface.BagObjectIds[i] = bag[i].ObjectId;
            surface.BagCounts[i] = bag[i].Count;
        }

        CharacterInventory.Row[] worn = book.PaperdollRows();
        surface.PaperdollSlots = new int[worn.Length];
        surface.PaperdollObjectIds = new int[worn.Length];
        for (int i = 0; i < worn.Length; i++)
        {
            surface.PaperdollSlots[i] = worn[i].EquippedSlot;
            surface.PaperdollObjectIds[i] = worn[i].ObjectId;
        }

        surface.Adena = book.Adena;
        surface.UsedSlots = book.Count;
        return surface;
    }

    /// <summary>
    /// Bag slots the window assigns by index. 0 is an empty cell. Every bag objectId is written once.
    /// </summary>
    public static int[] BagSlots(InventorySurface surface, int capacity)
    {
        int n = capacity < 0 ? 0 : capacity;
        var slots = new int[n];
        if (surface == null || surface.BagObjectIds == null)
            return slots;
        int limit = surface.BagObjectIds.Length < n ? surface.BagObjectIds.Length : n;
        for (int i = 0; i < limit; i++)
            slots[i] = surface.BagObjectIds[i];
        return slots;
    }

    public static List<string> Check(CharacterInventory book)
    {
        var errors = new List<string>();
        if (book == null)
        {
            errors.Add("inventory book is null");
            return errors;
        }

        InventorySurface surface = Project(book);
        var seen = new HashSet<int>();
        if (surface.BagObjectIds.Length != surface.BagCounts.Length)
            errors.Add("bag ids and counts differ in length");

        for (int i = 0; i < surface.BagObjectIds.Length; i++)
        {
            int id = surface.BagObjectIds[i];
            CharacterInventory.Row row;
            if (!book.TryGet(id, out row))
                errors.Add("bag shows objectId " + id + " which is not in the book");
            else if (row.EquippedSlot >= 0)
                errors.Add("objectId " + id + " is equipped and also in the bag");
            else if (i < surface.BagCounts.Length && surface.BagCounts[i] != row.Count)
                errors.Add("bag count for objectId " + id + " is " + surface.BagCounts[i] + " but the book has " + row.Count);
            if (!seen.Add(id))
                errors.Add("bag objectId " + id + " is duplicated");
        }

        if (surface.PaperdollSlots.Length != surface.PaperdollObjectIds.Length)
            errors.Add("paperdoll slots and ids differ in length");

        var usedSlots = new HashSet<int>();
        for (int i = 0; i < surface.PaperdollObjectIds.Length; i++)
        {
            int id = surface.PaperdollObjectIds[i];
            int slot = i < surface.PaperdollSlots.Length ? surface.PaperdollSlots[i] : -1;
            CharacterInventory.Row row;
            if (!book.TryGet(id, out row))
                errors.Add("paperdoll shows objectId " + id + " which is not in the book");
            else if (row.EquippedSlot != slot)
                errors.Add("objectId " + id + " is painted in slot " + slot + " but the book says " + row.EquippedSlot);
            if (!usedSlots.Add(slot))
                errors.Add("paperdoll slot " + slot + " is painted twice");
            if (!seen.Add(id))
                errors.Add("objectId " + id + " is in the bag and on the paperdoll");
        }

        int[] ids = book.ObjectIds();
        for (int i = 0; i < ids.Length; i++)
        {
            if (!seen.Contains(ids[i]))
                errors.Add("objectId " + ids[i] + " is in the book but not in the bag or on the paperdoll");
        }

        if (surface.Adena != book.Adena)
            errors.Add("adena label " + surface.Adena + " does not match the book " + book.Adena);
        if (surface.UsedSlots != book.Count)
            errors.Add("slot count " + surface.UsedSlots + " does not match the book " + book.Count);
        if (surface.BagObjectIds.Length + surface.PaperdollObjectIds.Length != book.Count && errors.Count == 0)
            errors.Add("painted rows " + (surface.BagObjectIds.Length + surface.PaperdollObjectIds.Length) + " do not match book " + book.Count);

        int[] painted = BagSlots(surface, surface.BagObjectIds.Length);
        for (int i = 0; i < surface.BagObjectIds.Length; i++)
        {
            if (i >= painted.Length || painted[i] != surface.BagObjectIds[i])
                errors.Add("bag slot " + i + " does not hold objectId " + surface.BagObjectIds[i]);
        }

        return errors;
    }
}
