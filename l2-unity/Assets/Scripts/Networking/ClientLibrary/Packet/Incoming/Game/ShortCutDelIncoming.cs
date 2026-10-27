using UnityEngine;

[IncomingGamePacket(GameServerPacketType.ShortCutDel)]
public sealed class ShortCutDelIncoming : IncomingWirePacket<ShortCutDelDto>
{
    public override void Apply(ShortCutDelDto packet)
    {
        UiIncomingPacketServices.Queue(() => UiIncomingPacketServices.Shortcuts.RemoveShotcutLocally(packet.Slot));
    }
}
