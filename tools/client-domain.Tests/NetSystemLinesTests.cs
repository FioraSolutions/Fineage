using Xunit;

namespace Fineage.Client.Domain.Tests;

/// <summary>
/// System-message fallback lines. Salvaged from the retired <c>client-zone-v2.Tests</c>
/// MessagePack codec host; only the assertions that touch surviving domain code remain.
/// </summary>
public class NetSystemLinesTests
{
    [Fact]
    public void System_lines_keep_server_text_and_fill_empty_shop_ids()
    {
        Assert.Equal("The transaction is complete.", NetSystemLines.Resolve(183, "The transaction is complete.", null));
        Assert.Equal("You do not have enough adena.", NetSystemLines.Resolve(180, "", null));
        Assert.Equal("Wear is not available.", NetSystemLines.Resolve(189, null, null));
        Assert.Equal("You have been teleported.", NetSystemLines.Resolve(193, "", null));
        Assert.Equal("2 unit(s) of the item Sword is/are required.",
            NetSystemLines.Resolve(186, "", new[] { "2", "Sword" }));
        Assert.Equal("You are too low level for supplemental magic.", NetSystemLines.BuffRefusal("low"));
    }

    /// <summary>
    /// Port of the retired <c>NpcDialogEditModeTests.EmptySystemText_UsesShopCatalog</c>:
    /// the message ids asserted there stay covered now that the EditMode codec test is gone.
    /// </summary>
    [Fact]
    public void Shop_catalog_lines_fill_an_empty_system_message()
    {
        Assert.Equal("You do not have enough adena.", NetSystemLines.Resolve(180, "", null));
        Assert.Equal("Wear is not available.", NetSystemLines.Resolve(189, "", null));
        Assert.Equal("Your warehouse is full.", NetSystemLines.Resolve(194, "", null));
    }
}
