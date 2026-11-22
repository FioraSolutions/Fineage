using System.Collections.Generic;
using NUnit.Framework;

public sealed class CharacterInventoryEditModeTests
{
    [Test]
    public void BuyDestroyEquipUnequip_has_no_duplicate_or_missing_objectIds()
    {
        CharacterInventory book = new CharacterInventory();
        var errors = new List<string>();
        Assert.AreEqual(CharacterInventory.ApplyStatus.Applied, book.Replace(new[]
        {
            Entry(1, 57, 1000),
            Entry(2, 2369, 1),
            Entry(3, 1835, 5)
        }, 3, errors));
        AssertNoProblems(book, errors);
        Assert.AreEqual(1000, book.Adena);
        Assert.AreEqual(3, book.Count);

        Assert.AreEqual(CharacterInventory.ApplyStatus.Applied, book.Apply(new[]
        {
            Change(CharacterInventory.Modify, Entry(1, 57, 800)),
            Change(CharacterInventory.Add, Entry(4, 2493, 1))
        }, 4, errors));
        AssertNoProblems(book, errors);
        CollectionAssert.AreEqual(new[] { 1, 2, 3, 4 }, BagIds(book));
        Assert.AreEqual(800, book.Adena);
        Assert.AreEqual(1, CountOf(book, 4));

        Assert.AreEqual(CharacterInventory.ApplyStatus.Applied, book.Apply(new[]
        {
            Change(CharacterInventory.Modify, Entry(3, 1835, 9))
        }, 5, errors));
        AssertNoProblems(book, errors);
        Assert.AreEqual(9, CountOf(book, 3));
        Assert.AreEqual(1, RowsWithItem(book, 1835));

        Assert.AreEqual(CharacterInventory.ApplyStatus.Applied, book.Apply(new[]
        {
            Change(CharacterInventory.Modify, Entry(3, 1835, 2))
        }, 6, errors));
        AssertNoProblems(book, errors);
        Assert.AreEqual(2, CountOf(book, 3));

        Assert.AreEqual(CharacterInventory.ApplyStatus.Applied, book.Apply(new[]
        {
            Change(CharacterInventory.Remove, Entry(2, 2369, 1))
        }, 7, errors));
        AssertNoProblems(book, errors);
        Assert.IsFalse(book.Contains(2));
        CollectionAssert.AreEqual(new[] { 1, 3, 4 }, BagIds(book));
        Assert.AreEqual(3, book.Count);

        Assert.AreEqual(CharacterInventory.ApplyStatus.Applied, book.Apply(new[]
        {
            Change(CharacterInventory.Modify, Entry(4, 2493, 1, 8))
        }, 8, errors));
        AssertNoProblems(book, errors);
        CollectionAssert.AreEqual(new[] { 1, 3 }, BagIds(book));
        CollectionAssert.AreEqual(new[] { 4 }, PaperdollIds(book));
        Assert.AreEqual(8, PaperdollSlot(book, 4));

        Assert.AreEqual(CharacterInventory.ApplyStatus.Applied, book.Apply(new[]
        {
            Change(CharacterInventory.Modify, Entry(4, 2493, 1))
        }, 9, errors));
        AssertNoProblems(book, errors);
        CollectionAssert.AreEqual(new[] { 1, 3, 4 }, BagIds(book));
        Assert.AreEqual(0, book.PaperdollRows().Length);
        Assert.AreEqual(800, book.Adena);
        Assert.AreEqual(2, CountOf(book, 3));
        Assert.AreEqual(1, CountOf(book, 4));
    }

    [Test]
    public void AddOfExistingObjectId_replaces_that_row()
    {
        CharacterInventory book = new CharacterInventory();
        var errors = new List<string>();
        book.Replace(new[] { Entry(7, 57, 10) }, 1, errors);
        errors.Clear();
        book.Apply(new[] { Change(CharacterInventory.Add, Entry(7, 57, 25)) }, 2, errors);
        Assert.AreEqual(1, book.Count);
        Assert.AreEqual(25, book.Adena);
        Assert.IsTrue(Joined(errors).Contains("already exists"));
        AssertNoProblems(book, new List<string>());
    }

    [Test]
    public void VersionGap_does_not_apply_and_requests_resync()
    {
        CharacterInventory book = new CharacterInventory();
        var errors = new List<string>();
        book.Replace(new[] { Entry(1, 57, 100) }, 4, errors);
        int generation = book.Generation;
        errors.Clear();
        CharacterInventory.ApplyStatus status = book.Apply(new[]
        {
            Change(CharacterInventory.Add, Entry(2, 2369, 1))
        }, 6, errors);
        Assert.AreEqual(CharacterInventory.ApplyStatus.Gap, status);
        Assert.IsTrue(book.ResyncRequired);
        Assert.AreEqual(generation, book.Generation);
        Assert.IsFalse(book.Contains(2));
        Assert.AreEqual(100, book.Adena);
        Assert.AreEqual(0, InventorySurface.Check(book).Count);
    }

    [Test]
    public void Replay_is_ignored()
    {
        CharacterInventory book = new CharacterInventory();
        var errors = new List<string>();
        book.Replace(new[] { Entry(1, 57, 100) }, 4, errors);
        book.Apply(new[] { Change(CharacterInventory.Modify, Entry(1, 57, 90)) }, 5, errors);
        int generation = book.Generation;
        errors.Clear();
        Assert.AreEqual(
            CharacterInventory.ApplyStatus.IgnoredReplay,
            book.Apply(new[] { Change(CharacterInventory.Modify, Entry(1, 57, 1)) }, 5, errors));
        Assert.AreEqual(generation, book.Generation);
        Assert.AreEqual(90, book.Adena);
    }

    [Test]
    public void UnknownChange_is_rejected_without_a_partial_apply()
    {
        CharacterInventory book = new CharacterInventory();
        var errors = new List<string>();
        book.Replace(new[] { Entry(1, 57, 100) }, 1, errors);
        errors.Clear();
        CharacterInventory.ApplyStatus status = book.Apply(new[]
        {
            Change(CharacterInventory.Modify, Entry(1, 57, 50)),
            Change(9, Entry(2, 2369, 1))
        }, 2, errors);
        Assert.AreEqual(CharacterInventory.ApplyStatus.Rejected, status);
        Assert.IsTrue(book.ResyncRequired);
        Assert.AreEqual(100, book.Adena);
        Assert.IsFalse(book.Contains(2));
    }

    [Test]
    public void ItemList_replaces_the_book()
    {
        CharacterInventory book = new CharacterInventory();
        var errors = new List<string>();
        book.Replace(new[] { Entry(1, 57, 5), Entry(2, 2369, 1, 7) }, 1, errors);
        errors.Clear();
        book.Replace(new[] { Entry(9, 57, 1) }, 2, errors);
        AssertNoProblems(book, errors);
        Assert.IsFalse(book.Contains(1));
        Assert.IsFalse(book.Contains(2));
        CollectionAssert.AreEqual(new[] { 9 }, BagIds(book));
        Assert.AreEqual(0, book.PaperdollRows().Length);
    }

    [Test]
    public void DuplicateObjectId_in_ItemList_keeps_one_row()
    {
        CharacterInventory book = new CharacterInventory();
        var errors = new List<string>();
        book.Replace(new[] { Entry(4, 2369, 1), Entry(4, 2369, 3) }, 1, errors);
        Assert.AreEqual(1, book.Count);
        Assert.AreEqual(3, CountOf(book, 4));
        Assert.IsTrue(book.ResyncRequired);
        Assert.AreEqual(0, InventorySurface.Check(book).Count);
    }

    [Test]
    public void Shortcut_resolves_objectId_then_a_single_itemId()
    {
        CharacterInventory book = new CharacterInventory();
        var errors = new List<string>();
        book.Replace(new[] { Entry(5, 1835, 4), Entry(6, 2369, 1), Entry(8, 2369, 1) }, 1, errors);
        CharacterInventory.Row row;
        string error;
        Assert.IsTrue(book.TryResolveShortcut(5, 1835, out row, out error));
        Assert.AreEqual(4, row.Count);
        Assert.IsNull(error);

        Assert.IsTrue(book.TryResolveShortcut(0, 1835, out row, out error));
        Assert.AreEqual(5, row.ObjectId);

        Assert.IsFalse(book.TryResolveShortcut(0, 2369, out row, out error));
        Assert.IsTrue(error.Contains("2 objectIds"));
    }

    [Test]
    public void BagSlots_match_the_book_order()
    {
        CharacterInventory book = new CharacterInventory();
        var errors = new List<string>();
        book.Replace(new[] { Entry(2, 2369, 1), Entry(1, 57, 3) }, 1, errors);
        InventorySurface surface = InventorySurface.Project(book);
        int[] slots = InventorySurface.BagSlots(surface, 4);
        CollectionAssert.AreEqual(new[] { 2, 1, 0, 0 }, slots);
        Assert.AreEqual(3, surface.Adena);
        Assert.AreEqual(2, surface.UsedSlots);
    }

    static void AssertNoProblems(CharacterInventory book, List<string> errors)
    {
        Assert.AreEqual("", Joined(errors), Joined(errors));
        List<string> problems = InventorySurface.Check(book);
        Assert.AreEqual("", Joined(problems), Joined(problems));
        Assert.IsFalse(book.ResyncRequired);
    }

    static string Joined(List<string> lines)
    {
        if (lines == null || lines.Count == 0)
            return "";
        return string.Join("; ", lines.ToArray());
    }

    static int[] BagIds(CharacterInventory book)
    {
        CharacterInventory.Row[] rows = book.BagRows();
        var ids = new int[rows.Length];
        for (int i = 0; i < rows.Length; i++)
            ids[i] = rows[i].ObjectId;
        return ids;
    }

    static int[] PaperdollIds(CharacterInventory book)
    {
        CharacterInventory.Row[] rows = book.PaperdollRows();
        var ids = new int[rows.Length];
        for (int i = 0; i < rows.Length; i++)
            ids[i] = rows[i].ObjectId;
        return ids;
    }

    static int PaperdollSlot(CharacterInventory book, int objectId)
    {
        CharacterInventory.Row[] rows = book.PaperdollRows();
        for (int i = 0; i < rows.Length; i++)
        {
            if (rows[i].ObjectId == objectId)
                return rows[i].EquippedSlot;
        }
        return -1;
    }

    static int CountOf(CharacterInventory book, int objectId)
    {
        CharacterInventory.Row row;
        Assert.IsTrue(book.TryGet(objectId, out row));
        return row.Count;
    }

    static int RowsWithItem(CharacterInventory book, int itemId)
    {
        int n = 0;
        int[] ids = book.ObjectIds();
        for (int i = 0; i < ids.Length; i++)
        {
            CharacterInventory.Row row;
            book.TryGet(ids[i], out row);
            if (row.ItemId == itemId)
                n++;
        }
        return n;
    }

    static CharacterInventory.Row Entry(int objectId, int itemId, int count, int equippedSlot = -1)
    {
        return new CharacterInventory.Row
        {
            ObjectId = objectId,
            ItemId = itemId,
            Count = count,
            EquippedSlot = equippedSlot,
            BodySlot = equippedSlot >= 0 ? "lhand" : "none",
            Kind = itemId == 57 ? "etc" : "weapon",
            Icon = "",
            WeaponType = "",
            ArmorType = ""
        };
    }

    static CharacterInventory.Change Change(int code, CharacterInventory.Row row)
    {
        return new CharacterInventory.Change { Code = code, Row = row };
    }
}
