
[IncomingGamePacket(GameServerPacketType.SetupGauge)]
public sealed class SetupGaugeIncoming : IncomingWirePacket<SetupGaugeDto>
{
    public override void Apply(SetupGaugeDto packet) =>
        CombatFeedbackIncomingPacketServices.ApplySetupGauge(packet);
}
