using Fineage.Net.Contract;

public sealed partial class NetSession
{
    internal void ApplyWeddingEngageAsk(WeddingEngageAskNetMsg ask) =>
        WeddingEngageAskApplyIncomingPacketServices.Apply(ask);
}