
[IncomingGamePacket(GameServerPacketType.TutorialShowHtml)]
public sealed class TutorialShowHtmlIncoming : IncomingWirePacket<TutorialShowHtmlDto>
{
    public override void Apply(TutorialShowHtmlDto packet) =>
        TutorialShowHtmlApplyIncomingPacketServices.ApplyTutorialShowHtml(packet);
}
