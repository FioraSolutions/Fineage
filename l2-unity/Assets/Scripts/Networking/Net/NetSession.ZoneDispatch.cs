using Fineage.Net.Contract;

public sealed partial class NetSession
{
    void HandleZone(PacketEnvelope envelope) => _zoneDispatch.Handle(envelope);
}
