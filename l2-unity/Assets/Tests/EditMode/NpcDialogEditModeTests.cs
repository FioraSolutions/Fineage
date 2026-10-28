using NUnit.Framework;

public class NpcDialogEditModeTests
{
    [Test]
    public void PacketOpcodes_MatchBackendNpcDialog()
    {
        Assert.AreEqual(44, (int)ZoneV2MsgType.NpcTalk);
        Assert.AreEqual(95, (int)ZoneV2MsgType.BypassRequest);
        Assert.AreEqual(1094, (int)ZoneV2MsgType.NpcHtml);
        Assert.AreEqual(1061, (int)ZoneV2MsgType.QuestList);
        Assert.AreEqual(96, (int)ZoneV2MsgType.RequestBuyItem);
        Assert.AreEqual(1095, (int)ZoneV2MsgType.BuyList);
        Assert.AreEqual(105, (int)ZoneV2MsgType.TeleportSelectRequest);
        Assert.AreEqual(1105, (int)ZoneV2MsgType.TeleportList);
        Assert.AreEqual(150, ZoneV2NpcSession.InteractionDistance);
    }

    [Test]
    public void NpcTalk_AndBuyRequest_RoundTrip()
    {
        byte[] talk = ZoneV2Messages.EncodeNpcTalk(50010);
        Assert.IsTrue(talk.Length > 0);
        byte[] buy = ZoneV2Messages.EncodeRequestBuyItem(7, 3000100, new[]
        {
            new ZoneV2IdCount { Id = 57, Count = 1 }
        });
        Assert.IsTrue(buy.Length > talk.Length);
    }

    [Test]
    public void HtmlSubset_ParsesTalkingIslandMerchantLinks()
    {
        const string page = "<html><body>Trader Lector:<br>"
            + "<font color=\"LEVEL\">Advice</font><br>"
            + "<a action=\"bypass -h npc_11_Buy 3000100\">Buy Fighter equipment.</a><br>"
            + "<a action=\"bypass -h npc_11_Sell\">Sell.</a><br>"
            + "<button value=\"Quest\" action=\"bypass Script\" width=70 height=20></button>"
            + "<img src=\"icon.etc_question_mark_i00\" width=32 height=32>"
            + "<table><tr><td>Tax</td></tr></table>"
            + "</body></html>";
        NpcHtmlSubset doc = NpcHtmlSubset.Parse(page);
        Assert.AreEqual("LEVEL", doc.Find("font")[0].Color);
        Assert.IsTrue(doc.Find("a")[0].Action.Contains("Buy 3000100"));
        Assert.AreEqual("Quest", doc.Find("button")[0].Value);
        Assert.AreEqual("icon.etc_question_mark_i00", doc.Find("img")[0].Src);
        Assert.IsTrue(doc.Find("td")[0].Text.Contains("Tax"));
        Assert.AreEqual("Chat 1", ZoneV2Bypass.Normalize("bypass -h npc_61000_Chat 1"));
    }

    [Test]
    public void EmptySystemText_UsesShopCatalog()
    {
        Assert.AreEqual("You do not have enough adena.", ZoneV2SystemLines.Resolve(180, "", null));
        Assert.AreEqual("Wear is not available.", ZoneV2SystemLines.Resolve(189, "", null));
        Assert.AreEqual("Your warehouse is full.", ZoneV2SystemLines.Resolve(194, "", null));
    }
}
