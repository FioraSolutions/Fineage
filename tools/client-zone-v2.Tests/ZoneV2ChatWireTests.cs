using MessagePack;
using Xunit;

namespace Fineage.Client.ZoneV2.Tests;

/// <summary>
/// Zone chat wire. Layout matches Fineage-Backend <c>ChatProtocol</c> / <c>SayMsg</c> at merge b967c27.
/// Unity EditMode cannot reference Assembly-CSharp; this project is the client wire test setup.
/// </summary>
public class ZoneV2ChatWireTests
{
    [Fact]
    public void Opcodes_match_the_backend_chat_contract()
    {
        Assert.Equal(8, (ushort)ZoneV2MsgType.Say);
        Assert.Equal(1012, (ushort)ZoneV2MsgType.CreatureSay);
        Assert.Equal(1040, (ushort)ZoneV2MsgType.SystemMessage);
        Assert.Equal(1099, (ushort)ZoneV2MsgType.Error);
        Assert.False(Enum.IsDefined(typeof(ZoneV2MsgType), (ushort)1019));
        for (ushort id = 1041; id <= 1049; id++)
            Assert.False(Enum.IsDefined(typeof(ZoneV2MsgType), id));
    }

    [Fact]
    public void Say_body_is_text_then_chat_type()
    {
        byte[] body = ZoneV2Messages.EncodeSay("hello", ZoneV2Chat.All);
        int i = 0;
        Assert.Equal(2, ZoneV2MsgPack.ReadArrayHeader(body, ref i));

        var msg = MessagePackSerializer.Deserialize<SayWireMsg>(body);
        Assert.Equal("hello", msg.Text);
        Assert.Equal(0, msg.ChatType);

        byte[] shout = ZoneV2Messages.EncodeSay("hear me", ZoneV2Chat.Shout);
        var shouted = MessagePackSerializer.Deserialize<SayWireMsg>(shout);
        Assert.Equal("hear me", shouted.Text);
        Assert.Equal(1, shouted.ChatType);
    }

    [Fact]
    public void Say_does_not_truncate_an_over_long_line()
    {
        string tooLong = new string('a', ZoneV2Chat.MaxTextLength + 1);
        var msg = MessagePackSerializer.Deserialize<SayWireMsg>(ZoneV2Messages.EncodeSay(tooLong, 0));
        Assert.Equal(101, msg.Text.Length);
        Assert.Equal(100, ZoneV2Chat.MaxTextLength);
        Assert.True(ZoneV2Chat.InputLimit > ZoneV2Chat.MaxTextLength);
    }

    [Fact]
    public void Creature_say_decodes_name_text_and_type()
    {
        var msg = new CreatureSayWireMsg
        {
            ObjectId = 42,
            Name = "Aria",
            Text = "hello",
            ChatType = ZoneV2Chat.Shout
        };

        Assert.True(ZoneV2Messages.TryDecodeCreatureSay(Pack(msg), out ZoneV2CreatureSay say));
        Assert.Equal(42, say.ObjectId);
        Assert.Equal("Aria", say.Name);
        Assert.Equal("hello", say.Text);
        Assert.Equal(1, say.ChatType);
        Assert.Equal("<color=#ffb266>Aria: hello</color>", ZoneV2Chat.FormatLine(say.Name, say.Text, say.ChatType));
        Assert.Equal("<color=#dcd9dc>Aria: hello</color>", ZoneV2Chat.FormatLine("Aria", "hello", ZoneV2Chat.All));
    }

    [Fact]
    public void Creature_say_keeps_the_first_four_keys_when_a_later_key_is_present()
    {
        var msg = new CreatureSayExtraWireMsg
        {
            ObjectId = 7,
            Name = "Aria",
            Text = "yo",
            ChatType = 0,
            Extra = 99
        };

        Assert.True(ZoneV2Messages.TryDecodeCreatureSay(Pack(msg), out ZoneV2CreatureSay say));
        Assert.Equal(7, say.ObjectId);
        Assert.Equal("yo", say.Text);
        Assert.Equal(0, say.ChatType);
    }

    [Fact]
    public void Creature_say_rejects_a_short_body()
    {
        Assert.False(ZoneV2Messages.TryDecodeCreatureSay(Pack(new ShortCreatureSayWireMsg
        {
            ObjectId = 1,
            Name = "Aria",
            Text = "hi"
        }), out _));
        Assert.False(ZoneV2Messages.TryDecodeCreatureSay(Array.Empty<byte>(), out _));
    }

    [Fact]
    public void System_message_decodes_welcome_flood_and_unsupported()
    {
        var welcome = new SystemMessageWireMsg
        {
            Id = 1,
            Params = ["Aria"],
            Text = "Welcome, Aria."
        };
        Assert.True(ZoneV2Messages.TryDecodeSystemMessage(Pack(welcome), out ZoneV2SystemMessage hello));
        Assert.Equal(1, hello.Id);
        Assert.Equal(new[] { "Aria" }, hello.Params);
        Assert.Equal("Welcome, Aria.", hello.Text);
        Assert.Equal("<color=#E6D38A>Welcome, Aria.</color>", ZoneV2Chat.FormatSystem(hello.Text));

        var flood = new SystemMessageWireMsg
        {
            Id = 3,
            Params = [],
            Text = "Please do not spam the chat."
        };
        Assert.True(ZoneV2Messages.TryDecodeSystemMessage(Pack(flood), out ZoneV2SystemMessage spam));
        Assert.Equal(3, spam.Id);
        Assert.Empty(spam.Params);
        Assert.Equal("Please do not spam the chat.", spam.Text);

        var unsupported = new SystemMessageWireMsg
        {
            Id = 2,
            Params = ["4"],
            Text = "not supported yet"
        };
        Assert.True(ZoneV2Messages.TryDecodeSystemMessage(Pack(unsupported), out ZoneV2SystemMessage no));
        Assert.Equal(2, no.Id);
        Assert.Equal("4", Assert.Single(no.Params));
        Assert.Equal("not supported yet", no.Text);
    }

    [Fact]
    public void System_message_accepts_nil_params_and_an_extra_key()
    {
        var nilParams = new SystemMessageNilParamsWireMsg
        {
            Id = 0,
            Params = null,
            Text = "plain"
        };
        Assert.True(ZoneV2Messages.TryDecodeSystemMessage(Pack(nilParams), out ZoneV2SystemMessage plain));
        Assert.Equal(0, plain.Id);
        Assert.Empty(plain.Params);
        Assert.Equal("plain", plain.Text);

        var extra = new SystemMessageExtraWireMsg
        {
            Id = 1,
            Params = ["Aria"],
            Text = "Welcome, Aria.",
            Extra = 5
        };
        Assert.True(ZoneV2Messages.TryDecodeSystemMessage(Pack(extra), out ZoneV2SystemMessage welcome));
        Assert.Equal("Welcome, Aria.", welcome.Text);
    }

    [Fact]
    public void System_message_rejects_a_short_body()
    {
        Assert.False(ZoneV2Messages.TryDecodeSystemMessage(Pack(new ShortSystemMessageWireMsg
        {
            Id = 1,
            Params = ["Aria"]
        }), out _));
    }

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

    static byte[] Pack<T>(T message)
    {
        return MessagePackSerializer.Serialize(message);
    }
}

[MessagePackObject]
public sealed class SayWireMsg
{
    [Key(0)] public string Text { get; set; } = "";
    [Key(1)] public int ChatType { get; set; }
}

[MessagePackObject]
public sealed class CreatureSayWireMsg
{
    [Key(0)] public int ObjectId { get; set; }
    [Key(1)] public string Name { get; set; } = "";
    [Key(2)] public string Text { get; set; } = "";
    [Key(3)] public int ChatType { get; set; }
}

[MessagePackObject]
public sealed class CreatureSayExtraWireMsg
{
    [Key(0)] public int ObjectId { get; set; }
    [Key(1)] public string Name { get; set; } = "";
    [Key(2)] public string Text { get; set; } = "";
    [Key(3)] public int ChatType { get; set; }
    [Key(4)] public int Extra { get; set; }
}

[MessagePackObject]
public sealed class ShortCreatureSayWireMsg
{
    [Key(0)] public int ObjectId { get; set; }
    [Key(1)] public string Name { get; set; } = "";
    [Key(2)] public string Text { get; set; } = "";
}

[MessagePackObject]
public sealed class SystemMessageWireMsg
{
    [Key(0)] public int Id { get; set; }
    [Key(1)] public string[] Params { get; set; } = [];
    [Key(2)] public string Text { get; set; } = "";
}

[MessagePackObject]
public sealed class SystemMessageNilParamsWireMsg
{
    [Key(0)] public int Id { get; set; }
    [Key(1)] public string[] Params { get; set; }
    [Key(2)] public string Text { get; set; } = "";
}

[MessagePackObject]
public sealed class SystemMessageExtraWireMsg
{
    [Key(0)] public int Id { get; set; }
    [Key(1)] public string[] Params { get; set; } = [];
    [Key(2)] public string Text { get; set; } = "";
    [Key(3)] public int Extra { get; set; }
}

[MessagePackObject]
public sealed class ShortSystemMessageWireMsg
{
    [Key(0)] public int Id { get; set; }
    [Key(1)] public string[] Params { get; set; } = [];
}
