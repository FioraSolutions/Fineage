using UnityEngine;

[IncomingGamePacket(GameServerPacketType.ShortCutDel)]
public sealed class ShortCutDelIncoming : IncomingWirePacket<ShortCutDelDto>
{
    public override void Apply(ShortCutDelDto packet)
    {
        ShortcutIncomingPacketServices.ApplyDel(packet);
    }
}
