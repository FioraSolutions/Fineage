using System;
using UnityEngine;

[IncomingGamePacket(GameServerPacketType.CharSelectionInfo)]
public sealed class CharSelectionInfoIncoming : IncomingWirePacket<CharSelectionInfoDto>
{
    public override void Apply(CharSelectionInfoDto packet)
    {
        int count = packet != null && packet.Characters != null ? packet.Characters.Count : -1;
        LobbyFlowLog.Info("CharSelectionInfo Apply chars=" + count + " selectedSlot=" + (packet != null ? packet.SelectedSlotId : -1));
        MapClassId.Init();
        CharacterIncomingPacketServices.Queue(() =>
        {
            try
            {
                CharacterIncomingPacketServices.CharSelect.SetInterludeCharacterList(packet.Characters);
                CharacterIncomingPacketServices.CharSelect.SelectInterludeSlot(packet.SelectedSlotId);

                CharacterIncomingPacketServices.Characters.SetCharacterInterludeList(packet.Characters);
                CharacterIncomingPacketServices.Characters.SelectInterludeCharacter(packet.SelectedSlotId);

                CharacterIncomingPacketServices.Login.Disconnect();
                LobbyFlowLog.Info("CharSelectionInfo → Game.OnAuthAllowed (show char select)");
                CharacterIncomingPacketServices.Game.OnAuthAllowed();
                LobbyFlowLog.Info("CharSelectionInfo DONE state=" +
                    (CharacterIncomingPacketServices.Manager != null ? CharacterIncomingPacketServices.Manager.GameState.ToString() : "null"));
            }
            catch (Exception ex)
            {
                LobbyFlowLog.Exception("CharSelectionInfo Apply (this packet breaks lobby)", ex);
            }
        });
    }
}
