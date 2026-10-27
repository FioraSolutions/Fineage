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
        IncomingPacketServices.Queue(() =>
        {
            try
            {
                IncomingPacketServices.CharSelect.SetInterludeCharacterList(packet.Characters);
                IncomingPacketServices.CharSelect.SelectInterludeSlot(packet.SelectedSlotId);

                IncomingPacketServices.Characters.SetCharacterInterludeList(packet.Characters);
                IncomingPacketServices.Characters.SelectInterludeCharacter(packet.SelectedSlotId);

                IncomingPacketServices.Login.Disconnect();
                LobbyFlowLog.Info("CharSelectionInfo → Game.OnAuthAllowed (show char select)");
                IncomingPacketServices.Game.OnAuthAllowed();
                LobbyFlowLog.Info("CharSelectionInfo DONE state=" +
                    (IncomingPacketServices.Manager != null ? IncomingPacketServices.Manager.GameState.ToString() : "null"));
            }
            catch (Exception ex)
            {
                LobbyFlowLog.Exception("CharSelectionInfo Apply (this packet breaks lobby)", ex);
            }
        });
    }
}
