using NUnit.Framework;

/// <summary>
/// Interlude itemgrp identity (icon / body / grade) must apply generally so GM shop
/// jewelry and enchant scrolls resolve official Interlude art even when Classic
/// already has the row, or only Interlude has it.
/// </summary>
public sealed class InterludeItemgrpVisualEditModeTests
{
    [Test]
    public void FromInterludeArmor_MapsJewelryAndArmorSlots()
    {
        Assert.AreEqual(ItemSlot.underwear, ItemSlotParser.FromInterludeArmor(0));
        Assert.AreEqual(ItemSlot.rear, ItemSlotParser.FromInterludeArmor(1));
        Assert.AreEqual(ItemSlot.neck, ItemSlotParser.FromInterludeArmor(3));
        Assert.AreEqual(ItemSlot.rfinger, ItemSlotParser.FromInterludeArmor(4));
        Assert.AreEqual(ItemSlot.head, ItemSlotParser.FromInterludeArmor(6));
        Assert.AreEqual(ItemSlot.gloves, ItemSlotParser.FromInterludeArmor(9));
        Assert.AreEqual(ItemSlot.chest, ItemSlotParser.FromInterludeArmor(10));
        Assert.AreEqual(ItemSlot.legs, ItemSlotParser.FromInterludeArmor(11));
        Assert.AreEqual(ItemSlot.feet, ItemSlotParser.FromInterludeArmor(12));
        Assert.AreEqual(ItemSlot.fullarmor, ItemSlotParser.FromInterludeArmor(15));
        Assert.AreEqual(ItemSlot.alldress, ItemSlotParser.FromInterludeArmor(16));
        Assert.AreEqual(ItemSlot.hat1, ItemSlotParser.FromInterludeArmor(17));
        Assert.AreEqual(ItemSlot.hat2, ItemSlotParser.FromInterludeArmor(18));
        Assert.AreEqual(ItemSlot.hat1, ItemSlotParser.FromInterludeArmor(19));
        Assert.AreEqual(ItemSlot.none, ItemSlotParser.FromInterludeArmor(13));
        Assert.AreEqual(ItemSlot.none, ItemSlotParser.FromInterludeArmor(99));
    }
}
