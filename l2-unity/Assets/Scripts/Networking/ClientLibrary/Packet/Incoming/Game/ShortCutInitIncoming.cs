using UnityEngine;

[IncomingGamePacket(GameServerPacketType.ShortCutInit)]
public sealed class ShortCutInitIncoming : IncomingWirePacket<ShortCutInitDto>
{
    public override void Apply(ShortCutInitDto packet)
    {
        // Debug.Log("GameServerPacket OnCharShortCutInit");
        IncomingPacketServices.Queue(() => IncomingPacketServices.Shortcuts.SetShortcutList(packet.ShortCuts));
    }
}
