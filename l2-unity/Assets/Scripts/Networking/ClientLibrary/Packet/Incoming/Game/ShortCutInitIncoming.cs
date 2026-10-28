using UnityEngine;

[IncomingGamePacket(GameServerPacketType.ShortCutInit)]
public sealed class ShortCutInitIncoming : IncomingWirePacket<ShortCutInitDto>
{
    public override void Apply(ShortCutInitDto packet) =>
        ShortcutIncomingPacketServices.ApplyInit(packet);
}
