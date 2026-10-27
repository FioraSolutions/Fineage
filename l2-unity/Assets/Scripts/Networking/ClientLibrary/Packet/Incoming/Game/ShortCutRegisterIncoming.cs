using UnityEngine;

[IncomingGamePacket(GameServerPacketType.ShortCutRegister)]
public sealed class ShortCutRegisterIncoming : IncomingWirePacket<ShortCutRegisterDto>
{
    public override void Apply(ShortCutRegisterDto packet)
    {
        ShortcutIncomingPacketServices.ApplyRegister(packet);
    }
}
