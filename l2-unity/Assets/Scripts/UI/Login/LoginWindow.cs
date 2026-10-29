using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public class LoginWindow : L2Window
{
    [Inject] LoginRuntime _login;
    [Inject] IZoneTransport _zoneTransport;

    private LoginClient Login => LoginPacketServices.LoginClient;
    private VisualElement _logo;
    private TextField _userInput;
    private TextField _passwordInput;
    private Label _labelError;

    private static LoginWindow _instance;
    public static LoginWindow Instance { get { return _instance; } }

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

    protected override void LoadAssets() {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Login/LoginWindow");
    }

    protected override IEnumerator BuildWindow(VisualElement root) {
        InitWindow(root);

        yield return new WaitForEndOfFrame();

        _labelError = (Label)GetElementById("LabelError");
        Button loginButton = (Button) GetElementById("LoginButton");
        loginButton.AddManipulator(new ButtonClickSoundManipulator(loginButton));
        loginButton.RegisterCallback<ClickEvent>(evt => LoginButtonPressed());

        Button exitButton = (Button)GetElementById("ExitButton");
        exitButton.AddManipulator(new ButtonClickSoundManipulator(exitButton));
        exitButton.RegisterCallback<ClickEvent>(evt => ExitButtonPressed());

        VisualElement userInputBg = GetElementById("UserInputBg");
        _userInput = (TextField) GetElementById("UserInputField");
        _userInput.RegisterCallback<FocusEvent>((evt) => OnInputFocus(evt, _userInput));
        _userInput.RegisterCallback<BlurEvent>((evt) => OnInputBlur(evt, _userInput));
        _userInput.maxLength = 16;
        _userInput.RegisterValueChangedCallback(OnLoginInputChanged);

        _userInput.AddManipulator(new HighlightedInputFieldManipulator(_userInput, userInputBg, 20));
        _userInput.AddManipulator(new BlinkingCursorManipulator(_userInput));
        _userInput.RegisterCallback<KeyDownEvent>((evt) => OnKeyPressed(evt, _userInput));

        VisualElement passwordInputBg = GetElementById("PasswordInputBg");
        _passwordInput = (TextField)GetElementById("PasswordInputField");
        _passwordInput.RegisterCallback<FocusEvent>((evt) => OnInputFocus(evt, _passwordInput));
        _passwordInput.RegisterCallback<BlurEvent>((evt) => OnInputBlur(evt, _passwordInput));
        _passwordInput.maxLength = 16;

        _passwordInput.AddManipulator(new HighlightedInputFieldManipulator(_passwordInput, passwordInputBg, 20));
        _passwordInput.AddManipulator(new BlinkingCursorManipulator(_passwordInput));
        _passwordInput.RegisterCallback<KeyDownEvent>((evt) => OnKeyPressed(evt, _passwordInput));


        _logo = root.Q<VisualElement>("L2Logo");

        _userInput.Focus();
    }



    private void OnKeyPressed(KeyDownEvent evt, TextField input) {
        KeyCode keyCode = evt.keyCode;
        if (keyCode == KeyCode.Tab) {
            if (input == _passwordInput) {
                _userInput.Focus();
            } else {
                _passwordInput.Focus();
            }

            evt.PreventDefault();

        } else if (keyCode == KeyCode.Return || keyCode == KeyCode.KeypadEnter) {
            AudioManager.Instance.PlayUISound("click_01");
            LoginButtonPressed();
            evt.PreventDefault();
        }
    }

    private void OnLoginInputChanged(ChangeEvent<string> evt) {
        string username = evt.newValue;

        // Regex pattern: ^[a-zA-Z0-9]{1,16}$
        // This allows only alphanumeric characters, 1-16 characters long
        Regex regex = new Regex(@"^[a-zA-Z0-9]{0,16}$");

        if (!regex.IsMatch(username)) {
            // If the input doesn't match the regex, revert to the previous value
            _userInput.SetValueWithoutNotify(evt.previousValue);
        }
    }

    private void OnInputFocus(FocusEvent evt, VisualElement input) {
        if (!input.ClassListContains("highlighted")) {
            input.AddToClassList("highlighted");
        }
    }

    private void OnInputBlur(BlurEvent evt, VisualElement input) {
        if (input.ClassListContains("highlighted")) {
            input.RemoveFromClassList("highlighted");
        }
    }

    private void LoginButtonPressed() {
        LoginClient login = Login;
        string account = _userInput.value.ToLower().Trim();
        string password = _passwordInput.value.Trim();

        if (string.IsNullOrEmpty(account) && login != null)
            account = (login.Account ?? string.Empty).ToLower().Trim();
        if (string.IsNullOrEmpty(password) && login != null)
            password = (login.Password ?? string.Empty).Trim();

        if (string.IsNullOrEmpty(account) || string.IsNullOrEmpty(password))
            return;

        IZoneTransport transport = _zoneTransport ?? (App.HasContainer ? App.Resolve<IZoneTransport>() : null);
        bool bypassesInterlude = transport != null && transport.BypassesInterludeLogin;
        if (FineageWireConfig.LoginPressStartsZoneSession(bypassesInterlude))
        {
            if (login != null && login.IsConnected)
            {
                Debug.Log("[Login] closing stuck login socket before ZoneV2 handoff");
                login.Disconnect();
            }

            if (FineageWireConfig.UseLoginHandoff)
                ShowErrorText("Connecting to " + FineageWireConfig.LoginHost + ":" + FineageWireConfig.LoginPort);

            try
            {
                if (transport != null && FineageWireConfig.IsZoneV2)
                    transport.BeginSession(account, password);
                else
                    ZoneV2Client.Ensure().BeginSession(account, password);
            }
            catch (Exception ex)
            {
                Debug.LogError("[Login] BeginSession failed: " + ex.Message);
                ShowErrorText(ex.Message);
            }
            return;
        }

        if (login == null)
        {
            ShowErrorText("Login client is not ready.");
            return;
        }

        if (login.IsConnected || login.IsConnecting)
        {
            ShowErrorText("Already connected to the login server.");
            return;
        }

        login.Account = account;
        login.Password = password;
        login.Connect();
    }

    private void ExitButtonPressed() {
        Application.Quit();
    }


    public void ShowErrorText(string text)
    {
        if (_labelError != null)
            _labelError.text = text;
    }

    public override void HideWindow() {
        base.HideWindow();
        if (_logo != null)
            _logo.style.display = DisplayStyle.None;
    }

    public override void ShowWindow() {
        base.ShowWindow();
        if (_logo != null)
            _logo.style.display = DisplayStyle.Flex;
        _userInput?.Focus();
    }
}