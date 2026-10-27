using UnityEngine;

[IncomingGamePacket(GameServerPacketType.ShortCutInit)]
public sealed class ShortCutInitIncoming : IncomingWirePacket<ShortCutInitDto>
{
    public override void Apply(ShortCutInitDto packet)
    {
        // Debug.Log("GameServerPacket OnCharShortCutInit");
        UiIncomingPacketServices.Queue(() => UiIncomingPacketServices.Shortcuts.SetShortcutList(packet.ShortCuts));
    }
}
