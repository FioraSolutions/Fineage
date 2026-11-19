using NUnit.Framework;

/// <summary>
/// Interlude EtcItemgrp columns must feed ConsumeType / etcitem_type so shop stackables
/// (Mana Potion 728, which is missing from the Classic table) open the quantity dialog
/// and UseItem does not NRE on a null EtcItemType.
/// </summary>
public sealed class InterludeEtcItemConsumeEditModeTests
{
    [Test]
    public void FromInterludeStackable_MapsPotionAndAdenaColumns()
    {
        Assert.AreEqual(ConsumeCategory.Normal, ConsumeType.FromInterludeStackable(0));
        Assert.AreEqual(ConsumeCategory.Stackable, ConsumeType.FromInterludeStackable(2));
        Assert.AreEqual(ConsumeCategory.Asset, ConsumeType.FromInterludeStackable(3));
        Assert.AreEqual(ConsumeCategory.Normal, ConsumeType.FromInterludeStackable(99));
    }

    [Test]
    public void EtcItemTypeFromInterludeFamily_MapsPotionArrowAndNeverReturnsNull()
    {
        Assert.AreEqual("potion", ConsumeType.EtcItemTypeFromInterludeFamily(3));
        Assert.AreEqual("arrow", ConsumeType.EtcItemTypeFromInterludeFamily(2));
        Assert.AreEqual("none", ConsumeType.EtcItemTypeFromInterludeFamily(0));
        Assert.AreEqual("none", ConsumeType.EtcItemTypeFromInterludeFamily(-1));
        Assert.IsNotNull(ConsumeType.EtcItemTypeFromInterludeFamily(0));
    }

    [Test]
    public void IsScrollType_NullOrPotion_IsNotAnEnchantScroll()
    {
        Assert.IsFalse(NetEnchant.IsScrollType(null));
        Assert.IsFalse(NetEnchant.IsScrollType(""));
        Assert.IsFalse(NetEnchant.IsScrollType("potion"));
        Assert.IsFalse(NetEnchant.IsScrollType("none"));
        Assert.IsTrue(NetEnchant.IsScrollType("scrl_enchant_wp"));
    }
}
