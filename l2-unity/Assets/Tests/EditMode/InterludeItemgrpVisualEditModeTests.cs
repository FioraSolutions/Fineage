using NUnit.Framework;

/// <summary>
/// Interlude itemgrp identity (icon / body / grade) must apply generally so GM shop
/// jewelry and enchant scrolls resolve official Interlude art even when Classic
/// already has the row, or only Interlude has it.
/// </summary>
public sealed class InterludeItemgrpVisualEditModeTests
{
    [Test]
    public void BodyPartFromInterlude_MapsJewelryAndArmorSlots()
    {
        Assert.AreEqual(ItemSlot.underwear, ArmorgrpTable.BodyPartFromInterlude(0));
        Assert.AreEqual(ItemSlot.rear, ArmorgrpTable.BodyPartFromInterlude(1));
        Assert.AreEqual(ItemSlot.neck, ArmorgrpTable.BodyPartFromInterlude(3));
        Assert.AreEqual(ItemSlot.rfinger, ArmorgrpTable.BodyPartFromInterlude(4));
        Assert.AreEqual(ItemSlot.head, ArmorgrpTable.BodyPartFromInterlude(6));
        Assert.AreEqual(ItemSlot.gloves, ArmorgrpTable.BodyPartFromInterlude(9));
        Assert.AreEqual(ItemSlot.chest, ArmorgrpTable.BodyPartFromInterlude(10));
        Assert.AreEqual(ItemSlot.legs, ArmorgrpTable.BodyPartFromInterlude(11));
        Assert.AreEqual(ItemSlot.feet, ArmorgrpTable.BodyPartFromInterlude(12));
        Assert.AreEqual(ItemSlot.fullarmor, ArmorgrpTable.BodyPartFromInterlude(15));
        Assert.AreEqual(ItemSlot.alldress, ArmorgrpTable.BodyPartFromInterlude(16));
        Assert.AreEqual(ItemSlot.hat1, ArmorgrpTable.BodyPartFromInterlude(17));
        Assert.AreEqual(ItemSlot.hat2, ArmorgrpTable.BodyPartFromInterlude(18));
        Assert.AreEqual(ItemSlot.hat1, ArmorgrpTable.BodyPartFromInterlude(19));
        Assert.AreEqual(ItemSlot.none, ArmorgrpTable.BodyPartFromInterlude(13));
        Assert.AreEqual(ItemSlot.none, ArmorgrpTable.BodyPartFromInterlude(99));
    }
}
