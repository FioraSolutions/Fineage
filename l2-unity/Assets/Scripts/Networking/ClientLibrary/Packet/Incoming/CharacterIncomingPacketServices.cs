using System;

/// <summary>
/// Character lobby / selection incoming handler access via LoginPacketServices and queue fallbacks (#142 sub-slice 51).
/// Per-packet Apply* encapsulation completed in sub-slice 75.
/// </summary>
public static class CharacterIncomingPacketServices
{
    public static LoginClient Login => LoginPacketServices.LoginClient;

    public static GameClient Game => LoginPacketServices.Game;

    public static GameManager Manager => LoginPacketServices.Manager;

    public static CharSelectWindow CharSelect => LoginPacketServices.CharSelect;

    public static CharacterSelector Characters => LoginPacketServices.Characters;

    public static IWorldDomainSink Domain => IncomingPacketServices.Domain;

    public static void Queue(Action action) => IncomingPacketServices.Queue(action);

    public static void QueueDomain(Action<IWorldDomainSink> apply) =>
        IncomingPacketServices.QueueDomain(apply);

    public static void QueueWorld(Action<WorldPacketApply> apply) =>
        IncomingPacketServices.QueueWorld(apply);

    public static void ApplyCharInfo(CharInfoDto packet)
    {
        if (packet == null || packet.Identity == null)
            return;

        PlayerEntity local = PlayerEntity.Instance;
        if (local != null && local.Identity != null && local.Identity.Id == packet.Identity.Id)
            return;

        MoveCostLog.NoteCharInfo();
        SpawnFrameBudget.ClearCancel(packet.Identity.Id);
        StorageNpc.getInstance().AddCharInfo(packet);
        DomainKnownlistAdd add = InterludeDomainMap.Char(packet);
        if (Domain != null && add != null)
            QueueDomain(domain => domain.ApplyKnownlistAdd(add));
        else
            QueueWorld(apply => apply.UpdateUser(packet));
    }

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

    public static void ApplyCharCreateFail(CharCreateFailDto packet) =>
        Queue(() => Manager.OnCreateUserFail(packet.Text));

    public static void ApplyCharTemplate(CharTemplatesDto templates) =>
        Queue(() => Manager.OnCreateUser(templates.PlayerTemplates));
}
