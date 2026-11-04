using Xunit;

namespace Fineage.Client.Domain.Tests;

/// <summary>
/// Zone chat display helpers. Salvaged from the retired <c>client-zone-v2.Tests</c>
/// MessagePack codec host; only the assertions that touch surviving domain code remain.
/// </summary>
public class ZoneV2ChatTests
{
    [Fact]
    public void Prefix_selects_the_chat_type_and_all_is_the_default()
    {
        ZoneV2Chat.Split("hello", ZoneV2Chat.All, out string text, out int type);
        Assert.Equal("hello", text);
        Assert.Equal(0, type);

        ZoneV2Chat.Split("hello", ZoneV2Chat.Party, out text, out type);
        Assert.Equal("hello", text);
        Assert.Equal(3, type);

        ZoneV2Chat.Split("!hear me", ZoneV2Chat.All, out text, out type);
        Assert.Equal("hear me", text);
        Assert.Equal(ZoneV2Chat.Shout, type);

        ZoneV2Chat.Split("\"Aria hi", 0, out text, out type);
        Assert.Equal("Aria hi", text);
        Assert.Equal(ZoneV2Chat.Tell, type);

        ZoneV2Chat.Split("#group", 0, out text, out type);
        Assert.Equal("group", text);
        Assert.Equal(ZoneV2Chat.Party, type);

        ZoneV2Chat.Split("@blood", 0, out text, out type);
        Assert.Equal("blood", text);
        Assert.Equal(ZoneV2Chat.Clan, type);

        ZoneV2Chat.Split("+wts", 0, out text, out type);
        Assert.Equal("wts", text);
        Assert.Equal(ZoneV2Chat.Trade, type);

        ZoneV2Chat.Split("$oath", 0, out text, out type);
        Assert.Equal("oath", text);
        Assert.Equal(ZoneV2Chat.Alliance, type);

        ZoneV2Chat.Split("!", 0, out text, out type);
        Assert.Equal("", text);
        Assert.Equal(ZoneV2Chat.Shout, type);

        Assert.True(ZoneV2Chat.ShowsOnAllTab(ZoneV2Chat.Shout));
        Assert.False(ZoneV2Chat.ShowsOnAllTab(ZoneV2Chat.Party));
    }

    [Fact]
    public void Chat_errors_use_the_server_message_as_the_system_line()
    {
        Assert.Equal("Say text was rejected.", ZoneV2EquipNotices.Line("bad_request", "Say text was rejected."));
        Assert.Equal("EnterWorld first.", ZoneV2EquipNotices.Line("world", "EnterWorld first."));
        Assert.Equal("Say body was not readable.", ZoneV2EquipNotices.Line("bad_request", "Say body was not readable."));
    }
}
