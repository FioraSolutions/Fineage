using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public class LicenseWindow : L2Window
{
    [Inject] LoginRuntime _login;

    [Inject] IZoneTransport _zoneTransport;

    private IZoneTransport Transport =>
        _zoneTransport ?? (App.HasContainer ? App.Resolve<IZoneTransport>() : null);

    private static LicenseWindow _instance;
    [System.Obsolete("Use [Inject] or LoginPacketServices.LicenseWindow instead")]
    public static LicenseWindow Instance { get { return _instance; } }

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

    public void DevAutoAgreeIfEnabled() {
        if (FineageDev.AutoAdvanceLobby)
            StartCoroutine(AutoAgreeWhenShown());
    }

    private IEnumerator AutoAgreeWhenShown() {
        yield return null;
        yield return null;
        if (!_isWindowHidden)
            AgreeButtonPressed();
    }

    private void Update() {
        if (!_isWindowHidden) {
            if (PollInput.EscapeDown) {
                AudioPacketServices.Audio.PlayUISound("click_01");
                DisagreeButtonPressed();
            } else if (PollInput.EnterDown) {
                AudioPacketServices.Audio.PlayUISound("click_01");
                AgreeButtonPressed();
            }
        }
    }

    protected override void LoadAssets() {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Login/LicenseWindow");
    }

    protected override IEnumerator BuildWindow(VisualElement root) {
        InitWindow(root);

        yield return new WaitForEndOfFrame();

        Button agreeButton = (Button)GetElementById("AgreeButton");
        agreeButton.AddManipulator(new ButtonClickSoundManipulator(agreeButton));
        agreeButton.RegisterCallback<ClickEvent>(evt => AgreeButtonPressed());

        Button disagreeButton = (Button)GetElementById("DisagreeButton");
        disagreeButton.AddManipulator(new ButtonClickSoundManipulator(disagreeButton));
        disagreeButton.RegisterCallback<ClickEvent>(evt => DisagreeButtonPressed());

        var highBtn = (RepeatButton)GetElementById("unity-high-button");
        var lowBtn = (RepeatButton)GetElementById("unity-low-button");
        highBtn.AddManipulator(new ButtonClickSoundManipulator(highBtn));
        lowBtn.AddManipulator(new ButtonClickSoundManipulator(lowBtn));
    }

    private void AgreeButtonPressed() {
        // The Interlude license handshake (Init → GGAuth → LoginOk → RequestServerList) is gone:
        // Fineage Net authenticates on the zone socket and has no license step. This window is
        // unreachable from the Net lobby, so just record the click.
        LobbyFlowLog.Warn("LicenseWindow: legacy Interlude license handshake has no Net equivalent");
    }

    private void DisagreeButtonPressed() {
        IZoneTransport transport = Transport;
        if (transport != null)
            transport.Disconnect();
    }
}
