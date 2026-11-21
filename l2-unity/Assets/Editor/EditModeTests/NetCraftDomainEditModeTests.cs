using Fineage.Net.Contract;
using NUnit.Framework;

public sealed class NetCraftDomainEditModeTests
{
    [SetUp]
    public void SetUp()
    {
        NetPlayerStoreRegistry.Reset();
        NetCraftSession.Reset();
    }

    [Test]
    public void Manufacture_store_type_uses_interlude_title_color()
    {
        Assert.AreEqual(
            PrivateStoreTitleStyles.Manufacture,
            PrivateStoreTitleStyles.ColorForStoreType(PrivateStoreProtocolClient.TypeManufacture));
    }

    [Test]
    public void PlayerStoreRegistry_tracks_manufacture_store()
    {
        NetPlayerStoreRegistry.Note(new PrivateStoreInfoNetMsg
        {
            ObjectId = 77,
            StoreType = PrivateStoreProtocolClient.TypeManufacture,
            Message = "Arrows"
        });

        Assert.IsTrue(NetPlayerStoreRegistry.IsManufactureStore(77));
        Assert.IsTrue(NetPlayerStoreRegistry.TryGet(77, out int type, out string message));
        Assert.AreEqual(PrivateStoreProtocolClient.TypeManufacture, type);
        Assert.AreEqual("Arrows", message);
    }

    [Test]
    public void RecipeBookList_mapping_preserves_dwarven_tab()
    {
        var dto = NetCraftMappings.ToRecipeBookListDto(new RecipeBookListNetMsg
        {
            Dwarven = 1,
            RecipeIds = new int[0]
        });
        Assert.AreEqual(1, dto.IsDwarven);
        Assert.AreEqual(1, NetCraftSession.LastBookDwarven);
    }

    [Test]
    public void CraftSession_tracks_mp_from_player_stats()
    {
        NetCraftSession.UpdateMpFromPlayer(25, 120);
        Assert.AreEqual(25, NetCraftSession.LastCurMp);
        Assert.AreEqual(120, NetCraftSession.LastMaxMp);
    }
}