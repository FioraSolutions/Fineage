using System;

/// <summary>
/// Character lobby / selection incoming handler access via LoginPacketServices and queue fallbacks (#142 sub-slice 51).
/// Per-packet Apply* encapsulation completed in sub-slice 75.
/// Henna apply lives on <see cref="HennaIncomingPacketServices"/> (sub-slice 99).
/// Char create ok apply lives on <see cref="CharCreateOkIncomingPacketServices"/> (sub-slice 100).
/// Char create fail apply lives on <see cref="CharCreateFailIncomingPacketServices"/> (sub-slice 101).
/// Char template apply lives on <see cref="CharTemplateIncomingPacketServices"/> (sub-slice 102).
/// Char info apply lives on <see cref="CharInfoIncomingPacketServices"/> (sub-slice 103).
/// </summary>
public static class CharacterIncomingPacketServices
{
    public static LoginClient Login => LoginPacketServices.LoginClient;

    public static GameClient Game => LoginPacketServices.Game;

    public static GameManager Manager => LoginPacketServices.Manager;

    public static CharSelectWindow CharSelect => LoginPacketServices.CharSelect;

    public static CharacterSelector Characters => LoginPacketServices.Characters;

    public static void Queue(Action action) => IncomingPacketServices.Queue(action);

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

    public static void OnCharSelectedParsed(CharSelectedDto packet)
    {
        Game.PlayerInfo = packet.PlayeInfo;
        Game.SetDataPreparationCompleted(false);
    }

    public static void ApplyCharSelected(CharSelectedDto packet)
    {
        Game.PlayerInfo = packet.PlayeInfo;
        Manager.OnCharacterSelect();
    }
}
