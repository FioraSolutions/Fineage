using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using VContainer;

public partial class CharSelectWindow
{
    private void ReLoginPressed() {
        if (_lobbyCharacters != null)
        {
            Manager.OnRelogin();
            return;
        }

        Manager.OnRelogin();
        GameManager.DisconnectZoneSession();
    }

    private void CreatePressed() {
        int count = _lobbyCharacters != null ? _lobbyCharacters.Count : 0;
        if (!LobbyCharacterText.CanCreate(count))
        {
            ShowLobbyStatus(LobbyCharacterText.ErrorMessage("character_limit", null));
            return;
        }

        GameManager manager = Manager;
        if (manager != null)
            manager.OnCreateUser(null);
    }

    private void DeletePressed()
    {
        if (_lobbyCharacters != null)
        {
            LobbyCharacter selected = SelectedLobbyCharacter();
            if (selected == null)
                return;
            // Net lobby: a row already flagged PendingDelete restores, otherwise the two-press
            // confirm deletes. The server replies with DeleteResult and then a fresh roster.
            if (selected.PendingDelete)
            {
                NetSession session = NetSession.TryGetBound();
                if (session == null)
                    return;
                _ = session.RestoreCharacter(selected.Id, selected.Slot);
                _deleteArmed = false;
                ShowLobbyStatus("Restoring " + selected.Name + "...");
                return;
            }

            if (!_deleteArmed || _deleteArmedSlot != selected.Slot)
            {
                _deleteArmed = true;
                _deleteArmedSlot = selected.Slot;
                ShowLobbyStatus("Delete " + selected.Name + "? Press Delete again to confirm.");
                RefreshLobbyActions();
                return;
            }

            _deleteArmed = false;
            NetSession session = NetSession.TryGetBound();
            if (session == null)
                return;
            _ = session.DeleteCharacter(selected.Id, selected.Slot);
            ShowLobbyStatus("Deleting " + selected.Name + "...");
            RefreshLobbyActions();
            return;
        }

        Characters.TryToDeleteCharacter();
    }
}
