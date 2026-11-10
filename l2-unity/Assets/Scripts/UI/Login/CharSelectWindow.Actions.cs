using UnityEngine;

public partial class CharSelectWindow
{
    public CharSelectInfoPackage GetSelectChar()
    {
        return _selectChar;
    }
    private void StartGamePressed()
    {
        LobbyCharacter selected = SelectedLobbyCharacter();
        if (!LobbyCharacterText.CanEnter(selected))
            return;
        if (_startButton != null)
            _startButton.SetEnabled(false);
        _ = NetSession.Ensure().SelectCharacter(selected.Id, selected.Name, selected.Slot);
    }

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
                _ = NetSession.Ensure().RestoreCharacter(selected.Id, selected.Slot);
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
            _ = NetSession.Ensure().DeleteCharacter(selected.Id, selected.Slot);
            ShowLobbyStatus("Deleting " + selected.Name + "...");
            RefreshLobbyActions();
            return;
        }

        Characters.TryToDeleteCharacter();
    }
}
