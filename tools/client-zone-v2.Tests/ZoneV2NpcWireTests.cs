using MessagePack;
using Xunit;

namespace Fineage.Client.ZoneV2.Tests;

public class ZoneV2NpcWireTests
{
    [Fact]
    public void Npc_dialog_opcodes_match_the_backend()
    {
        Assert.Equal(42, (ushort)ZoneV2MsgType.RequestQuestList);
        Assert.Equal(43, (ushort)ZoneV2MsgType.RequestQuestAbort);
        Assert.Equal(44, (ushort)ZoneV2MsgType.NpcTalk);
        Assert.Equal(45, (ushort)ZoneV2MsgType.QuestEvent);
        Assert.Equal(95, (ushort)ZoneV2MsgType.BypassRequest);
        Assert.Equal(96, (ushort)ZoneV2MsgType.RequestBuyItem);
        Assert.Equal(97, (ushort)ZoneV2MsgType.RequestSellItem);
        Assert.Equal(98, (ushort)ZoneV2MsgType.RequestMultiSellChoose);
        Assert.Equal(105, (ushort)ZoneV2MsgType.TeleportSelectRequest);
        Assert.Equal(106, (ushort)ZoneV2MsgType.WarehouseDepositRequest);
        Assert.Equal(108, (ushort)ZoneV2MsgType.SupportMagicRequest);
        Assert.Equal(1094, (ushort)ZoneV2MsgType.NpcHtml);
        Assert.Equal(1061, (ushort)ZoneV2MsgType.QuestList);
        Assert.Equal(1095, (ushort)ZoneV2MsgType.BuyList);
        Assert.Equal(1098, (ushort)ZoneV2MsgType.ShopBalance);
        Assert.Equal(1105, (ushort)ZoneV2MsgType.TeleportList);
        Assert.Equal(1106, (ushort)ZoneV2MsgType.WarehouseDepositList);
        Assert.Equal(1108, (ushort)ZoneV2MsgType.NewbieBuffResult);
        Assert.Equal(150, ZoneV2NpcSession.InteractionDistance);
    }

    [Fact]
    public void Talk_bypass_and_shop_requests_round_trip()
    {
        var talk = MessagePackSerializer.Deserialize<NpcTalkTestMsg>(ZoneV2Messages.EncodeNpcTalk(50010));
        Assert.Equal(50010, talk.NpcObjectId);

        var bypass = MessagePackSerializer.Deserialize<BypassTestMsg>(
            ZoneV2Messages.EncodeBypassRequest(50010, "Chat 1"));
        Assert.Equal(50010, bypass.NpcObjectId);
        Assert.Equal("Chat 1", bypass.Command);

        var buy = MessagePackSerializer.Deserialize<BuyRequestTestMsg>(
            ZoneV2Messages.EncodeRequestBuyItem(7, 3000100, new[]
            {
                new ZoneV2IdCount { Id = 10, Count = 2 }
            }));
        Assert.Equal(3000100, buy.ListId);
        Assert.Equal(10, buy.Items[0].ItemId);
        Assert.Equal(2, buy.Items[0].Count);

        var sell = MessagePackSerializer.Deserialize<SellRequestTestMsg>(
            ZoneV2Messages.EncodeRequestSellItem(7, new[]
            {
                new ZoneV2IdCount { Id = 44, Count = 1 }
            }));
        Assert.Equal(44, sell.Items[0].ObjectId);

        var choose = MessagePackSerializer.Deserialize<MultiChooseTestMsg>(
            ZoneV2Messages.EncodeRequestMultiSellChoose(7, 3, 100000, 1));
        Assert.Equal(100000, choose.EntryId);
        Assert.Equal(1, choose.Amount);

        var teleport = MessagePackSerializer.Deserialize<TeleportSelectTestMsg>(
            ZoneV2Messages.EncodeTeleportSelect(8, "NORMAL", 2));
        Assert.Equal("NORMAL", teleport.ListName);
        Assert.Equal(2, teleport.LocationId);

        Assert.Equal(new byte[] { 0x90 }, ZoneV2Messages.EncodeRequestQuestList());
    }

    [Fact]
    public void Npc_html_quest_shop_and_service_replies_decode()
    {
        var html = new NpcHtmlTestMsg
        {
            NpcObjectId = 11,
            NpcId = 30001,
            Html = "<html><body>Trader Lector:<br><a action=\"bypass -h npc_11_Buy 3000100\">Buy</a></body></html>",
            ItemId = 0
        };
        Assert.True(ZoneV2Messages.TryDecodeNpcHtml(Pack(html), out ZoneV2NpcHtml page));
        Assert.Equal(30001, page.NpcId);
        Assert.Contains("Buy 3000100", page.Html);

        var journal = new QuestListTestMsg
        {
            Active = new[]
            {
                new QuestJournalTestMsg
                {
                    QuestId = 999,
                    Name = "Roxxy's Message",
                    State = 1,
                    Cond = 1,
                    ProgressFlags = 0,
                    JournalWord = 1,
                    Repeatable = false
                }
            },
            CompletedMask = Completed(999),
            CompletedQuestIds = new[] { 999 },
            MaxActive = 25
        };
        Assert.True(ZoneV2Messages.TryDecodeQuestList(Pack(journal), out ZoneV2QuestList quests));
        Assert.Equal("Roxxy's Message", quests.Active[0].Name);
        Assert.Equal(1, quests.Active[0].JournalWord);
        Assert.Equal(25, quests.MaxActive);
        Assert.Contains(999, quests.CompletedQuestIds);
        Assert.True((quests.CompletedMask[999 / 8] & (1 << (999 % 8))) != 0);

        var buy = new BuyListTestMsg
        {
            NpcObjectId = 11,
            ListId = 3000100,
            BaseTaxPercent = 15,
            CastleTaxPercent = 0,
            Items = new[]
            {
                new BuyItemTestMsg { ItemId = 1, Price = 100, Count = -1 }
            }
        };
        Assert.True(ZoneV2Messages.TryDecodeBuyList(Pack(buy), out ZoneV2BuyList list));
        Assert.Equal(-1, list.Items[0].Count);
        Assert.Equal(15, list.BaseTaxPercent);

        var balance = new ShopBalanceTestMsg
        {
            Adena = 57,
            Weight = 10,
            MaxWeight = 69000,
            Slots = 2,
            MaxSlots = 80,
            BaseTaxPercent = 15,
            CastleTaxPercent = 0
        };
        Assert.True(ZoneV2Messages.TryDecodeShopBalance(Pack(balance), out ZoneV2ShopBalance shop));
        Assert.Equal(57, shop.Adena);
        Assert.Equal(80, shop.MaxSlots);

        var teleport = new TeleportListTestMsg
        {
            NpcObjectId = 12,
            ListName = "NORMAL",
            ListType = "NORMAL",
            Locations = new[]
            {
                new TeleportLocationTestMsg
                {
                    Id = 0,
                    Name = "The Village of Gludin",
                    X = 1,
                    Y = 2,
                    Z = 3,
                    FeeItemId = 57,
                    FeeCount = 0
                }
            }
        };
        Assert.True(ZoneV2Messages.TryDecodeTeleportList(Pack(teleport), out ZoneV2TeleportList gates));
        Assert.Equal(0, gates.Locations[0].FeeCount);
        Assert.Equal("The Village of Gludin", gates.Locations[0].Name);

        var buff = new NewbieBuffTestMsg { NpcObjectId = 13, SkillIds = new[] { 4322 }, Refusal = "" };
        Assert.True(ZoneV2Messages.TryDecodeNewbieBuffResult(Pack(buff), out ZoneV2NewbieBuffResult magic));
        Assert.Equal(4322, magic.SkillIds[0]);
        Assert.Equal("", magic.Refusal);
    }

    [Fact]
    public void System_lines_keep_server_text_and_fill_empty_shop_ids()
    {
        Assert.Equal("The transaction is complete.", ZoneV2SystemLines.Resolve(183, "The transaction is complete.", null));
        Assert.Equal("You do not have enough adena.", ZoneV2SystemLines.Resolve(180, "", null));
        Assert.Equal("Wear is not available.", ZoneV2SystemLines.Resolve(189, null, null));
        Assert.Equal("You have been teleported.", ZoneV2SystemLines.Resolve(193, "", null));
        Assert.Equal("2 unit(s) of the item Sword is/are required.",
            ZoneV2SystemLines.Resolve(186, "", new[] { "2", "Sword" }));
        Assert.Equal("You are too low level for supplemental magic.", ZoneV2SystemLines.BuffRefusal("low"));
    }

    [Fact]
    public void Talking_island_html_keeps_links_colors_buttons_images_and_tables()
    {
        const string page = "<html><body><title>Trader</title>Trader Lector:<br>"
            + "<font color=\"LEVEL\">Choose</font><br1>"
            + "<a action=\"bypass -h npc_11_Buy 3000100\">Buy Fighter equipment.</a><br>"
            + "<a action=\"bypass npc_11_Sell\">Sell.</a><br>"
            + "<button value=\"Quest\" action=\"bypass -h Script\" width=75 height=21></button>"
            + "<img src=\"icon.etc_question_mark_i00\" width=32 height=32>"
            + "<table><tr><td>Adena</td><td>15%</td></tr></table>"
            + "</body></html>";

        NpcHtmlSubset doc = NpcHtmlSubset.Parse(page);
        Assert.Equal("Trader", doc.Title);
        Assert.Equal("LEVEL", doc.Find("font")[0].Color);
        Assert.Equal("Choose", doc.Find("font")[0].Text.Trim());
        Assert.Contains(doc.Find("a"), link => link.Action.Contains("Buy 3000100"));
        Assert.Contains(doc.Find("a"), link => link.Action.Contains("Sell"));
        Assert.Equal("Quest", doc.Find("button")[0].Value);
        Assert.Contains("Script", doc.Find("button")[0].Action);
        Assert.Equal("icon.etc_question_mark_i00", doc.Find("img")[0].Src);
        Assert.Contains(doc.Find("td"), cell => cell.Text.Contains("Adena"));
        Assert.True(doc.Find("br").Count + doc.Find("br1").Count >= 2);

        Assert.Equal("Buy 3000100", ZoneV2Bypass.Normalize("bypass -h npc_11_Buy 3000100"));
        string list;
        int id;
        Assert.Equal(ZoneV2Bypass.Teleport, ZoneV2Bypass.Classify("npc_12_teleport NORMAL 3", out list, out id));
        Assert.Equal("NORMAL", list);
        Assert.Equal(3, id);
        Assert.Equal(ZoneV2Bypass.Teleport, ZoneV2Bypass.Classify("goto 4", out list, out id));
        Assert.Equal("NORMAL", list);
        Assert.Equal(4, id);
        Assert.Equal(ZoneV2Bypass.SupportMagic, ZoneV2Bypass.Classify("bypass -h SupportMagic", out list, out id));
        Assert.Equal(ZoneV2Bypass.Talk, ZoneV2Bypass.Classify("DepositP", out list, out id));
    }

    [Fact]
    public void Backend_npc_info_keeps_name_and_attackable_flag()
    {
        var merchant = new TownNpcInfoTestMsg
        {
            ObjectId = 20,
            NpcId = 30001,
            Name = "Lector",
            X = 10,
            Y = 20,
            Z = 30,
            Heading = 0,
            Attackable = false,
            RunSpeed = 80f,
            WalkSpeed = 40f,
            Title = "Trader",
            NpcType = "Merchant"
        };
        Assert.True(ZoneV2Messages.TryDecodeNpcInfo(Pack(merchant), out ZoneV2NpcInfo info));
        Assert.Equal("Lector", info.Name);
        Assert.False(info.Attackable);
        Assert.Equal("Trader", info.Title);
        Assert.Equal("Merchant", info.NpcType);
        Assert.Equal(10, info.X);
    }

    static byte[] Completed(int questId)
    {
        var mask = new byte[128];
        mask[questId / 8] = (byte)(1 << (questId % 8));
        return mask;
    }

    static byte[] Pack<T>(T msg) => MessagePackSerializer.Serialize(msg);
}

[MessagePackObject]
public sealed partial class NpcTalkTestMsg
{
    [Key(0)] public int NpcObjectId { get; set; }
}

[MessagePackObject]
public sealed partial class BypassTestMsg
{
    [Key(0)] public int NpcObjectId { get; set; }
    [Key(1)] public string Command { get; set; }
}

[MessagePackObject]
public sealed partial class ShopCountTestMsg
{
    [Key(0)] public int ItemId { get; set; }
    [Key(1)] public int Count { get; set; }
}

[MessagePackObject]
public sealed partial class ShopObjectTestMsg
{
    [Key(0)] public int ObjectId { get; set; }
    [Key(1)] public int Count { get; set; }
}

[MessagePackObject]
public sealed partial class BuyRequestTestMsg
{
    [Key(0)] public int NpcObjectId { get; set; }
    [Key(1)] public int ListId { get; set; }
    [Key(2)] public ShopCountTestMsg[] Items { get; set; }
}

[MessagePackObject]
public sealed partial class SellRequestTestMsg
{
    [Key(0)] public int NpcObjectId { get; set; }
    [Key(1)] public ShopObjectTestMsg[] Items { get; set; }
}

[MessagePackObject]
public sealed partial class MultiChooseTestMsg
{
    [Key(0)] public int NpcObjectId { get; set; }
    [Key(1)] public int ListId { get; set; }
    [Key(2)] public int EntryId { get; set; }
    [Key(3)] public int Amount { get; set; }
}

[MessagePackObject]
public sealed partial class TeleportSelectTestMsg
{
    [Key(0)] public int NpcObjectId { get; set; }
    [Key(1)] public string ListName { get; set; }
    [Key(2)] public int LocationId { get; set; }
}

[MessagePackObject]
public sealed partial class NpcHtmlTestMsg
{
    [Key(0)] public int NpcObjectId { get; set; }
    [Key(1)] public int NpcId { get; set; }
    [Key(2)] public string Html { get; set; }
    [Key(3)] public int ItemId { get; set; }
}

[MessagePackObject]
public sealed partial class QuestJournalTestMsg
{
    [Key(0)] public int QuestId { get; set; }
    [Key(1)] public string Name { get; set; }
    [Key(2)] public int State { get; set; }
    [Key(3)] public int Cond { get; set; }
    [Key(4)] public int ProgressFlags { get; set; }
    [Key(5)] public int JournalWord { get; set; }
    [Key(6)] public bool Repeatable { get; set; }
}

[MessagePackObject]
public sealed partial class QuestListTestMsg
{
    [Key(0)] public QuestJournalTestMsg[] Active { get; set; }
    [Key(1)] public byte[] CompletedMask { get; set; }
    [Key(2)] public int[] CompletedQuestIds { get; set; }
    [Key(3)] public int MaxActive { get; set; }
}

[MessagePackObject]
public sealed partial class BuyItemTestMsg
{
    [Key(0)] public int ItemId { get; set; }
    [Key(1)] public int Price { get; set; }
    [Key(2)] public int Count { get; set; }
}

[MessagePackObject]
public sealed partial class BuyListTestMsg
{
    [Key(0)] public int NpcObjectId { get; set; }
    [Key(1)] public int ListId { get; set; }
    [Key(2)] public int BaseTaxPercent { get; set; }
    [Key(3)] public int CastleTaxPercent { get; set; }
    [Key(4)] public BuyItemTestMsg[] Items { get; set; }
}

[MessagePackObject]
public sealed partial class ShopBalanceTestMsg
{
    [Key(0)] public int Adena { get; set; }
    [Key(1)] public int Weight { get; set; }
    [Key(2)] public int MaxWeight { get; set; }
    [Key(3)] public int Slots { get; set; }
    [Key(4)] public int MaxSlots { get; set; }
    [Key(5)] public int BaseTaxPercent { get; set; }
    [Key(6)] public int CastleTaxPercent { get; set; }
}

[MessagePackObject]
public sealed partial class TeleportLocationTestMsg
{
    [Key(0)] public int Id { get; set; }
    [Key(1)] public string Name { get; set; }
    [Key(2)] public int X { get; set; }
    [Key(3)] public int Y { get; set; }
    [Key(4)] public int Z { get; set; }
    [Key(5)] public int FeeItemId { get; set; }
    [Key(6)] public int FeeCount { get; set; }
}

[MessagePackObject]
public sealed partial class TeleportListTestMsg
{
    [Key(0)] public int NpcObjectId { get; set; }
    [Key(1)] public string ListName { get; set; }
    [Key(2)] public string ListType { get; set; }
    [Key(3)] public TeleportLocationTestMsg[] Locations { get; set; }
}

[MessagePackObject]
public sealed partial class NewbieBuffTestMsg
{
    [Key(0)] public int NpcObjectId { get; set; }
    [Key(1)] public int[] SkillIds { get; set; }
    [Key(2)] public string Refusal { get; set; }
}

[MessagePackObject]
public sealed partial class TownNpcInfoTestMsg
{
    [Key(0)] public int ObjectId { get; set; }
    [Key(1)] public int NpcId { get; set; }
    [Key(2)] public string Name { get; set; }
    [Key(3)] public int X { get; set; }
    [Key(4)] public int Y { get; set; }
    [Key(5)] public int Z { get; set; }
    [Key(6)] public int Heading { get; set; }
    [Key(7)] public bool Attackable { get; set; }
    [Key(8)] public float RunSpeed { get; set; }
    [Key(9)] public float WalkSpeed { get; set; }
    [Key(10)] public string Title { get; set; }
    [Key(11)] public string NpcType { get; set; }
}
