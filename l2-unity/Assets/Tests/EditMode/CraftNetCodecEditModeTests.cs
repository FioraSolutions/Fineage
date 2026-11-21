using Fineage.Net.Contract;
using NUnit.Framework;

public sealed class CraftNetCodecEditModeTests
{
    [Test]
    public void RecipeBookOpen_roundtrip()
    {
        var msg = new RecipeBookOpenNetMsg { Dwarven = 1 };
        var back = NetCodec.DecodeRecipeBookOpen(NetCodec.EncodeRecipeBookOpen(msg));
        Assert.AreEqual(msg.Dwarven, back.Dwarven);
    }

    [Test]
    public void RecipeMakeSelf_roundtrip()
    {
        var msg = new RecipeMakeSelfNetMsg { RecipeId = 1 };
        var back = NetCodec.DecodeRecipeMakeSelf(NetCodec.EncodeRecipeMakeSelf(msg));
        Assert.AreEqual(msg.RecipeId, back.RecipeId);
    }

    [Test]
    public void RecipeBookList_roundtrip()
    {
        var msg = new RecipeBookListNetMsg { Dwarven = 0, RecipeIds = new[] { 1, 2, 5 } };
        var back = NetCodec.DecodeRecipeBookList(NetCodec.EncodeRecipeBookList(msg));
        Assert.AreEqual(msg.Dwarven, back.Dwarven);
        CollectionAssert.AreEqual(msg.RecipeIds, back.RecipeIds);
    }

    [Test]
    public void RecipeItemMakeInfo_roundtrip()
    {
        var msg = new RecipeItemMakeInfoNetMsg { RecipeId = 1, Result = 0, CurrentMp = 40, MaxMp = 100 };
        var back = NetCodec.DecodeRecipeItemMakeInfo(NetCodec.EncodeRecipeItemMakeInfo(msg));
        Assert.AreEqual(msg.RecipeId, back.RecipeId);
        Assert.AreEqual(msg.Result, back.Result);
        Assert.AreEqual(msg.CurrentMp, back.CurrentMp);
        Assert.AreEqual(msg.MaxMp, back.MaxMp);
    }

    [Test]
    public void SetRecipeShop_roundtrip()
    {
        var msg = new SetRecipeShopNetMsg
        {
            Message = "Craft",
            Items = new[]
            {
                new RecipeShopRowNet { RecipeId = 1, Price = 100 },
                new RecipeShopRowNet { RecipeId = 2, Price = 250 }
            }
        };
        var back = NetCodec.DecodeSetRecipeShop(NetCodec.EncodeSetRecipeShop(msg));
        Assert.AreEqual(msg.Message, back.Message);
        Assert.AreEqual(2, back.Items.Length);
        Assert.AreEqual(100, back.Items[0].Price);
    }

    [Test]
    public void RecipeShopSellList_roundtrip()
    {
        var msg = new RecipeShopSellListNetMsg
        {
            ManufacturerObjectId = 9001,
            CurrentMp = 30,
            MaxMp = 80,
            BuyerAdena = 5000,
            Items = new[] { new RecipeShopRowNet { RecipeId = 1, Price = 99 } }
        };
        var back = NetCodec.DecodeRecipeShopSellList(NetCodec.EncodeRecipeShopSellList(msg));
        Assert.AreEqual(msg.ManufacturerObjectId, back.ManufacturerObjectId);
        Assert.AreEqual(msg.BuyerAdena, back.BuyerAdena);
        Assert.AreEqual(1, back.Items.Length);
        Assert.AreEqual(99, back.Items[0].Price);
    }

    [Test]
    public void RecipeShopItemInfo_roundtrip()
    {
        var msg = new RecipeShopItemInfoNetMsg
        {
            ManufacturerObjectId = 42,
            RecipeId = 1,
            CurrentMp = 10,
            MaxMp = 50,
            Result = 0
        };
        var back = NetCodec.DecodeRecipeShopItemInfo(NetCodec.EncodeRecipeShopItemInfo(msg));
        Assert.AreEqual(msg.ManufacturerObjectId, back.ManufacturerObjectId);
        Assert.AreEqual(msg.Result, back.Result);
    }
}