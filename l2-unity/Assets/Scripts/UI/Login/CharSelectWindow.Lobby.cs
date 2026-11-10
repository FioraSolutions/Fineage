using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public partial class CharSelectWindow
{
    public void SetLobbyCharacters(IReadOnlyList<LobbyCharacter> characters)
    {
        _lobbyCharacters = characters == null
            ? new List<LobbyCharacter>()
            : new List<LobbyCharacter>(characters);
        _deleteArmed = false;
        _deleteArmedSlot = -1;
        _lobbySelectIndex = -1;
        if (_charNameManipulator == null)
            return;
        ApplyLobbyNames();
    }

    public void SelectLobbySlot(int index)
    {
        _lobbySelectIndex = index;
        if (_charNameManipulator == null || _lobbyCharacters == null)
            return;
        if (index < 0 || index >= _lobbyCharacters.Count)
        {
            SetEmptyData();
            ShowLobbyStatus(_lobbyCharacters.Count == 0 ? LobbyCharacterText.EmptyRosterStatus : "");
            RefreshLobbyActions();
            return;
        }

        LobbyCharacter character = _lobbyCharacters[index];
        if (_deleteArmed && _deleteArmedSlot != character.Slot)
            _deleteArmed = false;
        _levelLabel.text = character.Level.ToString();
        _classLabel.text = LobbyCharacterText.ClassLabel(character.ClassId);
        int maxHp = character.MaxHp > 0 ? character.MaxHp : 0;
        int maxMp = character.MaxMp > 0 ? character.MaxMp : 0;
        int curHp = maxHp > 0 ? Mathf.Clamp(character.CurHp, 0, maxHp) : character.CurHp;
        int curMp = maxMp > 0 ? Mathf.Clamp(character.CurMp, 0, maxMp) : character.CurMp;
        _hpLabel.text = maxHp > 0 ? $"{curHp}/{maxHp}" : "0/0";
        _mpLabel.text = maxMp > 0 ? $"{curMp}/{maxMp}" : "0/0";
        _expLabel.text = LobbyExpPercent(character).ToString("0.00") + "%";
        _spLabel.text = character.Sp.ToString();
        _karmaLabel.text = character.Karma.ToString();
        StartCoroutine(UpdateLobbyBars(curHp, maxHp, curMp, maxMp, LobbyExpPercent(character) / 100f));
        if (index != _charNameManipulator.Index)
            _charNameManipulator.SelectIndex(index);

        if (character.PendingDelete)
            ShowLobbyStatus("Pending deletion. " + LobbyCharacterText.DeleteCountdown(character.DeleteSecondsLeft));
        else if (_deleteArmed && _deleteArmedSlot == character.Slot)
            ShowLobbyStatus("Delete " + character.Name + "? Press Delete again to confirm.");
        else
            ShowLobbyStatus("");

        RefreshLobbyActions();
    }

    static float LobbyExpPercent(LobbyCharacter c)
    {
        long cur = LevelServer.GetExp(c.Level);
        long next = LevelServer.GetExp(c.Level + 1);
        if (cur < 0 || next <= cur) return 0f;
        return Mathf.Clamp01((float)((c.Exp - cur) / (double)(next - cur))) * 100f;
    }

    public void ShowLobbyStatus(string text)
    {
        if (_lobbyStatus != null)
            _lobbyStatus.text = text ?? "";
    }

    public void RefreshLobbyActions()
    {
        if (_lobbyCharacters == null)
            return;
        LobbyCharacter selected = SelectedLobbyCharacter();
        bool pending = selected != null && selected.PendingDelete;
        if (_startButton != null)
            _startButton.SetEnabled(LobbyCharacterText.CanEnter(selected));
        if (_deleteButton != null)
            _deleteButton.SetEnabled(selected != null);
        if (_createButton != null)
            _createButton.SetEnabled(LobbyCharacterText.CanCreate(_lobbyCharacters.Count));
        if (_deleteButtonLabel != null)
        {
            if (pending)
                _deleteButtonLabel.text = "Restore";
            else if (_deleteArmed && selected != null && _deleteArmedSlot == selected.Slot)
                _deleteButtonLabel.text = "Confirm";
            else
                _deleteButtonLabel.text = "Delete";
        }
    }

    void ApplyLobbyNames()
    {
        string[] names = new string[_lobbyCharacters.Count];
        for (int i = 0; i < _lobbyCharacters.Count; i++)
            names[i] = _lobbyCharacters[i] != null ? _lobbyCharacters[i].Name : "";
        _charNameManipulator.UpdateValues(names);
        EnableInterludeButtons();
        if (_lobbyCharacters.Count == 0)
        {
            SetEmptyData();
            ShowLobbyStatus(LobbyCharacterText.EmptyRosterStatus);
        }

        RefreshLobbyActions();
    }

    LobbyCharacter SelectedLobbyCharacter()
    {
        if (_lobbyCharacters == null || _lobbySelectIndex < 0 || _lobbySelectIndex >= _lobbyCharacters.Count)
            return null;
        return _lobbyCharacters[_lobbySelectIndex];
    }

    void EnableInterludeButtons()
    {
        if (_lobbyCharacters != null)
            return;
        if (_startButton != null)
            _startButton.SetEnabled(true);
        if (_deleteButton != null)
            _deleteButton.SetEnabled(true);
        if (_createButton != null)
            _createButton.SetEnabled(true);
        if (_deleteButtonLabel != null)
            _deleteButtonLabel.text = "Delete";
    }
}
