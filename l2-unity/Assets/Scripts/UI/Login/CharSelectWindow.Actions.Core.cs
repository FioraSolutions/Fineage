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
}
