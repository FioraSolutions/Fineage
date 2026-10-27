using UnityEngine;

[IncomingGamePacket(GameServerPacketType.CharSelected)]
public sealed class CharSelectedIncoming : IncomingWirePacket<CharSelectedDto>
{
    protected override void OnParsed(CharSelectedDto packet)
    {
        CharacterIncomingPacketServices.Game.PlayerInfo = packet.PlayeInfo;
        CharacterIncomingPacketServices.Game.SetDataPreparationCompleted(false);
    }

    public override void Apply(CharSelectedDto packet)
    {
        CharacterIncomingPacketServices.Game.PlayerInfo = packet.PlayeInfo;
        CharacterIncomingPacketServices.Manager.OnCharacterSelect();
    }
}
