using UnityEngine;

[IncomingGamePacket(GameServerPacketType.CharSelected)]
public sealed class CharSelectedIncoming : IncomingWirePacket<CharSelectedDto>
{
    protected override void OnParsed(CharSelectedDto packet)
    {
        IncomingPacketServices.Game.PlayerInfo = packet.PlayeInfo;
        IncomingPacketServices.Game.SetDataPreparationCompleted(false);
    }

    public override void Apply(CharSelectedDto packet)
    {
        IncomingPacketServices.Game.PlayerInfo = packet.PlayeInfo;
        IncomingPacketServices.Manager.OnCharacterSelect();
    }
}
