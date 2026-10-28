using System;

/// <summary>
/// Char selection info wire packet handler entry (#142 batch slice 147).
/// Re-homes char-selection-info apply from <see cref="CharSelectionInfoIncomingPacketServices"/>; splits from <see cref="CharacterIncomingPacketServices"/> (sub-slice 75 encapsulation).
/// </summary>
public static class CharSelectionInfoApplyIncomingPacketServices
{
    public static LoginClient Login => LobbyApplyIncomingPacketServices.Login;

    public static GameClient Game => LobbyApplyIncomingPacketServices.Game;

    public static GameManager Manager => LobbyApplyIncomingPacketServices.Manager;

    public static CharSelectWindow CharSelect => LobbyApplyIncomingPacketServices.CharSelect;

    public static CharacterSelector Characters => LobbyApplyIncomingPacketServices.Characters;

    public static void Queue(Action action) => CharacterApplyIncomingPacketServices.Queue(action);

    public static void ApplyCharSelectionInfo(CharSelectionInfoDto packet)
    {
        int count = packet != null && packet.Characters != null ? packet.Characters.Count : -1;
        LobbyFlowLog.Info("CharSelectionInfo Apply chars=" + count + " selectedSlot=" + (packet != null ? packet.SelectedSlotId : -1));
        MapClassId.Init();
        Queue(() =>
        {
            try
            {
                CharSelect.SetInterludeCharacterList(packet.Characters);
                CharSelect.SelectInterludeSlot(packet.SelectedSlotId);

                Characters.SetCharacterInterludeList(packet.Characters);
                Characters.SelectInterludeCharacter(packet.SelectedSlotId);

                Login.Disconnect();
                LobbyFlowLog.Info("CharSelectionInfo → Game.OnAuthAllowed (show char select)");
                Game.OnAuthAllowed();
                LobbyFlowLog.Info("CharSelectionInfo DONE state=" +
                    (Manager != null ? Manager.GameState.ToString() : "null"));
            }
            catch (Exception ex)
            {
                LobbyFlowLog.Exception("CharSelectionInfo Apply (this packet breaks lobby)", ex);
            }
        });
    }
}
