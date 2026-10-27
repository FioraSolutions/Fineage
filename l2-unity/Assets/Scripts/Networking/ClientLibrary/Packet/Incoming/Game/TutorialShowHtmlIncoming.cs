using UnityEngine;

[IncomingGamePacket(GameServerPacketType.TutorialShowHtml)]
public sealed class TutorialShowHtmlIncoming : IncomingWirePacket<TutorialShowHtmlDto>
{
    public override void Apply(TutorialShowHtmlDto packet)
    {
        UiIncomingPacketServices.Queue(() =>
        {
            UiIncomingPacketServices.Html.InjectToWindow(packet.Html);
            UiIncomingPacketServices.Html.ShowWindowToCenter();
        });
    }
}
