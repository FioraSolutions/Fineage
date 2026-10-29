using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public class ServerSelectWindow : L2Window
{
    [Inject] LoginRuntime _login;

    private VisualTreeAsset _serverElementTemplate;
    private VisualElement _listContentContainer;
    private List<VisualElement> _serverElements = new List<VisualElement>();
    private IReadOnlyList<LobbyServer> _servers = System.Array.Empty<LobbyServer>();
    private int _selectedIndex = -1;
    private System.Action<LobbyServer> _onConfirm;
    private System.Action _onCancel;

    private static ServerSelectWindow _instance;
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
            if (Input.GetKeyDown(KeyCode.Escape)) {
                AudioManager.Instance.PlayUISound("click_01");
                CancelButtonPressed();
            } else if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return)) {
                AudioManager.Instance.PlayUISound("click_01");
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

        root.Add(_windowEle);

        yield return new WaitForEndOfFrame();
    }

    /// <summary>Fill the list and remember what Confirm / Cancel do. The first online server is preselected.</summary>
    public void ShowServers(IReadOnlyList<LobbyServer> servers, System.Action<LobbyServer> onConfirm, System.Action onCancel)
    {
        ResetWindow();
        _servers = servers ?? System.Array.Empty<LobbyServer>();
        _onConfirm = onConfirm;
        _onCancel = onCancel;

        for (int i = 0; i < _servers.Count; i++)
            AddServerRow(i, _servers[i]);
        for (int i = 0; i < _servers.Count; i++)
        {
            if (_servers[i].Online)
            {
                SelectServer(i);
                break;
            }
        }

        if (FineageDev.AutoAdvanceLobby && _selectedIndex >= 0)
            StartCoroutine(AutoConfirm());
    }

    private IEnumerator AutoConfirm() {
        yield return null;
        yield return null;
        if (!_isWindowHidden)
            ConfirmButtonPressed();
    }

    private void SelectServer(int index) {
        if (index < 0 || index >= _serverElements.Count)
            return;
        for (int i = 0; i < _serverElements.Count; i++)
            _serverElements[i].RemoveFromClassList("selected");
        _serverElements[index].AddToClassList("selected");
        _selectedIndex = index;
    }

    private void AddServerRow(int index, LobbyServer server) {
        VisualElement row = _serverElementTemplate.Instantiate()[0];
        Label nameLabel = row.Q<Label>("ServerName");
        Label loadLabel = row.Q<Label>("ServerStatus");
        Label statusLabel = row.Q<Label>("ServerStatusJ");
        Label charCountLabel = row.Q<Label>("CharacterCount");

        nameLabel.text = server.Name;
        loadLabel.text = server.Online ? "Light" : "Down";
        loadLabel.AddToClassList(server.Online ? "light" : "full");
        statusLabel.text = server.Online ? "Normal" : "Down";
        statusLabel.AddToClassList(server.Online ? "normal" : "heavy");
        charCountLabel.text = server.CharacterCount.ToString();

        if (index % 2 == 1)
            row.AddToClassList("odd");

        row.RegisterCallback<ClickEvent>(evt => SelectServer(index));
        row.AddManipulator(new SlotClickSoundManipulator(row));

        _listContentContainer.Add(row);
        _serverElements.Add(row);
    }

    private void ResetWindow() {
        foreach (VisualElement row in _serverElements)
            row.RemoveFromHierarchy();
        _serverElements.Clear();
        _servers = System.Array.Empty<LobbyServer>();
        _selectedIndex = -1;
    }

    private void ConfirmButtonPressed() {
        if (_selectedIndex < 0 || _selectedIndex >= _servers.Count)
            return;
        LobbyServer server = _servers[_selectedIndex];
        if (!server.Online)
            return;
        LobbyFlowLog.Info("ServerSelect confirm id=" + server.Id + " zone=" + server.Zone);
        _onConfirm?.Invoke(server);
    }

    private void CancelButtonPressed() {
        _onCancel?.Invoke();
    }
}
