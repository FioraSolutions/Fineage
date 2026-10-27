using UnityEngine;

[IncomingGamePacket(GameServerPacketType.TutorialShowHtml)]
public sealed class TutorialShowHtmlIncoming : IncomingWirePacket<TutorialShowHtmlDto>
{
    public override void Apply(TutorialShowHtmlDto packet)
    {
        IncomingPacketServices.Queue(() =>
        {
            IncomingPacketServices.Html.InjectToWindow(packet.Html);
            IncomingPacketServices.Html.ShowWindowToCenter();
        });
    }
}
