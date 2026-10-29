using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public class LicenseWindow : L2Window
{
    [Inject] LoginRuntime _login;

    private GameManager Manager => LoginPacketServices.Manager;

    private static LicenseWindow _instance;
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
            if (Input.GetKeyDown(KeyCode.Escape)) {
                AudioManager.Instance.PlayUISound("click_01");
                DisagreeButtonPressed();
            } else if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return)) {
                AudioManager.Instance.PlayUISound("click_01");
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
        Manager?.OnLicenseAccepted();
    }

    private void DisagreeButtonPressed() {
        Manager?.LogOut();
    }
}
