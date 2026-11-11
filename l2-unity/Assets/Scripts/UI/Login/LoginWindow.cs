using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class LoginWindow : L2Window
{
    [Inject] LoginRuntime _login;
    [Inject] IZoneTransport _zoneTransport;

    private LoginClient Login => LoginPacketServices.LoginClient;
    private VisualElement _logo;
    private TextField _userInput;
    private TextField _passwordInput;
    private Toggle _rememberToggle;
    // Local playtest convenience only (PlayerPrefs on this machine).
    const string RememberKey = "fineage.login.remember";
    const string RememberAccountKey = "fineage.login.account";
    const string RememberPasswordKey = "fineage.login.password";
    private Label _labelError;
    private Button _loginButton;
    private bool _loginBusy;

    private static LoginWindow _instance;
    [System.Obsolete("Use [Inject] or LoginPacketServices.LoginWindow instead")]
    public static LoginWindow Instance { get { return _instance; } }

}
