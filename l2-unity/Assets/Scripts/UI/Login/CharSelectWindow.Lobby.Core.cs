using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Linq;
using VContainer;

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
        SetLabel(_levelLabel, character.Level.ToString());
        SetLabel(_classLabel, LobbyCharacterText.ClassLabel(character.ClassId));
        int maxHp = character.MaxHp > 0 ? character.MaxHp : 0;
        int maxMp = character.MaxMp > 0 ? character.MaxMp : 0;
        int curHp = maxHp > 0 ? Mathf.Clamp(character.CurHp, 0, maxHp) : character.CurHp;
        int curMp = maxMp > 0 ? Mathf.Clamp(character.CurMp, 0, maxMp) : character.CurMp;
        SetLabel(_hpLabel, maxHp > 0 ? $"{curHp}/{maxHp}" : "0/0");
        SetLabel(_mpLabel, maxMp > 0 ? $"{curMp}/{maxMp}" : "0/0");
        SetLabel(_expLabel, LobbyExpPercent(character).ToString("0.00") + "%");
        SetLabel(_spLabel, character.Sp.ToString());
        SetLabel(_karmaLabel, character.Karma.ToString());
        StartCoroutine(UpdateLobbyBars(curHp, maxHp, curMp, maxMp, LobbyExpPercent(character) / 100f));
        if (index != _charNameManipulator.Index)
            _charNameManipulator.SelectIndex(index);

        if (character.PendingDelete)
            ShowLobbyStatus("Pending deletion. " + LobbyCharacterText.DeleteCountdown(character.DeleteSecondsLeft));
        else if (_deleteArmed && _deleteArmedSlot == character.Slot)
            ShowLobbyStatus("Delete " + character.Name + " permanently? Press Delete again to confirm.");
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
}
