using System;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class ServerSelectWindow : L2Window
{
    [Inject] LoginRuntime _login;
    [Inject] IZoneTransport _zoneTransport;

    private IZoneTransport Transport =>
        _zoneTransport ?? (App.HasContainer ? App.Resolve<IZoneTransport>() : null);

    private LoginWindow LoginWin => LoginPacketServices.LoginWindow;

    private VisualTreeAsset _serverElementTemplate;
    private VisualElement _listContentContainer;
    private List<VisualElement> _serverElements;
    private List<ServerData> _serverData;
    private int _selectedServerId = -1;
    private ServerData _selectServerData;

    private static ServerSelectWindow _instance;
    [System.Obsolete("Use [Inject] or LoginPacketServices.ServerSelect instead")]
    public static ServerSelectWindow Instance { get { return _instance; } }

    private void Awake() {
        if (_instance == null) {
            _instance = this;
            WireLoginPacketServices();
        } else {
            Destroy(this);
        }
    }

    private void Start()
    {
        WireLoginPacketServices();
    }

    private void WireLoginPacketServices()
    {
        LoginPacketServices.Bind(login: _login);
    }

    private void OnDestroy() {
        _instance = null;
    }

    private void Update() {
        if (!_isWindowHidden) {
            if (PollInput.EscapeDown) {
                AudioPacketServices.Audio.PlayUISound("click_01");
                CancelButtonPressed();
            } else if (PollInput.EnterDown) {
                AudioPacketServices.Audio.PlayUISound("click_01");
                ConfirmButtonPressed();
            }
        }
    }

    protected override void LoadAssets() {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Login/ServerList/ServerListWindow");
        _serverElementTemplate = LoadAsset("Data/UI/_Elements/Login/ServerList/ServerElement");
    }

    protected override IEnumerator BuildWindow(VisualElement root) {
        InitWindow(root);

        yield return new WaitForEndOfFrame();

        if (_windowEle == null)
        {
            Debug.LogError("[ServerSelect] BuildWindow: _windowEle null after InitWindow");
            yield break;
        }

        // Absolute ButtonBg/Label children steal picks unless Ignore — clicks never reach Button.
        foreach (var child in _windowEle.Query(className: "l2-button-bg").ToList())
            child.pickingMode = PickingMode.Ignore;
        foreach (var child in _windowEle.Query(className: "button-label").ToList())
            child.pickingMode = PickingMode.Ignore;

        Button confirmButton = _windowEle.Q<Button>("ConfirmButton");
        if (confirmButton != null)
        {
            confirmButton.AddManipulator(new ButtonClickSoundManipulator(confirmButton));
            confirmButton.RegisterCallback<ClickEvent>(evt => ConfirmButtonPressed());
        }
        else
            Debug.LogError("[ServerSelect] ConfirmButton missing in UXML");

        Button cancelButton = _windowEle.Q<Button>("CancelButton");
        if (cancelButton != null)
        {
            cancelButton.AddManipulator(new ButtonClickSoundManipulator(cancelButton));
            cancelButton.RegisterCallback<ClickEvent>(evt => CancelButtonPressed());
        }

        Button nameButton = _windowEle.Q<Button>("Name");
        if (nameButton != null)
            nameButton.AddManipulator(new ButtonClickSoundManipulator(nameButton));
        Button trafficButton = _windowEle.Q<Button>("Traffic");
        if (trafficButton != null)
            trafficButton.AddManipulator(new ButtonClickSoundManipulator(trafficButton));
        Button characterButton = _windowEle.Q<Button>("Character");
        if (characterButton != null)
            characterButton.AddManipulator(new ButtonClickSoundManipulator(characterButton));

        _listContentContainer = _windowEle.Q<VisualElement>("ServerContainer");

        _serverElements = new List<VisualElement>();

        root.Add(_windowEle);

        yield return new WaitForEndOfFrame();
    }

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