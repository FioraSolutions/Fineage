using System.Collections.Generic;
using NUnit.Framework;

/// <summary>NPC buy cart rules: infinite catalog, taxed price totals, RequestBuyItem wire.</summary>
public sealed class ShopNpcBuyEditModeTests
{
    const int TypeOther = (int)EnumType2.TYPE2_OTHER;
    const int TypeWeapon = (int)EnumType2.TYPE2_WEAPON;

    static Product Row(int itemId, int count, int price, int type2 = TypeOther)
    {
        return new Product(0, 0, count, type2, 0, 0, 0, price, itemId);
    }

    [Test]
    public void NpcBuy_catalog_row_stays_when_adding_to_cart()
    {
        var catalog = new List<Product> { Row(200, 1, 40, TypeWeapon) };
        Product cartLine = ShopPanelTransfer.CreateCartLine(ShopTransferMode.NpcMerchantBuy, catalog[0], 1);
        Assert.NotNull(cartLine);
        Assert.AreNotSame(catalog[0], cartLine);
        Assert.AreEqual(1, catalog[0].Count);
        Assert.AreEqual(40, cartLine.Price);

        var cart = new List<Product>();
        cart.Add(cartLine);
        Assert.AreEqual(1, catalog.Count);
        Assert.AreEqual(1, catalog[0].Count);
        Assert.AreEqual(1, cart.Count);
    }

    [Test]
    public void NpcBuy_partial_cart_remove_does_not_return_rows_to_catalog()
    {
        ShopTransferMode mode = ShopTransferMode.NpcMerchantBuy;
        Assert.False(ShopPanelTransfer.ReturnsToCatalog(mode));
        var catalog = new List<Product> { Row(200, 1, 40) };
        Product cartRow = ShopPanelTransfer.CreateCartLine(mode, catalog[0], 5);
        cartRow.SetCount(5);
        Assert.AreEqual(1, catalog[0].Count, "catalog still infinite");
    }

    [Test]
    public void PlayerBag_commit_consumes_catalog_count()
    {
        var catalog = new List<Product> { Row(200, 10, 500, TypeOther) };
        ShopPanelTransfer.ApplyCatalogQuantityTaken(ShopTransferMode.PlayerBagCommit, catalog[0], 3);
        Assert.AreEqual(7, catalog[0].Count);
    }

    [Test]
    public void Cart_total_sums_price_times_count_for_merchant_rows()
    {
        var cart = new List<Product>
        {
            Row(200, 2, 40),
            Row(500, 1, 100, TypeWeapon),
        };
        Assert.AreEqual(180, ShopCartTotals.SumUnitPriceTimesCount(cart));
    }

    [Test]
    public void Cart_total_is_zero_when_server_prices_are_zero_gm_shop()
    {
        var cart = new List<Product>();
        for (int i = 0; i < 6; i++)
            cart.Add(Row(6658 + i, 1, 0, TypeWeapon));
        Assert.AreEqual(0, ShopCartTotals.SumUnitPriceTimesCount(cart));
    }

    [Test]
    public void RequestBuyItemDto_writes_list_id_item_id_and_count()
    {
        var cart = new List<Product> { Row(200, 3, 40), Row(500, 1, 100, TypeWeapon) };
        var dto = new RequestBuyItemDto { ListId = 9917, Items = cart };
        var writer = new PacketWriter();
        dto.WriteTo(writer);
        var reader = new PacketReader(writer.ToArray(), skipOpcode: false);
        Assert.AreEqual(9917, reader.ReadI());
        Assert.AreEqual(2, reader.ReadI());
        Assert.AreEqual(200, reader.ReadI());
        Assert.AreEqual(3, reader.ReadI());
        Assert.AreEqual(500, reader.ReadI());
        Assert.AreEqual(1, reader.ReadI());
        Assert.AreEqual(0, reader.Remaining);
    }

    [Test]
    public void NetNpcOrders_item_counts_match_backend_contract()
    {
        var cart = new List<Product> { Row(6658, 2, 0), Row(57, 25_000, 0) };
        var rows = NetNpcOrders.ItemCounts(cart);
        Assert.AreEqual(2, rows.Length);
        Assert.AreEqual(6658, rows[0].Id);
        Assert.AreEqual(2, rows[0].Count);
        Assert.AreEqual(57, rows[1].Id);
        Assert.AreEqual(25_000, rows[1].Count);
    }
}