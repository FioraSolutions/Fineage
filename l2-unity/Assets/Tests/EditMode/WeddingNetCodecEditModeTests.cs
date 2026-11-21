using Fineage.Net.Contract;
using NUnit.Framework;

public sealed class WeddingNetCodecEditModeTests
{
    [Test]
    public void WeddingEngageAsk_roundtrip()
    {
        var msg = new WeddingEngageAskNetMsg { RequestorName = "Hero", RequestorObjectId = 42001 };
        byte[] bytes = NetCodec.EncodeWeddingEngageAsk(msg);
        WeddingEngageAskNetMsg back = NetCodec.DecodeWeddingEngageAsk(bytes);
        Assert.AreEqual("Hero", back.RequestorName);
        Assert.AreEqual(42001, back.RequestorObjectId);
    }

    [Test]
    public void WeddingEngageAnswer_roundtrip()
    {
        var msg = new WeddingEngageAnswerNetMsg { Response = 1 };
        byte[] bytes = NetCodec.EncodeWeddingEngageAnswer(msg);
        WeddingEngageAnswerNetMsg back = NetCodec.DecodeWeddingEngageAnswer(bytes);
        Assert.AreEqual(1, back.Response);
    }
}