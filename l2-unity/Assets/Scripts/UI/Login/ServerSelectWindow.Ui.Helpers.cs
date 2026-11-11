using System;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class ServerSelectWindow : L2Window
{
    public void UpdateServerList(int lastServer, List<ServerData> serverData, Dictionary<int, int> charsOnServers) {
        ResetWindow();

        _serverData = serverData;

        for (int i = 0; i < serverData.Count; i++) {
            charsOnServers.TryGetValue(serverData[i].serverId, out int charCount);

            AddServerRow(i, ParseServerName(serverData[i].serverId), ParseServerPing(serverData[i].status), ParseServerStatus(serverData[i].status), charCount);

            if (serverData[i].serverId == lastServer) {
                SelectServer(i);
            }
        }

        for(int i = serverData.Count; i < 20; i++) {
            //AddServerRow(i, "", "", "" ,  -1);
        }

        if(lastServer == 0 && _serverData != null && _serverData.Count > 0) {
            SelectServer(0);
        }

        if (FineageDev.AutoAdvanceLobby && _serverData != null && _serverData.Count > 0)
            StartCoroutine(AutoConfirm());
    }

    private IEnumerator AutoConfirm() {
        yield return null;
        yield return null;
        if (!_isWindowHidden)
            ConfirmButtonPressed();
    }

    public void SelectServer(int rowId) {
        for (int i = 0; i < _serverElements.Count; i++) {
            _serverElements[i].RemoveFromClassList("selected");
        }

        if(_serverElements.Count - 1 < rowId) {
            return;
        }

        _serverElements[rowId].AddToClassList("selected");

        if(_serverData == null || _serverData.Count == 0 
            || _serverData.Count - 1 < rowId || _serverData[rowId].ip == null) {
            return;
        }

        _selectServerData = _serverData[rowId];
        SetServerId(_serverData[rowId].serverId);

        //Debug.Log("Server selected: " + _selectedServerId);

        // Net has no server-list step: FineageWireConfig.ZoneHost/ZonePort already point at the
        // zone we hand off to, so the Interlude ServerIp/ServerPort stash is gone.
    }
}
