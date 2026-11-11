using FMOD;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using VContainer;

using UnityEngine.UIElements;

public partial class ShowListWindow : L2PopupWindow
{
    public void AddList(Dictionary<string, int> players)
    {
        _dropdown.value = "";
        var list = players.Keys.ToList();
        _dropdown.choices = list;
        _players = players;
    }

    private void OnDropdownValueChanged(ChangeEvent<string> evt)
    {
        string playeName = evt.newValue;
        _select = playeName;
    }

    private void OnClick(ClickEvent evt)
    {

        if (_players.ContainsKey(_select))
        {
            int objectId = _players[_select];
            ZoneCommands.Send(new RequestPackageSendableItemListCommand(objectId));
            HideWindow();
        }
    }

    private void OnCancel(ClickEvent evt)
    {
        HideWindow();
    }

    private void OnDestroy()
    {
        _instance = null;
    }
}
