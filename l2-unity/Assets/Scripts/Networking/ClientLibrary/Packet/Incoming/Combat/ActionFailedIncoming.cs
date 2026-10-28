
[IncomingGamePacket(GameServerPacketType.ActionFailed)]
public sealed class ActionFailedIncoming : IncomingWirePacket<ActionFailedDto>
{
    public override void Apply(ActionFailedDto packet) =>
        CombatFeedbackIncomingPacketServices.ApplyActionFailed(packet);
}
