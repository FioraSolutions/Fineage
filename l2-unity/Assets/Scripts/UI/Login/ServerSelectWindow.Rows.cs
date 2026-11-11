using System;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public partial class ServerSelectWindow
{
    private string ParseServerName(int serverId) {
        return ServerNameDAO.GetServer(serverId);
    }

    private string ParseServerPing(int status) {
        switch (status) {
            case 0: return "Light";
            case 1: return "Good";
            case 2: return "Heavy";
            case 3: return "Full";
            case 4: return "Down";
            case 5: return "GM Only";
            default: return "Unknown";
        }
    }

    private string ParseServerStatus(int status)
    {
        switch (status)
        {
            case 0: return "Down";
            case 1: return "Normal";
            default: return "Unknown";
        }
    }

    private string GetPingClass(string status) {
        switch (status) {
            case "Light": return "light";
            case "Good": return "light";
            case "Heavy": return "heavy";
            case "Full":
            case "Down":
            case "GM Only": return "full";
            default: return "normal";
        }
    }

    private string GetStatusClass(string status)
    {
        switch (status)
        {
            case "Down": return "heavy";
            case "Normal": return "normal";
            default: return "normal";
        }
    }

    private void AddServerRow(int id, string serverName, string ping , string status  , int charCount) {
        VisualElement row = _serverElementTemplate.Instantiate()[0];
        Label serverNameLabel = row.Q<Label>("ServerName");
        Label serverStatusLabel = row.Q<Label>("ServerStatus");
        Label charCountLabel = row.Q<Label>("CharacterCount");
        Label serverStatusJ = row.Q<Label>("ServerStatusJ");

        serverStatusLabel.AddToClassList(GetPingClass(ping));
        serverStatusJ.AddToClassList(GetStatusClass(status));

        serverNameLabel.text = serverName;
        serverStatusLabel.text = ping;

        serverStatusJ.text = status;




        if (charCount >= 0) {
            charCountLabel.text = charCount.ToString();
        } else {
            charCountLabel.text = "";
        }

        if (id % 2 == 1) {
            row.AddToClassList("odd");
        }

        if(charCount >= 0) {
            int rowId = id;
            row.RegisterCallback<ClickEvent>((evt) => {
                SelectServer(rowId);
            });
            row.AddManipulator(new SlotClickSoundManipulator(row));
        }

        _listContentContainer.Add(row);

        _serverElements.Add(row);
    }

    private void ResetWindow() {
        _serverElements.ForEach((x) => {
            x.RemoveFromHierarchy();
        });
        _serverElements.Clear();
        if(_serverData != null) {
            _serverData.Clear();
            _serverData = null;
        }

        SetServerId(-1);
    }

    private void SetServerId(int id) {
        _selectedServerId = id;
    }

    private void ConfirmButtonPressed() {

        // L2J ServerList status byte: 0 = down, non-zero = up (stub sends 1).
        bool online = _selectServerData != null && _selectServerData.status != 0 && _selectedServerId >= 0;
        LobbyFlowLog.Info("Confirm serverId=" + _selectedServerId +
                  " status=" + (_selectServerData != null ? _selectServerData.status.ToString() : "null") +
                  " online=" + online);

        if (online)
            LobbyFlowLog.Warn("ServerSelect: RequestServerLogin has no Net equivalent — Net lobby skips server select");
        else
        {
            LobbyFlowLog.Warn("Confirm rejected — server offline/invalid; disconnect zone session");
            if (LoginWin != null)
                LoginWin.ShowErrorText("Your login attempt failed due to high server load. Please try again later");
        }

        IZoneTransport transport = Transport;
        if (transport != null)
            transport.Disconnect();
    }

    private void CancelButtonPressed() {
        IZoneTransport transport = Transport;
        if (transport != null)
            transport.Disconnect();
    }
}
