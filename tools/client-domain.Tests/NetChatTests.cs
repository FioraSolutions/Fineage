using Xunit;

namespace Fineage.Client.Domain.Tests;

/// <summary>
/// Zone chat display helpers. Salvaged from the retired <c>client-zone-v2.Tests</c>
/// MessagePack codec host; only the assertions that touch surviving domain code remain.
/// </summary>
public class NetChatTests
{
    [Fact]
    public void Prefix_selects_the_chat_type_and_all_is_the_default()
    {
        NetChat.Split("hello", NetChat.All, out string text, out int type);
        Assert.Equal("hello", text);
        Assert.Equal(0, type);

        NetChat.Split("hello", NetChat.Party, out text, out type);
        Assert.Equal("hello", text);
        Assert.Equal(3, type);

        NetChat.Split("!hear me", NetChat.All, out text, out type);
        Assert.Equal("hear me", text);
        Assert.Equal(NetChat.Shout, type);

        NetChat.Split("\"Aria hi", 0, out text, out type);
        Assert.Equal("Aria hi", text);
        Assert.Equal(NetChat.Tell, type);

        NetChat.Split("#group", 0, out text, out type);
        Assert.Equal("group", text);
        Assert.Equal(NetChat.Party, type);

        NetChat.Split("@blood", 0, out text, out type);
        Assert.Equal("blood", text);
        Assert.Equal(NetChat.Clan, type);

        NetChat.Split("+wts", 0, out text, out type);
        Assert.Equal("wts", text);
        Assert.Equal(NetChat.Trade, type);

        NetChat.Split("$oath", 0, out text, out type);
        Assert.Equal("oath", text);
        Assert.Equal(NetChat.Alliance, type);

        NetChat.Split("!", 0, out text, out type);
        Assert.Equal("", text);
        Assert.Equal(NetChat.Shout, type);

        Assert.True(NetChat.ShowsOnAllTab(NetChat.Shout));
        Assert.False(NetChat.ShowsOnAllTab(NetChat.Party));
    }

    [Fact]
    public void Chat_errors_use_the_server_message_as_the_system_line()
    {
        Assert.Equal("Say text was rejected.", NetEquipNotices.Line("bad_request", "Say text was rejected."));
        Assert.Equal("EnterWorld first.", NetEquipNotices.Line("world", "EnterWorld first."));
        Assert.Equal("Say body was not readable.", NetEquipNotices.Line("bad_request", "Say body was not readable."));
    }
}
