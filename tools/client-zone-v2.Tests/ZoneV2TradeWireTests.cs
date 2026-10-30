using MessagePack;
using Xunit;

namespace Fineage.Client.ZoneV2.Tests;

public class ZoneV2TradeWireTests
{
    [Fact]
    public void Opcodes_match_the_backend_contract()
    {
        Assert.Equal(28, (ushort)ZoneV2MsgType.TradeRequest);
        Assert.Equal(29, (ushort)ZoneV2MsgType.TradeAnswerRequest);
        Assert.Equal(54, (ushort)ZoneV2MsgType.TradeAddItemRequest);
        Assert.Equal(55, (ushort)ZoneV2MsgType.TradeDoneRequest);
        Assert.Equal(1077, (ushort)ZoneV2MsgType.TradeAsk);
        Assert.Equal(1078, (ushort)ZoneV2MsgType.TradeRequestResult);
        Assert.Equal(1079, (ushort)ZoneV2MsgType.TradeStart);
        Assert.Equal(1080, (ushort)ZoneV2MsgType.TradeOfferUpdate);
        Assert.Equal(1081, (ushort)ZoneV2MsgType.TradeConfirmState);
        Assert.Equal(1082, (ushort)ZoneV2MsgType.TradeFinished);
    }

    [Fact]
    public void Trade_request_and_answer_round_trip()
    {
        byte[] request = ZoneV2Messages.EncodeTradeRequest(2001);
        var requestMsg = MessagePackSerializer.Deserialize<TradeRequestTestMsg>(request);
        Assert.Equal(2001, requestMsg.TargetObjectId);

        byte[] answer = ZoneV2Messages.EncodeTradeAnswerRequest(1001, 1);
        var answerMsg = MessagePackSerializer.Deserialize<TradeAnswerTestMsg>(answer);
        Assert.Equal(1001, answerMsg.RequestorObjectId);
        Assert.Equal(1, answerMsg.Response);
    }

    [Fact]
    public void Trade_add_item_and_done_encode()
    {
        byte[] add = ZoneV2Messages.EncodeTradeAddItemRequest(501, 3);
        var addMsg = MessagePackSerializer.Deserialize<TradeAddItemTestMsg>(add);
        Assert.Equal(501, addMsg.InventoryObjectId);
        Assert.Equal(3, addMsg.Count);

        byte[] done = ZoneV2Messages.EncodeTradeDoneRequest(1);
        var doneMsg = MessagePackSerializer.Deserialize<TradeDoneTestMsg>(done);
        Assert.Equal(1, doneMsg.Response);
    }

    [Fact]
    public void Trade_ask_start_offer_confirm_and_finished_decode()
    {
        var ask = new TradeAskTestMsg { RequestorObjectId = 1001, RequestorName = "Hero" };
        Assert.True(ZoneV2Messages.TryDecodeTradeAsk(Pack(ask), out ZoneV2TradeAsk decodedAsk));
        Assert.Equal(1001, decodedAsk.RequestorObjectId);
        Assert.Equal("Hero", decodedAsk.RequestorName);

        var start = new TradeStartTestMsg { PartnerObjectId = 2001, PartnerName = "Ally" };
        Assert.True(ZoneV2Messages.TryDecodeTradeStart(Pack(start), out ZoneV2TradeStart decodedStart));
        Assert.Equal(2001, decodedStart.PartnerObjectId);
        Assert.Equal("Ally", decodedStart.PartnerName);

        var offer = new TradeOfferUpdateTestMsg
        {
            IsOwn = 1,
            InventoryObjectId = 501,
            ItemId = 57,
            Count = 100,
            EnchantLevel = 0
        };
        Assert.True(ZoneV2Messages.TryDecodeTradeOfferUpdate(Pack(offer), out ZoneV2TradeOfferUpdate decodedOffer));
        Assert.Equal(1, decodedOffer.IsOwn);
        Assert.Equal(57, decodedOffer.ItemId);

        var confirm = new TradeConfirmStateTestMsg { SelfConfirmed = 1, PartnerConfirmed = 0 };
        Assert.True(ZoneV2Messages.TryDecodeTradeConfirmState(Pack(confirm), out ZoneV2TradeConfirmState decodedConfirm));
        Assert.Equal(1, decodedConfirm.SelfConfirmed);

        var finished = new TradeFinishedTestMsg { Response = 1 };
        Assert.True(ZoneV2Messages.TryDecodeTradeFinished(Pack(finished), out int finishedResponse));
        Assert.Equal(1, finishedResponse);
    }

    static byte[] Pack<T>(T msg) => MessagePackSerializer.Serialize(msg);
}

[MessagePackObject]
public sealed partial class TradeRequestTestMsg
{
    [Key(0)] public int TargetObjectId { get; set; }
}

[MessagePackObject]
public sealed partial class TradeAnswerTestMsg
{
    [Key(0)] public int RequestorObjectId { get; set; }
    [Key(1)] public int Response { get; set; }
}

[MessagePackObject]
public sealed partial class TradeAddItemTestMsg
{
    [Key(0)] public int InventoryObjectId { get; set; }
    [Key(1)] public int Count { get; set; }
}

[MessagePackObject]
public sealed partial class TradeDoneTestMsg
{
    [Key(0)] public int Response { get; set; }
}

[MessagePackObject]
public sealed partial class TradeAskTestMsg
{
    [Key(0)] public int RequestorObjectId { get; set; }
    [Key(1)] public string RequestorName { get; set; }
}

[MessagePackObject]
public sealed partial class TradeStartTestMsg
{
    [Key(0)] public int PartnerObjectId { get; set; }
    [Key(1)] public string PartnerName { get; set; }
}

[MessagePackObject]
public sealed partial class TradeOfferUpdateTestMsg
{
    [Key(0)] public int IsOwn { get; set; }
    [Key(1)] public int InventoryObjectId { get; set; }
    [Key(2)] public int ItemId { get; set; }
    [Key(3)] public int Count { get; set; }
    [Key(4)] public int EnchantLevel { get; set; }
}

[MessagePackObject]
public sealed partial class TradeConfirmStateTestMsg
{
    [Key(0)] public int SelfConfirmed { get; set; }
    [Key(1)] public int PartnerConfirmed { get; set; }
}

[MessagePackObject]
public sealed partial class TradeFinishedTestMsg
{
    [Key(0)] public int Response { get; set; }
}
